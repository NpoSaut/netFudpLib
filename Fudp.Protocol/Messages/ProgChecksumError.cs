using System;

namespace Fudp.Protocol.Messages
{
    [Identifer(0xff)]
    public class ProgChecksumError : Message
    {
        protected override void Decode(byte[] Data)
        {
        }

        public override byte[] Encode()
        {
            return new Byte[] { MessageIdentifer };
        }
    }
}
