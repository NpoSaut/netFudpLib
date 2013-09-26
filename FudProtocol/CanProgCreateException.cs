using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fudp
{
    class CanProgCreateException : CanProgException
    {
        public CanProgCreateException()
            : base()
        { }
        public CanProgCreateException(String Message)
            : base(Message)
        { }
        public CanProgCreateException(String Message, Exception InnerException)
            : base(Message, InnerException)
        { }
    }
}
