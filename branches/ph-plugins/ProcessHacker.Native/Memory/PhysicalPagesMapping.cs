using System;
using ProcessHacker.Native.Api;

namespace ProcessHacker.Native.Memory
{
    public sealed class PhysicalPagesMapping : MemoryAlloc
    {
        private PhysicalPages _physicalPages;

        internal PhysicalPagesMapping(PhysicalPages physicalPages, IntPtr baseAddress)
        {
            _physicalPages = physicalPages;
            _physicalPages.Reference();
            this.Memory = baseAddress;
        }

        protected override void Free()
        {
            _physicalPages.Unmap(this);
            _physicalPages.Dereference();
        }
    }
}
