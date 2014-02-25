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
using ProcessHacker.Native.Security;

namespace ProcessHacker.Native.Objects
{
    /// <summary>
    /// Represents a handle to a mailslot.
    /// </summary>
    public sealed class MailslotHandle : FileHandle
    {
        public static MailslotHandle Create(FileAccess access, string fileName, int maxMessageSize, long readTimeout)
        {
            return Create(
                access,
                fileName,
                ObjectFlags.CaseInsensitive,
                null,
                0,
                maxMessageSize,
                readTimeout,
                0
                );
        }

        public static MailslotHandle Create(
            FileAccess access,
            string fileName,
            ObjectFlags objectFlags,
            NativeHandle rootDirectory,
            int quota,
            int maxMessageSize,
            long readTimeout,
            FileCreateOptions createOptions
            )
        {
            NtStatus status;
            ObjectAttributes oa = new ObjectAttributes(fileName, objectFlags, rootDirectory);
            IoStatusBlock isb;
            IntPtr handle;

            try
            {
                if ((status = Win32.NtCreateMailslotFile(
                    out handle,
                    access,
                    ref oa,
                    out isb,
                    createOptions,
                    quota,
                    maxMessageSize,
                    ref readTimeout
                    )) >= NtStatus.Error)
                    Win32.Throw(status);
            }
            finally
            {
                oa.Dispose();
            }

            return new MailslotHandle(handle, true);
        }

        private MailslotHandle(IntPtr handle, bool owned)
            : base(handle, owned)
        { }  

        public MailslotHandle(string fileName, FileAccess access)
            : base(fileName, access)
        { }

        public MailslotHandle(string fileName, FileShareMode shareMode, FileAccess access)
            : base(fileName, shareMode, access)
        { }

        public MailslotHandle(string fileName, FileShareMode shareMode, FileCreateOptions openOptions, FileAccess access)
            : base(fileName, shareMode, openOptions, access)
        { }

        public MailslotHandle(
            string fileName,
            ObjectFlags objectFlags,
            NativeHandle rootDirectory,
            FileShareMode shareMode,
            FileCreateOptions openOptions,
            FileAccess access
            )
            : base(fileName, objectFlags, rootDirectory, shareMode, openOptions, access)
        { }
    }
}
