using System;
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
            var buff = new Byte[10 + FileInfo.FileName.Length];
            buff[0] = MessageIdentifer; //Идентификатор сообщения
            buff[1] = (byte)FileInfo.FileName.Length;
            Buffer.BlockCopy(Encoding.GetEncoding(1251).GetBytes(FileInfo.FileName), 0, buff, 2, FileInfo.FileName.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(FileInfo.Size), 0, buff, 2 + FileInfo.FileName.Length, intSize);
            Buffer.BlockCopy(BitConverter.GetBytes(FileInfo.ControlSum), 0, buff, 6 + FileInfo.FileName.Length, intSize);
            return buff;
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
