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
        public static extern bool EnumDeviceDrivers(int[] ImageBases, int Size, out int Needed);

        [DllImport("psapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool GetDeviceDriverBaseName(int ImageBase, StringBuilder FileName, int Size);

        [DllImport("psapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool GetDeviceDriverFileName(int ImageBase, StringBuilder FileName, int Size);

        #endregion

        #region Libraries

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int LoadLibrary(string FileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern int LoadLibraryEx(string FileName, int File, int Flags);

        [DllImport("kernel32.dll")]
        public static extern bool FreeLibrary(int Handle);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern int GetModuleHandle(string ModuleName);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern int GetProcAddress(int Module, string ProcName);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern int GetProcAddress(int Module, int ProcOrdinal);

        #endregion

        #region Memory

        [DllImport("kernel32.dll")]
        public static extern bool VirtualQueryEx(int Process, int Address,
            [MarshalAs(UnmanagedType.Struct)] ref MEMORY_BASIC_INFORMATION Buffer, int Size);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool VirtualProtectEx(int Process, int Address, int Size, int NewProtect, out int OldProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int VirtualAllocEx(int Process, int Address, int Size, MEMORY_STATE Type, MEMORY_PROTECTION Protect);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool VirtualFreeEx(int Process, int Address, int Size, MEMORY_STATE FreeType);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory(int Process, int BaseAddress, byte[] Buffer, int Size, out int BytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(int Process, int BaseAddress, IntPtr Buffer, int Size, out int BytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(int Process, int BaseAddress, byte[] Buffer, int Size, out int BytesWritten);

        #endregion

        #region Misc.
        
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int FormatMessage(
            int Flags, 
            int Source, 
            int MessageId,
            int LanguageId, 
            StringBuilder Buffer,
            int Size, 
            IntPtr Arguments
            );

        [DllImport("kernel32.dll")]
        public static extern int GetTickCount();

        #endregion

        #region Processes

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool IsProcessInJob(int ProcessHandle, int JobHandle, out bool Result);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetProcessAffinityMask(int ProcessHandle, uint ProcessAffinityMask);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetProcessAffinityMask(int ProcessHandle, out uint ProcessAffinityMask,
            out uint SystemAffinityMask);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CheckRemoteDebuggerPresent(int ProcessHandle, out bool DebuggerPresent);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetProcessId(int ProcessHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetCurrentProcess();

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetProcessDEPPolicy(int ProcessHandle, out DEPFLAGS Flags, out int Permanent);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool TerminateProcess(int ProcessHandle, int ExitCode);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int OpenProcess(PROCESS_RIGHTS DesiredAccess, int InheritHandle, int ProcessId);

        [DllImport("kernel32.dll")]
        public static extern bool DebugActiveProcess(int PID);

        [DllImport("kernel32.dll")]
        public static extern bool DebugActiveProcessStop(int PID);

        #endregion

        #region Resources/Handles

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(int Handle);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool DuplicateHandle(int SourceProcessHandle,
           int SourceHandle, int TargetProcessHandle, out int TargetHandle,
           STANDARD_RIGHTS DesiredAccess, int InheritHandle, uint Options);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetNamedPipeHandleState(int NamedPipeHandle, ref PIPE_STATE State,
            int CurInstances, int MaxCollectionCount, int CollectDataTimeout, int UserName, int MaxUserNameSize);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int WaitForSingleObject(int Object, int Timeout);

        #endregion

        #region Security

        #region LSA

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
            POLICY_RIGHTS DesiredAccess, out int PolicyHandle);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern int LsaClose(int Handle);

        #endregion

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool OpenProcessToken(int ProcessHandle, TOKEN_RIGHTS DesiredAccess,
            out int TokenHandle);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool OpenThreadToken(int ThreadHandle, TOKEN_RIGHTS DesiredAccess,
            bool OpenAsSelf, out int TokenHandle);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool DuplicateTokenEx(int ExistingToken, TOKEN_RIGHTS DesiredAccess,
            int TokenAttributes, SECURITY_IMPERSONATION_LEVEL ImpersonationLevel, TOKEN_TYPE TokenType,
            out int NewToken);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool GetTokenInformation(int TokenHandle,
            TOKEN_INFORMATION_CLASS TokenInformationClass, IntPtr TokenInformation,
            int TokenInformationLength, out int ReturnLength);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool GetTokenInformation(int TokenHandle,
            TOKEN_INFORMATION_CLASS TokenInformationClass, out int TokenInformation,
            int TokenInformationLength, out int ReturnLength);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool GetTokenInformation(int TokenHandle,
            TOKEN_INFORMATION_CLASS TokenInformationClass, ref TOKEN_SOURCE TokenInformation,
            int TokenInformationLength, out int ReturnLength);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool LookupAccountName(
            string SystemName,
            string AccountName,
            IntPtr SID,
            out int SIDSize,
            int ReferencedDomainName,
            int ReferencedDomainNameSize,
            out SID_NAME_USE Use
            );

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool LookupAccountSid(string SystemName,
            int SID, StringBuilder Name, out int NameSize,
            StringBuilder ReferencedDomainName, out int ReferencedDomainNameSize,
            out SID_NAME_USE Use);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool LookupPrivilegeDisplayName(int SystemName, string Name,
            StringBuilder DisplayName, out int DisplayNameSize, out int LanguageId);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool LookupPrivilegeName(int SystemName, ref LUID Luid,
            StringBuilder Name, out int RequiredSize);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool LookupPrivilegeValue(string SystemName, string PrivilegeName, ref LUID Luid);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool AdjustTokenPrivileges(int TokenHandle, int DisableAllPrivileges,
            ref TOKEN_PRIVILEGES NewState, int BufferLength,
            int PreviousState, int ReturnLength);

        #endregion      

        #region Services

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool CloseServiceHandle(int ServiceHandle);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool StartService(int Service, int NumServiceArgs, int Args);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool ChangeServiceConfig(
            int Service,
            SERVICE_TYPE ServiceType, 
            SERVICE_START_TYPE StartType,
            SERVICE_ERROR_CONTROL ErrorControl, 
            string BinaryPath, 
            string LoadOrderGroup,
            int TagID, 
            int Dependencies,
            string StartName,
            string Password,
            string DisplayName
            );

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool ControlService(
            int Service,
            SERVICE_CONTROL Control, 
            ref SERVICE_STATUS ServiceStatus
            );

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int CreateService(int SCManager,
            string ServiceName,
            string DisplayName,
            SERVICE_RIGHTS DesiredAccess, 
            SERVICE_TYPE ServiceType,
            SERVICE_START_TYPE StartType,
            SERVICE_ERROR_CONTROL ErrorControl,
            string BinaryPathName,
            string LoadOrderGroup,
            int TagID, 
            int Dependencies,
            string ServiceStartName,
            string Password
            );

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool DeleteService(int Service);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool QueryServiceStatus(
            int Service, 
            ref SERVICE_STATUS ServiceStatus
            );

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool QueryServiceStatusEx(int Service, int InfoLevel,
            ref SERVICE_STATUS_PROCESS ServiceStatus, int BufSize, out int BytesNeeded);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool QueryServiceConfig(int Service,
            IntPtr ServiceConfig,
            int BufSize, ref int BytesNeeded);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int OpenService(int SCManager,
            string ServiceName, SERVICE_RIGHTS DesiredAccess);

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
        public static extern bool EnumServicesStatusEx(int SCManager, int InfoLevel,
            SERVICE_QUERY_TYPE ServiceType, SERVICE_QUERY_STATE ServiceState,
            IntPtr Services, int BufSize, out int BytesNeeded, out int ServicesReturned,
            out int ResumeHandle, int GroupName);

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

        [DllImport("psapi.dll", SetLastError = true)]
        public static extern bool GetPerformanceInfo(ref PERFORMANCE_INFORMATION PerformanceInformation,
            int Size);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetProcessTimes(int ProcessHandle, out ulong CreationTime, out ulong ExitTime,
            out ulong KernelTime, out ulong UserTime);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetProcessIoCounters(int ProcessHandle, out IO_COUNTERS IoCounters);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetSystemTimes(out ulong IdleTime, out ulong KernelTime, out ulong UserTime);

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
        public static extern bool StackWalk64(MachineType MachineType, int ProcessHandle, int ThreadHandle,
            [MarshalAs(UnmanagedType.Struct)] ref STACKFRAME64 StackFrame,
            [MarshalAs(UnmanagedType.Struct)] ref CONTEXT ContextRecord, int ReadMemoryRoutine,
            [MarshalAs(UnmanagedType.FunctionPtr)] FunctionTableAccessProc64 FunctionTableAccessRoutine,
            [MarshalAs(UnmanagedType.FunctionPtr)] GetModuleBaseProc64 GetModuleBaseRoutine,
            int TranslateAddress);

        #endregion

        #region Terminal Server

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ProcessIdToSessionId(int ProcessId, out int SessionId);

        [DllImport("wtsapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool WTSQuerySessionInformation(int ServerHandle, int SessionID,
            WTS_INFO_CLASS InfoClass,
            out string Buffer,
            out int BytesReturned);

        [DllImport("wtsapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool WTSQuerySessionInformation(int ServerHandle, int SessionID,
            WTS_INFO_CLASS InfoClass,
            out int Buffer,
            out int BytesReturned);

        [DllImport("wtsapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool WTSQuerySessionInformation(int ServerHandle, int SessionID,
            WTS_INFO_CLASS InfoClass,
            out ushort Buffer,
            out int BytesReturned);

        [DllImport("wtsapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool WTSQuerySessionInformation(int ServerHandle, int SessionID,
            WTS_INFO_CLASS InfoClass,
            out WTS_CLIENT_DISPLAY[] Buffer,
            out int BytesReturned);

        [DllImport("wtsapi32.dll", SetLastError = true)]
        public static extern bool WTSLogoffSession(int ServerHandle, int SessionID, int Wait);

        [DllImport("wtsapi32.dll", SetLastError = true)]
        public static extern bool WTSDisconnectSession(int ServerHandle, int SessionID, int Wait);

        [DllImport("wtsapi32.dll", SetLastError = true)]
        public static extern bool WTSTerminateProcess(int ServerHandle, int ProcessID, int ExitCode);

        [DllImport("wtsapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool WTSEnumerateSessions(int ServerHandle, int Reserved,
            int Version, out IntPtr SessionInfo, out int Count);

        [DllImport("wtsapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool WTSEnumerateProcesses(int ServerHandle, int Reserved,
            int Version, out IntPtr ProcessInfo, out int Count);

        [DllImport("wtsapi32.dll", SetLastError = true)]
        public static extern bool WTSFreeMemory(IntPtr Memory);
        [DllImport("wtsapi32.dll", SetLastError = true)]
        public static extern bool WTSFreeMemory(string Memory);

        #endregion

        #region Threads

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetThreadPriority(int ThreadHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int CreateThread(int ThreadAttributes, int StackSize,
            [MarshalAs(UnmanagedType.FunctionPtr)] System.Threading.ThreadStart StartAddress,
            int Parameter, int CreationFlags, out int ThreadId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetProcessIdOfThread(int ThreadHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetThreadId(int ThreadHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int OpenThread(THREAD_RIGHTS DesiredAccess, int InheritHandle, int ThreadId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool TerminateThread(int ThreadHandle, int ExitCode);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int SuspendThread(int ThreadHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int ResumeThread(int ThreadHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetThreadContext(int ThreadHandle, ref CONTEXT Context);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetThreadContext(int ThreadHandle, ref CONTEXT Context);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CreateRemoteThread(int ProcessHandle, int ThreadAttributes,
            int StackSize, int StartAddress, int Parameter, int CreationFlags, out int ThreadId);

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
        public static extern int ZwQuerySection(int SectionHandle, SECTION_INFORMATION_CLASS SectionInformationClass,
            ref SECTION_BASIC_INFORMATION SectionInformation, int SectionInformationLength, out int ReturnLength);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int ZwQuerySection(int SectionHandle, SECTION_INFORMATION_CLASS SectionInformationClass,
            ref SECTION_IMAGE_INFORMATION SectionInformation, int SectionInformationLength, out int ReturnLength);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int ZwQueryMutant(int MutantHandle, MUTANT_INFORMATION_CLASS MutantInformationClass,
            ref MUTANT_BASIC_INFORMATION MutantInformation, int MutantInformationLength, out int ReturnLength);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int ZwQueryEvent(int EventHandle, EVENT_INFORMATION_CLASS EventInformationClass,
            ref EVENT_BASIC_INFORMATION EventInformation, int EventInformationLength, out int ReturnLength);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int ZwSetInformationThread(int ThreadHandle, THREAD_INFORMATION_CLASS ThreadInformationClass,
            ref int ThreadInformation, int ThreadInformationLength);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int ZwQueryInformationThread(int ThreadHandle, THREAD_INFORMATION_CLASS ThreadInformationClass,
            ref THREAD_BASIC_INFORMATION ThreadInformation, int ThreadInformationLength, out int ReturnLength);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int ZwQueryInformationThread(int ThreadHandle, THREAD_INFORMATION_CLASS ThreadInformationClass,
            out long ThreadInformation, int ThreadInformationLength, out int ReturnLength);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int ZwQueryInformationThread(int ThreadHandle, THREAD_INFORMATION_CLASS ThreadInformationClass,
            out int ThreadInformation, int ThreadInformationLength, out int ReturnLength);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int ZwQueryInformationProcess(int ProcessHandle, PROCESS_INFORMATION_CLASS ProcessInformationClass,
            ref POOLED_USAGE_AND_LIMITS ProcessInformation, int ProcessInformationLength, out int ReturnLength);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int ZwQueryInformationProcess(int ProcessHandle, PROCESS_INFORMATION_CLASS ProcessInformationClass,
            ref QUOTA_LIMITS ProcessInformation, int ProcessInformationLength, out int ReturnLength);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int ZwQueryInformationProcess(int ProcessHandle, PROCESS_INFORMATION_CLASS ProcessInformationClass,
            ref UNICODE_STRING ProcessInformation, int ProcessInformationLength, out int ReturnLength);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int ZwQueryInformationProcess(int ProcessHandle, PROCESS_INFORMATION_CLASS ProcessInformationClass,
            ref PROCESS_BASIC_INFORMATION ProcessInformation, int ProcessInformationLength, out int ReturnLength);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern uint ZwDuplicateObject(int SourceProcessHandle, int SourceHandle,
            int TargetProcessHandle, int TargetHandle, STANDARD_RIGHTS DesiredAccess, int Attributes, int Options);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern uint ZwDuplicateObject(int SourceProcessHandle, int SourceHandle,
            int TargetProcessHandle, out int TargetHandle, STANDARD_RIGHTS DesiredAccess, int Attributes, int Options);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern uint ZwQuerySystemInformation(SYSTEM_INFORMATION_CLASS SystemInformationClass,
            ref SYSTEM_BASIC_INFORMATION SystemInformation, int SystemInformationLength, out int ReturnLength);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern uint ZwQuerySystemInformation(SYSTEM_INFORMATION_CLASS SystemInformationClass,
            ref SYSTEM_CACHE_INFORMATION SystemInformation, int SystemInformationLength, out int ReturnLength);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern uint ZwQuerySystemInformation(SYSTEM_INFORMATION_CLASS SystemInformationClass,
            ref SYSTEM_PERFORMANCE_INFORMATION SystemInformation, int SystemInformationLength, out int ReturnLength);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern uint ZwQuerySystemInformation(SYSTEM_INFORMATION_CLASS SystemInformationClass,
            [MarshalAs(UnmanagedType.LPArray)] SYSTEM_PROCESSOR_PERFORMANCE_INFORMATION[] SystemInformation, 
            int SystemInformationLength, out int ReturnLength);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern uint ZwQuerySystemInformation(SYSTEM_INFORMATION_CLASS SystemInformationClass,
            IntPtr SystemInformation, int SystemInformationLength, out int ReturnLength);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern uint ZwSetSystemInformation(SYSTEM_INFORMATION_CLASS SystemInformationClass,
            ref SYSTEM_LOAD_AND_CALL_IMAGE SystemInformation, int SystemInformationLength);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern uint ZwQueryObject(int Handle, OBJECT_INFORMATION_CLASS ObjectInformationClass,
            IntPtr ObjectInformation, int ObjectInformationLength, out int ReturnLength);

        #endregion

        #region Windows

        [DllImport("user32.dll")]
        public static extern int EnumWindows([MarshalAs(UnmanagedType.FunctionPtr)] EnumWindowsProc Callback, int param);

        [DllImport("user32.dll")]
        public static extern int SetActiveWindow(int hWnd);

        #endregion
    }
}
