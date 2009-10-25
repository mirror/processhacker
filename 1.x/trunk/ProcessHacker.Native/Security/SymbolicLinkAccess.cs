using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessHacker.Native.Security
{
    [Flags]
    public enum SymbolicLinkAccess : uint
    {
        Query = 0x1,
        All = StandardRights.Required | Query
    }
}
