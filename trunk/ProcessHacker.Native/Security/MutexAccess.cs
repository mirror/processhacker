using System;

namespace ProcessHacker.Native.Security
{
    [Flags]
    public enum MutexAccess : uint
    {
        ModifyState = 0x1,
        All = 0x1f0001
    }
}
