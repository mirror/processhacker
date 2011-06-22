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
            IntPtr handle;

            Win32.LsaCreateAccount(policyHandle, sid, access, out handle).ThrowIf();

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
            IntPtr handle;

            Win32.LsaOpenAccount(policyHandle, sid, access, out handle).ThrowIf();

            this.Handle = handle;
        }

        public void AddPrivilege(Privilege privilege)
        {
            this.AddPrivileges(new PrivilegeSet { privilege });
        }

        public void AddPrivileges(PrivilegeSet privileges)
        {
            using (var privilegeSetMemory = privileges.ToMemory())
            {
                Win32.LsaAddPrivilegesToAccount(this, privilegeSetMemory).ThrowIf();
            }
        }

        public PrivilegeSet GetPrivileges()
        {
            IntPtr privileges;

            Win32.LsaEnumeratePrivilegesOfAccount(this, out privileges).ThrowIf();

            using (LsaMemoryAlloc privilegesAlloc = new LsaMemoryAlloc(privileges))
            {
                return new PrivilegeSet(privilegesAlloc);
            }
        }

        public QuotaLimits GetQuotas()
        {
            QuotaLimits quotas;

            Win32.LsaGetQuotasForAccount(this, out quotas).ThrowIf();

            return quotas;
        }

        public SecuritySystemAccess GetSystemAccess()
        {
            SecuritySystemAccess access;

            Win32.LsaGetSystemAccessAccount(this, out access).ThrowIf();

            return access;
        }

        public void RemovePrivilege(Privilege privilege)
        {
            this.RemovePrivileges(new PrivilegeSet { privilege });
        }

        public void RemovePrivileges()
        {
            Win32.LsaRemovePrivilegesFromAccount(this, true, IntPtr.Zero).ThrowIf();
        }

        public void RemovePrivileges(PrivilegeSet privileges)
        {
            using (MemoryAlloc privilegeSetMemory = privileges.ToMemory())
            {
                Win32.LsaRemovePrivilegesFromAccount(this, false, privilegeSetMemory).ThrowIf();
            }
        }

        public void SetQuotas(QuotaLimits quotas)
        {
            Win32.LsaSetQuotasForAccount(this, quotas).ThrowIf();
        }

        public void SetSystemAccess(SecuritySystemAccess access)
        {
            Win32.LsaSetSystemAccessAccount(this, access).ThrowIf();
        }
    }
}
