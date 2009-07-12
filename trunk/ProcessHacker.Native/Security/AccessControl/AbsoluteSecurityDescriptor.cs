using System;
using ProcessHacker.Common.Objects;
using ProcessHacker.Native.Api;

namespace ProcessHacker.Native.Security.AccessControl
{
    public sealed class AbsoluteSecurityDescriptor : SecurityDescriptor
    {
        private MemoryAlloc _memory;

        public AbsoluteSecurityDescriptor()
        {
            NtStatus status;

            this.Memory = _memory = new MemoryAlloc(Win32.SecurityDescriptorMinLength);

            if ((status = Win32.RtlCreateSecurityDescriptor(
                this.Memory,
                Win32.SecurityDescriptorRevision
                )) >= NtStatus.Error)
            {
                // Free the allocated memory and disable ownership.
                _memory.Dispose();
                this.DisableOwnership(false);
            }
        }

        public AbsoluteSecurityDescriptor(IntPtr memory)
        {
            this.Memory = _memory = new MemoryAlloc(memory, false);
            this.Read();
        }

        protected override void DisposeObject(bool disposing)
        {
            _memory.Dispose(disposing);
            base.DisableOwnership(disposing);
        }
    }
}
