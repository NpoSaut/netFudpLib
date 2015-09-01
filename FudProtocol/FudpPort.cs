using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Communications.PortHelpers;
using Communications.Protocols.IsoTP;
using Communications.Transactions;
using Fudp.Messages;
using NLog;

namespace Fudp
{
    public class FudpPort : IFudpPort
    {
        private readonly IIsoTpConnection _isoTpConnection;
        private readonly IDisposable _rxDisposer;
        private readonly Subject<Message> _tx;

        private readonly ILogger _logger = LogManager.GetLogger("FUDP");

        public FudpPort(IIsoTpConnection IsoTpConnection)
        {
            _isoTpConnection = IsoTpConnection;

            Options = new FudpPortOptions(_isoTpConnection.Options.DataCapacity);

            IConnectableObservable<ITransaction<Message>> rx = IsoTpConnection.Rx
                                                                              .SelectTransaction(packet =>
                                                                                                 {
                                                                                                     var msg = Message.DecodeMessage(packet.Data);
                                                                                                     _logger.Debug("FUDP: <-- {0}", msg);
                                                                                                     return msg;
                                                                                                 })
                                                                              .Publish();
            Rx = rx;
            _rxDisposer = rx.Connect();

            _tx = new Subject<Message>();
            //_tx.Subscribe(Write);

            //rx.Subscribe(f => Debug.Print("FUDP: <-- {0}", f.Wait()));
            //_tx.Subscribe(f => Debug.Print("FUDP: --> {0}", f));
        }

        /// <summary>Опции порта</summary>
        public FudpPortOptions Options { get; private set; }

        /// <summary>Начинает отправку кадра</summary>
        /// <param name="Frame">Кадр для отправки</param>
        /// <returns>Транзакция передачи</returns>
        public ITransaction<Message> BeginSend(Message Frame)
        {
            _logger.Debug("FUDP: --> {0}", Frame);
            return _isoTpConnection.BeginSend(new IsoTpPacket(Frame.Encode()))
                                   .AsCoreFor(Frame);
        }

        public void Dispose()
        {
            _rxDisposer.Dispose();
            _tx.Dispose();
            _isoTpConnection.Dispose();
        }

        /// <summary>Поток входящих сообщений</summary>
        public IObservable<ITransaction<Message>> Rx { get; private set; }

        /// <summary>Поток исходящих сообщений</summary>
        public IObserver<Message> Tx
        {
            get { return _tx; }
        }

        [Obsolete("", true)]
        private void Write(Message Message)
        {
            throw new NotImplementedException();
            //_isoTpConnection.Tx.OnNext(new IsoTpPacket(Message.Encode()));
        }
    }
}
