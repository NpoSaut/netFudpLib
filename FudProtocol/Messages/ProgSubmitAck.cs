using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fudp.Messages
{
    [Identifer(0x14)]
    class ProgSubmitAck : Message
    {
        protected override void Decode(byte[] Data)
        {
        }

        public override byte[] Encode()
        {
            return new[] {MessageIdentifer};
        }
    }
}
