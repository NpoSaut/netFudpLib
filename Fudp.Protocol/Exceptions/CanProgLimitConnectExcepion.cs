using System;

namespace Fudp.Protocol.Exceptions
{
    public class CanProgLimitConnectException : CanProgException
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
