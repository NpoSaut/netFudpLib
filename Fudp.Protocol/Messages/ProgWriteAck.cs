using System.IO;

namespace Fudp.Protocol.Messages
{
    /// <summary>
    /// Подтверждение записи
    /// </summary>
    [Identifer(0x0c)]
    public class ProgWriteAck : Message
    {
        public enum WriteStatusKind { OK, OutOfFile, Unknown }
        
        /// <summary>Статус записи</summary>
        public WriteStatusKind Status { get; private set; }

        public ProgWriteAck() : base() { }
        public ProgWriteAck(WriteStatusKind Status) : this()
        {
            this.Status = Status;
        }

        protected override void Decode(byte[] Data)
        {
            Status = (WriteStatusKind) Data[1];
        }

        public override byte[] Encode()
        {
            var ms = new MemoryStream();
            using (var wr = new BinaryWriter(ms))
            {
                ms.WriteByte(MessageIdentifer);
                ms.WriteByte((byte)Status);
            }
            return ms.ToArray();
        }
    }
}
