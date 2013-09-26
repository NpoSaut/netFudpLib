using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fudp
{
    class ProgRead : Message
    {
        private static Dictionary<int, string> errorMsg = new Dictionary<int, string>()
        {
            {0, "Файл создан успешно"},
            {1, "Файл не найден"},
            {2, "Недопустимое смещение (выходит за границу файла)"},
            {3, "Ошибка чтения"}
        };
        public Dictionary<int, string> ErrorMsg
        {
            get { return errorMsg; }
            set { ;}
        }

        Byte[] buff;
        public Byte[] Buff
        {
            get { return buff; }
            set { ;}
        }
        /// <summary>
        /// Запрошенниые данные
        /// </summary>
        Byte[] readBuff;
        public Byte[] ReadBuff
        {
            get { return readBuff; }
            set { readBuff = value; }
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
        /// Размер считываемой области
        /// </summary>
        private int readSize;
        public int ReadSize
        {
            get { return readSize; }
            set { readSize = value; }
        }

        public ProgRead()
        {
            
        }
        public override byte[] Encode()
        {
            buff = new byte[readSize + 2];
            buff[0] = 0x06;
            buff[1] = (byte)errorCode;
            Buffer.BlockCopy(readBuff, 0, buff, 2, readSize);
            return buff;
        }
        /// <summary>
        /// Декодирование ответного сообщения
        /// </summary>
        /// <param name="Data">Принятый массив байт</param>
        protected override void Decode(byte[] Data)
        {
            buff = new byte[Data.Length - 2];
            Buffer.BlockCopy(Data, 2, buff, 0, Data.Length - 2);
            errorCode = (int)Data[1];
        }

    }
}
