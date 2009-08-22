/*
 * Process Hacker - 
 *   module information
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
using ProcessHacker.Native.Api;

namespace ProcessHacker.Native.Debugging
{
    public class ModuleInformation : ILoadedModule
    {
        internal ModuleInformation(RtlProcessModuleInformation moduleInfo)
        {
            this.BaseAddress = moduleInfo.ImageBase;
            this.Size = moduleInfo.ImageSize;
            this.Flags = moduleInfo.Flags;
            this.LoadCount = moduleInfo.LoadCount;

            int nullIndex = Array.IndexOf<char>(moduleInfo.FullPathName, '\0');

            if (nullIndex != -1)
                this.FileName = new string(moduleInfo.FullPathName, 0, nullIndex);
            else
                this.FileName = new string(moduleInfo.FullPathName);

            this.BaseName = this.FileName.Substring(moduleInfo.OffsetToFileName);
        }

        public IntPtr BaseAddress { get; private set; }
        public int Size { get; private set; }
        public LdrpDataTableEntryFlags Flags { get; private set; }
        public ushort LoadCount { get; private set; }
        public string BaseName { get; private set; }
        public string FileName { get; private set; }
    }
}
