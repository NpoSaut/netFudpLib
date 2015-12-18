using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading;
using Communications.Can;
using Communications.PortHelpers;
using Communications.Protocols.IsoTP;
using Fudp.Messages;

namespace Fudp
{
    /// <summary>Содержит методы для поиска прошиваемых устройств в сети</summary>
    public class DeviceLocator : IDeviceLocator
    {
        /// <summary>Находит в сети все устройства с заданным шаблоном билетов.</summary>
        /// <param name="Template">Шаблон билета устройства</param>
        /// <param name="Port">Can-порт, через который осуществляется работа</param>
        /// <param name="Timeout">Таймаут (в милисекундах)</param>
        /// <param name="CancellationToken">Токен отмены операции</param>
        public IList<DeviceTicket> LocateDevices(DeviceTicket Template, ICanPort Port, TimeSpan Timeout, CancellationToken CancellationToken)
        {
            using (IIsoTpConnection connection = Port.OpenIsoTpConnection(CanProg.FuInit, CanProg.FuDev, new IsoTpConnectionParameters()))
            {
                using (var fudpPort = new FudpPort(connection))
                {
                    return fudpPort.Request(new ProgInit(Template),
                                            TimeSpan.FromMilliseconds(System.Threading.Timeout.Infinite),
                                            TimeSpan.FromMilliseconds(System.Threading.Timeout.Infinite),
                                            CancellationToken,
                                            flow => flow.OfType<ProgBCastResponse>()
                                                        .Select(resp => resp.Ticket)
                                                        .Take(Timeout)
                                                        .Distinct()
                                                        .ToList()
                                                        .First());
                }
            }
        }
    }
}
