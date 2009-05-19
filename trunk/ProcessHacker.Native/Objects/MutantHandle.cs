using System;
using System.Collections.Generic;
using System.Text;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Security;
using System.Runtime.InteropServices;

namespace ProcessHacker.Native.Objects
{
    public class MutantHandle : Win32Handle<MutantAccess>
    {
        public static MutantHandle Create(MutantAccess access, bool initialOwner)
        {
            return Create(access, null, initialOwner);
        }

        public static MutantHandle Create(MutantAccess access, string name, bool initialOwner)
        {
            return Create(access, name, null, initialOwner);
        }

        public static MutantHandle Create(MutantAccess access, string name, DirectoryHandle rootDirectory, bool initialOwner)
        {
            int status;
            ObjectAttributes oa = ObjectAttributes.Create(name, 0, rootDirectory);
            IntPtr handle;

            try
            {
                if ((status = Win32.NtCreateMutant(out handle, access, ref oa, initialOwner)) < 0)
                    Win32.ThrowLastError(status);
            }
            finally
            {
                oa.Dispose();
            }

            return new MutantHandle(handle, true);
        }

        private MutantHandle(IntPtr handle, bool owned)
            : base(handle, owned)
        { }

        public MutantHandle(string name, DirectoryHandle rootDirectory, MutantAccess access)
        {
            int status;
            ObjectAttributes oa = ObjectAttributes.Create(name, 0, rootDirectory);
            IntPtr handle;

            try
            {
                if ((status = Win32.NtOpenMutant(out handle, access, ref oa)) < 0)
                    Win32.ThrowLastError(status);
            }
            finally
            {
                oa.Dispose();
            }

            this.Handle = handle;
        }

        public MutantHandle(string name, MutantAccess access)
            : this(name, null, access)
        { }

        public MutantBasicInformation Query()
        {
            int status;
            MutantBasicInformation mbi;
            int retLength;

            if ((status = Win32.NtQueryMutant(this, MutantInformationClass.MutantBasicInformation,
                out mbi, Marshal.SizeOf(typeof(MutantBasicInformation)), out retLength)) < 0)
                Win32.ThrowLastError(status);

            return mbi;
        }

        public int Release()
        {
            int status;
            int previousCount;

            if ((status = Win32.NtReleaseMutant(this, out previousCount)) < 0)
                Win32.ThrowLastError(status);

            return previousCount;
        }
    }
}
