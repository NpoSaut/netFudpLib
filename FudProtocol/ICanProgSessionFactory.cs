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
        /// <returns>�������� ������</returns>
        CanProgSession OpenSession(IFudpPort Port, DeviceTicket Target);
    }
}