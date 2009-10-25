using System;

namespace ProcessHacker.Native.Security
{
    [Flags]
    public enum ObjectTypeAccess : uint
    {
        Create = 0x1,
        All = StandardRights.Required | Create
    }
}
