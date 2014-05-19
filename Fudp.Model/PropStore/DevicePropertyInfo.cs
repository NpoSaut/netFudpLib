namespace Fudp.Model.PropStore
{
    /// <summary>Запись о свойстве на устройстве</summary>
    public struct DevicePropertyInfo
    {
        /// <summary>Инициализирует новый экземпляр класса <see cref="T:System.Object" />.</summary>
        /// <param name="Key">Ключ свойства</param>
        /// <param name="Value">Значение свойства</param>
        public DevicePropertyInfo(int Key, int Value) : this()
        {
            this.Key = Key;
            this.Value = Value;
        }

        /// <summary>Ключ свойства</summary>
        public int Key { get; private set; }

        /// <summary>Значение свойства</summary>
        public int Value { get; private set; }
    }
}
