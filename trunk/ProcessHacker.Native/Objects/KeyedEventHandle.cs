/*
 * Process Hacker - 
 *   keyed event handle
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
    public class KeyedEventHandle : Win32Handle<KeyedEventAccess>
    {
        public static KeyedEventHandle Create(KeyedEventAccess access)
        {
            return Create(access, null);
        }

        public static KeyedEventHandle Create(KeyedEventAccess access, string name)
        {
            return Create(access, name, 0, null);
        }

        public static KeyedEventHandle Create(KeyedEventAccess access, string name, ObjectFlags objectFlags, DirectoryHandle rootDirectory)
        {
            NtStatus status;
            ObjectAttributes oa = new ObjectAttributes(name, objectFlags, rootDirectory);
            IntPtr handle;

            try
            {
                if ((status = Win32.NtCreateKeyedEvent(out handle, access, ref oa, 0)) >= NtStatus.Error)
                    Win32.ThrowLastError(status);
            }
            finally
            {
                oa.Dispose();
            }

            return new KeyedEventHandle(handle, true);
        }

        private KeyedEventHandle(IntPtr handle, bool owned)
            : base(handle, owned)
        { }

        public KeyedEventHandle(string name, DirectoryHandle rootDirectory, ObjectFlags objectFlags, KeyedEventAccess access)
        {
            NtStatus status;
            ObjectAttributes oa = new ObjectAttributes(name, objectFlags, rootDirectory);
            IntPtr handle;

            try
            {
                if ((status = Win32.NtOpenKeyedEvent(out handle, access, ref oa)) >= NtStatus.Error)
                    Win32.ThrowLastError(status);
            }
            finally
            {
                oa.Dispose();
            }

            this.Handle = handle;
        }

        public KeyedEventHandle(string name, KeyedEventAccess access)
            : this(name, null, 0, access)
        { }

        public void Release(IntPtr key, bool alertable, long timeout)
        {
            NtStatus status;

            if ((status = Win32.NtReleaseKeyedEvent(this, key, alertable, ref timeout)) >= NtStatus.Error)
                Win32.ThrowLastError(status);
        }

        public void Release(int key, bool alertable, long timeout)
        {
            this.Release(new IntPtr(key), alertable, timeout);
        }

        public void Release(int key, long timeout)
        {
            this.Release(key, false, timeout);
        }

        public void Release(int key)
        {
            this.Release(key, -1);
        }

        public void Wait(IntPtr key, bool alertable, long timeout)
        {
            NtStatus status;

            if ((status = Win32.NtWaitForKeyedEvent(this, key, alertable, ref timeout)) >= NtStatus.Error)
                Win32.ThrowLastError(status);
        }

        public void Wait(int key, bool alertable, long timeout)
        {
            this.Wait(new IntPtr(key), alertable, timeout);
        }

        public void Wait(int key, long timeout)
        {
            this.Wait(key, false, timeout);
        }

        public void Wait(int key)
        {
            this.Wait(key, -1);
        }
    }
}
