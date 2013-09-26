using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fudp.Messages
{
    class ProgWrite : Message
    {
        public const int DataSize = 4000;
        private int buffSize;
        public int BuffSize
        {
            get
            {
                if (buffSize - offset > DataSize + OverheadsBytes)
                    return DataSize + OverheadsBytes;
                else
                    return wBuff.Length - offset + OverheadsBytes;
            }
            set { buffSize = value; }
        }
        private Byte[] buff;
        public byte[] Buff
        {
            get { return buff; }
            set { ;}
        }
        /// <summary>
        /// Данные для записи
        /// </summary>
        private byte[] wBuff;
        public byte[] WBuff
        {
            get { return wBuff; }
            set { wBuff = value;}
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
        /// Смещение от начала файла
        /// </summary>
        private int offset;
        public int Offset
        {
            get { return offset; }
            set { offset = value; }
        }

        public int OverheadsBytes
        {
            get { return fileName.Length + 6; }
            set { ;}
        }

        /// <summary>
        /// Команда на запись в файл
        /// </summary>
        public ProgWrite()
        { }
        /// <summary>
        /// Кодирование сообщения
        /// </summary>
        /// <returns></returns>
        public override byte[] Encode()
        {
            buff = new Byte[BuffSize];
            buff[0] = 0x0b;
            buff[1] = (byte)fileName.Length;
            Buffer.BlockCopy(Encoding.GetEncoding(1251).GetBytes(fileName), 0, buff, 2, fileName.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(offset), 0, buff, 2 + fileName.Length, intSize);
            Buffer.BlockCopy(wBuff, offset, buff, OverheadsBytes, BuffSize - OverheadsBytes);
            return buff;
        }

        protected override void Decode(byte[] Data)
        {
            buff = new byte[(int)Data[1]];
            Buffer.BlockCopy(Data, 1, buff, 0, (int)Data[1]);
            fileName = Encoding.GetEncoding(1251).GetString(buff);
            buff = new byte[intSize];
            Buffer.BlockCopy(Data, 2 + fileName.Length, buff, 0, intSize);
            offset = BitConverter.ToInt32(buff, 0);
            wBuff = new byte[Data.Length - fileName.Length - 6];
            Buffer.BlockCopy(Data, 6 + fileName.Length, wBuff, 0, Data.Length - fileName.Length - 6);
        }

    }
}
