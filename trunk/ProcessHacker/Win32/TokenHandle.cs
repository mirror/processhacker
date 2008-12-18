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

namespace ProcessHacker
{
    public partial class Win32
    {
        /// <summary>
        /// Represents a handle to a Windows token.
        /// </summary>
        public class TokenHandle : Win32Handle
        {
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

            private TokenHandle(int Handle, bool Owned)
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

                if (OpenProcessToken(handle.Handle, access, out h) == 0)
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

                if (OpenThreadToken(handle.Handle, access, false, out h) == 0)
                    throw new Exception(GetLastErrorMessage());

                this.Handle = h;
            }

            /// <summary>
            /// Gets the token's username.
            /// </summary>
            /// <param name="IncludeDomain">Specifies whether to include the domain of the user.</param>
            /// <returns>The token's username.</returns>
            public string GetUsername(bool IncludeDomain)
            {
                int retLen = 0;

                GetTokenInformation(this.Handle, TOKEN_INFORMATION_CLASS.TokenUser, 0, 0, ref retLen);

                IntPtr data = Marshal.AllocHGlobal(retLen);

                try
                {
                    if (GetTokenInformation(this.Handle, TOKEN_INFORMATION_CLASS.TokenUser, data,
                        retLen, ref retLen) == 0)
                    {
                        throw new Exception(Win32.GetLastErrorMessage());
                    }

                    TOKEN_USER user = PtrToStructure<TOKEN_USER>(data);

                    return GetAccountName(user.User.SID, IncludeDomain);
                }
                finally
                {
                    Marshal.FreeHGlobal(data);
                }
            }
        }
    }
}
