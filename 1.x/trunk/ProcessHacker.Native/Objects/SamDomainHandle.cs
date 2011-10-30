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
            IntPtr handle;

            Win32.SamOpenDomain(
                serverHandle,
                access,
                domainId,
                out handle
                ).ThrowIf();

            this.Handle = handle;
        }

        public void EnumAliases(EnumAliasesDelegate callback)
        {
            int enumerationContext = 0;
            IntPtr buffer;
            int count;

            while (true)
            {
                Win32.SamEnumerateAliasesInDomain(
                    this,
                    ref enumerationContext,
                    out buffer,
                    0x100,
                    out count
                    ).ThrowIf();

                if (count == 0)
                    break;

                using (SamMemoryAlloc bufferAlloc = new SamMemoryAlloc(buffer))
                {
                    for (int i = 0; i < count; i++)
                    {
                        SamRidEnumeration data = bufferAlloc.ReadStruct<SamRidEnumeration>(0, SamRidEnumeration.SizeOf, i);

                        if (!callback(data.Name.Text, data.RelativeId))
                            return;
                    }
                }
            }
        }

        public void EnumGroups(EnumGroupsDelegate callback)
        {
            int enumerationContext = 0;
            IntPtr buffer;
            int count;

            while (true)
            {
                Win32.SamEnumerateGroupsInDomain(
                    this,
                    ref enumerationContext,
                    out buffer,
                    0x100,
                    out count
                    ).ThrowIf();

                if (count == 0)
                    break;

                using (SamMemoryAlloc bufferAlloc = new SamMemoryAlloc(buffer))
                {
                    for (int i = 0; i < count; i++)
                    {
                        SamRidEnumeration data = bufferAlloc.ReadStruct<SamRidEnumeration>(0, SamRidEnumeration.SizeOf, i);

                        if (!callback(data.Name.Text, data.RelativeId))
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
            int enumerationContext = 0;
            IntPtr buffer;
            int count;

            while (true)
            {
                Win32.SamEnumerateUsersInDomain(
                    this,
                    ref enumerationContext,
                    flags,
                    out buffer,
                    0x100,
                    out count
                    ).ThrowIf();

                if (count == 0)
                    break;

                using (SamMemoryAlloc bufferAlloc = new SamMemoryAlloc(buffer))
                {
                    for (int i = 0; i < count; i++)
                    {
                        var data = bufferAlloc.ReadStruct<SamRidEnumeration>(0, SamRidEnumeration.SizeOf, i);

                        if (!callback(data.Name.Text, data.RelativeId))
                            return;
                    }
                }
            }
        }

        public int[] GetAliasMembership(Sid sid)
        {
            IntPtr aliases;
            int count;

            Win32.SamGetAliasMembership(
                this,
                1,
                new IntPtr[] { sid },
                out count,
                out aliases
                ).ThrowIf();

            if (aliases != IntPtr.Zero)
            {
                using (var aliasesAlloc = new SamMemoryAlloc(aliases))
                    return aliasesAlloc.ReadInt32Array(0, count);
            }
            
            return new int[0];
        }

        private SamMemoryAlloc GetInformation(DomainInformationClass infoClass)
        {
            IntPtr buffer;

            Win32.SamQueryInformationDomain(
                this,
                infoClass,
                out buffer
                ).ThrowIf();

            return new SamMemoryAlloc(buffer);
        }

        public DomainPasswordPolicy GetPasswordPolicy()
        {
            using (SamMemoryAlloc data = this.GetInformation(DomainInformationClass.DomainPasswordInformation))
            {
                DomainPasswordInformation info = data.ReadStruct<DomainPasswordInformation>();

                return new DomainPasswordPolicy(
                    info.MinPasswordLength,
                    info.PasswordHistoryLength,
                    info.PasswordProperties,
                    new TimeSpan(-info.MaxPasswordAge),
                    new TimeSpan(-info.MinPasswordAge)
                    );
            }
        }

        public Sid GetSid(int relativeId)
        {
            IntPtr sid;

            Win32.SamRidToSid(
                this,
                relativeId,
                out sid
                ).ThrowIf();

            using (SamMemoryAlloc sidAlloc = new SamMemoryAlloc(sid))
                return new Sid(sidAlloc);
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
            IntPtr names;
            IntPtr use;

            Win32.SamLookupIdsInDomain(
                this,
                relativeIds.Length,
                relativeIds,
                out names,
                out use
                ).ThrowIf();

            using (SamMemoryAlloc namesAlloc = new SamMemoryAlloc(names))
            using (SamMemoryAlloc useAlloc = new SamMemoryAlloc(use))
            {
                string[] nameArray = new string[relativeIds.Length];
                SidNameUse[] useArray = new SidNameUse[relativeIds.Length];

                for (int i = 0; i < relativeIds.Length; i++)
                {
                    nameArray[i] = namesAlloc.ReadStruct<UnicodeString>(0, UnicodeString.SizeOf, i).Text;
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
            IntPtr relativeIds;
            IntPtr use;

            UnicodeString[] nameStr = new UnicodeString[names.Length];

            for (int i = 0; i < names.Length; i++)
                nameStr[i] = new UnicodeString(names[i]);

            try
            {
                Win32.SamLookupNamesInDomain(
                    this,
                    names.Length,
                    nameStr,
                    out relativeIds,
                    out use
                    ).ThrowIf();
            }
            finally
            {
                for (int i = 0; i < names.Length; i++)
                    nameStr[i].Dispose();
            }

            using (SamMemoryAlloc relativeIdsAlloc = new SamMemoryAlloc(relativeIds))
            using (SamMemoryAlloc useAlloc = new SamMemoryAlloc(use))
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
            Win32.SamSetInformationDomain(
                this,
                infoClass,
                buffer
                ).ThrowIf();
        }

        public unsafe void SetPasswordPolicy(DomainPasswordPolicy policy)
        {
            DomainPasswordInformation info = new DomainPasswordInformation
            {
                MinPasswordLength = policy.MinPasswordLength, 
                PasswordHistoryLength = policy.PasswordHistoryLength, 
                PasswordProperties = policy.PasswordProperties,
                MaxPasswordAge = -policy.MaxPasswordAge.Ticks, 
                MinPasswordAge = -policy.MinPasswordAge.Ticks
            };

            this.SetInformation(DomainInformationClass.DomainPasswordInformation, new IntPtr(&info));
        }
    }

    public class DomainPasswordPolicy
    {
        private ushort _minPasswordLength;
        private ushort _passwordHistoryLength;
        private DomainPasswordProperties _passwordProperties;
        private TimeSpan _maxPasswordAge;
        private TimeSpan _minPasswordAge;

        public DomainPasswordPolicy(
            ushort minPasswordLength,
            ushort passwordHistoryLength,
            DomainPasswordProperties passwordProperties,
            TimeSpan maxPasswordAge,
            TimeSpan minPasswordAge
            )
        {
            _minPasswordLength = minPasswordLength;
            _passwordHistoryLength = passwordHistoryLength;
            _passwordProperties = passwordProperties;
            _maxPasswordAge = maxPasswordAge;
            _minPasswordAge = minPasswordAge;
        }

        public ushort MinPasswordLength { get { return _minPasswordLength; } set { _minPasswordLength = value; } }
        public ushort PasswordHistoryLength { get { return _passwordHistoryLength; } set { _passwordHistoryLength = value; } }
        public DomainPasswordProperties PasswordProperties { get { return _passwordProperties; } set { _passwordProperties = value; } }
        public TimeSpan MaxPasswordAge { get { return _maxPasswordAge; } set { _maxPasswordAge = value; } }
        public TimeSpan MinPasswordAge { get { return _minPasswordAge; } set { _minPasswordAge = value; } }
    }
}
