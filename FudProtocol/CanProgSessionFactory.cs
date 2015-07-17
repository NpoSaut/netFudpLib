using System;
using Fudp.Exceptions;
using Fudp.Messages;
using Polly;

namespace Fudp
{
    public class CanProgSessionFactory : ICanProgSessionFactory
    {
        private static readonly Policy _retryPolicy =
            Policy
                .Handle<TimeoutException>()
                .Or<FudpException>()
                .RetryForever();

        public CanProgSession OpenSession(IFudpPort Port, DeviceTicket Target, TimeSpan Timeout)
        {
            ProgStatus status = _retryPolicy.Execute(() => Port.FudpRequest(new ProgInit(Target), Timeout));
            return new CanProgSession(Port, new PropertiesManager(status.Properties), Target);
        }
    }
}
