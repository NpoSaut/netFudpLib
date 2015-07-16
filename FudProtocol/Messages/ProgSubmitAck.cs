using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fudp.Messages
{
    /// <summary>Статус отмены изменений</summary>
    public enum SubmitAckStatus : int
    {
        /// <summary>Изменения успешно применены</summary>
        SubmitSuccessed = 0,
        /// <summary>Не удалось применить изменения</summary>
        SubmitFails = 1,
        /// <summary>Изменения успешно отменены</summary>
        CancelSuccessed = 2,
        /// <summary>Не удалось отменить изменения</summary>
        CancelFails = 3,
        /// <summary>Неизвестный код состояния</summary>
        Unknown = -1
    }

    [Identifer(0x14)]
    public class ProgSubmitAck : Message
    {
        public SubmitAckStatus Status { get; private set; }

        public ProgSubmitAck() : this(SubmitAckStatus.SubmitFails) { }
        public ProgSubmitAck(SubmitAckStatus Status) { this.Status = Status; }

        protected override void Decode(byte[] Data)
        {
            int statusCode = (Data.Length > 1 ? Data[1] : 0);
            Status = Enum.IsDefined(typeof (SubmitAckStatus), statusCode)
                         ? (SubmitAckStatus)statusCode
                         : SubmitAckStatus.Unknown;
        }

        public override byte[] Encode()
        {
            return new[]
                   {
                       MessageIdentifer,
                       (byte)Status
                   };
        }

        public override string ToString()
        {
            return string.Format("{0} [ {1} ]", base.ToString(), Status);
        }
    }
}
