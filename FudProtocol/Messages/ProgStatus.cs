using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Fudp.Messages
{
    [Identifer(0x02)]
    class ProgStatus : Message
    {
        public ProgStatus()
        { }
        /// <summary>
        /// Словарь свойств
        /// </summary>
        public Dictionary<int, int> Properties { get; private set; }
        /// <summary>
        /// Количество свойств
        /// </summary>
        private int numberProperties;
        public int NumberProperties
        {
            get { return numberProperties; }
            set { numberProperties = value; }
        }

        private Byte[] buff;
        public byte[] Buff
        {
            get { return buff; }
            set { ;}
        }

        public override byte[] Encode()
        {
            buff = new byte[5 * numberProperties + 1];
            buff[0] = 0x02;
            throw new NotImplementedException();
            //Buffer.BlockCopy(BitConverter.GetBytes(properties[pKeys.Version]), 1, b, 0, intSize);
            return buff;
                        
        }
        /// <summary>
        /// Декодирование сообщения
        /// </summary>
        /// <param name="Data">Принятый массив байт</param>
        protected override void Decode(byte[] Data)
        {
            MemoryStream ms = new MemoryStream(Data);
            ms.ReadByte();

            Properties = new Dictionary<int, int>();
            while (ms.Position < ms.Length)
            {
                int key = ms.ReadByte();
                var b = new byte[intSize];
                ms.Read(b, 0, intSize);
                int val = BitConverter.ToInt32(b, 0);
                Properties.Add(key, val);
            }

            //for (int i = 1; i < Data.Length; i += 5)
            //{
            //    byte[] b = new byte[intSize];                    
            //    Buffer.BlockCopy(Data, 1+i, b, 0, intSize);                    
            //    properties.Add(Data[i], BitConverter.ToInt32(b, 0));
            //    Console.WriteLine("Key: {0}", Data[i]);
            //}
        }
        
    }
}
