﻿namespace Fudp.Messages
{
    [Identifer(0x00)]
    public class ProgBCastResponse : ProgInit
    {
        public override string ToString()
        {
            return string.Format("BC-Resp от {0}", Ticket);
        }
    }
}