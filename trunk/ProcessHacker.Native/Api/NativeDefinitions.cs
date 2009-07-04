/*
 * Process Hacker - 
 *   native API consts and delegates
 *
 * Copyright (C) 2008-2009 wj32
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

namespace ProcessHacker.Native.Api
{
    public delegate void TimerApcRoutine(IntPtr context, int lowValue, int highValue);

    public static partial class Win32
    {
        public const int ExceptionMaximumParameters = 15;
        public const int FlsMaximumAvailable = 128;
#if _WIN64
        public const int GdiHandleBufferSize = 60;
#else
        public const int GdiHandleBufferSize = 34;
#endif
        public const int MaximumSupportedExtension = 512;
        public const int MaximumWaitObjects = 64;
        public const int MaxKeyNameLength = 512;
        public const int MaxKeyValueNameLength = 32767;
        public const int MaxStackDepth = 32;
        public const int PortMessageMaxDataLength = 0x130;
        public const int PortMessageMaxLength = 0x148;
        public const int ProcessHandleTracingMaxStacks = 16;
        public static readonly int ProcessHandleTracingQueryHandleTraceOffset =
            Marshal.OffsetOf(typeof(ProcessHandleTracingQuery), "HandleTrace").ToInt32();
        public const int SecurityDescriptorMinLength = 20;
        public const int SecurityDescriptorRevision = 1;
        public static readonly int SecurityMaxSidSize =
            Marshal.SizeOf(typeof(SidStruct)) - sizeof(int) + (SidMaxSubAuthorities * sizeof(int));
        public const int SidMaxSubAuthorities = 15;
        public const int SidRecommendedSubAuthorities = 1;
        public const int SidRevision = 1;
        public const int SizeOf80387Registers = 80;
        public const int TimeMsTo100Ns = 10000;

        public static readonly IntPtr PebLdrOffset = Marshal.OffsetOf(typeof(Peb), "Ldr");
        public static readonly IntPtr PebProcessParametersOffset = Marshal.OffsetOf(typeof(Peb), "ProcessParameters");
    }
}
