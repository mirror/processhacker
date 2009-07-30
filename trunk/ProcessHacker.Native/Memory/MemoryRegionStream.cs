using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;

namespace ProcessHacker.Native
{
    public class MemoryRegionStream : Stream
    {
        private MemoryRegion _memory;
        private long _position = 0;

        public MemoryRegionStream(MemoryRegion memory)
        {
            _memory = memory;
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return true; }
        }

        public override bool CanTimeout
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override void Flush()
        {
            // Do nothing
        }

        public override long Length
        {
            get { return _memory.Size; }
        }

        public override long Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            Marshal.Copy(_memory.Memory.Increment(_position += count), buffer, offset, count);

            return count;
        }

        public override int ReadByte()
        {
            return Marshal.ReadByte(_memory.Memory.Increment(_position++));
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            if (origin == SeekOrigin.Begin)
                _position = offset;
            else if (origin == SeekOrigin.Current)
                _position += offset;
            else if (origin == SeekOrigin.End)
                _position = _memory.Size + offset;

            return _position;
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            Marshal.Copy(buffer, offset, _memory.Memory.Increment(_position += count), count);
        }

        public override void WriteByte(byte value)
        {
            Marshal.WriteByte(_memory.Memory.Increment(_position++), value);
        }
    }
}
