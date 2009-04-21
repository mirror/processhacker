using System;

namespace ProcessHacker.Native.Security
{
    [Flags]
    public enum FileAccess : uint
    {
        ReadData = 0x0001,
        ListDirectory = 0x0001,
        WriteData = 0x0002,
        AddFile = 0x0002,
        AppendData = 0x0004,
        AddSubdirectory = 0x0004,
        CreatePipeInstance = 0x0004,
        ReadEa = 0x0008,
        WriteEa = 0x0010,
        Execute = 0x0020,
        Traverse = 0x0020,
        DeleteChild = 0x0040,
        ReadAttributes = 0x0080,
        WriteAttributes = 0x0100,
        All = StandardRights.Required | StandardRights.Synchronize | 0x1ff,
        GenericRead = StandardRights.Read | ReadData | ReadAttributes | ReadEa | 
            StandardRights.Synchronize,
        GenericWrite = StandardRights.Write | WriteData | WriteAttributes | WriteEa | 
            AppendData | StandardRights.Synchronize,
        GenericExecute = StandardRights.Execute | ReadAttributes | Execute | 
            StandardRights.Synchronize
    }
}
