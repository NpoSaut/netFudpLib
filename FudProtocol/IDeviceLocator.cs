using System;
using System.Collections.Generic;
using System.Threading;
using Communications.Can;

namespace Fudp
{
    public interface IDeviceLocator
    {
        /// <summary>Находит в сети все устройства с заданным шаблоном билетов.</summary>
        /// <param name="Template">Шаблон билета устройства</param>
        /// <param name="Port">Can-порт, через который осуществляется работа</param>
        /// <param name="Timeout">Таймаут (в милисекундах)</param>
        /// <param name="CancellationToken">Токен отмены операции</param>
        IList<DeviceTicket> LocateDevices(DeviceTicket Template, ICanPort Port, TimeSpan Timeout, CancellationToken CancellationToken);
    }
}
