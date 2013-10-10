using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fudp
{
    /// <summary>
    /// Интерфейс объекта, способного принимать изменения прогресса какого-то процесса
    /// </summary>
    public interface IProgressAcceptor
    {
        /// <summary>
        /// Метод, вызываемый при каждом изменении прогресса
        /// </summary>
        /// <param name="progress">Доля выполнения процесса (0..1)</param>
        void OnProgressChanged(double progress);
    }
}
