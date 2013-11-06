using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Fudp.Exceptions
{
    /// <Summary>
    /// Ошибка при записи в файл
    /// </Summary>
    [Serializable]
    public class CanProgWriteException : Exception
    {
        public Messages.ProgWriteAck.WriteStatusKind writeStatus { get; set; }

        public CanProgWriteException() : base("Ошибка при записи в файл")
        {
        }

        public CanProgWriteException(Exception inner) : base("Ошибка при записи в файл", inner)
        {
        }

        public CanProgWriteException(string message) : base(message)
        {
        }

        public CanProgWriteException(string message, Exception inner) : base(message, inner)
        {
        }

        protected CanProgWriteException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }

        public CanProgWriteException(Messages.ProgWriteAck.WriteStatusKind writeStatus)
        {
            this.writeStatus = writeStatus;
        }
    }
}
