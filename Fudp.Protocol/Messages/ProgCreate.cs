using System;
using System.Text;

namespace Fudp.Protocol.Messages
{
    [Identifer(0x09)]
    class ProgCreate :Message
    {
        private Byte[] buff;
        public byte[] Buff
        {
            get { return buff; }
            set { ;}
        }

        /// <summary>
        /// Имя файла
        /// </summary>
        private string fileName;
        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }
        /// <summary>
        /// Размер файла
        /// </summary>
        private int fileSize;
        public int FileSize
        {
            get { return fileSize; }
            set { fileSize = value; }
        }
        /// <summary>
        /// Код ошибки
        /// </summary>
        private int errorCode;
        public int ErrorCode
        {
            get { return errorCode; }
            set { ;}
        }

        private int crc;
        public int CRC
        {
            get { return crc; }
            set { crc = value; }
        }

        /// <summary>
        /// Команда на создание файла
        /// </summary>
        public ProgCreate()
        { }
        /// <summary>
        /// Кодирование сообщения
        /// </summary>
        /// <returns></returns>
        public override byte[] Encode()
        {
            buff = new Byte[10 + fileName.Length];
            buff[0] = MessageIdentifer;     //Идентификатор сообщения
            buff[1] = (byte)fileName.Length;
            Buffer.BlockCopy(Encoding.GetEncoding(1251).GetBytes(fileName), 0, buff, 2, fileName.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(fileSize), 0, buff, 2 + fileName.Length, intSize);
            Buffer.BlockCopy(BitConverter.GetBytes(crc), 0, buff, 6 + fileName.Length, intSize); 
            return buff;
        }
        protected override void Decode(byte[] Data)
        {
            byte[] filename = new byte[Data[1]];
            Buffer.BlockCopy(Data, 2, filename, 0, Data[1]);
            fileName = Encoding.GetEncoding(1251).GetString(filename);
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", base.ToString(), fileName);
        }
    }
}
