using System;
using System.Collections.Generic;
using System.Text;
using ProcessHacker.Native.Api;

namespace ProcessHacker.Native
{
    /// <summary>
    /// Represents a LocalAlloc() memory allocation.
    /// </summary>
    public class LocalMemoryAlloc : MemoryAlloc
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

            base.Size = size;
        }

        protected override void Free()
        {
            Win32.LocalFree(this);
        }

        public override void Resize(int newSize)
        {
            throw new NotImplementedException();
        }
    }
}
