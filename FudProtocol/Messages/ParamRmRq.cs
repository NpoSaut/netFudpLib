using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fudp.Messages
{
    /// <summary>
    /// Команда на удаление параметра из словаря свойств
    /// </summary>
    [Identifer(0x11)]
    public class ParamRmRq : Message
    {
        public ParamRmRq(byte ParamKey) { this.ParamKey = ParamKey; }

        public byte ParamKey { get; private set; }

        public override byte[] Encode()
        {
            byte[] buff = new byte[7];
            buff[0] = 0x11;
            buff[1] = (byte)ParamKey;
            return buff;
        }


        protected override void Decode(byte[] Data)
        {
            ParamKey = Data[1];
        }
    }
}
