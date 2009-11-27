using System;

namespace ProcessHacker.Native.Security
{
    [Flags]
    public enum SamServerAccess : uint
    {
        Connect = 0x1,
        Shutdown = 0x2,
        Initialize = 0x4,
        CreateDomain = 0x8,
        EnumerateDomains = 0x10,
        LookupDomain = 0x20,
        All = StandardRights.Required | Connect | Initialize | CreateDomain |
            Shutdown | EnumerateDomains | LookupDomain,
        GenericRead = StandardRights.Read | EnumerateDomains,
        GenericWrite = StandardRights.Write | Initialize | CreateDomain | Shutdown,
        GenericExecute = StandardRights.Execute | Connect | LookupDomain
    }
}
