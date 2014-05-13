using System;

namespace Fudp.Model
{
    public class DeviceFileEventArgs : EventArgs
    {
        public DeviceFileEventArgs(DeviceFileInfo File) { this.File = File; }
        public DeviceFileInfo File { get; private set; }
    }
}