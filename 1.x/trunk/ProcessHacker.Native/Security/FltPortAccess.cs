using System;

namespace ProcessHacker.Native.Security
{
    [Flags]
    public enum FltPortAccess : uint
    {
        Connect = 0x1,
        All = Connect | StandardRights.All
    }
}
