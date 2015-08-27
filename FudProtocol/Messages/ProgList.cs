using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Fudp.Messages
{
    [Identifer(0x04)]
    public class ProgList : Message
    {
        /// <summary>Список файлов</summary>
        public List<DevFileListNode> Files { get; set; }

        public static int CalcSizeOfHeader() { return 1; }

        public static int CalcSizeFor(DevFileListNode node)
        {
            if (node is DevFileInfo)
            {
                var file = (DevFileInfo)node;
                return 1 + file.FileName.Length + 4 + 4;
            }
            if (node is DevFileListIncompleteTransactionFlag)
                return 9;
            return 0;
        }

        public override byte[] Encode()
        {
            using (var memory = new MemoryStream())
            {
                var writer = new BinaryWriter(memory, DefaultEncoding);
                writer.Write(MessageIdentifer);
                foreach (DevFileListNode fileNode in Files)
                {
                    if (fileNode is DevFileInfo)
                    {
                        var file = (DevFileInfo)fileNode;
                        writer.Write((Byte)file.FileName.Length);
                        writer.Write(file.FileName.ToCharArray());
                        writer.Write((UInt32)file.Size);
                        writer.Write((UInt32)file.ControlSum);
                    }
                    if (fileNode is DevFileListIncompleteTransactionFlag)
                    {
                        var file = (DevFileListIncompleteTransactionFlag)fileNode;
                        writer.Write((byte)0);
                        writer.Write(file.Remaining);
                        writer.Write((uint)0);
                    }
                }
                return memory.ToArray();
            }
        }

        /// <summary>Декодирование ответного сообщения</summary>
        /// <param name="Data">Принятый массив байт</param>
        protected override void Decode(byte[] Data)
        {
            Files = new List<DevFileListNode>();

            var dataStream = new MemoryStream(Data);
            dataStream.Seek(1, SeekOrigin.Begin);

            while (dataStream.Position < dataStream.Length)
                Files.Add(DecodeFileListItem(dataStream));
        }

        private DevFileListNode DecodeFileListItem(Stream DataStream)
        {
            var reader = new BinaryReader(DataStream);
            int fileNameLength = reader.ReadByte();
            if (fileNameLength > 0)
            {
                var fileNameBytes = new byte[fileNameLength];
                DataStream.Read(fileNameBytes, 0, fileNameLength);
                return new DevFileInfo(Encoding.GetEncoding(1251).GetString(fileNameBytes), reader.ReadInt32(), (UInt16)reader.ReadUInt32());
            }
            uint remaining = reader.ReadUInt32();
            DataStream.Seek(4, SeekOrigin.Current);
            return new DevFileListIncompleteTransactionFlag(remaining);
        }

        public override string ToString()
        {
            return string.Format("{0} ({1} файлов: {2}{3})", base.ToString(), Files.Count, string.Join(", ", Files.Select(f => f.ToString()).Take(5)),
                                 Files.Count > 5 ? " ..." : "");
        }
    }
}
