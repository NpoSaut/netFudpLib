using Communications;
using Communications.Options;
using Fudp.Messages;

namespace Fudp
{
    /// <summary>Опции FUDP-порта</summary>
    public class FudpPortOptions : PortOptions<Message>
    {
        /// <summary>Создаёт новые опции порта без поддержки Loopback</summary>
        /// <param name="LowerLayerFrameCapacity">Максимальная вместимость пакета низлежащего уровня</param>
        public FudpPortOptions(int LowerLayerFrameCapacity) { this.LowerLayerFrameCapacity = LowerLayerFrameCapacity; }

        /// <summary>Максимальная вместимость пакета низлежащего уровня</summary>
        public int LowerLayerFrameCapacity { get; private set; }
    }
}
