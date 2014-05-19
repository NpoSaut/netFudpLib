using System.Collections.Generic;
using Fudp.Protocol;

namespace Fudp.Model
{
    /// <summary>Объект, выполняющий поиск всех доступных устройств</summary>
    public interface IDeviceLocator
    {
        /// <summary>Выполняет поиск устройств по заданному шаблону</summary>
        /// <param name="Pattern">Шаблон для поиска устройств</param>
        /// <param name="Timeout">Таймаут ожидания ответа от устройств</param>
        /// <returns>Список устройсв, удовлетворяющих условиям поиска</returns>
        IList<DeviceTicket> LocateDevices(DeviceTicket Pattern, int Timeout = 100);
    }
}
