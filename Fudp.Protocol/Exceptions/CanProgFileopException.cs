using System;

namespace Fudp.Protocol.Exceptions
{
    /// <summary>
    /// Ошибка при манипуляции с файлами на устройстве
    /// </summary>
    [Serializable]
    public class CanProgFileopException : CanProgException
    {
        public CanProgFileopException() : base("Ошибка при манипуляции с файлами на устройстве") { }
        public CanProgFileopException(string message) : base(message) { }
        public CanProgFileopException(string message, Exception inner) : base(message, inner) { }
        protected CanProgFileopException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
