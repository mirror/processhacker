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
    public sealed class KeyedEventHandle : NativeHandle<KeyedEventAccess>
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
                    Win32.Throw(status);
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

        public KeyedEventHandle(string name, KeyedEventAccess access)
            : this(name, null, 0, access)
        { }

        public KeyedEventHandle(string name, DirectoryHandle rootDirectory, ObjectFlags objectFlags, KeyedEventAccess access)
        {
            NtStatus status;
            ObjectAttributes oa = new ObjectAttributes(name, objectFlags, rootDirectory);
            IntPtr handle;

            try
            {
                if ((status = Win32.NtOpenKeyedEvent(out handle, access, ref oa)) >= NtStatus.Error)
                    Win32.Throw(status);
            }
            finally
            {
                oa.Dispose();
            }

            this.Handle = handle;
        }

        public NtStatus ReleaseKey(int key)
        {
            return this.ReleaseKey(key, false);
        }

        public NtStatus ReleaseKey(int key, long timeout)
        {
            return this.ReleaseKey(key, false, timeout);
        }

        public NtStatus ReleaseKey(int key, bool alertable)
        {
            return this.ReleaseKey(new IntPtr(key), alertable, long.MinValue, false);
        }

        public NtStatus ReleaseKey(int key, bool alertable, long timeout)
        {
            return this.ReleaseKey(new IntPtr(key), alertable, timeout, true);
        }

        public NtStatus ReleaseKey(IntPtr key, bool alertable, long timeout, bool relative)
        {
            NtStatus status;
            long realTimeout = relative ? -timeout : timeout;

            if (key.ToInt64() % 2 != 0)
                throw new ArgumentException("Key must be divisible by 2.");

            if ((status = Win32.NtReleaseKeyedEvent(
                this,
                key,
                alertable,
                ref realTimeout
                )) >= NtStatus.Error)
                Win32.Throw(status);

            return status;
        }

        public NtStatus WaitKey(int key)
        {
            return this.WaitKey(key, false);
        }

        public NtStatus WaitKey(int key, long timeout)
        {
            return this.WaitKey(key, false, timeout);
        }

        public NtStatus WaitKey(int key, bool alertable)
        {
            return this.WaitKey(new IntPtr(key), alertable, long.MinValue, false);
        }

        public NtStatus WaitKey(int key, bool alertable, long timeout)
        {
            return this.WaitKey(new IntPtr(key), alertable, timeout, true);
        }

        public NtStatus WaitKey(IntPtr key, bool alertable, long timeout, bool relative)
        {
            NtStatus status;
            long realTimeout = relative ? -timeout : timeout;

            if (key.ToInt64() % 2 != 0)
                throw new ArgumentException("Key must be divisible by 2.");

            if ((status = Win32.NtWaitForKeyedEvent(
                this,
                key,
                alertable,
                ref realTimeout
                )) >= NtStatus.Error)
                Win32.Throw(status);

            return status;
        }
    }
}
