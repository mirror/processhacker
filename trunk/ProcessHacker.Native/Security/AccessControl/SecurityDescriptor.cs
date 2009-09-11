/*
 * Process Hacker - 
 *   security descriptor
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
using ProcessHacker.Common.Objects;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;

namespace ProcessHacker.Native.Security.AccessControl
{
    public sealed class SecurityDescriptor : BaseObject
    {
        public static implicit operator IntPtr(SecurityDescriptor securityDescriptor)
        {
            return securityDescriptor.Memory;
        }

        private MemoryRegion _memory;
        private bool _memoryOwned = true;
        private Acl _dacl;
        private Acl _sacl;
        private Sid _owner;
        private Sid _group;

        public SecurityDescriptor()
        {
            NtStatus status;

            _memory = new MemoryAlloc(Win32.SecurityDescriptorMinLength);

            if ((status = Win32.RtlCreateSecurityDescriptor(
                _memory,
                Win32.SecurityDescriptorRevision
                )) >= NtStatus.Error)
            {
                _memory.Dispose();
                this.DisableOwnership(false);
                Win32.ThrowLastError(status);
            }
        }

        public SecurityDescriptor(Sid owner, Sid group, Acl dacl, Acl sacl)
            : this()
        {
            this.Owner = owner;
            this.Group = group;
            this.Dacl = dacl;
            this.Sacl = sacl;
        }

        public SecurityDescriptor(IntPtr memory)
            : this(new MemoryRegion(memory), false)
        { }

        public SecurityDescriptor(MemoryRegion memory, bool owned)
        {
            _memory = memory;
            _memoryOwned = owned;
            this.Read();
        }

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
            if (_memory != null && _memoryOwned)
                _memory.Dispose();
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

        public Acl Dacl
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
            set
            {
                if (value)
                    this.ControlFlags |= SecurityDescriptorControlFlags.DaclDefaulted;
                else
                    this.ControlFlags &= ~SecurityDescriptorControlFlags.DaclDefaulted;
            }
        }

        public Sid Group
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
            set
            {
                if (value)
                    this.ControlFlags |= SecurityDescriptorControlFlags.GroupDefaulted;
                else
                    this.ControlFlags &= ~SecurityDescriptorControlFlags.GroupDefaulted;
            }
        }

        public int Length
        {
            get { return Win32.RtlLengthSecurityDescriptor(this); }
        }

        public IntPtr Memory
        {
            get { return _memory; }
        }

        public Sid Owner
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
            set
            {
                if (value)
                    this.ControlFlags |= SecurityDescriptorControlFlags.OwnerDefaulted;
                else
                    this.ControlFlags &= ~SecurityDescriptorControlFlags.OwnerDefaulted;
            }
        }

        public Acl Sacl
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
            set
            {
                if (value)
                    this.ControlFlags |= SecurityDescriptorControlFlags.SaclDefaulted;
                else
                    this.ControlFlags &= ~SecurityDescriptorControlFlags.SaclDefaulted;
            }
        }

        public bool SelfRelative
        {
            get
            {
                return (this.ControlFlags & SecurityDescriptorControlFlags.SelfRelative) ==
                    SecurityDescriptorControlFlags.SelfRelative;
            }
        }

        public NtStatus CheckAccess(TokenHandle tokenHandle, int desiredAccess, GenericMapping genericMapping, out int grantedAccess)
        {
            NtStatus status;
            NtStatus accessStatus;
            int privilegeSetLength = 0;

            if ((status = Win32.NtAccessCheck(
                this,
                tokenHandle,
                desiredAccess,
                ref genericMapping,
                IntPtr.Zero,
                ref privilegeSetLength,
                out grantedAccess,
                out accessStatus
                )) >= NtStatus.Error)
                Win32.ThrowLastError(status);

            return accessStatus;
        }

        public bool IsValid()
        {
            return Win32.RtlValidSecurityDescriptor(this);
        }

        private void Read()
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
                this.SwapDacl(new Acl(dacl, true));
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
                this.SwapSacl(new Acl(sacl, true));
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
                this.SwapGroup(new Sid(group));
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
                this.SwapOwner(new Sid(owner));
            else
                this.SwapOwner(null);
        }

        private void SwapDacl(Acl dacl)
        {
            BaseObject.SwapRef<Acl>(ref _dacl, dacl);
        }

        private void SwapGroup(Sid group)
        {
            BaseObject.SwapRef<Sid>(ref _group, group);
        }

        private void SwapOwner(Sid owner)
        {
            BaseObject.SwapRef<Sid>(ref _owner, owner);
        }

        private void SwapSacl(Acl sacl)
        {
            BaseObject.SwapRef<Acl>(ref _sacl, sacl);
        }

        public SecurityDescriptor ToSelfRelative()
        {
            NtStatus status;
            int retLength;
            var data = new MemoryAlloc(Win32.SecurityDescriptorMinLength);

            retLength = data.Size;
            status = Win32.RtlMakeSelfRelativeSD(this, data, ref retLength);

            if (status == NtStatus.BufferTooSmall)
            {
                data.Resize(retLength);
                status = Win32.RtlMakeSelfRelativeSD(this, data, ref retLength);
            }

            if (status >= NtStatus.Error)
            {
                data.Dispose();
                Win32.ThrowLastError(status);
            }

            return new SecurityDescriptor(data, true);
        }
    }
}
