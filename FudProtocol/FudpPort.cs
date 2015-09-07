using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Communications.PortHelpers;
using Communications.Protocols.IsoTP;
using Communications.Transactions;
using Fudp.Exceptions;
using Fudp.Messages;
using NLog;

namespace Fudp
{
    public class FudpPort : IFudpPort
    {
        private readonly IIsoTpConnection _isoTpConnection;
        private readonly ILogger _logger = LogManager.GetLogger("FUDP");
        private readonly IDisposable _rxDisposer;
        private readonly Subject<Message> _tx;

        public FudpPort(IIsoTpConnection IsoTpConnection)
        {
            _isoTpConnection = IsoTpConnection;

            Options = new FudpPortOptions(_isoTpConnection.Options.DataCapacity);

            IConnectableObservable<ITransaction<Message>> rx = IsoTpConnection.Rx
                                                                              .SelectTransaction(packet =>
                                                                                                 {
                                                                                                     Message msg = Message.DecodeMessage(packet.Data);
                                                                                                     _logger.Debug("FUDP: <-- {0}", msg);
                                                                                                     return msg;
                                                                                                 },
                                                                                                 e => new FudpTransportException(e))
                                                                              .Publish();
            Rx = rx;
            _rxDisposer = rx.Connect();

            _tx = new Subject<Message>();
            //_tx.Subscribe(Write);

            //rx.Subscribe(f => Debug.Print("FUDP: <-- {0}", f.Wait()));
            //_tx.Subscribe(f => Debug.Print("FUDP: --> {0}", f));
        }

        /// <summary>����� �����</summary>
        public FudpPortOptions Options { get; private set; }

        /// <summary>�������� �������� �����</summary>
        /// <param name="Frame">���� ��� ��������</param>
        /// <returns>���������� ��������</returns>
        public ITransaction<Message> BeginSend(Message Frame)
        {
            _logger.Debug("FUDP: --> {0}", Frame);
            return _isoTpConnection.BeginSend(new IsoTpPacket(Frame.Encode()))
                                   .AsCoreFor(Frame, e => new FudpTransportException(e));
        }

        public void Dispose()
        {
            _rxDisposer.Dispose();
            _tx.Dispose();
            _isoTpConnection.Dispose();
        }

        /// <summary>����� �������� ���������</summary>
        public IObservable<ITransaction<Message>> Rx { get; private set; }

        /// <summary>����� ��������� ���������</summary>
        public IObserver<Message> Tx
        {
            get { return _tx; }
        }
    }
}
