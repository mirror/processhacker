using System;

namespace ProcessHacker.Native.Security
{
    [Flags]
    public enum ThreadAccess : uint
    {
        Terminate = 0x0001,
        SuspendResume = 0x0002,
        Alert = 0x0004,
        GetContext = 0x0008,
        SetContext = 0x0010,
        SetInformation = 0x0020,
        QueryInformation = 0x0040,
        SetThreadToken = 0x0080,
        Impersonate = 0x0100,
        DirectImpersonation = 0x0200,
        SetLimitedInformation = 0x0400,
        QueryLimitedInformation = 0x0800,
        // should be 0xfff on Vista, but is 0x3ff for backwards compatibility
        All = StandardRights.Required | StandardRights.Synchronize | 0x3ff
    }
}
