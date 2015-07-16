using System.Collections.Generic;

namespace Fudp
{
    public interface IPropertiesManager
    {
        int this[byte Key] { get; set; }
    }

    internal class PropertiesManager : IPropertiesManager
    {
        private readonly IDictionary<int, int> _cache;
        public PropertiesManager(IDictionary<int, int> Properties) { _cache = Properties; }

        public int this[byte Key]
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }
    }
}
