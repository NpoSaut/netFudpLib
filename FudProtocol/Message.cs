using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Fudp
{
    public abstract class Message
    {
        public const int intSize = 4;
        protected abstract void Decode(Byte[] Data);
        public abstract Byte[] Encode();

        public static T Decode<T>(Byte[] Data)
            where T : Message, new()
        {
            var res = new T();
            res.Decode(Data);
            return res;
        }        
    }
}
