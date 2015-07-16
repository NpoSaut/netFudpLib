using Communications;
using Fudp.Messages;

namespace Fudp
{
    /// <summary>FUDP-порт</summary>
    /// <remarks>Позволяет принимать и отправлять сообщения по протоколу FUDP</remarks>
    public interface IFudpPort : IPort<Message, FudpPortOptions> { }
}
