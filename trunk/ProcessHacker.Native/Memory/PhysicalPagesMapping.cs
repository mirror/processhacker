using System;
using ProcessHacker.Native.Api;

namespace ProcessHacker.Native.Memory
{
    public sealed class PhysicalPagesMapping : MemoryAlloc
    {
        private PhysicalPages _physicalPages;

        internal PhysicalPagesMapping(PhysicalPages physicalPages, IntPtr address, MemoryProtection protection)
        {
            _physicalPages = physicalPages;
            _physicalPages.Reference();
            _physicalPages.Map(address, protection, false);
        }

        protected override void Free()
        {
            _physicalPages.Map(this, 0, true);
            _physicalPages.Dereference();
        }
    }
}
