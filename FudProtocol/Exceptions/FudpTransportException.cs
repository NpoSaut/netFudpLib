using System;
using System.Runtime.Serialization;

namespace Fudp.Exceptions
{
    /// <Summary>Ошибка на уровне транспортировки FUDP-сообщений</Summary>
    [Serializable]
    public class FudpTransportException : FudpException
    {
        public FudpTransportException(Exception inner) : base("Ошибка на уровне транспортировки FUDP-сообщений", inner) { }

        protected FudpTransportException(
            SerializationInfo info,
            StreamingContext context) : base(info, context) { }
    }
}
