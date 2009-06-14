/*
 * Process Hacker - 
 *   heap memory allocation wrapper
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

using System;
using System.Runtime.InteropServices;
using System.Text;
using ProcessHacker.Native.Api;

namespace ProcessHacker.Native
{
    /// <summary>
    /// Represents a heap memory allocation.
    /// </summary>
    public class HeapMemoryAlloc : MemoryAlloc
    {
        public HeapMemoryAlloc(int size)
        {
            this.Memory = Win32.HeapAlloc(Win32.GetProcessHeap(), 0, size);

            if (this.Memory == IntPtr.Zero)
                throw new OutOfMemoryException();

            base.Size = size;
        }

        protected override void Free()
        {
            Win32.HeapFree(Win32.GetProcessHeap(), 0, this);
        }

        public override void Resize(int newSize)
        {
            IntPtr newMemory = IntPtr.Zero;

            newMemory = Win32.HeapReAlloc(Win32.GetProcessHeap(), 0, this.Memory, newSize);

            if (newMemory == IntPtr.Zero)
                throw new OutOfMemoryException();

            this.Memory = newMemory;
            base.Size = newSize;
        }
    }
}
