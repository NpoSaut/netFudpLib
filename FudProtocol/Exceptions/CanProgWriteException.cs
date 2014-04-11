using System;
using System.Runtime.Serialization;
using Fudp.Messages;

namespace Fudp.Exceptions
{
    /// <Summary>Ошибка при записи в файл</Summary>
    [Serializable]
    public class CanProgWriteException : Exception
    {
        public CanProgWriteException() : base("Ошибка при записи в файл") { }

        protected CanProgWriteException(
            SerializationInfo info,
            StreamingContext context) : base(info, context) { }

        public CanProgWriteException(ProgWriteAck.WriteStatusKind ErrorKind) : this() { this.ErrorKind = ErrorKind; }

        public ProgWriteAck.WriteStatusKind ErrorKind { get; set; }
    }
}
