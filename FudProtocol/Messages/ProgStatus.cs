using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fudp.Messages
{
    class ProgStatus : Message
    {
        public ProgStatus()
        { }
        /// <summary>
        /// Словарь свойств
        /// </summary>
        private Dictionary<int, int> properties = new Dictionary<int, int>();
        public Dictionary<int, int> Properties
        {
            get { return properties; }
            set { ;}
        }
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
            //Buffer.BlockCopy(BitConverter.GetBytes(properties[pKeys.Version]), 1, b, 0, intSize);
            return buff;
                        
        }
        /// <summary>
        /// Декодирование сообщения
        /// </summary>
        /// <param name="Data">Принятый массив байт</param>
        protected override void Decode(byte[] Data)
        {
            if (Data != null)
            {                
                for (int i = 1; i < Data.Length; i += 5)
                {
                    byte[] b = new byte[intSize];                    
                    Buffer.BlockCopy(Data, 1+i, b, 0, intSize);                    
                    properties.Add(Data[i], BitConverter.ToInt32(b, 0));
                    Console.WriteLine("Key: {0}", Data[i]);
                }
            }  
        }
        
    }
}
