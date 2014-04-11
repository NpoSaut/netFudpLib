using System;
using System.Text;

namespace Fudp.Messages
{
    [Identifer(0x07)]
    public class ProgRm : Message
    {
        /// <summary>Команда на удаление файла</summary>
        public ProgRm() { }

        /// <summary>Команда на удаление файла</summary>
        public ProgRm(String FileName) { this.FileName = FileName; }

        /// <summary>Имя файла</summary>
        public string FileName { get; private set; }

        /// <summary>Код ошибки</summary>
        public int ErrorCode { get; private set; }

        /// <summary>Кодирование сообщения</summary>
        public override byte[] Encode()
        {
            var buff = new Byte[2 + FileName.Length];
            buff[0] = MessageIdentifer; //Идентификатор сообщения
            buff[1] = (byte)FileName.Length;
            Buffer.BlockCopy(Encoding.GetEncoding(1251).GetBytes(FileName), 0, buff, 2, FileName.Length);
            return buff;
        }

        protected override void Decode(byte[] Data)
        {
            var buff = new byte[Data[1]];
            Buffer.BlockCopy(Data, 2, buff, 0, Data[1]);
            FileName = Encoding.GetEncoding(1251).GetString(buff);
        }

        public override string ToString()
        {
            return string.Format("{0} [ {1} ]", base.ToString(), FileName);
        }
    }
}
