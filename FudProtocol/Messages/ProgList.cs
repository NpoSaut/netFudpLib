using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

namespace Fudp.Messages
{
    [Identifer(0x04)]
    public class ProgList : Message
    {
        public ProgList()
        { }
        /// <summary>
        /// Список файлов
        /// </summary>
        public List<DevFileListNode> Files { get; private set; }

        public byte[] Buff { get; private set; }

        public override byte[] Encode()
        {            
//            int disOffset = 1;
//            Buff = new byte[4000];
//            Buff[0] = MessageIdentifer;
//            for (int i = 0; i < Files.Count; i++)
//            {
//                Buff[i + disOffset] = (byte)Files[i].FileSize;
//                Buffer.BlockCopy(Encoding.GetEncoding(1251).GetBytes(Files[i].FileName), 0, Buff, disOffset+1, Files[i].FileSize);
//                disOffset += Files[i].FileSize + 1;
//            }
            throw new NotImplementedException();
        }
        /// <summary>
        /// Декодирование ответного сообщения
        /// </summary>
        /// <param name="Data">Принятый массив байт</param>
        protected override void Decode(byte[] Data)
        {
            Files = new List<DevFileListNode>();

            var dataStream = new MemoryStream(Data);
            dataStream.Seek(1, SeekOrigin.Begin);

            while (dataStream.Position < dataStream.Length)
            {
                Files.Add(DecodeFileListItem(dataStream));                                  
            }
        }

        private DevFileListNode DecodeFileListItem(Stream DataStream)
        {
            var reader = new BinaryReader(DataStream);
            int fileNameLength = reader.ReadByte();
            if (fileNameLength > 0)
            {
                var fileNameBytes = new byte[fileNameLength];
                DataStream.Read(fileNameBytes, 0, fileNameLength);
                return new DevFileInfo(
                    Name: Encoding.GetEncoding(1251).GetString(fileNameBytes),
                    Size: reader.ReadInt32(),
                    Checksum: (UInt16)reader.ReadUInt32());
            }
            else
            {
                var remaining = reader.ReadUInt32();
                DataStream.Seek(4, SeekOrigin.Current);
                return new DevFileListIncompleteTransactionFlag(remaining);
            }
        }
    }
}
