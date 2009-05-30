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

namespace ProcessHacker.Native.Objects
{
    /// <summary>
    /// Represents a handle to a Windows token.
    /// </summary>
    public class TokenHandle : NativeHandle<TokenAccess>, IEquatable<TokenHandle>
    {
        public struct TokenGroupsData
        {
            public TokenGroups Groups;
            public MemoryAlloc Data;
        }

        /// <summary>
        /// Creates a token handle using an existing handle. 
        /// The handle will not be closed automatically.
        /// </summary>
        /// <param name="Handle">The handle value.</param>
        /// <returns>The token handle.</returns>
        public static TokenHandle FromHandle(IntPtr handle)
        {
            return new TokenHandle(handle, false);
        }

        public static TokenHandle Logon(string username, string domain, string password, LogonType logonType, LogonProvider logonProvider)
        {
            IntPtr token;

            if (!Win32.LogonUser(username, domain, password, logonType, logonProvider, out token))
                Win32.ThrowLastError();

            return new TokenHandle(token, true);
        }

        public static TokenHandle OpenCurrent(TokenAccess access)
        {
            return new TokenHandle(ThreadHandle.GetCurrent(), access, false);
        }

        public static TokenHandle OpenCurrentPrimary(TokenAccess access)
        {
            return new TokenHandle(ProcessHandle.GetCurrent(), access);
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
                    Win32.ThrowLastError();
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
                Win32.ThrowLastError();

            this.Handle = h;
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
                Win32.ThrowLastError();

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
                Win32.ThrowLastError(status);

            return equal;
        }

        /// <summary>
        /// Gets the elevation type of the token.
        /// </summary>
        /// <returns>A TOKEN_ELEVATION_TYPE enum.</returns>
        public TokenElevationType GetElevationType()
        {
            MemoryAlloc value = new MemoryAlloc(4);
            int retLen;

            if (!Win32.GetTokenInformation(this, TokenInformationClass.TokenElevationType,
                value, 4, out retLen))
                Win32.ThrowLastError();

            return (TokenElevationType)value.ReadInt32(0);
        }

        /// <summary>
        /// Gets the token's groups.
        /// </summary>
        /// <param name="IncludeDomains">Specifies whether to include the account's domains.</param>
        /// <returns>A TokenGroupsData struct.</returns>
        public TokenGroupsData GetGroups()
        {
            int retLen = 0;

            Win32.GetTokenInformation(this, TokenInformationClass.TokenGroups, IntPtr.Zero, 0, out retLen);

            MemoryAlloc data = new MemoryAlloc(retLen);

            if (!Win32.GetTokenInformation(this, TokenInformationClass.TokenGroups, data,
                data.Size, out retLen))
                Win32.ThrowLastError();

            return new TokenGroupsData() { Groups = GetGroupsInternal(data), Data = data };
        }

        private TokenGroups GetGroupsInternal(MemoryAlloc data)
        {
            uint number = data.ReadUInt32(0);
            TokenGroups groups = new TokenGroups();

            groups.GroupCount = number;
            groups.Groups = new SidAndAttributes[number];

            for (int i = 0; i < number; i++)
            {
                groups.Groups[i] = data.ReadStruct<SidAndAttributes>(4, i);
            }

            return groups;
        }

        /// <summary>
        /// Gets the token's owner.
        /// </summary>
        /// <returns>A WindowsSID instance.</returns>
        public WindowsSid GetOwner()
        {
            int retLen;

            Win32.GetTokenInformation(this, TokenInformationClass.TokenOwner, IntPtr.Zero, 0, out retLen);

            using (MemoryAlloc data = new MemoryAlloc(retLen))
            {
                if (!Win32.GetTokenInformation(this, TokenInformationClass.TokenOwner, data,
                    data.Size, out retLen))
                    Win32.ThrowLastError();

                return new WindowsSid(data.ReadIntPtr(0));
            }
        }

        /// <summary>
        /// Gets the token's primary group.
        /// </summary>
        /// <returns>A WindowsSID instance.</returns>
        public WindowsSid GetPrimaryGroup()
        {
            int retLen;

            Win32.GetTokenInformation(this, TokenInformationClass.TokenPrimaryGroup, IntPtr.Zero, 0, out retLen);

            using (MemoryAlloc data = new MemoryAlloc(retLen))
            {
                if (!Win32.GetTokenInformation(this, TokenInformationClass.TokenPrimaryGroup, data,
                    data.Size, out retLen))
                    Win32.ThrowLastError();

                return new WindowsSid(data.ReadIntPtr(0));
            }
        }

        /// <summary>
        /// Gets the token's privileges.
        /// </summary>
        /// <returns>A TOKEN_PRIVILEGES structure.</returns>
        public TokenPrivileges GetPrivileges()
        {
            int retLen;

            Win32.GetTokenInformation(this, TokenInformationClass.TokenPrivileges, IntPtr.Zero, 0, out retLen);

            using (MemoryAlloc data = new MemoryAlloc(retLen))
            {
                if (!Win32.GetTokenInformation(this, TokenInformationClass.TokenPrivileges, data,
                    data.Size, out retLen))
                    Win32.ThrowLastError();

                uint number = data.ReadUInt32(0);
                TokenPrivileges privileges = new TokenPrivileges();

                privileges.PrivilegeCount = number;
                privileges.Privileges = new LuidAndAttributes[number];

                for (int i = 0; i < number; i++)
                {
                    privileges.Privileges[i] = data.ReadStruct<LuidAndAttributes>(4, i);
                }

                return privileges;
            }
        }

        /// <summary>
        /// Gets the restricted token's restricting SIDs.
        /// </summary>
        /// <returns>A TokenGroupsData struct.</returns>
        public TokenGroupsData GetRestrictingGroups()
        {
            int retLen = 0;

            Win32.GetTokenInformation(this, TokenInformationClass.TokenRestrictedSids, IntPtr.Zero, 0, out retLen);

            MemoryAlloc data = new MemoryAlloc(retLen);

            if (!Win32.GetTokenInformation(this, TokenInformationClass.TokenRestrictedSids, data,
                data.Size, out retLen))
                Win32.ThrowLastError();

            return new TokenGroupsData() { Groups = GetGroupsInternal(data), Data = data };
        }

        /// <summary>
        /// Gets the token's session ID.
        /// </summary>
        /// <returns>The session ID.</returns>
        public int GetSessionId()
        {
            MemoryAlloc sessionId = new MemoryAlloc(4);
            int retLen;

            if (!Win32.GetTokenInformation(this, TokenInformationClass.TokenSessionId,
                sessionId, 4, out retLen))
                Win32.ThrowLastError();

            return sessionId.ReadInt32(0);
        }

        /// <summary>
        /// Gets the token's source.
        /// </summary>
        /// <returns>A TOKEN_SOURCE struct.</returns>
        public TokenSource GetSource()
        {
            TokenSource source = new TokenSource();
            int retLen;

            if (!Win32.GetTokenInformation(this, TokenInformationClass.TokenSource,
                 ref source, Marshal.SizeOf(source), out retLen))
                Win32.ThrowLastError();

            return source;
        }

        /// <summary>
        /// Gets the token's user.
        /// </summary>
        /// <returns>A WindowsSID instance.</returns>
        public WindowsSid GetUser()
        {
            int retLen;

            Win32.GetTokenInformation(this, TokenInformationClass.TokenUser, IntPtr.Zero, 0, out retLen);

            using (MemoryAlloc data = new MemoryAlloc(retLen))
            {
                if (!Win32.GetTokenInformation(this.Handle, TokenInformationClass.TokenUser, data,
                    data.Size, out retLen))
                    Win32.ThrowLastError();

                TokenUser user = data.ReadStruct<TokenUser>();

                return new WindowsSid(user.User.Sid);
            }
        }

        /// <summary>
        /// Gets whether the token has UAC elevation applied.
        /// </summary>
        /// <returns>A boolean.</returns>
        public bool IsElevated()
        {
            MemoryAlloc value = new MemoryAlloc(4);
            int retLen;

            if (!Win32.GetTokenInformation(this, TokenInformationClass.TokenElevation,
                value, 4, out retLen))
                Win32.ThrowLastError();

            return value.ReadInt32(0) != 0;
        }

        /// <summary>
        /// Gets whether virtualization is allowed.
        /// </summary>
        /// <returns>A boolean.</returns>
        public bool IsVirtualizationAllowed()
        {
            MemoryAlloc value = new MemoryAlloc(4);
            int retLen;

            if (!Win32.GetTokenInformation(this, TokenInformationClass.TokenVirtualizationAllowed,
                value, 4, out retLen))
                Win32.ThrowLastError();

            return value.ReadInt32(0) != 0;
        }

        /// <summary>
        /// Gets whether virtualization is enabled.
        /// </summary>
        /// <returns>A boolean.</returns>
        public bool IsVirtualizationEnabled()
        {
            MemoryAlloc value = new MemoryAlloc(4);
            int retLen;

            if (!Win32.GetTokenInformation(this, TokenInformationClass.TokenVirtualizationEnabled,
                value, 4, out retLen))
                Win32.ThrowLastError();

            return value.ReadInt32(0) != 0;
        }

        /// <summary>
        /// Sets a privilege's attributes.
        /// </summary>
        /// <param name="privilegeName">The name of the privilege.</param>
        /// <param name="attributes">The new attributes of the privilege.</param>
        public void SetPrivilege(string privilegeName, SePrivilegeAttributes attributes)
        {
            TokenPrivileges tkp = new TokenPrivileges();

            tkp.Privileges = new LuidAndAttributes[1];

            if (!Win32.LookupPrivilegeValue(null, privilegeName, out tkp.Privileges[0].Luid))
                throw new Exception("Invalid privilege name '" + privilegeName + "'.");

            tkp.PrivilegeCount = 1;
            tkp.Privileges[0].Attributes = attributes;

            int test = 0;
            IntPtr a = IntPtr.Zero;

            Win32.AdjustTokenPrivileges(this, false, ref tkp, 0, IntPtr.Zero, test);

            if (Marshal.GetLastWin32Error() != 0)
                Win32.ThrowLastError();
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
                Win32.ThrowLastError();
            }
        }
    }
}