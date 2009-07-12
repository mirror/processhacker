using System;
using ProcessHacker.Common.Objects;
using ProcessHacker.Native.Api;

namespace ProcessHacker.Native.Security.AccessControl
{
    public class SecurityDescriptor : BaseObject
    {
        private object _disposeLock = new object();
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
            _sd = new MemoryAlloc(sd, true);
        }

        protected override void DisposeObject(bool disposing)
        {
            _sd.Dispose(disposing);
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

        public Sid GetGroup()
        {
            bool defaulted;

            return this.GetGroup(out defaulted);
        }

        public Sid GetGroup(out bool defaulted)
        {
            IntPtr group;

            Win32.GetSecurityDescriptorGroup(this, out group, out defaulted);

            return Sid.FromPointer(group);
        }

        public Sid GetOwner()
        {
            bool defaulted;

            return this.GetOwner(out defaulted);
        }

        public Sid GetOwner(out bool defaulted)
        {
            IntPtr owner;

            Win32.GetSecurityDescriptorOwner(this, out owner, out defaulted);

            return Sid.FromPointer(owner);
        }

        public IntPtr GetSacl(out bool present, out bool defaulted)
        {
            IntPtr sacl;

            Win32.GetSecurityDescriptorSacl(this, out present, out sacl, out defaulted);

            return sacl;
        }
    }
}
