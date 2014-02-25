using System;
using ProcessHacker.Common.Objects;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;

namespace ProcessHacker.Native.Memory
{
    /// <summary>
    /// Represents an allocation of physical pages.
    /// </summary>
    public sealed class PhysicalPages : BaseObject
    {
        private ProcessHandle _processHandle;
        private int _count;
        private IntPtr[] _pfnArray;

        /// <summary>
        /// Allocates physical pages.
        /// </summary>
        /// <param name="pageCount">The number of pages to allocate.</param>
        public PhysicalPages(int pageCount)
            : this(pageCount, true)
        { }

        /// <summary>
        /// Allocates physical pages.
        /// </summary>
        /// <param name="count">
        /// The number of bytes to allocate, or the number of pages to allocate 
        /// if <paramref name="pages" /> is true. If a number of bytes is used, 
        /// it will be rounded up to the system page size.</param>
        /// <param name="pages">
        /// Whether <paramref name="count" /> specifies bytes or pages.
        /// </param>
        public PhysicalPages(int count, bool pages)
            : this(ProcessHandle.Current, count, pages)
        { }

        /// <summary>
        /// Allocates physical pages.
        /// </summary>
        /// <param name="processHandle">The process to allocate the pages in.</param>
        /// <param name="pageCount">The number of pages to allocate.</param>
        public PhysicalPages(ProcessHandle processHandle, int pageCount)
            : this(processHandle, pageCount, true)
        { }

        /// <summary>
        /// Allocates physical pages.
        /// </summary>
        /// <param name="processHandle">The process to allocate the pages in.</param>
        /// <param name="count">
        /// The number of bytes to allocate, or the number of pages to allocate 
        /// if <paramref name="pages" /> is true. If a number of bytes is used, 
        /// it will be rounded up to the system page size.</param>
        /// <param name="pages">
        /// Whether <paramref name="count" /> specifies bytes or pages.
        /// </param>
        public PhysicalPages(ProcessHandle processHandle, int count, bool pages)
        {
            if (pages)
                _count = count;
            else
                _count = Windows.BytesToPages(count);

            IntPtr pageCount = new IntPtr(_count);

            _pfnArray = new IntPtr[_count];

            if (!Win32.AllocateUserPhysicalPages(processHandle, ref pageCount, _pfnArray))
                Win32.Throw();

            if (pageCount.ToInt32() != _count)
                throw new Exception("Could not allocate all pages.");

            _processHandle = processHandle;
            _processHandle.Reference();
        }

        protected override void DisposeObject(bool disposing)
        {
            IntPtr freedPages = new IntPtr(_count);

            _processHandle.Dereference();

            if (!Win32.FreeUserPhysicalPages(_processHandle, ref freedPages, _pfnArray))
                Win32.Throw();

            if (freedPages.ToInt32() != _count)
                throw new Exception("Could not free all pages.");
        }

        public PhysicalPagesMapping Map(MemoryProtection protection)
        {
            return this.Map(IntPtr.Zero, protection);
        }

        public PhysicalPagesMapping Map(IntPtr address, MemoryProtection protection)
        {
            // Reserve an address range.
            IntPtr allocAddress = ProcessHandle.Current.AllocateMemory(
                address,
                _count * Windows.PageSize,
                MemoryFlags.Reserve | MemoryFlags.Physical,
                protection
                );

            // Map the physical memory into the address range.
            if (!Win32.MapUserPhysicalPages(
                allocAddress,
                new IntPtr(_count),
                _pfnArray
                ))
                Win32.Throw();

            return new PhysicalPagesMapping(this, allocAddress);
        }

        internal void Unmap(IntPtr address)
        {
            // Unmap the physical memory from the address range.
            if (!Win32.MapUserPhysicalPages(
                 address,
                 new IntPtr(_count),
                 null
                 ))
                Win32.Throw();

            ProcessHandle.Current.FreeMemory(address, 0, false);
        }
    }
}
