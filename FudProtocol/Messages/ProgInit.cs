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
            MemoryStream ms = new MemoryStream(BufferSize);
            ms.WriteByte(MessageIdentifer);
            ms.WriteByte((byte)Ticket.SystemId);
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
                    BlockId = BitConverter.ToUInt16(Data, 2),
                    Channel = Data[4] >> 6,
                    Module = Data[4] & 0x3f,
                    BlockSerialNumber = BitConverter.ToUInt16(Data, 5)
                };
        }
    }

    [Identifer(0x14)]
    public class ProgBroadcastAnswer : ProgInit
    { }
}
