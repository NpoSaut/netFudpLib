using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Fudp.Exceptions;

namespace Fudp.Messages
{
    [Identifer(0x02)]
    public class ProgStatus : Message
    {
        /// <summary>Словарь свойств</summary>
        public IDictionary<int, int> Properties { get; set; }

        public override byte[] Encode()
        {
            var ms = new MemoryStream();
            var sw = new BinaryWriter(ms);

            sw.Write((byte)0x02);
            foreach (var property in Properties)
            {
                sw.Write((byte)property.Key);
                sw.Write((int)property.Value);
            }

            return ms.ToArray();
        }

        /// <summary>Декодирование сообщения</summary>
        /// <param name="Data">Принятый массив байт</param>
        protected override void Decode(byte[] Data)
        {
            var ms = new MemoryStream(Data);
            ms.ReadByte();

            Properties = new Dictionary<int, int>();
            while (ms.Position < ms.Length)
            {
                int key = ms.ReadByte();
                var b = new byte[intSize];
                ms.Read(b, 0, intSize);
                int val = BitConverter.ToInt32(b, 0);
                try
                {
                    Properties.Add(key, val);
                }
                catch (ArgumentException exc)
                {
                    throw new FudpException(String.Format("Загрузчик отправил не корректный словарь свойств: {0}\nСвойство №{1}: {2}", exc.Message, key, val),
                                            exc);
                }
            }
        }

        public override string ToString()
        {
            return string.Format("{0} Properties: {1}", base.ToString(), string.Join("; ", Properties.Select(p => string.Format("{0}={1}", p.Key, p.Value))));
        }
    }
}
