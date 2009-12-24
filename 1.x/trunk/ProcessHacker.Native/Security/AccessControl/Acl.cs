/*
 * Process Hacker - 
 *   access control list
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
using System.Runtime.InteropServices;
using ProcessHacker.Common.Objects;
using ProcessHacker.Native.Api;

namespace ProcessHacker.Native.Security.AccessControl
{
    public sealed class Acl : BaseObject, IEnumerable<Ace>
    {
        public static Acl FromPointer(IntPtr memory)
        {
            return new Acl(new MemoryRegion(memory));
        }

        public static implicit operator IntPtr(Acl acl)
        {
            return acl.Memory;
        }

        private MemoryRegion _memory;

        public Acl(int size)
        {
            NtStatus status;

            // Reserve 8 bytes for the ACL header.
            if (size < 8)
                throw new ArgumentException("Size must be greater than or equal to 8 bytes.");

            // Allocate some memory.
            _memory = new MemoryAlloc(size);

            // Initialize the ACL.
            if ((status = Win32.RtlCreateAcl(
                _memory,
                size,
                Win32.AclRevision
                )) >= NtStatus.Error)
            {
                // Dispose memory and disable ownership.
                _memory.Dispose();
                _memory = null;
                this.DisableOwnership(false);
            }

            _memory.Reference();
            _memory.Dispose();
        }

        public Acl(Acl existingAcl)
        {
            // Allocate memory for the new ACL.
            _memory = new MemoryAlloc(existingAcl.Size);
            // Copy the ACL.
            _memory.WriteMemory(0, existingAcl, existingAcl.Size);
            _memory.Reference();
            _memory.Dispose();
        }

        public Acl(Acl existingAcl, int newSize)
            : this(newSize)
        {
            this.AddRange(0, existingAcl);
        }

        public Acl(MemoryRegion memory)
        {
            _memory = memory;
            _memory.Reference();
        }

        protected override void DisposeObject(bool disposing)
        {
            if (_memory != null)
                _memory.Dereference(disposing);
        }

        public Ace this[int index]
        {
            get { return this.GetAt(index); }
        }

        public int BytesFree
        {
            get { return this.GetSizeInformation().AclBytesFree; }
        }

        public int BytesUsed
        {
            get { return this.GetSizeInformation().AclBytesInUse; }
        }

        public int Count
        {
            get { return this.GetSizeInformation().AceCount; }
        }

        public IntPtr Memory
        {
            get { return _memory; }
        }

        public int Size
        {
            get
            {
                var sizeInfo = this.GetSizeInformation();

                return sizeInfo.AclBytesInUse + sizeInfo.AclBytesFree;
            }
        }

        public bool IsValid()
        {
            return Win32.RtlValidAcl(this);
        }

        public void AddAccessAllowed(int accessMask, Sid sid)
        {
            NtStatus status;

            if ((status = Win32.RtlAddAccessAllowedAce(
                this,
                Win32.AclRevision,
                accessMask,
                sid
                )) >= NtStatus.Error)
                Win32.Throw(status);
        }

        public void AddAccessAllowed(int accessMask, Sid sid, AceFlags flags)
        {
            NtStatus status;

            if ((status = Win32.RtlAddAccessAllowedAceEx(
                this,
                Win32.AclRevision,
                flags,
                accessMask,
                sid
                )) >= NtStatus.Error)
                Win32.Throw(status);
        }

        public void AddAccessDenied(int accessMask, Sid sid)
        {
            NtStatus status;

            if ((status = Win32.RtlAddAccessDeniedAce(
                this,
                Win32.AclRevision,
                accessMask,
                sid
                )) >= NtStatus.Error)
                Win32.Throw(status);
        }

        public void AddAccessDenied(int accessMask, Sid sid, AceFlags flags)
        {
            NtStatus status;

            if ((status = Win32.RtlAddAccessDeniedAceEx(
                this,
                Win32.AclRevision,
                flags,
                accessMask,
                sid
                )) >= NtStatus.Error)
                Win32.Throw(status);
        }

        public void AddAuditAccess(int accessMask, Sid sid, bool auditSuccess, bool auditFailure)
        {
            NtStatus status;

            if ((status = Win32.RtlAddAuditAccessAce(
                this,
                Win32.AclRevision,
                accessMask,
                sid,
                auditSuccess,
                auditFailure
                )) >= NtStatus.Error)
                Win32.Throw(status);
        }

        public void AddAuditAccess(int accessMask, Sid sid, bool auditSuccess, bool auditFailure, AceFlags flags)
        {
            NtStatus status;

            if ((status = Win32.RtlAddAuditAccessAceEx(
                this,
                Win32.AclRevision,
                flags,
                accessMask,
                sid,
                auditSuccess,
                auditFailure
                )) >= NtStatus.Error)
                Win32.Throw(status);
        }

        public void AddCompound(AceType type, int accessMask, Sid serverSid, Sid clientSid)
        {
            NtStatus status;

            if ((status = Win32.RtlAddCompoundAce(
                this,
                Win32.AclRevision,
                type,
                accessMask,
                serverSid,
                clientSid
                )) >= NtStatus.Error)
                Win32.Throw(status);
        }

        public void AddRange(int index, IEnumerable<Ace> aceList)
        {
            int totalSize = 0;

            // Compute the total size, in bytes.
            foreach (Ace ace in aceList)
            {
                totalSize += ace.Size;
            }

            using (var aceListMemory = new MemoryAlloc(totalSize))
            {
                int i = 0;

                // Copy the ACEs into one contiguous block.
                foreach (Ace ace in aceList)
                {
                    aceListMemory.WriteMemory(i, ace, ace.Size);
                    i += ace.Size;
                }

                NtStatus status;

                // Add the ACEs to the ACL.
                if ((status = Win32.RtlAddAce(
                    this,
                    Win32.AclRevision,
                    index,
                    aceListMemory,
                    totalSize
                    )) >= NtStatus.Error)
                    Win32.Throw(status);
            }
        }

        public Ace GetAt(int index)
        {
            NtStatus status;
            IntPtr ace;

            if ((status = Win32.RtlGetAce(this, index, out ace)) >= NtStatus.Error)
                Win32.Throw(status);

            return Ace.GetAce(ace);
        }

        public IEnumerator<Ace> GetEnumerator()
        {
            for (int i = 0; i < this.Count; i++)
                yield return this[i];
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public AclSizeInformation GetSizeInformation()
        {
            NtStatus status;
            AclSizeInformation sizeInfo;

            if ((status = Win32.RtlQueryInformationAcl(
                this,
                out sizeInfo,
                Marshal.SizeOf(typeof(AclSizeInformation)),
                AclInformationClass.AclSizeInformation
                )) >= NtStatus.Error)
                Win32.Throw(status);

            return sizeInfo;
        }

        public void RemoveAt(int index)
        {
            NtStatus status;

            if ((status = Win32.RtlDeleteAce(this, index)) >= NtStatus.Error)
                Win32.Throw(status);
        }
    }
}
