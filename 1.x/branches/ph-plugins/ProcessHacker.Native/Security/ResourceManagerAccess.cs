using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessHacker.Native.Security
{
    [Flags]
    public enum ResourceManagerAccess : uint
    {
        QueryInformation = 0x0001,
        SetInformation = 0x0002,
        Recover = 0x0004,
        Enlist = 0x0008,
        GetNotification = 0x0010,
        RegisterProtocol = 0x0020,
        CompletePropagation = 0x0040,
        GenericRead = StandardRights.Read | QueryInformation | StandardRights.Synchronize,
        GenericWrite = StandardRights.Write | SetInformation | Recover | Enlist | 
            GetNotification | RegisterProtocol | CompletePropagation | StandardRights.Synchronize,
        GenericExecute = StandardRights.Execute | Recover | Enlist | GetNotification | 
            CompletePropagation | StandardRights.Synchronize,
        All = StandardRights.Required | GenericRead | GenericWrite | GenericExecute
    }
}
