/*
 * Process Hacker - 
 *   windows API functions
 *                       
 * Copyright (C) 2009 Uday Shanbhag
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
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;

// you won't get some of this stuff from anywhere else... :)

namespace ProcessHacker.Native.Api
{
    public partial class Win32
    {
        #region Cryptography

        [DllImport("wintrust.dll", SetLastError = true)]
        public static extern bool CryptCATCatalogInfoFromContext(int CatInfoHandle,
            ref CatalogInfo CatInfo, int Flags);

        [DllImport("wintrust.dll", SetLastError = true)]
        public static extern int CryptCATAdminEnumCatalogFromHash(int CatAdminHandle,
            byte[] Hash, int HashSize, int Flags, int PrevCatInfoHandle);

        [DllImport("wintrust.dll", SetLastError = true)]
        public static extern bool CryptCATAdminAcquireContext(out int CatAdminHandle, ref Guid Subsystem,
            int Flags);

        [DllImport("wintrust.dll", SetLastError = true)]
        public static extern bool CryptCATAdminCalcHashFromFileHandle(int FileHandle, ref int HashSize,
            byte[] Hash, int Flags);

        [DllImport("wintrust.dll", SetLastError = true)]
        public static extern bool CryptCATAdminReleaseContext(int CatAdminHandle, int Flags);

        [DllImport("wintrust.dll", SetLastError = true)]
        public static extern bool CryptCATAdminReleaseCatalogContext(int CatAdminHandle, 
            int CatInfoHandle, int Flags);

        [DllImport("wintrust.dll", SetLastError = true)]
        public static extern uint WinVerifyTrust(int WindowHandle, ref Guid Action, ref WintrustData Data);

        #endregion

        #region Error Handling

        [DllImport("ntdll.dll")]
        public static extern int RtlNtStatusToDosError(int Status);

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

        #endregion

        #region Files

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int QueryDosDevice(string DeviceName, StringBuilder TargetPath, int MaxLength);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int CreateFile(string FileName, FileAccess DesiredAccess, FileShareMode ShareMode,
            int SecurityAttributes, FileCreationDisposition CreationDisposition, int FlagsAndAttributes,
            int TemplateFile);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadFile(int FileHandle, byte[] Buffer, int Bytes, out int ReadBytes, int Overlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteFile(int FileHandle, byte[] Buffer, int Bytes, out int WrittenBytes, int Overlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool DeviceIoControl(int FileHandle, int IoControlCode,
            byte[] InBuffer, int InBufferLength, byte[] OutBuffer, int OutBufferLength,
            out int BytesReturned, int Overlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        public unsafe static extern bool DeviceIoControl(int FileHandle, int IoControlCode,
            byte* InBuffer, int InBufferLength, byte* OutBuffer, int OutBufferLength,
            out int BytesReturned, int Overlapped);

        #endregion

        #region Jobs

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool TerminateJobObject(int JobHandle, int ExitCode);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool AssignProcessToJobObject(int JobHandle, int ProcessHandle);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int CreateJobObject(int SecurityAttributes, string Name);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int OpenJobObject(JobObjectAccess DesiredAccess, bool Inherit, string Name);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool QueryInformationJobObject(int JobHandle, JobObjectInformationClass JobInformationClass,
            IntPtr JobInformation, int JobInformationLength, out int ReturnLength);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool QueryInformationJobObject(int JobHandle, JobObjectInformationClass JobInformationClass,
            out JobObjectBasicUiRestrictions JobInformation, int JobInformationLength, out int ReturnLength);

        #endregion

        #region Kernel

        [DllImport("psapi.dll", SetLastError = true)]
        public static extern bool EnumDeviceDrivers(int[] ImageBases, int Size, out int Needed);

        [DllImport("psapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int GetDeviceDriverBaseName(int ImageBase, StringBuilder FileName, int Size);

        [DllImport("psapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int GetDeviceDriverFileName(int ImageBase, StringBuilder FileName, int Size);

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

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr LocalAlloc(AllocFlags Flags, int Bytes);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr LocalFree(IntPtr Memory);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetProcessHeaps(int NumberOfHeaps, int[] Heaps);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int HeapCompact(int Heap, bool NoSerialize);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int HeapFree(int Heap, int Flags, IntPtr Memory);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr HeapAlloc(int Heap, int Flags, int Bytes);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetProcessHeap();

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool VirtualQueryEx(int Process, int Address,
            [MarshalAs(UnmanagedType.Struct)] out MemoryBasicInformation Buffer, int Size);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool VirtualProtectEx(int Process, int Address, int Size, MemoryProtection NewProtect, out MemoryProtection OldProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int VirtualAllocEx(int Process, int Address, int Size, MemoryState Type, MemoryProtection Protect);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool VirtualFreeEx(int Process, int Address, int Size, MemoryState FreeType);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory(int Process, int BaseAddress, byte[] Buffer, int Size, out int BytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        public unsafe static extern bool ReadProcessMemory(int Process, int BaseAddress, void* Buffer, int Size, out int BytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(int Process, int BaseAddress, byte[] Buffer, int Size, out int BytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        public unsafe static extern bool WriteProcessMemory(int Process, int BaseAddress, void* Buffer, int Size, out int BytesWritten);

        #endregion

        #region Misc.

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool ExitWindowsEx(ExitWindowsFlags flags, int reason);

        [DllImport("powrprof.dll", SetLastError = true)]
        public static extern bool SetSuspendState(bool hibernate, bool forceCritical, bool disableWakeEvent);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool LockWorkStation();

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool QueryPerformanceFrequency(ref long PerformanceFrequency);

        [DllImport("kernel32.dll")]
        public static extern int GetTickCount();

        #endregion

        #region Named Pipes

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool DisconnectNamedPipe(int NamedPipe);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ConnectNamedPipe(int NamedPipe, int Overlapped);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int CreateNamedPipe(string Name, PipeAccessMode OpenMode, PipeMode PipeMode,
            int MaxInstances, int OutBufferSize, int InBufferSize, int DefaultTimeOut, int SecurityAttributes);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetNamedPipeClientProcessId(int NamedPipeHandle, out int ServerProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetNamedPipeHandleState(int NamedPipeHandle, out PipeState State,
            int CurInstances, int MaxCollectionCount, int CollectDataTimeout, int UserName, int MaxUserNameSize);

        #endregion

        #region Native API

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int NtAlertThread(int ThreadHandle);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int NtOpenSymbolicLinkObject(out int LinkHandle, int DesiredAccess,
            ref ObjectAttributes ObjectAttributes);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int NtQuerySymbolicLinkObject(int LinkHandle, ref UnicodeString LinkName,
            out int DataWritten);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int NtResumeProcess(int ProcessHandle);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int NtSuspendProcess(int ProcessHandle);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int NtQuerySection(int SectionHandle, SectionInformationClass SectionInformationClass,
            ref SectionBasicInformation SectionInformation, int SectionInformationLength, out int ReturnLength);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int NtQuerySection(int SectionHandle, SectionInformationClass SectionInformationClass,
            ref SectionImageInformation SectionInformation, int SectionInformationLength, out int ReturnLength);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int NtQueryMutant(int MutantHandle, MutantInformationClass MutantInformationClass,
            ref MutantBasicInformation MutantInformation, int MutantInformationLength, out int ReturnLength);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int NtQueryEvent(int EventHandle, EventInformationClass EventInformationClass,
            ref EventBasicInformation EventInformation, int EventInformationLength, out int ReturnLength);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int NtSetInformationThread(int ThreadHandle, ThreadInformationClass ThreadInformationClass,
            ref int ThreadInformation, int ThreadInformationLength);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int NtQueryInformationThread(int ThreadHandle, ThreadInformationClass ThreadInformationClass,
            ref ThreadBasicInformation ThreadInformation, int ThreadInformationLength, out int ReturnLength);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int NtQueryInformationThread(int ThreadHandle, ThreadInformationClass ThreadInformationClass,
            out long ThreadInformation, int ThreadInformationLength, out int ReturnLength);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int NtQueryInformationThread(int ThreadHandle, ThreadInformationClass ThreadInformationClass,
            out int ThreadInformation, int ThreadInformationLength, out int ReturnLength);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int NtQueryInformationThread(int ThreadHandle, ThreadInformationClass ThreadInformationClass,
            int[] ThreadInformation, int ThreadInformationLength, out int ReturnLength);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int NtQueryInformationThread(int ThreadHandle, ThreadInformationClass ThreadInformationClass,
            out uint ThreadInformation, int ThreadInformationLength, out int ReturnLength);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int NtQueryInformationProcess(int ProcessHandle, ProcessInformationClass ProcessInformationClass,
            IntPtr ProcessInformation, int ProcessInformationLength, out int ReturnLength);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int NtQueryInformationProcess(int ProcessHandle, ProcessInformationClass ProcessInformationClass,
            out int ProcessInformation, int ProcessInformationLength, out int ReturnLength);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int NtQueryInformationProcess(int ProcessHandle, ProcessInformationClass ProcessInformationClass,
            out PooledUsageAndLimits ProcessInformation, int ProcessInformationLength, out int ReturnLength);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int NtQueryInformationProcess(int ProcessHandle, ProcessInformationClass ProcessInformationClass,
            out QuotaLimits ProcessInformation, int ProcessInformationLength, out int ReturnLength);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int NtQueryInformationProcess(int ProcessHandle, ProcessInformationClass ProcessInformationClass,
            out UnicodeString ProcessInformation, int ProcessInformationLength, out int ReturnLength);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int NtQueryInformationProcess(int ProcessHandle, ProcessInformationClass ProcessInformationClass,
            out ProcessBasicInformation ProcessInformation, int ProcessInformationLength, out int ReturnLength);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int NtQueryInformationProcess(int ProcessHandle, ProcessInformationClass ProcessInformationClass,
            out MemExecuteOptions ProcessInformation, int ProcessInformationLength, out int ReturnLength);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int NtDuplicateObject(int SourceProcessHandle, int SourceHandle,
            int TargetProcessHandle, int TargetHandle, int DesiredAccess, int Attributes, int Options);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int NtDuplicateObject(int SourceProcessHandle, int SourceHandle,
            int TargetProcessHandle, out int TargetHandle, int DesiredAccess, int Attributes, int Options);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int NtQuerySystemInformation(SystemInformationClass SystemInformationClass,
            ref SystemBasicInformation SystemInformation, int SystemInformationLength, out int ReturnLength);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int NtQuerySystemInformation(SystemInformationClass SystemInformationClass,
            ref SystemCacheInformation SystemInformation, int SystemInformationLength, out int ReturnLength);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int NtQuerySystemInformation(SystemInformationClass SystemInformationClass,
            ref SystemPerformanceInformation SystemInformation, int SystemInformationLength, out int ReturnLength);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int NtQuerySystemInformation(SystemInformationClass SystemInformationClass,
            [MarshalAs(UnmanagedType.LPArray)] SystemProcessorPerformanceInformation[] SystemInformation,
            int SystemInformationLength, out int ReturnLength);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int NtQuerySystemInformation(SystemInformationClass SystemInformationClass,
            IntPtr SystemInformation, int SystemInformationLength, out int ReturnLength);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int NtSetSystemInformation(SystemInformationClass SystemInformationClass,
            ref SystemLoadAndCallImage SystemInformation, int SystemInformationLength);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int NtQueryObject(int Handle, ObjectInformationClass ObjectInformationClass,
            IntPtr ObjectInformation, int ObjectInformationLength, out int ReturnLength);

        #endregion

        #region Processes

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern void ExitProcess(int ExitCode);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool QueryProcessCycleTime(int ProcessHandle, out ulong CycleTime);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetPriorityClass(int ProcessHandle, int Priority);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetPriorityClass(int ProcessHandle);

        [DllImport("psapi.dll", SetLastError = true)]
        public static extern bool EmptyWorkingSet(int ProcessHandle);

        [DllImport("psapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int GetMappedFileName(
            int ProcessHandle,
            int Address,
            StringBuilder Buffer,
            int Size
            );

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool CreateProcessWithTokenW(
            int TokenHandle,
            LogonFlags Flags,
            string ApplicationName,
            string CommandLine,
            CreationFlags CreationFlags,
            int Environment,
            string CurrentDirectory,
            ref StartupInfo StartupInfo,
            ref ProcessInformation ProcessInfo
            );

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool CreateProcessAsUser(
            int TokenHandle,
            string ApplicationName,
            string CommandLine,
            int ProcessAttributes,
            int ThreadAttributes,
            bool InheritHandles,
            CreationFlags CreationFlags,
            int Environment,
            string CurrentDirectory,
            ref StartupInfo StartupInfo,
            ref ProcessInformation ProcessInformation
            );

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool CreateProcess(
            string ApplicationName,
            string CommandLine,
            int ProcessAttributes,
            int ThreadAttributes,
            bool InheritHandles,
            CreationFlags CreationFlags,
            int Environment,
            string CurrentDirectory,
            ref StartupInfo StartupInfo,
            ref ProcessInformation ProcessInformation
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetExitCodeProcess(int ProcessHandle, out int ExitCode);

        // Vista and higher
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool QueryFullProcessImageName(int ProcessHandle, bool UseNativeName,
            StringBuilder ExeName, ref int Size);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool IsProcessInJob(int ProcessHandle, int JobHandle, out int Result);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetProcessAffinityMask(int ProcessHandle, uint ProcessAffinityMask);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetProcessAffinityMask(int ProcessHandle, out uint ProcessAffinityMask,
            out uint SystemAffinityMask);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CheckRemoteDebuggerPresent(int ProcessHandle, out int DebuggerPresent);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetProcessId(int ProcessHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetCurrentProcess();

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetCurrentProcessId();

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetProcessDEPPolicy(int ProcessHandle, out DepFlags Flags, out int Permanent);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool TerminateProcess(int ProcessHandle, int ExitCode);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int OpenProcess(ProcessAccess DesiredAccess, int InheritHandle, int ProcessId);

        [DllImport("kernel32.dll")]
        public static extern bool DebugActiveProcess(int PID);

        [DllImport("kernel32.dll")]
        public static extern bool DebugActiveProcessStop(int PID);

        [DllImport("psapi.dll")]
        public static extern bool EnumProcessModules(int ProcessHandle, IntPtr[] ModuleHandles, int Size, out int RequiredSize);

        [DllImport("psapi.dll", CharSet = CharSet.Unicode)]
        public static extern int GetModuleBaseName(int ProcessHandle, IntPtr ModuleHandle, StringBuilder BaseName, int Size);

        [DllImport("psapi.dll", CharSet = CharSet.Unicode)]
        public static extern int GetModuleFileNameEx(int ProcessHandle, IntPtr ModuleHandle, StringBuilder FileName, int Size);

        [DllImport("psapi.dll")]
        public static extern bool GetModuleInformation(int ProcessHandle, IntPtr ModuleHandle, ref ModuleInfo ModInfo, int Size);

        #endregion

        #region Resources/Handles

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int CreateMutex(int attributes, bool initialOwner, string name);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetHandleInformation(int handle, HandleFlags mask, HandleFlags flags);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetHandleInformation(int handle, out HandleFlags flags);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(int Handle);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern WaitResult WaitForSingleObject(int Object, uint Timeout);

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
            UnicodeString[] UserRights, uint CountOfRights);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern int LsaOpenPolicy(int SystemName, ref ObjectAttributes ObjectAttributes,
            PolicyAccess DesiredAccess, out int PolicyHandle);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern int LsaClose(int Handle);

        #endregion

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool ImpersonateLoggedOnUser(int TokenHandle);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool RevertToSelf();

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool LogonUser(
            string Username,
            string Domain,
            string Password,
            LogonType LogonType,
            LogonProvider LogonProvider,
            out int TokenHandle
            );

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool OpenProcessToken(int ProcessHandle, TokenAccess DesiredAccess,
            out int TokenHandle);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool OpenThreadToken(int ThreadHandle, TokenAccess DesiredAccess,
            bool OpenAsSelf, out int TokenHandle);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool DuplicateTokenEx(int ExistingToken, TokenAccess DesiredAccess,
            int TokenAttributes, SecurityImpersonationLevel ImpersonationLevel, TokenType TokenType,
            out int NewToken);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool SetTokenInformation(int TokenHandle,
            TokenInformationClass TokenInformationClass, ref int TokenInformation,
            int TokenInformationLength);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool GetTokenInformation(int TokenHandle,
            TokenInformationClass TokenInformationClass, IntPtr TokenInformation,
            int TokenInformationLength, out int ReturnLength);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool GetTokenInformation(int TokenHandle,
            TokenInformationClass TokenInformationClass, out int TokenInformation,
            int TokenInformationLength, out int ReturnLength);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool GetTokenInformation(int TokenHandle,
            TokenInformationClass TokenInformationClass, ref TokenSource TokenInformation,
            int TokenInformationLength, out int ReturnLength);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool LookupAccountName(
            string SystemName,
            string AccountName,
            IntPtr SID,
            out int SIDSize,
            int ReferencedDomainName,
            int ReferencedDomainNameSize,
            out SidNameUse Use
            );

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool LookupAccountSid(string SystemName,
            int SID, StringBuilder Name, out int NameSize,
            StringBuilder ReferencedDomainName, out int ReferencedDomainNameSize,
            out SidNameUse Use);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool LookupPrivilegeDisplayName(int SystemName, string Name,
            StringBuilder DisplayName, out int DisplayNameSize, out int LanguageId);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool LookupPrivilegeName(int SystemName, ref Luid Luid,
            StringBuilder Name, out int RequiredSize);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool LookupPrivilegeValue(string SystemName, string PrivilegeName, ref Luid Luid);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool AdjustTokenPrivileges(int TokenHandle, int DisableAllPrivileges,
            ref TokenPrivileges NewState, int BufferLength,
            int PreviousState, int ReturnLength);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool InitializeSecurityDescriptor(IntPtr SecurityDescriptor, int Revision);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool SetSecurityDescriptorDacl(IntPtr SecurityDescriptor,
            [MarshalAs(UnmanagedType.Bool)] bool DaclPresent,
            int Dacl,
            [MarshalAs(UnmanagedType.Bool)] bool DaclDefaulted
            );

        #endregion      

        #region Services

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool CloseServiceHandle(int ServiceHandle);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool StartService(int Service, int NumServiceArgs, int Args);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool ChangeServiceConfig(
            int Service,
            ServiceType ServiceType, 
            ServiceStartType StartType,
            ServiceErrorControl ErrorControl, 
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
            ServiceControl Control, 
            ref ServiceStatus ServiceStatus
            );

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int CreateService(int SCManager,
            string ServiceName,
            string DisplayName,
            ServiceAccess DesiredAccess, 
            ServiceType ServiceType,
            ServiceStartType StartType,
            ServiceErrorControl ErrorControl,
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
            ref ServiceStatus ServiceStatus
            );

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool QueryServiceStatusEx(int Service, int InfoLevel,
            ref ServiceStatusProcess ServiceStatus, int BufSize, out int BytesNeeded);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool QueryServiceConfig(int Service,
            IntPtr ServiceConfig,
            int BufSize, ref int BytesNeeded);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool QueryServiceConfig2(int Service,
            ServiceInfoLevel InfoLevel, IntPtr Buffer, int BufferSize, out int ReturnLength);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int OpenService(int SCManager,
            string ServiceName, ServiceAccess DesiredAccess);

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
            ServiceQueryType ServiceType, ServiceQueryState ServiceState,
            IntPtr Services, int BufSize, out int BytesNeeded, out int ServicesReturned,
            out int ResumeHandle, int GroupName);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern int OpenSCManager(int MachineName, int DatabaseName,
            ScManagerAccess DesiredAccess);

        #endregion

        #region Shell

        [DllImport("shell32.dll", EntryPoint = "#61", CharSet = CharSet.Unicode)]
        public static extern int RunFileDlg(IntPtr hWnd, int unknown, int unknown2,
            string title, string prompt, int flags);

        [DllImport("shell32.dll")]
        public static extern bool ShellExecuteEx(
            [MarshalAs(UnmanagedType.Struct)] ref ShellExecuteInfo s);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int SetWindowsHookEx(int HookId, int HookFunction, int Module, int ThreadId);

        [DllImport("shell32.dll")]
        public extern static int ExtractIconEx(string libName, int iconIndex,
        IntPtr[] largeIcon, IntPtr[] smallIcon, int nIcons);

        [DllImport("shell32.dll")]
        public static extern int SHGetFileInfo(string pszPath,
                                    uint dwFileAttributes,
                                    ref ShFileInfo psfi,
                                    uint cbSizeFileInfo,
                                    uint uFlags);

        [DllImport("shell32.dll", EntryPoint = "#660")]
        public static extern bool FileIconInit(bool RestoreCache);

        #endregion

        #region Statistics

        [DllImport("psapi.dll", SetLastError = true)]
        public static extern bool GetPerformanceInfo(ref PerformanceInformation PerformanceInformation,
            int Size);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetProcessTimes(int ProcessHandle, out long CreationTime, out long ExitTime,
            out long KernelTime, out long UserTime);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetProcessIoCounters(int ProcessHandle, out IoCounters IoCounters);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetSystemTimes(out ulong IdleTime, out ulong KernelTime, out ulong UserTime);

        [DllImport("kernel32.dll")]
        public static extern bool GetThreadTimes(int hThread, out long lpCreationTime,
           out long lpExitTime, out long lpKernelTime, out long lpUserTime);

        #endregion

        #region Symbols/Stack Walking

        [DllImport("dbghelp.dll", SetLastError = true, CharSet = CharSet.Ansi)]
        public static extern bool SymInitialize(int ProcessHandle, string UserSearchPath, bool InvadeProcess);

        [DllImport("dbghelp.dll", SetLastError = true)]
        public static extern bool SymCleanup(int ProcessHandle);

        [DllImport("dbghelp.dll", SetLastError = true)]
        public static extern SymbolOptions SymGetOptions();

        [DllImport("dbghelp.dll", SetLastError = true)]
        public static extern SymbolOptions SymSetOptions(SymbolOptions SymOptions);

        [DllImport("dbghelp.dll", SetLastError = true, CharSet = CharSet.Ansi)]
        public static extern bool SymGetSearchPath(int ProcessHandle, IntPtr SearchPath, int SearchPathLength);

        [DllImport("dbghelp.dll", SetLastError = true, CharSet = CharSet.Ansi)]
        public static extern bool SymSetSearchPath(int ProcessHandle, string SearchPath);

        [DllImport("dbghelp.dll", SetLastError = true, CharSet = CharSet.Ansi)]
        public static extern long SymLoadModule64(
            int ProcessHandle,
            int FileHandle,
            string ImageName,
            string ModuleName,
            long BaseOfDll,
            int SizeOfDll
            );

        [DllImport("dbghelp.dll", SetLastError = true)]
        public static extern bool SymUnloadModule64(int ProcessHandle, long BaseOfDll);

        [DllImport("dbghelp.dll", SetLastError = true)]
        public static extern int SymFunctionTableAccess64(int ProcessHandle, long AddrBase);

        [DllImport("dbghelp.dll", SetLastError = true)]
        public static extern long SymGetModuleBase64(int ProcessHandle, long Address);

        [DllImport("dbghelp.dll", SetLastError = true)]
        public static extern int SymEnumSymbols(
            int ProcessHandle,
            int BaseOfDll,
            string Mask,
            SymEnumSymbolsProc EnumSymbolsCallback, int UserContext);

        [DllImport("dbghelp.dll", SetLastError = true)]
        public static extern bool SymFromAddr(
            int ProcessHandle,
            long Address,
            out long Displacement,
            IntPtr Symbol);

        [DllImport("dbghelp.dll", SetLastError = true)]
        public static extern bool SymFromIndex(
            int ProcessHandle,
            int BaseOfDll,
            int Index,
            IntPtr Symbol
            );

        [DllImport("dbghelp.dll", SetLastError = true)]
        public static extern bool StackWalk64(
            MachineType MachineType,
            int ProcessHandle,
            int ThreadHandle,
            ref StackFrame64 StackFrame,
            ref Context ContextRecord,
            ReadProcessMemoryProc64 ReadMemoryRoutine,
            FunctionTableAccessProc64 FunctionTableAccessRoutine,
            GetModuleBaseProc64 GetModuleBaseRoutine,
            int TranslateAddress
            );

        [DllImport("symsrv.dll", SetLastError = true)]
        public static extern bool SymbolServerSetOptions(
            SymbolServerOption Options,
            long Data
            );

        #endregion

        #region TCP

        [DllImport("iphlpapi.dll", SetLastError = true)]
        public extern static int SetTcpEntry(ref MibTcpRow TcpRow);

        [DllImport("iphlpapi.dll", SetLastError = true)]
        public extern static int GetExtendedTcpTable(IntPtr Table, ref int Size,
            bool Order, int IpVersion, // 2 for IPv4
            TcpTableClass TableClass, int Reserved);

        [DllImport("iphlpapi.dll", SetLastError = true)]
        public extern static int GetTcpStatistics(ref MibTcpStats pStats);

        [DllImport("iphlpapi.dll", SetLastError = true)]
        public static extern int GetTcpTable(byte[] tcpTable, out int pdwSize, bool bOrder);

        [DllImport("iphlpapi.dll", SetLastError = true)]
        public extern static int AllocateAndGetTcpExTableFromStack(ref IntPtr pTable, bool bOrder, IntPtr heap, int zero, int flags);

        #endregion

        #region Terminal Server

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ProcessIdToSessionId(int ProcessId, out int SessionId);

        [DllImport("wtsapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool WTSQuerySessionInformation(int ServerHandle, int SessionID,
            WtsInformationClass InfoClass,
            out string Buffer,
            out int BytesReturned);

        [DllImport("wtsapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool WTSQuerySessionInformation(int ServerHandle, int SessionID,
            WtsInformationClass InfoClass,
            out int Buffer,
            out int BytesReturned);

        [DllImport("wtsapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool WTSQuerySessionInformation(int ServerHandle, int SessionID,
            WtsInformationClass InfoClass,
            out ushort Buffer,
            out int BytesReturned);

        [DllImport("wtsapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool WTSQuerySessionInformation(int ServerHandle, int SessionID,
            WtsInformationClass InfoClass,
            out WtsClientDisplay[] Buffer,
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
        public static extern bool QueryThreadCycleTime(int ThreadHandle, out ulong CycleTime);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool QueueUserAPC(int APC, int ThreadHandle, int Data);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetExitCodeThread(int ThreadHandle, out int ExitCode);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetThreadPriority(int ThreadHandle, int Priority);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetThreadPriority(int ThreadHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int CreateThread(int ThreadAttributes, int StackSize,
            [MarshalAs(UnmanagedType.FunctionPtr)] ThreadStart StartAddress,
            int Parameter, int CreationFlags, out int ThreadId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetProcessIdOfThread(int ThreadHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetThreadId(int ThreadHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int OpenThread(ThreadAccess DesiredAccess, int InheritHandle, int ThreadId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool TerminateThread(int ThreadHandle, int ExitCode);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int SuspendThread(int ThreadHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int ResumeThread(int ThreadHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetThreadContext(int ThreadHandle, ref Context Context);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetThreadContext(int ThreadHandle, ref Context Context);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CreateRemoteThread(int ProcessHandle, int ThreadAttributes,
            int StackSize, int StartAddress, int Parameter, int CreationFlags, out int ThreadId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetCurrentThreadId();

        #endregion

        #region Toolhelp

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int CreateToolhelp32Snapshot(SnapshotFlags dwFlags, int th32ProcessID);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int Process32First(int hSnapshot,
            [MarshalAs(UnmanagedType.Struct)] ref ProcessEntry32 lppe);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int Process32Next(int hSnapshot,
            [MarshalAs(UnmanagedType.Struct)] ref ProcessEntry32 lppe);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int Thread32First(int hSnapshot,
            [MarshalAs(UnmanagedType.Struct)] ref ThreadEntry32 lppe);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int Thread32Next(int hSnapshot,
            [MarshalAs(UnmanagedType.Struct)] ref ThreadEntry32 lppe);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int Module32First(int hSnapshot,
            [MarshalAs(UnmanagedType.Struct)] ref ModuleEntry32 lppe);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int Module32Next(int hSnapshot,
            [MarshalAs(UnmanagedType.Struct)] ref ModuleEntry32 lppe);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int Heap32ListFirst(int hSnapshot,
            [MarshalAs(UnmanagedType.Struct)] ref HeapList32 lppe);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int Heap32ListNext(int hSnapshot,
            [MarshalAs(UnmanagedType.Struct)] ref HeapList32 lppe);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int Heap32First([MarshalAs(UnmanagedType.Struct)] ref HeapEntry32 lppe,
            int ProcessID, int HeapID);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int Heap32Next([MarshalAs(UnmanagedType.Struct)] ref HeapEntry32 lppe);

        #endregion

        #region UDP

        [DllImport("iphlpapi.dll", SetLastError = true)]
        public extern static int GetExtendedUdpTable(IntPtr Table, ref int Size,
            bool Order, int IpVersion, // 2 for IPv4
            UdpTableClass TableClass, int Reserved);

        [DllImport("iphlpapi.dll", SetLastError = true)]
        public static extern int GetUdpStatistics(ref MibUdpStats pStats);

        [DllImport("iphlpapi.dll", SetLastError = true)]
        public static extern int GetUdpTable(byte[] udpTable, out int pdwSize, bool bOrder);

        [DllImport("iphlpapi.dll", SetLastError = true)]
        public extern static int AllocateAndGetUdpExTableFromStack(ref IntPtr pTable, bool bOrder, IntPtr heap, int zero, int flags);

        #endregion

        #region User

        [DllImport("user32.dll")]
        public static extern bool SetUserObjectSecurity(
            int Handle,
            ref SiRequested SiRequested,
            IntPtr Sid
            );

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int OpenDesktop(
            string Desktop,
            int Flags,
            bool Inherit,
            DesktopAccess DesiredAccess
            );

        [DllImport("user32.dll")]
        public static extern bool CloseDesktop(int Handle);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int OpenWindowStation(
            string WinSta,
            bool Inherit,
            WindowStationAccess DesiredAccess
            );

        [DllImport("user32.dll")]
        public static extern bool CloseWindowStation(int Handle);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetThreadDesktop(int ThreadId);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetThreadDesktop(int DesktopHandle);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetProcessWindowStation();

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetProcessWindowStation(int WindowStationHandle);

        [DllImport("userenv.dll", SetLastError = true)]
        public static extern bool CreateEnvironmentBlock(
            out int Environment,
            int TokenHandle,
            bool Inherit
            );

        [DllImport("userenv.dll", SetLastError = true)]
        static extern bool LoadUserProfile(
            int TokenHandle,
            ref ProfileInformation ProfileInfo
            );

        [DllImport("userenv.dll", SetLastError = true)]
        static extern bool UnloadUserProfile(
            int TokenHandle,
            int ProfileHandle
            );

        #endregion

        #region Windows

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out Point location);

        [DllImport("user32.dll")]
        public static extern bool ChangeWindowMessageFilter(WindowMessage message, UipiFilterFlag flag);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr windowHandle, WindowMessage msg, int w, int l);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int SendMessageTimeout(IntPtr windowHandle, WindowMessage msg, int w, int l, 
            SmtoFlags flags, int timeout, out int result);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int PostMessage(IntPtr windowHandle, WindowMessage msg, int w, int l);

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool AllowSetForegroundWindow(int processId);

        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
        public static extern int SetWindowTheme(IntPtr hWnd, string appName, string idList);

        [DllImport("user32.dll")]
        public static extern int GetGuiResources(int ProcessHandle, bool UserObjects);

        [DllImport("user32.dll")]
        public static extern bool DestroyIcon(IntPtr Handle);

        [DllImport("user32.dll")]
        public static extern bool BringWindowToTop(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern int EnumWindows([MarshalAs(UnmanagedType.FunctionPtr)] EnumWindowsProc Callback, int param);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool EnumThreadWindows(
            int threadId,
            [MarshalAs(UnmanagedType.FunctionPtr)] EnumThreadWndProc callback,
            int param
            );

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool EnumChildWindows(
            IntPtr hWnd,
            [MarshalAs(UnmanagedType.FunctionPtr)] EnumChildProc callback,
            int param
            );

        [DllImport("user32.dll")]
        public static extern int GetWindowThreadProcessId(IntPtr hWnd, out int processId);

        [DllImport("user32.dll")]
        public static extern int SetActiveWindow(int hWnd);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool PeekMessage(out Message msg, IntPtr hWnd, uint messageFilterMin, uint messageFilterMax, PeekMessageFlags flags);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool TranslateMessage(ref Message msg);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool DispatchMessage(ref Message msg);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr DefWindowProc(IntPtr hWnd, WindowMessage msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern void PostQuitMessage(int exitCode);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
#if(_WIN64)
		private static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int index, [MarshalAs(UnmanagedType.FunctionPtr)] WndProcDelegate windowCallback);
#else
        private static extern IntPtr SetWindowLong(IntPtr hWnd, int index, [MarshalAs(UnmanagedType.FunctionPtr)] WndProcDelegate windowCallback);
#endif

        [DllImport("user32.dll", SetLastError = true, EntryPoint = "SetWindowLong", CharSet = CharSet.Auto)]
        public static extern IntPtr SetWindowLongStyle(IntPtr hWnd, int index, WindowStyles style);

        [DllImport("user32.dll", SetLastError = true, EntryPoint = "GetWindowLong", CharSet = CharSet.Auto)]
        public static extern WindowStyles GetWindowLongStyle(IntPtr hWnd, int index);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool GetClientRect(IntPtr hWnd, out Rectangle rect);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool GetWindowRect(IntPtr hWnd, out Rectangle rect);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndAfter, int x, int y, int w, int h, uint flags);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool ScreenToClient(IntPtr hWnd, ref Point rect);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr SetFocus(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool GetMonitorInfo(IntPtr hWnd, ref MonitorInformation info);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr MonitorFromWindow(IntPtr hWnd, uint flags);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern short GetAsyncKeyState(uint key);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr SetCapture(IntPtr handle);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool ShowWindow(IntPtr hWnd, ShowWindowType flags);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool SetMenu(IntPtr hWnd, IntPtr menuHandle);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool CloseWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool DestroyWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool IsIconic(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool AdjustWindowRect(ref Rectangle rect, WindowStyles style,
            [MarshalAs(UnmanagedType.Bool)]bool menu);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr RegisterClass(ref WindowClass wndClass);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool UnregisterClass([MarshalAs(UnmanagedType.LPTStr)] string className, IntPtr instanceHandle);

        [DllImport("user32.dll", SetLastError = true, EntryPoint = "CreateWindowEx", CharSet = CharSet.Auto)]
        public static extern IntPtr CreateWindow(int exStyle, [MarshalAs(UnmanagedType.LPTStr)] string className, [MarshalAs(UnmanagedType.LPTStr)] string windowName,
            WindowStyles style, int x, int y, int width, int height, IntPtr parent, IntPtr menuHandle, IntPtr instanceHandle, IntPtr zero);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetCaretBlinkTime();

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int InternalGetWindowText(IntPtr hWnd, StringBuilder str, int maxCount);

        #endregion
    }
}
