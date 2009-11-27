using System;

namespace ProcessHacker.Native.Security
{
    [Flags]
    public enum SamDomainAccess : uint
    {
        ReadPasswordParameters = 0x1,
        WritePasswordParameters = 0x2,
        ReadOtherParameters = 0x4,
        WriteOtherParameters = 0x8,
        CreateUser = 0x10,
        CreateGroup = 0x20,
        CreateAlias = 0x40,
        GetAliasMembership = 0x80,
        ListAccounts = 0x100,
        Lookup = 0x200,
        AdministerServer = 0x400,
        All = StandardRights.Required | ReadOtherParameters | WriteOtherParameters |
            WritePasswordParameters | CreateUser | CreateGroup | CreateAlias |
            GetAliasMembership | ListAccounts | ReadPasswordParameters |
            Lookup | AdministerServer,
        GenericRead = StandardRights.Read | GetAliasMembership | ReadOtherParameters,
        GenericWrite = StandardRights.Write | WriteOtherParameters | WritePasswordParameters |
            CreateUser | CreateGroup | CreateAlias | AdministerServer,
        GenericExecute = StandardRights.Execute | ReadPasswordParameters | ListAccounts |
            Lookup
    }
}
