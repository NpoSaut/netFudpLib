using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Fudp.Exceptions
{
    /// <Summary>
    /// Превышено время ожидания FUDP-сообщения
    /// </Summary>
    [Serializable]
    public class FudpReceiveTimeoutException : FudpException
    {
        public FudpReceiveTimeoutException() : base("Превышено время ожидания FUDP-сообщения")
        {
        }

        public FudpReceiveTimeoutException(Exception inner) : base("Превышено время ожидания FUDP-сообщения", inner)
        {
        }

        public FudpReceiveTimeoutException(string message) : base(message)
        {
        }

        public FudpReceiveTimeoutException(string message, Exception inner) : base(message, inner)
        {
        }

        protected FudpReceiveTimeoutException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
