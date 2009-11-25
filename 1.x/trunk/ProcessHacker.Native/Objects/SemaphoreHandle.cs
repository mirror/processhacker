/*
 * Process Hacker - 
 *   semaphore handle
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
using System.Runtime.InteropServices;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Security;

namespace ProcessHacker.Native.Objects
{
    public sealed class SemaphoreHandle : NativeHandle<SemaphoreAccess>
    {
        public static SemaphoreHandle Create(SemaphoreAccess access, int initialCount, int maximumCount)
        {
            return Create(access, null, initialCount, maximumCount);
        }

        public static SemaphoreHandle Create(SemaphoreAccess access, string name, int initialCount, int maximumCount)
        {
            return Create(access, name, 0, null, initialCount, maximumCount);
        }

        public static SemaphoreHandle Create(SemaphoreAccess access, string name, ObjectFlags objectFlags, DirectoryHandle rootDirectory, int initialCount, int maximumCount)
        {
            NtStatus status;
            ObjectAttributes oa = new ObjectAttributes(name, objectFlags, rootDirectory);
            IntPtr handle;

            try
            {
                if ((status = Win32.NtCreateSemaphore(out handle, access, ref oa,
                    initialCount, maximumCount)) >= NtStatus.Error)
                    Win32.Throw(status);
            }
            finally
            {
                oa.Dispose();
            }

            return new SemaphoreHandle(handle, true);
        }

        public static SemaphoreHandle FromHandle(IntPtr handle)
        {
            return new SemaphoreHandle(handle, false);
        }

        private SemaphoreHandle(IntPtr handle, bool owned)
            : base(handle, owned)
        { }

        public SemaphoreHandle(string name, ObjectFlags objectFlags, DirectoryHandle rootDirectory, SemaphoreAccess access)
        {
            NtStatus status;
            ObjectAttributes oa = new ObjectAttributes(name, objectFlags, rootDirectory);
            IntPtr handle;

            try
            {
                if ((status = Win32.NtOpenSemaphore(out handle, access, ref oa)) >= NtStatus.Error)
                    Win32.Throw(status);
            }
            finally
            {
                oa.Dispose();
            }

            this.Handle = handle;
        }

        public SemaphoreHandle(string name, SemaphoreAccess access)
            : this(name, 0, null, access)
        { }

        public SemaphoreBasicInformation GetBasicInformation()
        {
            NtStatus status;
            SemaphoreBasicInformation sbi;
            int retLength;

            if ((status = Win32.NtQuerySemaphore(this, SemaphoreInformationClass.SemaphoreBasicInformation,
                out sbi, Marshal.SizeOf(typeof(SemaphoreBasicInformation)), out retLength)) >= NtStatus.Error)
                Win32.Throw(status);

            return sbi;
        }

        public int Release()
        {
            return this.Release(1);
        }

        public int Release(int count)
        {
            NtStatus status;
            int previousCount;

            if ((status = Win32.NtReleaseSemaphore(this, count, out previousCount)) >= NtStatus.Error)
                Win32.Throw(status);

            return previousCount;
        }
    }
}
