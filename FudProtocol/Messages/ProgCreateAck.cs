using System.Collections.Generic;

namespace Fudp.Messages
{
    [Identifer(0x0a)]
    public class ProgCreateAck : Message
    {
        private static readonly Dictionary<int, string> _errorMsg = new Dictionary<int, string>
                                                                    {
                                                                        { 0, "Файл создан успешно" },
                                                                        { 1, "Файл с таким именем уже существуе" },
                                                                        { 2, "Превышено максимальное количество файлов" },
                                                                        { 3, "Недостаточно памяти" },
                                                                        { 4, "Ошибка создания" }
                                                                    };

        public ProgCreateAck(int ErrorCode = 0) { this.ErrorCode = ErrorCode; }

        public static Dictionary<int, string> ErrorMsg
        {
            get { return _errorMsg; }
        }

        public int ErrorCode { get; private set; }

        public override byte[] Encode()
        {
            var buff = new byte[2];
            buff[0] = MessageIdentifer;
            buff[1] = (byte)ErrorCode;
            return buff;
        }

        protected override void Decode(byte[] Data) { ErrorCode = Data[1]; }
    }
}
