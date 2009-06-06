using System;

namespace ProcessHacker.Native.Security
{
    [Flags]
    public enum KeyAccess : uint
    {
        QueryValue = 0x0001,
        SetValue = 0x0002,
        CreateSubKey = 0x0004,
        EnumerateSubKeys = 0x0008,
        Notify = 0x0010,
        CreateLink = 0x0020,
        Wow64_32Key = 0x0200,
        Wow64_64Key = 0x0100,
        Wow64_Res = 0x0300,
        Read = (StandardRights.Read | QueryValue | EnumerateSubKeys | Notify) & ~StandardRights.Synchronize,
        Write = (StandardRights.Write | SetValue | CreateSubKey) & ~StandardRights.Synchronize,
        Execute = Read & ~StandardRights.Synchronize,
        All = (StandardRights.All | QueryValue | SetValue | CreateSubKey | 
            EnumerateSubKeys | Notify | CreateLink) & ~StandardRights.Synchronize
    }
}
