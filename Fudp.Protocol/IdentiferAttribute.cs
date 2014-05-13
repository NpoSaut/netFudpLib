using System;

namespace Fudp.Protocol
{
    /// <summary>
    /// Указывает на идентефикатор для данного сообщения протокола FUDP
    /// </summary>
    internal class IdentiferAttribute : Attribute
    {
        public byte Id { get; private set; }

        public IdentiferAttribute(byte Id)
        {
            this.Id = Id;
        }
    }

    /// <summary>
    /// Говорит о том, что для класса-обёртки сообдения FUDP не установлен аттрибут идентификатора
    /// </summary>
    [Serializable]
    public class IdentiferAttributeNotSetException : Exception
    {
        public IdentiferAttributeNotSetException() : base("Аттрибут идентефикатора сообщения не установлен") { }
        public IdentiferAttributeNotSetException(Type MessageType) : base("Аттрибут идентефикатора сообщения не установлен для сообщения " + MessageType.Name) { }
        public IdentiferAttributeNotSetException(string message) : base(message) { }
        public IdentiferAttributeNotSetException(string message, Exception inner) : base(message, inner) { }
        protected IdentiferAttributeNotSetException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
