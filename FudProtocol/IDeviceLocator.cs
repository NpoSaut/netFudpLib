using System.Collections.Generic;
using Communications.Can;

namespace Fudp
{
    public interface IDeviceLocator
    {
        /// <summary>Находит в сети все устройства с заданным шаблоном билетов.</summary>
        /// <param name="Template">Шаблон билета устройства</param>
        /// <param name="OnPort">Can-порт, через который осуществляется работа</param>
        /// <param name="Timeout">
        ///     Таймаут (в милисекундах). Таймаут отсчитывается с момента получения последней IsoTP-транзакции, а
        ///     не с момента начала опроса
        /// </param>
        /// <returns></returns>
        List<DeviceTicket> LocateDevices(DeviceTicket Template, CanPort OnPort, int Timeout = 100);
    }
}
