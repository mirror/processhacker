/*
 * Process Hacker - 
 *   port communication handle
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
    public sealed class PortComHandle : NativeHandle<PortAccess>
    {
        public static PortComHandle Connect(string portName)
        {
            NtStatus status;
            UnicodeString portNameStr = new UnicodeString(portName);
            SecurityQualityOfService securityQos = 
                new SecurityQualityOfService(SecurityImpersonationLevel.SecurityImpersonation, true, false);
            IntPtr handle;

            try
            {
                if ((status = Win32.NtConnectPort(
                    out handle,
                    ref portNameStr,
                    ref securityQos,
                    IntPtr.Zero,
                    IntPtr.Zero,
                    IntPtr.Zero,
                    IntPtr.Zero,
                    IntPtr.Zero
                    )) >= NtStatus.Error)
                    Win32.Throw(status);
            }
            finally
            {
                portNameStr.Dispose();
            }

            return new PortComHandle(handle, true);
        }

        internal PortComHandle(IntPtr handle, bool owned)
            : base(handle, owned)
        { }

        public void Reply(PortMessage message)
        {
            NtStatus status;

            using (var messageMemory = message.ToMemory())
            {
                if ((status = Win32.NtReplyPort(this, messageMemory)) >= NtStatus.Error)
                    Win32.Throw(status);

                message.SetHeader(messageMemory);
            }
        }

        public PortMessage ReplyWaitReceive()
        {
            return this.ReplyWaitReceive(null);
        }

        public PortMessage ReplyWaitReceive(PortMessage message)
        {
            NtStatus status;
            IntPtr context;

            using (var buffer = PortMessage.AllocateBuffer())
            {
                MemoryAlloc messageMemory = null;

                if (message != null)
                    messageMemory = message.ToMemory();

                try
                {
                    if ((status = Win32.NtReplyWaitReceivePort(
                        this,
                        out context,
                        messageMemory ?? IntPtr.Zero,
                        buffer
                        )) >= NtStatus.Error)
                        Win32.Throw(status);

                    if (message != null)
                        message.SetHeader(messageMemory);
                }
                finally
                {
                    if (messageMemory != null)
                        messageMemory.Dispose();
                }

                return new PortMessage(buffer);
            }
        }

        public PortMessage ReplyWaitReply(PortMessage message)
        {
            NtStatus status;

            using (var messageMemory = message.ToMemory())
            {
                if ((status = Win32.NtReplyWaitReplyPort(
                    this,
                    messageMemory
                    )) >= NtStatus.Error)
                    Win32.Throw(status);

                return new PortMessage(messageMemory);
            }
        }

        public void Request(PortMessage message)
        {
            NtStatus status;

            using (var messageMemory = message.ToMemory())
            {
                if ((status = Win32.NtRequestPort(this, messageMemory)) >= NtStatus.Error)
                    Win32.Throw(status);

                message.SetHeader(messageMemory);
            }
        }

        public PortMessage RequestWaitReply(PortMessage message)
        {
            NtStatus status;

            using (var buffer = PortMessage.AllocateBuffer())
            using (var messageMemory = message.ToMemory())
            {
                if ((status = Win32.NtRequestWaitReplyPort(
                    this,
                    messageMemory,
                    buffer
                    )) >= NtStatus.Error)
                    Win32.Throw(status);

                message.SetHeader(messageMemory);

                return new PortMessage(buffer);
            }
        }
    }
}
