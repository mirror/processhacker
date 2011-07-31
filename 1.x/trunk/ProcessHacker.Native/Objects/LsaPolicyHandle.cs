/*
 * Process Hacker - 
 *   LSA policy handle
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
using System.Collections.Generic;
using ProcessHacker.Common;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Security;

namespace ProcessHacker.Native.Objects
{
    /// <summary>
    /// Represents a handle to a LSA policy.
    /// </summary>
    public sealed class LsaPolicyHandle : LsaHandle<LsaPolicyAccess>
    {
        private static WeakReference<LsaPolicyHandle> _lookupPolicyHandle;
        private static int _lookupPolicyHandleMisses = 0;

        public static LsaPolicyHandle LookupPolicyHandle
        {
            get
            {
                WeakReference<LsaPolicyHandle> weakRef = _lookupPolicyHandle;
                LsaPolicyHandle policyHandle = null;

                if (weakRef != null)
                {
                    policyHandle = weakRef.Target;
                }

                if (policyHandle == null)
                {
                    System.Threading.Interlocked.Increment(ref _lookupPolicyHandleMisses);

                    policyHandle = new LsaPolicyHandle(LsaPolicyAccess.LookupNames);

                    if (policyHandle != null)
                        _lookupPolicyHandle = new WeakReference<LsaPolicyHandle>(policyHandle);
                }

                return policyHandle;
            }
        }

        public static int LookupPolicyHandleMisses
        {
            get { return _lookupPolicyHandleMisses; }
        }

        public delegate bool EnumAccountsDelegate(Sid sid);
        public delegate bool EnumPrivilegesDelegate(Privilege privilege);

        /// <summary>
        /// Opens the local LSA policy object.
        /// </summary>
        /// <param name="access">The desired access to the policy.</param>
        public LsaPolicyHandle(LsaPolicyAccess access)
            : this(null, access)
        { }

        /// <summary>
        /// Opens a LSA policy object.
        /// </summary>
        /// <param name="systemName">The name of the system on which the policy resides.</param>
        /// <param name="access">The desired access to the policy.</param>
        public LsaPolicyHandle(string systemName, LsaPolicyAccess access)
        {
            NtStatus status;
            ObjectAttributes oa = new ObjectAttributes();
            UnicodeString systemNameStr;
            IntPtr handle;

            systemNameStr = new UnicodeString(systemName);

            try
            {
                if ((status = Win32.LsaOpenPolicy(
                    ref systemNameStr,
                    ref oa,
                    access,
                    out handle
                    )) >= NtStatus.Error)
                    Win32.Throw(status);
            }
            finally
            {
                systemNameStr.Dispose();
            }

            this.Handle = handle;
        }

        public void AddPrivilege(Sid accountSid, string privilege)
        {
            this.AddPrivileges(accountSid, new string[] { privilege });
        }

        public void AddPrivileges(Sid accountSid, string[] privileges)
        {
            NtStatus status;
            UnicodeString[] privilegeStrArray = new UnicodeString[privileges.Length];

            for (int i = 0; i < privileges.Length; i++)
                privilegeStrArray[i] = new UnicodeString(privileges[i]);

            try
            {
                if ((status = Win32.LsaAddAccountRights(
                    this,
                    accountSid,
                    privilegeStrArray,
                    privilegeStrArray.Length
                    )) >= NtStatus.Error)
                    Win32.Throw(status);
            }
            finally
            {
                for (int i = 0; i < privilegeStrArray.Length; i++)
                    privilegeStrArray[i].Dispose();
            }
        }

        public void DeletePrivateData(string name)
        {
            NtStatus status;
            UnicodeString nameStr;

            nameStr = new UnicodeString(name);

            try
            {
                if ((status = Win32.LsaStorePrivateData(
                    this,
                    ref nameStr,
                    IntPtr.Zero
                    )) >= NtStatus.Error)
                    Win32.Throw(status);
            }
            finally
            {
                nameStr.Dispose();
            }
        }

        /// <summary>
        /// Enumerates the accounts in the policy. This requires 
        /// ViewLocalInformation access.
        /// </summary>
        /// <param name="callback">The callback for the enumeration.</param>
        public void EnumAccounts(EnumAccountsDelegate callback)
        {
            NtStatus status;
            int enumerationContext = 0;
            IntPtr buffer;
            int count;

            while (true)
            {
                status = Win32.LsaEnumerateAccounts(
                    this,
                    ref enumerationContext,
                    out buffer,
                    0x100,
                    out count
                    );

                if (status == NtStatus.NoMoreEntries)
                    break;
                if (status >= NtStatus.Error)
                    Win32.Throw(status);

                using (var bufferAlloc = new LsaMemoryAlloc(buffer))
                {
                    for (int i = 0; i < count; i++)
                    {
                        if (!callback(new Sid(bufferAlloc.ReadIntPtr(0, i))))
                            return;
                    }
                }
            }
        }

        /// <summary>
        /// Enumerates the accounts in the policy with the specified privilege. 
        /// This requires LookupNames, ViewLocalInformation and usually 
        /// administrator access.
        /// </summary>
        /// <param name="privilegeName">The name of the required privilege.</param>
        /// <param name="callback">The callback for the enumeration.</param>
        public void EnumAccountsWithPrivilege(string privilegeName, EnumAccountsDelegate callback)
        {
            NtStatus status;
            UnicodeString privilegeNameStr;
            IntPtr buffer;
            int count;

            privilegeNameStr = new UnicodeString(privilegeName);

            try
            {
                if ((status = Win32.LsaEnumerateAccountsWithUserRight(
                    this,
                    ref privilegeNameStr,
                    out buffer,
                    out count
                    )) >= NtStatus.Error)
                    Win32.Throw(status);
            }
            finally
            {
                privilegeNameStr.Dispose();
            }

            Sid[] sids = new Sid[count];

            using (var bufferAlloc = new LsaMemoryAlloc(buffer))
            {
                for (int i = 0; i < count; i++)
                {
                    if (!callback(new Sid(bufferAlloc.ReadIntPtr(0, i))))
                        break;
                }
            }
        }

        /// <summary>
        /// Enumerates the privileges in the policy. This requires 
        /// ViewLocalInformation access.
        /// </summary>
        /// <param name="callback">The callback for the enumeration.</param>
        public void EnumPrivileges(EnumPrivilegesDelegate callback)
        {
            NtStatus status;
            int enumerationContext = 0;
            IntPtr buffer;
            int count;

            while (true)
            {
                status = Win32.LsaEnumeratePrivileges(
                    this,
                    ref enumerationContext,
                    out buffer,
                    0x100,
                    out count
                    );

                if (status == NtStatus.NoMoreEntries)
                    break;
                if (status >= NtStatus.Error)
                    Win32.Throw(status);

                using (var bufferAlloc = new LsaMemoryAlloc(buffer))
                {
                    for (int i = 0; i < count; i++)
                    {
                        if (!callback(new Privilege(bufferAlloc.ReadStruct<PolicyPrivilegeDefinition>(i).Name.Read())))
                            return;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the accounts in the policy. This requires 
        /// ViewLocalInformation access.
        /// </summary>
        public Sid[] GetAccounts()
        {
            List<Sid> sids = new List<Sid>();

            this.EnumAccounts((sid) =>
                {
                    sids.Add(sid);
                    return true;
                });

            return sids.ToArray();
        }

        /// <summary>
        /// Gets the accounts in the policy with the specified privilege. 
        /// This requires LookupNames, ViewLocalInformation and usually 
        /// administrator access.
        /// </summary>
        /// <param name="privilegeName">The name of the required privilege.</param>
        public Sid[] GetAccountsWithPrivilege(string privilegeName)
        {
            List<Sid> sids = new List<Sid>();

            this.EnumAccountsWithPrivilege(privilegeName, (sid) =>
            {
                sids.Add(sid);
                return true;
            });

            return sids.ToArray();
        }

        public Privilege[] GetPrivileges()
        {
            List<Privilege> privileges = new List<Privilege>();

            this.EnumPrivileges((privilege) =>
            {
                privileges.Add(privilege);
                return true;
            });

            return privileges.ToArray();
        }

        public string LookupName(Sid sid)
        {
            SidNameUse nameUse;

            return this.LookupName(sid, out nameUse);
        }

        public string LookupName(Sid sid, out SidNameUse nameUse)
        {
            string domainName;

            return this.LookupName(sid, out nameUse, out domainName);
        }

        public string LookupName(Sid sid, out string domainName)
        {
            SidNameUse nameUse;

            return this.LookupName(sid, out nameUse, out domainName);
        }

        public string LookupName(Sid sid, out SidNameUse nameUse, out string domainName)
        {
            NtStatus status;
            IntPtr referencedDomains;
            IntPtr names;

            if ((status = Win32.LsaLookupSids(
                    this,
                    1,
                    new IntPtr[] { sid },
                    out referencedDomains,
                    out names
                )) >= NtStatus.Error)
            {
                if (status == NtStatus.NoneMapped)
                {
                    nameUse = SidNameUse.Unknown;
                    domainName = null;
                    return null;
                }

                Win32.Throw(status);
            }

            using (var referencedDomainsAlloc = new LsaMemoryAlloc(referencedDomains))
            using (var namesAlloc = new LsaMemoryAlloc(names))
            {
                LsaTranslatedName translatedName = namesAlloc.ReadStruct<LsaTranslatedName>();

                nameUse = translatedName.Use;

                if (nameUse == SidNameUse.Invalid || nameUse == SidNameUse.Unknown)
                {
                    domainName = null;

                    return null;
                }

                if (translatedName.DomainIndex != -1)
                {
                    LsaReferencedDomainList domains = referencedDomainsAlloc.ReadStruct<LsaReferencedDomainList>();
                    MemoryRegion trustArray = new MemoryRegion(domains.Domains);
                    LsaTrustInformation trustInfo = trustArray.ReadStruct<LsaTrustInformation>(translatedName.DomainIndex);

                    domainName = trustInfo.Name.Read();
                }
                else
                {
                    domainName = null;
                }

                return translatedName.Name.Read();
            }
        }

        public string LookupPrivilegeDisplayName(Luid value)
        {
            return this.LookupPrivilegeDisplayName(this.LookupPrivilegeName(value));
        }

        public string LookupPrivilegeDisplayName(string name)
        {
            NtStatus status;
            UnicodeString nameStr;
            IntPtr displayName;
            short language;

            nameStr = new UnicodeString(name);

            try
            {
                if ((status = Win32.LsaLookupPrivilegeDisplayName(
                    this,
                    ref nameStr,
                    out displayName,
                    out language
                    )) >= NtStatus.Error)
                    Win32.Throw(status);
            }
            finally
            {
                nameStr.Dispose();
            }

            using (var displayNameAlloc = new LsaMemoryAlloc(displayName))
            {
                return displayNameAlloc.ReadStruct<UnicodeString>().Read();
            }
        }

        public string LookupPrivilegeName(Luid value)
        {
            NtStatus status;
            IntPtr name;

            if ((status = Win32.LsaLookupPrivilegeName(
                this,
                ref value,
                out name
                )) >= NtStatus.Error)
                Win32.Throw(status);

            using (var nameAlloc = new LsaMemoryAlloc(name))
            {
                return nameAlloc.ReadStruct<UnicodeString>().Read();
            }
        }

        public Luid LookupPrivilegeValue(string name)
        {
            NtStatus status;
            UnicodeString nameStr;
            Luid luid;

            nameStr = new UnicodeString(name);

            try
            {
                if ((status = Win32.LsaLookupPrivilegeValue(
                    this,
                    ref nameStr,
                    out luid
                    )) >= NtStatus.Error)
                    Win32.Throw(status);
            }
            finally
            {
                nameStr.Dispose();
            }

            return luid;
        }

        public Sid LookupSid(string name)
        {
            SidNameUse nameUse;

            return this.LookupSid(name, out nameUse);
        }

        public Sid LookupSid(string name, out SidNameUse nameUse)
        {
            string domainName;

            return this.LookupSid(name, out nameUse, out domainName);
        }

        public Sid LookupSid(string name, out string domainName)
        {
            SidNameUse nameUse;

            return this.LookupSid(name, out nameUse, out domainName);
        }

        public Sid LookupSid(string name, out SidNameUse nameUse, out string domainName)
        {
            NtStatus status;
            UnicodeString nameStr;
            IntPtr referencedDomains;
            IntPtr sids;

            nameStr = new UnicodeString(name);

            try
            {
                if ((status = Win32.LsaLookupNames2(
                    this,
                    0,
                    1,
                    new UnicodeString[] { nameStr },
                    out referencedDomains,
                    out sids
                    )) >= NtStatus.Error)
                {
                    if (status == NtStatus.NoneMapped)
                    {
                        nameUse = SidNameUse.Unknown;
                        domainName = null;
                        return null;
                    }

                    Win32.Throw(status);
                }
            }
            finally
            {
                nameStr.Dispose();
            }

            using (var referencedDomainsAlloc = new LsaMemoryAlloc(referencedDomains))
            using (var sidsAlloc = new LsaMemoryAlloc(sids))
            {
                LsaTranslatedSid2 translatedSid = sidsAlloc.ReadStruct<LsaTranslatedSid2>();

                nameUse = translatedSid.Use;

                if (nameUse == SidNameUse.Invalid || nameUse == SidNameUse.Unknown)
                {
                    domainName = null;

                    return null;
                }

                if (translatedSid.DomainIndex != -1)
                {
                    LsaReferencedDomainList domains = referencedDomainsAlloc.ReadStruct<LsaReferencedDomainList>();
                    MemoryRegion trustArray = new MemoryRegion(domains.Domains);
                    LsaTrustInformation trustInfo = trustArray.ReadStruct<LsaTrustInformation>(translatedSid.DomainIndex);

                    domainName = trustInfo.Name.Read();
                }
                else
                {
                    domainName = null;
                }

                return new Sid(translatedSid.Sid);
            }
        }

        public void RemovePrivilege(Sid accountSid, string privilege)
        {
            this.RemovePrivileges(accountSid, new string[] { privilege });
        }

        public void RemovePrivileges(Sid accountSid)
        {
            NtStatus status;

            if ((status = Win32.LsaRemoveAccountRights(
                this,
                accountSid,
                true,
                null,
                0
                )) >= NtStatus.Error)
                Win32.Throw(status);
        }

        public void RemovePrivileges(Sid accountSid, string[] privileges)
        {
            NtStatus status;
            UnicodeString[] privilegeStrArray = new UnicodeString[privileges.Length];

            for (int i = 0; i < privileges.Length; i++)
                privilegeStrArray[i] = new UnicodeString(privileges[i]);

            try
            {
                if ((status = Win32.LsaRemoveAccountRights(
                    this,
                    accountSid,
                    false,
                    privilegeStrArray,
                    privilegeStrArray.Length
                    )) >= NtStatus.Error)
                    Win32.Throw(status);
            }
            finally
            {
                for (int i = 0; i < privilegeStrArray.Length; i++)
                    privilegeStrArray[i].Dispose();
            }
        }

        public string RetrievePrivateData(string name)
        {
            NtStatus status;
            UnicodeString nameStr;
            IntPtr privateData;

            nameStr = new UnicodeString(name);

            try
            {
                if ((status = Win32.LsaRetrievePrivateData(
                    this,
                    ref nameStr,
                    out privateData
                    )) >= NtStatus.Error)
                    Win32.Throw(status);
            }
            finally
            {
                nameStr.Dispose();
            }

            using (var privateDataAlloc = new LsaMemoryAlloc(privateData))
                return privateDataAlloc.ReadStruct<UnicodeString>().Read();
        }

        public void StorePrivateData(string name, string privateData)
        {
            NtStatus status;
            UnicodeString nameStr;
            UnicodeString privateDataStr;

            nameStr = new UnicodeString(name);
            privateDataStr = new UnicodeString(privateData);

            try
            {
                if ((status = Win32.LsaStorePrivateData(
                    this,
                    ref nameStr,
                    ref privateDataStr
                    )) >= NtStatus.Error)
                    Win32.Throw(status);
            }
            finally
            {
                nameStr.Dispose();
                privateDataStr.Dispose();
            }
        }
    }
}
