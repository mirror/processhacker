using System;

namespace ProcessHacker.Native.Security
{
    [Flags]
    public enum SemaphoreAccess : uint
    {
        ModifyState = 0x2,
        All = 0x1f0003
    }
}
