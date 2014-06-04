using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fudp.Messages
{
    [Identifer(0x09)]
    public class ProgCreate :Message
    {
        /// <summary>
        /// Имя файла
        /// </summary>
        public string FileName { get; private set; }
        /// <summary>
        /// Размер файла
        /// </summary>
        public int FileSize { get; private set; }
        /// <summary>
        /// Код ошибки
        /// </summary>
        private int errorCode;
        public int ErrorCode
        {
            get { return errorCode; }
            set { ;}
        }

        public int CRC { get; private set; }

        /// <summary>
        /// Команда на создание файла
        /// </summary>
        public ProgCreate(string FileName, int FileSize, int Crc)
        {
            CRC = Crc;
            this.FileSize = FileSize;
            this.FileName = FileName;
        }

        /// <summary>
        /// Кодирование сообщения
        /// </summary>
        /// <returns></returns>
        public override byte[] Encode()
        {
            var buff = new Byte[10 + FileName.Length];
            buff[0] = MessageIdentifer;     //Идентификатор сообщения
            buff[1] = (byte)FileName.Length;
            Buffer.BlockCopy(Encoding.GetEncoding(1251).GetBytes(FileName), 0, buff, 2, FileName.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(FileSize), 0, buff, 2 + FileName.Length, intSize);
            Buffer.BlockCopy(BitConverter.GetBytes(CRC), 0, buff, 6 + FileName.Length, intSize); 
            return buff;
        }
        protected override void Decode(byte[] Data)
        {
            byte[] filename = new byte[Data[1]];
            Buffer.BlockCopy(Data, 2, filename, 0, Data[1]);
            FileName = Encoding.GetEncoding(1251).GetString(filename);
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", base.ToString(), FileName);
        }
    }
}
