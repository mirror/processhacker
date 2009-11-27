using System;

namespace ProcessHacker.Native.Security
{
    [Flags]
    public enum SamGroupAccess : uint
    {
        ReadInformation = 0x1,
        WriteAccount = 0x2,
        AddMember = 0x4,
        RemoveMember = 0x8,
        ListMembers = 0x10,
        All = StandardRights.Required | ListMembers | WriteAccount |
            AddMember | RemoveMember | ReadInformation,
        GenericRead = StandardRights.Read | ListMembers,
        GenericWrite = StandardRights.Write | WriteAccount | AddMember |
            RemoveMember,
        GenericExecute = StandardRights.Execute | ReadInformation
    }
}
