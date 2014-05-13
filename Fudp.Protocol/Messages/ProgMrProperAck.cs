using System;

namespace Fudp.Protocol.Messages
{
    [Identifer(0x0e)]
    class ProgMrProperAck : Message
    {
        public ProgMrProperAck()
        { }
        public override byte[] Encode()
        {
            return BitConverter.GetBytes(MessageIdentifer);
        }
        protected override void Decode(byte[] Data)
        {
           
        }
    }
}
