using Fudp.Model.Filesystem;

namespace Fudp.Model
{
    /// <summary>Модель FUDP-устройства</summary>
    public class Device
    {
        public Device(IOperatorsFactory OperatorsFactory) { Files = new FilesystemModel(OperatorsFactory.GetFileOperator()); }

        /// <summary>Файловая система устройства</summary>
        public FilesystemModel Files { get; private set; }
    }
}
