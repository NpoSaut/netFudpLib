using System;
using System.Collections.Generic;

namespace Fudp.Messages
{
    /// <summary>Подтверждение удаления параметра из словаря</summary>
    [Identifer(0x12)]
    public class ParamRmAck : Message
    {
        private static readonly Dictionary<int, string> ErrorMessagesDictionary = new Dictionary<int, string>
                                                                                  {
                                                                                      { 0, "Параметр удалён успешно" },
                                                                                      { 1, "Парметр \"только для чтения\"" },
                                                                                      { 2, "Параметр не найден" }
                                                                                  };

        public ParamRmAck(int Status = 0) { ErrorCode = Status; }

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
            var buff = new byte[7];
            buff[0] = MessageIdentifer;
            buff[1] = (byte)ErrorCode;
            return buff;
        }

        protected override void Decode(byte[] Data) { ErrorCode = Data[1]; }
    }
}
