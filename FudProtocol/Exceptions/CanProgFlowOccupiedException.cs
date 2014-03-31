using System;
using System.Runtime.Serialization;

namespace Fudp.Exceptions
{
    /// <Summary>На этом потоке уже имеется активный экземпляр CanProg</Summary>
    [Serializable]
    public class CanProgFlowOccupiedException : CanProgException
    { 
        public CanProgFlowOccupiedException() : base("На этом потоке уже имеется активный экземпляр CanProg") { }
        public CanProgFlowOccupiedException(Exception inner) : base("На этом потоке уже имеется активный экземпляр CanProg", inner) { }
        public CanProgFlowOccupiedException(string message) : base(message) { }
        public CanProgFlowOccupiedException(string message, Exception inner) : base(message, inner) { }

        protected CanProgFlowOccupiedException(
            SerializationInfo info,
            StreamingContext context) : base(info, context) { }
    }
}