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

// you won't get most of this stuff from anywhere else... :)

namespace ProcessHacker.Native.Api
{
    public partial class Win32
    {
        // IMPORTANT: All timeouts, etc. are in 100ns units except when stated otherwise.

        #region System Calls

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtAcceptConnectPort(
            [Out] out IntPtr PortHandle,
            [In] [Optional] IntPtr PortContext,
            [In] ref PortMessage ConnectionRequest,
            [In] bool AcceptConnection,
            [Optional] ref PortView ServerView,
            [Out] [Optional] out RemotePortView ClientView
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtAlertThread(
            [In] IntPtr ThreadHandle
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtAlertResumeThread(
            [In] IntPtr ThreadHandle,
            [Out] [Optional] out int PreviousSuspendCount
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtAllocateLocallyUniqueId(
            [Out] out Luid Luid
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtAllocateVirtualMemory(
            [In] IntPtr ProcessHandle,
            ref IntPtr BaseAddress,
            [In] IntPtr ZeroBits,
            ref IntPtr RegionSize,
            [In] MemoryFlags AllocationType,
            [In] MemoryProtection Protect
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtAreMappedFilesTheSame(
            [In] IntPtr File1MappedAsAnImage,
            [In] IntPtr File2MappedAsFile
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtAssignProcessToJobObject(
            [In] IntPtr JobHandle,
            [In] IntPtr ProcessHandle
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtCancelTimer(
            [In] IntPtr TimerHandle,
            [Out] [Optional] out bool CurrentState
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtClearEvent(
            [In] IntPtr EventHandle
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtClose(
            [In] IntPtr Handle
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtCompareTokens(
            [In] IntPtr FirstTokenHandle,
            [In] IntPtr SecondTokenHandle,
            [MarshalAs(UnmanagedType.I1)]
            [Out] out bool Equal
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtCompleteConnectPort(
            [In] IntPtr PortHandle
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtConnectPort(
            [Out] out IntPtr PortHandle,
            [In] ref UnicodeString PortName,
            [In] ref SecurityQualityOfService SecurityQos,
            [Optional] ref PortView ClientView,
            [Optional] ref RemotePortView ServerView,
            [Out] [Optional] out int MaxMessageLength,
            [Optional] IntPtr ConnectionInformation,
            [Optional] out int ConnectionInformationLength
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtConnectPort(
            [Out] out IntPtr PortHandle,
            [In] ref UnicodeString PortName,
            [In] ref SecurityQualityOfService SecurityQos,
            [Optional] ref PortView ClientView,
            [Optional] ref RemotePortView ServerView,
            [Out] [Optional] out int MaxMessageLength,
            [Optional] IntPtr ConnectionInformation,
            [Optional] int ConnectionInformationLength
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtCreateDebugObject(
            [Out] out IntPtr DebugObjectHandle,
            [In] DebugObjectAccess DesiredAccess,
            [In] [Optional] ref ObjectAttributes ObjectAttributes,
            [In] DebugObjectFlags Flags
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtCreateDebugObject(
            [Out] out IntPtr DebugObjectHandle,
            [In] DebugObjectAccess DesiredAccess,
            [In] [Optional] IntPtr ObjectAttributes,
            [In] DebugObjectFlags Flags
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtCreateDirectoryObject(
            [Out] out IntPtr DirectoryHandle,
            [In] DirectoryAccess DesiredAccess,
            [In] ref ObjectAttributes ObjectAttributes
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtCreateEvent(
            [Out] out IntPtr EventHandle,
            [In] EventAccess DesiredAccess,
            [In] [Optional] ref ObjectAttributes ObjectAttributes,
            [In] EventType EventType,
            [In] bool InitialState
            ); 

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtCreateEvent(
            [Out] out IntPtr EventHandle,
            [In] EventAccess DesiredAccess,
            [In] [Optional] IntPtr ObjectAttributes,
            [In] EventType EventType,
            [In] bool InitialState
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtCreateEventPair(
            [Out] out IntPtr EventPairHandle,
            [In] EventPairAccess DesiredAccess,
            [In] [Optional] ref ObjectAttributes ObjectAttributes
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtCreateEventPair(
            [Out] out IntPtr EventPairHandle,
            [In] EventPairAccess DesiredAccess,
            [In] [Optional] IntPtr ObjectAttributes
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtCreateJobObject(
            [Out] out IntPtr JobHandle,
            [In] JobObjectAccess DesiredAccess,
            [In] [Optional] ref ObjectAttributes ObjectAttributes
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtCreateJobObject(
            [Out] out IntPtr JobHandle,
            [In] JobObjectAccess DesiredAccess,
            [In] [Optional] IntPtr ObjectAttributes
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtCreateJobSet(
            [In] int NumJob,
            JobSetArray[] UserJobSet,
            [In] int Flags
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtCreateKeyedEvent(
            [Out] out IntPtr KeyedEventHandle,
            [In] KeyedEventAccess DesiredAccess,
            [In] [Optional] ref ObjectAttributes ObjectAttributes,
            [In] int Flags
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtCreateKeyedEvent(
            [Out] out IntPtr KeyedEventHandle,
            [In] KeyedEventAccess DesiredAccess,
            [In] [Optional] IntPtr ObjectAttributes,
            [In] int Flags
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtCreateMutant(
            [Out] out IntPtr MutantHandle,
            [In] MutantAccess DesiredAccess,
            [In] [Optional] ref ObjectAttributes ObjectAttributes,
            [In] bool InitialOwner
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtCreateMutant(
            [Out] out IntPtr MutantHandle,
            [In] MutantAccess DesiredAccess,
            [In] [Optional] IntPtr ObjectAttributes,
            [In] bool InitialOwner
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtCreatePort(
            [Out] out IntPtr PortHandle,
            [In] ref ObjectAttributes ObjectAttributes,
            [In] int MaxConnectionInfoLength,
            [In] int MaxMessageLength,
            [In] [Optional] int MaxPoolUsage
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtCreateProcess(
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
        public static extern NtStatus NtCreateProcess(
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
        public static extern NtStatus NtCreateProfile(
            [Out] out IntPtr ProfileHandle,
            [In] IntPtr ProcessHandle,
            [In] IntPtr ProfileBase,
            [In] IntPtr ProfileSize,
            [In] int BucketSize,
            [In] IntPtr Buffer,
            [In] int BufferSize,
            [In] KProfileSource ProfileSource,
            [In] IntPtr Affinity
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtCreateSection(
            [Out] out IntPtr SectionHandle,
            [In] SectionAccess DesiredAccess,
            [In] [Optional] ref ObjectAttributes ObjectAttributes,
            [In] [Optional] ref long MaximumSize,
            [In] int PageAttributes,
            [In] int SectionAttributes,
            [In] [Optional] IntPtr FileHandle
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtCreateSection(
            [Out] out IntPtr SectionHandle,
            [In] SectionAccess DesiredAccess,
            [In] [Optional] IntPtr ObjectAttributes,
            [In] [Optional] ref long MaximumSize,
            [In] int PageAttributes,
            [In] int SectionAttributes,
            [In] [Optional] IntPtr FileHandle
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtCreateSemaphore(
            [Out] out IntPtr SemaphoreHandle,
            [In] SemaphoreAccess DesiredAccess,
            [In] [Optional] ref ObjectAttributes ObjectAttributes,
            [In] int InitialCount,
            [In] int MaximumCount
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtCreateSemaphore(
            [Out] out IntPtr SemaphoreHandle,
            [In] SemaphoreAccess DesiredAccess,
            [In] [Optional] IntPtr ObjectAttributes,
            [In] int InitialCount,
            [In] int MaximumCount
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtCreateSymbolicLinkObject(
            [Out] out IntPtr LinkHandle,
            [In] SymbolicLinkAccess DesiredAccess,
            [In] ref ObjectAttributes ObjectAttributes,
            [In] ref UnicodeString LinkTarget
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtCreateThread(
            [Out] out IntPtr ThreadHandle,
            [In] ThreadAccess DesiredAccess,
            [In] [Optional] ref ObjectAttributes ObjectAttributes,
            [In] IntPtr ProcessHandle,
            [Out] out ClientId ClientId,
            [In] ref Context ThreadContext,
            [In] ref InitialTeb InitialTeb,
            [In] bool CreateSuspended
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtCreateTimer(
            [Out] out IntPtr TimerHandle,
            [In] TimerAccess DesiredAccess,
            [In] [Optional] ref ObjectAttributes ObjectAttributes,
            [In] TimerType TimerType
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtCreateTimer(
            [Out] out IntPtr TimerHandle,
            [In] TimerAccess DesiredAccess,
            [In] [Optional] IntPtr ObjectAttributes,
            [In] TimerType TimerType
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtCreateWaitablePort(
            [Out] out IntPtr PortHandle,
            [In] ref ObjectAttributes ObjectAttributes,
            [In] int MaxConnectionInfoLength,
            [In] int MaxMessageLength,
            [In] [Optional] int MaxPoolUsage
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtDebugActiveProcess(
            [In] IntPtr ProcessHandle,
            [In] IntPtr DebugObjectHandle
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtDebugContinue(
            [In] IntPtr DebugObjectHandle,
            [In] ref ClientId ClientId,
            [In] NtStatus ContinueStatus
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtDelayExecution(
            [In] bool Alertable,
            [In] ref long DelayInterval
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtDeviceIoControlFile(
            [In] IntPtr FileHandle,
            [In] IntPtr Event,
            [In] IntPtr ApcRoutine,
            [In] IntPtr ApcContext,
            [Out] out IoStatusBlock IoStatusBlock,
            [In] int IoControlCode,
            [In] IntPtr InputBuffer,
            [In] int InputBufferLength,
            [In] IntPtr OutputBuffer,
            [In] int OutputBufferLength
            );

        [DllImport("ntdll.dll")]
        public unsafe static extern NtStatus NtDeviceIoControlFile(
            [In] IntPtr FileHandle,
            [In] IntPtr Event,
            [In] IntPtr ApcRoutine,
            [In] IntPtr ApcContext,
            [Out] out IoStatusBlock IoStatusBlock,
            [In] int IoControlCode,
            [In] void* InputBuffer,
            [In] int InputBufferLength,
            [In] void* OutputBuffer,
            [In] int OutputBufferLength
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtDuplicateObject(
            [In] IntPtr SourceProcessHandle,
            [In] IntPtr SourceHandle,
            [In] IntPtr TargetProcessHandle,
            [Out] out IntPtr TargetHandle,
            [In] int DesiredAccess,
            [In] HandleFlags Attributes,
            [In] DuplicateOptions Options
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtDuplicateToken(
            [In] IntPtr ExistingTokenHandle,
            [In] TokenAccess DesiredAccess,
            [In] ref ObjectAttributes ObjectAttributes,
            [In] bool EffectiveOnly,
            [In] TokenType TokenType,
            [Out] out IntPtr NewTokenHandle
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtExtendSection(
            [In] IntPtr SectionHandle,
            ref long NewSectionSize
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtFlushVirtualMemory(
            [In] IntPtr ProcessHandle,
            ref IntPtr BaseAddress,
            ref IntPtr RegionSize,
            [Out] out IoStatusBlock IoStatus
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtFreeVirtualMemory(
            [In] IntPtr ProcessHandle,
            ref IntPtr BaseAddress,
            ref IntPtr RegionSize,
            [In] MemoryFlags FreeType
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtGetContextThread(
            [In] IntPtr ThreadHandle,
            ref Context ThreadContext
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtGetCurrentProcessorNumber();

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtGetNextProcess(
            [In] [Optional] IntPtr ProcessHandle,
            [In] ProcessAccess DesiredAccess,
            [In] HandleFlags HandleAttributes,
            [In] int Flags,
            [Out] out IntPtr NewProcessHandle
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtGetNextThread(
            [In] [Optional] IntPtr ProcessHandle,
            [In] [Optional] IntPtr ThreadHandle,
            [In] ThreadAccess DesiredAccess,
            [In] HandleFlags HandleAttributes,
            [In] int Flags,
            [Out] out IntPtr NewThreadHandle
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtImpersonateAnonymousToken(
            [In] IntPtr ThreadHandle
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtImpersonateClientOfPort(
            [In] IntPtr PortHandle,
            [In] ref PortMessage Message
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtImpersonateThread(
            [In] IntPtr ServerThreadHandle,
            [In] IntPtr ClientThreadHandle,
            [In] ref SecurityQualityOfService SecurityQos
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtIsProcessInJob(
            [In] IntPtr ProcessHandle,
            [In] [Optional] IntPtr JobHandle
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtListenPort(
            [In] IntPtr PortHandle,
            [Out] out PortMessage ConnectionRequest
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtLoadDriver(
            [In] ref UnicodeString DriverPath
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtLockVirtualMemory(
            [In] IntPtr ProcessHandle,
            ref IntPtr BaseAddress,
            ref IntPtr RegionSize,
            [In] MemoryFlags MapType
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtMakePermanentObject(
            [In] IntPtr Handle
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtMakeTemporaryObject(
            [In] IntPtr Handle
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtMapViewOfSection(
            [In] IntPtr SectionHandle,
            [In] IntPtr ProcessHandle,
            ref IntPtr BaseAddress,
            [In] IntPtr ZeroBits,
            [In] IntPtr CommitSize,
            [Optional] ref long SectionOffset,
            ref IntPtr ViewSize,
            [In] SectionInherit InheritDisposition,
            [In] MemoryFlags AllocationType,
            [In] MemoryProtection Win32Protect
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtOpenDirectoryObject(
            [Out] out IntPtr DirectoryHandle,
            [In] DirectoryAccess DesiredAccess,
            [In] ref ObjectAttributes ObjectAttributes
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtOpenEvent(
            [Out] out IntPtr EventHandle,
            [In] EventAccess DesiredAccess,
            [In] ref ObjectAttributes ObjectAttributes
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtOpenEventPair(
            [Out] out IntPtr EventPairHandle,
            [In] EventPairAccess DesiredAccess,
            [In] ref ObjectAttributes ObjectAttributes
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtOpenJobObject(
            [Out] out IntPtr JobHandle,
            [In] JobObjectAccess DesiredAccess,
            [In] ref ObjectAttributes ObjectAttributes
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtOpenKeyedEvent(
            [Out] out IntPtr KeyedEventHandle,
            [In] KeyedEventAccess DesiredAccess,
            [In] ref ObjectAttributes ObjectAttributes
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtOpenMutant(
            [Out] out IntPtr MutantHandle,
            [In] MutantAccess DesiredAccess,
            [In] ref ObjectAttributes ObjectAttributes
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtOpenProcess(
            [Out] out IntPtr ProcessHandle,
            [In] ProcessAccess DesiredAccess,
            [In] ref ObjectAttributes ObjectAttributes,
            [In] [Optional] ref ClientId ClientId
            );      

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtOpenProcess(
            [Out] out IntPtr ProcessHandle,
            [In] ProcessAccess DesiredAccess,
            [In] ref ObjectAttributes ObjectAttributes,
            [In] [Optional] IntPtr ClientId
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtOpenProcessToken(
            [In] IntPtr ProcessHandle,
            [In] TokenAccess DesiredAccess,
            [Out] out IntPtr TokenHandle
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtOpenProcessTokenEx(
            [In] IntPtr ProcessHandle,
            [In] TokenAccess DesiredAccess,
            [In] int HandleAttributes,
            [Out] out IntPtr TokenHandle
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtOpenSection(
            [Out] out IntPtr SectionHandle,
            [In] SectionAccess DesiredAccess,
            [In] ref ObjectAttributes ObjectAttributes
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtOpenSemaphore(
            [Out] out IntPtr SemaphoreHandle,
            [In] SemaphoreAccess DesiredAccess,
            [In] ref ObjectAttributes ObjectAttributes
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtOpenSymbolicLinkObject(
            [Out] out IntPtr LinkHandle,
            [In] SymbolicLinkAccess DesiredAccess,
            [In] ref ObjectAttributes ObjectAttributes
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtOpenThread(
            [Out] out IntPtr ThreadHandle,
            [In] ThreadAccess DesiredAccess,
            [In] ref ObjectAttributes ObjectAttributes,
            [In] [Optional] ref ClientId ClientId
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtOpenThread(
            [Out] out IntPtr ThreadHandle,
            [In] ThreadAccess DesiredAccess,
            [In] ref ObjectAttributes ObjectAttributes,
            [In] [Optional] IntPtr ClientId
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtOpenThreadToken(
            [In] IntPtr ThreadHandle,
            [In] TokenAccess DesiredAccess,
            [In] bool OpenAsSelf,
            [Out] out IntPtr TokenHandle
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtOpenThreadTokenEx(
            [In] IntPtr ThreadHandle,
            [In] TokenAccess DesiredAccess,
            [In] bool OpenAsSelf,
            [In] int HandleAttributes,
            [Out] out IntPtr TokenHandle
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtOpenTimer(
            [Out] out IntPtr TimerHandle,
            [In] TimerAccess DesiredAccess,
            [In] ref ObjectAttributes ObjectAttributes
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtProtectVirtualMemory(
            [In] IntPtr ProcessHandle,
            ref IntPtr BaseAddress,
            ref IntPtr RegionSize,
            [In] MemoryProtection NewProtect,
            [Out] out MemoryProtection OldProtect
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtPulseEvent(
            [In] IntPtr EventHandle,
            [Out] [Optional] out int PreviousState
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtQueryDirectoryObject(
            [In] IntPtr DirectoryHandle,
            [In] IntPtr Buffer,
            [In] int Length,
            [In] bool ReturnSingleEntry,
            [In] bool RestartScan,
            ref int Context,
            [Out] [Optional] out int ReturnLength
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtQueryEvent(
            [In] IntPtr EventHandle,
            [In] EventInformationClass EventInformationClass,
            [Out] out EventBasicInformation EventInformation,
            [In] int EventInformationLength,
            [Out] [Optional] out int ReturnLength
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtQueryInformationJobObject(
            [In] [Optional] IntPtr JobHandle,
            [In] JobObjectInformationClass JobObjectInformationClass,
            [In] IntPtr JobObjectInformation,
            [In] int JobObjectInformationLength,
            [Out] [Optional] out int ReturnLength
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtQueryInformationProcess(
            [In] IntPtr ProcessHandle,
            [In] ProcessInformationClass ProcessInformationClass,
            IntPtr ProcessInformation,
            [In] int ProcessInformationLength,
            [Out] [Optional] out int ReturnLength
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtQueryInformationProcess(
            [In] IntPtr ProcessHandle,
            [In] ProcessInformationClass ProcessInformationClass,
            [Out] out int ProcessInformation,
            [In] int ProcessInformationLength,
            [Out] [Optional] out int ReturnLength
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtQueryInformationProcess(
            [In] IntPtr ProcessHandle,
            [In] ProcessInformationClass ProcessInformationClass,
            [Out] out PooledUsageAndLimits ProcessInformation,
            [In] int ProcessInformationLength,
            [Out] [Optional] out int ReturnLength
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtQueryInformationProcess(
            [In] IntPtr ProcessHandle,
            [In] ProcessInformationClass ProcessInformationClass,
            [Out] out QuotaLimits ProcessInformation,
            [In] int ProcessInformationLength,
            [Out] [Optional] out int ReturnLength
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtQueryInformationProcess(
            [In] IntPtr ProcessHandle,
            [In] ProcessInformationClass ProcessInformationClass,
            [Out] out MemExecuteOptions ProcessInformation,
            [In] int ProcessInformationLength,
            [Out] [Optional] out int ReturnLength
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtQueryInformationProcess(
            [In] IntPtr ProcessHandle,
            [In] ProcessInformationClass ProcessInformationClass,
            [Out] out ProcessBasicInformation ProcessInformation,
            [In] int ProcessInformationLength,
            [Out] [Optional] out int ReturnLength
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtQueryInformationProcess(
            [In] IntPtr ProcessHandle,
            [In] ProcessInformationClass ProcessInformationClass,
            [Out] out UnicodeString ProcessInformation,
            [In] int ProcessInformationLength,
            [Out] [Optional] out int ReturnLength
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtQueryInformationThread(
            [In] IntPtr ThreadHandle,
            [In] ThreadInformationClass ThreadInformationClass,
            ref ThreadBasicInformation ThreadInformation,
            [In] int ThreadInformationLength,
            [Out] [Optional] out int ReturnLength
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtQueryInformationThread(
            [In] IntPtr ThreadHandle,
            [In] ThreadInformationClass ThreadInformationClass,
            [Out] out int ThreadInformation,
            [In] int ThreadInformationLength,
            [Out] [Optional] out int ReturnLength
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtQueryInformationThread(
            [In] IntPtr ThreadHandle,
            [In] ThreadInformationClass ThreadInformationClass,
            IntPtr ThreadInformation,
            [In] int ThreadInformationLength,
            [Out] [Optional] out int ReturnLength
            );

        [DllImport("ntdll.dll")]
        public unsafe static extern NtStatus NtQueryInformationThread(
            [In] IntPtr ThreadHandle,
            [In] ThreadInformationClass ThreadInformationClass,
            void* ThreadInformation,
            [In] int ThreadInformationLength,
            [Out] [Optional] out int ReturnLength
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtQueryIntervalProfile(
            [In] KProfileSource Source,
            [Out] out int Interval 
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtQueryMutant(
            [In] IntPtr MutantHandle,
            [In] MutantInformationClass MutantInformationClass,
            [Out] out MutantBasicInformation MutantInformation,
            [In] int MutantInformationLength,
            [Out] [Optional] out int ReturnLength
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtQueryObject(
            [In] IntPtr Handle,
            [In] ObjectInformationClass ObjectInformationClass,
            [Out] IntPtr ObjectInformation,
            [In] int ObjectInformationLength,
            [Out] out int ReturnLength
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtQueryPortInformationProcess();

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtQuerySection(
            [In] IntPtr SectionHandle,
            [In] SectionInformationClass SectionInformationClass,
            [Out] out SectionBasicInformation SectionInformation,
            [In] IntPtr SectionInformationLength,
            [Out] [Optional] out IntPtr ReturnLength
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtQuerySection(
            [In] IntPtr SectionHandle,
            [In] SectionInformationClass SectionInformationClass,
            [Out] out SectionImageInformation SectionInformation,
            [In] IntPtr SectionInformationLength,
            [Out] [Optional] out IntPtr ReturnLength
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtQuerySecurityObject(
            [In] IntPtr Handle,
            [In] SecurityInformation SecurityInformation,
            [In] IntPtr SecurityDescriptor,
            [In] int SecurityDescriptorLength,
            [Out] out int ReturnLength
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtQuerySemaphore(
            [In] IntPtr SemaphoreHandle,
            [In] SemaphoreInformationClass SemaphoreInformationClass,
            [Out] out SemaphoreBasicInformation SemaphoreInformation,
            [In] int SemaphoreInformationLength,
            [Out] [Optional] out int ReturnLength
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtQuerySymbolicLinkObject(
            [In] IntPtr LinkHandle,
            ref UnicodeString LinkName,
            [Out] [Optional] out int ReturnLength
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtQuerySystemInformation(
            [In] SystemInformationClass SystemInformationClass,
            [Out] out SystemBasicInformation SystemInformation,
            [In] int SystemInformationLength,
            [Out] [Optional] out int ReturnLength
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtQuerySystemInformation(
            [In] SystemInformationClass SystemInformationClass,
            IntPtr SystemInformation,
            [In] int SystemInformationLength,
            [Out] [Optional] out int ReturnLength
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtQuerySystemInformation(
            [In] SystemInformationClass SystemInformationClass,
            [MarshalAs(UnmanagedType.LPArray)] SystemProcessorPerformanceInformation[] SystemInformation,
            [In] int SystemInformationLength,
            [Out] [Optional] out int ReturnLength
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtQuerySystemInformation(
            [In] SystemInformationClass SystemInformationClass,
            [Out] out SystemPerformanceInformation SystemInformation,
            [In] int SystemInformationLength,
            [Out] [Optional] out int ReturnLength
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtQuerySystemInformation(
            [In] SystemInformationClass SystemInformationClass,
            [Out] out SystemCacheInformation SystemInformation,
            [In] int SystemInformationLength,
            [Out] [Optional] out int ReturnLength
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtQueryTimer(
            [In] IntPtr TimerHandle,
            [In] TimerInformationClass TimerInformationClass,
            [Out] out TimerBasicInformation TimerInformation,
            [In] int TimerInformationLength,
            [Out] [Optional] out int ReturnLength
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtQueryVirtualMemory(
            [In] IntPtr ProcessHandle,
            [In] IntPtr BaseAddress,
            [In] MemoryInformationClass MemoryInformationClass,
            [In] IntPtr Buffer,
            [In] IntPtr MemoryInformationLength,
            [Out] [Optional] out IntPtr ReturnLength
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtQueueApcThread(
            [In] IntPtr ThreadHandle,
            [In] IntPtr ApcRoutine,
            [In] [Optional] IntPtr ApcArgument1,
            [In] [Optional] IntPtr ApcArgument2,
            [In] [Optional] IntPtr ApcArgument3
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtReadRequestData(
            [In] IntPtr PortHandle,
            [In] ref PortMessage Message,
            [In] int DataEntryIndex,
            [In] IntPtr Buffer,
            [In] IntPtr BufferSize,
            [Out] [Optional] out IntPtr ReturnLength
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtReadVirtualMemory(
            [In] IntPtr ProcessHandle,
            [In] [Optional] IntPtr BaseAddress,
            [In] IntPtr Buffer,
            [In] IntPtr BufferSize,
            [Out] [Optional] out IntPtr ReturnLength
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtRegisterThreadTerminatePort(
            [In] IntPtr PortHandle
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtReleaseKeyedEvent(
            [In] IntPtr KeyedEventHandle,
            [In] IntPtr KeyValue,
            [In] bool Alertable,
            [In] [Optional] ref long Timeout
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtReleaseMutant(
            [In] IntPtr MutantHandle,
            [Out] [Optional] out int PreviousCount
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtReleaseSemaphore(
            [In] IntPtr SemaphoreHandle,
            [In] int ReleaseCount,
            [Out] [Optional] out int PreviousCount
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtRemoveProcessDebug(
            [In] IntPtr ProcessHandle,
            [In] IntPtr DebugObjectHandle
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtReplyPort(
            [In] IntPtr PortHandle,
            [In] ref PortMessage ReplyMessage
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtReplyWaitReceivePort(
            [In] IntPtr PortHandle,
            [Out] [Optional] out IntPtr PortContext,
            [In] [Optional] ref PortMessage ReplyMessage,
            [Out] out PortMessage ReceiveMessage
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtReplyWaitReceivePortEx(
            [In] IntPtr PortHandle,
            [Out] [Optional] out IntPtr PortContext,
            [In] [Optional] ref PortMessage ReplyMessage,
            [Out] out PortMessage ReceiveMessage,
            [In] [Optional] ref long Timeout
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtReplyWaitReplyPort(
            [In] IntPtr PortHandle,
            ref PortMessage ReplyMessage
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtRequestPort(
            [In] IntPtr PortHandle,
            [In] ref PortMessage RequestMessage
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtRequestWaitReplyPort(
            [In] IntPtr PortHandle,
            [In] ref PortMessage RequestMessage,
            [Out] out PortMessage ReplyMessage
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtResetEvent(
            [In] IntPtr EventHandle,
            [Out] [Optional] out int PreviousState
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtResumeProcess(
            [In] IntPtr ProcessHandle
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtResumeThread(
            [In] IntPtr ThreadHandle,
            [Out] [Optional] out int PreviousSuspendCount
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtSetContextThread(
            [In] IntPtr ThreadHandle,
            [In] ref Context ThreadContext
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtSetEvent(
            [In] IntPtr EventHandle,
            [Out] [Optional] out int PreviousState
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtSetEventBoostPriority(
            [In] IntPtr EventHandle
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtSetHighEventPair(
            [In] IntPtr EventPairHandle
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtSetHighWaitLowEventPair(
            [In] IntPtr EventPairHandle
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtSetInformationDebugObject(
            [In] IntPtr DebugObjectHandle,
            [In] DebugObjectInformationClass DebugObjectInformationClass,
            [In] IntPtr DebugObjectInformation,
            [In] int DebugObjectInformationLength,
            [Out] [Optional] out int ReturnLength
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtSetInformationJobObject(
            [In] IntPtr JobHandle,
            [In] JobObjectInformationClass JobObjectInformationClass,
            [In] IntPtr JobObjectInformation,
            [In] int JobObjectInformationLength
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtSetInformationObject(
            [In] IntPtr Handle,
            [In] ObjectInformationClass ObjectInformationClass,
            [In] IntPtr ObjectInformation,
            [In] int ObjectInformationLength
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtSetInformationProcess(
            [In] IntPtr ProcessHandle,
            [In] ProcessInformationClass ProcessInformationClass,
            [In] IntPtr ProcessInformation,
            [In] int ProcessInformationLength
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtSetInformationProcess(
            [In] IntPtr ProcessHandle,
            [In] ProcessInformationClass ProcessInformationClass,
            [In] ref int ProcessInformation,
            [In] int ProcessInformationLength
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtSetInformationThread(
            [In] IntPtr ThreadHandle,
            [In] ThreadInformationClass ThreadInformationClass,
            [In] IntPtr ThreadInformation,
            [In] int ThreadInformationLength
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtSetInformationThread(
            [In] IntPtr ThreadHandle,
            [In] ThreadInformationClass ThreadInformationClass,
            [In] ref int ThreadInformation,
            [In] int ThreadInformationLength
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtSetIntervalProfile(
            [In] int Interval,
            [In] KProfileSource Source
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtSetLowEventPair(
            [In] IntPtr EventPairHandle
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtSetLowWaitHighEventPair(
            [In] IntPtr EventPairHandle
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtSetSecurityObject(
            [In] IntPtr Handle,
            [In] SecurityInformation SecurityInformation,
            [In] IntPtr SecurityDescriptor
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtSetSystemInformation(
            [In] SystemInformationClass SystemInformationClass,
            [In] ref SystemLoadAndCallImage SystemInformation,
            [In] int SystemInformationLength
            );

        /// <param name="Period">Period, in milliseconds.</param>
        [DllImport("ntdll.dll")]
        public static extern NtStatus NtSetTimer(
            [In] IntPtr TimerHandle,
            [In] ref long DueTime,
            [In] [Optional] TimerApcRoutine TimerApcRoutine,
            [In] [Optional] IntPtr TimerContext,
            [In] bool ResumeTimer,
            [In] [Optional] int Period,
            [Out] [Optional] out bool PreviousState
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtSignalAndWaitForSingleObject(
            [In] IntPtr SignalHandle,
            [In] IntPtr WaitHandle,
            [In] bool Alertable,
            [In] [Optional] ref long Timeout
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtStartProfile(
            [In] IntPtr ProfileHandle
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtStopProfile(
            [In] IntPtr ProfileHandle
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtSuspendProcess(
            [In] IntPtr ProcessHandle
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtSuspendThread(
            [In] IntPtr ThreadHandle,
            [Out] [Optional] out int PreviousSuspendCount
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtTerminateJobObject(
            [In] IntPtr JobHandle,
            [In] int ExitStatus
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtTerminateProcess(
            [In] [Optional] IntPtr ProcessHandle,
            [In] int ExitStatus
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtTerminateThread(
            [In] [Optional] IntPtr ThreadHandle,
            [In] int ExitStatus
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtTestAlert();

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtUnloadDriver(
            [In] ref UnicodeString DriverPath
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtUnlockVirtualMemory(
            [In] IntPtr ProcessHandle,
            ref IntPtr BaseAddress,
            ref IntPtr RegionSize,
            [In] MemoryFlags MapType
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtUnmapViewOfSection(
            [In] IntPtr ProcessHandle,
            [In] IntPtr BaseAddress
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtWaitForDebugEvent(
            [In] IntPtr DebugObjectHandle,
            [In] bool Alertable,
            [In] [Optional] ref long Timeout,
            [Out] out DbgUiWaitStateChange WaitStateChange
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtWaitForKeyedEvent(
            [In] IntPtr KeyedEventHandle,
            [In] IntPtr KeyValue,
            [In] bool Alertable,
            [In] [Optional] ref long Timeout
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtWaitForMultipleObjects(
            [In] int Count,
            [In] IntPtr[] Handles,
            [In] WaitType WaitType,
            [In] bool Alertable,
            [In] [Optional] ref long Timeout
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtWaitForMultipleObjects32(
            [In] int Count,
            [In] int[] Handles,
            [In] WaitType WaitType,
            [In] bool Alertable,
            [In] [Optional] ref long Timeout
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtWaitForSingleObject(
            [In] IntPtr Handle,
            [In] bool Alertable,
            [In] [Optional] ref long Timeout
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtWaitHighEventPair(
            [In] IntPtr EventPairHandle
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtWaitLowEventPair(
            [In] IntPtr EventPairHandle
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtWriteRequestData(
            [In] IntPtr PortHandle,
            [In] ref PortMessage Message,
            [In] int DataEntryIndex,
            [In] IntPtr Buffer,
            [In] IntPtr BufferSize,
            [Out] [Optional] out IntPtr ReturnLength
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtWriteVirtualMemory(
            [In] IntPtr ProcessHandle,
            [In] [Optional] IntPtr BaseAddress,
            [In] IntPtr Buffer,
            [In] IntPtr BufferSize,
            [Out] [Optional] out IntPtr ReturnLength
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtYieldExecution();

        #endregion

        #region Run-Time Library

        #region Processes and Threads

        [DllImport("ntdll.dll")]
        public static extern void RtlAcquirePebLock();

        [DllImport("ntdll.dll")]
        public static extern NtStatus RtlAllocateFromPeb(
            [In] int Size,
            [Out] out IntPtr Block
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus RtlCreateEnvironment(
            [In] bool CloneCurrentEnvironment,
            [Out] out IntPtr Environment
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus RtlCreateProcessParameters(
            [Out] out IntPtr ProcessParameters,
            [In] ref UnicodeString ImagePathName,
            [In] ref UnicodeString DllPath,
            [In] ref UnicodeString CurrentDirectory,
            [In] ref UnicodeString CommandLine,
            [In] IntPtr Environment,
            [In] ref UnicodeString WindowTitle,
            [In] ref UnicodeString DesktopInfo,
            [In] ref UnicodeString ShellInfo,
            [In] ref UnicodeString RuntimeData
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus RtlCreateUserProcess(
            [In] ref UnicodeString NtImagePathName,
            [In] int Attributes,
            [In] IntPtr ProcessParameters,
            [In] IntPtr ProcessSecurityDescriptor,
            [In] IntPtr ThreadSecurityDescriptor,
            [In] IntPtr ParentProcess,
            [In] bool InheritHandles,
            [In] IntPtr DebugPort,
            [In] IntPtr ExceptionPort,
            [In] ref RtlUserProcessInformation ProcessInformation
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus RtlCreateUserThread(
            [In] IntPtr Process,
            [In] IntPtr ThreadSecurityDescriptor,
            [In] bool CreateSuspended,
            [In] int StackZeroBits,
            [In] [Optional] IntPtr MaximumStackSize,
            [In] [Optional] IntPtr InitialStackSize,
            [In] IntPtr StartAddress,
            [In] IntPtr Parameter,
            [Out] out IntPtr Thread,
            [Out] out ClientId ClientId
            );

        [DllImport("ntdll.dll")]
        public static extern IntPtr RtlDeNormalizeProcessParameters(
            [In] IntPtr ProcessParameters
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus RtlDestroyEnvironment(
            [In] IntPtr Environment
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus RtlDestroyProcessParameters(
            [In] IntPtr ProcessParameters
            );

        [DllImport("ntdll.dll")]
        public static extern void RtlExitUserThread(
            [In] int ExitStatus
            );

        [DllImport("ntdll.dll")]
        public static extern void RtlFreeUserThreadStack(
            [In] IntPtr Process,
            [In] IntPtr Thread
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus RtlFreeToPeb(
            [In] IntPtr Block,
            [In] int Size
            );

        [DllImport("ntdll.dll")]
        public static extern void RtlInitializeContext(
            [In] IntPtr Process,
            ref Context Context,
            [In] IntPtr Parameter,
            [In] IntPtr InitialPc,
            [In] IntPtr InitialSp
            );

        [DllImport("ntdll.dll")]
        public static extern IntPtr RtlNormalizeProcessParameters(
            [In] IntPtr ProcessParameters
            );

        [DllImport("ntdll.dll")]
        public static extern int RtlNtStatusToDosError(
            [In] NtStatus Status
            );

        [DllImport("ntdll.dll")]
        public static extern void RtlReleasePebLock();

        [DllImport("ntdll.dll")]
        public static extern NtStatus RtlRemoteCall(
            [In] IntPtr Process,
            [In] IntPtr Thread,
            [In] IntPtr CallSite,
            [In] int ArgumentCount,
            [In] IntPtr[] Arguments,
            [In] bool PassContext,
            [In] bool AlreadySuspended
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus RtlSetCurrentEnvironment(
            [In] IntPtr Environment,
            [Out] out IntPtr PreviousEnvironment
            );

        #endregion

        #region Security ID Routines

        [DllImport("ntdll.dll")]
        public static extern NtStatus RtlAllocateAndInitializeSid(
            [In] ref SidIdentifierAuthority IdentifierAuthority,
            [In] int SubAuthorityCount,
            [In] int SubAuthority0,
            [In] int SubAuthority1,
            [In] int SubAuthority2,
            [In] int SubAuthority3,
            [In] int SubAuthority4,
            [In] int SubAuthority5,
            [In] int SubAuthority6,
            [In] int SubAuthority7,
            [Out] out IntPtr Sid
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus RtlConvertSidToUnicodeString(
            ref UnicodeString UnicodeString,
            [In] IntPtr Sid,
            [In] bool AllocateDestinationString
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus RtlCopySid(
            [In] int DestinationSidLength,
            [In] IntPtr DestinationSid,
            [In] IntPtr SourceSid
            );

        [DllImport("ntdll.dll")]  
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool RtlEqualSid(
            [In] IntPtr Sid1,
            [In] IntPtr Sid2
            );

        [DllImport("ntdll.dll")]  
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool RtlEqualPrefixSid(
            [In] IntPtr Sid1,
            [In] IntPtr Sid2
            );

        [DllImport("ntdll.dll")]
        public static extern IntPtr RtlFreeSid(
            [In] IntPtr Sid
            );

        [DllImport("ntdll.dll")]
        public unsafe static extern SidIdentifierAuthority* RtlIdentifierAuthoritySid(
            [In] IntPtr Sid
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus RtlInitializeSid(
            [In] IntPtr Sid,
            [In] ref SidIdentifierAuthority IdentifierAuthority,
            [In] int SubAuthorityCount
            );

        [DllImport("ntdll.dll")]
        public static extern int RtlLengthRequiredSid(
            [In] int SubAuthorityCount
            );

        [DllImport("ntdll.dll")]
        public static extern int RtlLengthSid(
            [In] IntPtr Sid
            );

        [DllImport("ntdll.dll")]
        public unsafe static extern int* RtlSubAuthoritySid(
            [In] IntPtr Sid,
            [In] int SubAuthority
            );

        [DllImport("ntdll.dll")]
        public unsafe static extern byte* RtlSubAuthorityCountSid(
            [In] IntPtr Sid
            );

        [DllImport("ntdll.dll")]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool RtlValidSid(
            [In] IntPtr Sid
            );

        #endregion

        #region Strings

        [DllImport("ntdll.dll")]
        public static extern int RtlCompareUnicodeString(
            [In] ref UnicodeString String1,
            [In] ref UnicodeString String2,
            [In] bool CaseInSensitive
            );

        [DllImport("ntdll.dll", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool RtlCreateUnicodeString(
            [Out] out UnicodeString DestinationString,
            [In] string SourceString
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus RtlDuplicateUnicodeString(
            [In] RtlDuplicateUnicodeStringFlags Flags,
            [In] ref UnicodeString StringIn,
            [Out] out UnicodeString StringOut
            );

        [DllImport("ntdll.dll")]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool RtlEqualUnicodeString(
            [In] ref UnicodeString String1,
            [In] ref UnicodeString String2,
            [In] bool CaseInSensitive
            );

        [DllImport("ntdll.dll")]
        public static extern void RtlFreeUnicodeString(
            [In] ref UnicodeString UnicodeString
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus RtlHashUnicodeString(
            [In] ref UnicodeString String,
            [In] bool CaseInSensitive,
            [In] HashStringAlgorithm HashAlgorithm,
            [Out] out int HashValue
            );

        [DllImport("ntdll.dll")]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool RtlPrefixUnicodeString(
            [In] ref UnicodeString String1,
            [In] ref UnicodeString String2,
            [In] bool CaseInSensitive
            );

        [DllImport("ntdll.dll")]
        public static extern NtStatus RtlValidateUnicodeString(
            [In] int Flags,
            [In] ref UnicodeString String
            );

        #endregion

        #endregion
    }
}
