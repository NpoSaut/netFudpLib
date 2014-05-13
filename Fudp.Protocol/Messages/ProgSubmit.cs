using System;

namespace Fudp.Protocol.Messages
{
    /// <summary>Тип выхода из режима программирования</summary>
    public enum SubmitStatus : byte
    {
        /// <summary>Применить изменения</summary>
        Submit = 0,
        /// <summary>Отвергнуть изменения</summary>
        Cancel = 1
    }

    /// <summary>
    /// Команда загрузчику на выход из режима программирования
    /// </summary>
    [Identifer(0x13)]
    public class ProgSubmit : Message
    {
        public SubmitStatus Status { get; private set; }

        public ProgSubmit() : this(SubmitStatus.Submit) { }
        public ProgSubmit(SubmitStatus Status) { this.Status = Status; }

        protected override void Decode(byte[] Data)
        {
            Status = (SubmitStatus)(Data.Length > 1 ? Data[1] : 0);
        }

        public override byte[] Encode()
        {
            return new Byte[]
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
