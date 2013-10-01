using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fudp
{
    /// <summary>
    /// Билет устройства - содержит идентефикационные данные устройства
    /// </summary>
    public class DeviceTicket
    {
        /// <summary>ID системы</summary>
        public int SystemId { get; set; }
        /// <summary>ID блока</summary>
        public int BlockId { get; set; }
        /// <summary>Серийный номер блока</summary>
        public int BlockSerialNumber { get; set; }
        /// <summary>Номер модуля</summary>
        public int Module { get; set; }
        /// <summary>Номер канала</summary>
        public int Channel { get; set; }

        /// <summary>Создаёт билет устройства с параметрами по-умолчанию</summary>
        public DeviceTicket()
        { }
        /// <summary>
        /// Создаёт билет устройства с заданными параметрами
        /// </summary>
        /// <param name="SystemId">Id Системы</param>
        /// <param name="BlockId">Id блока</param>
        /// <param name="BlockSerialNumber">Серийный номер блока</param>
        /// <param name="Module">Номер модуля</param>
        /// <param name="Channel">Номер канала (полукомплекта), начиная с 1</param>
        public DeviceTicket(
            int SystemId,
            int BlockId,
            int BlockSerialNumber,
            int Module,
            int Channel)
            : this()
        {
            this.SystemId = SystemId;
            this.BlockId = BlockId;
            this.BlockSerialNumber = BlockSerialNumber;
            this.Module = Module;
            this.Channel = Channel;
        }
        /// <summary>
        /// Создаёт броадкаст-билет устройств с заданными параметрами
        /// </summary>
        /// <param name="SystemId">Id Системы</param>
        /// <param name="BlockId">Id блока</param>
        /// <param name="Module">Номер модуля</param>
        /// <param name="Channel">Номер канала (полукомплекта), начиная с 1</param>
        public DeviceTicket(
            int SystemId,
            int BlockId,
            int Module,
            int Channel)
            : this(SystemId, BlockId, 0, Module, Channel)
        { }

        public override string ToString()
        {
            return
                string.Format("{0}:{1}-{4:D5}/{3} [{2}]",
                    SystemId,
                    BlockId,
                    Module,
                    Channel,
                    BlockSerialNumber);
        }
    }
}
