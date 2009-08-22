using System;
using System.Collections.Generic;
using System.Text;
using ProcessHacker.Native.Api;

namespace ProcessHacker.Native
{
    public interface ILoadedModule
    {
        IntPtr BaseAddress { get; }
        int Size { get; }
        LdrpDataTableEntryFlags Flags { get; }
        string BaseName { get; }
        string FileName { get; }
    }
}
