using System;

namespace ProcessHacker.Native.Security
{
    [Flags]
    public enum TmAccess : uint
    {
        QueryInformation = 0x0001,
        SetInformation = 0x0002,
        Recover = 0x0004,
        Rename = 0x0008,
        CreateRm = 0x0010,
        // About to be deprecated - for DTC use only.
        BindTransaction = 0x0020,
        GenericRead = StandardRights.Read | QueryInformation,
        GenericWrite = StandardRights.Write | SetInformation | Recover | Rename | CreateRm,
        GenericExecute = StandardRights.Execute,
        All = StandardRights.Required | GenericRead | GenericWrite | 
            GenericExecute | BindTransaction,
    }
}
