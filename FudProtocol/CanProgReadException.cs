using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fudp
{
    class CanProgReadException : CanProgException
    {
        public CanProgReadException()
            : base()
        { }
        public CanProgReadException(String Message)
            : base(Message)
        { }
        public CanProgReadException(String Message, Exception InnerException)
            : base(Message, InnerException)
        { }
    }
}
