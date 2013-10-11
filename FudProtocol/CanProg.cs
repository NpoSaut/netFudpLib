﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Communications.Appi;
using Communications.Can;
using Communications.Protocols.IsoTP;
using Fudp.Messages;
using Fudp.Exceptions;

namespace Fudp
{
    
    /// <summary>
    /// Класс компанует, отправляет сообщение и получает ответ
    /// </summary>
    public class CanProg : IDisposable
    {
        private bool DisposeFlowOnExit = false;
        public static IList<ICanProgLog> Logs { get; set; }

        public CanProg(CanFlow Flow)
        {
            this.Flow = Flow;
            Properties = new Dictionary<int, int>();
        }

        public const UInt16 FuInit = 0xfc08;
        public const UInt16 FuProg = 0xfc28;
        public const UInt16 FuDev =  0xfc48;
        /// <summary>
        /// Словарь свойств файлов
        /// </summary>
        public Dictionary<int, int> Properties { get; private set; }
        /// <summary>
        /// Порт
        /// </summary>
        public CanFlow Flow { get; private set; }

        public DeviceTicket Device { get; private set; }
        /// <summary>
        /// Отправляет сообщение
        /// </summary>
        /// <param name="flow">CAN порт</param>
        /// <param name="device">Класс содержащий параметры системы и блока</param>
        /// <param name="Data">Отправляемые данные</param>
        public static void SendMsg(CanFlow flow, Message msg, int TimeOut = 2000, UInt16 WithTransmitDescriptior = FuProg, UInt16 WithAcknowlegmentDescriptior = FuDev)
        {
#if DEBUG
            Logs.PushFormatTextEvent("--> Отправляем {0}", msg);
#endif
            flow.Clear();
            IsoTp.Send(flow, WithTransmitDescriptior, WithAcknowlegmentDescriptior, msg.Encode(), TimeSpan.FromMilliseconds(TimeOut));
        }
        /// <summary>
        /// Получает ответ от устройства
        /// </summary>
        /// <param name="flow">CAN порт</param>
        /// <param name="device">Класс содержащий параметры системы и блока</param>
        /// <returns>Принятые данные</returns>
        public static Message GetMsg(CanFlow flow, DeviceTicket device, int TimeOut = 2000, UInt16 WithTransmitDescriptior = FuDev, UInt16 WithAcknowlegmentDescriptior = FuProg)
        {
            var tr = IsoTp.Receive(flow, WithTransmitDescriptior, WithAcknowlegmentDescriptior, TimeSpan.FromMilliseconds(TimeOut));
            return Message.DecodeMessage(tr.Data);
        }
        public static MT GetMsg<MT>(CanFlow flow, DeviceTicket device, int TimeOut = 2000, UInt16 WithTransmitDescriptior = FuDev, UInt16 WithAcknowlegmentDescriptior = FuProg)
            where MT : Message
        {
#if DEBUG
            Logs.PushFormatTextEvent("<-- Ожидаем сообщения {0}", typeof(MT));
#endif
            DateTime startDt = DateTime.Now;
            while (startDt.AddMilliseconds(TimeOut) >= DateTime.Now)
            {
                var tr = IsoTp.Receive(flow, WithTransmitDescriptior, WithAcknowlegmentDescriptior, TimeSpan.FromMilliseconds(TimeOut));
                var mes = Message.DecodeMessage(tr.Data);
                var Tmes = mes as MT;
                if (Tmes != null) return Tmes;
                else Console.WriteLine("#CanProg: был запрос на чтение сообщения {0}, вместо этого было прочитано сообщение {1} {{{2}}}", typeof(MT).Name, mes.GetType().Name, mes.ToString());
            }
            throw new TimeoutException("Не получено требуемого сообщения за указанное время");
        }
        /// <summary>
        /// Устанавливает соединение
        /// </summary>
        /// <param name="Flow">Can-порт</param>
        /// <param name="device">Класс содержащий параметры системы и блока</param>
        /// <returns></returns>
        public static CanProg Connect(CanPort Port, DeviceTicket device)
        {
            var session = Connect(new CanFlow(Port, FuDev, FuInit, FuProg), device);
            session.DisposeFlowOnExit = true;
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
            CanProg CP = new CanProg(Flow)
            {
                Device = device
            };
            ProgInit Init = new ProgInit(device);
            int i = 0;
            while(true)
            {
                
                if (i == 10)
                {
                    throw new CanProgLimitConnectException("Превышен лимит попыток подключения");
                }
                Flow.Clear();
                SendMsg(Flow, Init, 100, WithTransmitDescriptior: FuInit);
                i++;
                try
                {
                    var xxx = GetMsg<ProgStatus>(CP.Flow, CP.Device, 1000);
                    CP.Properties = xxx.Properties;
                    //CP.Properties = GetMsg<ProgStatus>(CP.Flow, CP.Device, 100).Properties;
                    break;
                }
                catch (TimeoutException)
                {
                }
            }
            return CP;
        }
        /// <summary>
        /// Запрос списка файлов
        /// </summary>
        /// <returns></returns>
        public List<DevFileInfo> ListFiles()
        {
            ProgListRq ListRq = new ProgListRq();
            SendMsg(Flow, ListRq);
            return GetMsg<ProgList>(Flow, Device).ListDevFileInfo;
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
                SendMsg(Flow, ReadRq);
                
                ProgRead Read = (ProgRead)GetMsg(Flow, Device);
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
            SendMsg(Flow, Rm);

            return GetMsg<ProgRmAck>(Flow, Device).ErrorCode;
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
        /// <param name="fileName">Имя файла</param>
        /// <param name="Data">Данные</param>
        /// <returns></returns>
        public void CreateFile(DevFileInfo fileInfo, IProgressAcceptor ProgressAcceptor = null)
        {
            ProgCreate Create = new ProgCreate()
            {
                FileName = fileInfo.FileName,
                FileSize = fileInfo.FileSize,
                CRC = FudpCrc.CalcCrc(fileInfo.Data)
            };

            SendMsg(Flow, Create);
            ProgCreateAck CreateAck = GetMsg<ProgCreateAck>(Flow, Device);
            if (CreateAck.ErrorCode != 0)
                switch (CreateAck.ErrorCode)
                {
                    case 1: throw new CanProgFileAlreadyExistsException(CreateAck.ErrorMsg[CreateAck.ErrorCode]);
                    case 2: throw new CanProgMaximumFilesCountAchivedException(CreateAck.ErrorMsg[CreateAck.ErrorCode]);
                    case 3: throw new CanProgMemoryIsOutException(CreateAck.ErrorMsg[CreateAck.ErrorCode]);
                    case 4: throw new CanProgCreateException(CreateAck.ErrorMsg[CreateAck.ErrorCode]);
                    default: throw new CanProgException();
                }
            Write(fileInfo);
        }

        private void Write(DevFileInfo fileInfo, int offset = 0, IProgressAcceptor ProgressAcceptor = null)
        {
            ProgressAcceptor.OnProgressChanged(Math.Min(1, (double)offset / fileInfo.FileSize));
            if (fileInfo.FileSize - offset > 0)
            {
                ProgWrite PWrite = new ProgWrite()
                {
                    FileName = fileInfo.FileName,
                    WBuff = fileInfo.Data,
                    BuffSize = fileInfo.FileSize,
                    Offset = offset
                };

                SendMsg(Flow, PWrite);

                Write(
                    fileInfo,
                    offset: PWrite.BuffSize >= ProgWrite.DataSize + PWrite.OverheadsBytes ?
                            offset += PWrite.BuffSize - PWrite.OverheadsBytes : offset += PWrite.BuffSize,
                    ProgressAcceptor: ProgressAcceptor
                    );
            }
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
            SendMsg(Flow, psr);
            ParamSetAck psa = GetMsg<ParamSetAck>(Flow, Device);
            if (psa.ErrorCode == 0)
                Console.WriteLine(psa.ErrorMsg[psa.ErrorCode]);
            else
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
            SendMsg(Flow, prr);
            ParamRmAck pra = GetMsg<ParamRmAck>(Flow, Device);
            if (pra.ErrorCode == 0)
                Console.WriteLine(pra.ErrorMsg[pra.ErrorCode]);
            else
                throw new CanProgCreateException(pra.ErrorMsg[pra.ErrorCode]);
        }
        /// <summary>
        /// Разрыв соединения
        /// </summary>
        public void Dispose()
        {
            SendMsg(Flow, new ProgSubmit());
            if (DisposeFlowOnExit) Flow.Dispose();
        }
    }
}
