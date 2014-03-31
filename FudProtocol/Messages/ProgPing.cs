using System;

namespace Fudp.Messages
{
    /// <summary>Сообщение PROG_PONG для поддержания соединения с загрузчиком</summary>
    [Identifer(0x15)]
    public class ProgPing : Message
    {
        public ProgPing() { }
        public ProgPing(Byte Counter) : this() { this.Counter = Counter; }

        /// <summary>Номер запроса</summary>
        public Byte Counter { get; private set; }

        protected override void Decode(byte[] Data) { Counter = Data[1]; }
        public override byte[] Encode() { return new[] { MessageIdentifer, Counter }; }
    }
}
