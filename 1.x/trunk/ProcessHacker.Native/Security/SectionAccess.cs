using System;

namespace ProcessHacker.Native.Security
{
    [Flags]
    public enum SectionAccess : uint
    {
        Query = 0x0001,
        MapWrite = 0x0002,
        MapRead = 0x0004,
        MapExecute = 0x0008,
        ExtendSize = 0x0010,
        MapExecuteExplicit = 0x0020,
        All = StandardRights.Required | Query | MapWrite | MapRead | MapExecute | ExtendSize
    }
}
