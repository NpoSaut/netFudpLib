using System.Collections.Generic;
using System.IO;

namespace Fudp.Model
{
    /// <summary>Файловый FUDP-оператор</summary>
    public interface IFileOperator
    {
        /// <summary>Перечисляет все файлы на устройстве</summary>
        IEnumerable<DeviceFileInfo> EnumerateFiles();

        /// <summary>Создаёт файл на устройстве</summary>
        /// <param name="File">Информация о файле</param>
        void CreateFile(DeviceFileInfo File);

        /// <summary>Открывает файл для чтения или записи</summary>
        /// <param name="File">Информация о файле</param>
        /// <returns>Stream-оболочка для чтения или записи в файл</returns>
        Stream OpenFile(DeviceFileInfo File);

        /// <summary>Удаляет файл</summary>
        /// <param name="File">Файл для удаления</param>
        void DeleteFile(DeviceFileInfo File);
    }
}
