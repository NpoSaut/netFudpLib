using Communications.Can;
using Communications.Protocols.IsoTP;

namespace Fudp
{
    public class CanFudpPortProvider : IFudpPortProvider
    {
        private readonly ICanPort _canPort;
        private readonly IsoTpConnectionParameters _isoTpParameters;
        private readonly ushort _receiveDescriptor;
        private readonly ushort _transmitDescriptor;

        public CanFudpPortProvider(ICanPort CanPort, IsoTpConnectionParameters IsoTpParameters, ushort ReceiveDescriptor, ushort TransmitDescriptor)
        {
            _canPort = CanPort;
            _receiveDescriptor = ReceiveDescriptor;
            _transmitDescriptor = TransmitDescriptor;
            _isoTpParameters = IsoTpParameters;
        }

        public IFudpPort OpenPort() { return new FudpPort(new IsoTpOverCanPort(_canPort, _transmitDescriptor, _receiveDescriptor, _isoTpParameters)); }
    }
}
