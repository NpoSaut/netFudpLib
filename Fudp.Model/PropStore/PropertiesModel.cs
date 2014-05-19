using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Fudp.Model.PropStore
{
    public class PropertiesModel : IDictionary<int, int>
    {
        private readonly Dictionary<int, int> _properties;

        public PropertiesModel(IPropertyOperator Operator) { _properties = Operator.EnumerateKeys().ToDictionary(Operator.GetProperty); }
        protected IPropertyOperator Operator { get; private set; }

        #region Events and Invocators

        public event EventHandler<DevicePropertyChangedEventArgs> PropertyChanged;

        protected virtual void OnPropertyChanged(DevicePropertyChangedEventArgs e)
        {
            EventHandler<DevicePropertyChangedEventArgs> handler = PropertyChanged;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<DevicePropertyCreatedEventArg> PropertyCreated;

        protected virtual void OnPropertyCreated(DevicePropertyCreatedEventArg e)
        {
            EventHandler<DevicePropertyCreatedEventArg> handler = PropertyCreated;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<DevicePropertyRemovedEventArgs> PropertyRemoved;

        #endregion

        void ICollection<KeyValuePair<int, int>>.Clear()
        {
            foreach (var key in _properties.Keys.ToList())
                Remove(key);
        }

        public void Add(int key, int value)
        {
            Operator.SetProperty(key, value);
            _properties.Add(key, value);
            OnPropertyCreated(new DevicePropertyCreatedEventArg(key, value));
        }

        public bool Remove(int key)
        {
            Operator.RemoveProperty(key);
            bool res = _properties.Remove(key);
            OnPropertyRemoved(new DevicePropertyRemovedEventArgs(key));
            return res;
        }

        protected virtual void OnPropertyRemoved(DevicePropertyRemovedEventArgs e)
        {
            EventHandler<DevicePropertyRemovedEventArgs> handler = PropertyRemoved;
            if (handler != null) handler(this, e);
        }

        private void SetValue(int Key, int Value)
        {
            int oldValue = _properties[Key];
            Operator.SetProperty(Key, Value);
            _properties[Key] = Value;
            OnPropertyChanged(new DevicePropertyChangedEventArgs(Key, oldValue, Value));
        }

        private int GetValue(int Key) { return _properties[Key]; }

        #region Реализация IDictionary

        IEnumerator<KeyValuePair<int, int>> IEnumerable<KeyValuePair<int, int>>.GetEnumerator() { return _properties.GetEnumerator(); }

        IEnumerator IEnumerable.GetEnumerator() { return ((IEnumerable)_properties).GetEnumerator(); }

        void ICollection<KeyValuePair<int, int>>.Add(KeyValuePair<int, int> item) { ((IDictionary<int, int>)this).Add(item.Key, item.Value); }

        bool ICollection<KeyValuePair<int, int>>.Contains(KeyValuePair<int, int> item) { return _properties.Contains(item); }

        void ICollection<KeyValuePair<int, int>>.CopyTo(KeyValuePair<int, int>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<int, int>>)_properties).CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<int, int>>.Remove(KeyValuePair<int, int> item) { return _properties.Remove(item.Key); }

        int ICollection<KeyValuePair<int, int>>.Count
        {
            get { return _properties.Count; }
        }

        bool ICollection<KeyValuePair<int, int>>.IsReadOnly
        {
            get { return false; }
        }

        bool IDictionary<int, int>.ContainsKey(int key) { return _properties.ContainsKey(key); }

        bool IDictionary<int, int>.TryGetValue(int key, out int value) { return _properties.TryGetValue(key, out value); }

        public int this[int key]
        {
            get { return GetValue(key); }
            set { SetValue(key, value); }
        }

        ICollection<int> IDictionary<int, int>.Keys
        {
            get { return _properties.Keys; }
        }

        ICollection<int> IDictionary<int, int>.Values
        {
            get { return _properties.Values; }
        }

        #endregion
    }
}
