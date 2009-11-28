/*
 * Process Hacker - 
 *   token handle
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
using System.Runtime.InteropServices;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Security;
using ProcessHacker.Native.Security.AccessControl;

namespace ProcessHacker.Native.Objects
{
    /// <summary>
    /// Represents a handle to a Windows token.
    /// </summary>
    public sealed class TokenHandle : NativeHandle<TokenAccess>, IEquatable<TokenHandle>
    {
        private static readonly TokenSource _phTokenSource = new TokenSource("PROCHACK", Luid.Allocate());

        public static TokenSource PhTokenSource
        {
            get { return _phTokenSource; }
        }

        public static TokenHandle Create(
            TokenAccess access,
            TokenType tokenType,
            Sid user,
            Sid[] groups,
            PrivilegeSet privileges
            )
        {
            using (var administratorsSid = Sid.GetWellKnownSid(WellKnownSidType.WinBuiltinAdministratorsSid))
            using (var thandle = TokenHandle.OpenCurrentPrimary(TokenAccess.Query))
                return Create(access, 0, thandle, tokenType, user, groups, privileges, administratorsSid, administratorsSid);
        }

        public static TokenHandle Create(
            TokenAccess access,
            ObjectFlags objectFlags,
            TokenHandle existingTokenHandle,
            TokenType tokenType,
            Sid user,
            Sid[] groups,
            PrivilegeSet privileges,
            Sid owner,
            Sid primaryGroup
            )
        {
            var statistics = existingTokenHandle.GetStatistics();

            return Create(
                access,
                null,
                objectFlags,
                null,
                tokenType,
                statistics.AuthenticationId,
                statistics.ExpirationTime,
                user,
                groups,
                privileges,
                owner,
                primaryGroup,
                null,
                _phTokenSource
                );
        }

        public static TokenHandle Create(
            TokenAccess access,
            string name,
            ObjectFlags objectFlags,
            DirectoryHandle rootDirectory,
            TokenType tokenType,
            Luid authenticationId,
            long expirationTime,
            Sid user,
            Sid[] groups,
            PrivilegeSet privileges,
            Sid owner,
            Sid primaryGroup,
            Acl defaultDacl,
            TokenSource source
            )
        {
            NtStatus status;
            TokenUser tokenUser = new TokenUser(user);
            TokenGroups tokenGroups = new TokenGroups(groups);
            TokenPrivileges tokenPrivileges = new TokenPrivileges(privileges);
            TokenOwner tokenOwner = new TokenOwner(owner);
            TokenPrimaryGroup tokenPrimaryGroup = new TokenPrimaryGroup(primaryGroup);
            TokenDefaultDacl tokenDefaultDacl = new TokenDefaultDacl(defaultDacl);
            ObjectAttributes oa = new ObjectAttributes(name, objectFlags, rootDirectory);
            IntPtr handle;

            try
            {
                if ((status = Win32.NtCreateToken(
                    out handle,
                    access,
                    ref oa,
                    tokenType,
                    ref authenticationId,
                    ref expirationTime,
                    ref tokenUser,
                    ref tokenGroups,
                    ref tokenPrivileges,
                    ref tokenOwner,
                    ref tokenPrimaryGroup,
                    ref tokenDefaultDacl,
                    ref source
                    )) >= NtStatus.Error)
                    Win32.Throw(status);
            }
            finally
            {
                oa.Dispose();
            }

            return new TokenHandle(handle, true);
        }

        /// <summary>
        /// Creates a token handle using an existing handle. 
        /// The handle will not be closed automatically.
        /// </summary>
        /// <param name="handle">The handle value.</param>
        /// <returns>The token handle.</returns>
        public static TokenHandle FromHandle(IntPtr handle)
        {
            return new TokenHandle(handle, false);
        }

        public static TokenHandle Logon(string username, string domain, string password, LogonType logonType, LogonProvider logonProvider)
        {
            IntPtr token;

            if (!Win32.LogonUser(username, domain, password, logonType, logonProvider, out token))
                Win32.Throw();

            return new TokenHandle(token, true);
        }

        public static TokenHandle OpenCurrent(TokenAccess access)
        {
            return new TokenHandle(ThreadHandle.GetCurrent(), access, false);
        }

        public static TokenHandle OpenCurrentPrimary(TokenAccess access)
        {
            return new TokenHandle(ProcessHandle.Current, access);
        }

        public static TokenHandle OpenSelf(TokenAccess access)
        {
            return new TokenHandle(ThreadHandle.GetCurrent(), access, true);
        }

        public static TokenHandle OpenSystemToken(TokenAccess access)
        {
            using (var phandle = new ProcessHandle(4, OSVersion.MinProcessQueryInfoAccess))
            {
                return phandle.GetToken(access);
            }
        }

        public static TokenHandle OpenSystemToken(TokenAccess access, SecurityImpersonationLevel impersonationLevel, TokenType type)
        {
            using (var phandle = new ProcessHandle(4, OSVersion.MinProcessQueryInfoAccess))
            {
                using (var thandle = phandle.GetToken(TokenAccess.Duplicate | access))
                {
                    return thandle.Duplicate(access, impersonationLevel, type);
                }
            }
        }

        public TokenHandle(IntPtr handle, bool owned)
            : base(handle, owned)
        { }

        /// <summary>
        /// Creates a new token handle from a process.
        /// </summary>
        /// <param name="handle">The process handle.</param>
        /// <param name="access">The desired access to the token.</param>
        public TokenHandle(ProcessHandle handle, TokenAccess access)
        {
            IntPtr h;

            if (KProcessHacker.Instance != null)
            {
                h = new IntPtr(KProcessHacker.Instance.KphOpenProcessToken(handle, access));
            }
            else
            {
                if (!Win32.OpenProcessToken(handle, access, out h))
                {
                    this.MarkAsInvalid();
                    Win32.Throw();
                }
            }

            this.Handle = h;
        }

        /// <summary>
        /// Creates a new token handle from a thread.
        /// </summary>
        /// <param name="handle">The thread handle.</param>
        /// <param name="access">The desired access to the token.</param>
        public TokenHandle(ThreadHandle handle, TokenAccess access)
            : this(handle, access, false)
        { }

        /// <summary>
        /// Creates a new token handle from a thread.
        /// </summary>
        /// <param name="handle">The thread handle.</param>
        /// <param name="access">The desired access to the token.</param>
        /// <param name="openAsSelf">If the thread is currently impersonating, opens the original token.</param>
        public TokenHandle(ThreadHandle handle, TokenAccess access, bool openAsSelf)
        {
            IntPtr h;

            if (!Win32.OpenThreadToken(handle, access, openAsSelf, out h))
            {
                this.MarkAsInvalid();
                Win32.Throw();
            }

            this.Handle = h;
        }

        public void AdjustGroups(Sid[] groups)
        {
            TokenGroups tokenGroups = new TokenGroups();

            tokenGroups.GroupCount = groups.Length;
            tokenGroups.Groups = new SidAndAttributes[groups.Length];

            for (int i = 0; i < groups.Length; i++)
                tokenGroups.Groups[i] = groups[i].ToSidAndAttributes();

            if (!Win32.AdjustTokenGroups(this, false, ref tokenGroups, 0, IntPtr.Zero, IntPtr.Zero))
                Win32.Throw();
        }

        public void AdjustPrivileges(PrivilegeSet privileges)
        {
            var tokenPrivileges = privileges.ToTokenPrivileges();

            Win32.AdjustTokenPrivileges(this, false, ref tokenPrivileges, 0, IntPtr.Zero, IntPtr.Zero);

            if (Marshal.GetLastWin32Error() != 0)
                Win32.Throw();
        }

        public bool CheckPrivileges(PrivilegeSet privileges)
        {
            NtStatus status;
            bool result;

            using (var privilegesMemory = privileges.ToMemory())
            {
                if ((status = Win32.NtPrivilegeCheck(
                    this,
                    privilegesMemory,
                    out result
                    )) >= NtStatus.Error)
                    Win32.Throw(status);

                return result;
            }
        }

        /// <summary>
        /// Duplicates the token.
        /// </summary>
        /// <param name="access">The desired access to the new token.</param>
        /// <param name="impersonationLevel">The new impersonation level.</param>
        /// <param name="type">The new token type.</param>
        /// <returns>A new token.</returns>
        public TokenHandle Duplicate(TokenAccess access, SecurityImpersonationLevel impersonationLevel, TokenType type)
        {
            IntPtr token;

            if (!Win32.DuplicateTokenEx(this, access, IntPtr.Zero, impersonationLevel, type, out token))
                Win32.Throw();

            return new TokenHandle(token, true);
        }

        /// <summary>
        /// Determins whether the token is the same as another token.
        /// </summary>
        /// <param name="other">The other token.</param>
        /// <returns>Whether they are equal.</returns>
        public bool Equals(TokenHandle other)
        {
            NtStatus status;
            bool equal;

            if ((status = Win32.NtCompareTokens(
                this,
                other,
                out equal
                )) >= NtStatus.Error)
                Win32.Throw(status);

            return equal;
        }

        /// <summary>
        /// Gets the elevation type of the token.
        /// </summary>
        /// <returns>A TOKEN_ELEVATION_TYPE enum.</returns>
        public TokenElevationType GetElevationType()
        {
            return (TokenElevationType)this.GetInformationInt32(TokenInformationClass.TokenElevationType);
        }

        /// <summary>
        /// Gets the token's groups.
        /// </summary>
        /// <returns>A TokenGroupsData struct.</returns>
        public Sid[] GetGroups()
        {
            return this.GetGroupsInternal(TokenInformationClass.TokenGroups);
        }

        private Sid[] GetGroupsInternal(TokenInformationClass infoClass)
        {
            int retLen = 0;

            Win32.GetTokenInformation(this, infoClass, IntPtr.Zero, 0, out retLen);

            using (MemoryAlloc data = new MemoryAlloc(retLen))
            {
                if (!Win32.GetTokenInformation(this, infoClass, data,
                    data.Size, out retLen))
                    Win32.Throw();

                int count = data.ReadStruct<TokenGroups>().GroupCount;
                Sid[] sids = new Sid[count];

                for (int i = 0; i < count; i++)
                {
                    var saa = data.ReadStruct<SidAndAttributes>(TokenGroups.GroupsOffset, i);
                    sids[i] = new Sid(saa.Sid, saa.Attributes);
                }

                return sids;
            }
        }

        private int GetInformationInt32(TokenInformationClass infoClass)
        {
            NtStatus status;
            int value;

            value = this.GetInformationInt32(infoClass, out status);

            if (status >= NtStatus.Error)
                Win32.Throw(status);

            return value;
        }

        private int GetInformationInt32(TokenInformationClass infoClass, out NtStatus status)
        {
            int value;
            int retLength;

            status = Win32.NtQueryInformationToken(
                this,
                infoClass,
                out value,
                sizeof(int),
                out retLength
                );

            return value;
        }

        private IntPtr GetInformationIntPtr(TokenInformationClass infoClass)
        {
            NtStatus status;
            IntPtr value;

            value = this.GetInformationIntPtr(infoClass, out status);

            if (status >= NtStatus.Error)
                Win32.Throw(status);

            return value;
        }

        private IntPtr GetInformationIntPtr(TokenInformationClass infoClass, out NtStatus status)
        {
            IntPtr value;
            int retLength;

            status = Win32.NtQueryInformationToken(
                this,
                infoClass,
                out value,
                IntPtr.Size,
                out retLength
                );

            return value;
        }

        public TokenHandle GetLinkedToken()
        {
            NtStatus status;
            IntPtr handle;

            handle = this.GetInformationIntPtr(TokenInformationClass.TokenLinkedToken, out status);

            if (status == NtStatus.NoSuchLogonSession)
                return null;
            if (status >= NtStatus.Error)
                Win32.Throw(status);

            return new TokenHandle(handle, true);
        }

        /// <summary>
        /// Gets the token's owner.
        /// </summary>
        /// <returns>A WindowsSID instance.</returns>
        public Sid GetOwner()
        {
            int retLen;

            Win32.GetTokenInformation(this, TokenInformationClass.TokenOwner, IntPtr.Zero, 0, out retLen);

            using (MemoryAlloc data = new MemoryAlloc(retLen))
            {
                if (!Win32.GetTokenInformation(this, TokenInformationClass.TokenOwner, data,
                    data.Size, out retLen))
                    Win32.Throw();

                return new Sid(data.ReadIntPtr(0));
            }
        }

        /// <summary>
        /// Gets the token's primary group.
        /// </summary>
        /// <returns>A WindowsSID instance.</returns>
        public Sid GetPrimaryGroup()
        {
            int retLen;

            Win32.GetTokenInformation(this, TokenInformationClass.TokenPrimaryGroup, IntPtr.Zero, 0, out retLen);

            using (MemoryAlloc data = new MemoryAlloc(retLen))
            {
                if (!Win32.GetTokenInformation(this, TokenInformationClass.TokenPrimaryGroup, data,
                    data.Size, out retLen))
                    Win32.Throw();

                return new Sid(data.ReadIntPtr(0));
            }
        }

        /// <summary>
        /// Gets the token's privileges.
        /// </summary>
        /// <returns>A TOKEN_PRIVILEGES structure.</returns>
        public Privilege[] GetPrivileges()
        {
            int retLen;

            Win32.GetTokenInformation(this, TokenInformationClass.TokenPrivileges, IntPtr.Zero, 0, out retLen);

            using (MemoryAlloc data = new MemoryAlloc(retLen))
            {
                if (!Win32.GetTokenInformation(this, TokenInformationClass.TokenPrivileges, data,
                    data.Size, out retLen))
                    Win32.Throw();

                uint count = data.ReadUInt32(0);
                Privilege[] privileges = new Privilege[count];

                for (int i = 0; i < count; i++)
                {
                    var laa = data.ReadStruct<LuidAndAttributes>(sizeof(int), i);
                    privileges[i] = new Privilege(this, laa.Luid, laa.Attributes);
                }

                return privileges;
            }
        }

        /// <summary>
        /// Gets the restricted token's restricting SIDs.
        /// </summary>
        /// <returns>A TokenGroupsData struct.</returns>
        public Sid[] GetRestrictingGroups()
        {
            return this.GetGroupsInternal(TokenInformationClass.TokenRestrictedSids);
        }

        /// <summary>
        /// Gets the token's session ID.
        /// </summary>
        /// <returns>The session ID.</returns>
        public int GetSessionId()
        {
            return this.GetInformationInt32(TokenInformationClass.TokenSessionId);
        }

        /// <summary>
        /// Gets the token's source.
        /// </summary>
        /// <returns>A TOKEN_SOURCE struct.</returns>
        public TokenSource GetSource()
        {
            TokenSource source;
            int retLen;

            if (!Win32.GetTokenInformation(this, TokenInformationClass.TokenSource,
                 out source, Marshal.SizeOf(typeof(TokenSource)), out retLen))
                Win32.Throw();

            return source;
        }

        /// <summary>
        /// Gets statistics about the token.
        /// </summary>
        /// <returns>A TOKEN_STATISTICS structure.</returns>
        public TokenStatistics GetStatistics()
        {
            TokenStatistics statistics;
            int retLen;

            if (!Win32.GetTokenInformation(this, TokenInformationClass.TokenStatistics,
                out statistics, Marshal.SizeOf(typeof(TokenStatistics)), out retLen))
                Win32.Throw();

            return statistics;
        }

        /// <summary>
        /// Gets the token's user.
        /// </summary>
        /// <returns>A WindowsSID instance.</returns>
        public Sid GetUser()
        {
            int retLen;

            Win32.GetTokenInformation(this, TokenInformationClass.TokenUser, IntPtr.Zero, 0, out retLen);

            using (MemoryAlloc data = new MemoryAlloc(retLen))
            {
                if (!Win32.GetTokenInformation(this.Handle, TokenInformationClass.TokenUser, data,
                    data.Size, out retLen))
                    Win32.Throw();

                TokenUser user = data.ReadStruct<TokenUser>();

                return new Sid(user.User.Sid, user.User.Attributes);
            }
        }

        /// <summary>
        /// Gets whether the token has UAC elevation applied.
        /// </summary>
        /// <returns>A boolean.</returns>
        public bool IsElevated()
        {
            return this.GetInformationInt32(TokenInformationClass.TokenElevation) != 0;
        }

        /// <summary>
        /// Gets whether virtualization is allowed.
        /// </summary>
        /// <returns>A boolean.</returns>
        public bool IsVirtualizationAllowed()
        {
            return this.GetInformationInt32(TokenInformationClass.TokenVirtualizationAllowed) != 0;
        }

        /// <summary>
        /// Gets whether virtualization is enabled.
        /// </summary>
        /// <returns>A boolean.</returns>
        public bool IsVirtualizationEnabled()
        {
            return this.GetInformationInt32(TokenInformationClass.TokenVirtualizationEnabled) != 0;
        }

        private void SetInformationInt32(TokenInformationClass infoClass, int value)
        {
            NtStatus status;

            if ((status = Win32.NtSetInformationToken(
                this,
                infoClass,
                ref value,
                sizeof(int)
                )) >= NtStatus.Error)
                Win32.Throw(status);
        }

        /// <summary>
        /// Sets a privilege's attributes.
        /// </summary>
        /// <param name="privilegeName">The name of the privilege.</param>
        /// <param name="attributes">The new attributes of the privilege.</param>
        public void SetPrivilege(string privilegeName, SePrivilegeAttributes attributes)
        {
            if (!this.TrySetPrivilege(privilegeName, attributes))
                Win32.Throw();
        }

        public void SetPrivilege(Luid privilegeLuid, SePrivilegeAttributes attributes)
        {
            if (!this.TrySetPrivilege(privilegeLuid, attributes))
                Win32.Throw();
        }

        public void SetSessionId(int sessionId)
        {
            this.SetInformationInt32(TokenInformationClass.TokenSessionId, sessionId);
        }

        /// <summary>
        /// Sets whether virtualization is enabled.
        /// </summary>
        /// <param name="enabled">Whether virtualization is enabled.</param>
        public void SetVirtualizationEnabled(bool enabled)
        {
            int value = enabled ? 1 : 0;

            if (!Win32.SetTokenInformation(this, TokenInformationClass.TokenVirtualizationEnabled, ref value, 4))
            {
                Win32.Throw();
            }
        }

        /// <summary>
        /// Sets a privilege's attributes.
        /// </summary>
        /// <param name="privilegeName">The name of the privilege.</param>
        /// <param name="attributes">The new attributes of the privilege.</param>
        /// <returns>True if the function succeeded, otherwise false.</returns>
        public bool TrySetPrivilege(string privilegeName, SePrivilegeAttributes attributes)
        {
            return this.TrySetPrivilege(
                LsaPolicyHandle.LookupPolicyHandle.LookupPrivilegeValue(privilegeName),
                attributes
                );
        }

        public bool TrySetPrivilege(Luid privilegeLuid, SePrivilegeAttributes attributes)
        {
            TokenPrivileges tkp = new TokenPrivileges();

            tkp.Privileges = new LuidAndAttributes[1];

            tkp.PrivilegeCount = 1;
            tkp.Privileges[0].Attributes = attributes;
            tkp.Privileges[0].Luid = privilegeLuid;

            Win32.AdjustTokenPrivileges(this, false, ref tkp, 0, IntPtr.Zero, IntPtr.Zero);

            if (Marshal.GetLastWin32Error() != 0)
                return false;

            return true;
        }
    }
}