using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ProcessHacker.Native.Api;

namespace ProcessHacker.Native.Security
{
    public class SecurityDescriptor : IDisposable
    {
        private object _disposeLock = new object();
        private bool _disposed = false;
        private MemoryAlloc _sd;

        public static implicit operator IntPtr(SecurityDescriptor securityDescriptor)
        {
            return securityDescriptor.Memory;
        }

        public SecurityDescriptor(MemoryAlloc sd)
        {
            _sd = sd;
        }

        public SecurityDescriptor(IntPtr sd)
        {
            _sd = MemoryAlloc.FromPointer(sd);
            _sd.Owned = false;
        }

        ~SecurityDescriptor()
        {
            this.Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
                Monitor.Enter(_disposeLock);

            try
            {
                if (!_disposed)
                {
                    _sd.Dispose();
                    _disposed = true;
                }
            }
            finally
            {
                if (disposing)
                    Monitor.Exit(_disposeLock);
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public MemoryAlloc Memory
        {
            get { return _sd; }
        }

        public IntPtr GetDacl(out bool present, out bool defaulted)
        {
            IntPtr dacl;

            Win32.GetSecurityDescriptorDacl(this, out present, out dacl, out defaulted);

            return dacl;
        }

        public IntPtr GetGroup(out bool defaulted)
        {
            IntPtr group;

            Win32.GetSecurityDescriptorGroup(this, out group, out defaulted);

            return group;
        }

        public IntPtr GetOwner(out bool defaulted)
        {
            IntPtr owner;

            Win32.GetSecurityDescriptorOwner(this, out owner, out defaulted);

            return owner;
        }

        public IntPtr GetSacl(out bool present, out bool defaulted)
        {
            IntPtr sacl;

            Win32.GetSecurityDescriptorSacl(this, out present, out sacl, out defaulted);

            return sacl;
        }
    }
}
