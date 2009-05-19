using System;
using System.Runtime.InteropServices;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Security;

namespace ProcessHacker.Native.Objects
{
    public class SemaphoreHandle : Win32Handle<SemaphoreAccess>
    {
        public static SemaphoreHandle Create(SemaphoreAccess access, int initialCount, int maximumCount)
        {
            return Create(access, null, initialCount, maximumCount);
        }

        public static SemaphoreHandle Create(SemaphoreAccess access, string name, int initialCount, int maximumCount)
        {
            return Create(access, name, null, initialCount, maximumCount);
        }

        public static SemaphoreHandle Create(SemaphoreAccess access, string name, DirectoryHandle rootDirectory, int initialCount, int maximumCount)
        {
            int status;
            ObjectAttributes oa = ObjectAttributes.Create(name, 0, rootDirectory);
            IntPtr handle;

            try
            {
                if ((status = Win32.NtCreateSemaphore(out handle, access, ref oa, 
                    initialCount, maximumCount)) < 0)
                    Win32.ThrowLastError(status);
            }
            finally
            {
                oa.Dispose();
            }

            return new SemaphoreHandle(handle, true);
        }

        private SemaphoreHandle(IntPtr handle, bool owned)
            : base(handle, owned)
        { }

        public SemaphoreHandle(string name, DirectoryHandle rootDirectory, SemaphoreAccess access)
        {
            int status;
            ObjectAttributes oa = ObjectAttributes.Create(name, 0, rootDirectory);
            IntPtr handle;

            try
            {
                if ((status = Win32.NtOpenSemaphore(out handle, access, ref oa)) < 0)
                    Win32.ThrowLastError(status);
            }
            finally
            {
                oa.Dispose();
            }

            this.Handle = handle;
        }

        public SemaphoreHandle(string name, SemaphoreAccess access)
            : this(name, null, access)
        { }

        public SemaphoreBasicInformation Query()
        {
            int status;
            SemaphoreBasicInformation sbi;
            int retLength;

            if ((status = Win32.NtQuerySemaphore(this, SemaphoreInformationClass.SemaphoreBasicInformation,
                out sbi, Marshal.SizeOf(typeof(SemaphoreBasicInformation)), out retLength)) < 0)
                Win32.ThrowLastError(status);

            return sbi;
        }

        public int Release(int count)
        {
            int status;
            int previousCount;

            if ((status = Win32.NtReleaseSemaphore(this, count, out previousCount)) < 0)
                Win32.ThrowLastError(status);

            return previousCount;
        }

        public int Release()
        {
            return this.Release(1);
        }
    }
}
