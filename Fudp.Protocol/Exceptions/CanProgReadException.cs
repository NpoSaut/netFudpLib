using System;

namespace Fudp.Protocol.Exceptions
{
    public class CanProgReadException : CanProgException
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
