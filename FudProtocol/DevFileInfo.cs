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
        public ushort ControlSum { get; set; }
        /// <summary>
        /// Данные
        /// </summary>
        public byte[] Data { get; set; }

        public override string ToString()
        {
            return string.Format("{0} ({1} Б)", FileName, FileSize);
        }
    }
}
