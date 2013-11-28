using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fudp.Messages
{
    /// <summary>Статус отмены изменений</summary>
    public enum SubmitAckStatus : byte
    {
        /// <summary>Изменения успешно применены</summary>
        SubmitSuccessed = 0,
        /// <summary>Не удалось применить изменения</summary>
        SubmitFails = 1,
        /// <summary>Изменения успешно отменены</summary>
        CancelSuccessed = 2,
        /// <summary>Не удалось отменить изменения</summary>
        CancelFails = 3
    }

    [Identifer(0x14)]
    class ProgSubmitAck : Message
    {
        public SubmitAckStatus Status { get; private set; }

        public ProgSubmitAck() : this(SubmitAckStatus.SubmitFails) { }
        public ProgSubmitAck(SubmitAckStatus Status) { this.Status = Status; }

        protected override void Decode(byte[] Data)
        {
            Status = (SubmitAckStatus)(Data.Length > 1 ? Data[1] : 0);
        }

        public override byte[] Encode()
        {
            return new[]
                   {
                       MessageIdentifer,
                       (byte)Status
                   };
        }
    }
}
