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
using System.Security;
using System.Text;
using ProcessHacker.Common.Threading;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;

namespace ProcessHacker.Native.Api
{
    public delegate bool EnumWindowsProc(IntPtr hWnd, uint param);
    public delegate bool EnumChildProc(IntPtr hWnd, uint param);
    public delegate bool EnumThreadWndProc(IntPtr hWnd, uint param);
    public delegate IntPtr WndProcDelegate(IntPtr hWnd, WindowMessage msg, IntPtr wParam, IntPtr lParam);

    public delegate bool SymEnumSymbolsProc(IntPtr SymInfo, int SymbolSize, int UserContext);
    public unsafe delegate bool ReadProcessMemoryProc64(IntPtr ProcessHandle, ulong BaseAddress, IntPtr Buffer,
        int Size, out int BytesRead);
    public delegate IntPtr FunctionTableAccessProc64(IntPtr ProcessHandle, ulong AddrBase);
    public delegate ulong GetModuleBaseProc64(IntPtr ProcessHandle, ulong Address);

    /// <summary>
    /// Provides interfacing to the Win32 and Native APIs.
    /// </summary>
    [SuppressUnmanagedCodeSecurity]
    public static partial class Win32
    {
        private static FastMutex _dbgHelpLock = new FastMutex();

        /// <summary>
        /// A mutex which controls access to the dbghelp.dll functions.
        /// </summary>
        public static FastMutex DbgHelpLock
        {
            get { return _dbgHelpLock; }
        }

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

        public static Win32Error GetLastErrorCode()
        {
            return (Win32Error)Marshal.GetLastWin32Error();
        }

        /// <summary>
        /// Gets the error message associated with the last error that occured.
        /// </summary>
        /// <returns>An error message.</returns>
        public static string GetLastErrorMessage()
        {
            return GetLastErrorCode().GetMessage();
        }

        /// <summary>
        /// Throws a WindowsException with the last error that occurred.
        /// </summary>
        public static void Throw()
        {
            Throw(GetLastErrorCode());
        }

        public static void Throw(NtStatus status)
        {
            throw new WindowsException(status);
        }

        public static void Throw(int error)
        {
            Throw((Win32Error)error);
        }

        public static void Throw(Win32Error error)
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
                    Throw(status);
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
                    Throw();
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
            MibTcpStats tcpStats;
            GetTcpStatistics(out tcpStats);
            return tcpStats;
        }

        public static MibTcpTableOwnerPid GetTcpTable()
        {
            MibTcpTableOwnerPid table = new MibTcpTableOwnerPid();
            int length = 0;

            GetExtendedTcpTable(IntPtr.Zero, ref length, false, AiFamily.INet, TcpTableClass.OwnerPidAll, 0);

            using (MemoryAlloc mem = new MemoryAlloc(length))
            {
                GetExtendedTcpTable(mem, ref length, false, AiFamily.INet, TcpTableClass.OwnerPidAll, 0);

                int count = mem.ReadInt32(0);

                table.NumEntries = count;
                table.Table = new MibTcpRowOwnerPid[count];

                for (int i = 0; i < count; i++)
                    table.Table[i] = mem.ReadStruct<MibTcpRowOwnerPid>(sizeof(int), i);
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
            WtsProcessInfo* dataP = (WtsProcessInfo*)data.Memory;

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
            MibUdpStats udpStats;
            GetUdpStatistics(out udpStats);
            return udpStats;
        }

        public static MibUdpTableOwnerPid GetUdpTable()
        {
            MibUdpTableOwnerPid table = new MibUdpTableOwnerPid();
            int length = 0;

            GetExtendedUdpTable(IntPtr.Zero, ref length, false, AiFamily.INet, UdpTableClass.OwnerPid, 0);

            using (MemoryAlloc mem = new MemoryAlloc(length))
            {
                GetExtendedUdpTable(mem, ref length, false, AiFamily.INet, UdpTableClass.OwnerPid, 0);
                        
                int count = mem.ReadInt32(0);

                table.NumEntries = count;
                table.Table = new MibUdpRowOwnerPid[count];

                for (int i = 0; i < count; i++)
                    table.Table[i] = mem.ReadStruct<MibUdpRowOwnerPid>(sizeof(int), i);
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
            char* chptr = (char*)ptr;
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
