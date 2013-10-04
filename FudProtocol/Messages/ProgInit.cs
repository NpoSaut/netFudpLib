using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Fudp.Messages
{
    [Identifer(0x01)]
    public class ProgInit : Message
    {
        public DeviceTicket Ticket { get; set; }

        const int BufferSize = 7;
        
        public ProgInit()
        {
            Ticket = new DeviceTicket();
        }
        public ProgInit(DeviceTicket Device)
        {
            this.Ticket = Device;
        }
        /// <summary>
        /// Кодирование сообщения
        /// </summary>
        public override byte[] Encode()
        {
            var buff = new byte[BufferSize];
            buff[0] = MessageIdentifer;
            buff[1] = (byte)((Ticket.BlockId & 0xff0) >> 4);
            buff[2] = (byte)((Ticket.BlockId & 0x00f) << 4 | Ticket.Modification & 0x0f);
            buff[3] = (byte)(Ticket.Module);
            buff[4] = (byte)((Ticket.Channel & 0x0f) << 4 | (Ticket.BlockSerialNumber & 0xf0000) >> 16);
            buff[5] = (byte)((Ticket.BlockSerialNumber & 0x0ff00) >> 8);
            buff[6] = (byte)(Ticket.BlockSerialNumber & 0x000ff);
            return buff;
        }

        protected override void Decode(byte[] Data)
        {
            this.Ticket =
                new DeviceTicket()
                {
                    BlockId = (Data[1] << 4) | ((Data[2] & 0xf0) >> 4),
                    Modification = Data[2] & 0x0f,
                    Module = Data[3],
                    Channel = (Data[4] & 0xf0) >> 4,
                    BlockSerialNumber = ((Data[4] & 0x0f) << 16) | (Data[5] << 8) | Data[6]
                };
        }
    }

    [Identifer(0x14)]
    public class ProgBroadcastAnswer : ProgInit
    { }
}
