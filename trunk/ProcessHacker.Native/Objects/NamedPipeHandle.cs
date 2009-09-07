/*
 * Process Hacker - 
 *   named pipe handle
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
using ProcessHacker.Native.Api;

namespace ProcessHacker.Native.Objects
{
    /// <summary>
    /// Represents a handle to a named pipe.
    /// </summary>
    public sealed class NamedPipeHandle : FileHandle
    {
        /// <summary>
        /// Waits for an instance of the specified named pipe to 
        /// become available for connection.
        /// </summary>
        /// <param name="name">The name of the named pipe.</param>
        /// <param name="timeout">
        /// The timeout, in milliseconds. Use zero for the default timeout interval 
        /// and -1 for an infinite timeout.
        /// </param>
        /// <returns>
        /// True if an instance of the pipe was available before the timeout 
        /// interval elapsed, otherwise false.
        /// </returns>
        public static bool Wait(string name, int timeout)
        {
            return Win32.WaitNamedPipe(name, timeout);
        }

        public NamedPipeHandle(string name, PipeAccessMode openMode, PipeMode pipeMode, int maxInstances,
            int outBufferSize, int inBufferSize, int defaultTimeOut)
        {
            this.Handle = Win32.CreateNamedPipe(name, openMode, pipeMode, maxInstances,
                outBufferSize, inBufferSize, defaultTimeOut, IntPtr.Zero);

            if (this.Handle == NativeHandle.MinusOne)
            {
                this.MarkAsInvalid();
                Win32.ThrowLastError();
            }
        }

        public void Connect()
        {
            if (!Win32.ConnectNamedPipe(this, IntPtr.Zero))
                Win32.ThrowLastError();
        }

        public void Disconnect()
        {
            if (!Win32.DisconnectNamedPipe(this))
                Win32.ThrowLastError();
        }
    }
}
