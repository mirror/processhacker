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
    /// Represents a handle to a file.
    /// </summary>
    public class NamedPipeHandle : FileHandle
    {
        public NamedPipeHandle(string name, PipeAccessMode openMode, PipeMode pipeMode, int maxInstances,
            int outBufferSize, int inBufferSize, int defaultTimeOut)
        {
            this.Handle = Win32.CreateNamedPipe(name, openMode, pipeMode, maxInstances,
                outBufferSize, inBufferSize, defaultTimeOut, 0);

            if (this.Handle == 0)
                Win32.ThrowLastError();
        }

        public void Connect()
        {
            if (!Win32.ConnectNamedPipe(this, 0))
                Win32.ThrowLastError();
        }

        public void Disconnect()
        {
            if (!Win32.DisconnectNamedPipe(this))
                Win32.ThrowLastError();
        }
    }

    [Flags]
    public enum PipeAccessMode : uint
    {
        Inbound = 0x1,
        Outbound = 0x2,
        Duplex = 0x3,
        FirstPipeInstance = 0x80000,
        WriteThrough = 0x80000000,
        Overlapped = 0x40000000,
        WriteDac = 0x40000,
        WriteOwner = 0x80000,
        AccessSystemSecurity = 0x01000000
    }

    [Flags]
    public enum PipeMode : int
    {
        TypeByte = 0x0,
        TypeMessage = 0x4,
        ReadModeByte = 0x0,
        ReadModeMessage = 0x2,
        Wait = 0x0,
        NoWait = 0x1,
        AcceptRemoteClients = 0x0,
        RejectRemoteClients = 0x8
    }

    [Flags]
    public enum PipeState : int
    {
        NoWait = 0x1,
        ReadModeMessage = 0x2
    }
}
