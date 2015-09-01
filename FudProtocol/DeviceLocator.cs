using System;
using System.Collections.Generic;
using System.Reactive.Linq;
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
        /// <param name="Timeout">
        ///     Таймаут (в милисекундах). Таймаут отсчитывается с момента получения последней IsoTP-транзакции, а
        ///     не с момента начала опроса
        /// </param>
        /// <returns></returns>
        public IList<DeviceTicket> LocateDevices(DeviceTicket Template, ICanPort Port, int Timeout = 100)
        {
            using (IIsoTpConnection connection = Port.OpenIsoTpConnection(CanProg.FuInit, CanProg.FuDev, new IsoTpConnectionParameters()))
            {
                using (var fudpPort = new FudpPort(connection))
                {
                    return fudpPort.Request(new ProgInit(Template),
                                            flow => flow.OfType<ProgBCastResponse>()
                                                        .Select(resp => resp.Ticket)
                                                        .Take(TimeSpan.FromMilliseconds(Timeout))
                                                        .Distinct()
                                                        .ToList()
                                                        .First());
                }

                //Template.BlockSerialNumber = 0;
                //var initMessage = new IsoTpPacket();

                //IsoTpPacket xxx = connection.Request(initMessage);

                //var x = connection.Request(initMessage, flow => flow.Select(f => f.));
            }

            //using (var flow = new CanFlow(Port, CanProg.FuDev, CanProg.FuInit, CanProg.FuProg))
            //{
            //    Template.BlockSerialNumber = 0;

            //    var helloMessage = new ProgInit(Template);
            //    IsoTp.Send(flow, CanProg.FuInit, CanProg.FuDev, helloMessage.Encode());

            //    var res = new List<DeviceTicket>();
            //    var sw = new Stopwatch();
            //    sw.Start();
            //    while (sw.ElapsedMilliseconds < Timeout)
            //    {
            //        try
            //        {
            //            TpReceiveTransaction tr = IsoTp.Receive(flow, CanProg.FuDev, CanProg.FuProg, TimeSpan.FromMilliseconds(Timeout - sw.ElapsedMilliseconds));
            //            Message msg = Message.DecodeMessage(tr.Data);
            //            if (msg is ProgBCastResponse) res.Add((msg as ProgBCastResponse).Ticket);
            //        }
            //        catch (IsoTpReceiveTimeoutException)
            //        {
            //            break;
            //        }
            //    }
            //    return res.Distinct().ToList();
            //}
        }
    }
}
