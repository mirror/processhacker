using System;

namespace ProcessHacker.Native.Security
{
    [Flags]
    public enum DirectoryAccess : uint
    {
        Query = 0x1,
        Traverse = 0x2,
        CreateObject = 0x4,
        CreateSubdirectory = 0x8,
        All = StandardRights.Required | Query | Traverse |
            CreateObject | CreateSubdirectory
    }
}
