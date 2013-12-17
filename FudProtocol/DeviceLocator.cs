using System;
using System.Collections.Generic;
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
        private ICanSocket Socket { get; set; }
        
        public DeviceLocator(ICanSocket Socket) { this.Socket = Socket; }

        /// <summary>
        /// Находит в сети все устройства с заданным шаблоном билетов.
        /// </summary>
        /// <param name="Template">Шаблон билета устройства</param>
        /// <param name="Timeout">Таймаут (в милисекундах). Таймаут отсчитывается с момента получения последней IsoTP-транзакции, а не с момента начала опроса</param>
        public List<DeviceTicket> LocateDevices(DeviceTicket Template, int Timeout = 500)
        {
            return LocateDevices(Template, Socket, Timeout);
        }

        /// <summary>
        /// Находит в сети все устройства с заданным шаблоном билетов.
        /// </summary>
        /// <param name="Template">Шаблон билета устройства</param>
        /// <param name="Socket">Can-порт, через который осуществляется работа</param>
        /// <param name="Timeout">Таймаут (в милисекундах). Таймаут отсчитывается с момента получения последней IsoTP-транзакции, а не с момента начала опроса</param>
        /// <returns></returns>
        public static List<DeviceTicket> LocateDevices(DeviceTicket Template, ICanSocket Socket, int Timeout = 500)
        {
            Template.BlockSerialNumber = 0;

            var helloMessage = new Messages.ProgInit(Template);
            IsoTp.Send(Socket, CanProg.FuInit, CanProg.FuDev, helloMessage.Encode());

            var res = new List<DeviceTicket>();
            while (true)
            {
                try
                {
                    var tr = IsoTp.Receive(Socket, CanProg.FuDev, CanProg.FuProg, TimeSpan.FromMilliseconds(Timeout));
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
