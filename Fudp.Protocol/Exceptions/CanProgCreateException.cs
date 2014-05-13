using System;

namespace Fudp.Protocol.Exceptions
{
    /// <summary>
    /// Ошибка при создании файла на устройстве
    /// </summary>
    [Serializable]
    public class CanProgCreateException : CanProgFileopException
    {
        public CanProgCreateException() : base("Ошибка при создании файла") { }
        public CanProgCreateException(string message) : base(message) { }
        public CanProgCreateException(string message, Exception inner) : base(message, inner) { }
        protected CanProgCreateException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    /// <summary>
    /// Ошибка создания файла: Файл уже существует
    /// </summary>
    [Serializable]
    public class CanProgFileAlreadyExistsException : CanProgCreateException
    {
        public CanProgFileAlreadyExistsException() : base("Файл с таким именем уже существует") { }
        public CanProgFileAlreadyExistsException(string message) : base(message) { }
        public CanProgFileAlreadyExistsException(string message, Exception inner) : base(message, inner) { }
        protected CanProgFileAlreadyExistsException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    /// <summary>
    /// Ошибка создания файла: Достигнуто максимальное количество файлов на устройстве
    /// </summary>
    [Serializable]
    public class CanProgMaximumFilesCountAchivedException : CanProgCreateException
    {
        public CanProgMaximumFilesCountAchivedException() : base("Достигнуто максимальное количество файлов на устройстве") { }
        public CanProgMaximumFilesCountAchivedException(string message) : base(message) { }
        public CanProgMaximumFilesCountAchivedException(string message, Exception inner) : base(message, inner) { }
        protected CanProgMaximumFilesCountAchivedException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    /// <summary>
    /// Ошибка создания файла: Память устройства переполнена
    /// </summary>
    [Serializable]
    public class CanProgMemoryIsOutException : CanProgCreateException
    {
        public CanProgMemoryIsOutException() : base("Память устройства переполнена") { }
        public CanProgMemoryIsOutException(string message) : base(message) { }
        public CanProgMemoryIsOutException(string message, Exception inner) : base(message, inner) { }
        protected CanProgMemoryIsOutException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
