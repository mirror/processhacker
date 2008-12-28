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
        /// Represents a token handle owned by another process.
        /// </summary>
        /// <remarks>
        /// This is a wrapper class so that we can view information 
        /// about tokens other processes have handles to. TokenProperties 
        /// only takes an IWithToken object.
        /// </remarks>
        public class RemoteTokenHandle : RemoteHandle, IWithToken
        {
            public RemoteTokenHandle(ProcessHandle phandle, int handle)
                : base(phandle, handle)
            { }

            public TokenHandle GetToken()
            {
                return GetToken(TOKEN_RIGHTS.TOKEN_ALL_ACCESS);
            }

            public TokenHandle GetToken(Win32.TOKEN_RIGHTS access)
            {
                return new TokenHandle(this.GetHandle((int)access), true);
            }
        }
    }
}
