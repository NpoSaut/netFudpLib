using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fudp
{
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
            buff[0] = 0x03;
            return buff;
        }
        protected override void Decode(byte[] Data)
        {
            
        }
    }
}

