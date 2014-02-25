/*
 * Process Hacker - 
 *   local security authority handle
 * 
 * Copyright (C) 2008-2009 wj32
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
using ProcessHacker.Native.Security.AccessControl;

namespace ProcessHacker.Native.Objects
{
    /// <summary>
    /// Represents a handle managed by the Local Security Authority.
    /// </summary>
    public class LsaHandle<TAccess> : NativeHandle<TAccess>
        where TAccess : struct
    {
        protected LsaHandle()
        { }

        protected LsaHandle(IntPtr handle, bool owned)
            : base(handle, owned)
        { }

        protected override void Close()
        {
            Win32.LsaClose(this);
        }

        public void Delete()
        {
            NtStatus status;

            if ((status = Win32.LsaDelete(this)) >= NtStatus.Error)
                Win32.Throw(status);
        }

        public override SecurityDescriptor GetSecurity(SecurityInformation securityInformation)
        {
            NtStatus status;
            IntPtr securityDescriptor;

            if ((status = Win32.LsaQuerySecurityObject(
                this,
                securityInformation,
                out securityDescriptor
                )) >= NtStatus.Error)
                Win32.Throw(status);

            return new SecurityDescriptor(new LsaMemoryAlloc(securityDescriptor));
        }

        public override void SetSecurity(SecurityInformation securityInformation, SecurityDescriptor securityDescriptor)
        {
            NtStatus status;

            if ((status = Win32.LsaSetSecurityObject(
                this,
                securityInformation,
                securityDescriptor
                )) >= NtStatus.Error)
                Win32.Throw(status);
        }
    }
}
