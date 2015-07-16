using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fudp.Messages
{
    [Identifer(0x0a)]
    public class ProgCreateAck : Message
    {
        private static Dictionary<int, string> errorMsg = new Dictionary<int, string>()
        {
            {0, "Файл создан успешно"},
            {1, "Файл с таким именем уже существуе"},
            {2, "Превышено максимальное количество файлов"},
            {3, "Недостаточно памяти"},
            {4, "Ошибка создания"}
        };
        public Dictionary<int, string> ErrorMsg
        {
            get { return errorMsg; }
        }
        /// <summary>
        /// Код ошибки
        /// </summary>
        private int errorCode = 0;
        public int ErrorCode
        {
            get { return errorCode; }
        }
        private Byte[] buff;
        public byte[] Buff
        {
            get { return buff; }
        }
        public ProgCreateAck()
        {
        }

        public override byte[] Encode()
        {
            buff = new byte[2];
            buff[0] = MessageIdentifer;
            buff[1] = (byte)errorCode;
            return buff;
        }
        /// <summary>
        /// Декодирование ответного сообщения
        /// </summary>
        /// <param name="Data">Принятый массив байт</param>
        protected override void Decode(byte[] Data)
        {
            errorCode = Data[1];
        }
    }
}
