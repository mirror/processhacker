using System;
using ProcessHacker.Native.Api;

namespace ProcessHacker.Native
{
    /// <summary>
    /// Represents a LocalAlloc() memory allocation.
    /// </summary>
    public sealed class LocalMemoryAlloc : MemoryAlloc
    {
        public LocalMemoryAlloc(IntPtr memory)
            : this(memory, true)
        { }

        public LocalMemoryAlloc(IntPtr memory, bool owned)
            : base(memory, owned)
        { }

        public LocalMemoryAlloc(int size)
            : this(size, AllocFlags.LPtr)
        { }

        public LocalMemoryAlloc(int size, AllocFlags flags)
        {
            this.Memory = Win32.LocalAlloc(flags, size);

            if (this.Memory == IntPtr.Zero)
                throw new OutOfMemoryException();

            this.Size = size;
        }

        protected override void Free()
        {
            Win32.LocalFree(this);
        }

        public override void Resize(int newSize)
        {
            IntPtr newMemory;

            newMemory = Win32.LocalReAlloc(this, AllocFlags.LMemFixed, newSize);

            if (newMemory == IntPtr.Zero)
                throw new OutOfMemoryException();

            this.Memory = newMemory;
            this.Size = newSize;
        }
    }
}
