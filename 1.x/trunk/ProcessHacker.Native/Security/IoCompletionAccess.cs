using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessHacker.Native.Security
{
    [Flags]
    public enum IoCompletionAccess : uint
    {
        QueryState = 0x1,
        ModifyState = 0x2,
        All = StandardRights.Required | StandardRights.Synchronize | 0x3
    }
}
