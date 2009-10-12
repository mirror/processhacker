/*
 * Process Hacker - 
 *   byte stream reader
 * 
 * Copyright (C) 2008-2009 wj32
 * 
 * This file is part of PNG.Net.
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

namespace ProcessHacker.Common
{
    public sealed class ByteStreamReader : Stream
    {
        private byte[] _data;
        private long _position;

        public ByteStreamReader(byte[] data) 
        {
            _data = data;
            _position = 0;
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void Flush()
        {
            // don't need to flush
        }

        public override long Length
        {
            get { return _data.LongLength; }
        }

        public override long Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            long length = (_position + count > _data.Length) ? _data.Length - _position - 1 : count;

            if (_position >= _data.Length)
                return 0;

            for (long i = 0; i < length; i++, _position++)
                buffer[offset + i] = _data[_position];

            return (int)length;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    _position = offset;
                    break;

                case SeekOrigin.Current:
                    _position += offset;
                    break;

                case SeekOrigin.End:
                    _position = _data.Length - 1 + offset;
                    break;

                default:
                    throw new ArgumentException();
            }

            return _position;
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
    }
}
