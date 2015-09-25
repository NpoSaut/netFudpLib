using System;

namespace Fudp
{
    public interface IChecksumProvider
    {
        UInt16 GetChecksum(Byte[] Data);
    }
}