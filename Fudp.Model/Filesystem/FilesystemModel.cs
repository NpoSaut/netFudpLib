using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Fudp.Model.Filesystem
{
    /// <summary>Модель файловой системы</summary>
    public class FilesystemModel : ICollection<DeviceFileInfo>
    {
        private readonly List<DeviceFileInfo> _files;

        public FilesystemModel(IFileOperator Operator)
        {
            this.Operator = Operator;
            _files = Operator.EnumerateFiles().ToList();
            Files = new ReadOnlyCollection<DeviceFileInfo>(_files);
        }

        protected IFileOperator Operator { get; private set; }

        /// <summary>Список файлов на устройстве</summary>
        public ReadOnlyCollection<DeviceFileInfo> Files { get; private set; }

        #region Реализация ICollection

        IEnumerator<DeviceFileInfo> IEnumerable<DeviceFileInfo>.GetEnumerator() { return _files.GetEnumerator(); }

        IEnumerator IEnumerable.GetEnumerator() { return _files.GetEnumerator(); }

        void ICollection<DeviceFileInfo>.Add(DeviceFileInfo item) { Create(item); }

        void ICollection<DeviceFileInfo>.Clear()
        {
            foreach (DeviceFileInfo file in _files.ToList())
                Delete(file);
        }

        bool ICollection<DeviceFileInfo>.Contains(DeviceFileInfo item) { return _files.Contains(item); }

        void ICollection<DeviceFileInfo>.CopyTo(DeviceFileInfo[] array, int arrayIndex) { _files.CopyTo(array, arrayIndex); }

        bool ICollection<DeviceFileInfo>.Remove(DeviceFileInfo item) { Delete(item); }

        int ICollection<DeviceFileInfo>.Count
        {
            get { return _files.Count; }
        }

        bool ICollection<DeviceFileInfo>.IsReadOnly
        {
            get { return false; }
        }

        #endregion

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
        private void OnFileCreated(DeviceFileEventArgs e)
        {
            EventHandler<DeviceFileEventArgs> handler = FileCreated;
            if (handler != null) handler(this, e);
        }

        /// <summary>Вызывает событие удаления файла</summary>
        private void OnFileDeleted(DeviceFileEventArgs e)
        {
            EventHandler<DeviceFileEventArgs> handler = FileDeleted;
            if (handler != null) handler(this, e);
        }

        /// <summary>Создаёт файл на устройстве</summary>
        /// <param name="File">Ссылка на создаваемый файл</param>
        /// <returns>Созданный файл</returns>
        public DeviceFileInfo Create(DeviceFileInfo File)
        {
            Operator.CreateFile(File);
            AddFileToCollection(File);
            return File;
        }

        /// <summary>Удалить файл</summary>
        /// <param name="File">Ссылка на файл</param>
        public void Delete(DeviceFileInfo File)
        {
            Operator.DeleteFile(File);
            RemoveFileFromCollection(File);
        }

        /// <summary>Открывает файл для чтения или записи</summary>
        /// <param name="File">Ссылка на файл</param>
        /// <returns></returns>
        public Stream OpenFile(DeviceFileInfo File) { return Operator.OpenFile(File); }
    }
}
