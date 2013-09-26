using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fudp.Messages
{
    class ProgReadRq : Message
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
        /// Отступ от начала файла
        /// </summary>
        private int offset;
        public int Offset
        {
            get { return offset; }
            set { offset = value; }
        }
        
        private int fileSize;
        public int FileSize
        {
            get { return fileSize - offset; }
            set { fileSize = value; }
        }
        /// <summary>
        /// Размер считываемой области
        /// </summary>
        private int readSize;
        public int ReadSize
        {
            get
            {
                if (FileSize > 4000)
                {
                    return 4000;
                }
                else
                    return FileSize;
            }
            set { ; }
        }
        
        /// <summary>
        /// Запрос на чтение
        /// </summary>
        public ProgReadRq()
        {            
        }        
        /// <summary>
        /// Кодирование сообщения
        /// </summary>
        /// <returns></returns>
        public override byte[] Encode()
        {
            buff = new Byte[10 + fileName.Length];
            buff[0] = 0x05;     //Идентификатор сообщения
            buff[1] = (byte)fileName.Length;
            Buffer.BlockCopy(Encoding.GetEncoding(1251).GetBytes(fileName), 0, buff, 2, fileName.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(offset), 0, buff, 2 + fileName.Length, intSize);
            Buffer.BlockCopy(BitConverter.GetBytes(ReadSize), 0, buff, 6 + fileName.Length, intSize);
            return buff;
        }

        protected override void Decode(byte[] Data)
        {
            byte[] filename = new byte[Data[1]];
            byte[] bOffset = new byte[intSize];
            byte[] bReadSize = new byte[intSize];
            Buffer.BlockCopy(Data, 2, filename, 0, Data[1]);
            Buffer.BlockCopy(Data, 2+Data[1], bOffset, 0, intSize);
            Buffer.BlockCopy(Data, 6 + Data[1], bReadSize, 0, intSize);
            fileName = Encoding.GetEncoding(1251).GetString(filename);
            offset = BitConverter.ToInt32(bOffset, 0);
            readSize = BitConverter.ToInt32(bReadSize, 0);
        }
    }
}
