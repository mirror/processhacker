using System;

namespace ProcessHacker.Native.Security
{
    [Flags]
    public enum TypeObjectAccess : uint
    {
        Create = 0x1,
        All = StandardRights.Required | Create
    }
}
