/*
 * Process Hacker - 
 *   section handle
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
    public sealed class SectionHandle : NativeHandle<SectionAccess>
    {
        public static SectionHandle Create(
            SectionAccess access,
            SectionAttributes sectionAttributes,
            MemoryProtection pageAttributes,
            FileHandle fileHandle
            )
        {
            return Create(access, 0, sectionAttributes, pageAttributes, fileHandle);
        }

        public static SectionHandle Create(
            SectionAccess access,
            long maximumSize,
            SectionAttributes sectionAttributes,
            MemoryProtection pageAttributes,
            FileHandle fileHandle
            )
        {
            return Create(access, null, maximumSize, sectionAttributes, pageAttributes, fileHandle);
        }

        public static SectionHandle Create(
            SectionAccess access,
            long maximumSize,
            SectionAttributes sectionAttributes,
            MemoryProtection pageAttributes
            )
        {
            return Create(access, null, maximumSize, sectionAttributes, pageAttributes, null);
        }

        public static SectionHandle Create(
            SectionAccess access,
            string name,
            long maximumSize,
            SectionAttributes sectionAttributes,
            MemoryProtection pageAttributes,
            FileHandle fileHandle
            )
        {
            return Create(access, name, 0, null, maximumSize, sectionAttributes, pageAttributes, fileHandle);
        }

        public static SectionHandle Create(
            SectionAccess access,
            string name,
            ObjectFlags objectFlags,
            DirectoryHandle rootDirectory,
            long maximumSize,
            SectionAttributes sectionAttributes,
            MemoryProtection pageAttributes,
            FileHandle fileHandle
            )
        {
            NtStatus status;
            ObjectAttributes oa = new ObjectAttributes(name, objectFlags, rootDirectory);
            IntPtr handle;

            try
            {
                if (maximumSize != 0)
                {
                    if ((status = Win32.NtCreateSection(
                        out handle,
                        access,
                        ref oa,
                        ref maximumSize,
                        pageAttributes,
                        sectionAttributes,
                        fileHandle ?? IntPtr.Zero
                        )) >= NtStatus.Error)
                        Win32.Throw(status);
                }
                else
                {
                    if ((status = Win32.NtCreateSection(
                        out handle,
                        access,
                        ref oa,
                        IntPtr.Zero,
                        pageAttributes,
                        sectionAttributes,
                        fileHandle ?? IntPtr.Zero
                        )) >= NtStatus.Error)
                        Win32.Throw(status);
                }
            }
            finally
            {
                oa.Dispose();
            }

            return new SectionHandle(handle, true);
        }

        public static SectionHandle FromHandle(IntPtr handle)
        {
            return new SectionHandle(handle, false);
        }

        private SectionHandle(IntPtr handle, bool owned)
            : base(handle, owned)
        { }

        public SectionHandle(string name, ObjectFlags objectFlags, DirectoryHandle rootDirectory, SectionAccess access)
        {
            NtStatus status;
            ObjectAttributes oa = new ObjectAttributes(name, objectFlags, rootDirectory);
            IntPtr handle;

            try
            {
                if ((status = Win32.NtOpenSection(out handle, access, ref oa)) >= NtStatus.Error)
                    Win32.Throw(status);
            }
            finally
            {
                oa.Dispose();
            }

            this.Handle = handle;
        }

        public SectionHandle(string name, SectionAccess access)
            : this(name, 0, null, access)
        { }

        public long Extend(long newSize)
        {
            NtStatus status;

            if ((status = Win32.NtExtendSection(this, ref newSize)) >= NtStatus.Error)
                Win32.Throw(status);

            return newSize;
        }

        public SectionBasicInformation GetBasicInformation()
        {
            NtStatus status;
            SectionBasicInformation sbi;
            IntPtr retLength;

            if ((status = Win32.NtQuerySection(this, SectionInformationClass.SectionBasicInformation,
                out sbi, new IntPtr(Marshal.SizeOf(typeof(SectionBasicInformation))), out retLength)) >= NtStatus.Error)
                Win32.Throw(status);

            return sbi;
        }

        public SectionImageInformation GetImageInformation()
        {
            NtStatus status;
            SectionImageInformation sii;
            IntPtr retLength;

            if ((status = Win32.NtQuerySection(this, SectionInformationClass.SectionImageInformation,
                out sii, new IntPtr(Marshal.SizeOf(typeof(SectionImageInformation))), out retLength)) >= NtStatus.Error)
                Win32.Throw(status);

            return sii;
        }

        public SectionView MapView(int sectionOffset, int size, MemoryProtection protection)
        {
            return this.MapView(IntPtr.Zero, sectionOffset, new IntPtr(size), protection);
        }

        public SectionView MapView(IntPtr baseAddress, long sectionOffset, IntPtr size, MemoryProtection protection)
        {
            return this.MapView(ProcessHandle.Current, baseAddress, sectionOffset, size, protection);
        }

        public SectionView MapView(
            ProcessHandle processHandle,
            IntPtr baseAddress,
            long sectionOffset,
            IntPtr size,
            MemoryProtection protection
            )
        {
            return this.MapView(
                processHandle,
                baseAddress,
                size,
                sectionOffset,
                size,
                SectionInherit.ViewShare,
                0,
                protection
                );
        }

        public SectionView MapView(
            ProcessHandle processHandle,
            IntPtr baseAddress,
            IntPtr commitSize,
            long sectionOffset,
            IntPtr viewSize,
            SectionInherit inheritDisposition,
            MemoryFlags allocationType,
            MemoryProtection protection
            )
        {
            NtStatus status;

            // sectionOffset requires 2 << 15 = 0x10000 = 65536 alignment.
            // viewSize will be rounded up to the page size.
            if ((status = Win32.NtMapViewOfSection(
                this,
                processHandle,
                ref baseAddress,
                IntPtr.Zero,
                commitSize,
                ref sectionOffset,
                ref viewSize,
                inheritDisposition,
                allocationType,
                protection
                )) >= NtStatus.Error)
                Win32.Throw(status);

            return new SectionView(baseAddress, viewSize);
        }
    }
}
