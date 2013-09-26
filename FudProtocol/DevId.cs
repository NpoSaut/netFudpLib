using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fudp
{

    public class DevId
    {
        public DevId()
        { }
        /// <summary>
        /// ID системы
        /// </summary>
        private int idSystem;
        public int IdSystem
        {
            get { return idSystem; }
            set { idSystem = value; }
        }
        /// <summary>
        /// ID блока
        /// </summary>
        private int idBlock;
        public int IdBlock
        {
            get { return idBlock; }
            set { idBlock = value; }
        }
        /// <summary>
        /// Модификация блока
        /// </summary>
        private int modificationOfBlock;
        public int ModificationOfBlock
        {
            get { return modificationOfBlock; }
            set { modificationOfBlock = value; }
        }
        
    }
}
