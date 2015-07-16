using System;
using Communications;
using Fudp.Messages;

namespace Fudp
{
    /// <summary>����������, ����������� ������ ����������������</summary>
    public interface ICanProgSessionFactory
    {
        /// <summary>��������� ������ ��������� ���������� ��</summary>
        /// <param name="Port">������������ FUDP-����</param>
        /// <param name="Target">����� ���� ����������</param>
        /// <param name="Timeout"></param>
        /// <returns>�������� ������</returns>
        CanProgSession OpenSession(IFudpPort Port, DeviceTicket Target, TimeSpan Timeout);
    }
}