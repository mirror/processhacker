using System;

namespace ProcessHacker.Native.Security
{
    [Flags]
    public enum ProfileAccess : uint
    {
        Control = 0x1,
        All = StandardRights.Required | Control
    }
}
