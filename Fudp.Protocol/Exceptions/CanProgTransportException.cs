using System;

namespace Fudp.Protocol.Exceptions
{
    /// <summary>
    /// Ошибка на уровне транспорта FUDP-сообщений
    /// </summary>
    [Serializable]
    public class CanProgTransportException : CanProgException
    {
        public CanProgTransportException() : base("Ошибка на уровне транспорта FUDP-сообщений. Сообщение не было доставлено") { }
        public CanProgTransportException(Exception inner) : base("Ошибка на уровне транспорта FUDP-сообщений. Сообщение не было доставлено", inner) { }
        public CanProgTransportException(string message) : base(message) { }
        public CanProgTransportException(string message, Exception inner) : base(message, inner) { }
        protected CanProgTransportException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
