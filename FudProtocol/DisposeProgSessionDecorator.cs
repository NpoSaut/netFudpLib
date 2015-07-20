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
        public IList<DevFileInfo> ListFiles()
        {
            return _core.ListFiles();
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
        public void DeleteFile(string FileName)
        {
            _core.DeleteFile(FileName);
        }

        /// <summary>������� �� �������� �����</summary>
        /// <param name="fileInfo">���������� � ����������� �����</param>
        /// <param name="ProgressAcceptor">������� ��������� ���������� �����</param>
        /// <param name="CancelToken">����� ������</param>
        /// <returns></returns>
        public void CreateFile(DevFileInfo fileInfo, IProgressAcceptor ProgressAcceptor = null, CancellationToken CancelToken = new CancellationToken())
        {
            _core.CreateFile(fileInfo, ProgressAcceptor, CancelToken);
        }

        /// <summary>������� �� �������� ��� ��������� ������ � ������� �������</summary>
        /// <param name="paramKey">����</param>
        /// <param name="paramValue">�������� ��������</param>
        public void SetProperty(byte paramKey, int paramValue)
        {
            _core.SetProperty(paramKey, paramValue);
        }

        /// <summary>�������� ������ �� ������� �������</summary>
        /// <param name="paramKey">����</param>
        public void DeleteProperty(byte paramKey)
        {
            _core.DeleteProperty(paramKey);
        }

        /// <summary>���������� ������ �� ���������� ������ ����������������</summary>
        public SubmitAckStatus Submit(SubmitStatus Status)
        {
            return _core.Submit(Status);
        }
    }
}
