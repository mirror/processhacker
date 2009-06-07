/*
 * Process Hacker - 
 *   windows API wrapper code
 *
 * Copyright (C) 2009 Flavio Erlich 
 * Copyright (C) 2009 Dean
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
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;

namespace ProcessHacker.Native.Api
{
    /// <summary>
    /// Provides interfacing to the Win32 and Native APIs.
    /// </summary>
    public partial class Win32
    {
        public delegate bool EnumWindowsProc(IntPtr hWnd, uint param);
        public delegate bool EnumChildProc(IntPtr hWnd, uint param);
        public delegate bool EnumThreadWndProc(IntPtr hWnd, uint param);
        public delegate IntPtr WndProcDelegate(IntPtr hWnd, WindowMessage msg, IntPtr wParam, IntPtr lParam);

        public delegate int SymEnumSymbolsProc(IntPtr pSymInfo, int SymbolSize, int UserContext);
        public unsafe delegate bool ReadProcessMemoryProc64(IntPtr ProcessHandle, ulong BaseAddress, byte* Buffer,
            int Size, out int BytesRead);
        public delegate IntPtr FunctionTableAccessProc64(IntPtr ProcessHandle, ulong AddrBase);
        public delegate ulong GetModuleBaseProc64(IntPtr ProcessHandle, ulong Address);

        #region Consts

        public const int DontResolveDllReferences = 0x1;
        public const int ErrorNoMoreItems = 259;
        public const int SeeMaskInvokeIdList = 0xc;
        public const uint ServiceNoChange = 0xffffffff;
        public const uint ShgFiIcon = 0x100;
        public const uint ShgFiLargeIcon = 0x0;
        public const uint ShgFiSmallIcon = 0x1;

        #endregion    

        #region Errors

        /// <summary>
        /// Gets the error message associated with the specified error code.
        /// </summary>
        /// <param name="ErrorCode">The error code.</param>
        /// <returns>An error message.</returns>
        public static string GetErrorMessage(int ErrorCode)
        {
            StringBuilder buffer = new StringBuilder(0x100);

            if (FormatMessage(0x3200, IntPtr.Zero, ErrorCode, 0, buffer, buffer.Capacity, IntPtr.Zero) == 0)
                return "Unknown error (0x" + ErrorCode.ToString("x") + ")";

            StringBuilder result = new StringBuilder();
            int i = 0;

            while (i < buffer.Length)
            {
                if (!char.IsLetterOrDigit(buffer[i]) && 
                    !char.IsPunctuation(buffer[i]) && 
                    !char.IsSymbol(buffer[i]) && 
                    !char.IsWhiteSpace(buffer[i]))
                    break;

                result.Append(buffer[i]);
                i++;
            }

            return result.ToString().Replace("\r\n", "");
        }

        public static int GetHR(int error)
        {
            if ((error & 0x80000000) == 0x80000000)
                return error;

            return (int)(0x80070000 | (uint)(error & 0xffff));
        }

        /// <summary>
        /// Gets the error message associated with the last error that occured.
        /// </summary>
        /// <returns>An error message.</returns>
        public static string GetLastErrorMessage()
        {
            return GetErrorMessage(Marshal.GetLastWin32Error());
        }

        /// <summary>
        /// Throws a Win32Exception with the last error that occurred.
        /// </summary>
        public static void ThrowLastError()
        {
            ThrowLastError(Marshal.GetLastWin32Error());
        }

        public static void ThrowLastError(NtStatus status)
        {
            throw new WindowsException(status);
        }

        public static void ThrowLastError(int error)
        {
            throw new WindowsException(error);
        }

        #endregion

        #region Handles

        public unsafe static void DuplicateObject(
            IntPtr sourceProcessHandle,
            IntPtr sourceHandle,
            int desiredAccess,
            HandleFlags handleAttributes,
            DuplicateOptions options
            )
        {
            IntPtr dummy;

            DuplicateObject(
                sourceProcessHandle,
                sourceHandle,
                IntPtr.Zero,
                out dummy,
                desiredAccess,
                handleAttributes,
                options
                );
        }

        public unsafe static void DuplicateObject(
            IntPtr sourceProcessHandle,
            IntPtr sourceHandle,
            IntPtr targetProcessHandle,
            out IntPtr targetHandle,
            int desiredAccess,
            HandleFlags handleAttributes,
            DuplicateOptions options
            )
        {
            if (KProcessHacker.Instance != null)
            {
                int target;

                KProcessHacker.Instance.KphDuplicateObject(
                    sourceProcessHandle.ToInt32(),
                    sourceHandle.ToInt32(),
                    targetProcessHandle.ToInt32(),
                    out target,
                    desiredAccess,
                    handleAttributes,
                    options);
                targetHandle = new IntPtr(target);
            }
            else
            {
                NtStatus status;

                if ((status = NtDuplicateObject(
                    sourceProcessHandle,
                    sourceHandle,
                    targetProcessHandle,
                    out targetHandle,
                    desiredAccess,
                    handleAttributes,
                    options)) >= NtStatus.Error)
                    ThrowLastError(status);
            }
        }

        #endregion

        #region Processes

        public static int GetProcessSessionId(int ProcessId)
        {
            int sessionId;

            try
            {
                if (!ProcessIdToSessionId(ProcessId, out sessionId))
                    ThrowLastError();
            }
            catch
            {
                using (ProcessHandle phandle = new ProcessHandle(ProcessId, OSVersion.MinProcessQueryInfoAccess))
                {
                    return phandle.GetToken(TokenAccess.Query).GetSessionId();
                }
            }

            return sessionId;
        }

        #endregion

        #region TCP

        public static MibTcpStats GetTcpStats()
        {
            MibTcpStats tcpStats = new MibTcpStats();
            GetTcpStatistics(out tcpStats);
            return tcpStats;
        }

        public static MibTcpTableOwnerPid GetTcpTable()
        {
            MibTcpTableOwnerPid table = new MibTcpTableOwnerPid();
            int length = 0;

            GetExtendedTcpTable(IntPtr.Zero, ref length, false, 2, TcpTableClass.OwnerPidAll, 0);

            using (MemoryAlloc mem = new MemoryAlloc(length))
            {
                GetExtendedTcpTable(mem, ref length, false, 2, TcpTableClass.OwnerPidAll, 0);

                int count = mem.ReadInt32(0);

                table.NumEntries = count;
                table.Table = new MibTcpRowOwnerPid[count];

                for (int i = 0; i < count; i++)
                    table.Table[i] = mem.ReadStruct<MibTcpRowOwnerPid>(4, i);
            }

            return table;
        }

        #endregion

        #region Terminal Server

        public struct WtsEnumProcessesFastData
        {
            public int[] PIDs;
            public IntPtr[] SIDs;
            public WtsMemoryAlloc Memory;
        }

        public unsafe static WtsEnumProcessesFastData TSEnumProcessesFast()
        {
            IntPtr processes;
            int count;
            int[] pids;
            IntPtr[] sids;

            WTSEnumerateProcesses(IntPtr.Zero, 0, 1, out processes, out count);

            pids = new int[count];
            sids = new IntPtr[count];

            WtsMemoryAlloc data = new WtsMemoryAlloc(processes);
            WtsProcessInfo* dataP = (WtsProcessInfo*)data.Memory.ToPointer();

            for (int i = 0; i < count; i++)
            {
                pids[i] = dataP[i].ProcessId;
                sids[i] = dataP[i].Sid;
            }

            return new WtsEnumProcessesFastData() { PIDs = pids, SIDs = sids, Memory = data };
        }

        #endregion

        #region UDP

        public static MibUdpStats GetUdpStats()
        {
            MibUdpStats udpStats = new MibUdpStats();
            GetUdpStatistics(out udpStats);
            return udpStats;
        }

        public static MibUdpTableOwnerPid GetUdpTable()
        {
            MibUdpTableOwnerPid table = new MibUdpTableOwnerPid();
            int length = 0;

            GetExtendedUdpTable(IntPtr.Zero, ref length, false, 2, UdpTableClass.OwnerPid, 0);

            using (MemoryAlloc mem = new MemoryAlloc(length))
            {
                GetExtendedUdpTable(mem, ref length, false, 2, UdpTableClass.OwnerPid, 0);
                        
                int count = mem.ReadInt32(0);

                table.NumEntries = count;
                table.Table = new MibUdpRowOwnerPid[count];

                for (int i = 0; i < count; i++)
                    table.Table[i] = mem.ReadStruct<MibUdpRowOwnerPid>(4, i);
            }

            return table;
        }

        #endregion

        #region Unsafe

        /// <summary>
        /// Converts a multi-string into a managed string array. A multi-string 
        /// consists of an array of null-terminated strings plus an extra null to 
        /// terminate the array.
        /// </summary>
        /// <param name="ptr">The pointer to the array.</param>
        /// <returns>A string array.</returns>
        public unsafe static string[] GetMultiString(IntPtr ptr)
        {
            List<string> list = new List<string>();
            char* chptr = (char*)ptr.ToPointer();
            StringBuilder currentString = new StringBuilder();

            while (true)
            {
                while (*chptr != 0)
                {
                    currentString.Append(*chptr);  
                    chptr++;
                }

                string str = currentString.ToString();

                if (str == "")
                {
                    break;
                }
                else
                {
                    list.Add(str);
                    currentString = new StringBuilder();
                }
            }

            return list.ToArray();
        }

        #endregion
    }
}
