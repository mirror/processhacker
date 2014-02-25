/*
 * Process Hacker - 
 *   run-time library heap
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
using ProcessHacker.Native.Api;

namespace ProcessHacker.Native
{
    public struct Heap
    {
        public static Heap FromHandle(IntPtr handle)
        {
            return new Heap(handle);
        }

        public static Heap GetDefault()
        {
            return new Heap(Win32.GetProcessHeap());
        }

        public static Heap[] GetHeaps()
        {
            IntPtr[] heapAddresses = new IntPtr[64];
            int retHeaps;

            retHeaps = Win32.RtlGetProcessHeaps(heapAddresses.Length, heapAddresses);

            // Reallocate the buffer if it wasn't large enough.
            if (retHeaps > heapAddresses.Length)
            {
                heapAddresses = new IntPtr[retHeaps];
                retHeaps = Win32.RtlGetProcessHeaps(heapAddresses.Length, heapAddresses);
            }

            int numberOfHeaps = Math.Min(heapAddresses.Length, retHeaps);
            Heap[] heaps = new Heap[numberOfHeaps];

            for (int i = 0; i < numberOfHeaps; i++)
                heaps[i] = new Heap(heapAddresses[i]);

            return heaps;
        }

        private IntPtr _heap;

        private Heap(IntPtr heap)
        {
            _heap = heap;
        }

        public Heap(HeapFlags flags)
            : this(flags, 0, 0)
        { }

        public Heap(HeapFlags flags, int reserveSize, int commitSize)
        {
            _heap = Win32.RtlCreateHeap(
                flags,
                IntPtr.Zero,
                reserveSize.ToIntPtr(),
                commitSize.ToIntPtr(),
                IntPtr.Zero,
                IntPtr.Zero
                );

            if (_heap == IntPtr.Zero)
                throw new OutOfMemoryException();
        }

        public IntPtr Address
        {
            get { return _heap; }
        }

        public IntPtr Allocate(HeapFlags flags, int size)
        {
            IntPtr memory = Win32.RtlAllocateHeap(_heap, flags, size.ToIntPtr());

            if (memory == IntPtr.Zero)
                throw new OutOfMemoryException();

            return memory;
        }

        public int Compact(HeapFlags flags)
        {
            return Win32.RtlCompactHeap(_heap, flags).ToInt32();
        }

        public void Destroy()
        {
            Win32.RtlDestroyHeap(_heap);
        }

        public void Free(HeapFlags flags, IntPtr memory)
        {
            Win32.RtlFreeHeap(_heap, flags, memory);
        }

        public int GetBlockSize(HeapFlags flags, IntPtr memory)
        {
            return Win32.RtlSizeHeap(_heap, flags, memory).ToInt32();
        }

        public IntPtr Reallocate(HeapFlags flags, IntPtr memory, int size)
        {
            IntPtr newMemory = Win32.RtlReAllocateHeap(_heap, flags, memory, size.ToIntPtr());

            if (newMemory == IntPtr.Zero)
                throw new OutOfMemoryException();

            return newMemory;
        }
    }
}
