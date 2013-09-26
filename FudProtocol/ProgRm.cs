using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fudp
{
    class ProgRm : Message
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
        /// Код ошибки
        /// </summary>
        private int errorCode;
        public int ErrorCode
        {
            get { return errorCode; }
            set { ;}
        }

        /// <summary>
        /// Команда на удаление файла
        /// </summary>
        public ProgRm()
        { }
        /// <summary>
        /// Кодирование сообщения
        /// </summary>
        /// <returns></returns>
        public override byte[] Encode()
        {
            buff = new Byte[2 + fileName.Length];
            buff[0] = 0x07;     //Идентификатор сообщения
            buff[1] = (byte)fileName.Length;
            Buffer.BlockCopy(Encoding.GetEncoding(1251).GetBytes(fileName), 0, buff, 2, fileName.Length);
            return buff;
        }

        protected override void Decode(byte[] Data)
        {
            buff = new byte[Data[1]];
            Buffer.BlockCopy(Data, 2, buff, 0, Data[1]);
            fileName = Encoding.GetEncoding(1251).GetString(buff);
        }        
    }
}
