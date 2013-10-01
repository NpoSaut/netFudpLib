using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fudp.Messages
{
    [Identifer(0x03)]
    class ProgListRq : Message
    {       
        /// <summary>
        /// Запроса списка фалов
        /// </summary>
        public ProgListRq()
        {
        }
        
        /// <summary>
        /// Кодирование сообщения
        /// </summary>
        /// <returns></returns>
        public override byte[] Encode()
        {
            byte [] buff = new byte[7];
            buff[0] = MessageIdentifer;
            return buff;
        }
        protected override void Decode(byte[] Data)
        {
            
        }
    }
}

