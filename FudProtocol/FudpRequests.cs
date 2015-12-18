using System;
using System.Threading;
using Communications.PortHelpers;
using Fudp.Exceptions;
using Fudp.Messages;
using Polly;

namespace Fudp
{
    public static class FudpRequests
    {
        private static readonly Policy _retryPolicy;

        private static readonly TimeSpan _transactionTimeout = TimeSpan.FromSeconds(50);

        static FudpRequests()
        {
            _retryPolicy =
                Policy
                    .Handle<TimeoutException>()
                    .Or<Exception>()
                    .Retry(3);
        }

        private static TAnswer FudpRequest<TAnswer>(this IFudpPort Port, Message Request, TimeSpan Timeout, CancellationToken CancellationToken)
            where TAnswer : Message
        {
            Message answer = _retryPolicy.Execute(() => Port.Request(Request, Timeout, _transactionTimeout, CancellationToken));
            if (!(answer is TAnswer))
                throw new FudpUnexpectedFrameReceivedException(typeof (TAnswer), answer);
            return (TAnswer)answer;
        }

        public static ProgPong FudpRequest(this IFudpPort Port, ProgPing Request, TimeSpan Timeout, CancellationToken CancellationToken)
        {
            return FudpRequest<ProgPong>(Port, Request, Timeout, CancellationToken);
        }

        public static ProgStatus FudpRequest(this IFudpPort Port, ProgInit Request, TimeSpan Timeout, CancellationToken CancellationToken)
        {
            return FudpRequest<ProgStatus>(Port, Request, Timeout, CancellationToken);
        }

        public static ProgList FudpRequest(this IFudpPort Port, ProgListRq Request, TimeSpan Timeout, CancellationToken CancellationToken)
        {
            return FudpRequest<ProgList>(Port, Request, Timeout, CancellationToken);
        }

        public static ProgRead FudpRequest(this IFudpPort Port, ProgReadRq Request, TimeSpan Timeout, CancellationToken CancellationToken)
        {
            return FudpRequest<ProgRead>(Port, Request, Timeout, CancellationToken);
        }

        public static ProgRmAck FudpRequest(this IFudpPort Port, ProgRm Request, TimeSpan Timeout, CancellationToken CancellationToken)
        {
            return FudpRequest<ProgRmAck>(Port, Request, Timeout, CancellationToken);
        }

        public static ProgCreateAck FudpRequest(this IFudpPort Port, ProgCreate Request, TimeSpan Timeout, CancellationToken CancellationToken)
        {
            return FudpRequest<ProgCreateAck>(Port, Request, Timeout, CancellationToken);
        }

        public static ProgWriteAck FudpRequest(this IFudpPort Port, ProgWrite Request, TimeSpan Timeout, CancellationToken CancellationToken)
        {
            return FudpRequest<ProgWriteAck>(Port, Request, Timeout, CancellationToken);
        }

        public static ProgSubmitAck FudpRequest(this IFudpPort Port, ProgSubmit Request, TimeSpan Timeout, CancellationToken CancellationToken)
        {
            return FudpRequest<ProgSubmitAck>(Port, Request, Timeout, CancellationToken);
        }

        public static ParamSetAck FudpRequest(this IFudpPort Port, ParamSetRq Request, TimeSpan Timeout, CancellationToken CancellationToken)
        {
            return FudpRequest<ParamSetAck>(Port, Request, Timeout, CancellationToken);
        }

        public static ParamRmAck FudpRequest(this IFudpPort Port, ParamRmRq Request, TimeSpan Timeout, CancellationToken CancellationToken)
        {
            return FudpRequest<ParamRmAck>(Port, Request, Timeout, CancellationToken);
        }
    }
}
