using System;
using System.Text;

namespace Fudp.Messages
{
    [Identifer(0x05)]
    public class ProgReadRq : Message
    {
        /// <summary>Запрос на чтение</summary>
        public ProgReadRq() { }

        /// <summary>Создаёт запрос на чтение с заданными параметрами</summary>
        /// <param name="FileName">Имя файла</param>
        /// <param name="Offset">Отступ от начала файла</param>
        /// <param name="Length">Размер считываемого буфера</param>
        public ProgReadRq(string FileName, int Offset, int Length) : this()
        {
            this.FileName = FileName;
            this.Offset = Offset;
            this.Length = Length;
        }

        /// <summary>Имя файла</summary>
        public string FileName { get; set; }

        /// <summary>Отступ от начала файла</summary>
        public int Offset { get; private set; }

        /// <summary>Размер считываемой области</summary>
        public int Length { get; private set; }

        /// <summary>Кодирование сообщения</summary>
        public override byte[] Encode()
        {
            var buff = new Byte[10 + FileName.Length];
            buff[0] = MessageIdentifer; //Идентификатор сообщения
            buff[1] = (byte)FileName.Length;
            Buffer.BlockCopy(Encoding.GetEncoding(1251).GetBytes(FileName), 0, buff, 2, FileName.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(Offset), 0, buff, 2 + FileName.Length, intSize);
            Buffer.BlockCopy(BitConverter.GetBytes(Length), 0, buff, 6 + FileName.Length, intSize);
            return buff;
        }

        protected override void Decode(byte[] Data)
        {
            var filename = new byte[Data[1]];
            var bOffset = new byte[intSize];
            var bReadSize = new byte[intSize];
            Buffer.BlockCopy(Data, 2, filename, 0, Data[1]);
            Buffer.BlockCopy(Data, 2 + Data[1], bOffset, 0, intSize);
            Buffer.BlockCopy(Data, 6 + Data[1], bReadSize, 0, intSize);
            FileName = Encoding.GetEncoding(1251).GetString(filename);
            Offset = BitConverter.ToInt32(bOffset, 0);
            Length = BitConverter.ToInt32(bReadSize, 0);
        }

        /// <summary>Рассчитывает длину заголовка пакета <see cref="ProgReadRq" />
        /// </summary>
        /// <param name="FileName">Имя файла в запросе</param>
        public static int GetHeaderLength(string FileName) { return FileName.Length + 10; }

        public override string ToString() { return string.Format("{0} [{1} from {1} -- {2}Б]", FileName, Offset, Length); }
    }
}
