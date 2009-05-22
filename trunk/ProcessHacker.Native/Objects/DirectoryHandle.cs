/*
 * Process Hacker - 
 *   directory handle
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
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Security;

namespace ProcessHacker.Native.Objects
{
    public class DirectoryHandle : Win32Handle
    {
        public struct ObjectEntry
        {
            private string _name;
            private string _typeName;

            public ObjectEntry(string name, string typeName)
            {
                _name = name;
                _typeName = typeName;
            }

            public string Name { get { return _name; } }
            public string TypeName { get { return _typeName; } }
        }

        public static DirectoryHandle Create(DirectoryAccess access, string name)
        {
            return Create(access, name, 0, null);
        }

        public static DirectoryHandle Create(DirectoryAccess access, string name, ObjectFlags objectFlags, DirectoryHandle rootDirectory)
        {
            NtStatus status;
            ObjectAttributes oa = new ObjectAttributes(name, objectFlags, rootDirectory);
            IntPtr handle;

            try
            {
                if ((status = Win32.NtCreateDirectoryObject(out handle, access, ref oa)) >= NtStatus.Error)
                    Win32.ThrowLastError(status);
            }
            finally
            {
                oa.Dispose();
            }

            return new DirectoryHandle(handle, true);
        }

        private DirectoryHandle(IntPtr handle, bool owned)
            : base(handle, owned)
        { }

        public DirectoryHandle(string name, ObjectFlags objectFlags, DirectoryHandle rootDirectory, DirectoryAccess access)
        {
            NtStatus status;
            ObjectAttributes oa = new ObjectAttributes(name, objectFlags, rootDirectory);
            IntPtr handle;

            try
            {
                if ((status = Win32.NtOpenDirectoryObject(out handle, access, ref oa)) >= NtStatus.Error)
                    Win32.ThrowLastError(status);
            }
            finally
            {
                oa.Dispose();
            }

            this.Handle = handle;
        }

        public DirectoryHandle(string name, DirectoryAccess access)
            : this(name, 0, null, access)
        { }

        /// <summary>
        /// Gets the objects contained in the directory object.
        /// </summary>
        /// <returns>An array of object entries.</returns>
        public ObjectEntry[] Query()
        {
            NtStatus status;
            int context = 0;
            int retLength;
            var objectList = new List<ObjectEntry>();

            using (var data = new MemoryAlloc(0x400))
            {
                // NtQueryDirectoryObject isn't very nice.
                while ((status = Win32.NtQueryDirectoryObject(
                    this,
                    data,
                    data.Size,
                    false,
                    false,
                    ref context,
                    out retLength
                    )) == NtStatus.InfoLengthMismatch)
                {
                    if (data.Size > 16 * 1024 * 1024)
                        Win32.ThrowLastError(status);

                    data.Resize(data.Size * 2);
                }

                if (status < 0)
                    Win32.ThrowLastError(status);

                int i = 0;

                while (true)
                {
                    ObjectDirectoryInformation info = data.ReadStruct<ObjectDirectoryInformation>(i);

                    if (info.Name.Buffer == IntPtr.Zero)
                        break;

                    objectList.Add(new ObjectEntry(info.Name.Read(), info.TypeName.Read()));
                    i++;
                }
            }

            return objectList.ToArray();
        }
    }
}
