using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Communications.Can;
using Communications.Protocols.IsoTP;

namespace Fudp
{
    public static class DeviceLocator
    {
        public static List<DeviceTicket> LocateDevices(DeviceTicket Template, CanPort OnPort, int EchoTimeout = 500)
        {
            using (var flow = new CanFlow(OnPort, CanProg.FuDev))
            {
                if (Template.BlockSerialNumber != 0) Template.BlockSerialNumber = 0;

                var HelloMessage = new Messages.ProgInit(Template);
                IsoTp.BeginSend(flow, CanProg.FuProg, CanProg.FuDev, HelloMessage.Encode()).Wait();

                return
                    flow.Read(TimeSpan.FromMilliseconds(EchoTimeout), false)
                        .Select(f => new Communications.Protocols.IsoTP.Frames.SingleFrame(f.Data).Data)
                        .Select(d => Messages.Message.Decode<Messages.ProgInit>(d))
                        .Select(m => m.Ticket)
                        .ToList();
            }
        }
    }
}
