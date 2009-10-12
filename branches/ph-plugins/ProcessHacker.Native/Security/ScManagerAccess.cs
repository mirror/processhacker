using System;

namespace ProcessHacker.Native.Security
{
    [Flags]
    public enum ScManagerAccess : uint
    {
        Connect = 0x0001,
        CreateService = 0x0002,
        EnumerateService = 0x0004,
        Lock = 0x0008,
        QueryLockStatus = 0x0010,
        ModifyBootConfig = 0x0020,
        All = StandardRights.Required | Connect | CreateService | EnumerateService |
            Lock | QueryLockStatus | ModifyBootConfig
    }
}
