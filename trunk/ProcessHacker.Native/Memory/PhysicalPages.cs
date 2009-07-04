using System;
using ProcessHacker.Common.Objects;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;

namespace ProcessHacker.Native.Memory
{
    public sealed class PhysicalPages : BaseObject
    {
        private ProcessHandle _processHandle;
        private int _count;
        private IntPtr[] _pfnArray;

        public PhysicalPages(ProcessHandle processHandle, int count, bool pages)
        {
            if (pages)
                _count = count;
            else
                _count = (count - 1) / Windows.PageSize + 1;

            IntPtr pageCount = new IntPtr(_count);

            _pfnArray = new IntPtr[_count];

            if (!Win32.AllocateUserPhysicalPages(processHandle, ref pageCount, _pfnArray))
                Win32.ThrowLastError();

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
                Win32.ThrowLastError();

            if (freedPages.ToInt32() != _count)
                throw new Exception("Could not free all pages.");
        }

        public PhysicalPagesMapping Map(IntPtr address, MemoryProtection protection)
        {
            return new PhysicalPagesMapping(this, address, protection);
        }

        internal void Map(IntPtr address, MemoryProtection protection, bool unmap)
        {
            if (!unmap)
            {
                IntPtr allocAddress = Win32.VirtualAllocEx(
                    ProcessHandle.GetCurrent(),
                    address,
                    _count * Windows.PageSize,
                    MemoryState.Reserve | MemoryState.Physical,
                    protection
                    );

                if (!Win32.MapUserPhysicalPages(
                    allocAddress,
                    new IntPtr(_count),
                    _pfnArray
                    ))
                    Win32.ThrowLastError();
            }
            else
            {
                if (!Win32.MapUserPhysicalPages(
                    address,
                    new IntPtr(_count),
                    _pfnArray
                    ))
                    Win32.ThrowLastError();
            }
        }
    }
}
