using System;

namespace ProcessHacker.Native.Security
{
    [Flags]
    public enum JobObjectAccess : uint
    {
        AssignProcess = 0x0001,
        SetAttributes = 0x0002,
        Query = 0x0004,
        Terminate = 0x0008,
        SetSecurityAttributes = 0x0010,
        All = StandardRights.Required | StandardRights.Synchronize | 0x1f
    }
}
