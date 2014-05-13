using System;

namespace Fudp.Protocol.Exceptions
{
    [Serializable]
    public class FudpIdentiferMismatchException : FudpException
    {
        public FudpIdentiferMismatchException() : base("Идентефикатор сообщения не соответствует ожидаемому") { }
        public FudpIdentiferMismatchException(string message) : base(message) { }
        public FudpIdentiferMismatchException(string message, Exception inner) : base(message, inner) { }
        protected FudpIdentiferMismatchException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
