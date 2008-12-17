/*
 * Process Hacker
 * 
 * Copyright (C) 2008 wj32
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.ComponentModel;

// you won't get some of this stuff from anywhere else... :)

namespace ProcessHacker
{
    public partial class Win32
    {
        #region Kernel

        [DllImport("psapi.dll", SetLastError = true)]
        public static extern int EnumDeviceDrivers(int[] ImageBases, int Size, ref int Needed);

        [DllImport("psapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetDeviceDriverBaseName(int ImageBase,
            [Out] System.Text.StringBuilder FileName, int Size);

        [DllImport("psapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetDeviceDriverFileName(int ImageBase,
            [Out] System.Text.StringBuilder FileName, int Size);

        #endregion

        #region Libraries

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int LoadLibrary(string FileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern int LoadLibraryEx(string FileName, int File, int Flags);

        [DllImport("kernel32.dll")]
        public static extern int FreeLibrary(int Handle);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern int GetModuleHandle(string ModuleName);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern int GetProcAddress(int Module, string ProcName);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern int GetProcAddress(int Module, int ProcOrdinal);

        #endregion

        #region Memory

        [DllImport("kernel32.dll")]
        public static extern int VirtualQueryEx(int Process, int Address,
            [MarshalAs(UnmanagedType.Struct)] ref MEMORY_BASIC_INFORMATION Buffer, int Size);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int VirtualProtect(int Address, int Size, int NewProtect, ref int OldProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int VirtualProtectEx(int Process, int Address, int Size, int NewProtect, ref int OldProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int VirtualAllocEx(int Process, int Address, int Size, MEMORY_STATE Type, MEMORY_PROTECTION Protect);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int ReadProcessMemory(int Process, int BaseAddress, byte[] Buffer, int Size, ref int BytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int ReadProcessMemory(int Process, int BaseAddress, int Buffer, int Size, ref int BytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int WriteProcessMemory(int Process, int BaseAddress, byte[] Buffer, int Size, ref int BytesWritten);

        #endregion

        #region Misc.

        #endregion

        #region Processes

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int SetProcessAffinityMask(int ProcessHandle, uint ProcessAffinityMask);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetProcessAffinityMask(int ProcessHandle, ref uint ProcessAffinityMask,
            ref uint SystemAffinityMask);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int CheckRemoteDebuggerPresent(int ProcessHandle, ref int DebuggerPresent);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetProcessId(int ProcessHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetCurrentProcess();

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetProcessDEPPolicy(int ProcessHandle, ref DEPFLAGS Flags, ref int Permanent);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int TerminateProcess(int ProcessHandle, int ExitCode);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int OpenProcess(PROCESS_RIGHTS DesiredAccess, int InheritHandle, int ProcessId);

        [DllImport("kernel32.dll")]
        public static extern int DebugActiveProcess(int PID);

        [DllImport("kernel32.dll")]
        public static extern int DebugActiveProcessStop(int PID);

        #endregion

        #region Resources/Handles

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int CloseHandle(int Handle);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int DuplicateHandle(int hSourceProcessHandle,
           int hSourceHandle, int hTargetProcessHandle, ref int lpTargetHandle,
           uint dwDesiredAccess, int bInheritHandle, uint dwOptions);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetNamedPipeHandleState(int NamedPipeHandle, ref PIPE_STATE State,
            int CurInstances, int MaxCollectionCount, int CollectDataTimeout, int UserName, int MaxUserNameSize);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int WaitForSingleObject(int Object, int Timeout);

        #endregion

        #region Security

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern int LsaFreeMemory(IntPtr Memory);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern int LsaEnumerateAccountsWithUserRight(
            int PolicyHandle, int UserRights, out IntPtr SIDs, out int CountReturned);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern int LsaAddAccountRights(int PolicyHandle, int AccountSid,
            LSA_UNICODE_STRING[] UserRights, uint CountOfRights);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern int LsaOpenPolicy(int SystemName, ref LSA_OBJECT_ATTRIBUTES ObjectAttributes,
            POLICY_RIGHTS DesiredAccess, ref int PolicyHandle);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern int LsaClose(int Handle);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern int OpenProcessToken(int ProcessHandle, TOKEN_RIGHTS DesiredAccess,
            out int TokenHandle);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern int OpenThreadToken(int ThreadHandle, TOKEN_RIGHTS DesiredAccess,
            bool OpenAsSelf, out int TokenHandle);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int ConvertSidToStringSid(
            int pSID,
            [In, Out, MarshalAs(UnmanagedType.LPTStr)] ref string pStringSid
        );

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int ConvertStringSidToSid(
            [In, MarshalAs(UnmanagedType.LPTStr)] string pStringSid,
            ref IntPtr pSID
        );

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern int CreateProcessWithTokenW(int Token, int LogonFlags,
            [MarshalAs(UnmanagedType.LPWStr)] string ApplicationName,
            [MarshalAs(UnmanagedType.LPWStr)] string CommandLine, int CreationFlags,
            int Environment, int CurrentDirectory, STARTUPINFO StartupInfo,
            PROCESS_INFORMATION ProcessInfo);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern int DuplicateTokenEx(int ExistingToken, TOKEN_RIGHTS DesiredAccess,
            int TokenAttributes, SECURITY_IMPERSONATION_LEVEL ImpersonationLevel, TOKEN_TYPE TokenType,
            ref int NewToken);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetTokenInformation(int TokenHandle,
            TOKEN_INFORMATION_CLASS TokenInformationClass, int TokenInformation,
            int TokenInformationLength, ref int ReturnLength);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetTokenInformation(int TokenHandle,
            TOKEN_INFORMATION_CLASS TokenInformationClass, IntPtr TokenInformation,
            int TokenInformationLength, ref int ReturnLength);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetTokenInformation(int TokenHandle,
            TOKEN_INFORMATION_CLASS TokenInformationClass, ref int TokenInformation,
            int TokenInformationLength, ref int ReturnLength);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetTokenInformation(int TokenHandle,
            TOKEN_INFORMATION_CLASS TokenInformationClass, ref TOKEN_GROUPS TokenInformation,
            int TokenInformationLength, ref int ReturnLength);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetTokenInformation(int TokenHandle,
            TOKEN_INFORMATION_CLASS TokenInformationClass, ref TOKEN_USER TokenInformation,
            int TokenInformationLength, ref int ReturnLength);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetTokenInformation(int TokenHandle,
            TOKEN_INFORMATION_CLASS TokenInformationClass, ref TOKEN_PRIVILEGES TokenInformation,
            int TokenInformationLength, ref int ReturnLength);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int LookupAccountSid(string SystemName,
            int SID, [Out] System.Text.StringBuilder Name, ref int NameSize,
            [Out] System.Text.StringBuilder ReferencedDomainName, ref int ReferencedDomainNameSize,
            ref SID_NAME_USE Use);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int LookupAccountSid(int SystemName,
            int SID, [Out] System.Text.StringBuilder Name, ref int NameSize,
            [Out] StringBuilder ReferencedDomainName, ref int ReferencedDomainNameSize,
            ref SID_NAME_USE Use);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int LookupPrivilegeDisplayName(int SystemName, string Name,
            [Out] StringBuilder DisplayName, ref int DisplayNameSize, ref int LanguageId);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int LookupPrivilegeName(int SystemName, ref LUID Luid,
            [Out] StringBuilder Name, ref int RequiredSize);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int LookupPrivilegeValue(string SystemName, string PrivilegeName,
            [MarshalAs(UnmanagedType.Struct)] ref LUID Luid);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern int AdjustTokenPrivileges(int TokenHandle, int DisableAllPrivileges,
            [MarshalAs(UnmanagedType.Struct)] ref TOKEN_PRIVILEGES NewState, int BufferLength,
            int PreviousState, int ReturnLength);

        #endregion      

        #region Services

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern int CloseServiceHandle(int ServiceHandle);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern int StartService(int Service, int NumServiceArgs, int Args);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int ChangeServiceConfig(int Service,
            SERVICE_TYPE ServiceType, SERVICE_START_TYPE StartType,
            SERVICE_ERROR_CONTROL ErrorControl,
            [MarshalAs(UnmanagedType.LPTStr)] string BinaryPath,
            [MarshalAs(UnmanagedType.LPTStr)] string LoadOrderGroup,
            int TagID, int Dependencies,
            [MarshalAs(UnmanagedType.LPTStr)] string StartName,
            int Password, int DisplayName);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern int ControlService(int Service,
            SERVICE_CONTROL Control, ref SERVICE_STATUS ServiceStatus);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int CreateService(int SCManager,
            [MarshalAs(UnmanagedType.LPTStr)] string ServiceName,
            [MarshalAs(UnmanagedType.LPTStr)] string DisplayName,
            SERVICE_RIGHTS DesiredAccess, SERVICE_TYPE ServiceType,
            SERVICE_START_TYPE StartType, SERVICE_ERROR_CONTROL ErrorControl,
            [MarshalAs(UnmanagedType.LPTStr)] string BinaryPathName,
            [MarshalAs(UnmanagedType.LPTStr)] string LoadOrderGroup,
            int TagID, int Dependencies,
            [MarshalAs(UnmanagedType.LPTStr)] string ServiceStartName,
            [MarshalAs(UnmanagedType.LPTStr)] string Password);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern int DeleteService(int Service);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int QueryServiceStatus(int Service, ref SERVICE_STATUS ServiceStatus);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int QueryServiceStatusEx(int Service, int InfoLevel,
            ref SERVICE_STATUS_PROCESS ServiceStatus, int BufSize, out int BytesNeeded);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int QueryServiceConfig(int Service,
            int ServiceConfig,
            int BufSize, ref int BytesNeeded);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int QueryServiceConfig(int Service,
            IntPtr ServiceConfig,
            int BufSize, ref int BytesNeeded);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int QueryServiceConfig(int Service,
            [MarshalAs(UnmanagedType.Struct)] ref QUERY_SERVICE_CONFIG ServiceConfig,
            int BufSize, ref int BytesNeeded);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int OpenService(int SCManager,
            [MarshalAs(UnmanagedType.LPTStr)] string ServiceName, SERVICE_RIGHTS DesiredAccess);

        /// <summary>
        /// Enumerates services in the specified service control manager database. 
        /// The name and status of each service are provided, along with additional 
        /// data based on the specified information level.
        /// </summary>
        /// <param name="SCManager">A handle to the service control manager database.</param>
        /// <param name="InfoLevel">Set this to 0.</param>
        /// <param name="ServiceType">The type of services to be enumerated.</param>
        /// <param name="ServiceState">The state of the services to be enumerated.</param>
        /// <param name="Services">A pointer to the buffer that receives the status information.</param>
        /// <param name="BufSize">The size of the buffer pointed to by the Services parameter, in bytes.</param>
        /// <param name="BytesNeeded">A pointer to a variable that receives the number of bytes needed to 
        /// return the remaining service entries, if the buffer is too small.</param>
        /// <param name="ServicesReturned">A pointer to a variable that receives the number of service 
        /// entries returned.</param>
        /// <param name="ResumeHandle">A pointer to a variable that, on input, specifies the 
        /// starting point of enumeration. You must set this value to zero the first time the 
        /// EnumServicesStatusEx function is called.</param>
        /// <param name="GroupName">Must be 0 for this definition.</param>
        /// <returns>A non-zero value for success, zero for failure.</returns>
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int EnumServicesStatusEx(int SCManager, int InfoLevel,
            SERVICE_QUERY_TYPE ServiceType, SERVICE_QUERY_STATE ServiceState,
            ref int Services, int BufSize, ref int BytesNeeded, ref int ServicesReturned,
            ref int ResumeHandle, int GroupName);

        /// <summary>
        /// Enumerates services in the specified service control manager database. 
        /// The name and status of each service are provided, along with additional 
        /// data based on the specified information level.
        /// </summary>
        /// <param name="SCManager">A handle to the service control manager database.</param>
        /// <param name="InfoLevel">Set this to 0.</param>
        /// <param name="ServiceType">The type of services to be enumerated.</param>
        /// <param name="ServiceState">The state of the services to be enumerated.</param>
        /// <param name="Services">A pointer to the buffer that receives the status information.</param>
        /// <param name="BufSize">The size of the buffer pointed to by the Services parameter, in bytes.</param>
        /// <param name="BytesNeeded">A pointer to a variable that receives the number of bytes needed to 
        /// return the remaining service entries, if the buffer is too small.</param>
        /// <param name="ServicesReturned">A pointer to a variable that receives the number of service 
        /// entries returned.</param>
        /// <param name="ResumeHandle">A pointer to a variable that, on input, specifies the 
        /// starting point of enumeration. You must set this value to zero the first time the 
        /// EnumServicesStatusEx function is called.</param>
        /// <param name="GroupName">Must be 0 for this definition.</param>
        /// <returns>A non-zero value for success, zero for failure.</returns>
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int EnumServicesStatusEx(int SCManager, int InfoLevel,
            SERVICE_QUERY_TYPE ServiceType, SERVICE_QUERY_STATE ServiceState,
            IntPtr Services, int BufSize, ref int BytesNeeded, ref int ServicesReturned,
            ref int ResumeHandle, int GroupName);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern int OpenSCManager(int MachineName, int DatabaseName,
            SC_MANAGER_RIGHTS DesiredAccess);

        #endregion

        #region Shell

        [DllImport("shell32.dll")]
        public static extern int ShellExecuteEx(
            [MarshalAs(UnmanagedType.Struct)] ref SHELLEXECUTEINFO s);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int SetWindowsHookEx(int HookId, int HookFunction, int Module, int ThreadId);

        [DllImport("shell32.dll")]
        public extern static int ExtractIconEx(string libName, int iconIndex,
        IntPtr[] largeIcon, IntPtr[] smallIcon, int nIcons);

        [DllImport("shell32.dll")]
        public static extern int SHGetFileInfo(string pszPath,
                                    uint dwFileAttributes,
                                    ref SHFILEINFO psfi,
                                    uint cbSizeFileInfo,
                                    uint uFlags);

        #endregion

        #region Statistics

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetProcessTimes(int ProcessHandle, ref ulong CreationTime, ref ulong ExitTime,
            ref ulong KernelTime, ref ulong UserTime);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetProcessIoCounters(int ProcessHandle, ref IO_COUNTERS IoCounters);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetSystemTimes(ref ulong IdleTime, ref ulong KernelTime, ref ulong UserTime);

        [DllImport("kernel32.dll")]
        public static extern bool GetThreadTimes(int hThread, out long lpCreationTime,
           out long lpExitTime, out long lpKernelTime, out long lpUserTime);

        #endregion

        #region Symbols/Stack Walking

        [DllImport("dbghelp.dll", SetLastError = true)]
        public static extern int SymCleanup(int ProcessHandle);

        [DllImport("dbghelp.dll", SetLastError = true)]
        public static extern int SymEnumSymbols(int ProcessHandle, int BaseOfDll, int Mask,
            [MarshalAs(UnmanagedType.FunctionPtr)] SymEnumSymbolsProc EnumSymbolsCallback, int UserContext);

        [DllImport("dbghelp.dll", SetLastError = true)]
        public static extern int SymEnumSymbols(int ProcessHandle, int BaseOfDll, string Mask,
            [MarshalAs(UnmanagedType.FunctionPtr)] SymEnumSymbolsProc EnumSymbolsCallback, int UserContext);

        [DllImport("dbghelp.dll", SetLastError = true)]
        public static extern int SymFromAddr(int ProcessHandle, long Address, ref long Displacement, ref SYMBOL_INFO Symbol);

        [DllImport("dbghelp.dll", SetLastError = true)]
        public static extern int SymFromIndex(int ProcessHandle, int BaseOfDll, int Index, ref SYMBOL_INFO Symbol);

        [DllImport("dbghelp.dll", SetLastError = true)]
        public static extern int SymFunctionTableAccess64(int ProcessHandle, int AddrBase);

        [DllImport("dbghelp.dll", SetLastError = true)]
        public static extern int SymGetModuleBase64(int ProcessHandle, int dwAddr);

        [DllImport("dbghelp.dll", SetLastError = true)]
        public static extern int SymInitialize(int ProcessHandle, int UserSearchPath, int InvadeProcess);

        [DllImport("dbghelp.dll", SetLastError = true)]
        public static extern int StackWalk64(MachineType MachineType, int ProcessHandle, int ThreadHandle,
            [MarshalAs(UnmanagedType.Struct)] ref STACKFRAME64 StackFrame,
            [MarshalAs(UnmanagedType.Struct)] ref CONTEXT ContextRecord, int ReadMemoryRoutine,
            [MarshalAs(UnmanagedType.FunctionPtr)] FunctionTableAccessProc64 FunctionTableAccessRoutine,
            [MarshalAs(UnmanagedType.FunctionPtr)] GetModuleBaseProc64 GetModuleBaseRoutine,
            int TranslateAddress);

        [DllImport("dbghelp.dll", SetLastError = true)]
        public static extern int StackWalk64(MachineType MachineType, int ProcessHandle, int ThreadHandle,
            [MarshalAs(UnmanagedType.Struct)] ref STACKFRAME64 StackFrame,
            [MarshalAs(UnmanagedType.Struct)] ref CONTEXT ContextRecord, int ReadMemoryRoutine,
            int FunctionTableAccessRoutine,
            int GetModuleBaseRoutine,
            int TranslateAddress);

        #endregion

        #region Terminal Server

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int ProcessIdToSessionId(int ProcessId, ref int SessionId);

        [DllImport("wtsapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int WTSQuerySessionInformation(int ServerHandle, int SessionID,
            WTS_INFO_CLASS InfoClass,
            [MarshalAs(UnmanagedType.LPTStr)] ref string Buffer,
            ref int BytesReturned);

        [DllImport("wtsapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int WTSQuerySessionInformation(int ServerHandle, int SessionID,
            WTS_INFO_CLASS InfoClass,
            ref int Buffer,
            ref int BytesReturned);

        [DllImport("wtsapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int WTSQuerySessionInformation(int ServerHandle, int SessionID,
            WTS_INFO_CLASS InfoClass,
            ref ushort Buffer,
            ref int BytesReturned);

        [DllImport("wtsapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int WTSQuerySessionInformation(int ServerHandle, int SessionID,
            WTS_INFO_CLASS InfoClass,
            ref WTS_CLIENT_DISPLAY[] Buffer,
            ref int BytesReturned);

        [DllImport("wtsapi32.dll", SetLastError = true)]
        public static extern int WTSLogoffSession(int ServerHandle, int SessionID, int Wait);

        [DllImport("wtsapi32.dll", SetLastError = true)]
        public static extern int WTSDisconnectSession(int ServerHandle, int SessionID, int Wait);

        [DllImport("wtsapi32.dll", SetLastError = true)]
        public static extern int WTSTerminateProcess(int ServerHandle, int ProcessID, int ExitCode);

        [DllImport("wtsapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int WTSEnumerateSessions(int ServerHandle, int Reserved,
            int Version, ref WTS_SESSION_INFO[] SessionInfo, ref int Count);

        [DllImport("wtsapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int WTSEnumerateSessions(int ServerHandle, int Reserved,
            int Version, ref int SessionInfo, ref int Count);

        [DllImport("wtsapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int WTSEnumerateProcesses(int ServerHandle, int Reserved,
            int Version, ref WTS_PROCESS_INFO[] ProcessInfo, ref int Count);

        [DllImport("wtsapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int WTSEnumerateProcesses(int ServerHandle, int Reserved,
            int Version, ref int ProcessInfo, ref int Count);

        [DllImport("wtsapi32.dll", SetLastError = true)]
        public static extern int WTSFreeMemory(int Memory);
        [DllImport("wtsapi32.dll", SetLastError = true)]
        public static extern int WTSFreeMemory(WTS_PROCESS_INFO[] Memory);
        [DllImport("wtsapi32.dll", SetLastError = true)]
        public static extern int WTSFreeMemory(WTS_SESSION_INFO[] Memory);
        [DllImport("wtsapi32.dll", SetLastError = true)]
        public static extern int WTSFreeMemory(string Memory);

        #endregion

        #region Threads

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int CreateThread(int ThreadAttributes, int StackSize,
            [MarshalAs(UnmanagedType.FunctionPtr)] System.Threading.ThreadStart StartAddress,
            int Parameter, int CreationFlags, ref int ThreadId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetProcessIdOfThread(int ThreadHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetThreadId(int ThreadHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int OpenThread(THREAD_RIGHTS DesiredAccess, int InheritHandle, int ThreadId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int TerminateThread(int ThreadHandle, int ExitCode);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int SuspendThread(int ThreadHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int ResumeThread(int ThreadHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetThreadContext(int ThreadHandle, ref CONTEXT Context);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int CreateRemoteThread(int ProcessHandle, int ThreadAttributes,
            int StackSize, int StartAddress, int Parameter, int CreationFlags, ref int ThreadId);

        #endregion

        #region Toolhelp

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int CreateToolhelp32Snapshot(SnapshotFlags dwFlags, int th32ProcessID);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int Process32First(int hSnapshot,
            [MarshalAs(UnmanagedType.Struct)] ref PROCESSENTRY32 lppe);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int Process32Next(int hSnapshot,
            [MarshalAs(UnmanagedType.Struct)] ref PROCESSENTRY32 lppe);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int Thread32First(int hSnapshot,
            [MarshalAs(UnmanagedType.Struct)] ref THREADENTRY32 lppe);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int Thread32Next(int hSnapshot,
            [MarshalAs(UnmanagedType.Struct)] ref THREADENTRY32 lppe);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int Module32First(int hSnapshot,
            [MarshalAs(UnmanagedType.Struct)] ref MODULEENTRY32 lppe);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int Module32Next(int hSnapshot,
            [MarshalAs(UnmanagedType.Struct)] ref MODULEENTRY32 lppe);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int Heap32ListFirst(int hSnapshot,
            [MarshalAs(UnmanagedType.Struct)] ref HEAPLIST32 lppe);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int Heap32ListNext(int hSnapshot,
            [MarshalAs(UnmanagedType.Struct)] ref HEAPLIST32 lppe);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int Heap32First([MarshalAs(UnmanagedType.Struct)] ref HEAPENTRY32 lppe,
            int ProcessID, int HeapID);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int Heap32Next([MarshalAs(UnmanagedType.Struct)] ref HEAPENTRY32 lppe);

        #endregion

        #region Undocumented

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int ZwQuerySymbolicLinkObject(int LinkHandle, ref UNICODE_STRING LinkTarget,
            ref int ReturnedLength);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int ZwQueryInformationProcess(int ProcessHandle, PROCESSINFOCLASS ProcessInformationClass,
            IntPtr ProcessInformation, int ProcessInformationLength, ref int ReturnLength);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int ZwQueryInformationProcess(int ProcessHandle, PROCESSINFOCLASS ProcessInformationClass,
            int ProcessInformation, int ProcessInformationLength, ref int ReturnLength);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int ZwQueryInformationProcess(int ProcessHandle, PROCESSINFOCLASS ProcessInformationClass,
            ref UNICODE_STRING ProcessInformation, int ProcessInformationLength, ref int ReturnLength);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int ZwQueryInformationProcess(int ProcessHandle, PROCESSINFOCLASS ProcessInformationClass,
            ref PROCESS_BASIC_INFORMATION ProcessInformation, int ProcessInformationLength, ref int ReturnLength);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern uint ZwDuplicateObject(int SourceProcessHandle, int SourceHandle,
            int TargetProcessHandle, int TargetHandle, STANDARD_RIGHTS DesiredAccess, int Attributes, int Options);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern uint ZwDuplicateObject(int SourceProcessHandle, int SourceHandle,
            int TargetProcessHandle, ref int TargetHandle, STANDARD_RIGHTS DesiredAccess, int Attributes, int Options);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern uint ZwQuerySystemInformation(SYSTEM_INFORMATION_CLASS SystemInformationClass,
            int SystemInformation, int SystemInformationLength, ref int ReturnLength);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern uint ZwQuerySystemInformation(SYSTEM_INFORMATION_CLASS SystemInformationClass,
            SYSTEM_HANDLE_INFORMATION[] SystemInformation, int SystemInformationLength, ref int ReturnLength);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern uint ZwQuerySystemInformation(SYSTEM_INFORMATION_CLASS SystemInformationClass,
            uint[] SystemInformation, int SystemInformationLength, ref int ReturnLength);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern uint ZwQueryObject(int Handle, OBJECT_INFORMATION_CLASS ObjectInformationClass,
            ref OBJECT_BASIC_INFORMATION ObjectInformation, int ObjectInformationLength, ref int ReturnLength);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern uint ZwQueryObject(int Handle, OBJECT_INFORMATION_CLASS ObjectInformationClass,
            ref OBJECT_TYPE_INFORMATION ObjectInformation, int ObjectInformationLength, ref int ReturnLength);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern uint ZwQueryObject(int Handle, OBJECT_INFORMATION_CLASS ObjectInformationClass,
            ref OBJECT_NAME_INFORMATION ObjectInformation, int ObjectInformationLength, ref int ReturnLength);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern uint ZwQueryObject(int Handle, OBJECT_INFORMATION_CLASS ObjectInformationClass,
            IntPtr ObjectInformation, int ObjectInformationLength, ref int ReturnLength);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern uint ZwQueryObject(int Handle, OBJECT_INFORMATION_CLASS ObjectInformationClass,
            int ObjectInformation, int ObjectInformationLength, ref int ReturnLength);

        #endregion

        #region Windows

        [DllImport("user32.dll")]
        public static extern int EnumWindows([MarshalAs(UnmanagedType.FunctionPtr)] EnumWindowsProc Callback, int param);

        [DllImport("user32.dll")]
        public static extern int SetActiveWindow(int hWnd);

        #endregion
    }
}
