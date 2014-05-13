using System;

namespace Fudp.Protocol.Exceptions
{
    [Serializable]
    public class CanProgException : Exception
    {
        public CanProgException() { }
        public CanProgException(string message) : base(message) { }
        public CanProgException(string message, Exception inner) : base(message, inner) { }
        protected CanProgException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
