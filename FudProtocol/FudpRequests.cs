using System;
using System.Reactive.Linq;
using Communications;
using Fudp.Messages;

namespace Fudp
{
    public static class FudpRequests
    {
        private static TAnswer FudpRequest<TAnswer>(this IFudpPort Port, Message Request, TimeSpan Timeout)
            where TAnswer : Message
        {
            return Observable.Repeat(Request)
                             .Select(rq => Port.Request(Request, Timeout))
                             .Cast<TAnswer>()
                             .Repeat(3)
                             .First();

            //var ans = (TAnswer)Port.Request(Request, Timeout);
            //if (ans == null)
            //    throw new ApplicationException("Ожидали не то сообщение");
            //return ans;
        }

        public static ProgPong FudpRequest(this IFudpPort Port, ProgPing Request, TimeSpan Timeout) { return FudpRequest<ProgPong>(Port, Request, Timeout); }

        public static ProgStatus FudpRequest(this IFudpPort Port, ProgInit Request, TimeSpan Timeout)
        {
            return FudpRequest<ProgStatus>(Port, Request, Timeout);
        }

        public static ProgList FudpRequest(this IFudpPort Port, ProgListRq Request, TimeSpan Timeout) { return FudpRequest<ProgList>(Port, Request, Timeout); }

        public static ProgRead FudpRequest(this IFudpPort Port, ProgReadRq Request, TimeSpan Timeout) { return FudpRequest<ProgRead>(Port, Request, Timeout); }

        public static ProgRmAck FudpRequest(this IFudpPort Port, ProgRm Request, TimeSpan Timeout) { return FudpRequest<ProgRmAck>(Port, Request, Timeout); }

        public static ProgCreateAck FudpRequest(this IFudpPort Port, ProgCreate Request, TimeSpan Timeout)
        {
            return FudpRequest<ProgCreateAck>(Port, Request, Timeout);
        }

        public static ProgWriteAck FudpRequest(this IFudpPort Port, ProgWrite Request, TimeSpan Timeout)
        {
            return FudpRequest<ProgWriteAck>(Port, Request, Timeout);
        }

        public static ProgSubmitAck FudpRequest(this IFudpPort Port, ProgSubmit Request, TimeSpan Timeout)
        {
            return FudpRequest<ProgSubmitAck>(Port, Request, Timeout);
        }

        public static ParamSetAck FudpRequest(this IFudpPort Port, ParamSetRq Request, TimeSpan Timeout)
        {
            return FudpRequest<ParamSetAck>(Port, Request, Timeout);
        }

        public static ParamRmAck FudpRequest(this IFudpPort Port, ParamRmRq Request, TimeSpan Timeout)
        {
            return FudpRequest<ParamRmAck>(Port, Request, Timeout);
        }
    }
}
