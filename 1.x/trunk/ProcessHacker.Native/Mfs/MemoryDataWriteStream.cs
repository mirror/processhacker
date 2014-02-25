/*
 * Process Hacker - 
 *   MFS buffered write stream
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
using System.Collections.Generic;
using System.IO;
using System.Text;
using ProcessHacker.Common;

namespace ProcessHacker.Native.Mfs
{
    public class MemoryDataWriteStream : Stream
    {
        private MemoryObject _obj;
        private byte[] _buffer;
        private int _bufferLength;

        internal MemoryDataWriteStream(MemoryObject obj, int bufferSize)
        {
            _obj = obj;
            _buffer = new byte[bufferSize];
        }

        public override bool CanRead
        {
            get { return false; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanTimeout
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override long Length
        {
            get { return 0; }
        }

        public override long Position
        {
            get { return 0; }
            set { }
        }

        protected override void Dispose(bool disposing)
        {
            this.Flush();
            base.Dispose(disposing);
        }

        public override void Flush()
        {
            if (_bufferLength > 0)
            {
                _obj.AppendData(_buffer, 0, _bufferLength);
                _bufferLength = 0;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            int availableBufferLength = _buffer.Length - _bufferLength;

            Utils.ValidateBuffer(buffer, offset, count);

            if (availableBufferLength == 0)
            {
                this.Flush();
            }
            else
            {
                if (count < availableBufferLength)
                    availableBufferLength = count;

                Array.Copy(buffer, offset, _buffer, _bufferLength, availableBufferLength);
                _bufferLength += availableBufferLength;
                offset += availableBufferLength;
                count -= availableBufferLength;

                if (count > 0)
                    this.Flush();
            }

            if (count > 0)
            {
                _obj.AppendData(buffer, offset, count);
            }
        }

        public override void WriteByte(byte value)
        {
            if (_bufferLength >= _buffer.Length)
                this.Flush();

            _buffer[_bufferLength++] = value;
        }
    }
}
