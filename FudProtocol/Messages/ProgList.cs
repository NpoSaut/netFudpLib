using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fudp.Messages
{
    [Identifer(0x04)]
    class ProgList : Message
    {
        public ProgList()
        { }
        /// <summary>
        /// Список файлов
        /// </summary>
        public List<DevFileInfo> ListDevFileInfo { get; private set; }

        public byte[] Buff { get; private set; }

        public override byte[] Encode()
        {            
            int disOffset = 1;
            Buff = new byte[4000];
            Buff[0] = MessageIdentifer;
            for (int i = 0; i < ListDevFileInfo.Count; i++)
            {
                Buff[i + disOffset] = (byte)ListDevFileInfo[i].FileSize;
                Buffer.BlockCopy(Encoding.GetEncoding(1251).GetBytes(ListDevFileInfo[i].FileName), 0, Buff, disOffset+1, ListDevFileInfo[i].FileSize);
                disOffset += ListDevFileInfo[i].FileSize + 1;
            }
            throw new NotImplementedException();
        }
        /// <summary>
        /// Декодирование ответного сообщения
        /// </summary>
        /// <param name="Data">Принятый массив байт</param>
        protected override void Decode(byte[] Data)
        {
            ListDevFileInfo = new List<DevFileInfo>();
            var bData = new byte[Data.Length - 1];
            Buffer.BlockCopy(Data, 1, bData, 0, Data.Length - 1);
            int fileNameSize = 0;
            for (int i = 0; i < bData.Length; i += 9 + fileNameSize)
            {
                fileNameSize = (int)bData[i];
                var bFileName = new byte[fileNameSize];
                var bFileSize = new byte[intSize];
                var bControlSum = new byte[intSize];
                Buffer.BlockCopy(bData, 1 + i, bFileName, 0, fileNameSize);
                Buffer.BlockCopy(bData, 1 + fileNameSize + i, bFileSize, 0, intSize);
                Buffer.BlockCopy(bData, 5 + fileNameSize + i, bControlSum, 0, intSize);

                var fileInfo = new DevFileInfo(
                    Name: Encoding.GetEncoding(1251).GetString(bFileName),
                    Size: BitConverter.ToInt32(bFileSize,0),
                    Checksum: BitConverter.ToUInt16(bControlSum,0));
                                
                ListDevFileInfo.Add(fileInfo);
            }
        }
    }
}
