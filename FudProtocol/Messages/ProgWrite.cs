using System;
using System.IO;
using System.Text;

namespace Fudp.Messages
{
    [Identifer(0x0b)]
    public class ProgWrite : Message
    {
        public const int PacketSize = 4000;

        public ProgWrite() { }

        /// <summary>
        ///     Создаёт сообщение записи для указаного файла с указанным отступом на программируемом устройстве из буфера с
        ///     указанием отступа в буфере
        /// </summary>
        /// <param name="FileName">Имя файла</param>
        /// <param name="TargetOffset">Отступ в программируемом устройстве</param>
        /// <param name="DataOffset">Отступ в подаваемом буфере данных</param>
        /// <param name="DataBuffer">Буфер данных</param>
        /// <param name="MaxLength">Ограничение количества подаваемых данных</param>
        public ProgWrite(String FileName, int TargetOffset, int DataOffset, Byte[] DataBuffer, int MaxLength = Int32.MaxValue)
            : this()
        {
            int DataLength = Math.Min(Math.Min(PacketSize - GetHeaderLength(FileName), MaxLength), DataBuffer.Length - DataOffset);

            Data = new Byte[DataLength];
            Buffer.BlockCopy(DataBuffer, DataOffset, Data, 0, Data.Length);
            Offset = TargetOffset;
            this.FileName = FileName;
        }

        /// <summary>
        ///     Создаёт сообщение записи для указаного файла с указанным отступом, предпологая, что указанный буфер данных
        ///     равен буферу отправляемого файла
        /// </summary>
        /// <param name="FileName">Имя файла</param>
        /// <param name="Offset">Отступ в файле на программируемом устройстве и в передаваемом буфере данных</param>
        /// <param name="DataBuffer">Буфер данных</param>
        public ProgWrite(String FileName, int Offset, Byte[] DataBuffer)
            : this(FileName, Offset, 0, DataBuffer) { }

        /// <summary>Создаёт сообщение записи для указаного файла с указанным отступом</summary>
        /// <param name="File">Файл для отправки</param>
        /// <param name="Offset">Отступ</param>
        public ProgWrite(DevFile File, int Offset)
            : this(File.FileName, Offset, Offset, File.Data) { }

        /// <summary>Данные для отправки</summary>
        public byte[] Data { get; private set; }

        /// <summary>Имя файла</summary>
        public string FileName { get; private set; }

        /// <summary>Смещение от начала файла</summary>
        public int Offset { get; private set; }

        /// <summary>Вычисляет длину заголовка</summary>
        private int GetHeaderLength(String FileName)
        {
            const int StaticHeaderLength = 6;
            return FileName.Length + StaticHeaderLength;
        }

        /// <summary>Кодирование сообщения</summary>
        /// <returns></returns>
        public override byte[] Encode()
        {
            var ms = new MemoryStream(PacketSize);
            using (var w = new BinaryWriter(ms, DefaultEncoding))
            {
                w.Write(MessageIdentifer);
                w.Write((byte)FileName.Length);
                w.Write(FileName.ToCharArray());
                w.Write(Offset);
                w.Write(Data);
            }
            return ms.ToArray();
        }

        protected override void Decode(byte[] Data)
        {
            var ms = new MemoryStream(Data);
            using (var r = new BinaryReader(ms, Encoding.GetEncoding(1251)))
            {
                r.ReadByte();
                int FileNameLength = r.ReadByte();
                FileName = new String(r.ReadChars(FileNameLength));
                Offset = r.ReadInt32();
                this.Data = r.ReadBytes((int)(r.BaseStream.Length - r.BaseStream.Position));
            }
        }

        public override string ToString() { return string.Format("{0} [ {1} : {2} ]", base.ToString(), FileName, Offset); }
    }
}
