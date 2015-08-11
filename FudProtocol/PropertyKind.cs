namespace Fudp
{
    public enum PropertyKind : byte
    {
        FirmwareVersion = 1,
        FirmwareSubversion = 2,
        FirmwareUpdateDate = 3,
        FrimwareChecksum = 6,
        FirmwareLabel = 7,

        CellId = 129,
        SoftwareModuleId = 130,
        SerialNumber = 131,
        FactoryDate = 132, // год*100 + месяц
        HalfsetNumber = 133,
        CellModuficationNumber = 134,

        LoaderKind = 192,
        LoaderVersion = 193,
        HasFilesystem = 194,
        LoaderCurrentProtocolVersion = 195,
        LoaderSupportedProtocolVersion = 196,
        LoaderSubversion = 197,
        LoaderConfiguration = 198
    }
}