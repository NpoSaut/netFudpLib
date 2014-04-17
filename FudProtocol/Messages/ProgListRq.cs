using System.IO;

namespace Fudp.Messages
{
    [Identifer(0x03)]
    internal class ProgListRq : Message
    {
        /// <summary>Запроса списка фалов</summary>
        public ProgListRq(ushort Offset = 0, ushort Count = 0)
        {
            this.Offset = Offset;
            this.Count = Count;
        }

        public ushort Offset { get; private set; }
        public ushort Count { get; private set; }

        /// <summary>Кодирование сообщения</summary>
        /// <returns></returns>
        public override byte[] Encode()
        {
            using (var ms = new MemoryStream())
            {
                var bw = new BinaryWriter(ms);
                bw.Write(MessageIdentifer);
                bw.Write(Offset);
                bw.Write(Count);
                return ms.ToArray();
            }
        }

        protected override void Decode(byte[] Data) { }
    }
}
