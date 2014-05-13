using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fudp.Protocol.Messages
{
    public abstract class Message
    {
        public const int intSize = 4;
        protected abstract void Decode(Byte[] Data);
        public abstract Byte[] Encode();

        private static readonly Encoding _DefaultEncodung = Encoding.GetEncoding(1251);
        protected static Encoding DefaultEncodung { get { return _DefaultEncodung; } }

        private static readonly Lazy<Dictionary<byte, Type>> _Identifers = new Lazy<Dictionary<byte,Type>>(InitializeIdentifers, true);
        public static Dictionary<byte, Type> Identifers
        {
            get { return _Identifers.Value; }
        }
        

        public byte MessageIdentifer { get { return GetIdentifer(this.GetType()); } }

        public static byte GetIdentifer<MessageType>() { return GetIdentifer(typeof(MessageType)); }
        public static byte GetIdentifer(Type MessageType)
        {
            var attr = MessageType.GetCustomAttributes(typeof(IdentiferAttribute), false).OfType<IdentiferAttribute>().FirstOrDefault();
            if (attr == null) throw new IdentiferAttributeNotSetException(MessageType);
            return attr.Id;
        }

        public static T Decode<T>(Byte[] Data)
            where T : Message
        {
            var res = Activator.CreateInstance<T>();
            if (Data[0] != GetIdentifer<T>()) throw new Exceptions.FudpIdentiferMismatchException();
            res.Decode(Data);
            return res;
        }
        public static Message DecodeMessage(Byte[] Data)
        {
            byte id = Data[0];
            if (!Identifers.ContainsKey(id)) throw new Exceptions.FudpUnknownIdentiferException(id);
            var res = (Message)Activator.CreateInstance(Identifers[id]);
            res.Decode(Data);
            return res;
        }


        private static Dictionary<byte, Type> InitializeIdentifers()
        {
            return
                System.Reflection.Assembly.GetAssembly(typeof(Message))
                    .GetTypes()
                    .Where(T => T.IsSubclassOf(typeof(Message)))
                    .ToDictionary(GetIdentifer);
        }

        public override string ToString() { return this.GetType().Name; }
    }
}
