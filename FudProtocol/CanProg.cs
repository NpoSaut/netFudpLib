using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Communications.Appi;
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
        public const int CurrentProtocolVersion = 2;
        public const int LastCompatibleProtocolVersion = 2;
        private const int ProtocolVersionKey = 195;
        private const int LastCompatibleProtocolVersionKey = 196;

        private bool DisposeFlowOnExit = false;
        public static IList<ICanProgLog> Logs { get; set; }

        public enum CheckVersionResult
        {
            /// <summary>Версии идентичны</summary>
            Equails,
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
            if (deviceCurrentProtocolVersion == CurrentProtocolVersion) return CheckVersionResult.Equails;
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
        
        const int MaxAttempts = 20;

        /// <summary>
        /// Отправляет сообщение
        /// </summary>
        /// <param name="flow">CAN порт</param>
        /// <param name="device">Класс содержащий параметры системы и блока</param>
        /// <param name="Data">Отправляемые данные</param>
        public static void SendMsg(CanFlow flow, Message msg, int TimeOut = 2000, UInt16 WithTransmitDescriptior = FuProg, UInt16 WithAcknowlegmentDescriptior = FuDev)
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
                    IsoTp.Send(flow, WithTransmitDescriptior, WithAcknowlegmentDescriptior, msg.Encode(), TimeSpan.FromMilliseconds(TimeOut));
                    break;
                }
                catch(IsoTpProtocolException istoProtocolException)
                {
                    Logs.PushFormatTextEvent("Исключение во время передачи: {0}", istoProtocolException.Message);
                    System.Threading.Thread.Sleep(1000);
                    if (attempt >= MaxAttempts-1) throw new CanProgTransportException(istoProtocolException);
                }
            }
        }

        public static AnswerType Request<AnswerType>(CanFlow flow, Message RequestMessage, int TimeOut = 2000, UInt16 ThisSideDescriptior = FuProg, UInt16 TheirSideDescriptior = FuDev)
            where AnswerType : Message
        {
            Exception LastException = null;
            for (int attempt = 0; attempt < MaxAttempts; attempt++)
            {
                try
                {
                    SendMsg(flow, RequestMessage, TimeOut, ThisSideDescriptior, TheirSideDescriptior);
                    return GetMsg<AnswerType>(flow, TimeOut, TheirSideDescriptior, ThisSideDescriptior);
                }
                catch (IsoTpProtocolException ex) { LastException = ex; }
                catch (FudpReceiveTimeoutException ex) { LastException = ex; }
                System.Threading.Thread.Sleep(1000);
            }
            Logs.PushFormatTextEvent("Исключение во время передачи: {0}", LastException);
            throw new CanProgTransportException(LastException);
        }

        /// <summary>
        /// Получает ответ от устройства
        /// </summary>
        /// <param name="flow">CAN порт</param>
        /// <param name="device">Класс содержащий параметры системы и блока</param>
        /// <returns>Принятые данные</returns>
        public static Message GetMsg(CanFlow flow, int TimeOut = 2000, UInt16 WithTransmitDescriptior = FuDev, UInt16 WithAcknowlegmentDescriptior = FuProg)
        {
            var tr = IsoTp.Receive(flow, WithTransmitDescriptior, WithAcknowlegmentDescriptior, TimeSpan.FromMilliseconds(TimeOut));
            return Message.DecodeMessage(tr.Data);
        }
        public static MT GetMsg<MT>(CanFlow flow, int TimeOut = 2000, UInt16 WithTransmitDescriptior = FuDev, UInt16 WithAcknowlegmentDescriptior = FuProg)
            where MT : Message
        {
            DateTime startDt = DateTime.Now;
            while (startDt.AddMilliseconds(TimeOut) >= DateTime.Now)
            {
                try
                {
                    var tr = IsoTp.Receive(flow, WithTransmitDescriptior, WithAcknowlegmentDescriptior,
                        TimeSpan.FromMilliseconds(TimeOut));
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
                        Console.WriteLine(
                            "#CanProg: был запрос на чтение сообщения {0}, вместо этого было прочитано сообщение {1} {{{2}}}",
                            typeof (MT).Name, mes.GetType().Name, mes);
                    }
                }
                catch (IsoTpProtocolException timeoutException) { }
                //{ throw new FudpReceiveTimeoutException(string.Format("Превышено врем ожидания FUDP-сообщения (ожидали сообщения {0})", typeof(MT)), timeoutException); }
            }
            throw new FudpReceiveTimeoutException(string.Format("Превышено врем ожидания FUDP-сообщения (ожидали сообщения {0})", typeof(MT)));
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
            CanProg res = new CanProg(Flow) { Device = device };
            int i = 0;
            while(true)
            {
                if (i == 10)
                {
                    throw new CanProgLimitConnectException("Превышен лимит попыток подключения");
                }
                Flow.Clear();
                SendMsg(Flow, new ProgInit(device), 100, WithTransmitDescriptior: FuInit);
                i++;
                try
                {
                    var xxx = GetMsg<ProgStatus>(res.Flow, 1000);
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

            ProgCreateAck CreateAck = Request<ProgCreateAck>(Flow, Create);
            if (CreateAck.ErrorCode != 0)
                switch (CreateAck.ErrorCode)
                {
                    case 1: throw new CanProgFileAlreadyExistsException(CreateAck.ErrorMsg[CreateAck.ErrorCode]);
                    case 2: throw new CanProgMaximumFilesCountAchivedException(CreateAck.ErrorMsg[CreateAck.ErrorCode]);
                    case 3: throw new CanProgMemoryIsOutException(CreateAck.ErrorMsg[CreateAck.ErrorCode]);
                    case 4: throw new CanProgCreateException(CreateAck.ErrorMsg[CreateAck.ErrorCode]);
                    default: throw new CanProgException();
                }

            int pointer = 0;
            while (pointer < fileInfo.FileSize)
            {
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
        /// <summary>
        /// Разрыв соединения
        /// </summary>
        public void Dispose()
        {
            Submit();
            if (DisposeFlowOnExit) Flow.Dispose();
        }

        /// <summary>
        /// Отправляет запрос на завершение сеанса программирования
        /// </summary>
        private void Submit()
        {
            Request<ProgSubmitAck>(Flow, new ProgSubmit());
        }
    }
}
