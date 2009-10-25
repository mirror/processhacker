using System;

namespace ProcessHacker.Native.Security
{
    [Flags]
    public enum TokenAccess : uint
    {
        AssignPrimary = 0x0001,
        Duplicate = 0x0002,
        Impersonate = 0x0004,
        Query = 0x0008,
        QuerySource = 0x0010,
        AdjustPrivileges = 0x0020,
        AdjustGroups = 0x0040,
        AdjustDefault = 0x0080,
        AdjustSessionId = 0x0100,
        All = StandardRights.Required | AssignPrimary | Duplicate | Impersonate |
            Query | QuerySource | AdjustPrivileges | AdjustGroups | AdjustDefault |
            AdjustSessionId,
        GenericRead = StandardRights.Read | Query,
        GenericWrite = StandardRights.Write | AdjustPrivileges | AdjustGroups | AdjustDefault,
        GenericExecute = StandardRights.Execute
    }
}
