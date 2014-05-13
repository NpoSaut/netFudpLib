using System;

namespace Fudp.Protocol.Messages
{
    [Identifer(0x08)]
    class ProgRmAck : Message
    {
        public ProgRmAck()
        { }

        /// <summary>
        /// Код ошибки
        /// </summary>
        private int errorCode;
        public int ErrorCode
        {
            get { return errorCode; }
            set { ;}
        }

        private Byte[] buff;
        public byte[] Buff
        {
            get { return buff; }
            set { ;}
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
