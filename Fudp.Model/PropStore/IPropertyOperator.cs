using System.Collections.Generic;

namespace Fudp.Model.PropStore
{
    /// <summary>Оператор свойств</summary>
    public interface IPropertyOperator
    {
        /// <summary>Перечисляет ключи всех установленных свойств</summary>
        IEnumerable<int> EnumerateKeys();

        /// <summary>Получает значения свойства по ключу</summary>
        /// <param name="Key">Ключ свойства</param>
        /// <returns>Значение свойства</returns>
        int GetProperty(int Key);

        /// <summary>Устанавливает значение свойства, создавая его при необходимости</summary>
        /// <param name="Key">Ключ свойства</param>
        /// <param name="Value">Значение свойства</param>
        void SetProperty(int Key, int Value);

        /// <summary>Удаляет свойства с указанным ключём</summary>
        /// <param name="Key">Ключ свойства для удаления</param>
        void RemoveProperty(int Key);
    }
}
