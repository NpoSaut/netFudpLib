using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fudp.Messages
{
    /// <summary>
    /// Подтверждение удаления параметра из словаря
    /// </summary>
    class ParamRmAck : Message
    {
        public ParamRmAck()
        { }

        private static Dictionary<int, string> errorMsg = new Dictionary<int, string>()
        {
            {0, "Парамтр удален успешно"},
            {1, "Парметр \"только для чтения\""},
            {2, "Параметр не найден"}
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
            buff[0] = 0x12;
            buff[1] = (byte)errorCode;
            return buff;
        }

        protected override void Decode(byte[] Data)
        {
            errorCode = Data[1];
        }
    }
}
