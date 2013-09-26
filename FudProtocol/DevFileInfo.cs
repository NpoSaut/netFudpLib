using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fudp
{
    public class DevFileInfo
    {
        public DevFileInfo()
        { }
        /// <summary>
        /// Имя файла
        /// </summary>
        private string fileName;
        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }
        /// <summary>
        /// Размер файла
        /// </summary>
        private int fileSize;
        public int FileSize
        {
            get { return fileSize; }
            set { fileSize = value; }
        }
        /// <summary>
        /// Контрольня сумма файла
        /// </summary>
        private ushort controlSum;
        public ushort ControlSum
        {
            get { return controlSum; }
            set { controlSum = value; }
        }
        /// <summary>
        /// Данные
        /// </summary>
        private byte[] data;
        public byte[] Data
        {
            get { return data; }
            set { data = value; }
        }
        public ushort CalcCrc()
        {
            ushort crc = 0xffff;
            for (int i = 0; i < data.Length; i++)
            {
                crc = crc_ccitt(crc, data[i]);
            }
            return crc;
        }
        private ushort crc_ccitt(ushort crc, byte cdata)
        {
            byte b = 0xff;
            cdata ^= (byte)(crc & b);
            cdata ^= (byte)(cdata << 4);

            return (ushort)(((((ushort)cdata << 8)) | ((crc >> 8))) ^
                (ushort)(cdata >> 4) ^
                ((ushort)cdata << 3));
        }
    }
}
