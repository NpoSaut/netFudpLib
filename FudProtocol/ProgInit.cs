using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fudp
{
    
    public class ProgInit : Message
    {

        private Byte[] buff;
        const int BufferSize = 7;
        public byte[] Buff
        {
            get { return buff; }
            set { ;}
        }
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
        /// <summary>
        /// Переход в режим программирования
        /// </summary>
        public ProgInit()
        {
        }
        /// <summary>
        /// Кодирование сообщения
        /// </summary>
        public override byte[] Encode()
        {
            buff = new byte[BufferSize];
            buff[0] = 0x01;     //идентификатор сообщения
            buff[1] = (byte)idSystem;
            Buffer.BlockCopy(BitConverter.GetBytes(idBlock), 0, buff, 2, 2);
            buff[4] = (byte)modificationOfBlock;
            buff[5] = 0xf2;
            buff[6] = 0x5b;
            return buff;
        }

        protected override void Decode(byte[] Data)
        {
            byte[] bIdBlock = new byte[4];
            idSystem = Data[1];            
            Buffer.BlockCopy(Data, 2, bIdBlock, 0, 2);
            idBlock = BitConverter.ToInt32(bIdBlock, 0);
            modificationOfBlock = Data[5];
        }
    }
}
