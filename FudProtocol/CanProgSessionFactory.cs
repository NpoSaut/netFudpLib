using System;
using System.Reactive.Linq;
using Fudp.Messages;

namespace Fudp
{
    public class CanProgSessionFactory : ICanProgSessionFactory
    {
        public CanProgSession OpenSession(IFudpPort Port, DeviceTicket Target, TimeSpan Timeout)
        {
            ProgStatus status = Observable.Repeat<Object>(null)
                                          .Select(x => Port.FudpRequest(new ProgInit(Target), Timeout))
                                          .Repeat()
                                          .First();

            return new CanProgSession(Port, new PropertiesManager(status.Properties), Target);
        }
    }
}
