using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fudp
{
    class ProgMrProperAck : Message
    {
        public ProgMrProperAck()
        { }
        public override byte[] Encode()
        {
            return BitConverter.GetBytes(0x0e);
        }
        protected override void Decode(byte[] Data)
        {
           
        }
    }
}
