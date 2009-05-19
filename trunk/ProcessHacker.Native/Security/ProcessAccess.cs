using System;

namespace ProcessHacker.Native.Security
{
    [Flags]
    public enum ProcessAccess : uint
    {
        Terminate = 0x0001,
        CreateThread = 0x0002,
        SetSessionId = 0x0004,
        VmOperation = 0x0008,
        VmRead = 0x0010,
        VmWrite = 0x0020,
        DupHandle = 0x0040,
        CreateProcess = 0x0080,
        SetQuota = 0x0100,
        SetInformation = 0x0200,
        QueryInformation = 0x0400,
        SetPort = 0x0800,
        SuspendResume = 0x0800,
        QueryLimitedInformation = 0x1000,
        // should be 0xffff on Vista, but is 0xfff for backwards compatibility
        All = StandardRights.Required | StandardRights.Synchronize | 0xfff
    }
}
