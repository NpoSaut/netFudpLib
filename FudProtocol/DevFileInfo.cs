using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fudp
{
    public class DevFileInfo
    {
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

        /// <summary>
        /// Создаёт образ файла на устройстве с указанными размером и контрольной суммой
        /// </summary>
        /// <param name="Name">Имя файла</param>
        /// <param name="Size">Размер файла</param>
        /// <param name="Checksum">Контрольная сумма</param>
        public DevFileInfo(String Name, int Size, UInt16 Checksum)
        {
            this.FileName = Name;
            this.FileSize = Size;
            this.ControlSum = Checksum;
            this.Data = null;
        }
        /// <summary>
        /// Представляет файл на устройстве
        /// </summary>
        /// <param name="Name">Имя файла</param>
        /// <param name="Data">Данные файла</param>
        public DevFileInfo(String Name, Byte[] Data)
        {
            this.FileName = Name;
            this.Data = Data;
            this.FileSize = Data.Length;
            this.ControlSum = FudpCrc.CalcCrc(Data);
        }

        public override string ToString()
        {
            return string.Format("{0} ({1} Б)", FileName, FileSize);
        }
    }
}
