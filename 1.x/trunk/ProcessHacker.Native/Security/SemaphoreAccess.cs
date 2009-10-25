using System;

namespace ProcessHacker.Native.Security
{
    [Flags]
    public enum SemaphoreAccess : uint
    {
        QueryState = 0x1,
        ModifyState = 0x2,
        All = StandardRights.Required | StandardRights.Synchronize |
            QueryState | ModifyState
    }
}
