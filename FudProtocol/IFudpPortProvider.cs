using Communications.Protocols.IsoTP;

namespace Fudp
{
    public interface IFudpPortProvider
    {
        IFudpPort OpenPort(IsoTpConnectionParameters IsoTpParameters);
    }
}
