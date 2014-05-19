using Fudp.Model.Filesystem;
using Fudp.Model.PropStore;

namespace Fudp.Model
{
    /// <summary>Фабрика операторов</summary>
    public interface IOperatorsFactory
    {
        /// <summary>Получает элемент <seealso cref="IFileOperator" />.</summary>
        /// <returns>Файловый оператор</returns>
        IFileOperator GetFileOperator();

        /// <summary>Получает элемент <seealso cref="IPropertyOperator" />.</summary>
        /// <returns>Оператор параметров</returns>
        IPropertyOperator GetPropertyOperator();
    }
}
