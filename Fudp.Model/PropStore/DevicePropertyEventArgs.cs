using System;

namespace Fudp.Model.PropStore
{
    public class DevicePropertyRemovedEventArgs : DevicePropertyEventArgs
    {
        /// <summary>»нициализирует новый экземпл€р класса <see cref="T:System.EventArgs" />.</summary>
        public DevicePropertyRemovedEventArgs(int Key) : base(Key) { }
    }

    public class DevicePropertyChangedEventArgs : DevicePropertyEventArgs
    {
        /// <summary>»нициализирует новый экземпл€р класса <see cref="T:System.EventArgs" />.</summary>
        public DevicePropertyChangedEventArgs(int Key, int OldValue, int NewValue) : base(Key)
        {
            this.OldValue = OldValue;
            this.NewValue = NewValue;
        }

        public int OldValue { get; private set; }
        public int NewValue { get; private set; }
    }

    public class DevicePropertyCreatedEventArg : DevicePropertyEventArgs
    {
        /// <summary>»нициализирует новый экземпл€р класса <see cref="T:System.EventArgs" />.</summary>
        public DevicePropertyCreatedEventArg(int Key, int Value) : base(Key)
        {
            this.Value = Value;
        }

        public int Value { get; private set; }
    }

    public abstract class DevicePropertyEventArgs : EventArgs
    {
        /// <summary>»нициализирует новый экземпл€р класса <see cref="T:System.EventArgs" />.</summary>
        protected DevicePropertyEventArgs(int Key)
        {
            this.Key = Key;
        }

        public int Key { get; private set; }
    }
}