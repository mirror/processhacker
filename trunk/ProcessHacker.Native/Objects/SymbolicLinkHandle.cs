using System;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Security;

namespace ProcessHacker.Native.Objects
{
    public class SymbolicLinkHandle : Win32Handle<SymbolicLinkAccess>
    {
        public static SymbolicLinkHandle Create(SymbolicLinkAccess access, string name, string linkTarget)
        {
            return Create(access, name, null, linkTarget);
        }

        public static SymbolicLinkHandle Create(SymbolicLinkAccess access, string name, DirectoryHandle rootDirectory, string linkTarget)
        {
            int status;
            ObjectAttributes oa = ObjectAttributes.Create(name, 0, rootDirectory);
            UnicodeString linkTargetString = UnicodeString.Create(linkTarget);
            IntPtr handle;

            try
            {
                if ((status = Win32.NtCreateSymbolicLinkObject(out handle, access,
                    ref oa, ref linkTargetString)) < 0)
                    Win32.ThrowLastError(status);
            }
            finally
            {
                oa.Dispose();
                linkTargetString.Dispose();
            }

            return new SymbolicLinkHandle(handle, true);
        }

        private SymbolicLinkHandle(IntPtr handle, bool owned)
            : base(handle, owned)
        { }

        public SymbolicLinkHandle(string name, DirectoryHandle rootDirectory, SymbolicLinkAccess access)
        {
            int status;
            ObjectAttributes oa = ObjectAttributes.Create(name, 0, rootDirectory);
            IntPtr handle;

            try
            {
                if ((status = Win32.NtOpenSymbolicLinkObject(out handle, access, ref oa)) < 0)
                    Win32.ThrowLastError(status);
            }
            finally
            {
                oa.Dispose();
            }

            this.Handle = handle;
        }

        public SymbolicLinkHandle(string name, SymbolicLinkAccess access)
            : this(name, null, access)
        { }

        public string GetTarget()
        {
            int status;
            int retLength;
            UnicodeString str = new UnicodeString();

            using (var buffer = new MemoryAlloc(0x200))
            {
                str.Length = 0;
                str.MaximumLength = (ushort)buffer.Size;
                str.Buffer = buffer;

                if ((status = Win32.NtQuerySymbolicLinkObject(this, ref str, out retLength)) < 0)
                {
                    buffer.Resize(retLength);
                    str.MaximumLength = (ushort)retLength;
                    str.Buffer = buffer;
                }

                if ((status = Win32.NtQuerySymbolicLinkObject(this, ref str, out retLength)) < 0)
                    Win32.ThrowLastError(status);

                return str.Read();
            }
        }
    }
}
