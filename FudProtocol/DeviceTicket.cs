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
        /// <summary>ID блока</summary>
        public int BlockId { get; set; }
        /// <summary>Серийный номер блока</summary>
        public int BlockSerialNumber { get; set; }
        /// <summary>Номер модуля</summary>
        public int Module { get; set; }
        /// <summary>Номер канала</summary>
        public int Channel { get; set; }
        /// <summary>Модификация</summary>
        public int Modification { get; set; }

        /// <summary>
        /// Создаёт билет устройства с заданными параметрами
        /// </summary>
        /// <param name="SystemId">Id Системы</param>
        /// <param name="BlockId">Id блока</param>
        /// <param name="BlockSerialNumber">Серийный номер блока</param>
        /// <param name="Module">Номер модуля</param>
        /// <param name="Channel">Номер канала (полукомплекта), начиная с 1</param>
        public DeviceTicket(
            int BlockId = 0,
            int Modification = 0,
            int BlockSerialNumber = 0,
            int Module = 0,
            int Channel = 0)
        {
            this.BlockId = BlockId;
            this.Modification = Modification;
            this.BlockSerialNumber = BlockSerialNumber;
            this.Module = Module;
            this.Channel = Channel;
        }

        public override string ToString()
        {
            return
                string.Format("{0}:{1} [{2}] {3:D5}/{4}",
                    BlockId,
                    Modification,
                    Module,
                    BlockSerialNumber,
                    Channel);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var t = (DeviceTicket)obj;
            return
                this.BlockId == this.BlockId &&
                this.Modification == this.Modification &&
                this.Module == this.Module &&
                this.BlockSerialNumber == this.BlockSerialNumber &&
                this.Channel == this.Channel;
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }
    }
}
