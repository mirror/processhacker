using System;

namespace ProcessHacker.Native.Security
{
    [Flags]
    public enum LsaTrustedAccess : uint
    {
        QueryDomainName = 0x00000001,
        QueryControllers = 0x00000002,
        SetControllers = 0x00000004,
        QueryPosix = 0x00000008,
        SetPosix = 0x00000010,
        SetAuth = 0x00000020,
        QueryAuth = 0x00000040,
        All = StandardRights.Required | QueryDomainName | QueryControllers |
            SetControllers | QueryPosix | SetPosix | SetAuth | QueryAuth,
        GenericRead = StandardRights.Read | QueryDomainName,
        GenericWrite = StandardRights.Write | SetControllers | SetPosix |
            SetAuth,
        GenericExecute = StandardRights.Execute | QueryControllers | QueryPosix
    }
}
