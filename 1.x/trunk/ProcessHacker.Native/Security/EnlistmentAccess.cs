using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessHacker.Native.Security
{
    [Flags]
    public enum EnlistmentAccess : uint
    {
        QueryInformation = 0x0001,
        SetInformation = 0x0002,
        Recover = 0x0004,
        SubordinateRights = 0x0008,
        SuperiorRights = 0x0010,
        GenericRead = StandardRights.Read | QueryInformation,
        GenericWrite = StandardRights.Write | SetInformation | Recover | 
            SubordinateRights | SuperiorRights,
        GenericExecute = StandardRights.Execute | Recover | SubordinateRights | 
            SuperiorRights,
        All = StandardRights.Required | GenericRead | GenericWrite | GenericExecute
    }
}
