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

        public DirectoryHandle Create(DirectoryAccess access, string name)
        {
            return this.Create(access, name, null);
        }

        public DirectoryHandle Create(DirectoryAccess access, string name, DirectoryHandle rootDirectory)
        {
            int status;
            ObjectAttributes oa = ObjectAttributes.Create(name, 0, rootDirectory);
            IntPtr handle;

            try
            {
                if ((status = Win32.NtCreateDirectoryObject(out handle, access, ref oa)) < 0)
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

        public DirectoryHandle(string name, DirectoryHandle rootDirectory, DirectoryAccess access)
        {
            int status;
            ObjectAttributes oa = ObjectAttributes.Create(name, 0, rootDirectory);
            IntPtr handle;

            try
            {
                if ((status = Win32.NtOpenDirectoryObject(out handle, access, ref oa)) < 0)
                    Win32.ThrowLastError(status);
            }
            finally
            {
                oa.Dispose();
            }

            this.Handle = handle;
        }

        public DirectoryHandle(string name, DirectoryAccess access)
            : this(name, null, access)
        { }

        /// <summary>
        /// Gets the objects contained in the directory object.
        /// </summary>
        /// <returns>An array of object entries.</returns>
        public ObjectEntry[] Query()
        {
            int status;
            int context = 0;
            int retLength;
            var objectList = new List<ObjectEntry>();

            using (var data = new MemoryAlloc(0x400))
            {
                // NtQueryDirectoryObject isn't very nice.
                while ((uint)(status = Win32.NtQueryDirectoryObject(
                    this,
                    data,
                    data.Size,
                    false,
                    false,
                    ref context,
                    out retLength
                    )) == Win32.STATUS_INFO_LENGTH_MISMATCH)
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
