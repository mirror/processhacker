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
using System.Collections.Generic;
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
            NtStatus status;
            UnicodeString nameStr;
            IntPtr handle;

            nameStr = new UnicodeString(name);

            try
            {
                if ((status = Win32.SamCreateAliasInDomain(
                    domainHandle,
                    ref nameStr,
                    access,
                    out handle,
                    out aliasId
                    )) >= NtStatus.Error)
                    Win32.Throw(status);
            }
            finally
            {
                nameStr.Dispose();
            }

            return new SamAliasHandle(handle, true);
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
            NtStatus status;
            IntPtr handle;

            if ((status = Win32.SamOpenAlias(
                domainHandle,
                access,
                aliasId,
                out handle
                )) >= NtStatus.Error)
                Win32.Throw(status);

            this.Handle = handle;
        }

        public void Delete()
        {
            NtStatus status;

            if ((status = Win32.SamDeleteAlias(this)) >= NtStatus.Error)
                Win32.Throw(status);
        }

        private SamMemoryAlloc GetInformation(AliasInformationClass infoClass)
        {
            NtStatus status;
            IntPtr buffer;

            if ((status = Win32.SamQueryInformationAlias(
                this,
                infoClass,
                out buffer
                )) >= NtStatus.Error)
                Win32.Throw(status);

            return new SamMemoryAlloc(buffer);
        }

        public string GetAdminComment()
        {
            using (var data = this.GetInformation(AliasInformationClass.AliasAdminCommentInformation))
            {
                return data.ReadStruct<AliasAdmCommentInformation>().AdminComment.Read();
            }
        }

        public string GetName()
        {
            using (var data = this.GetInformation(AliasInformationClass.AliasNameInformation))
            {
                return data.ReadStruct<AliasNameInformation>().Name.Read();
            }
        }
    }
}
