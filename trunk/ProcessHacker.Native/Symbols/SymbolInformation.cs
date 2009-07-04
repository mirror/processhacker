using System;
using ProcessHacker.Native.Api;
using System.Runtime.InteropServices;

namespace ProcessHacker.Native.Symbols
{
    public sealed class SymbolInformation
    {
        internal SymbolInformation(IntPtr symbolInfo, int symbolSize)
        {
            SymbolInfo si = (SymbolInfo)Marshal.PtrToStructure(symbolInfo, typeof(SymbolInfo));

            this.Flags = si.Flags;
            this.Index = si.Index;
            this.ModuleBase = si.ModBase;
            this.Name = Marshal.PtrToStringAnsi(symbolInfo.Increment(Win32.SymbolInfoNameOffset), si.NameLen);
            this.Size = symbolSize;
            this.Address = si.Address;
        }

        public long Address
        {
            get;
            private set;
        }

        public SymbolFlags Flags
        {
            get;
            private set;
        }

        public int Index
        {
            get;
            private set;
        }

        public ulong ModuleBase
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            private set;
        }

        public int Size
        {
            get;
            private set;
        }
    }
}
