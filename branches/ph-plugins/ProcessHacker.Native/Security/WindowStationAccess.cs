using System;

namespace ProcessHacker.Native.Security
{
    [Flags]
    public enum WindowStationAccess : uint
    {
        EnumDesktops = 0x0001,
        ReadAttributes = 0x0002,
        AccessClipboard = 0x0004,
        CreateDesktop = 0x0008,
        WriteAttributes = 0x0010,
        AccessGlobalAtoms = 0x0020,
        ExitWindows = 0x0040,
        Enumerate = 0x0100,
        ReadScreen = 0x0200,
        All = StandardRights.Required | AccessClipboard | 
            AccessGlobalAtoms | CreateDesktop | EnumDesktops | Enumerate | 
            ExitWindows | ReadAttributes | ReadScreen | WriteAttributes,
        GenericRead = StandardRights.Read | EnumDesktops | Enumerate | 
            ReadAttributes | ReadScreen,
        GenericWrite = StandardRights.Write | AccessClipboard | 
            CreateDesktop | WriteAttributes,
        GenericExecute = StandardRights.Execute | AccessGlobalAtoms | 
            ExitWindows
    }
}
