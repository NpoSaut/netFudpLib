using System;
using Fudp.Protocol.Messages;

namespace Fudp.Operators
{
    public interface IFudpConnection : IDisposable
    {
        int MaximumPacketLength { get; }
        void SendMessage(Message Msg);
        TMessage ReceiveMessage<TMessage>() where TMessage : Message;
        TAnswer Request<TAnswer>(Message Request) where TAnswer : Message;
    }
}
