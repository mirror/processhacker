using System;
using System.Collections.Generic;
using System.Text;
using ProcessHacker.Common;

namespace ProcessHacker.Native
{
    public class AlignedMemoryAlloc : MemoryAlloc
    {
        private IntPtr _realMemory;

        public AlignedMemoryAlloc(int size, int alignment)
        {
            // Make sure the alignment is positive and a power of two.
            if (alignment <= 0 || Utils.CountBits(alignment) != 1)
                throw new ArgumentOutOfRangeException("alignment");

            // Since we are going to align our pointer, we need to account for 
            // any padding at the beginning.
            _realMemory = MemoryAlloc.PrivateHeap.Allocate(0, size + alignment - 1);

            // aligned memory = (memory + alignment - 1) & ~(alignment - 1)
            this.Memory = _realMemory.Align(alignment);
            this.Size = size;
        }

        protected override void Free()
        {
            MemoryAlloc.PrivateHeap.Free(0, _realMemory);
        }

        public override void Resize(int newSize)
        {
            throw new NotSupportedException();
        }
    }
}
