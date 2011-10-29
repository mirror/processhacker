/*
 * Process Hacker - 
 *   SAM alias handle
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
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Security;

namespace ProcessHacker.Native.Objects
{
    /// <summary>
    /// Represents a handle to a SAM alias.
    /// </summary>
    public sealed class SamAliasHandle : SamHandle<SamAliasAccess>
    {
        public static SamAliasHandle Create(SamAliasAccess access, SamDomainHandle domainHandle, string name, out int aliasId)
        {
            IntPtr handle;

            UnicodeString nameStr = new UnicodeString(name);

            try
            {
                Win32.SamCreateAliasInDomain(
                    domainHandle,
                    ref nameStr,
                    access,
                    out handle,
                    out aliasId
                    ).ThrowIf();
            }
            finally
            {
                nameStr.Dispose();
            }

            return new SamAliasHandle(handle, true);
        }

        public static SamAliasHandle Open(string name, SamAliasAccess access)
        {
            return Open(Sid.FromName(name), access);
        }

        public static SamAliasHandle Open(Sid sid, SamAliasAccess access)
        {
            using (SamDomainHandle dhandle = new SamDomainHandle(sid.DomainName, SamDomainAccess.Lookup))
            {
                return new SamAliasHandle(dhandle, dhandle.LookupName(sid.Name), access);
            }
        }

        private SamAliasHandle(IntPtr handle, bool owned)
            : base(handle, owned)
        { }

        /// <summary>
        /// Opens a SAM alias.
        /// </summary>
        /// <param name="domainHandle">A handle to a SAM domain.</param>
        /// <param name="aliasId">The relative ID of the alias to open.</param>
        /// <param name="access">The desired access to the alias.</param>
        public SamAliasHandle(SamDomainHandle domainHandle, int aliasId, SamAliasAccess access)
        {
            IntPtr handle;

            Win32.SamOpenAlias(
                domainHandle,
                access,
                aliasId,
                out handle
                ).ThrowIf();

            this.Handle = handle;
        }

        public void AddMember(Sid sid)
        {
            Win32.SamAddMemberToAlias(this, sid).ThrowIf();
        }

        public void AddMembers(Sid[] sids)
        {
            IntPtr[] sidArray = new IntPtr[sids.Length];

            for (int i = 0; i < sids.Length; i++)
                sidArray[i] = sids[i];

            Win32.SamAddMultipleMembersToAlias(
                this,
                sidArray,
                sids.Length
                ).ThrowIf();
        }

        public void Delete()
        {
            Win32.SamDeleteAlias(this).ThrowIf();
        }

        private SamMemoryAlloc GetInformation(AliasInformationClass infoClass)
        {
            IntPtr buffer;

            Win32.SamQueryInformationAlias(
                this,
                infoClass,
                out buffer
                ).ThrowIf();

            return new SamMemoryAlloc(buffer);
        }

        public string AdminComment
        {
            get
            {
                using (SamMemoryAlloc data = this.GetInformation(AliasInformationClass.AliasAdminCommentInformation))
                {
                    return data.ReadStruct<AliasAdmCommentInformation>().AdminComment.Text;
                }
            }
        }

        public Sid[] Members
        {
            get
            {
                IntPtr members;
                int count;

                Win32.SamGetMembersInAlias(
                    this,
                    out members,
                    out count
                    ).ThrowIf();

                using (SamMemoryAlloc membersAlloc = new SamMemoryAlloc(members))
                {
                    Sid[] sids = new Sid[count];

                    for (int i = 0; i < sids.Length; i++)
                        sids[i] = new Sid(membersAlloc.ReadIntPtr(0, i));

                    return sids;
                }
            }
        }

        public string Name
        {
            get
            {
                using (SamMemoryAlloc data = this.GetInformation(AliasInformationClass.AliasNameInformation))
                {
                    return data.ReadStruct<AliasNameInformation>().Name.Text;
                }
            }
        }

        public void RemoveMember(Sid sid)
        {
            Win32.SamRemoveMemberFromAlias(this, sid).ThrowIf();
        }

        public void RemoveMembers(Sid[] sids)
        {
            IntPtr[] sidArray = new IntPtr[sids.Length];

            for (int i = 0; i < sids.Length; i++)
                sidArray[i] = sids[i];

            Win32.SamRemoveMultipleMembersFromAlias(
                this,
                sidArray,
                sids.Length
                ).ThrowIf();
        }
    }
}
