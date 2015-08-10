using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fudp.Messages
{
    [Identifer(0x08)]
    public class ProgRmAck : Message
    {
        public ProgRmAck(int ErrorCode = 0) { this.ErrorCode = ErrorCode; }

        public int ErrorCode { get; private set; }

        public override byte[] Encode()
        {
            var buff = new byte[2];
            buff[0] = MessageIdentifer;
            buff[1] = (byte)ErrorCode;
            return buff;
        }

        protected override void Decode(byte[] Data)
        {
            ErrorCode = Data[1];
        }
    }
}
