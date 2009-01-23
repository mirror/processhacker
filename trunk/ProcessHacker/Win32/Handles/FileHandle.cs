/*
 * Process Hacker - 
 *   file handle
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
using System.Runtime.InteropServices;

namespace ProcessHacker
{
    public partial class Win32
    {
        /// <summary>
        /// Represents a handle to a file.
        /// </summary>
        public class FileHandle : Win32Handle
        {
            public FileHandle(string fileName, FILE_RIGHTS desiredAccess, FILE_SHARE_MODE shareMode,
                FILE_CREATION_DISPOSITION creationDisposition)
            {
                this.Handle = CreateFile(fileName, desiredAccess, shareMode, 0, creationDisposition, 0, 0);

                if (this.Handle == 0)
                    ThrowLastWin32Error();
            }

            public FileHandle(string fileName, FILE_RIGHTS desiredAccess, FILE_SHARE_MODE shareMode)
                : this(fileName, desiredAccess, shareMode, FILE_CREATION_DISPOSITION.OpenExisting)
            { }

            public FileHandle(string fileName, FILE_RIGHTS desiredAccess)
                : this(fileName, desiredAccess, FILE_SHARE_MODE.Exclusive)
            { }

            /// <summary>
            /// Sends an I/O control message to the device's associated driver.
            /// </summary>
            /// <param name="controlCode">The device-specific control code.</param>
            /// <param name="inBuffer">The input.</param>
            /// <param name="outBuffer">The output buffer.</param>
            /// <returns>The bytes returned in the output buffer.</returns>
            public int IoControl(uint controlCode, byte[] inBuffer, byte[] outBuffer)
            {
                int returnBytes;
                byte[] inArr = inBuffer;
                int inLen = inArr != null ? inBuffer.Length : 0;
                byte[] outArr = outBuffer;
                int outLen = outArr != null ?outBuffer.Length : 0;

                if (!DeviceIoControl(this, (int)controlCode, inArr, inLen, outArr, outLen, out returnBytes, 0))
                    ThrowLastWin32Error();

                return returnBytes;
            }

            /// <summary>
            /// Reads data from the file.
            /// </summary>
            /// <param name="buffer">The buffer to store the data in.</param>
            /// <returns>The number of bytes read from the file.</returns>
            public int Read(byte[] buffer)
            {
                int bytesRead;

                if (!ReadFile(this, buffer, buffer.Length, out bytesRead, 0))
                    ThrowLastWin32Error();

                return bytesRead;
            }

            /// <summary>
            /// Reads data from the file.
            /// </summary>
            /// <param name="length">The length to read.</param>
            /// <returns>The read data.</returns>
            public byte[] Read(int length)
            {
                byte[] buffer = new byte[length];

                this.Read(buffer);

                return buffer;
            }

            /// <summary>
            /// Writes data to the file.
            /// </summary>
            /// <param name="buffer">The data.</param>
            /// <returns>The number of bytes written to the file.</returns>
            public int Write(byte[] buffer)
            {
                int bytesWritten;

                if (!WriteFile(this, buffer, buffer.Length, out bytesWritten, 0))
                    ThrowLastWin32Error();

                return bytesWritten;
            }
        }
    }
}
