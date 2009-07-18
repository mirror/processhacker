using System;
using ProcessHacker.Native.Api;

namespace ProcessHacker.Native
{
    public sealed class Heap
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
