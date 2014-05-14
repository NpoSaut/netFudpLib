using System;
using System.Collections.Generic;
using System.IO;
using Fudp.Model;
using Fudp.Protocol;
using Fudp.Protocol.Exceptions;
using Fudp.Protocol.Messages;

namespace Fudp.Operators
{
    /// <summary>Манипулирует файлами через FUDP-протокол</summary>
    public class FudpFileOperator : IFileOperator
    {
        /// <summary>Создаёт FUDP-файловый оператор</summary>
        /// <param name="Connection">FUDP-соединение</param>
        public FudpFileOperator(IFudpConnection Connection) { this.Connection = Connection; }

        /// <summary>FUDP-соединение</summary>
        public IFudpConnection Connection { get; private set; }

        /// <summary>Перечисляет все файлы на устройстве</summary>
        public IEnumerable<DeviceFileInfo> EnumerateFiles() { return RequestFiles(); }

        /// <summary>Создаёт файл на устройстве</summary>
        /// <param name="File">Информация о файле</param>
        public void CreateFile(DeviceFileInfo File)
        {
            var create = new ProgCreate(File.Path, File.Size, File.Checksum);

            var createAck = Connection.Request<ProgCreateAck>(create);
            if (createAck.ErrorCode == 0) return;
            switch (createAck.ErrorCode)
            {
                case 1:
                    throw new CanProgFileAlreadyExistsException(createAck.ErrorMsg[createAck.ErrorCode]);
                case 2:
                    throw new CanProgMaximumFilesCountAchivedException(createAck.ErrorMsg[createAck.ErrorCode]);
                case 3:
                    throw new CanProgMemoryIsOutException(createAck.ErrorMsg[createAck.ErrorCode]);
                case 4:
                    throw new CanProgCreateException(createAck.ErrorMsg[createAck.ErrorCode]);
                default:
                    throw new CanProgException();
            }
        }

        /// <summary>Открывает файл для чтения или записи</summary>
        /// <param name="File">Информация о файле</param>
        /// <returns>Stream-оболочка для чтения или записи в файл</returns>
        public Stream OpenFile(DeviceFileInfo File) { throw new NotImplementedException(); }

        /// <summary>Удаляет файл</summary>
        /// <param name="File">Файл для удаления</param>
        public void DeleteFile(DeviceFileInfo File)
        {
            var removeRequest = new ProgRm(File.Path);
            var removeResponse = Connection.Request<ProgRmAck>(removeRequest);
        }

        /// <summary>Запрашивает порцию файлов с устройства</summary>
        /// <param name="Offset">Отступ от начала списка файлов</param>
        private IEnumerable<DeviceFileInfo> RequestFiles(int Offset = 0)
        {
            var listRq = new ProgListRq((ushort)Offset);
            int counter = 0;
            foreach (DevFileListNode file in Connection.Request<ProgList>(listRq).Files)
            {
                if (file is DevFileInfo)
                {
                    counter++;
                    var f = (DevFileInfo)file;
                    yield return new DeviceFileInfo(f.FileName, f.FileSize, f.ControlSum);
                }
                else
                {
                    IEnumerable<DeviceFileInfo> appendix = RequestFiles(Offset + counter);
                    foreach (DeviceFileInfo subfile in appendix) yield return subfile;
                }
            }
        }
    }
}
