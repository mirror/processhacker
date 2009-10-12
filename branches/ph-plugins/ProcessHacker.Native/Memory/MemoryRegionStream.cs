/*
 * Process Hacker - 
 *   memory region stream
 * 
 * Copyright (C) 2009 wj32
 * 
 * This file is part of Process Hacker.
 * 
 * Process Hacker is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * Process Hacker is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Process Hacker.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.IO;
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
