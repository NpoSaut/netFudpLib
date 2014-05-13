using System;
using System.Collections.Generic;

namespace Fudp.Protocol.Messages
{
    [Identifer(0x06)]
    public class ProgRead : Message
    {
        private static readonly Dictionary<int, string> MessagesDescriptions = new Dictionary<int, string>()
        {
            { 0, "Файл создан успешно" },
            { 1, "Файл не найден" },
            { 2, "Недопустимое смещение (выходит за границу файла)" },
            { 3, "Ошибка чтения" }
        };

        /// <summary>Запрошенные данные</summary>
        public byte[] ReadData { get; set; }

        /// <summary>Код ошибки</summary>
        public int ErrorCode { get; private set; }

        /// <summary>Размер считываемой области</summary>
        public int ReadSize { get; set; }

        /// <summary>Описание ошибки</summary>
        public string ErrorMessage
        {
            get { return MessagesDescriptions[ErrorCode]; }
        }

        public ProgRead() { }

        public override byte[] Encode()
        {
            var buff = new byte[ReadSize + 2];
            buff[0] = MessageIdentifer;
            buff[1] = (byte)ErrorCode;
            Buffer.BlockCopy(ReadData, 0, buff, 2, ReadSize);
            return buff;
        }

        /// <summary>Декодирование ответного сообщения</summary>
        /// <param name="Data">Принятый массив байт</param>
        protected override void Decode(byte[] Data)
        {
            ReadData = new byte[Data.Length - 2];
            Buffer.BlockCopy(Data, 2, ReadData, 0, ReadData.Length);
            ErrorCode = Data[1];
        }

    }
}
