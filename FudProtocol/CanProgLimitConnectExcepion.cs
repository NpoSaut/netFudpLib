using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fudp
{
    class CanProgLimitConnectException : Exception
    {
        public CanProgLimitConnectException()
            : base()
        { }
        public CanProgLimitConnectException(String Message)
            : base(Message)
        { }
        public CanProgLimitConnectException(String Message, Exception InnerException)
            : base(Message, InnerException)
        { }
    }
}
