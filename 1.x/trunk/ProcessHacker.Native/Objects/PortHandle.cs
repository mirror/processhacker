/*
 * Process Hacker - 
 *   port handle
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
using ProcessHacker.Native.Lpc;
using ProcessHacker.Native.Security;

namespace ProcessHacker.Native.Objects
{
    public sealed class PortHandle : NativeHandle<PortAccess>
    {
        public static PortHandle Create(
            string name,
            ObjectFlags objectFlags,
            DirectoryHandle rootDirectory
            )
        {
            return Create(
                name,
                objectFlags,
                rootDirectory,
                Win32.PortMessageMaxDataLength,
                Win32.PortMessageMaxLength,
                0
                );
        }

        public static PortHandle Create(
            string name,
            ObjectFlags objectFlags,
            DirectoryHandle rootDirectory,
            int maxConnectionInfoLength,
            int maxMessageLength,
            int maxPoolUsage
            )
        {
            NtStatus status;
            ObjectAttributes oa = new ObjectAttributes(name, objectFlags, rootDirectory);
            IntPtr handle;

            try
            {
                if ((status = Win32.NtCreatePort(
                    out handle,
                    ref oa,
                    maxConnectionInfoLength,
                    maxMessageLength,
                    maxPoolUsage
                    )) >= NtStatus.Error)
                    Win32.Throw(status);
            }
            finally
            {
                oa.Dispose();
            }

            return new PortHandle(handle, true);
        }

        public static PortHandle CreateWaitable(
            string name,
            ObjectFlags objectFlags,
            DirectoryHandle rootDirectory
            )
        {
            return CreateWaitable(
                name,
                objectFlags,
                rootDirectory,
                Win32.PortMessageMaxDataLength,
                Win32.PortMessageMaxLength,
                0
                );
        }

        public static PortHandle CreateWaitable(
            string name,
            ObjectFlags objectFlags,
            DirectoryHandle rootDirectory,
            int maxConnectionInfoLength,
            int maxMessageLength,
            int maxPoolUsage
            )
        {
            NtStatus status;
            ObjectAttributes oa = new ObjectAttributes(name, objectFlags, rootDirectory);
            IntPtr handle;

            try
            {
                if ((status = Win32.NtCreateWaitablePort(
                    out handle,
                    ref oa,
                    maxConnectionInfoLength,
                    maxMessageLength,
                    maxPoolUsage
                    )) >= NtStatus.Error)
                    Win32.Throw(status);
            }
            finally
            {
                oa.Dispose();
            }

            return new PortHandle(handle, true);
        }

        private PortHandle(IntPtr handle, bool owned)
            : base(handle, owned)
        { }

        public PortComHandle AcceptConnect(PortMessage message, bool accept)
        {
            NtStatus status;
            IntPtr portHandle;

            using (var messageMemory = message.ToMemory())
            {
                if ((status = Win32.NtAcceptConnectPort(
                    out portHandle,
                    IntPtr.Zero,
                    messageMemory,
                    accept,
                    IntPtr.Zero,
                    IntPtr.Zero
                    )) >= NtStatus.Error)
                    Win32.Throw(status);

                if (!NativeHandle.IsInvalid(portHandle))
                    return new PortComHandle(portHandle, true);
                else
                    return null;
            }
        }

        public void CompleteConnect()
        {
            NtStatus status;

            if ((status = Win32.NtCompleteConnectPort(this)) >= NtStatus.Error)
                Win32.Throw(status);
        }

        public PortMessage Listen()
        {
            NtStatus status;

            using (var buffer = PortMessage.AllocateBuffer())
            {
                if ((status = Win32.NtListenPort(this, buffer)) >= NtStatus.Error)
                    Win32.Throw(status);

                return new PortMessage(buffer);
            }
        }
    }
}
