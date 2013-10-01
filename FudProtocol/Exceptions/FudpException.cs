using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fudp.Exceptions
{
    [Serializable]
    public class FudpException : Exception
    {
        public FudpException() { }
        public FudpException(string message) : base(message) { }
        public FudpException(string message, Exception inner) : base(message, inner) { }
        protected FudpException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
