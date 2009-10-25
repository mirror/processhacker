/*
 * Process Hacker - 
 *   remote token handle
 * 
 * Copyright (C) 2008 wj32
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

using ProcessHacker.Native.Api;
using ProcessHacker.Native.Security;
using System;

namespace ProcessHacker.Native.Objects
{
    /// <summary>
    /// Represents a token handle owned by another process.
    /// </summary>
    /// <remarks>
    /// This is a wrapper class so that we can view information 
    /// about tokens other processes have handles to. TokenProperties 
    /// only takes an IWithToken object.
    /// </remarks>
    public sealed class RemoteTokenHandle : RemoteHandle, IWithToken
    {
        public RemoteTokenHandle(ProcessHandle phandle, IntPtr handle)
            : base(phandle, handle)
        { }

        public new IntPtr GetHandle(int rights)
        {
            IntPtr newHandle = IntPtr.Zero;

            // We can use KPH here. RemoteHandle doesn't.
            Win32.DuplicateObject(this.ProcessHandle, this.Handle, new IntPtr(-1), out newHandle, rights, 0, 0);

            return newHandle;
        }

        public TokenHandle GetToken()
        {
            return GetToken(TokenAccess.All);
        }

        public TokenHandle GetToken(TokenAccess access)
        {
            return new TokenHandle(this.GetHandle((int)access), true);
        }
    }
}
