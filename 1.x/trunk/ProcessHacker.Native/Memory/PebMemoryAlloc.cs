/*
 * Process Hacker - 
 *   PEB memory allocation
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
using System.ComponentModel;
using ProcessHacker.Native.Api;

namespace ProcessHacker.Native
{
    /// <summary>
    /// Represents a memory allocation from the PEB.
    /// </summary>
    public sealed class PebMemoryAlloc : MemoryAlloc
    {
        public PebMemoryAlloc(int size)
        {
            NtStatus status;
            IntPtr block;

            if ((status = Win32.RtlAllocateFromPeb(size, out block)) >= NtStatus.Error)
                Win32.Throw(status);

            this.Memory = block;
            this.Size = size;
        }

        protected override void Free()
        {
            NtStatus status;

            if ((status = Win32.RtlFreeToPeb(this, this.Size)) >= NtStatus.Error)
                Win32.Throw(status);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void Resize(int newSize)
        {
            throw new NotSupportedException();
        }
    }
}
