using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fudp.Messages
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
