using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fudp.Messages
{   
    /// <summary>
    /// Команда на содание или изменение записи в словаре свойств
    /// </summary>
    [Identifer(0x0f)]
    class ParamSetRq : Message
    {
        public byte ParamKey { get; set; }

        private int paramValue;
        public int ParamVlue
        {
            private get { return paramValue; }
            set { paramValue = value; }
        }
        
        public ParamSetRq()
        { }

        public override byte[] Encode()
        {
            byte[] buff = new byte[7];
            buff[0] = MessageIdentifer;
            buff[1] = ParamKey;
            Buffer.BlockCopy(BitConverter.GetBytes(paramValue), 0, buff, 2, 4);
            return buff;
        }

        protected override void Decode(byte[] Data)
        {
            ParamKey = Data[1];
            paramValue = BitConverter.ToInt32(Data, 2);
        }

        public override string ToString()
        {
            return string.Format("{0}  [ set {{{1}}} to \"{2}\" ]", base.ToString(), ParamKey, paramValue);
        }
    }
}
