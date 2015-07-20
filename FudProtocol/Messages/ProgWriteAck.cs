using System.IO;

namespace Fudp.Messages
{
    /// <summary>Подтверждение записи</summary>
    [Identifer(0x0c)]
    public class ProgWriteAck : Message
    {
        public enum WriteStatusKind
        {
            OK = 0,
            OutOfFile = 1,
            FileDoesNotExist = 2,
            Unknown = 255
        }

        public ProgWriteAck(WriteStatusKind Status = WriteStatusKind.OK) : this() { this.Status = Status; }

        /// <summary>Статус записи</summary>
        public WriteStatusKind Status { get; private set; }

        protected override void Decode(byte[] Data) { Status = (WriteStatusKind)Data[1]; }

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
