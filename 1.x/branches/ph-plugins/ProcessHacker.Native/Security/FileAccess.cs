using System;

namespace ProcessHacker.Native.Security
{
    [Flags]
    public enum FileAccess : uint
    {
        ReadData = 0x0001, // File, Named Pipe
        ListDirectory = 0x0001, // Directory

        WriteData = 0x0002, // File, Named Pipe
        AddFile = 0x0002, // Directory

        AppendData = 0x0004, // File
        AddSubdirectory = 0x0004, // Directory
        CreatePipeInstance = 0x0004, // Named Pipe

        ReadEa = 0x0008, // File, Directory

        WriteEa = 0x0010, // File, Directory

        Execute = 0x0020, // File
        Traverse = 0x0020, // Directory

        DeleteChild = 0x0040, // Directory

        ReadAttributes = 0x0080, // All

        WriteAttributes = 0x0100, // All

        All = StandardRights.Required | StandardRights.Synchronize | 0x1ff,
        GenericRead = StandardRights.Read | ReadData | ReadAttributes | ReadEa | 
            StandardRights.Synchronize,
        GenericWrite = StandardRights.Write | WriteData | WriteAttributes | WriteEa | 
            AppendData | StandardRights.Synchronize,
        GenericExecute = StandardRights.Execute | ReadAttributes | Execute | 
            StandardRights.Synchronize
    }
}
