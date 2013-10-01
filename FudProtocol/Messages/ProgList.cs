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
        private List<DevFileInfo> listDevFileInfo;
        public List<DevFileInfo> ListDevFileInfo
        {
            get { return listDevFileInfo; }
            set { ;}
        }
        byte[] buff;
        public byte[] Buff
        {
            get { return buff; }
            set { ;}
        }

        public override byte[] Encode()
        {            
            int disOffset = 1;
            buff = new byte[4000];
            buff[0] = MessageIdentifer;
            for (int i = 0; i < listDevFileInfo.Count; i++)
            {
                buff[i + disOffset] = (byte)listDevFileInfo[i].FileSize;
                Buffer.BlockCopy(Encoding.GetEncoding(1251).GetBytes(listDevFileInfo[i].FileName), 0, buff, disOffset+1, listDevFileInfo[i].FileSize);
                disOffset += listDevFileInfo[i].FileSize + 1;
            }
            throw new NotImplementedException();
        }
        /// <summary>
        /// Декодирование ответного сообщения
        /// </summary>
        /// <param name="Data">Принятый массив байт</param>
        protected override void Decode(byte[] Data)
        {
            listDevFileInfo = new List<DevFileInfo>();
            byte[] bData = new byte[Data.Length - 1];
            Buffer.BlockCopy(Data, 1, bData, 0, Data.Length - 1);
            int FileNameSize = 0;
            for (int i = 0; i < bData.Length; i += 9 + FileNameSize)
            {
                FileNameSize = (int)bData[i];
                byte[] bFileName = new byte[FileNameSize];
                byte[] bFileSize = new byte[intSize];
                byte[] bControlSum = new byte[intSize];
                Buffer.BlockCopy(bData, 1 + i, bFileName, 0, FileNameSize);
                Buffer.BlockCopy(bData, 1 + FileNameSize + i, bFileSize, 0, intSize);
                Buffer.BlockCopy(bData, 5 + FileNameSize + i, bControlSum, 0, intSize);

                DevFileInfo FileInfo = new DevFileInfo();
                FileInfo.FileName = Encoding.GetEncoding(1251).GetString(bFileName);
                FileInfo.FileSize = BitConverter.ToInt32(bFileSize,0);
                FileInfo.ControlSum = BitConverter.ToUInt16(bControlSum,0);
                                
                listDevFileInfo.Add(FileInfo);
            }
        }
    }
}
