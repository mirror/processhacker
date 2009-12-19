/*
 * Process Hacker - 
 *   symbol information
 * 
 * Copyright (C) 2009 wj32
 * 
 * This file is part of Process Hacker.
 * 
 * Process Hacker is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * Process Hacker is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Process Hacker.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Runtime.InteropServices;
using ProcessHacker.Native.Api;

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
            this.Name = Marshal.PtrToStringAnsi(symbolInfo.Increment(SymbolInfo.NameOffset), si.NameLen);
            this.Size = symbolSize;
            this.Address = si.Address;
        }

        public ulong Address
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
