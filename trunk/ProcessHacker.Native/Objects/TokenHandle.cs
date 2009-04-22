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
    public class TokenHandle : Win32Handle<TokenAccess>
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
        public static TokenHandle FromHandle(int handle)
        {
            return new TokenHandle(handle, false);
        }

        public TokenHandle(int handle, bool owned)
            : base(handle, owned)
        { }

        /// <summary>
        /// Creates a new token handle from a process.
        /// </summary>
        /// <param name="handle">The process handle.</param>
        /// <param name="access">The desired access to the token.</param>
        public TokenHandle(ProcessHandle handle, TokenAccess access)
        {
            int h;

            if (KProcessHacker.Instance != null)
            {
                h = KProcessHacker.Instance.KphOpenProcessToken(handle, access);
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
        {
            int h;

            if (!Win32.OpenThreadToken(handle, access, false, out h))
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
            int token;

            if (!Win32.DuplicateTokenEx(this, access, 0, impersonationLevel, type, out token))
                Win32.ThrowLastError();

            return new TokenHandle(token, true);
        }

        /// <summary>
        /// Gets the elevation type of the token.
        /// </summary>
        /// <returns>A TOKEN_ELEVATION_TYPE enum.</returns>
        public TokenElevationType GetElevationType()
        {
            int value;
            int retLen;

            if (!Win32.GetTokenInformation(this, TokenInformationClass.TokenElevationType,
                out value, 4, out retLen))
                Win32.ThrowLastError();

            return (TokenElevationType)value;
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

                return new WindowsSid(data.ReadInt32(0));
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

                return new WindowsSid(data.ReadInt32(0));
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
                if (!Win32.GetTokenInformation(this, TokenInformationClass.TokenPrivileges, data.Memory,
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
            int sessionId;
            int retLen;

            if (!Win32.GetTokenInformation(this, TokenInformationClass.TokenSessionId,
                out sessionId, 4, out retLen))
                Win32.ThrowLastError();

            return sessionId;
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

                return new WindowsSid(user.User.SID);
            }
        }

        /// <summary>
        /// Gets whether the token has UAC elevation applied.
        /// </summary>
        /// <returns>A boolean.</returns>
        public bool IsElevated()
        {
            int value;
            int retLen;

            if (!Win32.GetTokenInformation(this, TokenInformationClass.TokenElevation,
                out value, 4, out retLen))
                Win32.ThrowLastError();

            return value != 0;
        }

        /// <summary>
        /// Gets whether virtualization is allowed.
        /// </summary>
        /// <returns>A boolean.</returns>
        public bool IsVirtualizationAllowed()
        {
            int value;
            int retLen;

            if (!Win32.GetTokenInformation(this, TokenInformationClass.TokenVirtualizationAllowed,
                out value, 4, out retLen))
                Win32.ThrowLastError();

            return value != 0;
        }

        /// <summary>
        /// Gets whether virtualization is enabled.
        /// </summary>
        /// <returns>A boolean.</returns>
        public bool IsVirtualizationEnabled()
        {
            int value;
            int retLen;

            if (!Win32.GetTokenInformation(this, TokenInformationClass.TokenVirtualizationEnabled,
                out value, 4, out retLen))
                Win32.ThrowLastError();

            return value != 0;
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

            if (!Win32.LookupPrivilegeValue(null, privilegeName, ref tkp.Privileges[0].Luid))
                throw new Exception("Invalid privilege name '" + privilegeName + "'.");

            tkp.PrivilegeCount = 1;
            tkp.Privileges[0].Attributes = attributes;

            Win32.AdjustTokenPrivileges(this, 0, ref tkp, 0, 0, 0);

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
                Win32.ThrowLastError();
        }
    }
}