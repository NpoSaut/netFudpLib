using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fudp.Messages
{
    [Identifer(0x0d)]
    class ProgMrPropper : Message
    {
        private Byte[] buff;

         public byte[] Buff
        {
            get { return buff; }
            set { ;}
        }
        /// <summary>
        /// Команда на очистку памяти
        /// </summary>
        public ProgMrPropper()
        { }
        /// <summary>
        /// Кодирование сообщения
        /// </summary>
        public override byte[] Encode()
        {
            buff = new Byte[5];
            buff[0] = MessageIdentifer;     //Идентификатор сообщения
            buff[1] = 0x4e;     // /
            buff[2] = 0x8a;     // |
            buff[3] = 0x14;     // <  шифр безопасности
            buff[4] = 0x39;     // |
                                // \  
            return buff;
        }
        protected override void Decode(byte[] Data)
        {
            buff = new byte[4];
            Buffer.BlockCopy(Data, 1, buff, 0, 4);
        }
    }
}
