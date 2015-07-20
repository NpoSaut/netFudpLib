using System;
using Communications;
using Communications.Can;
using Fudp.Messages;

namespace Fudp
{
    /// <summary>Инструмент, открывающий сессию программирования</summary>
    public interface ICanProgSessionFactory
    {
        /// <summary>Открывает сессию удалённого обновления ПО</summary>
        /// <param name="Target">Билет цели обновления</param>
        /// <param name="CanPort"></param>
        /// <returns>Открытая сессия</returns>
        IProgSession OpenSession(ICanPort CanPort, DeviceTicket Target);
    }
}