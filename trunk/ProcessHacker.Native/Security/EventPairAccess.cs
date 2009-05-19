using System;

namespace ProcessHacker.Native.Security
{
    [Flags]
    public enum EventPairAccess : uint
    {
        All = StandardRights.Required | StandardRights.Synchronize
    }
}
