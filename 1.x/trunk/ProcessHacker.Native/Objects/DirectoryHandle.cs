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
    /// <summary>
    /// Represents a directory object, which contains a collection of objects.
    /// </summary>
    public class DirectoryHandle : NativeHandle<DirectoryAccess>
    {
        public delegate bool EnumObjectsDelegate(ObjectEntry obj);

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
                    Win32.Throw(status);
            }
            finally
            {
                oa.Dispose();
            }

            return new DirectoryHandle(handle, true);
        }

        protected DirectoryHandle()
        { }

        protected DirectoryHandle(IntPtr handle, bool owned)
            : base(handle, owned)
        { }

        public DirectoryHandle(string name, DirectoryAccess access)
            : this(name, 0, null, access)
        { }

        public DirectoryHandle(string name, ObjectFlags objectFlags, DirectoryHandle rootDirectory, DirectoryAccess access)
        {
            NtStatus status;
            ObjectAttributes oa = new ObjectAttributes(name, objectFlags, rootDirectory);
            IntPtr handle;

            try
            {
                if (KProcessHacker.Instance != null)
                {
                    handle = KProcessHacker.Instance.KphOpenDirectoryObject(access, oa).ToIntPtr();
                }
                else
                {
                    if ((status = Win32.NtOpenDirectoryObject(out handle, access, ref oa)) >= NtStatus.Error)
                        Win32.Throw(status);
                }
            }
            finally
            {
                oa.Dispose();
            }

            this.Handle = handle;
        }

        public void EnumObjects(EnumObjectsDelegate callback)
        {
            NtStatus status;
            int context = 0;
            bool firstTime = true;
            int retLength;

            using (var data = new MemoryAlloc(0x200))
            {
                while (true)
                {
                    while ((status = Win32.NtQueryDirectoryObject(
                        this,
                        data,
                        data.Size,
                        false,
                        firstTime,
                        ref context,
                        out retLength
                        )) == NtStatus.MoreEntries)
                    {
                        // Check if we have at least one entry. If not, 
                        // we need to double the buffer size and try again.
                        if (data.ReadStruct<ObjectDirectoryInformation>(0).Name.Buffer != IntPtr.Zero)
                            break;

                        if (data.Size > 16 * 1024 * 1024)
                            Win32.Throw(status);

                        data.ResizeNew(data.Size * 2);
                    }

                    if (status >= NtStatus.Error)
                        Win32.Throw(status);

                    int i = 0;

                    while (true)
                    {
                        ObjectDirectoryInformation info = data.ReadStruct<ObjectDirectoryInformation>(i);

                        if (info.Name.Buffer == IntPtr.Zero)
                            break;

                        if (!callback(new ObjectEntry(info.Name.Read(), info.TypeName.Read())))
                            return;

                        i++;
                    }

                    if (status != NtStatus.MoreEntries)
                        break;

                    firstTime = false;
                }
            }
        }

        /// <summary>
        /// Gets the objects contained in the directory object.
        /// </summary>
        /// <returns>An array of object entries.</returns>
        public ObjectEntry[] GetObjects()
        {
            var objects = new List<ObjectEntry>();

            this.EnumObjects((obj) =>
                {
                    objects.Add(obj);
                    return true;
                });

            return objects.ToArray();
        }
    }
}
