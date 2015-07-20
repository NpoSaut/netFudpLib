using System;
using System.Runtime.Serialization;

namespace Fudp.Exceptions
{
    /// <Summary>Ошибка удаления файла</Summary>
    [Serializable]
    public class CanProgDeleteException : CanProgException
    {
        private readonly int _errorCode;
        public CanProgDeleteException(int ErrorCode) : base(string.Format("Ошибка удаления файла (Код ошибки {0})", ErrorCode)) { _errorCode = ErrorCode; }

        protected CanProgDeleteException(
            SerializationInfo info,
            StreamingContext context) : base(info, context) { }
    }
}
