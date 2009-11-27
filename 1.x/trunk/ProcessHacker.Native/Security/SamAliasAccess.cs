using System;

namespace ProcessHacker.Native.Security
{
    [Flags]
    public enum SamAliasAccess : uint
    {
        AddMember = 0x1,
        RemoveMember = 0x2,
        ListMembers = 0x4,
        ReadInformation = 0x8,
        WriteAccount = 0x10,
        All = StandardRights.Required | ReadInformation | WriteAccount |
            ListMembers | AddMember | RemoveMember,
        GenericRead = StandardRights.Read | ListMembers,
        GenericWrite = StandardRights.Write | WriteAccount | AddMember |
            RemoveMember,
        GenericExecute = StandardRights.Execute | ReadInformation
    }
}
