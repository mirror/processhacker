using System;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;

namespace ProcessHacker.Native
{
    public class VirtualMemoryAlloc : MemoryAlloc
    {
        public VirtualMemoryAlloc(int size)
        {
            this.Memory = ProcessHandle.Current.AllocateMemory(size, MemoryProtection.ReadWrite);
            this.Size = size;
        }

        protected override void Free()
        {
            ProcessHandle.Current.FreeMemory(this, this.Size);
        }

        public override void Resize(int newSize)
        {
            throw new NotSupportedException();
        }
    }
}
