using System;

namespace ProcessHacker.Native.Security
{
    [Flags]
    public enum ServiceAccess : uint
    {
        QueryConfig = 0x0001,
        ChangeConfig = 0x0002,
        QueryStatus = 0x0004,
        EnumerateDependents = 0x0008,
        Start = 0x0010,
        Stop = 0x0020,
        PauseContinue = 0x0040,
        Interrogate = 0x0080,
        UserDefinedControl = 0x0100,
        All = StandardRights.Required | QueryConfig | ChangeConfig | QueryStatus | 
            EnumerateDependents | Start | Stop | PauseContinue | Interrogate | UserDefinedControl
    }
}
