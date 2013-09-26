using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fudp
{
    class CanProgException : Exception
    {
        public CanProgException()
            : base()
        { }
        public CanProgException(String Message)
            : base(Message)
        { }
        public CanProgException(String Message, Exception InnerException)
            : base(Message, InnerException)
        { }
    }
}
