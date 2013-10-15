using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Fudp.Messages
{
    [Identifer(0x0b)]
    public class ProgWrite : Message
    {
        public const int PacketSize = 4000;
        
        /// <summary>
        /// Данные для отправки
        /// </summary>
        public byte[] Data { get; private set; }
        
        /// <summary>
        /// Имя файла
        /// </summary>
        public string FileName { get; private set; }
        /// <summary>
        /// Смещение от начала файла
        /// </summary>
        public int Offset { get; private set; }

        /// <summary>
        /// Вычисляет длину заголовка
        /// </summary>
        private int GetHeaderLength(String FileName)
        {
            const int StaticHeaderLength = 6;
            return FileName.Length + StaticHeaderLength;
        }

        public ProgWrite()
        {}
        /// <summary>
        /// Создаёт сообщение записи для указаного файла с указанным отступом на программируемом устройстве из буфера с указанием отступа в буфере
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
            
            this.Data = new Byte[DataLength];
            Buffer.BlockCopy(DataBuffer, DataOffset, Data, 0, Data.Length);
            this.Offset = TargetOffset;
            this.FileName = FileName;
        }
        /// <summary>
        /// Создаёт сообщение записи для указаного файла с указанным отступом, предпологая, что указанный буфер данных равен буферу отправляемого файла
        /// </summary>
        /// <param name="FileName">Имя файла</param>
        /// <param name="Offset">Отступ в файле на программируемом устройстве и в передаваемом буфере данных</param>
        /// <param name="DataBuffer">Буфер данных</param>
        public ProgWrite(String FileName, int Offset, Byte[] DataBuffer)
            : this(FileName, Offset, Offset, DataBuffer)
        { }
        /// <summary>
        /// Создаёт сообщение записи для указаного файла с указанным отступом
        /// </summary>
        /// <param name="File">Файл для отправки</param>
        /// <param name="Offset">Отступ</param>
        public ProgWrite(DevFileInfo File, int Offset)
            : this(File.FileName, Offset, File.Data)
        { }


        /// <summary>
        /// Кодирование сообщения
        /// </summary>
        /// <returns></returns>
        public override byte[] Encode()
        {
            MemoryStream ms = new MemoryStream(PacketSize);
            using (BinaryWriter w = new BinaryWriter(ms, DefaultEncodung))
            {
                w.Write((byte)MessageIdentifer);
                w.Write((byte)FileName.Length);
                w.Write(FileName.ToCharArray());
                w.Write((int)Offset);
                w.Write(Data);
            }
            return ms.ToArray();
        }

        protected override void Decode(byte[] Data)
        {
            MemoryStream ms = new MemoryStream(Data);
            using (BinaryReader r = new BinaryReader(ms, Encoding.GetEncoding(1251)))
            {
                r.ReadByte();
                int FileNameLength = r.ReadByte();
                this.FileName = new String(r.ReadChars(FileNameLength));
                this.Offset = r.ReadInt32();
                this.Data = r.ReadBytes((int)(r.BaseStream.Length - r.BaseStream.Position));
            }
        }

    }
}
