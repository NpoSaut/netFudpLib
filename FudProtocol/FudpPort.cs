using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Communications.Protocols.IsoTP;
using Fudp.Messages;

namespace Fudp
{
    public class FudpPort : IFudpPort
    {
        private readonly IIsoTpConnection _isoTpConnection;
        private readonly IDisposable _rxDisposer;
        private readonly Subject<Message> _tx;

        public FudpPort(IIsoTpConnection IsoTpConnection)
        {
            Options = new FudpPortOptions(_isoTpConnection.Options.DataCapacity);

            _isoTpConnection = IsoTpConnection;
            IConnectableObservable<Message> rx = IsoTpConnection.Rx
                                                                .Select(packet => Message.DecodeMessage(packet.Data))
                                                                .Publish();
            _rxDisposer = rx.Connect();

            _tx = new Subject<Message>();
            _tx.Subscribe(Write);
        }

        /// <summary>����� �����</summary>
        public FudpPortOptions Options { get; private set; }

        public void Dispose()
        {
            _rxDisposer.Dispose();
            _tx.Dispose();
        }

        /// <summary>����� �������� ���������</summary>
        public IObservable<Message> Rx { get; private set; }

        /// <summary>����� ��������� ���������</summary>
        public IObserver<Message> Tx
        {
            get { return _tx; }
        }

        private void Write(Message Message) { _isoTpConnection.Tx.OnNext(new IsoTpPacket(Message.Encode())); }
    }
}