using System;
using System.Collections.Generic;
using System.Threading;
using Fudp.Messages;

namespace Fudp
{
    /// <summary>Сессия удалённого программирования</summary>
    public interface IProgSession : IDisposable
    {
        /// <summary>Билет устройства, с которым установлена сессия</summary>
        DeviceTicket Device { get; }

        #region Files

        /// <summary>Запрашивает список файлов на устройстве</summary>
        /// <param name="CancellationToken"></param>
        IList<DevFileInfo> ListFiles(CancellationToken CancellationToken);

        /// <summary>Производит чтение содержимого файла</summary>
        /// <param name="File">Файл для чтения</param>
        /// <param name="ProgressAcceptor">Прогресс</param>
        /// <param name="CancellationToken">Токен отмена операции</param>
        Byte[] ReadFile(DevFileInfo File, IProgressAcceptor ProgressAcceptor, CancellationToken CancellationToken);

        /// <summary>Удаляет указанный файл с устройства</summary>
        /// <param name="FileName">Путь к файлу для удаления</param>
        /// <param name="CancellationToken"></param>
        void DeleteFile(string FileName, CancellationToken CancellationToken);

        /// <summary>Команда на создание файла</summary>
        /// <param name="fileInfo">Информация о создаваемом файле</param>
        /// <param name="CancellationToken">Токен отмены</param>
        /// <param name="ProgressAcceptor">Приёмник прогресса выполнения файла</param>
        /// <returns></returns>
        void CreateFile(DevFile fileInfo, CancellationToken CancellationToken, IProgressAcceptor ProgressAcceptor = null);

        #endregion

        #region Properties

        /// <summary>Команда на создание или изменение записи в словаре свойств</summary>
        /// <param name="paramKey">Ключ</param>
        /// <param name="paramValue">Значение свойства</param>
        /// <param name="CancellationToken"></param>
        void SetProperty(byte paramKey, int paramValue, CancellationToken CancellationToken);

        /// <summary>Удаление записи из словаря свойств</summary>
        /// <param name="paramKey">Ключ</param>
        /// <param name="CancellationToken"></param>
        void DeleteProperty(byte paramKey, CancellationToken CancellationToken);

        #endregion

        /// <summary>Отправляет запрос на завершение сеанса программирования</summary>
        SubmitAckStatus Submit(SubmitStatus Status, CancellationToken CancellationToken);
    }
}
