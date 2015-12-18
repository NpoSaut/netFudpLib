using System;
using System.Threading;
using Communications;
using Communications.Can;
using Fudp.Messages;

namespace Fudp
{
    /// <summary>Инструмент, открывающий сессию программирования</summary>
    public interface ICanProgSessionFactory
    {
        /// <summary>Открывает сессию удалённого обновления ПО</summary>
        /// <param name="CanPort"></param>
        /// <param name="Target">Билет цели обновления</param>
        /// <param name="CancellationToken"></param>
        /// <returns>Открытая сессия</returns>
        IProgSession OpenSession(ICanPort CanPort, DeviceTicket Target, CancellationToken CancellationToken);
    }
}