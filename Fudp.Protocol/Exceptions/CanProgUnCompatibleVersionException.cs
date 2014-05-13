using System;
using System.Runtime.Serialization;

namespace Fudp.Protocol.Exceptions
{
    /// <Summary>
    /// Версии протокола не совместимы
    /// </Summary>
    [Serializable]
    public class CanProgUnCompatibleVersionException : Exception
    {
        /// <summary>Версия протокола программатора</summary>
        public int ProgrammerProtocolVersion { get; set; }
        /// <summary>Версия протокола на устройстве</summary>
        public int DeviceProtocolVersoin { get; set; }

        public CanProgUnCompatibleVersionException() : base("Версии протокола не совместимы") { }
        public CanProgUnCompatibleVersionException(Exception inner) : base("Версии протокола не совместимы", inner) { }
        public CanProgUnCompatibleVersionException(string message) : base(message) { }
        public CanProgUnCompatibleVersionException(string message, Exception inner) : base(message, inner) { }

        protected CanProgUnCompatibleVersionException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        { }

        public CanProgUnCompatibleVersionException(int ProgrammerProtocolVersion, int DeviceProtocolVersoin)
            : base(string.Format("Версии протокола не совместимы: версия программатора {0}, версия загрузчика {1}", ProgrammerProtocolVersion, DeviceProtocolVersoin))
        {
            this.ProgrammerProtocolVersion = ProgrammerProtocolVersion;
            this.DeviceProtocolVersoin = DeviceProtocolVersoin;
        }
    }
}
