/*
 * Process Hacker - 
 *   windows API functions
 *                       
 * Copyright (C) 2009 Flavio Erlich
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
        public static extern bool CryptCATCatalogInfoFromContext(
            [In] IntPtr CatInfoHandle,
            [Out] out CatalogInfo CatInfo,
            [In] int Flags
        );

        [DllImport("wintrust.dll", SetLastError = true)]
        public static extern IntPtr CryptCATAdminEnumCatalogFromHash(
            [In] IntPtr CatAdminHandle,
            [In] byte[] Hash,
            [In] int HashSize,
            [In] int Flags,
            [In] IntPtr PrevCatInfoHandle
        );

        [DllImport("wintrust.dll", SetLastError = true)]
        public static extern bool CryptCATAdminAcquireContext(
            [Out] out IntPtr CatAdminHandle,
            [In] [MarshalAs(UnmanagedType.LPStruct)] Guid Subsystem,
            [In] int Flags
        );

        [DllImport("wintrust.dll", SetLastError = true)]
        public static extern bool CryptCATAdminCalcHashFromFileHandle(
            [In] IntPtr FileHandle,
            ref int HashSize,
            [In] byte[] Hash,
            [In] int Flags
        );

        [DllImport("wintrust.dll", SetLastError = true)]
        public static extern bool CryptCATAdminReleaseContext(
            [In] IntPtr CatAdminHandle,
            [In] int Flags
        );

        [DllImport("wintrust.dll", SetLastError = true)]
        public static extern bool CryptCATAdminReleaseCatalogContext(
            [In] IntPtr CatAdminHandle, 
            [In] IntPtr CatInfoHandle,
            [In] int Flags
        );

        [DllImport("wintrust.dll", SetLastError = true)]
        public static extern uint WinVerifyTrust(
             [In] IntPtr hWnd,
             [In] [MarshalAs(UnmanagedType.LPStruct)] Guid ActionId,
             [In] ref WintrustData WintrustData
        );

        #endregion

        #region Error Handling

        [DllImport("ntdll.dll")]
        public static extern int RtlNtStatusToDosError([In] int Status);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int FormatMessage(
            [In] int Flags,
            [In] [Optional] IntPtr Source,
            [In] int MessageId,
            [In] int LanguageId,
            [Out] StringBuilder Buffer,
            [In] int Size,
            [In] [Optional] IntPtr Arguments
            );

        #endregion

        #region Files

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int QueryDosDevice(
            [In] [Optional] string DeviceName,
            [Out] StringBuilder TargetPath,
            [In] int MaxLength
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateFile(
            [In] string FileName, 
            [In] FileAccess DesiredAccess,
            [In] FileShareMode ShareMode,
            [In] [Optional] int SecurityAttributes,
            [In] FileCreationDisposition CreationDisposition,
            [In] int FlagsAndAttributes,
            [In] [Optional] IntPtr TemplateFile
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadFile(
            [In] IntPtr FileHandle,
            [Out] byte[] Buffer,
            [In] int Bytes, 
            [Out] [Optional] out int ReadBytes,
            [Optional] IntPtr Overlapped
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteFile(
            [In] IntPtr FileHandle, 
            [Out] byte[] Buffer,
            [In] int Bytes, 
            [Out] [Optional] out int WrittenBytes,
            [Optional] IntPtr Overlapped
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool DeviceIoControl(
            [In] IntPtr FileHandle,
            [In] int IoControlCode,
            [In] [Optional] byte[] InBuffer,
            [In] int InBufferLength, 
            [Out] [Optional] byte[] OutBuffer,
            [In] int OutBufferLength,
            [Out] [Optional]out int BytesReturned,
            [Optional] IntPtr Overlapped
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public unsafe static extern bool DeviceIoControl(
            [In] IntPtr FileHandle,
            [In] int IoControlCode,
            [In] [Optional] byte* InBuffer,
            [In] int InBufferLength, 
            [Out] [Optional] byte* OutBuffer,
            [In] int OutBufferLength,
            [Out] [Optional] out int BytesReturned,
            [Optional] IntPtr Overlapped
            );

        #endregion

        #region Jobs

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool TerminateJobObject(
            [In] IntPtr JobHandle,
            [In] int ExitCode
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool AssignProcessToJobObject(
            [In] IntPtr JobHandle,
            [In] IntPtr ProcessHandle
            );

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr CreateJobObject(
            [In] [Optional] IntPtr SecurityAttributes,
            [In] [Optional] string Name
            );

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr OpenJobObject(
            [In] JobObjectAccess DesiredAccess, 
            [In] bool Inherit, 
            [In] string Name
            );

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool QueryInformationJobObject(
            [In] [Optional] IntPtr JobHandle,
            [In] JobObjectInformationClass JobInformationClass,
            [Out] IntPtr JobInformation, 
            [In] int JobInformationLength, 
            [Out] [Optional] out int ReturnLength
            );

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool QueryInformationJobObject(
            [In] [Optional] IntPtr JobHandle, 
            [In] JobObjectInformationClass JobInformationClass,
            [Out] out JobObjectBasicUiRestrictions JobInformation,
            [In] int JobInformationLength,
            [Out] [Optional] out int ReturnLength
            );

        #endregion

        #region Kernel

        [DllImport("psapi.dll", SetLastError = true)]
        public static extern bool EnumDeviceDrivers(
            [Out] IntPtr[] ImageBases,
            [In] int Size,
            [Out] out int Needed
            );

        [DllImport("psapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int GetDeviceDriverBaseName(
            [In] IntPtr ImageBase, 
            [Out] StringBuilder FileName, 
            [In] int Size
            );

        [DllImport("psapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int GetDeviceDriverFileName(
            [In] IntPtr ImageBase, 
            [Out] StringBuilder FileName, 
            [In] int Size
            );

        #endregion

        #region Libraries

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr LoadLibrary(
            [In] string FileName
            );

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr LoadLibraryEx(
            [In] string FileName, 
            IntPtr File,
            [In] int Flags
            );

        [DllImport("kernel32.dll")]
        public static extern bool FreeLibrary(
            [In] IntPtr Handle
            );

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr GetModuleHandle(
            [In] [Optional] string ModuleName
            );

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr GetProcAddress(
            [In] IntPtr Module, 
            [In] string ProcName
            );

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr GetProcAddress(
            [In] IntPtr Module, 
            [In] IntPtr ProcOrdinal
            );

        #endregion

        #region Memory

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr LocalAlloc(
            [In] AllocFlags Flags, 
            [In] int Bytes
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr LocalFree(
            [In] IntPtr Memory
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetProcessHeaps(
            [In] int NumberOfHeaps, 
            [Out] IntPtr[] Heaps
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int HeapCompact(
            [In] IntPtr Heap, 
            [In] bool NoSerialize
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool HeapFree(
            [In] IntPtr Heap, 
            [In] int Flags, 
            [In] IntPtr Memory
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr HeapAlloc(
            [In] IntPtr Heap, 
            [In] int Flags, 
            [In] int Bytes
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetProcessHeap();

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int VirtualQueryEx(
            [In] IntPtr Process, 
            [In] [Optional] IntPtr Address,
            [Out] [MarshalAs(UnmanagedType.Struct)] out MemoryBasicInformation Buffer,
            [In] int Size
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool VirtualProtectEx(
            [In] IntPtr Process,
            [In] IntPtr Address,
            [In] int Size,
            [In] MemoryProtection NewProtect, 
            [Out] out MemoryProtection OldProtect
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr VirtualAllocEx(
            [In] IntPtr Process,
            [In] [Optional] IntPtr Address,
            [In] int Size,
            [In] MemoryState Type,
            [In] MemoryProtection Protect
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool VirtualFreeEx(
            [In] IntPtr Process,
            [In] IntPtr Address,
            [In] int Size,
            [In] MemoryState FreeType
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ReadProcessMemory(
            [In] IntPtr Process,
            [In] IntPtr BaseAddress,
            [Out] byte[] Buffer,
            [In] int Size, 
            [Out] out int BytesRead
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public unsafe static extern bool ReadProcessMemory(
            [In] IntPtr Process, 
            [In] IntPtr BaseAddress,
            [Out] void* Buffer, 
            [In] int Size, 
            [Out] out int BytesRead
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool WriteProcessMemory(
            [In] IntPtr Process,
            [In] IntPtr BaseAddress,
            [In] byte[] Buffer,
            [In] int Size, 
            [Out] out int BytesWritten
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public unsafe static extern bool WriteProcessMemory(
            [In] IntPtr Process,
            [In] IntPtr BaseAddress,
            [In] void* Buffer,
            [In] int Size, 
            [Out] out int BytesWritten
            );

        #endregion

        #region Misc.

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ExitWindowsEx(
            [In] ExitWindowsFlags flags,
            [In] int reason
            );

        [DllImport("powrprof.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetSuspendState(
            [In] bool hibernate,
            [In] bool forceCritical,
            [In] bool disableWakeEvent
            );

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool LockWorkStation();

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool QueryPerformanceFrequency(
            [Out] out long PerformanceFrequency
            );

        [DllImport("kernel32.dll")]
        public static extern int GetTickCount();

        #endregion

        #region Named Pipes

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DisconnectNamedPipe(
            [In] IntPtr NamedPipe
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ConnectNamedPipe(
            [In] IntPtr NamedPipe, 
            [Optional] IntPtr Overlapped
            );

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr CreateNamedPipe(
            [In] string Name,
            [In] PipeAccessMode OpenMode,
            [In] PipeMode PipeMode,
            [In] int MaxInstances,
            [In] int OutBufferSize,
            [In] int InBufferSize,
            [In] int DefaultTimeOut,
            [In] [Optional] IntPtr SecurityAttributes
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetNamedPipeClientProcessId(
            [In] IntPtr NamedPipeHandle, 
            [Out] out int ServerProcessId
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetNamedPipeHandleState(
            [In] int NamedPipeHandle, 
            [Out] [Optional] out PipeState State,
            [Out] [Optional] out int CurInstances,
            [Out] [Optional] out int MaxCollectionCount,
            [Out] [Optional] out int CollectDataTimeout,
            [Out] [Optional] out int UserName,
            [In] int MaxUserNameSize
            );

        #endregion

        #region Native API

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int NtAlertThread(
            [In] IntPtr ThreadHandle
            );

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int NtCreateProcess(
            [Out] out IntPtr ProcessHandle,
            [In] ProcessAccess DesiredAccess,
            [In] [Optional] IntPtr ObjectAttributes,
            [In] IntPtr ParentProcess,
            [In] bool InheritHandleTable,
            [In] [Optional] IntPtr SectionHandle,
            [In] [Optional] IntPtr DebugPort,
            [In] [Optional] IntPtr ExceptionPort
            );

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int NtCreateSection(
            [Out] out IntPtr SectionHandle,
            [In] SectionAccess DesiredAccess,
            [In] [Optional] IntPtr ObjectAttributes,
            [In] [Optional] ref LargeInteger MaximumSize,
            [In] int PageAttributes,
            [In] int SectionAttributes,
            [In] [Optional] IntPtr FileHandle
            );

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int NtDuplicateObject(
            [In] IntPtr SourceProcessHandle,
            [In] IntPtr SourceHandle,
            [In] IntPtr TargetProcessHandle,
            [Out] out IntPtr TargetHandle,
            [In] int DesiredAccess,
            [In] int Attributes,
            [In] int Options
            );

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int NtGetNextProcess(
            [In] IntPtr ProcessHandle,
            [In] ProcessAccess DesiredAccess,
            [In] int HandleAttributes,
            [In] int Flags,
            [Out] out IntPtr NewProcessHandle
            );

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int NtGetNextThread(
            [In] IntPtr ProcessHandle,
            [In] ProcessAccess DesiredAccess,
            [In] int HandleAttributes,
            [In] int Flags,
            [Out] out int NewProcessHandle
            );

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int NtOpenSymbolicLinkObject(
            [Out] out IntPtr LinkHandle,
            [In] int DesiredAccess,
            [In] ref ObjectAttributes ObjectAttributes
            );

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int NtQuerySymbolicLinkObject(
            [In] IntPtr LinkHandle, 
            ref UnicodeString LinkName,
            [Out] [Optional] out int DataWritten
            );

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int NtResumeProcess(
            [In] IntPtr ProcessHandle
            );

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int NtSuspendProcess(
            [In] IntPtr ProcessHandle
            );

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int NtQuerySection(
            [In] IntPtr SectionHandle, 
            [In] SectionInformationClass SectionInformationClass,
            ref SectionBasicInformation SectionInformation, 
            [In] int SectionInformationLength, 
            [Out] [Optional] out int ReturnLength
            );

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int NtQuerySection(
            [In] IntPtr SectionHandle, 
            [In] SectionInformationClass SectionInformationClass,
            ref SectionImageInformation SectionInformation,
            [In] int SectionInformationLength,
            [Out] [Optional] out int ReturnLength
            );

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int NtQueryMutant(
            [In] IntPtr MutantHandle,
            [In] MutantInformationClass MutantInformationClass,
            ref MutantBasicInformation MutantInformation,
            [In] int MutantInformationLength, 
            [Out] [Optional] out int ReturnLength
            );

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int NtQueryEvent(
            [In] IntPtr EventHandle, 
            [In] EventInformationClass EventInformationClass,
            ref EventBasicInformation EventInformation,
            [In] int EventInformationLength, 
            [Out] [Optional] out int ReturnLength
            );

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int NtSetInformationThread(
            [In] IntPtr ThreadHandle,
            [In] ThreadInformationClass ThreadInformationClass,
            [In] IntPtr ThreadInformation,
            [In] int ThreadInformationLength
            );

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int NtQueryInformationThread(
            [In] IntPtr ThreadHandle,
            [In] ThreadInformationClass ThreadInformationClass,
            ref ThreadBasicInformation ThreadInformation,
            [In] int ThreadInformationLength, 
            [Out] [Optional] out int ReturnLength
            );

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int NtQueryInformationThread(
            [In] IntPtr ThreadHandle,
            [In] ThreadInformationClass ThreadInformationClass,
            [Out] out int ThreadInformation,
            [In] int ThreadInformationLength,
            [Out] [Optional] out int ReturnLength
            );

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int NtQueryInformationThread(
            [In] IntPtr ThreadHandle,
            [In] ThreadInformationClass ThreadInformationClass,
            IntPtr ThreadInformation,
            [In] int ThreadInformationLength, 
            [Out] [Optional] out int ReturnLength
            );

        [DllImport("ntdll.dll", SetLastError = true)]
        public unsafe static extern int NtQueryInformationThread(
            [In] IntPtr ThreadHandle,
            [In] ThreadInformationClass ThreadInformationClass,
            void* ThreadInformation,
            [In] int ThreadInformationLength,
            [Out] [Optional] out int ReturnLength
            );

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int NtQueryInformationProcess(
            [In] IntPtr ProcessHandle,
            [In] ProcessInformationClass ProcessInformationClass,
            IntPtr ProcessInformation,
            [In] int ProcessInformationLength,
            [Out] [Optional] out int ReturnLength
            );

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int NtQueryInformationProcess(
            [In] IntPtr ProcessHandle,
            [In] ProcessInformationClass ProcessInformationClass,
            [Out] out int ProcessInformation,
            [In] int ProcessInformationLength,
            [Out] [Optional] out int ReturnLength
            );

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int NtQueryInformationProcess(
            [In] IntPtr ProcessHandle,
            [In] ProcessInformationClass ProcessInformationClass,
            [Out] out PooledUsageAndLimits ProcessInformation,
            [In] int ProcessInformationLength,
            [Out] [Optional] out int ReturnLength
            );

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int NtQueryInformationProcess(
            [In] IntPtr ProcessHandle,
            [In] ProcessInformationClass ProcessInformationClass,
            [Out] out QuotaLimits ProcessInformation,
            [In] int ProcessInformationLength,
            [Out] [Optional] out int ReturnLength
            );

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int NtQueryInformationProcess(
            [In] IntPtr ProcessHandle,
            [In] ProcessInformationClass ProcessInformationClass,
            [Out] out MemExecuteOptions ProcessInformation,
            [In] int ProcessInformationLength,
            [Out] [Optional] out int ReturnLength
            );

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int NtQueryInformationProcess(
            [In] IntPtr ProcessHandle,
            [In] ProcessInformationClass ProcessInformationClass,
            [Out] out ProcessBasicInformation ProcessInformation,
            [In] int ProcessInformationLength,
            [Out] [Optional] out int ReturnLength
            );

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int NtQueryInformationProcess(
            [In] IntPtr ProcessHandle,
            [In] ProcessInformationClass ProcessInformationClass,
            [Out] out UnicodeString ProcessInformation,
            [In] int ProcessInformationLength,
            [Out] [Optional] out int ReturnLength
            );
        
        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int NtQuerySystemInformation(
            [In] SystemInformationClass SystemInformationClass,
            ref SystemBasicInformation SystemInformation,
            [In] int SystemInformationLength,
            [Out] [Optional] out int ReturnLength
            );

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int NtQuerySystemInformation(
            [In] SystemInformationClass SystemInformationClass,
            IntPtr SystemInformation,
            [In] int SystemInformationLength,
            [Out] [Optional] out int ReturnLength
            );

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int NtQuerySystemInformation(
            [In] SystemInformationClass SystemInformationClass,
            [MarshalAs(UnmanagedType.LPArray)] SystemProcessorPerformanceInformation[] SystemInformation,
            [In] int SystemInformationLength,
            [Out] [Optional] out int ReturnLength
            );

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int NtQuerySystemInformation(
            [In] SystemInformationClass SystemInformationClass,
            ref SystemPerformanceInformation SystemInformation,
            [In] int SystemInformationLength,
            [Out] [Optional] out int ReturnLength
            );

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int NtQuerySystemInformation(
            [In] SystemInformationClass SystemInformationClass,
            ref SystemCacheInformation SystemInformation,
            [In] int SystemInformationLength,
            [Out] [Optional] out int ReturnLength
            );
        
        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int NtSetSystemInformation(
            [In] SystemInformationClass SystemInformationClass,
            [In] ref SystemLoadAndCallImage SystemInformation, 
            [In] int SystemInformationLength
            );

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int NtQueryObject(
            [In] IntPtr Handle, 
            [In] ObjectInformationClass ObjectInformationClass,
            [Out] IntPtr ObjectInformation,
            [In] int ObjectInformationLength, 
            [Out] out int ReturnLength
            );

        #endregion

        #region Processes

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern void ExitProcess(
            [In] int ExitCode
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool QueryProcessCycleTime(
            [In] IntPtr ProcessHandle, 
            [Out] out ulong CycleTime
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetPriorityClass(
            [In] IntPtr ProcessHandle, 
            [In] int Priority
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetPriorityClass(
            [In] IntPtr ProcessHandle
            );

        [DllImport("psapi.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EmptyWorkingSet(
            [In] IntPtr ProcessHandle
            );

        [DllImport("psapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int GetMappedFileName(
            [In] IntPtr ProcessHandle,
            [In] IntPtr Address,
            [Out] StringBuilder Buffer,
            [In] int Size
            );

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CreateProcessWithTokenW(
            [In] IntPtr TokenHandle,
            [In] LogonFlags Flags,
            [In] [Optional] string ApplicationName,
            [Optional] string CommandLine,
            [In] CreationFlags CreationFlags,
            [In] [Optional] int Environment,
            [In] [Optional] string CurrentDirectory,
            [In] ref StartupInfo StartupInfo,
            [Out] out ProcessInformation ProcessInfo
            );

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CreateProcessAsUser(
            [In] [Optional] IntPtr TokenHandle,
            [In] [Optional] string ApplicationName,
            [Optional] string CommandLine,
            [In] [Optional] IntPtr ProcessAttributes,
            [In] [Optional] IntPtr ThreadAttributes,
            [In] bool InheritHandles,
            [In] CreationFlags CreationFlags,
            [In] [Optional] IntPtr Environment,
            [In] [Optional] string CurrentDirectory,
            [In] ref StartupInfo StartupInfo,
            [Out] out ProcessInformation ProcessInformation
            );

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CreateProcess(
            [In] [Optional] string ApplicationName,
            [Optional] string CommandLine,
            [In] [Optional] IntPtr ProcessAttributes,
            [In] [Optional] IntPtr ThreadAttributes,
            [In] bool InheritHandles,
            [In] CreationFlags CreationFlags,
            [In] [Optional] IntPtr Environment,
            [In] [Optional] string CurrentDirectory,
            [In] ref StartupInfo StartupInfo,
            [Out] out ProcessInformation ProcessInformation
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetExitCodeProcess(
            [In] IntPtr ProcessHandle, 
            [Out] out int ExitCode
            );

        // Vista and higher
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool QueryFullProcessImageName(
            [In] IntPtr ProcessHandle, 
            [In] [MarshalAs(UnmanagedType.Bool)] bool UseNativeName,
            [Out] StringBuilder ExeName, 
            ref int Size
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsProcessInJob(
            [In] IntPtr ProcessHandle,
            [In] [Optional] IntPtr JobHandle,
            [Out] [MarshalAs(UnmanagedType.Bool)] out bool Result
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetProcessAffinityMask(
            [In] IntPtr ProcessHandle, 
            [In] uint ProcessAffinityMask
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetProcessAffinityMask(
            [In] IntPtr ProcessHandle, 
            [Out] out uint ProcessAffinityMask,
            [Out] out uint SystemAffinityMask
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CheckRemoteDebuggerPresent(
            [In] IntPtr ProcessHandle, 
            [MarshalAs(UnmanagedType.Bool)] ref bool DebuggerPresent
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetProcessId(
            [In] IntPtr ProcessHandle
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetCurrentProcess();

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetCurrentProcessId();

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetProcessDEPPolicy(
            [In] IntPtr ProcessHandle, 
            [Out] out DepFlags Flags,
            [Out] [MarshalAs(UnmanagedType.Bool)] out bool Permanent
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool TerminateProcess(
            [In] IntPtr ProcessHandle, 
            [In] int ExitCode
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(
            [In] ProcessAccess DesiredAccess, 
            [In] bool InheritHandle, 
            [In] int ProcessId
            );

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DebugActiveProcess(
            [In] int Pid
            );

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DebugActiveProcessStop(
            [In] int Pid
            );

        [DllImport("psapi.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumProcessModules(
            [In] IntPtr ProcessHandle, 
            [Out] IntPtr[] ModuleHandles, 
            [In] int Size, 
            [Out] out int RequiredSize
            );

        [DllImport("psapi.dll", CharSet = CharSet.Unicode)]
        public static extern int GetModuleBaseName(
            [In] IntPtr ProcessHandle, 
            [In] [Optional] IntPtr ModuleHandle, 
            [Out] StringBuilder BaseName, 
            [In] int Size
            );

        [DllImport("psapi.dll", CharSet = CharSet.Unicode)]
        public static extern int GetModuleFileNameEx(
            [In] IntPtr ProcessHandle, 
            [In] [Optional] IntPtr ModuleHandle,
            [Out] StringBuilder FileName, 
            [In] int Size
            );

        [DllImport("psapi.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetModuleInformation(
            [In] IntPtr ProcessHandle, 
            [In] [Optional] IntPtr ModuleHandle,
            [Out] ModuleInfo ModInfo, 
            [In] int Size
            );

        #endregion

        #region Resources/Handles

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr CreateMutex(
            [In] [Optional] IntPtr attributes,
            [In] bool initialOwner, 
            [In] [Optional] string name
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetHandleInformation(
            [In] IntPtr handle,
            [In] Win32HandleFlags mask,
            [In] Win32HandleFlags flags
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetHandleInformation(
            [In] IntPtr handle,
            [Out] out Win32HandleFlags flags
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(
            [In] IntPtr Handle
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern WaitResult WaitForSingleObject(
            [In] IntPtr Object, 
            [In] uint Timeout
            );

        #endregion

        #region Security

        #region LSA

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern int LsaFreeMemory(
            [In] IntPtr Memory
            );

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern int LsaEnumerateAccountsWithUserRight(
            [In] IntPtr PolicyHandle, 
            [In] IntPtr UserRights,
            [Out] out IntPtr SIDs,
            [Out] out int CountReturned
            );

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern int LsaAddAccountRights(
            [In] IntPtr PolicyHandle, 
            [In] IntPtr AccountSid,
            [In] UnicodeString[] UserRights, 
            [In] uint CountOfRights
            );

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern int LsaOpenPolicy(
            [In] IntPtr SystemName, 
            [In] ref ObjectAttributes ObjectAttributes,
            [In] PolicyAccess DesiredAccess,
            ref IntPtr PolicyHandle
            );

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern int LsaClose(
            [In] IntPtr Handle
            );

        #endregion


        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ImpersonateLoggedOnUser(
            [In] IntPtr TokenHandle
            );

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RevertToSelf();

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool LogonUser(
            [In] string Username,
            [In] [Optional] string Domain,
            [In] string Password,
            [In] LogonType LogonType,
            [In] LogonProvider LogonProvider,
            [Out] out IntPtr TokenHandle
            );

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool OpenProcessToken(
            [In] IntPtr ProcessHandle, 
            [In] TokenAccess DesiredAccess,
            [Out] out IntPtr TokenHandle
            );

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool OpenThreadToken(
            [In] IntPtr ThreadHandle, 
            [In] TokenAccess DesiredAccess,
            [In] bool OpenAsSelf, 
            [Out] out IntPtr TokenHandle
            );

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DuplicateTokenEx(
            [In] IntPtr ExistingToken, 
            [In] TokenAccess DesiredAccess,
            [In] [Optional] IntPtr TokenAttributes, 
            [In] SecurityImpersonationLevel ImpersonationLevel, 
            [In] TokenType TokenType,
            [Out] out IntPtr NewToken
            );

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetTokenInformation(
            [In] IntPtr TokenHandle,
            [In] TokenInformationClass TokenInformationClass,
            [In] ref int TokenInformation,
            [In] int TokenInformationLength
            );

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetTokenInformation(
            [In] IntPtr TokenHandle,
            [In] TokenInformationClass TokenInformationClass,
            [Out] [Optional] IntPtr TokenInformation,
            [In] int TokenInformationLength,
            [Out] out int ReturnLength
            );

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetTokenInformation(
            [In] IntPtr TokenHandle,
            [In] TokenInformationClass TokenInformationClass,
            [Out] out IntPtr TokenInformation,
            [In] int TokenInformationLength,
            [Out] out int ReturnLength
            );

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetTokenInformation(
            [In] IntPtr TokenHandle,
            [In] TokenInformationClass TokenInformationClass,
            [Optional] ref TokenSource TokenInformation,
            [In] int TokenInformationLength,
            [Out] out int ReturnLength
            );

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool LookupAccountName(
            [In] [Optional] string SystemName,
            [In] string AccountName,
            [Out] [Optional] IntPtr SID,
            ref int SIDSize,
            [Out] StringBuilder ReferencedDomainName,
            ref int ReferencedDomainNameSize,
            [Out] out SidNameUse Use
            );

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool LookupAccountSid(
            [In] [Optional] string SystemName,
            [In] IntPtr SID,
            [Out] [Optional] StringBuilder Name, 
            ref int NameSize,
            [Out] [Optional] StringBuilder ReferencedDomainName, 
            ref int ReferencedDomainNameSize,
            [Out] out SidNameUse Use
            );

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool LookupPrivilegeDisplayName(
            [In] [Optional] string SystemName, 
            [In] string Name,
            [Out] [Optional] StringBuilder DisplayName, 
            ref int DisplayNameSize,
            [Out] out int LanguageId
            );

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool LookupPrivilegeName(
            [In] [Optional] string SystemName, 
            [In] ref Luid Luid,
            [Out] [Optional] StringBuilder Name, 
            ref int RequiredSize
            );

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool LookupPrivilegeValue(
            [In] [Optional] string SystemName, 
            [In] string PrivilegeName, 
            [Out] out Luid Luid
            );

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AdjustTokenPrivileges(
            [In] IntPtr TokenHandle,
            [In] [MarshalAs(UnmanagedType.Bool)] bool DisableAllPrivileges,
            [In] [Optional] ref TokenPrivileges NewState,
            [In] int BufferLength,
            [Out] [Optional] IntPtr PreviousState, 
            [Out] [Optional] int ReturnLength
            );

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool InitializeSecurityDescriptor(
            IntPtr SecurityDescriptor, 
            [In] int Revision
            );

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetSecurityDescriptorDacl(
            IntPtr SecurityDescriptor,
            [In] [MarshalAs(UnmanagedType.Bool)] bool DaclPresent,
            [In] [Optional] IntPtr Dacl,
            [In] [MarshalAs(UnmanagedType.Bool)] bool DaclDefaulted
            );

        #endregion      

        #region Services

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseServiceHandle(
            [In] IntPtr ServiceHandle
            );

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool StartService(
            [In] IntPtr Service, 
            [In] int NumServiceArgs, 
            [In] [Optional] string[] Args
            );

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ChangeServiceConfig(
            [In] IntPtr Service,
            [In] ServiceType ServiceType,
            [In] ServiceStartType StartType,
            [In] ServiceErrorControl ErrorControl, 
            [In] [Optional] string BinaryPath,
            [In] [Optional] string LoadOrderGroup,
            [Out] [Optional] out int TagID, 
            [In] [Optional] string Dependencies,
            [In] [Optional] string StartName,
            [In] [Optional] string Password,
            [In] [Optional] string DisplayName
            );

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ControlService(
            [In] IntPtr Service,
            [In] ServiceControl Control, 
            [Out] out ServiceStatus ServiceStatus
            );

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr CreateService(
            [In] IntPtr SCManager,
            [In] string ServiceName,
            [In] [Optional] string DisplayName,
            [In] ServiceAccess DesiredAccess,
            [In] ServiceType ServiceType,
            [In] ServiceStartType StartType,
            [In] ServiceErrorControl ErrorControl,
            [In] [Optional] string BinaryPathName,
            [In] [Optional] string LoadOrderGroup,
            [Out] [Optional] IntPtr TagID,
            [In] [Optional] IntPtr Dependencies,
            [In] [Optional] string ServiceStartName,
            [In] [Optional] string Password
            );

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteService(
            [In] IntPtr Service
            );

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool QueryServiceStatus(
            [In] IntPtr Service, 
            [Out] out ServiceStatus ServiceStatus
            );

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool QueryServiceStatusEx(
            [In] IntPtr Service, 
            [In] IntPtr InfoLevel,
            [Out] [Optional] ServiceStatusProcess ServiceStatus,
            [In] int BufSize, 
            [Out] out int BytesNeeded
            );

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool QueryServiceConfig(
            [In] IntPtr Service,
            [Out] [Optional] IntPtr ServiceConfig,
            [In] int BufSize, 
            [Out] out int BytesNeeded
            );

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool QueryServiceConfig2(
            [In] IntPtr Service,
            [In] ServiceInfoLevel InfoLevel,
            [Out] [Optional] IntPtr Buffer, 
            [In] int BufferSize, 
            [Out] out int ReturnLength
            );

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr OpenService(
            [In] IntPtr SCManager,
            [In] string ServiceName, 
            [In] ServiceAccess DesiredAccess
            );

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
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumServicesStatusEx(
            [In] IntPtr SCManager,
            [In] IntPtr InfoLevel,
            [In] ServiceQueryType ServiceType,
            [In] ServiceQueryState ServiceState,
            [Out] [Optional] IntPtr Services,
            [In] int BufSize, 
            [Out] out int BytesNeeded,
            [Out] out int ServicesReturned,
            ref int ResumeHandle,
            [In] [Optional] string GroupName
            );

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern IntPtr OpenSCManager(
            [In] [Optional] string MachineName, 
            [In] [Optional] string DatabaseName,
            [In] ScManagerAccess DesiredAccess
            );

        #endregion

        #region Shell

        [DllImport("shell32.dll", EntryPoint = "#61", CharSet = CharSet.Unicode)]
        public static extern int RunFileDlg(
            [In] IntPtr hWnd,
            [In] IntPtr Icon,
            [In] string Path,
            [In] string Title,
            [In] string Prompt,
            [In] RunFileDialogFlags Flags
            );

        [DllImport("shell32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShellExecuteEx(
            [MarshalAs(UnmanagedType.Struct)] ref ShellExecuteInfo s
            );

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(
            [In] int HookId, 
            [In] IntPtr HookFunction,
            [In] IntPtr Module,
            [In] int ThreadId
            );

        [DllImport("shell32.dll")]
        public extern static int ExtractIconEx(
            [In] string libName,
            [In] int iconIndex,
            [Out] IntPtr[] largeIcon,
            [Out] IntPtr[] smallIcon,
            [In] int nIcons
            );

        [DllImport("shell32.dll")]
        public static extern int SHGetFileInfo(
            [In] string pszPath,
            [In] uint dwFileAttributes,
            [Out] out ShFileInfo psfi,
            [In] uint cbSizeFileInfo,
            [In] uint uFlags);

        [DllImport("shell32.dll", EntryPoint = "#660")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FileIconInit([In] bool RestoreCache);

        #endregion

        #region Statistics

        [DllImport("psapi.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetPerformanceInfo(
            [Out] out PerformanceInformation PerformanceInformation,
            [In] int Size
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetProcessTimes(
            [In] IntPtr ProcessHandle,
            [Out] out FileTime CreationTime,
            [Out] out FileTime ExitTime,
            [Out] out FileTime KernelTime,
            [Out] out FileTime UserTime
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetProcessIoCounters(
            [In] IntPtr ProcessHandle, 
            [Out] out IoCounters IoCounters
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetSystemTimes(
            [Out] out FileTime IdleTime,
            [Out] out FileTime KernelTime,
            [Out] out FileTime UserTime
            );

        // From MSDN: Do not cast a pointer to a FILETIME structure to either a ULARGE_INTEGER* or __int64* value because it can cause alignment faults on 64-bit Windows.
        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetThreadTimes(
            [In] IntPtr hThread,
            [Out] out FileTime lpCreationTime,
            [Out] out FileTime lpExitTime,
            [Out] out FileTime lpKernelTime,
            [Out] out FileTime lpUserTime
            );

        #endregion

        #region Symbols/Stack Walking

        [DllImport("dbghelp.dll", SetLastError = true, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SymInitialize(
            [In] IntPtr ProcessHandle,
            [In] [Optional] string UserSearchPath,
            [In] bool InvadeProcess
            );

        [DllImport("dbghelp.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SymCleanup(
            [In] IntPtr ProcessHandle
            );

        [DllImport("dbghelp.dll", SetLastError = true)]
        public static extern SymbolOptions SymGetOptions();

        [DllImport("dbghelp.dll", SetLastError = true)]
        public static extern SymbolOptions SymSetOptions(
            [In] SymbolOptions SymOptions
            );

        [DllImport("dbghelp.dll", SetLastError = true, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SymGetSearchPath(
            [In] IntPtr ProcessHandle,
            [Out] StringBuilder SearchPath, 
            [In] int SearchPathLength
            );

        [DllImport("dbghelp.dll", SetLastError = true, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SymSetSearchPath(
            [In] IntPtr ProcessHandle, 
            [In] [Optional] string SearchPath
            );

        [DllImport("dbghelp.dll", SetLastError = true, CharSet = CharSet.Ansi)]
        public static extern long SymLoadModule64(
            [In] IntPtr ProcessHandle,
            [In] [Optional] IntPtr FileHandle,
            [In] [Optional] string ImageName,
            [In] [Optional] string ModuleName,
            [In] ulong BaseOfDll,
            [In] int SizeOfDll
            );

        [DllImport("dbghelp.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SymUnloadModule64(
            [In] IntPtr ProcessHandle,
            [In] ulong BaseOfDll
            );

        [DllImport("dbghelp.dll", SetLastError = true)]
        public static extern IntPtr SymFunctionTableAccess64(
            [In] IntPtr ProcessHandle,
            [In] ulong AddrBase
            );

        [DllImport("dbghelp.dll", SetLastError = true)]
        public static extern ulong SymGetModuleBase64(
            [In] IntPtr ProcessHandle, 
            [In] ulong Address
            );

        [DllImport("dbghelp.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SymEnumSymbols(
            [In] IntPtr ProcessHandle,
            [In] ulong BaseOfDll,
            [In] [Optional] string Mask,
            [In] SymEnumSymbolsProc EnumSymbolsCallback, 
            [In] [Optional] IntPtr UserContext
            );

        [DllImport("dbghelp.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SymFromAddr(
            [In] IntPtr ProcessHandle,
            [In] ulong Address,
            [Out] out ulong Displacement,
            [In] IntPtr Symbol
            );

        [DllImport("dbghelp.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SymFromIndex(
            [In] IntPtr ProcessHandle,
            [In] ulong BaseOfDll,
            [In] int Index,
            IntPtr Symbol
            );

        [DllImport("dbghelp.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SymGetLineFromAddr64(
            [In] IntPtr ProcessHandle,
            [In] ulong Address,
            [Out] out int Displacement,
            [Out] out ImagehlpLine64 Line
            );

        [DllImport("dbghelp.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool StackWalk64(
            [In] MachineType MachineType,
            [In] IntPtr ProcessHandle,
            [In] IntPtr ThreadHandle,
            ref StackFrame64 StackFrame,
            ref Context ContextRecord,
            [In] [Optional] ReadProcessMemoryProc64 ReadMemoryRoutine,
            [In] [Optional] FunctionTableAccessProc64 FunctionTableAccessRoutine,
            [In] [Optional] GetModuleBaseProc64 GetModuleBaseRoutine,
            [In] [Optional] IntPtr TranslateAddress
            );

        [DllImport("symsrv.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SymbolServerSetOptions(
            [In] SymbolServerOption Options,
            [In] ulong Data
            );

        #endregion

        #region TCP

        [DllImport("iphlpapi.dll", SetLastError = true)]
        public extern static int SetTcpEntry(
            [In] ref MibTcpRow TcpRow
            );

        [DllImport("iphlpapi.dll", SetLastError = true)]
        public extern static int GetExtendedTcpTable(
            [Out] IntPtr Table,
            ref int Size,
            [In] bool Order,
            [In] int IpVersion, // 2 for IPv4
            [In] TcpTableClass TableClass,
            [In] int Reserved
            );

        [DllImport("iphlpapi.dll", SetLastError = true)]
        public extern static int GetTcpStatistics(
            [Out] out MibTcpStats pStats
            );

        [DllImport("iphlpapi.dll", SetLastError = true)]
        public static extern int GetTcpTable(
            [Out] byte[] tcpTable, 
            ref int pdwSize, 
            [In] bool bOrder
            );

        [DllImport("iphlpapi.dll", SetLastError = true)]
        public extern static int AllocateAndGetTcpExTableFromStack(
            [Out] out IntPtr pTable, 
            [In] bool bOrder,
            [In] IntPtr heap,
            [In] int flags,
            [In] int family
            );

        #endregion

        #region Terminal Server

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ProcessIdToSessionId(
            [In] int ProcessId,
            [Out] out int SessionId
            );

        [DllImport("wtsapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool WTSQuerySessionInformation(
            [In] IntPtr ServerHandle,
            [In] int SessionID,
            [In] WtsInformationClass InfoClass,
            [Out] out IntPtr Buffer,
            [Out] out int BytesReturned
            );

        [DllImport("wtsapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool WTSQuerySessionInformation(
            [In] IntPtr ServerHandle,
            [In] int SessionID,
            [In] WtsInformationClass InfoClass,
            [Out] out string Buffer,
            [Out] out int BytesReturned
            );

        [DllImport("wtsapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool WTSLogoffSession(
            [In] IntPtr ServerHandle,
            [In] int SessionID,
            [In] bool Wait
            );

        [DllImport("wtsapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool WTSDisconnectSession(
            [In] IntPtr ServerHandle,
            [In] int SessionID,
            [In] bool Wait
            );

        [DllImport("wtsapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool WTSTerminateProcess(
            [In] IntPtr ServerHandle,
            [In] int ProcessID,
            [In] int ExitCode
            );

        [DllImport("wtsapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool WTSEnumerateSessions(
            [In] IntPtr ServerHandle,
            [In] int Reserved,
            [In] int Version,
            [Out] out IntPtr SessionInfo,
            [Out] out int Count
            );

        [DllImport("wtsapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool WTSEnumerateProcesses(
            [In] IntPtr ServerHandle,
            [In] int Reserved,
            [In] int Version, 
            [Out] out IntPtr ProcessInfo,
            [Out] out int Count
            );

        [DllImport("wtsapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool WTSFreeMemory(
            [In] IntPtr Memory
            );

        [DllImport("wtsapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool WTSFreeMemory([In] string Memory);

        #endregion

        #region Threads

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool QueryThreadCycleTime(
            [In] IntPtr ThreadHandle, 
            [Out] out ulong CycleTime
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool QueueUserAPC(
            [In] IntPtr APC,
            [In] IntPtr ThreadHandle,
            [In] IntPtr Data
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetExitCodeThread(
            [In] IntPtr ThreadHandle,
            [Out] out int ExitCode
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetThreadPriority(
            [In] IntPtr ThreadHandle, 
            [In] int Priority
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetThreadPriority([In] IntPtr ThreadHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int CreateThread(
            [In] [Optional] IntPtr ThreadAttributes,
            [In] int StackSize,
            [In] [MarshalAs(UnmanagedType.FunctionPtr)] ThreadStart StartAddress,
            [In] IntPtr Parameter,
            [In] int CreationFlags,
            [Out] out int ThreadId
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetProcessIdOfThread(
            [In] IntPtr ThreadHandle
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetThreadId(
            [In] IntPtr ThreadHandle
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenThread(
            [In] ThreadAccess DesiredAccess, 
            [In] bool InheritHandle, 
            [In] int ThreadId
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool TerminateThread(
            IntPtr ThreadHandle,
            [In] int ExitCode
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int SuspendThread(
            [In] IntPtr ThreadHandle
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int ResumeThread(
            [In] IntPtr ThreadHandle
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetThreadContext(
            [In] IntPtr ThreadHandle, 
            [In] ref Context Context
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetThreadContext(
            [In] IntPtr ThreadHandle, 
            ref Context Context
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CreateRemoteThread(
            [In] IntPtr ProcessHandle,
            [In] IntPtr ThreadAttributes,
            [In] int StackSize,
            [In] IntPtr StartAddress,
            [In] IntPtr Parameter,
            [In] int CreationFlags,
            [Out] out int ThreadId
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetCurrentThreadId();

        #endregion

        #region Toolhelp

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateToolhelp32Snapshot(
            [In] SnapshotFlags dwFlags,
            [In] int th32ProcessID
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool Process32First(
            [In] IntPtr hSnapshot,
            [MarshalAs(UnmanagedType.Struct)] ref ProcessEntry32 lppe
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool Process32Next(
            [In] IntPtr hSnapshot,
            [Out] [MarshalAs(UnmanagedType.Struct)] out ProcessEntry32 lppe
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool Thread32First(
            [In] IntPtr hSnapshot,
            [MarshalAs(UnmanagedType.Struct)] ref ThreadEntry32 lppe
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool Thread32Next(
            [In] IntPtr hSnapshot,
            [Out] [MarshalAs(UnmanagedType.Struct)] out ThreadEntry32 lppe
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool Module32First(
            [In] IntPtr hSnapshot,
            [MarshalAs(UnmanagedType.Struct)] ref ModuleEntry32 lppe
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool Module32Next(
            [In] IntPtr hSnapshot,
            [Out] [MarshalAs(UnmanagedType.Struct)] out ModuleEntry32 lppe
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool Heap32ListFirst(
            [In] IntPtr hSnapshot,
            [MarshalAs(UnmanagedType.Struct)] ref HeapList32 lppe
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool Heap32ListNext(
            [In] IntPtr hSnapshot,
            [Out] [MarshalAs(UnmanagedType.Struct)] out HeapList32 lppe
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool Heap32First(
            [MarshalAs(UnmanagedType.Struct)] ref HeapEntry32 lppe,
            [In] int ProcessID,
            [In] IntPtr HeapID
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int Heap32Next(
            [Out] [MarshalAs(UnmanagedType.Struct)] out HeapEntry32 lppe
            );

        #endregion

        #region UDP

        [DllImport("iphlpapi.dll", SetLastError = true)]
        public extern static int GetExtendedUdpTable(
            [Out] IntPtr Table, 
            ref int Size,
            [In] bool Order,
            [In] int IpVersion, // 2 for IPv4
            [In] UdpTableClass TableClass,
            [In] int Reserved
            );

        [DllImport("iphlpapi.dll", SetLastError = true)]
        public static extern int GetUdpStatistics(
            [Out] out MibUdpStats pStats
            );

        [DllImport("iphlpapi.dll", SetLastError = true)]
        public static extern int GetUdpTable(
            [Out] byte[] udpTable, 
            ref int pdwSize, 
            [In] bool bOrder
            );

        [DllImport("iphlpapi.dll", SetLastError = true)]
        public extern static int AllocateAndGetUdpExTableFromStack(
            [Out] out IntPtr pTable, 
            [In] bool bOrder,
            [In] IntPtr heap,
            [In] int flags,
            [In] int family
            );

        #endregion

        #region User

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetUserObjectSecurity(
            [In] IntPtr Handle,
            [In] ref SiRequested SiRequested,
            [In] IntPtr Sid
            );

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr OpenDesktop(
            [In] string Desktop,
            [In] int Flags,
            [In] bool Inherit,
            [In] DesktopAccess DesiredAccess
            );

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseDesktop(
            [In] IntPtr Handle
            );

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr OpenWindowStation(
            [In] string WinSta,
            [In] bool Inherit,
            [In] WindowStationAccess DesiredAccess
            );

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseWindowStation(
            [In] IntPtr Handle
            );

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetThreadDesktop(
            [In] int ThreadId
            );

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetThreadDesktop(
            [In] IntPtr DesktopHandle
            );

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetProcessWindowStation();

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetProcessWindowStation(
            [In] IntPtr WindowStationHandle
            );

        [DllImport("userenv.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CreateEnvironmentBlock(
            [Out] out IntPtr Environment,
            [In] IntPtr TokenHandle,
            [In] bool Inherit
            );

        [DllImport("userenv.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool LoadUserProfile(
            [In] IntPtr TokenHandle,
            ref ProfileInformation ProfileInfo
            );

        [DllImport("userenv.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool UnloadUserProfile(
            [In] IntPtr TokenHandle,
            [In] IntPtr ProfileHandle
            );

        #endregion

        #region Windows

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetCursorPos(
            [Out]out Point location
            );

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ChangeWindowMessageFilter(
            [In] WindowMessage message, 
            [In] UipiFilterFlag flag
            );

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(
            [In] IntPtr windowHandle, 
            [In] WindowMessage msg,
            [In] int w,
            [In] int l
            );

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessageTimeout(
            [In] IntPtr windowHandle,
            [In] WindowMessage msg,
            [In] int w,
            [In] int l,
            [In] SmtoFlags flags,
            [In] int timeout,
            [Out] out int result
            );

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool PostMessage(
            [In] IntPtr windowHandle,
            [In] WindowMessage msg,
            [In] int w,
            [In] int l
            );

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(
            [In] IntPtr hWnd
            );

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AllowSetForegroundWindow(
            [In] int processId
            );

        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
        public static extern int SetWindowTheme(
            [In] IntPtr hWnd,
            [In] string appName,
            [In] string idList
            );

        [DllImport("user32.dll")]
        public static extern int GetGuiResources(
            [In] IntPtr ProcessHandle,
            [In] int UserObjects
            );

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DestroyIcon(
            [In] IntPtr Handle
            );

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BringWindowToTop(
            [In] IntPtr hWnd
            );

        [DllImport("user32.dll")]
        public static extern int EnumWindows(
            [In] [MarshalAs(UnmanagedType.FunctionPtr)] EnumWindowsProc Callback,
            [In] int param
            );

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumThreadWindows(
            [In] int threadId,
            [In] [MarshalAs(UnmanagedType.FunctionPtr)] EnumThreadWndProc callback,
            [In] int param
            );

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumChildWindows(
            [In] IntPtr hWnd,
            [In] [MarshalAs(UnmanagedType.FunctionPtr)] EnumChildProc callback,
            [In] int param
            );

        [DllImport("user32.dll")]
        public static extern int GetWindowThreadProcessId(
            [In] IntPtr hWnd,
            [Out] out int processId
            );

        [DllImport("user32.dll")]
        public static extern IntPtr SetActiveWindow(
            [In] IntPtr hWnd
            );

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool PeekMessage(
            [Out] out Message msg, 
            [In] IntPtr hWnd,
            [In] uint messageFilterMin,
            [In] uint messageFilterMax,
            [In] PeekMessageFlags flags
            );

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool TranslateMessage(
            [In] ref Message msg
            );

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern IntPtr DispatchMessage(
            [In] ref Message msg
            );

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr DefWindowProc(
            [In] IntPtr hWnd, 
            [In] WindowMessage msg,
            [In] IntPtr wParam,
            [In] IntPtr lParam
            );

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern void PostQuitMessage(
            [In] int exitCode
            );

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
#if(_WIN64)
		private static extern IntPtr SetWindowLongPtr(
            [In] IntPtr hWnd, 
            [In] int index, 
            [In] [MarshalAs(UnmanagedType.FunctionPtr)] WndProcDelegate windowCallback
            );
#else
        private static extern IntPtr SetWindowLong(
            [In] IntPtr hWnd,
            [In] int index,
            [In] [MarshalAs(UnmanagedType.FunctionPtr)] WndProcDelegate windowCallback
            );
#endif

        [DllImport("user32.dll", SetLastError = true, EntryPoint = "SetWindowLong", CharSet = CharSet.Auto)]
        public static extern IntPtr SetWindowLongStyle(
            [In] IntPtr hWnd,
            [In] int index,
            [In] WindowStyles style
            );

        [DllImport("user32.dll", SetLastError = true, EntryPoint = "GetWindowLong", CharSet = CharSet.Auto)]
        public static extern WindowStyles GetWindowLongStyle(
            [In] IntPtr hWnd,
            [In] int index
            );

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetClientRect(
            [In] IntPtr hWnd, 
            [Out] out Rectangle rect
            );

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(
            [In] IntPtr hWnd, 
            [Out] out Rectangle rect
            );

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(
            [In] IntPtr hWnd,
            [In] IntPtr hWndAfter,
            [In] int x,
            [In] int y,
            [In] int w,
            [In] int h,
            [In] uint flags
            );

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ScreenToClient(
            [In] IntPtr hWnd, 
            ref Point rect
            );

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr SetFocus(
            [In] IntPtr hWnd
            );

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetParent(
            [In] IntPtr hWnd
            );

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetMonitorInfo(
            [In] IntPtr hWnd, 
            [Out] out MonitorInformation info
            );

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr MonitorFromWindow(
            [In] IntPtr hWnd, 
            [In] uint flags
            );

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern short GetAsyncKeyState([In] uint key);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr SetCapture([In] IntPtr handle);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShowWindow(
            [In] IntPtr hWnd, 
            [In] ShowWindowType flags
            );

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetMenu(
            [In] IntPtr hWnd, 
            [In] IntPtr menuHandle
            );

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseWindow(
            [In] IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DestroyWindow(
            [In] IntPtr hWnd
            );

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsIconic(
            [In] IntPtr hWnd
            );

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AdjustWindowRect(
            ref Rectangle rect, 
            [In] WindowStyles style,
            [In] [MarshalAs(UnmanagedType.Bool)]bool menu
            );

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr RegisterClass(
            [In] ref WindowClass wndClass
            );

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnregisterClass(
            [In] [MarshalAs(UnmanagedType.LPTStr)] string className,
            [In] IntPtr instanceHandle
            );

        [DllImport("user32.dll", SetLastError = true, EntryPoint = "CreateWindowEx", CharSet = CharSet.Auto)]
        public static extern IntPtr CreateWindow(
            [In] int exStyle,
            [In] [MarshalAs(UnmanagedType.LPTStr)] string className,
            [In] [MarshalAs(UnmanagedType.LPTStr)] string windowName,
            [In] WindowStyles style,
            [In] int x,
            [In] int y,
            [In] int width,
            [In] int height,
            [In] IntPtr parent,
            [In] IntPtr menuHandle,
            [In] IntPtr instanceHandle,
            [In] IntPtr zero
            );

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetCaretBlinkTime();

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int InternalGetWindowText(
            [In] IntPtr hWnd, 
            [Out] StringBuilder str, 
            int maxCount
            );

        #endregion
    }
}
