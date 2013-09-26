using System;
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
    public class CanProg
    {
        public CanProg(CanPort Port)
        {
            this.Port = Port;
            Properties = new Dictionary<int, int>();
        }

        public const int FuInit = 0xfc28;
        public const int FuProg = 0xfc48;
        public const int FuDev = 0xfc68;
        /// <summary>
        /// Словарь свойств файлов
        /// </summary>
        public Dictionary<int, int> Properties { get; private set; }
        /// <summary>
        /// Порт
        /// </summary>
        public CanPort Port { get; private set; }

        public DeviceTicket Device { get; private set; }
        /// <summary>
        /// Дескриптор
        /// </summary>
        private static int transmitDescriptior;
        /// <summary>
        /// Дескриптор
        /// </summary>
        private static int acknowlegmentDescriptior;
        /// <summary>
        /// Отправляет сообщение
        /// </summary>
        /// <param name="d">CAN порт</param>
        /// <param name="device">Класс содержащий параметры системы и блока</param>
        /// <param name="Data">Отправляемые данные</param>
        public static void SendMsg(CanPort d, Message msg, int TimeOut = 2000)
        {
            IsoTp.BeginSend(d, transmitDescriptior, acknowlegmentDescriptior, msg.Encode(), TimeSpan.FromMilliseconds(TimeOut)).Wait();
        }
        /// <summary>
        /// Получает ответ от устройства
        /// </summary>
        /// <param name="port">CAN порт</param>
        /// <param name="device">Класс содержащий параметры системы и блока</param>
        /// <returns>Принятые данные</returns>
        public static Byte[] GetMsg(CanPort port, DeviceTicket device, int TimeOut = 2000)
        {
            try
            {
                TpReceiveTransaction rt = new TpReceiveTransaction(port, acknowlegmentDescriptior, transmitDescriptior);
                rt.Timeout = TimeSpan.FromMilliseconds(TimeOut);
                return rt.Receive();
            }
            catch
            {
                if (TimeOut == 2000)
                {
                    //b = false;
                    throw;
                }
                return null;
            }
        }
        /// <summary>
        /// Устанавливает соединение
        /// </summary>
        /// <param name="Port">Номер порта</param>
        /// <param name="device">Класс содержащий параметры системы и блока</param>
        /// <returns></returns>
        public static CanProg Connect(CanPort Port, DeviceTicket device, int TransmitDescriptor, int AcknowlegmentDescriptor)
        {
            CanProg CP = new CanProg(Port)
            {
                Device = device
            };
            CanProg.transmitDescriptior = TransmitDescriptor;
            CanProg.acknowlegmentDescriptior = AcknowlegmentDescriptor;
            ProgInit Init = new ProgInit(device);
            int i = 0;
            while(true)
            {
                
                if (i == 10)
                {
                    throw new CanProgLimitConnectException("Превышен лимит попыток подключения");
                }
                
                SendMsg(CP.Port, Init, 100);
                i++;
                try
                {
                    CP.Properties = Message.Decode<ProgStatus>(GetMsg(CP.Port, CP.Device, 100)).Properties;
                    if (CP.Properties.Count != 0)
                    {
                        break;
                    }                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
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
            SendMsg(Port, ListRq);
            return Message.Decode<ProgList>(GetMsg(Port, Device)).ListDevFileInfo;
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
                SendMsg(Port, ReadRq);
                
                ProgRead Read = Message.Decode<ProgRead>(GetMsg(Port, Device));
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
            SendMsg(Port, Rm);

            return Message.Decode<ProgRmAck>(GetMsg(Port, Device)).ErrorCode;
        }
        /// <summary>
        /// Команда на очистку памяти
        /// </summary>
        public void MrProper()
        {
            ProgMrPropper MrPropper = new ProgMrPropper();
            SendMsg(Port, MrPropper);
        }
        /// <summary>
        /// Команда на создание файла
        /// </summary>
        /// <param name="fileName">Имя файла</param>
        /// <param name="Data">Данные</param>
        /// <returns></returns>
        public void CreateFile(DevFileInfo fileInfo)
        {
            ProgCreate Create = new ProgCreate()
            {
                FileName = fileInfo.FileName,
                FileSize = fileInfo.FileSize,
                CRC = fileInfo.CalcCrc()
            };

            SendMsg(Port, Create);
            ProgCreateAck CreateAck = Message.Decode<ProgCreateAck>(GetMsg(Port, Device));
            if (CreateAck.ErrorCode != 0)
                switch (CreateAck.ErrorCode)
                {
                    case 1: throw new FileNotFoundException(CreateAck.ErrorMsg[CreateAck.ErrorCode]);
                    case 2: throw new IndexOutOfRangeException(CreateAck.ErrorMsg[CreateAck.ErrorCode]);
                    case 3: throw new ArgumentOutOfRangeException(CreateAck.ErrorMsg[CreateAck.ErrorCode]);
                    case 4: throw new CanProgCreateException(CreateAck.ErrorMsg[CreateAck.ErrorCode]);
                    default: throw new CanProgException();
                }
            Write(fileInfo);
        }

        private void Write(DevFileInfo fileInfo, int offset = 0)
        {
            if (fileInfo.FileSize - offset > 0)
            {
                ProgWrite PWrite = new ProgWrite()
                {
                    FileName = fileInfo.FileName,
                    WBuff = fileInfo.Data,
                    BuffSize = fileInfo.FileSize,
                    Offset = offset
                };

                SendMsg(Port, PWrite);

                Write(fileInfo,
                    PWrite.BuffSize >= ProgWrite.DataSize + PWrite.OverheadsBytes ?
                        offset += PWrite.BuffSize - PWrite.OverheadsBytes : offset += PWrite.BuffSize
                    );
            }            
        }
        /// <summary>
        /// Команда на содание или изменение записи в словаре свойств
        /// </summary>
        /// <param name="paramKey">Ключ</param>
        /// <param name="paramValue">Значение свойства</param>
        public void SetParam(pKeys paramKey, int paramValue)
        {
            ParamSetRq psr = new ParamSetRq()
            {
                ParamKey = paramKey,
                ParamVlue = paramValue
            };
            SendMsg(Port, psr);
            ParamSetAck psa = Message.Decode<ParamSetAck>(GetMsg(Port, Device));
            if (psa.ErrorCode == 0)
                Console.WriteLine(psa.ErrorMsg[psa.ErrorCode]);
            else
                throw new CanProgCreateException(psa.ErrorMsg[psa.ErrorCode]);
        }
        /// <summary>
        /// Удаление записи из словаря свойств
        /// </summary>
        /// <param name="paramKey">Ключ</param>
        public void DeleteProperty(pKeys paramKey)
        {
            ParamRmRq prr = new ParamRmRq()
            {
                ParamKey = paramKey
            };
            SendMsg(Port, prr);
            ParamRmAck pra = Message.Decode<ParamRmAck>(GetMsg(Port, Device));
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
            byte[] b = new byte[1];
            b[1] = 0x0f;
            //SendMsg(port, device, b);
        }
    }
}
