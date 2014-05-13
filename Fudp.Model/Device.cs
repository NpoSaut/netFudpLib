using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Fudp.Model
{
    /// <summary>Модель FUDP-устройства</summary>
    public class Device
    {
        private readonly List<DeviceFileInfo> _files = new List<DeviceFileInfo>();
        public Device() { Files = new ReadOnlyCollection<DeviceFileInfo>(_files); }

        /// <summary>Список файлов на устройстве</summary>
        public ReadOnlyCollection<DeviceFileInfo> Files { get; private set; }

        /// <summary>Создание нового файла</summary>
        public event EventHandler<DeviceFileEventArgs> FileCreated;

        /// <summary>Удаление файла</summary>
        public event EventHandler<DeviceFileEventArgs> FileDeleted;

        /// <summary>Добавляет файл в коллекцию и вызывает необходимые события</summary>
        /// <param name="File">Новый файл</param>
        protected void AddFileToCollection(DeviceFileInfo File)
        {
            _files.Add(File);
            OnFileCreated(new DeviceFileEventArgs(File));
        }

        /// <summary>Удаляет файл из коллекции и вызывает необходимые события</summary>
        /// <param name="File">Удаляемый файл</param>
        protected void RemoveFileFromCollection(DeviceFileInfo File)
        {
            _files.Remove(File);
            OnFileDeleted(new DeviceFileEventArgs(File));
        }

        /// <summary>Вызывает событие создания нового файла</summary>
        protected void OnFileCreated(DeviceFileEventArgs e)
        {
            EventHandler<DeviceFileEventArgs> handler = FileCreated;
            if (handler != null) handler(this, e);
        }

        /// <summary>Вызывает событие удаления файла</summary>
        protected void OnFileDeleted(DeviceFileEventArgs e)
        {
            EventHandler<DeviceFileEventArgs> handler = FileDeleted;
            if (handler != null) handler(this, e);
        }
    }
}
