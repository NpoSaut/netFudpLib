using System;
using System.IO;
using System.Text;

namespace Fudp.Messages
{
    [Identifer(0x09)]
    public class ProgCreate : Message
    {
        [Obsolete("Только для внутренних нужд")]
        public ProgCreate() { }

        /// <summary>Команда на создание файла</summary>
        public ProgCreate(DevFileInfo FileInfo) { this.FileInfo = FileInfo; }

        public DevFileInfo FileInfo { get; private set; }

        /// <summary>Кодирование сообщения</summary>
        /// <returns></returns>
        public override byte[] Encode()
        {
            byte[] fileNameBytes = Encoding.GetEncoding(1251).GetBytes(FileInfo.FileName);

            var ms = new MemoryStream();
            var w = new BinaryWriter(ms);
            w.Write(MessageIdentifer);
            w.Write((byte)fileNameBytes.Length);
            w.Write(fileNameBytes);
            w.Write((uint)FileInfo.Size);
            w.Write((uint)FileInfo.ControlSum);
            return ms.ToArray();
        }

        protected override void Decode(byte[] Data)
        {
            byte fileNameLength = Data[1];
            var filename = new byte[fileNameLength];
            Buffer.BlockCopy(Data, 2, filename, 0, Data[1]);

            FileInfo = new DevFileInfo(
                Encoding.GetEncoding(1251).GetString(filename),
                BitConverter.ToInt32(Data, 2 + fileNameLength),
                (ushort)BitConverter.ToInt32(Data, 6 + fileNameLength)
                );
        }

        public override string ToString() { return string.Format("{0} {1}", base.ToString(), FileInfo.FileName); }
    }
}
