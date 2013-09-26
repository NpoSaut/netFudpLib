using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Communications.Can;

namespace Fudp
{
    public class CanVirtPort : CanPort
    { 
        public CanVirtPort(String Name)
            : base(Name)
        {
            
        }
        protected override void SendImplementation(IList<CanFrame> Frames)
        {
            foreach (var fr in Frames)
                Console.WriteLine(String.Format("{0}  <<-  {1}", Name, fr));
        }
    }
}
