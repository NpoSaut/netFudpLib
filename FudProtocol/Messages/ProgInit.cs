using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Fudp.Messages
{
    
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
            MemoryStream ms = new MemoryStream(BufferSize);
            ms.WriteByte(0x01);
            ms.Write(BitConverter.GetBytes(Ticket.BlockId), 0, 2);
            ms.WriteByte((byte)((Ticket.Channel & 0x03) << 6 | Ticket.Module & 0x3f));
            ms.Write(BitConverter.GetBytes(Ticket.BlockSerialNumber), 0, 2);
            return ms.ToArray();
        }

        protected override void Decode(byte[] Data)
        {
            this.Ticket =
                new DeviceTicket()
                {
                    SystemId = Data[1],
                    BlockId = BitConverter.ToInt32(Data, 2),
                    Channel = Data[4] >> 6,
                    Module = Data[4] ^ 0x3f,
                    BlockSerialNumber = BitConverter.ToInt32(Data, 6)
                };
        }
    }
}
