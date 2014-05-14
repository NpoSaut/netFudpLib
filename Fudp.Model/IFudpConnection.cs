using Fudp.Protocol.Messages;

namespace Fudp.Model
{
    public interface IFudpConnection
    {
        void SendMessage(Message Msg);
        TMessage ReceiveMessage<TMessage>();
        TMessage Request<TMessage>(Message Request);
    }
}