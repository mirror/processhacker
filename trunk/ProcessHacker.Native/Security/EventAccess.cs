using System;

namespace ProcessHacker.Native.Security
{
    [Flags]
    public enum EventAccess : uint
    {
        ModifyState = 0x2,
        All = 0x1f0003
    }
}
