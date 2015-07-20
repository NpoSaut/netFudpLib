using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Communications.Can;
using Communications.Protocols.IsoTP;
using Communications.Protocols.IsoTP.Exceptions;
using Communications.Protocols.IsoTP.Frames;
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
                .Or<IsoTpWrongFrameException>()
                .RetryForever();

        public IProgSession OpenSession(ICanPort CanPort, DeviceTicket Target)
        {
            IObserver<Message> initChannel = Observer.Create<Message>(f => CanPort.Tx.OnNext(new SingleFrame(f.Encode()).GetCanFrame(CanProg.FuInit)));

            ProgStatus status;
            using (IIsoTpConnection connectIsoTpPort = CanPort.OpenIsoTpConnection(CanProg.FuProg, CanProg.FuDev, TimeSpan.FromMilliseconds(500)))
            {
                using (IFudpPort connectFudpPort = new FudpPort(connectIsoTpPort))
                {
                    // ReSharper disable once AccessToDisposedClosure
                    status = _retryPolicy.Execute(() => Connect(connectFudpPort, initChannel, Target));
                }
            }
            return CreateSession(status, CanPort, Target);
        }

        private ProgStatus Connect(IFudpPort Port, IObserver<Message> InitChannel, DeviceTicket Target)
        {
            IConnectableObservable<Message> flow = Port.Rx.Replay();
            using (flow.Connect())
            {
                InitChannel.OnNext(new ProgInit(Target));
                return flow.OfType<ProgStatus>().Timeout(TimeSpan.FromMilliseconds(500)).First();
            }
        }

        private IProgSession CreateSession(ProgStatus Status, ICanPort CanPort, DeviceTicket Target)
        {
            IIsoTpConnection isoTpPort = CanPort.OpenIsoTpConnection(CanProg.FuProg, CanProg.FuDev, TimeSpan.FromMilliseconds(500));
            var fudpPort = new FudpPort(isoTpPort);
            return new DisposeProgSessionDecorator(
                new FudpProgSession(fudpPort, new PropertiesManager(Status.Properties), Target),
                fudpPort, isoTpPort);
        }
    }
}
