/*
 * Process Hacker - 
 *   LSA account handle
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
    /// Represents a handle to a LSA account.
    /// </summary>
    public sealed class LsaAccountHandle : LsaHandle<LsaAccountAccess>
    {
        public static LsaAccountHandle Create(LsaAccountAccess access, LsaPolicyHandle policyHandle, Sid sid)
        {
            NtStatus status;
            IntPtr handle;

            if ((status = Win32.LsaCreateAccount(
                policyHandle,
                sid,
                access,
                out handle
                )) >= NtStatus.Error)
                Win32.Throw(status);

            return new LsaAccountHandle(handle, true);
        }

        private LsaAccountHandle(IntPtr handle, bool owned)
            : base(handle, owned)
        { }

        /// <summary>
        /// Opens a LSA account.
        /// </summary>
        /// <param name="policyHandle">A handle to a LSA policy.</param>
        /// <param name="sid">The SID of the account to open.</param>
        /// <param name="access">The desired access to the account.</param>
        public LsaAccountHandle(LsaPolicyHandle policyHandle, Sid sid, LsaAccountAccess access)
        {
            NtStatus status;
            IntPtr handle;

            if ((status = Win32.LsaOpenAccount(
                policyHandle,
                sid,
                access,
                out handle
                )) >= NtStatus.Error)
                Win32.Throw(status);

            this.Handle = handle;
        }

        public void AddPrivilege(Privilege privilege)
        {
            this.AddPrivileges(new PrivilegeSet { privilege });
        }

        public void AddPrivileges(PrivilegeSet privileges)
        {
            NtStatus status;

            using (var privilegeSetMemory = privileges.ToMemory())
            {
                if ((status = Win32.LsaAddPrivilegesToAccount(
                    this,
                    privilegeSetMemory
                    )) >= NtStatus.Error)
                    Win32.Throw(status);
            }
        }

        public PrivilegeSet GetPrivileges()
        {
            NtStatus status;
            IntPtr privileges;

            if ((status = Win32.LsaEnumeratePrivilegesOfAccount(
                this,
                out privileges
                )) >= NtStatus.Error)
                Win32.Throw(status);

            using (var privilegesAlloc = new LsaMemoryAlloc(privileges))
            {
                return new PrivilegeSet(privilegesAlloc);
            }
        }

        public QuotaLimits GetQuotas()
        {
            NtStatus status;
            QuotaLimits quotas;

            if ((status = Win32.LsaGetQuotasForAccount(
                this,
                out quotas
                )) >= NtStatus.Error)
                Win32.Throw(status);

            return quotas;
        }

        public SecuritySystemAccess GetSystemAccess()
        {
            NtStatus status;
            SecuritySystemAccess access;

            if ((status = Win32.LsaGetSystemAccessAccount(
                this,
                out access
                )) >= NtStatus.Error)
                Win32.Throw(status);

            return access;
        }

        public void RemovePrivilege(Privilege privilege)
        {
            this.RemovePrivileges(new PrivilegeSet { privilege });
        }

        public void RemovePrivileges()
        {
            NtStatus status;

            if ((status = Win32.LsaRemovePrivilegesFromAccount(
                this,
                true,
                IntPtr.Zero
                )) >= NtStatus.Error)
                Win32.Throw(status);
        }

        public void RemovePrivileges(PrivilegeSet privileges)
        {
            NtStatus status;

            using (var privilegeSetMemory = privileges.ToMemory())
            {
                if ((status = Win32.LsaRemovePrivilegesFromAccount(
                    this,
                    false,
                    privilegeSetMemory
                    )) >= NtStatus.Error)
                    Win32.Throw(status);
            }
        }

        public void SetQuotas(QuotaLimits quotas)
        {
            NtStatus status;

            if ((status = Win32.LsaSetQuotasForAccount(
                this,
                ref quotas
                )) >= NtStatus.Error)
                Win32.Throw(status);
        }

        public void SetSystemAccess(SecuritySystemAccess access)
        {
            NtStatus status;

            if ((status = Win32.LsaSetSystemAccessAccount(
                this,
                access
                )) >= NtStatus.Error)
                Win32.Throw(status);
        }
    }
}
