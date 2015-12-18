﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using Fudp.Exceptions;
using Fudp.Messages;

namespace Fudp
{
    public class FudpProgSession : IProgSession
    {
        private readonly IDisposable _pinger;
        private readonly IFudpPort _port;
        private readonly IPropertiesManager _propertiesManager;
        private readonly TimeSpan _timeout = TimeSpan.FromMilliseconds(7000);
        private bool _submited;

        public FudpProgSession(IFudpPort Port, IPropertiesManager PropertiesManager, DeviceTicket Device)
        {
            _port = Port;
            _propertiesManager = PropertiesManager;
            this.Device = Device;

            // TODO: Нужно сделать так, чтобы PING не шёл постоянно
            _pinger = Observable.Interval(TimeSpan.FromMilliseconds(1000))
                                .Select((x, i) => (Byte)i)
                                .Subscribe(Ping);
        }

        public DeviceTicket Device { get; private set; }

        public void Dispose()
        {
            _pinger.Dispose();
            //if (!_submited)
            //{
            //    try
            //    {
            //        // TODO: Убедиться в безопасности Dispose()
            //        // Убедиться в том, что если Dispose() возникает из-за отключения АППИ, мы не зависнем на этом месте из-за того,
            //        // что будем долго пытаться отправить Submit();
            //        Submit(SubmitStatus.Cancel, CancellationToken.None);
            //    }
            //        // ReSharper disable once EmptyGeneralCatchClause
            //    catch { }
            //}
        }

        public IList<DevFileInfo> ListFiles(CancellationToken CancellationToken) { return RequestFiles(CancellationToken).ToList(); }

        public void DeleteFile(string FileName, CancellationToken CancellationToken)
        {
            var removeRequest = new ProgRm(FileName);
            ProgRmAck removeResponse = _port.FudpRequest(removeRequest, _timeout, CancellationToken);
            if (removeResponse.ErrorCode != 0)
                throw new CanProgDeleteException(removeResponse.ErrorCode);
            OnFileRemoved(FileName);
        }

        /// <summary>Команда на создание или изменение записи в словаре свойств</summary>
        /// <param name="paramKey">Ключ</param>
        /// <param name="paramValue">Значение свойства</param>
        /// <param name="CancellationToken">Токен отмены</param>
        public void SetProperty(byte paramKey, int paramValue, CancellationToken CancellationToken)
        {
            ParamSetAck psa = _port.FudpRequest(new ParamSetRq(paramKey, paramValue),
                                                _timeout, CancellationToken);

            if (psa.ErrorCode != 0)
                throw new CanProgCreateException(psa.ErrorMessage);
            _propertiesManager[paramKey] = paramValue;
        }

        /// <summary>Удаление записи из словаря свойств</summary>
        /// <param name="paramKey">Ключ</param>
        /// <param name="CancellationToken">Токен отмены</param>
        public void DeleteProperty(byte paramKey, CancellationToken CancellationToken)
        {
            ParamRmAck pra = _port.FudpRequest(new ParamRmRq(paramKey), _timeout, CancellationToken);
            switch (pra.ErrorCode)
            {
                case 0:
                    break;
                default:
                    throw new CanProgCreateException(pra.ErrorMessage);
            }
        }

        /// <summary>Отправляет запрос на завершение сеанса программирования</summary>
        public SubmitAckStatus Submit(SubmitStatus Status, CancellationToken CancellationToken)
        {
            var submitMessage = new ProgSubmit(Status);
            ProgSubmitAck submitAnswer = _port.FudpRequest(submitMessage,
                                                           Status == SubmitStatus.Submit ? TimeSpan.FromSeconds(2) : _timeout, CancellationToken);
            SubmitAckStatus status = submitAnswer.Status;
            Console.WriteLine("SUBMIT STATUS: {0}", status);
            _submited = true;
            return status;
        }

        public Byte[] ReadFile(DevFileInfo File, IProgressAcceptor ProgressAcceptor, CancellationToken CancellationToken)
        {
            var buff = new Byte[File.Size];

            int pointer = 0;

            int maximumReadSize = _port.Options.LowerLayerFrameCapacity - ProgReadRq.GetHeaderLength(File.FileName);

            if (ProgressAcceptor != null) ProgressAcceptor.OnProgressChanged(0);
            while (pointer < buff.Length)
            {
                CancellationToken.ThrowIfCancellationRequested();

                var request = new ProgReadRq(File.FileName, pointer, Math.Min(File.Size - pointer, maximumReadSize));
                ProgRead response = _port.FudpRequest(request, _timeout, CancellationToken);

                if (response.ErrorCode == 0)
                {
                    Buffer.BlockCopy(response.ReadData, 0, buff, pointer, response.ReadData.Length);
                    pointer += response.ReadData.Length;
                }
                else
                {
                    switch (response.ErrorCode)
                    {
                        case 1:
                            throw new FileNotFoundException(response.ErrorMessage);
                        case 2:
                            throw new IndexOutOfRangeException(response.ErrorMessage);
                        case 3:
                            throw new CanProgReadException(response.ErrorMessage);
                        default:
                            throw new CanProgException();
                    }
                }

                if (ProgressAcceptor != null) ProgressAcceptor.OnProgressChanged(Math.Min(1, ((double)pointer / File.Size)));
            }

            return buff;
        }

        /// <summary>Команда на создание файла</summary>
        /// <param name="File">Информация о создаваемом файле</param>
        /// <param name="CancellationToken">Токен отмены</param>
        /// <param name="ProgressAcceptor">Приёмник прогресса выполнения файла</param>
        /// <returns></returns>
        public void CreateFile(DevFile File, CancellationToken CancellationToken, IProgressAcceptor ProgressAcceptor = null)
        {
            ProgCreateAck createAck = _port.FudpRequest(new ProgCreate(File), _timeout, CancellationToken);
            if (createAck.ErrorCode != 0)
            {
                switch (createAck.ErrorCode)
                {
                    case 1:
                        throw new CanProgFileAlreadyExistsException(ProgCreateAck.ErrorMsg[createAck.ErrorCode]);
                    case 2:
                        throw new CanProgMaximumFilesCountAchivedException(ProgCreateAck.ErrorMsg[createAck.ErrorCode]);
                    case 3:
                        throw new CanProgMemoryIsOutException(ProgCreateAck.ErrorMsg[createAck.ErrorCode]);
                    case 4:
                        throw new CanProgCreateException(ProgCreateAck.ErrorMsg[createAck.ErrorCode]);
                    default:
                        throw new CanProgException();
                }
            }

            int pointer = 0;
            while (pointer < File.Size)
            {
                CancellationToken.ThrowIfCancellationRequested();
                pointer += Write(File, File.Data, pointer, CancellationToken);

                if (ProgressAcceptor != null) ProgressAcceptor.OnProgressChanged(Math.Min(1, ((double)pointer / File.Size)));
            }

            OnFileCreated(File);
        }

        private IEnumerable<DevFileInfo> RequestFiles(CancellationToken CancellationToken, int Offset = 0)
        {
            int counter = 0;
            foreach (DevFileListNode file in _port.FudpRequest(new ProgListRq((ushort)Offset), _timeout, CancellationToken).Files)
            {
                if (file is DevFileInfo)
                {
                    counter++;
                    yield return (DevFileInfo)file;
                }
                else
                {
                    IEnumerable<DevFileInfo> appendix = RequestFiles(CancellationToken, Offset + counter);
                    foreach (DevFileInfo subfile in appendix)
                        yield return subfile;
                }
            }
        }

        private void Ping(byte i)
        {
            //_port.FudpRequest(new ProgPing(i), _timeout);
        }

        private int Write(DevFileInfo fileInfo, byte[] Data, int DataOffset, CancellationToken CancellationToken)
        {
            var request = new ProgWrite(fileInfo.FileName, Data, DataOffset, DataOffset);
            ProgWriteAck result = _port.FudpRequest(request,
                                                    _timeout, CancellationToken);

            if (result.Status != ProgWriteAck.WriteStatusKind.OK)
                throw new CanProgWriteException(result.Status);

            return request.Data.Length;
        }

        private void OnFileCreated(DevFileInfo FileInfo) { }
        private void OnFileRemoved(string FileName) { }
    }
}
