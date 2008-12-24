/*
 * Process Hacker
 * 
 * Copyright (C) 2008 wj32
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace ProcessHacker
{
    public partial class Win32
    {
        /// <summary>
        /// Represents a handle to a Windows token.
        /// </summary>
        public class TokenHandle : Win32Handle
        {
            public struct TokenGroupsData
            {
                public TOKEN_GROUPS Groups;
                public MemoryAlloc Data;
            }

            /// <summary>
            /// Creates a token handle using an existing handle. 
            /// The handle will not be closed automatically.
            /// </summary>
            /// <param name="Handle">The handle value.</param>
            /// <returns>The token handle.</returns>
            public static TokenHandle FromHandle(int Handle)
            {
                return new TokenHandle(Handle, false);
            }

            public TokenHandle(int Handle, bool Owned)
                : base(Handle, Owned)
            { }

            /// <summary>
            /// Creates a new token handle from a process.
            /// </summary>
            /// <param name="handle">The process handle.</param>
            /// <param name="access">The desired access to the token.</param>
            public TokenHandle(ProcessHandle handle, TOKEN_RIGHTS access)
            {
                int h;

                if (!OpenProcessToken(handle.Handle, access, out h))
                    throw new Exception(GetLastErrorMessage());

                this.Handle = h;
            }

            /// <summary>
            /// Creates a new token handle from a thread.
            /// </summary>
            /// <param name="handle">The thread handle.</param>
            /// <param name="access">The desired access to the token.</param>
            public TokenHandle(ThreadHandle handle, TOKEN_RIGHTS access)
            {
                int h;

                if (!OpenThreadToken(handle.Handle, access, false, out h))
                    throw new Exception(GetLastErrorMessage());

                this.Handle = h;
            }

            /// <summary>
            /// Gets the elevation type of the token.
            /// </summary>
            /// <returns>A TOKEN_ELEVATION_TYPE enum.</returns>
            public TOKEN_ELEVATION_TYPE GetElevationType()
            {
                int value;
                int retLen;

                if (!GetTokenInformation(this, TOKEN_INFORMATION_CLASS.TokenElevationType,
                    out value, 4, out retLen))
                    throw new Exception(GetLastErrorMessage());

                return (TOKEN_ELEVATION_TYPE)value;
            }

            /// <summary>
            /// Gets the token's groups.
            /// </summary>
            /// <param name="IncludeDomains">Specifies whether to include the account's domains.</param>
            /// <returns>A TokenGroupsData struct.</returns>
            public TokenGroupsData GetGroups()
            {
                int retLen = 0;

                GetTokenInformation(this, TOKEN_INFORMATION_CLASS.TokenGroups, IntPtr.Zero, 0, out retLen);

                MemoryAlloc data = new MemoryAlloc(retLen);
                
                if (!GetTokenInformation(this, TOKEN_INFORMATION_CLASS.TokenGroups, data,
                    data.Size, out retLen))
                    throw new Exception(GetLastErrorMessage());

                return new TokenGroupsData() { Groups = GetGroupsInternal(data), Data = data };
            }

            private TOKEN_GROUPS GetGroupsInternal(MemoryAlloc data)
            {
                uint number = data.ReadUInt32(0);
                TOKEN_GROUPS groups = new TOKEN_GROUPS();

                groups.GroupCount = number;
                groups.Groups = new SID_AND_ATTRIBUTES[number];

                for (int i = 0; i < number; i++)
                {
                    groups.Groups[i] = data.ReadStruct<SID_AND_ATTRIBUTES>(4, i);
                }

                return groups;
            }

            /// <summary>
            /// Gets the token's owner.
            /// </summary>
            /// <returns>A WindowsSID instance.</returns>
            public WindowsSID GetOwner()
            {
                int retLen;

                GetTokenInformation(this, TOKEN_INFORMATION_CLASS.TokenOwner, IntPtr.Zero, 0, out retLen);

                using (MemoryAlloc data = new MemoryAlloc(retLen))
                {
                    if (!GetTokenInformation(this, TOKEN_INFORMATION_CLASS.TokenOwner, data,
                        data.Size, out retLen))
                        throw new Exception(Win32.GetLastErrorMessage());

                    return new WindowsSID(data.ReadInt32(0));
                }
            }

            /// <summary>
            /// Gets the token's primary group.
            /// </summary>
            /// <returns>A WindowsSID instance.</returns>
            public WindowsSID GetPrimaryGroup()
            {
                int retLen;

                GetTokenInformation(this, TOKEN_INFORMATION_CLASS.TokenPrimaryGroup, IntPtr.Zero, 0, out retLen);

                using (MemoryAlloc data = new MemoryAlloc(retLen))
                {
                    if (!GetTokenInformation(this, TOKEN_INFORMATION_CLASS.TokenPrimaryGroup, data,
                        data.Size, out retLen))
                        throw new Exception(Win32.GetLastErrorMessage());

                    return new WindowsSID(data.ReadInt32(0));
                }
            }

            /// <summary>
            /// Gets the restricted token's restricting SIDs.
            /// </summary>
            /// <returns>A TokenGroupsData struct.</returns>
            public TokenGroupsData GetRestrictingGroups()
            {
                int retLen = 0;

                GetTokenInformation(this, TOKEN_INFORMATION_CLASS.TokenRestrictedSids, IntPtr.Zero, 0, out retLen);

                MemoryAlloc data = new MemoryAlloc(retLen);
             
                if (!GetTokenInformation(this, TOKEN_INFORMATION_CLASS.TokenRestrictedSids, data,
                    data.Size, out retLen))
                    throw new Exception(GetLastErrorMessage());

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

                if (!GetTokenInformation(this, TOKEN_INFORMATION_CLASS.TokenSessionId,
                    out sessionId, 4, out retLen))
                    throw new Exception(GetLastErrorMessage());

                return sessionId;
            }

            /// <summary>
            /// Gets the token's source.
            /// </summary>
            /// <returns>A TOKEN_SOURCE struct.</returns>
            public TOKEN_SOURCE GetSource()
            {
                TOKEN_SOURCE source = new TOKEN_SOURCE();
                int retLen;

                if (!GetTokenInformation(this, TOKEN_INFORMATION_CLASS.TokenSource,
                    ref source, Marshal.SizeOf(source), out retLen))
                    throw new Exception(GetLastErrorMessage());

                return source;
            }

            /// <summary>
            /// Gets the token's user.
            /// </summary>
            /// <returns>A WindowsSID instance.</returns>
            public WindowsSID GetUser()
            {
                int retLen;

                GetTokenInformation(this, TOKEN_INFORMATION_CLASS.TokenUser, IntPtr.Zero, 0, out retLen);

                using (MemoryAlloc data = new MemoryAlloc(retLen))
                {
                    if (!GetTokenInformation(this.Handle, TOKEN_INFORMATION_CLASS.TokenUser, data,
                        data.Size, out retLen))
                        throw new Exception(Win32.GetLastErrorMessage());

                    TOKEN_USER user = data.ReadStruct<TOKEN_USER>();

                    return new WindowsSID(user.User.SID);
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

                if (!GetTokenInformation(this, TOKEN_INFORMATION_CLASS.TokenElevation,
                    out value, 4, out retLen))
                    throw new Exception(GetLastErrorMessage());

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

                if (!GetTokenInformation(this, TOKEN_INFORMATION_CLASS.TokenVirtualizationAllowed,
                    out value, 4, out retLen))
                    throw new Exception(GetLastErrorMessage());

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

                if (!GetTokenInformation(this, TOKEN_INFORMATION_CLASS.TokenVirtualizationEnabled,
                    out value, 4, out retLen))
                    throw new Exception(GetLastErrorMessage());

                return value != 0;
            }
        }
    }
}
