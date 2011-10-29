/*
 * Process Hacker - 
 *   key handle
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
    public class KeyHandle : NativeHandle<KeyAccess>
    {
        public static KeyHandle Create(
            KeyAccess access,
            string name,
            RegOptions createOptions
            )
        {
            return Create(access, name, 0, null, createOptions);
        }

        public static KeyHandle Create(
            KeyAccess access,
            string name,
            ObjectFlags objectFlags,
            KeyHandle rootDirectory,
            RegOptions createOptions
            )
        {
            KeyCreationDisposition creationDisposition;

            return Create(access, name, objectFlags, rootDirectory, createOptions, out creationDisposition);
        }

        public static KeyHandle Create(
            KeyAccess access,
            string name,
            ObjectFlags objectFlags,
            KeyHandle rootDirectory,
            RegOptions createOptions,
            out KeyCreationDisposition creationDisposition
            )
        {
            ObjectAttributes oa = new ObjectAttributes(name, objectFlags, rootDirectory);
            IntPtr handle;

            try
            {
                Win32.NtCreateKey(
                    out handle,
                    access,
                    ref oa,
                    0,
                    IntPtr.Zero,
                    createOptions,
                    out creationDisposition
                    ).ThrowIf();
            }
            finally
            {
                oa.Dispose();
            }

            return new KeyHandle(handle, true);
        }

        private KeyHandle(IntPtr handle, bool owned)
            : base(handle, owned)
        { }

        public KeyHandle(string name, KeyAccess access)
            : this(name, 0, null, access)
        { }

        public KeyHandle(string name, ObjectFlags objectFlags, KeyHandle rootDirectory, KeyAccess access)
        {
            ObjectAttributes oa = new ObjectAttributes(name, objectFlags, rootDirectory);
            IntPtr handle;

            try
            {
                Win32.NtOpenKey(
                    out handle,
                    access,
                    ref oa
                    ).ThrowIf();
            }
            finally
            {
                oa.Dispose();
            }

            this.Handle = handle;
        }

        public void Delete()
        {
            Win32.NtDeleteKey(this).ThrowIf();
        }

        public void DeleteValue(string name)
        {
            UnicodeString nameStr = new UnicodeString(name);

            try
            {
                Win32.NtDeleteValueKey(this, ref nameStr).ThrowIf();
            }
            finally
            {
                nameStr.Dispose();
            }
        }
    }
}
