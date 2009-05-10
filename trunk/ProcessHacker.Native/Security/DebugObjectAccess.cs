using System;

namespace ProcessHacker.Native.Security
{
    [Flags]
    public enum DebugObjectAccess : uint
    {
        ReadEvent = 0x1,
        ProcessAssign = 0x2,
        SetInformation = 0x4,
        QueryInformation = 0x8,
        All = StandardRights.Required | StandardRights.Synchronize | 
            ReadEvent | ProcessAssign | SetInformation | QueryInformation
    }
}
