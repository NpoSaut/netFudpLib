using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Communications.Can;
using Communications.Protocols.IsoTP;
using Communications.Protocols.IsoTP.Exceptions;

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
        /// <param name="Timeout">Таймаут (в милисекундах). Таймаут отсчитывается с момента получения последней IsoTP-транзакции, а не с момента начала опроса</param>
        public List<DeviceTicket> LocateDevices(DeviceTicket Template, int Timeout = 100)
        {
            return LocateDevices(Template, Port, Timeout);
        }

        /// <summary>
        /// Находит в сети все устройства с заданным шаблоном билетов.
        /// </summary>
        /// <param name="Template">Шаблон билета устройства</param>
        /// <param name="OnPort">Can-порт, через который осуществляется работа</param>
        /// <param name="Timeout">Таймаут (в милисекундах). Таймаут отсчитывается с момента получения последней IsoTP-транзакции, а не с момента начала опроса</param>
        /// <returns></returns>
        public static List<DeviceTicket> LocateDevices(DeviceTicket Template, CanPort OnPort, int Timeout = 100)
        {
            using (var flow = new CanFlow(OnPort, CanProg.FuDev, CanProg.FuInit, CanProg.FuProg))
            {
                Template.BlockSerialNumber = 0;

                var helloMessage = new Messages.ProgInit(Template);
                IsoTp.Send(flow, CanProg.FuInit, CanProg.FuDev, helloMessage.Encode());

                var res = new List<DeviceTicket>();
                var sw = new Stopwatch();
                sw.Start();
                while (sw.ElapsedMilliseconds < Timeout)
                {
                    try
                    {
                        var tr = IsoTp.Receive(flow, CanProg.FuDev, CanProg.FuProg, TimeSpan.FromMilliseconds(Timeout - sw.ElapsedMilliseconds));
                        var msg = Messages.Message.DecodeMessage(tr.Data);
                        if (msg is Messages.ProgBCastResponse) res.Add((msg as Messages.ProgBCastResponse).Ticket);
                    }
                    catch (IsoTpReceiveTimeoutException)
                    {
                        break;
                    }
                }
                return res.Distinct().ToList();
            }
        }
    }
}
