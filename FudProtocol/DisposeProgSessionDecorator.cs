using System;
using System.Collections.Generic;
using System.Threading;
using Fudp.Messages;

namespace Fudp
{
    internal class DisposeProgSessionDecorator : IProgSession
    {
        private readonly IProgSession _core;
        private readonly ICollection<IDisposable> _itemsToDispose;

        public DisposeProgSessionDecorator(IProgSession Core, params IDisposable[] ItemsToDispose)
            : this(Core, (ICollection<IDisposable>)ItemsToDispose) { }

        public DisposeProgSessionDecorator(IProgSession Core, ICollection<IDisposable> ItemsToDispose)
        {
            _core = Core;
            _itemsToDispose = ItemsToDispose;
        }

        /// <summary>
        ///     ��������� ������������ ����������� ������, ��������� � ���������, �������������� ��� ������� �������������
        ///     ��������.
        /// </summary>
        public void Dispose()
        {
            _core.Dispose();
            foreach (IDisposable disposableItem in _itemsToDispose)
                disposableItem.Dispose();
        }

        /// <summary>����� ����������, � ������� ����������� ������</summary>
        public DeviceTicket Device
        {
            get { return _core.Device; }
        }

        /// <summary>����������� ������ ������ �� ����������</summary>
        /// <param name="CancellationToken"></param>
        public IList<DevFileInfo> ListFiles(CancellationToken CancellationToken)
        {
            return _core.ListFiles(CancellationToken);
        }

        /// <summary>���������� ������ ����������� �����</summary>
        /// <param name="File">���� ��� ������</param>
        /// <param name="ProgressAcceptor">��������</param>
        /// <param name="CancellationToken">����� ������ ��������</param>
        public byte[] ReadFile(DevFileInfo File, IProgressAcceptor ProgressAcceptor, CancellationToken CancellationToken)
        {
            return _core.ReadFile(File, ProgressAcceptor, CancellationToken);
        }

        /// <summary>������� ��������� ���� � ����������</summary>
        /// <param name="FileName">���� � ����� ��� ��������</param>
        /// <param name="CancellationToken"></param>
        public void DeleteFile(string FileName, CancellationToken CancellationToken)
        {
            _core.DeleteFile(FileName, CancellationToken);
        }

        /// <summary>������� �� �������� �����</summary>
        /// <param name="fileInfo">���������� � ����������� �����</param>
        /// <param name="CancellationToken">����� ������</param>
        /// <param name="ProgressAcceptor">������� ��������� ���������� �����</param>
        /// <returns></returns>
        public void CreateFile(DevFile fileInfo, CancellationToken CancellationToken = default(CancellationToken), IProgressAcceptor ProgressAcceptor = null)
        {
            _core.CreateFile(fileInfo, CancellationToken: CancellationToken, ProgressAcceptor: ProgressAcceptor);
        }

        /// <summary>������� �� �������� ��� ��������� ������ � ������� �������</summary>
        /// <param name="paramKey">����</param>
        /// <param name="paramValue">�������� ��������</param>
        /// <param name="CancellationToken"></param>
        public void SetProperty(byte paramKey, int paramValue, CancellationToken CancellationToken)
        {
            _core.SetProperty(paramKey, paramValue, CancellationToken);
        }

        /// <summary>�������� ������ �� ������� �������</summary>
        /// <param name="paramKey">����</param>
        /// <param name="CancellationToken"></param>
        public void DeleteProperty(byte paramKey, CancellationToken CancellationToken)
        {
            _core.DeleteProperty(paramKey, CancellationToken);
        }

        /// <summary>���������� ������ �� ���������� ������ ����������������</summary>
        public SubmitAckStatus Submit(SubmitStatus Status, CancellationToken CancellationToken)
        {
            return _core.Submit(Status, CancellationToken);
        }
    }
}
