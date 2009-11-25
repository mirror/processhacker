/*
 * Process Hacker - 
 *   mutant handle
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
using System.Runtime.InteropServices;

namespace ProcessHacker.Native.Objects
{
    public sealed class MutantHandle : NativeHandle<MutantAccess>
    {
        public static MutantHandle Create(MutantAccess access, bool initialOwner)
        {
            return Create(access, null, initialOwner);
        }

        public static MutantHandle Create(MutantAccess access, string name, bool initialOwner)
        {
            return Create(access, name, 0, null, initialOwner);
        }

        public static MutantHandle Create(MutantAccess access, string name, ObjectFlags objectFlags, DirectoryHandle rootDirectory, bool initialOwner)
        {
            NtStatus status;
            ObjectAttributes oa = new ObjectAttributes(name, objectFlags, rootDirectory);
            IntPtr handle;

            try
            {
                if ((status = Win32.NtCreateMutant(out handle, access, ref oa, initialOwner)) >= NtStatus.Error)
                    Win32.Throw(status);
            }
            finally
            {
                oa.Dispose();
            }

            return new MutantHandle(handle, true);
        }

        public static MutantHandle FromHandle(IntPtr handle)
        {
            return new MutantHandle(handle, false);
        }

        private MutantHandle(IntPtr handle, bool owned)
            : base(handle, owned)
        { }

        public MutantHandle(string name, ObjectFlags objectFlags, DirectoryHandle rootDirectory, MutantAccess access)
        {
            NtStatus status;
            ObjectAttributes oa = new ObjectAttributes(name, objectFlags, rootDirectory);
            IntPtr handle;

            try
            {
                if ((status = Win32.NtOpenMutant(out handle, access, ref oa)) >= NtStatus.Error)
                    Win32.Throw(status);
            }
            finally
            {
                oa.Dispose();
            }

            this.Handle = handle;
        }

        public MutantHandle(string name, MutantAccess access)
            : this(name, 0, null, access)
        { }

        public MutantBasicInformation GetBasicInformation()
        {
            NtStatus status;
            MutantBasicInformation mbi;
            int retLength;

            if ((status = Win32.NtQueryMutant(this, MutantInformationClass.MutantBasicInformation,
                out mbi, Marshal.SizeOf(typeof(MutantBasicInformation)), out retLength)) >= NtStatus.Error)
                Win32.Throw(status);

            return mbi;
        }

        public MutantOwnerInformation GetOwnerInformation()
        {
            NtStatus status;
            MutantOwnerInformation moi;
            int retLength;

            if ((status = Win32.NtQueryMutant(this, MutantInformationClass.MutantOwnerInformation,
                out moi, Marshal.SizeOf(typeof(MutantOwnerInformation)), out retLength)) >= NtStatus.Error)
                Win32.Throw(status);

            return moi;
        }

        public int Release()
        {
            NtStatus status;
            int previousCount;

            if ((status = Win32.NtReleaseMutant(this, out previousCount)) >= NtStatus.Error)
                Win32.Throw(status);

            return previousCount;
        }
    }
}
