/*
 * Process Hacker - 
 *   terminal server memory allocation wrapper
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

namespace ProcessHacker.Native
{
    /// <summary>
    /// Represents a memory allocation managed by the Terminal Server API.
    /// </summary>
    public class WtsMemoryAlloc : MemoryAlloc
    {
        /// <summary>
        /// Creates a memory allocation from an existing Terminal Server managed allocation. 
        /// </summary>
        /// <param name="memory">A pointer to the allocated memory.</param>
        /// <returns>A new memory allocation object.</returns>
        public static new WtsMemoryAlloc FromPointer(IntPtr memory)
        {
            return new WtsMemoryAlloc() { Memory = memory };
        }

        private WtsMemoryAlloc()
        { }

        public override void Resize(int newSize)
        {
            throw new NotSupportedException();
        }

        public override int Size
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        protected override void Free()
        {
            Win32.WTSFreeMemory(this);
        }
    }
}
