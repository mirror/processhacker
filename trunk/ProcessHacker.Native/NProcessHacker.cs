/*
 * Process Hacker - 
 *   interfacing code to native library
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
using System.Security;
using ProcessHacker.Native.Api;

namespace ProcessHacker.Native
{
    [SuppressUnmanagedCodeSecurity]
    public static class NProcessHacker
    {
        public enum WS_INFORMATION_CLASS
        {
            WsCount = 0,
            WsPrivateCount,
            WsSharedCount,
            WsShareableCount,
            WsAllCounts
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WS_ALL_COUNTS
        {
            public int Count;
            public int PrivateCount;
            public int SharedCount;
            public int ShareableCount;
        }

        [DllImport("nprocesshacker.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void KphHookDeinit();

        [DllImport("nprocesshacker.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void KphHookInit();

        [DllImport("nprocesshacker.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern NtStatus PhpQueryProcessWs(
            [In] IntPtr ProcessHandle,
            [In] WS_INFORMATION_CLASS WsInformationClass,
            [Out] out int WsInformation,
            [In] int WsInformationLength,
            [Out] out int ReturnLength
            );

        [DllImport("nprocesshacker.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern NtStatus PhpQueryProcessWs(
            [In] IntPtr ProcessHandle,
            [In] WS_INFORMATION_CLASS WsInformationClass,
            [Out] out WS_ALL_COUNTS WsInformation,
            [In] int WsInformationLength,
            [Out] out int ReturnLength
            );

        [DllImport("nprocesshacker.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern NtStatus PhQueryNameFileObject(
            [In] IntPtr FileHandle,
            [In] IntPtr FileObjectNameInformation,
            [In] int FileObjectNameInformationLength,
            [Out] [Optional] out int ReturnLength
            );

        [DllImport("nprocesshacker.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void PhVoid();

        [DllImport("nprocesshacker.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern VerifyResult PhvVerifyFile(string FileName);
    }
}
