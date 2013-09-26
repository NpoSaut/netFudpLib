using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fudp
{
    /// <summary>
    /// Команда на содание или изменение записи в словаре свойств
    /// </summary>
    public enum pKeys : int
    {
        Version = 1,
        Subversion = 2,
        LastUpgrateData = 3,
        DorabotkaVersion = 4,
        DorabotkaData = 5,
        SystemType = 128,
        BlockType = 129,
        BlockModification = 130,
        BlockSerialNumber = 131,
        MakingDate = 132,
        LoaderType = 192,
        LoaderVersion = 193,
        FileSystem = 192
    }

    class ParamSetRq : Message
    {
        private pKeys paramKey;
        public pKeys ParamKey
        {
            private get { return paramKey;}
            set { paramKey = value; }
        }

        private int paramValue;
        public int ParamVlue
        {
            private get { return paramValue; }
            set { paramValue = value; }
        }
        
        public ParamSetRq()
        { }

        public override byte[] Encode()
        {
            byte[] buff = new byte[7];
            buff[0] = 0x0f;
            buff[1] = (byte)paramKey;
            Buffer.BlockCopy(BitConverter.GetBytes(paramValue), 0, buff, 2, 4);
            return buff;
        }

        protected override void Decode(byte[] Data)
        {
            paramKey = (pKeys)Data[1];
            paramValue = BitConverter.ToInt32(Data, 2);
        }
    }
}
