using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using Communications.Can;
using Communications.PortHelpers;
using Communications.Protocols.IsoTP;
using Communications.Protocols.IsoTP.Exceptions;
using Communications.Protocols.IsoTP.Frames;
using Communications.Transactions;
using Fudp.Exceptions;
using Fudp.Messages;
using Polly;

namespace Fudp
{
    public class CanProgSessionFactory : ICanProgSessionFactory
    {
        private static readonly Policy _retryPolicy =
            Policy
                .Handle<FudpException>()
                .Or<IsoTpProtocolException>()
                .Or<TimeoutException>()
                .RetryForever();

        public IProgSession OpenSession(ICanPort CanPort, DeviceTicket Target, CancellationToken CancellationToken)
        {
            IObserver<Message> initChannel =
                Observer.Create<Message>(f => CanPort.BeginSend(new SingleFrame(f.Encode()).GetCanFrame(CanProg.FuInit))
                                                     .Wait(CancellationToken));

            ProgStatus status;
            using (IIsoTpConnection connectIsoTpPort = CanPort.OpenIsoTpConnection(CanProg.FuProg, CanProg.FuDev, new IsoTpConnectionParameters()))
            {
                using (IFudpPort connectFudpPort = new FudpPort(connectIsoTpPort))
                {
                    // ReSharper disable once AccessToDisposedClosure
                    status = _retryPolicy.Execute(() => Connect(connectFudpPort, initChannel, Target, CancellationToken));
                }
            }
            return CreateSession(status, CanPort, Target);
        }

        private ProgStatus Connect(IFudpPort Port, IObserver<Message> InitChannel, DeviceTicket Target, CancellationToken CancellationToken)
        {
            CancellationToken.ThrowIfCancellationRequested();
            //return Port.FudpRequest(new ProgInit(Target), TimeSpan.FromMilliseconds(500), CancellationToken);

            IConnectableObservable<ITransaction<Message>> flow = Port.Rx.Replay();
            using (flow.Connect())
            {
                InitChannel.OnNext(new ProgInit(Target));
                return flow.Timeout(TimeSpan.FromMilliseconds(500))
                           .WaitForTransactionCompleated(TimeSpan.FromMilliseconds(500), CancellationToken)
                           .OfType<ProgStatus>().First();
            }
        }

        private IProgSession CreateSession(ProgStatus Status, ICanPort CanPort, DeviceTicket Target)
        {
            IIsoTpConnection isoTpPort = CanPort.OpenIsoTpConnection(CanProg.FuProg, CanProg.FuDev, new IsoTpConnectionParameters());
            var fudpPort = new FudpPort(isoTpPort);
            return new DisposeProgSessionDecorator(
                new FudpProgSession(fudpPort, new PropertiesManager(Status.Properties), Target),
                fudpPort, isoTpPort);
        }
    }
}
