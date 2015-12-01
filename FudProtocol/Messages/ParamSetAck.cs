using System;
using System.Collections.Generic;

namespace Fudp.Messages
{
    /// <summary>Подтверждение создания или изменения записи в словаре свойств</summary>
    [Identifer(0x10)]
    public class ParamSetAck : Message
    {
        private static readonly Dictionary<int, string> _errorMessagesDictionary = new Dictionary<int, string>
                                                                                   {
                                                                                       { 0, "Значение свойств запиано успешно" },
                                                                                       { 1, "Свойство \"только для чтения\"" },
                                                                                       { 2, "Превышено максимальное количество свойств" }
                                                                                   };

        public ParamSetAck() : this(-1) { }

        public ParamSetAck(int Status) { ErrorCode = Status; }

        public String ErrorMessage
        {
            get
            {
                return _errorMessagesDictionary.ContainsKey(ErrorCode)
                           ? _errorMessagesDictionary[ErrorCode]
                           : "Неизвестная ошибка";
            }
        }

        public int ErrorCode { get; private set; }

        public override byte[] Encode()
        {
            var buff = new byte[7];
            buff[0] = MessageIdentifer;
            buff[1] = (byte)ErrorCode;
            return buff;
        }

        protected override void Decode(byte[] Data) { ErrorCode = Data[1]; }

        public override string ToString() { return string.Format("{0} [ {1} ]", base.ToString(), ErrorMessage); }
    }
}
