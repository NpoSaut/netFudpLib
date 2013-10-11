using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fudp.Messages
{
    /// <summary>
    /// Команда загрузчику на выход из режима программирования
    /// </summary>
    [Identifer(0x13)]
    public class ProgSubmit : Message
    {
        protected override void Decode(byte[] Data)
        {}

        public override byte[] Encode()
        {
            return new Byte[] { MessageIdentifer };
        }
    }
}
