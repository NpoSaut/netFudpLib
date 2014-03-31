using System;

namespace Fudp.Messages
{
    /// <summary>Статус соединения с загрузчиком</summary>
    public enum PongStatus
    {
        /// <summary>Подключено, связь в порядке</summary>
        Connected = 0,

        /// <summary>Потеря одного пакета</summary>
        PacketLost = 1,

        /// <summary>Потеря нескольких пакетов, связь разорвана</summary>
        CounterError = 2
    }

    /// <summary>Сообщение PROG_PONG - ответ на сообщение поддержания связи</summary>
    [Identifer(0x16)]
    public class ProgPong : Message
    {
        public ProgPong() { }

        public ProgPong(byte Counter, PongStatus Status) : this()
        {
            this.Counter = Counter;
            this.Status = Status;
        }

        /// <summary>Номер запроса</summary>
        public Byte Counter { get; private set; }

        /// <summary>Статус соединения</summary>
        public PongStatus Status { get; private set; }

        protected override void Decode(byte[] Data)
        {
            Counter = Data[1];
            //Status = (PongStatus)Data[2];
        }

        public override byte[] Encode()
        {
            return new[]
                   {
                       MessageIdentifer,
                       Counter,
                       (Byte)Status
                   };
        }
    }
}
