using Communications;
using Fudp.Messages;

namespace Fudp
{
    /// <summary>Инструмент, открывающий сессию программирования</summary>
    public interface ICanProgSessionFactory
    {
        /// <summary>Открывает сессию удалённого обновления ПО</summary>
        /// <param name="Port">Используемый FUDP-порт</param>
        /// <param name="Target">Билет цели обновления</param>
        /// <returns>Открытая сессия</returns>
        CanProgSession OpenSession(IFudpPort Port, DeviceTicket Target);
    }
}