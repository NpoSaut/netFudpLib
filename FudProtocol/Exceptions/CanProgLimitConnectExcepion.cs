﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fudp.Exceptions
{
    public class CanProgLimitConnectException : CanProgException
    {
        public CanProgLimitConnectException()
            : base()
        { }
        public CanProgLimitConnectException(String Message)
            : base(Message)
        { }
        public CanProgLimitConnectException(String Message, Exception InnerException)
            : base(Message, InnerException)
        { }
    }
}
