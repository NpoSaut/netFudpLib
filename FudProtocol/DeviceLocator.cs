using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Communications.Can;
using Communications.Protocols.IsoTP;

namespace Fudp
{
    /// <summary>
    /// Содержит методы для поиска прошиваемых устройств в сети
    /// </summary>
    public class DeviceLocator
    {
        public CanPort Port { get; set; }

        public DeviceLocator(CanPort OnPort)
        {
            this.Port = OnPort;
        }

        /// <summary>
        /// Находит в сети все устройства с заданным шаблоном билетов.
        /// </summary>
        /// <param name="Template">Шаблон билета устройства</param>
        /// <param name="Timeout">Таймаут (в милисекундах). Таймаут отсчитываетс с момента получения последней IsoTP-транзакции, а не с момента начала опроса</param>
        public List<DeviceTicket> LocateDevices(DeviceTicket Template, int Timeout = 500)
        {
            return LocateDevices(Template, Port, Timeout);
        }

        /// <summary>
        /// Находит в сети все устройства с заданным шаблоном билетов.
        /// </summary>
        /// <param name="Template">Шаблон билета устройства</param>
        /// <param name="OnPort">Can-порт, через который осуществляется работа</param>
        /// <param name="Timeout">Таймаут (в милисекундах). Таймаут отсчитываетс с момента получения последней IsoTP-транзакции, а не с момента начала опроса</param>
        /// <returns></returns>
        public static List<DeviceTicket> LocateDevices(DeviceTicket Template, CanPort OnPort, int Timeout = 500)
        {
            using (var flow = new CanFlow(OnPort, CanProg.FuDev, CanProg.FuInit, CanProg.FuProg))
            {
                if (Template.BlockSerialNumber != 0) Template.BlockSerialNumber = 0;

                var HelloMessage = new Messages.ProgInit(Template);
                IsoTp.Send(flow, CanProg.FuInit, CanProg.FuDev, HelloMessage.Encode());

                var res = new List<DeviceTicket>();
                while (true)
                {
                    try
                    {
                        var tr = IsoTp.Receive(flow, CanProg.FuDev, CanProg.FuProg, TimeSpan.FromMilliseconds(Timeout));
                        var msg = Messages.Message.DecodeMessage(tr.Data);
                        if (msg is Messages.ProgBroadcastAnswer) res.Add((msg as Messages.ProgBroadcastAnswer).Ticket);
                    }
                    catch (TimeoutException)
                    {
                        break;
                    }
                }
                return res.Distinct().ToList();
            }
        }
    }
}
