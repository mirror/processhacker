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
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Security;

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
            ObjectAttributes oa = new ObjectAttributes(name, objectFlags, rootDirectory);
            IntPtr handle;

            try
            {
                Win32.NtCreateMutant(out handle, access, ref oa, initialOwner).ThrowIf();
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
            ObjectAttributes oa = new ObjectAttributes(name, objectFlags, rootDirectory);
            IntPtr handle;

            try
            {
                Win32.NtOpenMutant(out handle, access, ref oa).ThrowIf();
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

        public MutantBasicInformation BasicInformation
        {
            get
            {
                MutantBasicInformation mbi;
                int retLength;

                Win32.NtQueryMutant(
                    this,
                    MutantInformationClass.MutantBasicInformation,
                    out mbi,
                    MutantBasicInformation.SizeOf,
                    out retLength
                    ).ThrowIf();

                return mbi;
            }
        }

        public MutantOwnerInformation OwnerInformation
        {
            get
            {
                MutantOwnerInformation moi;
                int retLength;

                Win32.NtQueryMutant(
                    this,
                    MutantInformationClass.MutantOwnerInformation,
                    out moi,
                    MutantOwnerInformation.SizeOf,
                    out retLength
                    ).ThrowIf();

                return moi;
            }
        }

        public int Release()
        {
            int previousCount;

            Win32.NtReleaseMutant(this, out previousCount).ThrowIf();

            return previousCount;
        }
    }
}
