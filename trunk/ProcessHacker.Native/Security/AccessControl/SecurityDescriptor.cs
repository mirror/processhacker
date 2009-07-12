using System;
using ProcessHacker.Common.Objects;
using ProcessHacker.Native.Api;

namespace ProcessHacker.Native.Security.AccessControl
{
    public abstract class SecurityDescriptor : BaseObject
    {
        public static implicit operator IntPtr(SecurityDescriptor securityDescriptor)
        {
            return securityDescriptor.Memory;
        }

        private IntPtr _memory;
        private Acl _dacl;
        private Acl _sacl;
        private Sid _owner;
        private Sid _group;

        protected override void DisposeObject(bool disposing)
        {
            if (_dacl != null)
                _dacl.Dereference();
            if (_sacl != null)
                _sacl.Dereference();
            if (_owner != null)
                _owner.Dereference();
            if (_group != null)
                _group.Dereference();
        }

        public SecurityDescriptorControlFlags ControlFlags
        {
            get
            {
                NtStatus status;
                SecurityDescriptorControlFlags control;
                int revision;

                if ((status = Win32.RtlGetControlSecurityDescriptor(
                    this,
                    out control,
                    out revision
                    )) >= NtStatus.Error)
                    Win32.ThrowLastError(status);

                return control;
            }
            set
            {
                NtStatus status;

                if ((status = Win32.RtlSetControlSecurityDescriptor(
                    this,
                    value,
                    value
                    )) >= NtStatus.Error)
                    Win32.ThrowLastError(status);
            }
        }

        public virtual Acl Dacl
        {
            get { return _dacl; }
            set
            {
                NtStatus status;

                if ((status = Win32.RtlSetDaclSecurityDescriptor(
                    this,
                    value != null,
                    value ?? IntPtr.Zero,
                    false
                    )) >= NtStatus.Error)
                    Win32.ThrowLastError(status);

                this.SwapDacl(value);
            }
        }

        public bool DaclDefaulted
        {
            get
            {
                return (this.ControlFlags & SecurityDescriptorControlFlags.DaclDefaulted) == 
                    SecurityDescriptorControlFlags.DaclDefaulted;
            }
        }

        public virtual Sid Group
        {
            get { return _group; }
            set
            {
                NtStatus status;

                if ((status = Win32.RtlSetGroupSecurityDescriptor(
                    this,
                    value,
                    false
                    )) >= NtStatus.Error)
                    Win32.ThrowLastError(status);

                this.SwapGroup(value);
            }
        }

        public bool GroupDefaulted
        {
            get
            {
                return (this.ControlFlags & SecurityDescriptorControlFlags.GroupDefaulted) ==
                    SecurityDescriptorControlFlags.GroupDefaulted;
            }
        }

        public IntPtr Memory
        {
            get { return _memory; }
            protected set { _memory = value; }
        }

        public virtual Sid Owner
        {
            get { return _owner; }
            set
            {
                NtStatus status;

                if ((status = Win32.RtlSetOwnerSecurityDescriptor(
                    this,
                    value,
                    false
                    )) >= NtStatus.Error)
                    Win32.ThrowLastError(status);

                this.SwapOwner(value);
            }
        }

        public bool OwnerDefaulted
        {
            get
            {
                return (this.ControlFlags & SecurityDescriptorControlFlags.OwnerDefaulted) ==
                    SecurityDescriptorControlFlags.OwnerDefaulted;
            }
        }

        public virtual Acl Sacl
        {
            get { return _sacl; }
            set
            {
                NtStatus status;

                if ((status = Win32.RtlSetSaclSecurityDescriptor(
                    this,
                    value != null,
                    value ?? IntPtr.Zero,
                    false
                    )) >= NtStatus.Error)
                    Win32.ThrowLastError(status);

                this.SwapSacl(value);
            }
        }

        public bool SaclDefaulted
        {
            get
            {
                return (this.ControlFlags & SecurityDescriptorControlFlags.SaclDefaulted) ==
                    SecurityDescriptorControlFlags.SaclDefaulted;
            }
        }

        protected void SwapDacl(Acl dacl)
        {
            BaseObject.SwapRef<Acl>(ref _dacl, dacl);
        }

        protected void SwapGroup(Sid group)
        {
            BaseObject.SwapRef<Sid>(ref _group, group);
        }

        protected void SwapOwner(Sid owner)
        {
            BaseObject.SwapRef<Sid>(ref _owner, owner);
        }

        protected void SwapSacl(Acl sacl)
        {
            BaseObject.SwapRef<Acl>(ref _sacl, sacl);
        }

        protected virtual void Read()
        {
            NtStatus status;
            bool present, defaulted;
            IntPtr dacl, group, owner, sacl;

            // Read the DACL.
            if ((status = Win32.RtlGetDaclSecurityDescriptor(
                this,
                out present,
                out dacl,
                out defaulted
                )) >= NtStatus.Error)
                Win32.ThrowLastError(status);

            if (present)
                this.SwapDacl(new Acl(dacl));
            else
                this.SwapDacl(null);

            // Read the SACL.
            if ((status = Win32.RtlGetSaclSecurityDescriptor(
                this,
                out present,
                out sacl,
                out defaulted
                )) >= NtStatus.Error)
                Win32.ThrowLastError(status);

            if (present)
                this.SwapSacl(new Acl(sacl));
            else
                this.SwapSacl(null);

            // Read the group.
            if ((status = Win32.RtlGetGroupSecurityDescriptor(
                this,
                out group,
                out defaulted
                )) >= NtStatus.Error)
                Win32.ThrowLastError(status);

            if (group != IntPtr.Zero)
                this.SwapGroup(Sid.FromPointer(group));
            else
                this.SwapGroup(null);

            // Read the owner.
            if ((status = Win32.RtlGetOwnerSecurityDescriptor(
                this,
                out owner,
                out defaulted
                )) >= NtStatus.Error)
                Win32.ThrowLastError(status);

            if (owner != IntPtr.Zero)
                this.SwapOwner(Sid.FromPointer(owner));
            else
                this.SwapOwner(null);
        }
    }
}
