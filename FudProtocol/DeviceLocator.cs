﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Communications.Can;
using Communications.Protocols.IsoTP;
using Communications.Protocols.IsoTP.Exceptions;
using Fudp.Messages;

namespace Fudp
{
    /// <summary>Содержит методы для поиска прошиваемых устройств в сети</summary>
    public class DeviceLocator : IDeviceLocator
    {
        /// <summary>Находит в сети все устройства с заданным шаблоном билетов.</summary>
        /// <param name="Template">Шаблон билета устройства</param>
        /// <param name="OnPort">Can-порт, через который осуществляется работа</param>
        /// <param name="Timeout">
        ///     Таймаут (в милисекундах). Таймаут отсчитывается с момента получения последней IsoTP-транзакции, а
        ///     не с момента начала опроса
        /// </param>
        /// <returns></returns>
        public List<DeviceTicket> LocateDevices(DeviceTicket Template, CanPort OnPort, int Timeout = 100)
        {
            using (var flow = new CanFlow(OnPort, CanProg.FuDev, CanProg.FuInit, CanProg.FuProg))
            {
                Template.BlockSerialNumber = 0;

                var helloMessage = new ProgInit(Template);
                IsoTp.Send(flow, CanProg.FuInit, CanProg.FuDev, helloMessage.Encode());

                var res = new List<DeviceTicket>();
                var sw = new Stopwatch();
                sw.Start();
                while (sw.ElapsedMilliseconds < Timeout)
                {
                    try
                    {
                        TpReceiveTransaction tr = IsoTp.Receive(flow, CanProg.FuDev, CanProg.FuProg, TimeSpan.FromMilliseconds(Timeout - sw.ElapsedMilliseconds));
                        Message msg = Message.DecodeMessage(tr.Data);
                        if (msg is ProgBCastResponse) res.Add((msg as ProgBCastResponse).Ticket);
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
