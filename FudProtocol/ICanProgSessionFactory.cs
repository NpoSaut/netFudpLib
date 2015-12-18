using System;
using System.Threading;
using Communications;
using Communications.Can;
using Fudp.Messages;

namespace Fudp
{
    /// <summary>����������, ����������� ������ ����������������</summary>
    public interface ICanProgSessionFactory
    {
        /// <summary>��������� ������ ��������� ���������� ��</summary>
        /// <param name="CanPort"></param>
        /// <param name="Target">����� ���� ����������</param>
        /// <param name="CancellationToken"></param>
        /// <returns>�������� ������</returns>
        IProgSession OpenSession(ICanPort CanPort, DeviceTicket Target, CancellationToken CancellationToken);
    }
}