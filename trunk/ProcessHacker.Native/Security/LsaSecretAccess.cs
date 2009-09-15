using System;

namespace ProcessHacker.Native.Security
{
    [Flags]
    public enum LsaSecretAccess : uint
    {
        SetValue = 0x00000001,
        QueryValue = 0x00000002,
        All = StandardRights.Required | SetValue | QueryValue,
        GenericRead = StandardRights.Read | QueryValue,
        GenericWrite = StandardRights.Write | SetValue,
        GenericExecute = StandardRights.Execute
    }
}
