using System;

namespace ProcessHacker.Native.Security
{
    [Flags]
    public enum StandardRights : uint
    {
        Delete = 0x00010000,
        ReadControl = 0x00020000,
        WriteDac = 0x00040000,
        WriteOwner = 0x00080000,
        Synchronize = 0x00100000,
        Required = 0x000f0000,
        Read = ReadControl,
        Write = ReadControl,
        Execute = ReadControl,
        All = 0x001f0000,

        SpecificRightsAll = 0x0000ffff,
        AccessSystemSecurity = 0x01000000,
        MaximumAllowed = 0x02000000,
        GenericRead = 0x80000000,
        GenericWrite = 0x40000000,
        GenericExecute = 0x20000000,
        GenericAll = 0x10000000
    }
}
