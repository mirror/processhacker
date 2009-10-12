using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessHacker.Native.Security
{
    [Flags]
    public enum TransactionAccess : uint
    {
        QueryInformation = 0x0001,
        SetInformation = 0x0002,
        Enlist = 0x0004,
        Commit = 0x0008,
        Rollback = 0x0010,
        Propagate = 0x0020,
        RightReserved1 = 0x0040,
        GenericRead = StandardRights.Read | QueryInformation | StandardRights.Synchronize,
        GenericWrite = StandardRights.Write | SetInformation | Commit | Enlist | Rollback | 
            Propagate | StandardRights.Synchronize,
        GenericExecute = StandardRights.Execute | Commit | Rollback | StandardRights.Synchronize,
        All = StandardRights.Required | GenericRead | GenericWrite | GenericExecute,

        ResourceManagerRights = GenericRead | StandardRights.Write | SetInformation | 
            Enlist | Rollback | Propagate | StandardRights.Synchronize
    }
}
