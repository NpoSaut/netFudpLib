using System;

namespace Fudp
{
    public abstract class DevFileListNode { }

    internal class DevFileListIncompleteTransactionFlag : DevFileListNode
    {
        public DevFileListIncompleteTransactionFlag(uint Remaining) { this.Remaining = Remaining; }
        public uint Remaining { get; private set; }

        public override string ToString() { return string.Format("{{INCOMPLETE-{0}}}", Remaining); }
    }

    public class DevFileInfo : DevFileListNode {
        /// <summary>Создаёт образ файла на устройстве с указанными размером и контрольной суммой</summary>
        /// <param name="Name">Имя файла</param>
        /// <param name="Size">Размер файла</param>
        /// <param name="Checksum">Контрольная сумма</param>
        public DevFileInfo(String Name, Int32 Size, UInt16 Checksum)
        {
            FileName = Name;
            this.Size = Size;
            ControlSum = Checksum;
        }

        /// <summary>Имя файла</summary>
        public string FileName { get; private set; }

        /// <summary>Размер файла</summary>
        public Int32 Size { get; private set; }

        /// <summary>Контрольная сумма файла</summary>
        public UInt16 ControlSum { get; private set; }

        public override string ToString() { return string.Format("{0} ({1}Б {2:x4})", FileName, Size, ControlSum); }
    }

    public class DevFile : DevFileInfo
    {
        /// <summary>Представляет файл на устройстве</summary>
        /// <param name="Name">Имя файла</param>
        /// <param name="Data">Данные файла</param>
        public DevFile(String Name, Byte[] Data)
            : base (Name, Data.Length, FudpCrc.CalcCrc(Data))
        {
            this.Data = Data;
        }

        public DevFile(DevFileInfo Info)
            : base(Info.FileName, Info.Size, Info.ControlSum)
        {
            Data = new byte[Size];
        }

        /// <summary>Данные</summary>
        public byte[] Data { get; private set; }
    }
}
