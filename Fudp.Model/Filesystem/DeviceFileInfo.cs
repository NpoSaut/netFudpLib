using System;

namespace Fudp.Model.Filesystem
{
    /// <summary>Запись о файле на устройстве</summary>
    public class DeviceFileInfo
    {
        /// <summary>Создаёт экземпляр <see cref="DeviceFileInfo" /> с указанными свойствами.</summary>
        /// <param name="Path">Путь к файлу</param>
        /// <param name="Size">Размер файла</param>
        /// <param name="Checksum">Контрольная сумма</param>
        public DeviceFileInfo(string Path, int Size, ushort Checksum)
        {
            this.Checksum = Checksum;
            this.Size = Size;
            this.Path = Path;
        }

        /// <summary>Путь к файлу</summary>
        public String Path { get; private set; }

        /// <summary>Размер файла</summary>
        public int Size { get; private set; }

        /// <summary>Контрольная сумма файла</summary>
        public UInt16 Checksum { get; private set; }
    }
}
