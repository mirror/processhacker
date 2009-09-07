/*
 * Process Hacker - 
 *   mailslot handle
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
    /// Represents a handle to a mailslot.
    /// </summary>
    public sealed class MailslotHandle : FileHandle
    {
        public static MailslotHandle Create(string name, int maxMessageSize, int readTimeout)
        {
            IntPtr handle;

            handle = Win32.CreateMailslot(name, maxMessageSize, readTimeout, IntPtr.Zero);

            if (handle == NativeHandle.MinusOne)
                Win32.ThrowLastError();

            return new MailslotHandle(handle);
        }

        private MailslotHandle(IntPtr handle)
        {
            this.Handle = handle;
        }
    }
}
