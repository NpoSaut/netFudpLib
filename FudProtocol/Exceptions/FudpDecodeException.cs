using System;
using System.Runtime.Serialization;

namespace Fudp.Exceptions
{
    /// <Summary>Ошибка декодирования FUDP-сообщения</Summary>
    [Serializable]
    public class FudpDecodeException : FudpException
    {
        private readonly byte[] _data;
        public const string ExceptionMessage = "Ошибка декодирования FUDP-сообщения";
        public FudpDecodeException(byte[] Data, Exception inner) : base(string.Format("{0} (Данные: {1})", ExceptionMessage, BitConverter.ToString(Data)), inner) { _data = Data; }

        protected FudpDecodeException(
            SerializationInfo info,
            StreamingContext context) : base(info, context) { }
    }
}
