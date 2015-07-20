using System;
using Communications;
using Communications.Can;
using Fudp.Messages;

namespace Fudp
{
    /// <summary>����������, ����������� ������ ����������������</summary>
    public interface ICanProgSessionFactory
    {
        /// <summary>��������� ������ ��������� ���������� ��</summary>
        /// <param name="Target">����� ���� ����������</param>
        /// <param name="CanPort"></param>
        /// <returns>�������� ������</returns>
        IProgSession OpenSession(ICanPort CanPort, DeviceTicket Target);
    }
}