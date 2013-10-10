using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Fudp
{
    public class FudpFileStream : Stream
    {
        public CanProg Session { get; set; }

        public override bool CanRead { get { return false; } }
        public override bool CanSeek { get { return true; } }
        public override bool CanWrite { get { return true; } }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        private long _Length;

        public override long Length
        {
            get { return _Length; }
        }
        
        public override long Position { get; set; }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin: Position = offset; break;
                case SeekOrigin.Current: Position += offset; break;
                case SeekOrigin.End: Position = Length - 1 + offset; break;
            }
            return Position;
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
    }
}
