using System;

namespace ProcessHacker.Native.Security
{
    [Flags]
    public enum PortAccess : uint
    {
        Connect = 0x1,
        All = StandardRights.Required | StandardRights.Synchronize |
            Connect
    }
}
