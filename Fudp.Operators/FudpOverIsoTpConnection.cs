using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Communications.Can;
using Communications.Protocols.IsoTP;
using Communications.Protocols.IsoTP.Exceptions;
using Fudp.Protocol.Exceptions;
using Fudp.Protocol.Messages;

namespace Fudp.Operators
{
    public class FudpOverIsoTpConnection : IFudpConnection
    {
        private static readonly Dictionary<CanFlow, FudpOverIsoTpConnection> ProgsOnFlows = new Dictionary<CanFlow, FudpOverIsoTpConnection>();
        protected readonly Timer PingTimer;

        private Byte _pingCounter;

        const int DefaultMaximumSendAttempts = 7;
        private const int DefaultFudpTimeout = 700;

        public FudpOverIsoTpConnection(CanPort Port)
        {
            Flow = new CanFlow(Port, FudpOptions.FuDev, FudpOptions.FuProg);
            PingTimer = new Timer(PingTimer_Callback, null, Timeout.Infinite, Timeout.Infinite);
        }

        protected CanFlow Flow { get; private set; }

        /// <summary>Отправляет сообщение</summary>
        /// <param name="Msg">Отправляемое сообщение</param>
        /// <param name="TimeOut">Таймаут на ожидание ответа</param>
        /// <param name="WithTransmitDescriptor">Дескриптор, с которым передаётся сообщение</param>
        /// <param name="WithAcknowledgmentDescriptor">Дескриптор, с которым передаются подтверждения на сообщение</param>
        /// <param name="MaxAttempts">Максимальное количество попыток отправки</param>
        public void SendMessage(Message Msg, int TimeOut = DefaultFudpTimeout, UInt16 WithTransmitDescriptor = FudpOptions.FuProg, UInt16 WithAcknowledgmentDescriptor = FudpOptions.FuDev, int MaxAttempts = DefaultMaximumSendAttempts)
        {
            lock (Flow)
            {
                SuspendPingTimer(Flow);
                Flow.Clear();
                for (int attempt = 0; attempt < MaxAttempts; attempt++)
                {
                    //Logs.PushFormatTextEvent("--> {0}", Msg);
                    Debug.Print("--> {0}", Msg);
                    try
                    {
                        Flow.Clear();
                        IsoTp.Send(Flow, WithTransmitDescriptor, WithAcknowledgmentDescriptor, Msg.Encode(), TimeSpan.FromMilliseconds(TimeOut));
                        break;
                    }
                    catch (IsoTpProtocolException istoProtocolException)
                    {
                        //Logs.PushFormatTextEvent("Исключение во время передачи: {0}", istoProtocolException.Message);
                        Debug.Print("Исключение во время передачи: {0}", istoProtocolException.Message);
                        Thread.Sleep(100);
                        if (attempt >= MaxAttempts - 1) throw new CanProgTransportException(istoProtocolException);
                    }
                }
            }
            ResetPingTimer(Flow);
        }

        public TMessage ReceiveMessage<TMessage>(int TimeOut = DefaultFudpTimeout, UInt16 WithTransmitDescriptor = FudpOptions.FuDev, UInt16 WithAcknowledgmentDescriptor = FudpOptions.FuProg)
            where TMessage : Message
        {
            lock (Flow)
            {
                SuspendPingTimer(Flow);
                var sw = new Stopwatch();
                sw.Start();
                while (10 * sw.ElapsedMilliseconds < TimeOut)
                {
                    try
                    {
                        var tr = IsoTp.Receive(Flow, WithTransmitDescriptor, WithAcknowledgmentDescriptor, TimeSpan.FromMilliseconds(TimeOut));
                        var mes = Message.DecodeMessage(tr.Data);
                        var typedMes = mes as TMessage;
                        if (typedMes != null)
                        {
                            Debug.Print("<-- {0}", typedMes);
                            ResetPingTimer(Flow);
                            return typedMes;
                        }
                        else
                        {
                            Debug.Print("<-- {0} - игнорируем (ожидали {1})", mes, typeof(TMessage));
                        }
                    }
                    catch (IsoTpProtocolException) { }
                }
                ResetPingTimer(Flow);
                throw new FudpReceiveTimeoutException(string.Format("Превышено время ожидания FUDP-сообщения (ожидали сообщения {0})", typeof(TMessage)));
            }
        }

        public TAnswer Request<TAnswer>(Message Request, int TimeOut = DefaultFudpTimeout, UInt16 ThisSideDescriptor = FudpOptions.FuProg, UInt16 TheirSideDescriptor = FudpOptions.FuDev, int MaxAttempts = DefaultMaximumSendAttempts)
            where TAnswer : Message
        {
            lock (Flow)
            {
                Exception lastException = null;
                for (int attempt = 0; attempt < MaxAttempts; attempt++)
                {
                    try
                    {
                        SendMessage(Request, TimeOut, ThisSideDescriptor, TheirSideDescriptor, MaxAttempts);
                        return ReceiveMessage<TAnswer>(TimeOut, TheirSideDescriptor, ThisSideDescriptor);
                    }
                    catch (IsoTpProtocolException ex) { lastException = ex; }
                    catch (FudpReceiveTimeoutException ex) { lastException = ex; }
                    Thread.Sleep(200);
                }
                Debug.Print("Исключение во время передачи: {0}", lastException);
                throw new CanProgTransportException(lastException);
            }
        }

        public int MaximumPacketLength
        {
            get { return 4095; }
        }

        private void PingTimer_Callback(object State)
        {
            Debug.Print("PING!");
            var pingMessage = new ProgPing(_pingCounter);
            var pongMessage = Request<ProgPong>(pingMessage, 200);
            _pingCounter++;
        }

        #region Сброс и приостановка PING-таймера

        private void ResetPingTimer() { PingTimer.Change(1000, Timeout.Infinite); }

        private static void ResetPingTimer(FudpOverIsoTpConnection Session) { Session.ResetPingTimer(); }

        private static void ResetPingTimer(CanFlow Flow)
        {
            FudpOverIsoTpConnection connection;
            lock (ProgsOnFlows)
            {
                connection = ProgsOnFlows[Flow];
            }
            connection.ResetPingTimer();
        }

        private void SuspendPingTimer() { PingTimer.Change(Timeout.Infinite, Timeout.Infinite); }
        private static void SuspendPingTimer(FudpOverIsoTpConnection Connection) { Connection.SuspendPingTimer(); }

        private static void SuspendPingTimer(CanFlow Flow)
        {
            FudpOverIsoTpConnection connection;
            lock (ProgsOnFlows)
            {
                connection = ProgsOnFlows[Flow];
            }
            SuspendPingTimer(connection);
        }

        #endregion

        /// <summary>
        /// Выполняет определяемые приложением задачи, связанные с удалением, высвобождением или сбросом неуправляемых ресурсов.
        /// </summary>
        public void Dispose()
        {
            Flow.Dispose();
        }
    }
}
