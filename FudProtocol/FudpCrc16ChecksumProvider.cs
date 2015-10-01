namespace Fudp
{
    public class FudpCrc16ChecksumProvider : IChecksumProvider
    {
        public ushort GetChecksum(byte[] Data) { return FudpCrc.CalcCrc(Data); }
    }
}
