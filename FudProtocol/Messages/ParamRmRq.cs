using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fudp.Messages
{
    /// <summary>
    /// Команда на удаление параметра из словаря свойств
    /// </summary>
    class ParamRmRq : Message
    {
        public ParamRmRq()
        { }

        private pKeys paramKey;
        public pKeys ParamKey
        {
            private get { return paramKey; }
            set { paramKey = value; }
        }

        public override byte[] Encode()
        {
            byte[] buff = new byte[7];
            buff[0] = 0x11;
            buff[1] = (byte)paramKey;
            return buff;
        }


        protected override void Decode(byte[] Data)
        {
            paramKey = (pKeys)Data[1];
        }
    }
}
