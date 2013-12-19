using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using Communications.Can;
using Communications.Protocols.IsoTP;
using Communications.Protocols.IsoTP.Exceptions;
using Fudp.Messages;
using Fudp.Exceptions;

namespace Fudp
{
    
    /// <summary>
    /// Класс компанует, отправляет сообщение и получает ответ
    /// </summary>
    public class CanProg : IDisposable
    {
        public const int CurrentProtocolVersion = 4;
        public const int LastCompatibleProtocolVersion = 4;
        private const int ProtocolVersionKey = 195;
        private const int LastCompatibleProtocolVersionKey = 196;

        private bool _disposeFlowOnExit = false;
        public static IList<ICanProgLog> Logs { get; set; }

        public enum CheckVersionResult
        {
            /// <summary>Версии идентичны</summary>
            Equals,
            /// <summary>Версии совместимы</summary>
            Compatible,
            /// <summary>Версии не совместимы</summary>
            UnCompatible
        }
        /// <summary>
        /// Проверяет совместимость версии загрузчика с версией программатора
        /// </summary>
        /// <returns></returns>
        public CheckVersionResult CheckProtocolVersion()
        {
            int deviceCurrentProtocolVersion = Properties.ContainsKey(ProtocolVersionKey) ? Properties[ProtocolVersionKey] : 1;
            int deviceLastCompatibleProtocolVersion = Properties.ContainsKey(LastCompatibleProtocolVersionKey) ? Properties[LastCompatibleProtocolVersionKey] : 1;

            // Версии протокола идентичны
            if (deviceCurrentProtocolVersion == CurrentProtocolVersion) return CheckVersionResult.Equals;
            // Версия программатора устарела, но является совместимой с версией загрузчика
            else if (CurrentProtocolVersion < deviceCurrentProtocolVersion && CurrentProtocolVersion >= deviceLastCompatibleProtocolVersion) return CheckVersionResult.Compatible;
            // Версия загрузчика устарела, но является совместимой с версией программатора
            else if (CurrentProtocolVersion > deviceCurrentProtocolVersion && deviceCurrentProtocolVersion >= LastCompatibleProtocolVersion) return CheckVersionResult.Compatible;
            // Версии не совместимы
            else return CheckVersionResult.UnCompatible;
        }

        public CanProg(CanFlow Flow)
        {
            this.Flow = Flow;
            Properties = new Dictionary<int, int>();
            SubmitAction = SubmitStatus.Cancel;
        }

        public const UInt16 FuInit = 0x66a8;
        public const UInt16 FuProg = 0x66c8;
        public const UInt16 FuDev =  0x66e8;
        /// <summary>
        /// Словарь свойств файлов
        /// </summary>
        public Dictionary<int, int> Properties { get; private set; }
        /// <summary>
        /// Порт
        /// </summary>
        public CanFlow Flow { get; private set; }

        public DeviceTicket Device { get; private set; }

        const int DefaultMaximumSendAttempts = 7;
        private const int DefaultIsoTpTimeoutMs = 300;

        /// <summary>
        /// Отправляет сообщение
        /// </summary>
        /// <param name="flow">CAN-поток</param>
        /// <param name="msg">Отправляемое сообщение</param>
        /// <param name="TimeOut">Таймаут на ожидание ответа</param>
        /// <param name="WithTransmitDescriptor">Дескриптор, с которым передаётся сообщение</param>
        /// <param name="WithAcknowledgmentDescriptor">Дескриптор, с которым передаются подтверждения на сообщение</param>
        public static void SendMsg(CanFlow flow, Message msg, int TimeOut = DefaultIsoTpTimeoutMs, UInt16 WithTransmitDescriptor = FuProg, UInt16 WithAcknowledgmentDescriptor = FuDev, int MaxAttempts = DefaultMaximumSendAttempts)
        {
            flow.Clear();
            for (int attempt = 0; attempt < MaxAttempts; attempt ++)
            {
#if DEBUG
                Logs.PushFormatTextEvent("--> {0}", msg);
#endif
                try
                {
                    flow.Clear();
                    IsoTp.Send(flow, WithTransmitDescriptor, WithAcknowledgmentDescriptor, msg.Encode(), TimeSpan.FromMilliseconds(TimeOut));
                    break;
                }
                catch(IsoTpProtocolException istoProtocolException)
                {
                    Logs.PushFormatTextEvent("Исключение во время передачи: {0}", istoProtocolException.Message);
                    System.Threading.Thread.Sleep(200);
                    if (attempt >= MaxAttempts-1) throw new CanProgTransportException(istoProtocolException);
                }
            }
        }

        public static AnswerType Request<AnswerType>(CanFlow flow, Message RequestMessage, int TimeOut = DefaultIsoTpTimeoutMs, UInt16 ThisSideDescriptor = FuProg, UInt16 TheirSideDescriptor = FuDev, int MaxAttempts = DefaultMaximumSendAttempts)
            where AnswerType : Message
        {
            Exception LastException = null;
            for (int attempt = 0; attempt < MaxAttempts; attempt++)
            {
                try
                {
                    SendMsg(flow, RequestMessage, TimeOut, ThisSideDescriptor, TheirSideDescriptor, MaxAttempts);
                    return GetMsg<AnswerType>(flow, TimeOut, TheirSideDescriptor, ThisSideDescriptor);
                }
                catch (IsoTpProtocolException ex) { LastException = ex; }
                catch (FudpReceiveTimeoutException ex) { LastException = ex; }
                System.Threading.Thread.Sleep(200);
            }
            Logs.PushFormatTextEvent("Исключение во время передачи: {0}", LastException);
            throw new CanProgTransportException(LastException);
        }

        public static Message GetMsg(CanFlow flow, int TimeOut = DefaultIsoTpTimeoutMs, UInt16 WithTransmitDescriptor = FuDev, UInt16 WithAcknowledgmentDescriptor = FuProg)
        {
            var tr = IsoTp.Receive(flow, WithTransmitDescriptor, WithAcknowledgmentDescriptor, TimeSpan.FromMilliseconds(TimeOut));
            return Message.DecodeMessage(tr.Data);
        }
        public static MT GetMsg<MT>(CanFlow flow, int TimeOut = DefaultIsoTpTimeoutMs, UInt16 WithTransmitDescriptor = FuDev, UInt16 WithAcknowledgmentDescriptor = FuProg)
            where MT : Message
        {
            var sw = new Stopwatch();
            sw.Start();
            while (sw.ElapsedMilliseconds < TimeOut)
            {
                try
                {
                    var tr = IsoTp.Receive(flow, WithTransmitDescriptor, WithAcknowledgmentDescriptor,
                        TimeSpan.FromMilliseconds(TimeOut - sw.ElapsedMilliseconds));
                    var mes = Message.DecodeMessage(tr.Data);
                    var typedMes = mes as MT;
                    if (typedMes != null)
                    {
#if DEBUG
                        Logs.PushFormatTextEvent("<-- {0}", typedMes);
#endif
                        return typedMes;
                    }
                    else
                    {
#if DEBUG
                        Logs.PushFormatTextEvent("<-- {0} - игнорируем (ожидали {1})", mes, typeof (MT));
#endif
                    }
                }
                catch (IsoTpProtocolException) { }
                //{ throw new FudpReceiveTimeoutException(string.Format("Превышено врем ожидания FUDP-сообщения (ожидали сообщения {0})", typeof(MT)), timeoutException); }
            }
            throw new FudpReceiveTimeoutException(string.Format("Превышено врем ожидания FUDP-сообщения (ожидали сообщения {0})", typeof(MT)));
        }
        /// <summary>
        /// Устанавливает соединение
        /// </summary>
        /// <param name="Port">Can-порт</param>
        /// <param name="device">Класс содержащий параметры системы и блока</param>
        /// <returns></returns>
        public static CanProg Connect(CanPort Port, DeviceTicket device)
        {
            var session = Connect(new CanFlow(Port, FuDev, FuInit, FuProg), device);
            session._disposeFlowOnExit = true;
            return session;
        }
        /// <summary>
        /// Устанавливает соединение
        /// </summary>
        /// <param name="Flow">Пото Can-сообщений</param>
        /// <param name="device">Класс содержащий параметры системы и блока</param>
        /// <returns></returns>
        public static CanProg Connect(CanFlow Flow, DeviceTicket device)
        {
            Logs.PushFormatTextEvent("Пробуем подключиться к {0}", device);
            var res = new CanProg(Flow) { Device = device };
            int i = 0;
            while(true)
            {
                Logs.PushFormatTextEvent("Попытка {0}", i+1);
                if (i >= 10) throw new CanProgLimitConnectException("Превышен лимит попыток подключения");
                
                Flow.Clear();

                Logs.PushFormatTextEvent("Отправляем ProgInit");
                SendMsg(Flow, new ProgInit(device), 100, WithTransmitDescriptor: FuInit);
                i++;
                try
                {
                    Logs.PushFormatTextEvent("Ждём ответа на ProgInit");
                    var xxx = GetMsg<ProgStatus>(res.Flow, 100);
                    Logs.PushFormatTextEvent("Получили ответ на ProgInit");
                    res.Properties = xxx.Properties;
                    break;
                }
                catch (IsoTpProtocolException) { }
                catch (FudpReceiveTimeoutException) { }
            }

            // Проверка версии
            if (res.CheckProtocolVersion() == CheckVersionResult.UnCompatible)
                throw new CanProgUnCompatibleVersionException();

            return res;
        }

        /// <summary>
        /// Запрос списка файлов
        /// </summary>
        /// <returns></returns>
        public List<DevFileInfo> ListFiles()
        {
            ProgListRq ListRq = new ProgListRq();
            return Request<ProgList>(Flow, ListRq).ListDevFileInfo;
        }
        /// <summary>
        /// Запрос на чтение
        /// </summary>
        public Byte[] ReadFile(DevFileInfo fileInfo)
        {
            
            Byte[] buff = new Byte[fileInfo.FileSize + 1];
            ProgReadRq ReadRq = new ProgReadRq()
            {
                FileName = fileInfo.FileName,
                FileSize = fileInfo.FileSize,
                Offset = 0,
            };

            while (ReadRq.FileSize > 0)
            {
                Request<ProgRead>(Flow, ReadRq);
                
                ProgRead Read = (ProgRead)GetMsg(Flow);
                if (Read.ErrorCode != 0)
                    switch (Read.ErrorCode)
                    {
                        case 1: throw new FileNotFoundException(Read.ErrorMsg[Read.ErrorCode]);
                        case 2: throw new IndexOutOfRangeException(Read.ErrorMsg[Read.ErrorCode]);
                        case 3: throw new CanProgReadException(Read.ErrorMsg[Read.ErrorCode]);
                        default: throw new CanProgException();
                    }
                Buffer.BlockCopy(Read.Buff, 0, buff, ReadRq.Offset, Read.Buff.Length);
                ReadRq.Offset += Read.Buff.Length;
            }
            return buff;
        }
        /// <summary>
        /// Команда на удаление
        /// </summary>
        /// <param name="fileName">Имя фала</param>
        public int DeleteFile(String FileName)
        {
            ProgRm Rm = new ProgRm()
            {
                FileName = FileName
            };
            return Request<ProgRmAck>(Flow, Rm).ErrorCode;
        }
        /// <summary>
        /// Команда на очистку памяти
        /// </summary>
        public void Erase()
        {
            ProgMrPropper MrPropper = new ProgMrPropper();
            SendMsg(Flow, MrPropper);
        }

        /// <summary>
        /// Команда на создание файла
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <param name="ProgressAcceptor"></param>
        /// <param name="CancelToken"></param>
        /// <param name="fileName">Имя файла</param>
        /// <param name="Data">Данные</param>
        /// <returns></returns>
        public void CreateFile(DevFileInfo fileInfo, IProgressAcceptor ProgressAcceptor = null, CancellationToken CancelToken = default(CancellationToken))
        {
            var create = new ProgCreate()
            {
                FileName = fileInfo.FileName,
                FileSize = fileInfo.FileSize,
                CRC = FudpCrc.CalcCrc(fileInfo.Data)
            };

            var createAck = Request<ProgCreateAck>(Flow, create);
            if (createAck.ErrorCode != 0)
                switch (createAck.ErrorCode)
                {
                    case 1: throw new CanProgFileAlreadyExistsException(createAck.ErrorMsg[createAck.ErrorCode]);
                    case 2: throw new CanProgMaximumFilesCountAchivedException(createAck.ErrorMsg[createAck.ErrorCode]);
                    case 3: throw new CanProgMemoryIsOutException(createAck.ErrorMsg[createAck.ErrorCode]);
                    case 4: throw new CanProgCreateException(createAck.ErrorMsg[createAck.ErrorCode]);
                    default: throw new CanProgException();
                }

            int pointer = 0;
            while (pointer < fileInfo.FileSize)
            {
                CancelToken.ThrowIfCancellationRequested();
                pointer += Write(fileInfo, pointer);

                if (ProgressAcceptor != null) ProgressAcceptor.OnProgressChanged(Math.Min(1, ((double)pointer / fileInfo.FileSize)));
            }
        }

        private int Write(DevFileInfo fileInfo, int offset)
        {
            ProgWrite WriteMessage = new ProgWrite(fileInfo, offset);

            var result = Request<ProgWriteAck>(Flow, WriteMessage);
            if (result.Status != ProgWriteAck.WriteStatusKind.OK)
                throw new CanProgWriteException(result.Status);

            return WriteMessage.Data.Length;
        }
        /// <summary>
        /// Команда на содание или изменение записи в словаре свойств
        /// </summary>
        /// <param name="paramKey">Ключ</param>
        /// <param name="paramValue">Значение свойства</param>
        public void SetParam(byte paramKey, int paramValue)
        {
            ParamSetRq psr = new ParamSetRq()
            {
                ParamKey = paramKey,
                ParamVlue = paramValue
            };
            ParamSetAck psa = Request<ParamSetAck>(Flow, psr);
            if (psa.ErrorCode != 0)
                throw new CanProgCreateException(psa.ErrorMsg[psa.ErrorCode]);
        }
        /// <summary>
        /// Удаление записи из словаря свойств
        /// </summary>
        /// <param name="paramKey">Ключ</param>
        public void DeleteProperty(byte paramKey)
        {
            ParamRmRq prr = new ParamRmRq()
            {
                ParamKey = paramKey
            };
            ParamRmAck pra = Request<ParamRmAck>(Flow, prr);
            if (pra.ErrorCode == 0)
                Console.WriteLine(pra.ErrorMsg[pra.ErrorCode]);
            else
                throw new CanProgCreateException(pra.ErrorMsg[pra.ErrorCode]);
        }

        public SubmitStatus SubmitAction { get; set; }
        private bool _submited = false;

        /// <summary>
        /// Разрыв соединения
        /// </summary>
        public void Dispose()
        {
            if (!_submited)
                try
                {
                    // TODO: Убедиться в безопасности Dispose()
                    // Убедиться в том, что если Dispose() возникает из-за отключения АППИ, мы не зависнем на этом месте из-за того,
                    // что будем долго пытаться отправить Submit();
                    Submit(SubmitAction);
                }
                // ReSharper disable once EmptyGeneralCatchClause
                catch { }
            if (_disposeFlowOnExit) Flow.Dispose();
        }

        /// <summary>
        /// Отправляет запрос на завершение сеанса программирования
        /// </summary>
        public void Submit(SubmitStatus Status)
        {
            var submitMessage = new ProgSubmit(Status);
            int sendAttempts = Status == SubmitStatus.Submit ? DefaultMaximumSendAttempts : 3;
            Request<ProgSubmitAck>(Flow, submitMessage);
            _submited = true;
        }
    }
}
