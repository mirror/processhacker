using System;

namespace ProcessHacker.Native.Security
{
    [Flags]
    public enum LsaAccountAccess : uint
    {
        View = 0x00000001,
        AdjustPrivileges = 0x00000002,
        AdjustQuotas = 0x00000004,
        AdjustSystemAccess = 0x00000008,
        All = StandardRights.Required | View | AdjustPrivileges | AdjustQuotas |
            AdjustSystemAccess,
        GenericRead = StandardRights.Read | View,
        GenericWrite = StandardRights.Write | AdjustPrivileges | AdjustQuotas |
            AdjustSystemAccess,
        GenericExecute = StandardRights.Execute
    }
}
