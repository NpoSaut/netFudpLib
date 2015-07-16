using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fudp.Messages
{
    /// <summary>
    /// Подтверждение создания или изменения записи в словаре свойств
    /// </summary>
    [Identifer(0x10)]
    public class ParamSetAck : Message
    {
        
        public ParamSetAck()
        { }

        private static readonly Dictionary<int, string> ErrorMessagesDictionary = new Dictionary<int, string>()
        {
            {0, "Значение свойств запиано успешно"},
            {1, "Свойство \"только для чтения\""},
            {2, "Превышено максимальное количество свойств"}
        };
        public String ErrorMessage
        {
            get
            {
                return ErrorMessagesDictionary.ContainsKey(ErrorCode)
                           ? ErrorMessagesDictionary[ErrorCode]
                           : "Неизвестная ошибка";
            }
        }

        public int ErrorCode { get; private set; }

        public override byte[] Encode()
        {
            byte[] buff = new byte[7];
            buff[0] = MessageIdentifer;
            buff[1] = (byte)ErrorCode;
            return buff;
        }

        protected override void Decode(byte[] Data)
        {
            ErrorCode = Data[1];
        }

        public override string ToString()
        {
            return string.Format("{0} [ {1} ]", base.ToString(), ErrorMessage);
        }
    }
}
