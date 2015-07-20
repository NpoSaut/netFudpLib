using System;
using System.Collections.Generic;
using System.Threading;
using Fudp.Messages;

namespace Fudp
{
    internal class DisposeProgSessionDecorator : IProgSession
    {
        private readonly IProgSession _core;
        private readonly ICollection<IDisposable> _itemsToDispose;

        public DisposeProgSessionDecorator(IProgSession Core, params IDisposable[] ItemsToDispose)
            : this(Core, (ICollection<IDisposable>)ItemsToDispose) { }

        public DisposeProgSessionDecorator(IProgSession Core, ICollection<IDisposable> ItemsToDispose)
        {
            _core = Core;
            _itemsToDispose = ItemsToDispose;
        }

        /// <summary>
        ///     Выполняет определяемые приложением задачи, связанные с удалением, высвобождением или сбросом неуправляемых
        ///     ресурсов.
        /// </summary>
        public void Dispose()
        {
            _core.Dispose();
            foreach (IDisposable disposableItem in _itemsToDispose)
                disposableItem.Dispose();
        }

        /// <summary>Билет устройства, с которым установлена сессия</summary>
        public DeviceTicket Device
        {
            get { return _core.Device; }
        }

        /// <summary>Запрашивает список файлов на устройстве</summary>
        public IList<DevFileInfo> ListFiles()
        {
            return _core.ListFiles();
        }

        /// <summary>Производит чтение содержимого файла</summary>
        /// <param name="File">Файл для чтения</param>
        /// <param name="ProgressAcceptor">Прогресс</param>
        /// <param name="CancellationToken">Токен отмена операции</param>
        public byte[] ReadFile(DevFileInfo File, IProgressAcceptor ProgressAcceptor, CancellationToken CancellationToken)
        {
            return _core.ReadFile(File, ProgressAcceptor, CancellationToken);
        }

        /// <summary>Удаляет указанный файл с устройства</summary>
        /// <param name="FileName">Путь к файлу для удаления</param>
        public void DeleteFile(string FileName)
        {
            _core.DeleteFile(FileName);
        }

        /// <summary>Команда на создание файла</summary>
        /// <param name="fileInfo">Информация о создаваемом файле</param>
        /// <param name="ProgressAcceptor">Приёмник прогресса выполнения файла</param>
        /// <param name="CancelToken">Токен отмены</param>
        /// <returns></returns>
        public void CreateFile(DevFileInfo fileInfo, IProgressAcceptor ProgressAcceptor = null, CancellationToken CancelToken = new CancellationToken())
        {
            _core.CreateFile(fileInfo, ProgressAcceptor, CancelToken);
        }

        /// <summary>Команда на создание или изменение записи в словаре свойств</summary>
        /// <param name="paramKey">Ключ</param>
        /// <param name="paramValue">Значение свойства</param>
        public void SetProperty(byte paramKey, int paramValue)
        {
            _core.SetProperty(paramKey, paramValue);
        }

        /// <summary>Удаление записи из словаря свойств</summary>
        /// <param name="paramKey">Ключ</param>
        public void DeleteProperty(byte paramKey)
        {
            _core.DeleteProperty(paramKey);
        }

        /// <summary>Отправляет запрос на завершение сеанса программирования</summary>
        public SubmitAckStatus Submit(SubmitStatus Status)
        {
            return _core.Submit(Status);
        }
    }
}
