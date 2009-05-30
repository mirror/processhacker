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
using System.Collections.Generic;
using System.Text;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Security;

namespace ProcessHacker.Native.Objects
{
    public class PortHandle : NativeHandle<PortAccess>
    {
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
                    Win32.ThrowLastError(status);
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
                    Win32.ThrowLastError(status);
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
    }
}
