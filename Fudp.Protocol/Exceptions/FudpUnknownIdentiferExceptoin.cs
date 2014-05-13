using System;

namespace Fudp.Protocol.Exceptions
{
    [Serializable]
    public class FudpUnknownIdentiferException : FudpException
    {
        public FudpUnknownIdentiferException() : base("Неизвестный идентификатор FUDP-сообщения") { }
        public FudpUnknownIdentiferException(byte id) : base(String.Format("Неизвестный идентификатор FUDP-сообщения (0x{0:X2})", id)) { this.Identifer = id; }
        public FudpUnknownIdentiferException(string message) : base(message) { }
        public FudpUnknownIdentiferException(string message, Exception inner) : base(message, inner) { }
        protected FudpUnknownIdentiferException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }

        public byte Identifer { get; set; }
    }
}
