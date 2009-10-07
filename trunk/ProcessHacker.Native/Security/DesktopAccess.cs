using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessHacker.Native.Security
{
    [Flags]
    public enum DesktopAccess : uint
    {
        ReadObjects = 0x0001,
        CreateWindow = 0x0002,
        CreateMenu = 0x0004,
        HookControl = 0x0008,
        JournalRecord = 0x0010,
        JournalPlayback = 0x0020,
        Enumerate = 0x0040,
        WriteObjects = 0x0080,
        SwitchDesktop = 0x0100,
        All = CreateMenu | CreateWindow | Enumerate | HookControl |
            JournalPlayback | JournalRecord | ReadObjects | SwitchDesktop |
            WriteObjects | StandardRights.Required,
        GenericRead = Enumerate | ReadObjects | StandardRights.Read,
        GenericWrite = CreateMenu | CreateWindow | HookControl | JournalPlayback |
            JournalRecord | WriteObjects | StandardRights.Write,
        GenericExecute = SwitchDesktop | StandardRights.Execute
    }
}
