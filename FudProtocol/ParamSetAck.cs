using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fudp
{
    /// <summary>
    /// Подтверждение содания или изменения записи в словаре свойств
    /// </summary>
    class ParamSetAck : Message
    {
        
        public ParamSetAck()
        { }

        private static Dictionary<int, string> errorMsg = new Dictionary<int, string>()
        {
            {0, "Значение свойств запиано успешно"},
            {1, "Свойство \"только для чтения\""},
            {2, "Первышено максимальное количество свойств"}
        };
        public Dictionary<int, string> ErrorMsg
        {
            get { return errorMsg; }
            set { ;}
        }

        private int errorCode;
        public int ErrorCode
        {
            get { return errorCode; }
            set { ;}
        }

        public override byte[] Encode()
        {
            byte[] buff = new byte[7];
            buff[0] = 0x10;
            buff[1] = (byte)errorCode;
            return buff;
        }

        protected override void Decode(byte[] Data)
        {
            errorCode = Data[1];
        }
    }
}
