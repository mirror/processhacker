using System;

namespace ProcessHacker.Native.Security
{
    [Flags]
    public enum MutantAccess : uint
    {
        QueryState = 0x1,
        All = StandardRights.Required | StandardRights.Synchronize | 
            QueryState
    }
}
