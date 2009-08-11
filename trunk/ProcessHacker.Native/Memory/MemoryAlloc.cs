/*
 * Process Hacker - 
 *   memory allocation wrapper
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
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using ProcessHacker.Common.Objects;
using ProcessHacker.Native.Api;

namespace ProcessHacker.Native
{
    /// <summary>
    /// Represents an unmanaged memory allocation from the heap.
    /// </summary>
    public class MemoryAlloc : MemoryRegion
    {
        // A private heap just for the client.
        private static Heap _privateHeap = new Heap(HeapFlags.Class1 | HeapFlags.Growable);
        private static Heap _processHeap = Heap.GetDefault();

        public static IntPtr PrivateHeap
        {
            get { return _privateHeap.Address; }
        }

        /// <summary>
        /// Creates a new, invalid memory allocation. 
        /// You must set the pointer using the Memory property.
        /// </summary>
        protected MemoryAlloc()
            : base()
        { }

        public MemoryAlloc(IntPtr memory)
            : this(memory, true)
        { }

        public MemoryAlloc(IntPtr memory, bool owned)
            : this(memory, 0, owned)
        { }

        public MemoryAlloc(IntPtr memory, int size, bool owned)
            : base(memory, size, owned)
        { }

        /// <summary>
        /// Creates a new memory allocation with the specified size.
        /// </summary>
        /// <param name="size">The amount of memory, in bytes, to allocate.</param>
        public MemoryAlloc(int size)
        {
            this.Memory = _privateHeap.Allocate(0, size);
            this.Size = size;
        }

        /// <summary>
        /// Creates a new memory allocation with the specified size.
        /// </summary>
        /// <param name="size">The amount of memory, in bytes, to allocate.</param>
        /// <param name="flags">Any flags to use.</param>
        public MemoryAlloc(int size, HeapFlags flags)
        {
            this.Memory = _privateHeap.Allocate(flags, size);
            this.Size = size;
        }

        protected override void Free()
        {
            _privateHeap.Free(0, this);
        }

        /// <summary>
        /// Resizes the memory allocation.
        /// </summary>
        /// <param name="newSize">The new size of the allocation.</param>
        public virtual void Resize(int newSize)
        {
            this.Memory = _privateHeap.Reallocate(0, this.Memory, newSize);
            this.Size = newSize;
        }
    }
}
