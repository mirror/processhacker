using System;

namespace ProcessHacker.Native.Security
{
    [Flags]
    public enum SamUserAccess : uint
    {
        ReadGeneral = 0x1,
        ReadPreferences = 0x2,
        WritePreferences = 0x4,
        ReadLogon = 0x8,
        ReadAccount = 0x10,
        WriteAccount = 0x20,
        ChangePassword = 0x40,
        ForcePasswordChange = 0x80,
        ListGroups = 0x100,
        ReadGroupInformation = 0x200,
        WriteGroupInformation = 0x400,
        All = StandardRights.Required | ReadPreferences | ReadLogon |
            ListGroups | ReadGroupInformation | WritePreferences |
            ChangePassword | ForcePasswordChange | ReadGeneral |
            ReadAccount | WriteAccount | WriteGroupInformation,
        GenericRead = StandardRights.Read | ReadPreferences |
            ReadLogon | ReadAccount | ListGroups | ReadGroupInformation,
        GenericWrite = StandardRights.Write | WritePreferences |
            ChangePassword,
        GenericExecute = StandardRights.Execute | ReadGeneral |
            ChangePassword
    }
}
