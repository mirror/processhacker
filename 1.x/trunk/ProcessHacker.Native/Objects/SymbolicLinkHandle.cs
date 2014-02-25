/*
 * Process Hacker - 
 *   symbolic link handle
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
    public sealed class SymbolicLinkHandle : NativeHandle<SymbolicLinkAccess>
    {
        public static SymbolicLinkHandle Create(SymbolicLinkAccess access, string name, string linkTarget)
        {
            return Create(access, name, 0, null, linkTarget);
        }

        public static SymbolicLinkHandle Create(SymbolicLinkAccess access, string name, ObjectFlags objectFlags, DirectoryHandle rootDirectory, string linkTarget)
        {
            NtStatus status;
            ObjectAttributes oa = new ObjectAttributes(name, objectFlags, rootDirectory);
            IntPtr handle;

            try
            {
                UnicodeString linkTargetString = new UnicodeString(linkTarget);

                try
                {
                    if ((status = Win32.NtCreateSymbolicLinkObject(out handle, access,
                        ref oa, ref linkTargetString)) >= NtStatus.Error)
                        Win32.Throw(status);
                }
                finally
                {
                    linkTargetString.Dispose();
                }
            }
            finally
            {
                oa.Dispose();
            }

            return new SymbolicLinkHandle(handle, true);
        }

        private SymbolicLinkHandle(IntPtr handle, bool owned)
            : base(handle, owned)
        { }

        public SymbolicLinkHandle(string name, ObjectFlags objectFlags, DirectoryHandle rootDirectory, SymbolicLinkAccess access)
        {
            NtStatus status;
            ObjectAttributes oa = new ObjectAttributes(name, objectFlags, rootDirectory);
            IntPtr handle;

            try
            {
                if ((status = Win32.NtOpenSymbolicLinkObject(out handle, access, ref oa)) >= NtStatus.Error)
                    Win32.Throw(status);
            }
            finally
            {
                oa.Dispose();
            }

            this.Handle = handle;
        }

        public SymbolicLinkHandle(string name, SymbolicLinkAccess access)
            : this(name, 0, null, access)
        { }

        public string GetTarget()
        {
            NtStatus status;
            int retLength;
            UnicodeString str = new UnicodeString();

            using (var buffer = new MemoryAlloc(0x200))
            {
                str.Length = 0;
                str.MaximumLength = (ushort)buffer.Size;
                str.Buffer = buffer;

                if ((status = Win32.NtQuerySymbolicLinkObject(this, ref str, out retLength)) >= NtStatus.Error)
                {
                    buffer.ResizeNew(retLength);
                    str.MaximumLength = (ushort)retLength;
                    str.Buffer = buffer;
                }

                if ((status = Win32.NtQuerySymbolicLinkObject(this, ref str, out retLength)) >= NtStatus.Error)
                    Win32.Throw(status);

                return str.Read();
            }
        }
    }
}
