/*
 * Process Hacker - 
 *   SAM group handle
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
    /// Represents a handle to a SAM group.
    /// </summary>
    public sealed class SamGroupHandle : SamHandle<SamGroupAccess>
    {
        public static SamGroupHandle Create(SamGroupAccess access, SamDomainHandle domainHandle, string name, out int groupId)
        {
            NtStatus status;
            UnicodeString nameStr;
            IntPtr handle;

            nameStr = new UnicodeString(name);

            try
            {
                if ((status = Win32.SamCreateGroupInDomain(
                    domainHandle,
                    ref nameStr,
                    access,
                    out handle,
                    out groupId
                    )) >= NtStatus.Error)
                    Win32.Throw(status);
            }
            finally
            {
                nameStr.Dispose();
            }

            return new SamGroupHandle(handle, true);
        }

        public static SamGroupHandle Open(string name, SamGroupAccess access)
        {
            return Open(Sid.FromName(name), access);
        }

        public static SamGroupHandle Open(Sid sid, SamGroupAccess access)
        {
            using (var dhandle = new SamDomainHandle(sid.DomainName, SamDomainAccess.Lookup))
            {
                return new SamGroupHandle(dhandle, dhandle.LookupName(sid.Name), access);
            }
        }

        private SamGroupHandle(IntPtr handle, bool owned)
            : base(handle, owned)
        { }

        /// <summary>
        /// Opens a SAM group.
        /// </summary>
        /// <param name="domainHandle">A handle to a SAM domain.</param>
        /// <param name="groupId">The relative ID of the group to open.</param>
        /// <param name="access">The desired access to the group.</param>
        public SamGroupHandle(SamDomainHandle domainHandle, int groupId, SamGroupAccess access)
        {
            NtStatus status;
            IntPtr handle;

            if ((status = Win32.SamOpenGroup(
                domainHandle,
                access,
                groupId,
                out handle
                )) >= NtStatus.Error)
                Win32.Throw(status);

            this.Handle = handle;
        }

        public void AddMember(int memberId)
        {
            NtStatus status;

            if ((status = Win32.SamAddMemberToGroup(this, memberId, 0)) >= NtStatus.Error)
                Win32.Throw(status);
        }

        public void Delete()
        {
            NtStatus status;

            if ((status = Win32.SamDeleteGroup(this)) >= NtStatus.Error)
                Win32.Throw(status);
        } 

        private SamMemoryAlloc GetInformation(GroupInformationClass infoClass)
        {
            NtStatus status;
            IntPtr buffer;

            if ((status = Win32.SamQueryInformationGroup(
                this,
                infoClass,
                out buffer
                )) >= NtStatus.Error)
                Win32.Throw(status);

            return new SamMemoryAlloc(buffer);
        }

        public string GetAdminComment()
        {
            using (var data = this.GetInformation(GroupInformationClass.GroupAdminCommentInformation))
            {
                return data.ReadStruct<GroupAdmInformation>().AdminComment.Read();
            }
        }

        public int[] GetMembers()
        {
            NtStatus status;
            IntPtr memberIds;
            IntPtr attributes;
            int count;

            if ((status = Win32.SamGetMembersInGroup(
                this,
                out memberIds,
                out attributes,
                out count
                )) >= NtStatus.Error)
                Win32.Throw(status);

            using (var memberIdsAlloc = new SamMemoryAlloc(memberIds))
            using (var attributesAlloc = new SamMemoryAlloc(attributes))
            {
                return memberIdsAlloc.ReadInt32Array(0, count);
            }
        }

        public string GetName()
        {
            using (var data = this.GetInformation(GroupInformationClass.GroupNameInformation))
            {
                return data.ReadStruct<GroupNameInformation>().Name.Read();
            }
        }

        public void RemoveMember(int memberId)
        {
            NtStatus status;

            if ((status = Win32.SamRemoveMemberFromGroup(this, memberId)) >= NtStatus.Error)
                Win32.Throw(status);
        }
    }
}
