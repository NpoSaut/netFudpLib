using System;
using System.Reactive.Linq;
using Fudp.Messages;

namespace Fudp
{
    public class CanProgSessionFactory : ICanProgSessionFactory
    {
        private readonly TimeSpan _timeout;
        public CanProgSessionFactory(TimeSpan Timeout) { _timeout = Timeout; }

        public CanProgSession OpenSession(IFudpPort Port, DeviceTicket Target)
        {
            ProgStatus status = Observable.Repeat<Object>(null)
                                          .Select(x => Port.FudpRequest(new ProgInit(Target), _timeout))
                                          .Repeat()
                                          .First();

            return new CanProgSession(Port, new PropertiesManager(status.Properties), Target);
        }
    }
}
