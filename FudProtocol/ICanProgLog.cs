using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fudp
{
    public interface ICanProgLog
    {
        void PushTextEvent(String Message);
        void PushTextEvent(String Message, String Details);
    }

    public static class CanProgLoggersHelper
    {
        public static void PushTextEvent(this IEnumerable<ICanProgLog> Loggers, String Message)
        {
            foreach (var l in Loggers) l.PushTextEvent(Message);
        }
        public static void PushTextEvent(this IEnumerable<ICanProgLog> Loggers, String Message, String Details)
        {
            foreach (var l in Loggers) l.PushTextEvent(Message, Details);
        }

        public static void PushFormatTextEvent(this IEnumerable<ICanProgLog> Loggers, String Format, params object[] ps)
        {
            Loggers.PushTextEvent(string.Format(Format, ps));
        }
    }
}
