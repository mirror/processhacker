using System;

namespace ProcessHacker.Native.Security
{
    [Flags]
    public enum KeyedEventAccess : uint
    {
        Wait = 0x1,
        Wake = 0x2,
        All = StandardRights.Required | Wait | Wake
    }
}
