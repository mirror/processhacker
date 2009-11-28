/*
 * Process Hacker - 
 *   SAM domain handle
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
    /// Represents a handle to a SAM domain.
    /// </summary>
    public sealed class SamDomainHandle : SamHandle<SamDomainAccess>
    {
        internal static DateTime ToDateTime(long fileTime)
        {
            if (fileTime == long.MaxValue)
                return DateTime.MaxValue;
            if (fileTime == 0)
                return DateTime.MinValue;

            return DateTime.FromFileTime(fileTime);
        }

        public delegate bool EnumAliasesDelegate(string name, int aliasId);
        public delegate bool EnumGroupsDelegate(string name, int groupId);
        public delegate bool EnumUsersDelegate(string name, int userId);

        private SamDomainHandle(IntPtr handle, bool owned)
            : base(handle, owned)
        { }

        /// <summary>
        /// Opens a SAM account.
        /// </summary>
        /// <param name="name">The name of the domain to open.</param>
        /// <param name="access">The desired access to the domain.</param>
        public SamDomainHandle(string name, SamDomainAccess access)
            : this(SamServerHandle.ConnectServerHandle, SamServerHandle.ConnectServerHandle.LookupDomain(name), access)
        { }

        /// <summary>
        /// Opens a SAM account.
        /// </summary>
        /// <param name="domainId">The SID of the domain to open.</param>
        /// <param name="access">The desired access to the domain.</param>
        public SamDomainHandle(Sid domainId, SamDomainAccess access)
            : this(SamServerHandle.ConnectServerHandle, domainId, access)
        { }

        /// <summary>
        /// Opens a SAM account.
        /// </summary>
        /// <param name="serverHandle">A handle to a SAM server.</param>
        /// <param name="name">The name of the domain to open.</param>
        /// <param name="access">The desired access to the domain.</param>
        public SamDomainHandle(SamServerHandle serverHandle, string name, SamDomainAccess access)
            : this(serverHandle, serverHandle.LookupDomain(name), access)
        { }

        /// <summary>
        /// Opens a SAM account.
        /// </summary>
        /// <param name="serverHandle">A handle to a SAM server.</param>
        /// <param name="domainId">The SID of the domain to open.</param>
        /// <param name="access">The desired access to the domain.</param>
        public SamDomainHandle(SamServerHandle serverHandle, Sid domainId, SamDomainAccess access)
        {
            NtStatus status;
            IntPtr handle;

            if ((status = Win32.SamOpenDomain(
                serverHandle,
                access,
                domainId,
                out handle
                )) >= NtStatus.Error)
                Win32.Throw(status);

            this.Handle = handle;
        }

        public void EnumAliases(EnumAliasesDelegate callback)
        {
            NtStatus status;
            int enumerationContext = 0;
            IntPtr buffer;
            int count;

            while (true)
            {
                status = Win32.SamEnumerateAliasesInDomain(
                    this,
                    ref enumerationContext,
                    out buffer,
                    0x100,
                    out count
                    );

                if (status >= NtStatus.Error)
                    Win32.Throw(status);
                if (count == 0)
                    break;

                using (var bufferAlloc = new SamMemoryAlloc(buffer))
                {
                    for (int i = 0; i < count; i++)
                    {
                        var data = bufferAlloc.ReadStruct<SamRidEnumeration>(i);

                        if (!callback(data.Name.Read(), data.RelativeId))
                            return;
                    }
                }
            }
        }

        public void EnumGroups(EnumGroupsDelegate callback)
        {
            NtStatus status;
            int enumerationContext = 0;
            IntPtr buffer;
            int count;

            while (true)
            {
                status = Win32.SamEnumerateGroupsInDomain(
                    this,
                    ref enumerationContext,
                    out buffer,
                    0x100,
                    out count
                    );

                if (status >= NtStatus.Error)
                    Win32.Throw(status);
                if (count == 0)
                    break;

                using (var bufferAlloc = new SamMemoryAlloc(buffer))
                {
                    for (int i = 0; i < count; i++)
                    {
                        var data = bufferAlloc.ReadStruct<SamRidEnumeration>(i);

                        if (!callback(data.Name.Read(), data.RelativeId))
                            return;
                    }
                }
            }
        }

        public void EnumUsers(EnumUsersDelegate callback)
        {
            this.EnumUsers(callback, UserAccountFlags.AccountTypeMask);
        }

        public void EnumUsers(EnumUsersDelegate callback, UserAccountFlags flags)
        {
            NtStatus status;
            int enumerationContext = 0;
            IntPtr buffer;
            int count;

            while (true)
            {
                status = Win32.SamEnumerateUsersInDomain(
                    this,
                    ref enumerationContext,
                    flags,
                    out buffer,
                    0x100,
                    out count
                    );

                if (status >= NtStatus.Error)
                    Win32.Throw(status);
                if (count == 0)
                    break;

                using (var bufferAlloc = new SamMemoryAlloc(buffer))
                {
                    for (int i = 0; i < count; i++)
                    {
                        var data = bufferAlloc.ReadStruct<SamRidEnumeration>(i);

                        if (!callback(data.Name.Read(), data.RelativeId))
                            return;
                    }
                }
            }
        }

        private SamMemoryAlloc GetInformation(DomainInformationClass infoClass)
        {
            NtStatus status;
            IntPtr buffer;

            if ((status = Win32.SamQueryInformationDomain(
                this,
                infoClass,
                out buffer
                )) >= NtStatus.Error)
                Win32.Throw(status);

            return new SamMemoryAlloc(buffer);
        }

        public string LookupId(int relativeId)
        {
            return this.LookupIds(new int[] { relativeId })[0];
        }

        public string[] LookupIds(int[] relativeIds)
        {
            SidNameUse[] uses;

            return this.LookupIds(relativeIds, out uses);
        }

        public string[] LookupIds(int[] relativeIds, out SidNameUse[] uses)
        {
            NtStatus status;
            IntPtr names;
            IntPtr use;

            if ((status = Win32.SamLookupIdsInDomain(
                this,
                relativeIds.Length,
                relativeIds,
                out names,
                out use
                )) >= NtStatus.Error)
                Win32.Throw(status);

            using (var namesAlloc = new SamMemoryAlloc(names))
            using (var useAlloc = new SamMemoryAlloc(use))
            {
                string[] nameArray = new string[relativeIds.Length];
                SidNameUse[] useArray = new SidNameUse[relativeIds.Length];

                for (int i = 0; i < relativeIds.Length; i++)
                {
                    nameArray[i] = namesAlloc.ReadStruct<UnicodeString>(i).Read();
                    useArray[i] = (SidNameUse)useAlloc.ReadInt32(0, i);
                }

                uses = useArray;

                return nameArray;
            }
        }

        public int LookupName(string name)
        {
            return this.LookupNames(new string[] { name })[0];
        }

        public int[] LookupNames(string[] names)
        {
            SidNameUse[] uses;

            return this.LookupNames(names, out uses);
        }

        public int[] LookupNames(string[] names, out SidNameUse[] uses)
        {
            NtStatus status;
            UnicodeString[] nameStr;
            IntPtr relativeIds;
            IntPtr use;

            nameStr = new UnicodeString[names.Length];

            for (int i = 0; i < names.Length; i++)
                nameStr[i] = new UnicodeString(names[i]);

            try
            {
                if ((status = Win32.SamLookupNamesInDomain(
                    this,
                    names.Length,
                    nameStr,
                    out relativeIds,
                    out use
                    )) >= NtStatus.Error)
                    Win32.Throw(status);
            }
            finally
            {
                for (int i = 0; i < names.Length; i++)
                    nameStr[i].Dispose();
            }

            using (var relativeIdsAlloc = new SamMemoryAlloc(relativeIds))
            using (var useAlloc = new SamMemoryAlloc(use))
            {
                SidNameUse[] useArray = new SidNameUse[names.Length];

                for (int i = 0; i < names.Length; i++)
                    useArray[i] = (SidNameUse)useAlloc.ReadInt32(0, i);

                uses = useArray;

                return relativeIdsAlloc.ReadInt32Array(0, names.Length);
            }
        }

        private void SetInformation(DomainInformationClass infoClass, IntPtr buffer)
        {
            NtStatus status;

            if ((status = Win32.SamSetInformationDomain(
                this,
                infoClass,
                buffer
                )) >= NtStatus.Error)
                Win32.Throw(status);
        }
    }
}
