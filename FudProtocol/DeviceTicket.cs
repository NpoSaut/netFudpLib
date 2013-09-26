using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fudp
{

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

    }
}
