using Fudp.Protocol.Messages;

namespace Fudp.Operators
{
    public interface IFudpConnection
    {
        void SendMessage(Message Msg);
        TMessage ReceiveMessage<TMessage>() where TMessage : Message;
        TMessage Request<TMessage>(Message Request) where TMessage : Message;
    }
}