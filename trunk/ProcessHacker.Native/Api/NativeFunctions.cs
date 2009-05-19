/*
 * Process Hacker - 
 *   native API functions
 *                       
 * Copyright (C) 2009 Flavio Erlich
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
using ProcessHacker.Native.Security;

// you won't get some of this stuff from anywhere else... :)

namespace ProcessHacker.Native.Api
{
    public partial class Win32
    {
        [DllImport("ntdll.dll")]
        public static extern int NtAlertThread(
            [In] IntPtr ThreadHandle
            );

        [DllImport("ntdll.dll")]
        public static extern int NtAlertResumeThread(
            [In] IntPtr ThreadHandle,
            [Out] [Optional] out int PreviousSuspendCount
            );

        [DllImport("ntdll.dll")]
        public static extern int NtAllocateLocallyUniqueId(
            [Out] out Luid Luid
            );

        [DllImport("ntdll.dll")]
        public static extern int NtCancelTimer(
            [In] IntPtr TimerHandle,
            [Out] [Optional] out bool CurrentState
            );

        [DllImport("ntdll.dll")]
        public static extern int NtClearEvent(
            [In] IntPtr EventHandle
            );

        [DllImport("ntdll.dll")]
        public static extern int NtCreateDebugObject(
            [Out] out IntPtr DebugObjectHandle,
            [In] DebugObjectAccess DesiredAccess,
            [In] [Optional] ref ObjectAttributes ObjectAttributes,
            [In] DebugObjectFlags Flags
            );

        [DllImport("ntdll.dll")]
        public static extern int NtCreateDebugObject(
            [Out] out IntPtr DebugObjectHandle,
            [In] DebugObjectAccess DesiredAccess,
            [In] [Optional] IntPtr ObjectAttributes,
            [In] DebugObjectFlags Flags
            );

        [DllImport("ntdll.dll")]
        public static extern int NtCreateDirectoryObject(
            [Out] out IntPtr DirectoryHandle,
            [In] DirectoryAccess DesiredAccess,
            [In] ref ObjectAttributes ObjectAttributes
            );

        [DllImport("ntdll.dll")]
        public static extern int NtCreateEvent(
            [Out] out IntPtr EventHandle,
            [In] EventAccess DesiredAccess,
            [In] [Optional] ref ObjectAttributes ObjectAttributes,
            [In] EventType EventType,
            [In] bool InitialState
            ); 

        [DllImport("ntdll.dll")]
        public static extern int NtCreateEvent(
            [Out] out IntPtr EventHandle,
            [In] EventAccess DesiredAccess,
            [In] [Optional] IntPtr ObjectAttributes,
            [In] EventType EventType,
            [In] bool InitialState
            );

        [DllImport("ntdll.dll")]
        public static extern int NtCreateEventPair(
            [Out] out IntPtr EventPairHandle,
            [In] EventPairAccess DesiredAccess,
            [In] [Optional] ref ObjectAttributes ObjectAttributes
            );

        [DllImport("ntdll.dll")]
        public static extern int NtCreateEventPair(
            [Out] out IntPtr EventPairHandle,
            [In] EventPairAccess DesiredAccess,
            [In] [Optional] IntPtr ObjectAttributes
            );

        [DllImport("ntdll.dll")]
        public static extern int NtCreateKeyedEvent(
            [Out] out IntPtr KeyedEventHandle,
            [In] KeyedEventAccess DesiredAccess,
            [In] [Optional] ref ObjectAttributes ObjectAttributes,
            [In] int Flags
            );

        [DllImport("ntdll.dll")]
        public static extern int NtCreateKeyedEvent(
            [Out] out IntPtr KeyedEventHandle,
            [In] KeyedEventAccess DesiredAccess,
            [In] [Optional] IntPtr ObjectAttributes,
            [In] int Flags
            );

        [DllImport("ntdll.dll")]
        public static extern int NtCreateMutant(
            [Out] out IntPtr MutantHandle,
            [In] MutantAccess DesiredAccess,
            [In] [Optional] ref ObjectAttributes ObjectAttributes,
            [In] bool InitialOwner
            );

        [DllImport("ntdll.dll")]
        public static extern int NtCreateMutant(
            [Out] out IntPtr MutantHandle,
            [In] MutantAccess DesiredAccess,
            [In] [Optional] IntPtr ObjectAttributes,
            [In] bool InitialOwner
            );

        [DllImport("ntdll.dll")]
        public static extern int NtCreateProcess(
            [Out] out IntPtr ProcessHandle,
            [In] ProcessAccess DesiredAccess,
            [In] [Optional] ref ObjectAttributes ObjectAttributes,
            [In] IntPtr ParentProcess,
            [In] bool InheritHandleTable,
            [In] [Optional] IntPtr SectionHandle,
            [In] [Optional] IntPtr DebugPort,
            [In] [Optional] IntPtr ExceptionPort
            );

        [DllImport("ntdll.dll")]
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

        [DllImport("ntdll.dll")]
        public static extern int NtCreateSection(
            [Out] out IntPtr SectionHandle,
            [In] SectionAccess DesiredAccess,
            [In] [Optional] ref ObjectAttributes ObjectAttributes,
            [In] [Optional] ref LargeInteger MaximumSize,
            [In] int PageAttributes,
            [In] int SectionAttributes,
            [In] [Optional] IntPtr FileHandle
            );

        [DllImport("ntdll.dll")]
        public static extern int NtCreateSection(
            [Out] out IntPtr SectionHandle,
            [In] SectionAccess DesiredAccess,
            [In] [Optional] IntPtr ObjectAttributes,
            [In] [Optional] ref LargeInteger MaximumSize,
            [In] int PageAttributes,
            [In] int SectionAttributes,
            [In] [Optional] IntPtr FileHandle
            );

        [DllImport("ntdll.dll")]
        public static extern int NtCreateSemaphore(
            [Out] out IntPtr SemaphoreHandle,
            [In] SemaphoreAccess DesiredAccess,
            [In] [Optional] ref ObjectAttributes ObjectAttributes,
            [In] int InitialCount,
            [In] int MaximumCount
            );

        [DllImport("ntdll.dll")]
        public static extern int NtCreateSemaphore(
            [Out] out IntPtr SemaphoreHandle,
            [In] SemaphoreAccess DesiredAccess,
            [In] [Optional] IntPtr ObjectAttributes,
            [In] int InitialCount,
            [In] int MaximumCount
            );

        [DllImport("ntdll.dll")]
        public static extern int NtCreateSymbolicLinkObject(
            [Out] out IntPtr LinkHandle,
            [In] SymbolicLinkAccess DesiredAccess,
            [In] ref ObjectAttributes ObjectAttributes,
            [In] ref UnicodeString LinkTarget
            );

        [DllImport("ntdll.dll")]
        public static extern int NtCreateTimer(
            [Out] out IntPtr TimerHandle,
            [In] TimerAccess DesiredAccess,
            [In] [Optional] ref ObjectAttributes ObjectAttributes,
            [In] TimerType TimerType
            );

        [DllImport("ntdll.dll")]
        public static extern int NtCreateTimer(
            [Out] out IntPtr TimerHandle,
            [In] TimerAccess DesiredAccess,
            [In] [Optional] IntPtr ObjectAttributes,
            [In] TimerType TimerType
            );

        [DllImport("ntdll.dll")]
        public static extern int NtDebugActiveProcess(
            [In] IntPtr ProcessHandle,
            [In] IntPtr DebugObjectHandle
            );

        [DllImport("ntdll.dll")]
        public static extern int NtDelayExecution(
            [In] bool Alertable,
            [In] ref long DelayInterval
            );

        [DllImport("ntdll.dll")]
        public static extern int NtDuplicateObject(
            [In] IntPtr SourceProcessHandle,
            [In] IntPtr SourceHandle,
            [In] IntPtr TargetProcessHandle,
            [Out] out IntPtr TargetHandle,
            [In] int DesiredAccess,
            [In] HandleFlags Attributes,
            [In] int Options
            );

        [DllImport("ntdll.dll")]
        public static extern int NtGetNextProcess(
            [In] [Optional] IntPtr ProcessHandle,
            [In] ProcessAccess DesiredAccess,
            [In] HandleFlags HandleAttributes,
            [In] int Flags,
            [Out] out IntPtr NewProcessHandle
            );

        [DllImport("ntdll.dll")]
        public static extern int NtGetNextThread(
            [In] [Optional] IntPtr ProcessHandle,
            [In] [Optional] IntPtr ThreadHandle,
            [In] ThreadAccess DesiredAccess,
            [In] HandleFlags HandleAttributes,
            [In] int Flags,
            [Out] out IntPtr NewThreadHandle
            );

        [DllImport("ntdll.dll")]
        public static extern int NtLoadDriver(
            [In] ref UnicodeString DriverPath
            );

        [DllImport("ntdll.dll")]
        public static extern int NtMakePermanentObject(
            [In] IntPtr Handle
            );

        [DllImport("ntdll.dll")]
        public static extern int NtMakeTemporaryObject(
            [In] IntPtr Handle
            );

        [DllImport("ntdll.dll")]
        public static extern int NtOpenDirectoryObject(
            [Out] out IntPtr DirectoryHandle,
            [In] DirectoryAccess DesiredAccess,
            [In] ref ObjectAttributes ObjectAttributes
            );

        [DllImport("ntdll.dll")]
        public static extern int NtOpenEvent(
            [Out] out IntPtr EventHandle,
            [In] EventAccess DesiredAccess,
            [In] ref ObjectAttributes ObjectAttributes
            );

        [DllImport("ntdll.dll")]
        public static extern int NtOpenEventPair(
            [Out] out IntPtr EventPairHandle,
            [In] EventPairAccess DesiredAccess,
            [In] ref ObjectAttributes ObjectAttributes
            );

        [DllImport("ntdll.dll")]
        public static extern int NtOpenKeyedEvent(
            [Out] out IntPtr KeyedEventHandle,
            [In] KeyedEventAccess DesiredAccess,
            [In] ref ObjectAttributes ObjectAttributes
            );

        [DllImport("ntdll.dll")]
        public static extern int NtOpenMutant(
            [Out] out IntPtr MutantHandle,
            [In] MutantAccess DesiredAccess,
            [In] ref ObjectAttributes ObjectAttributes
            );

        [DllImport("ntdll.dll")]
        public static extern int NtOpenSemaphore(
            [Out] out IntPtr SemaphoreHandle,
            [In] SemaphoreAccess DesiredAccess,
            [In] ref ObjectAttributes ObjectAttributes
            );

        [DllImport("ntdll.dll")]
        public static extern int NtOpenSymbolicLinkObject(
            [Out] out IntPtr LinkHandle,
            [In] SymbolicLinkAccess DesiredAccess,
            [In] ref ObjectAttributes ObjectAttributes
            );

        [DllImport("ntdll.dll")]
        public static extern int NtOpenTimer(
            [Out] out IntPtr TimerHandle,
            [In] TimerAccess DesiredAccess,
            [In] ref ObjectAttributes ObjectAttributes
            );

        [DllImport("ntdll.dll")]
        public static extern int NtPulseEvent(
            [In] IntPtr EventHandle,
            [Out] [Optional] out int PreviousState
            );

        [DllImport("ntdll.dll")]
        public static extern int NtQueryDirectoryObject(
            [In] IntPtr DirectoryHandle,
            [In] IntPtr Buffer,
            [In] int Length,
            [In] bool ReturnSingleEntry,
            [In] bool RestartScan,
            ref int Context,
            [Out] [Optional] out int ReturnLength
            );

        [DllImport("ntdll.dll")]
        public static extern int NtQueryEvent(
            [In] IntPtr EventHandle,
            [In] EventInformationClass EventInformationClass,
            [Out] out EventBasicInformation EventInformation,
            [In] int EventInformationLength,
            [Out] [Optional] out int ReturnLength
            );

        [DllImport("ntdll.dll")]
        public static extern int NtQueryInformationProcess(
            [In] IntPtr ProcessHandle,
            [In] ProcessInformationClass ProcessInformationClass,
            IntPtr ProcessInformation,
            [In] int ProcessInformationLength,
            [Out] [Optional] out int ReturnLength
            );

        [DllImport("ntdll.dll")]
        public static extern int NtQueryInformationProcess(
            [In] IntPtr ProcessHandle,
            [In] ProcessInformationClass ProcessInformationClass,
            [Out] out int ProcessInformation,
            [In] int ProcessInformationLength,
            [Out] [Optional] out int ReturnLength
            );

        [DllImport("ntdll.dll")]
        public static extern int NtQueryInformationProcess(
            [In] IntPtr ProcessHandle,
            [In] ProcessInformationClass ProcessInformationClass,
            [Out] out PooledUsageAndLimits ProcessInformation,
            [In] int ProcessInformationLength,
            [Out] [Optional] out int ReturnLength
            );

        [DllImport("ntdll.dll")]
        public static extern int NtQueryInformationProcess(
            [In] IntPtr ProcessHandle,
            [In] ProcessInformationClass ProcessInformationClass,
            [Out] out QuotaLimits ProcessInformation,
            [In] int ProcessInformationLength,
            [Out] [Optional] out int ReturnLength
            );

        [DllImport("ntdll.dll")]
        public static extern int NtQueryInformationProcess(
            [In] IntPtr ProcessHandle,
            [In] ProcessInformationClass ProcessInformationClass,
            [Out] out MemExecuteOptions ProcessInformation,
            [In] int ProcessInformationLength,
            [Out] [Optional] out int ReturnLength
            );

        [DllImport("ntdll.dll")]
        public static extern int NtQueryInformationProcess(
            [In] IntPtr ProcessHandle,
            [In] ProcessInformationClass ProcessInformationClass,
            [Out] out ProcessBasicInformation ProcessInformation,
            [In] int ProcessInformationLength,
            [Out] [Optional] out int ReturnLength
            );

        [DllImport("ntdll.dll")]
        public static extern int NtQueryInformationProcess(
            [In] IntPtr ProcessHandle,
            [In] ProcessInformationClass ProcessInformationClass,
            [Out] out UnicodeString ProcessInformation,
            [In] int ProcessInformationLength,
            [Out] [Optional] out int ReturnLength
            );

        [DllImport("ntdll.dll")]
        public static extern int NtQueryInformationThread(
            [In] IntPtr ThreadHandle,
            [In] ThreadInformationClass ThreadInformationClass,
            ref ThreadBasicInformation ThreadInformation,
            [In] int ThreadInformationLength,
            [Out] [Optional] out int ReturnLength
            );

        [DllImport("ntdll.dll")]
        public static extern int NtQueryInformationThread(
            [In] IntPtr ThreadHandle,
            [In] ThreadInformationClass ThreadInformationClass,
            [Out] out int ThreadInformation,
            [In] int ThreadInformationLength,
            [Out] [Optional] out int ReturnLength
            );

        [DllImport("ntdll.dll")]
        public static extern int NtQueryInformationThread(
            [In] IntPtr ThreadHandle,
            [In] ThreadInformationClass ThreadInformationClass,
            IntPtr ThreadInformation,
            [In] int ThreadInformationLength,
            [Out] [Optional] out int ReturnLength
            );

        [DllImport("ntdll.dll")]
        public unsafe static extern int NtQueryInformationThread(
            [In] IntPtr ThreadHandle,
            [In] ThreadInformationClass ThreadInformationClass,
            void* ThreadInformation,
            [In] int ThreadInformationLength,
            [Out] [Optional] out int ReturnLength
            );

        [DllImport("ntdll.dll")]
        public static extern int NtQueryMutant(
            [In] IntPtr MutantHandle,
            [In] MutantInformationClass MutantInformationClass,
            [Out] out MutantBasicInformation MutantInformation,
            [In] int MutantInformationLength,
            [Out] [Optional] out int ReturnLength
            );

        [DllImport("ntdll.dll")]
        public static extern int NtQueryObject(
            [In] IntPtr Handle,
            [In] ObjectInformationClass ObjectInformationClass,
            [Out] IntPtr ObjectInformation,
            [In] int ObjectInformationLength,
            [Out] out int ReturnLength
            );

        [DllImport("ntdll.dll")]
        public static extern int NtQuerySection(
            [In] IntPtr SectionHandle,
            [In] SectionInformationClass SectionInformationClass,
            [Out] out SectionBasicInformation SectionInformation,
            [In] int SectionInformationLength,
            [Out] [Optional] out int ReturnLength
            );

        [DllImport("ntdll.dll")]
        public static extern int NtQuerySection(
            [In] IntPtr SectionHandle,
            [In] SectionInformationClass SectionInformationClass,
            [Out] out SectionImageInformation SectionInformation,
            [In] int SectionInformationLength,
            [Out] [Optional] out int ReturnLength
            );

        [DllImport("ntdll.dll")]
        public static extern int NtQuerySemaphore(
            [In] IntPtr SemaphoreHandle,
            [In] SemaphoreInformationClass SemaphoreInformationClass,
            [Out] out SemaphoreBasicInformation SemaphoreInformation,
            [In] int SemaphoreInformationLength,
            [Out] [Optional] out int ReturnLength
            );

        [DllImport("ntdll.dll")]
        public static extern int NtQuerySymbolicLinkObject(
            [In] IntPtr LinkHandle,
            ref UnicodeString LinkName,
            [Out] [Optional] out int ReturnLength
            );

        [DllImport("ntdll.dll")]
        public static extern int NtQuerySystemInformation(
            [In] SystemInformationClass SystemInformationClass,
            [Out] out SystemBasicInformation SystemInformation,
            [In] int SystemInformationLength,
            [Out] [Optional] out int ReturnLength
            );

        [DllImport("ntdll.dll")]
        public static extern int NtQuerySystemInformation(
            [In] SystemInformationClass SystemInformationClass,
            IntPtr SystemInformation,
            [In] int SystemInformationLength,
            [Out] [Optional] out int ReturnLength
            );

        [DllImport("ntdll.dll")]
        public static extern int NtQuerySystemInformation(
            [In] SystemInformationClass SystemInformationClass,
            [MarshalAs(UnmanagedType.LPArray)] SystemProcessorPerformanceInformation[] SystemInformation,
            [In] int SystemInformationLength,
            [Out] [Optional] out int ReturnLength
            );

        [DllImport("ntdll.dll")]
        public static extern int NtQuerySystemInformation(
            [In] SystemInformationClass SystemInformationClass,
            [Out] out SystemPerformanceInformation SystemInformation,
            [In] int SystemInformationLength,
            [Out] [Optional] out int ReturnLength
            );

        [DllImport("ntdll.dll")]
        public static extern int NtQuerySystemInformation(
            [In] SystemInformationClass SystemInformationClass,
            [Out] out SystemCacheInformation SystemInformation,
            [In] int SystemInformationLength,
            [Out] [Optional] out int ReturnLength
            );

        [DllImport("ntdll.dll")]
        public static extern int NtQueryTimer(
            [In] IntPtr TimerHandle,
            [In] TimerInformationClass TimerInformationClass,
            [Out] out TimerBasicInformation TimerInformation,
            [In] int TimerInformationLength,
            [Out] [Optional] out int ReturnLength
            );

        [DllImport("ntdll.dll")]
        public static extern int NtQueueApcThread(
            [In] IntPtr ThreadHandle,
            [In] IntPtr ApcRoutine,
            [In] [Optional] IntPtr ApcArgument1,
            [In] [Optional] IntPtr ApcArgument2,
            [In] [Optional] IntPtr ApcArgument3
            );

        [DllImport("ntdll.dll")]
        public static extern int NtReleaseKeyedEvent(
            [In] IntPtr KeyedEventHandle,
            [In] IntPtr KeyValue,
            [In] bool Alertable,
            [In] [Optional] ref long Timeout
            );

        [DllImport("ntdll.dll")]
        public static extern int NtReleaseMutant(
            [In] IntPtr MutantHandle,
            [Out] [Optional] out int PreviousCount
            );

        [DllImport("ntdll.dll")]
        public static extern int NtReleaseSemaphore(
            [In] IntPtr SemaphoreHandle,
            [In] int ReleaseCount,
            [Out] [Optional] out int PreviousCount
            );

        [DllImport("ntdll.dll")]
        public static extern int NtRemoveProcessDebug(
            [In] IntPtr ProcessHandle,
            [In] IntPtr DebugObjectHandle
            );

        [DllImport("ntdll.dll")]
        public static extern int NtResetEvent(
            [In] IntPtr EventHandle,
            [Out] [Optional] out int PreviousState
            );

        [DllImport("ntdll.dll")]
        public static extern int NtResumeProcess(
            [In] IntPtr ProcessHandle
            );

        [DllImport("ntdll.dll")]
        public static extern int NtResumeThread(
            [In] IntPtr ThreadHandle,
            [Out] [Optional] out int PreviousSuspendCount
            );

        [DllImport("ntdll.dll")]
        public static extern int NtSetEvent(
            [In] IntPtr EventHandle,
            [Out] [Optional] out int PreviousState
            );

        [DllImport("ntdll.dll")]
        public static extern int NtSetEventBoostPriority(
            [In] IntPtr EventHandle
            );

        [DllImport("ntdll.dll")]
        public static extern int NtSetHighEventPair(
            [In] IntPtr EventPairHandle
            );

        [DllImport("ntdll.dll")]
        public static extern int NtSetHighWaitLowEventPair(
            [In] IntPtr EventPairHandle
            );

        [DllImport("ntdll.dll")]
        public static extern int NtSetInformationThread(
            [In] IntPtr ThreadHandle,
            [In] ThreadInformationClass ThreadInformationClass,
            [In] IntPtr ThreadInformation,
            [In] int ThreadInformationLength
            );

        [DllImport("ntdll.dll")]
        public static extern int NtSetLowEventPair(
            [In] IntPtr EventPairHandle
            );

        [DllImport("ntdll.dll")]
        public static extern int NtSetLowWaitHighEventPair(
            [In] IntPtr EventPairHandle
            );

        [DllImport("ntdll.dll")]
        public static extern int NtSetSystemInformation(
            [In] SystemInformationClass SystemInformationClass,
            [In] ref SystemLoadAndCallImage SystemInformation,
            [In] int SystemInformationLength
            );

        [DllImport("ntdll.dll")]
        public static extern int NtSetTimer(
            [In] IntPtr TimerHandle,
            [In] ref long DueTime,
            [In] [Optional] ProcessHacker.Native.Objects.TimerHandle.TimerApcRoutine TimerApcRoutine,
            [In] [Optional] IntPtr TimerContext,
            [In] bool ResumeTimer,
            [In] [Optional] int Period,
            [Out] [Optional] out bool PreviousState
            );

        [DllImport("ntdll.dll")]
        public static extern int NtSignalAndWaitForSingleObject(
            [In] IntPtr SignalHandle,
            [In] IntPtr WaitHandle,
            [In] bool Alertable,
            [In] [Optional] ref long Timeout
            );

        [DllImport("ntdll.dll")]
        public static extern int NtSuspendProcess(
            [In] IntPtr ProcessHandle
            );

        [DllImport("ntdll.dll")]
        public static extern int NtSuspendThread(
            [In] IntPtr ThreadHandle,
            [Out] [Optional] out int PreviousSuspendCount
            );

        [DllImport("ntdll.dll")]
        public static extern int NtUnloadDriver(
            [In] ref UnicodeString DriverPath
            );

        [DllImport("ntdll.dll")]
        public static extern int NtWaitForKeyedEvent(
            [In] int KeyedEventHandle,
            [In] IntPtr KeyValue,
            [In] bool Alertable,
            [In] [Optional] ref long Timeout
            );

        [DllImport("ntdll.dll")]
        public static extern int NtWaitHighEventPair(
            [In] IntPtr EventPairHandle
            );

        [DllImport("ntdll.dll")]
        public static extern int NtWaitLowEventPair(
            [In] IntPtr EventPairHandle
            );
    }
}
