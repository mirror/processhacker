/*
 * Process Hacker Driver - 
 *   processes and threads
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

#ifndef _PS_H
#define _PS_H

#include "types.h"
#include "ex.h"
#include "mm.h"
#include "ob.h"
#include "se.h"

extern POBJECT_TYPE *PsJobType;

/* FUNCTION DEFS */

NTSTATUS NTAPI PsGetContextThread(
    PETHREAD Thread,
    PCONTEXT ThreadContext,
    KPROCESSOR_MODE PreviousMode
    );

PVOID NTAPI PsGetThreadWin32Thread(
    PETHREAD Thread
    );

NTSTATUS NTAPI PsLookupProcessThreadByCid(
    PCLIENT_ID ClientId,
    PEPROCESS *Process,
    PETHREAD *Thread
    );

NTSTATUS NTAPI PsSetContextThread(
    PETHREAD Thread,
    PCONTEXT ThreadContext,
    KPROCESSOR_MODE PreviousMode
    );

/* FUNCTION TYPEDEFS */

typedef PVOID (NTAPI *_PsGetProcessJob)(
    PEPROCESS Process
    );

typedef NTSTATUS (NTAPI *_PsResumeProcess)(
    PEPROCESS Process
    );

typedef NTSTATUS (NTAPI *_PsSuspendProcess)(
    PEPROCESS Process
    );

typedef NTSTATUS (__fastcall *_PsTerminateProcess)(
    PEPROCESS Process,
    NTSTATUS ExitStatus
    );

/* STRUCTS */

typedef struct _KEXECUTE_OPTIONS
{
    ULONG ExecuteDisable: 1;
    ULONG ExecuteEnable: 1;
    ULONG DisableThunkEmulation: 1;
    ULONG Permanent: 1;
    ULONG ExecuteDispatchEnable: 1;
    ULONG ImageDispatchEnable: 1;
    ULONG Spare: 2;
} KEXECUTE_OPTIONS, *PKEXECUTE_OPTIONS;

typedef struct _KPROCESS2
{
    DISPATCHER_HEADER Header;
    LIST_ENTRY ProfileListHead;
#if (NTDDI_VERSION >= NTDDI_LONGHORN)
    ULONG DirectoryTableBase;
    ULONG Unused0;
#else
    LARGE_INTEGER DirectoryTableBase;
#endif
#if defined(_M_IX86)
    KGDTENTRY LdtDescriptor;
    KIDTENTRY Int21Descriptor;
    USHORT IopmOffset;
    UCHAR Iopl;
    UCHAR Unused;
#endif
    volatile ULONG ActiveProcessors;
    ULONG KernelTime;
    ULONG UserTime;
    LIST_ENTRY ReadyListHead;
    SINGLE_LIST_ENTRY SwapListEntry;
    PVOID VdmTrapcHandler;
    LIST_ENTRY ThreadListHead;
    KSPIN_LOCK ProcessLock;
    KAFFINITY Affinity;
    union
    {
        struct
        {
            LONG AutoAlignment:1;
            LONG DisableBoost:1;
            LONG DisableQuantum:1;
            LONG ReservedFlags:29;
        };
        LONG ProcessFlags;
    };
    SCHAR BasePriority;
    SCHAR QuantumReset;
    UCHAR State;
    UCHAR ThreadSeed;
    UCHAR PowerState;
    UCHAR IdealNode;
    UCHAR Visited;
    union
    {
        KEXECUTE_OPTIONS Flags;
        UCHAR ExecuteOptions;
    };
    ULONG StackCount;
    LIST_ENTRY ProcessListEntry;
} KPROCESS2, *PKPROCESS2;

typedef struct _PEB2
{
    UCHAR InheritedAddressSpace;
    UCHAR ReadImageFileExecOptions;
    UCHAR BeingDebugged;
#if (NTDDI_VERSION >= NTDDI_LONGHORN)
    struct
    {
        UCHAR ImageUsesLargePages:1;
        UCHAR IsProtectedProcess:1;
        UCHAR IsLegacyProcess:1;
        UCHAR SpareBits:5;
    };
#else
    BOOLEAN SpareBool;
#endif
    HANDLE Mutant;
    PVOID ImageBaseAddress;
    /* PPEB_LDR_DATA Ldr; */
    PVOID Ldr;
    struct _RTL_USER_PROCESS_PARAMETERS *ProcessParameters;
    PVOID SubSystemData;
    PVOID ProcessHeap;
#if (NTDDI_VERSION >= NTDDI_LONGHORN)
    struct _RTL_CRITICAL_SECTION *FastPebLock;
    PVOID AltThunkSListPtr;
    PVOID IFEOKey;
    ULONG Spare;
    union
    {
        PVOID* KernelCallbackTable;
        PVOID UserSharedInfoPtr;
    };
    ULONG SystemReserved[1];
    ULONG SpareUlong;
#else
    PVOID FastPebLock;
    PPEBLOCKROUTINE FastPebLockRoutine;
    PPEBLOCKROUTINE FastPebUnlockRoutine;
    ULONG EnvironmentUpdateCount;
    PVOID* KernelCallbackTable;
    PVOID EventLogSection;
    PVOID EventLog;
#endif
    /* PPEB_FREE_BLOCK FreeList; */
    PVOID FreeList;
    ULONG TlsExpansionCounter;
    PVOID TlsBitmap;
    ULONG TlsBitmapBits[0x2];
    PVOID ReadOnlySharedMemoryBase;
    PVOID ReadOnlySharedMemoryHeap;
    PVOID* ReadOnlyStaticServerData;
    PVOID AnsiCodePageData;
    PVOID OemCodePageData;
    PVOID UnicodeCaseTableData;
    ULONG NumberOfProcessors;
    ULONG NtGlobalFlag;
    LARGE_INTEGER CriticalSectionTimeout;
    ULONG HeapSegmentReserve;
    ULONG HeapSegmentCommit;
    ULONG HeapDeCommitTotalFreeThreshold;
    ULONG HeapDeCommitFreeBlockThreshold;
    ULONG NumberOfHeaps;
    ULONG MaximumNumberOfHeaps;
    PVOID* ProcessHeaps;
    PVOID GdiSharedHandleTable;
    PVOID ProcessStarterHelper;
    PVOID GdiDCAttributeList;
#if (NTDDI_VERSION >= NTDDI_LONGHORN)
    struct _RTL_CRITICAL_SECTION *LoaderLock;
#else
    PVOID LoaderLock;
#endif
    ULONG OSMajorVersion;
    ULONG OSMinorVersion;
    USHORT OSBuildNumber;
    USHORT OSCSDVersion;
    ULONG OSPlatformId;
    ULONG ImageSubSystem;
    ULONG ImageSubSystemMajorVersion;
    ULONG ImageSubSystemMinorVersion;
    ULONG ImageProcessAffinityMask;
    ULONG GdiHandleBuffer[0x22];
    /* PPOST_PROCESS_INIT_ROUTINE PostProcessInitRoutine; */
    PVOID PostProcessInitRoutine;
    struct _RTL_BITMAP *TlsExpansionBitmap;
    ULONG TlsExpansionBitmapBits[0x20];
    ULONG SessionId;
#if (NTDDI_VERSION >= NTDDI_WINXP)
    ULARGE_INTEGER AppCompatFlags;
    ULARGE_INTEGER AppCompatFlagsUser;
    PVOID pShimData;
    PVOID AppCompatInfo;
    UNICODE_STRING CSDVersion;
    struct _ACTIVATION_CONTEXT_DATA *ActivationContextData;
    struct _ASSEMBLY_STORAGE_MAP *ProcessAssemblyStorageMap;
    struct _ACTIVATION_CONTEXT_DATA *SystemDefaultActivationContextData;
    struct _ASSEMBLY_STORAGE_MAP *SystemAssemblyStorageMap;
    ULONG MinimumStackCommit;
#endif
#if (NTDDI_VERSION >= NTDDI_WS03)
    PVOID *FlsCallback;
    LIST_ENTRY FlsListHead;
    struct _RTL_BITMAP *FlsBitmap;
    ULONG FlsBitmapBits[4];
    ULONG FlsHighIndex;
#endif
#if (NTDDI_VERSION >= NTDDI_LONGHORN)
    PVOID WerRegistrationData;
    PVOID WerShipAssertPtr;
#endif
} PEB2, *PPEB2;

typedef struct _ALPC_PROCESS_CONTEXT
{
    EX_PUSH_LOCK Lock;
    LIST_ENTRY ViewListHead;
    ULONG PagedPoolQuotaCache;
} ALPC_PROCESS_CONTEXT, *PALPC_PROCESS_CONTEXT;

typedef struct _EPROCESS2
{
    KPROCESS2 Pcb;
    EX_PUSH_LOCK ProcessLock;
    LARGE_INTEGER CreateTime;
    LARGE_INTEGER ExitTime;
    EX_RUNDOWN_REF RundownProtect;
    PVOID UniqueProcessId;
    LIST_ENTRY ActiveProcessLinks;
    ULONG QuotaUsage[3];
    ULONG QuotaPeak[3];
    ULONG CommitCharge;
    ULONG PeakVirtualSize;
    ULONG VirtualSize;
    LIST_ENTRY SessionProcessLinks;
    PVOID DebugPort;
    union
    {
        PVOID ExceptionPortData;
        ULONG ExceptionPortValue;
        ULONG ExceptionPortState: 3;
    };
    PHANDLE_TABLE ObjectTable;
    EX_FAST_REF Token;
    ULONG WorkingSetPage;
    EX_PUSH_LOCK AddressCreationLock;
    PETHREAD RotateInProgress;
    PETHREAD ForkInProgress;
    ULONG HardwareTrigger;
    PMM_AVL_TABLE PhysicalVadRoot;
    PVOID CloneRoot;
    ULONG NumberOfPrivatePages;
    ULONG NumberOfLockedPages;
    PVOID Win32Process;
    PVOID Job;
    PVOID SectionObject;
    PVOID SectionBaseAddress;
    PVOID QuotaBlock;
    PVOID WorkingSetWatch;
    PVOID Win32WindowStation;
    PVOID InheritedFromUniqueProcessId;
    PVOID LdtInformation;
    PVOID VadFreeHint;
    PVOID VdmObjects;
    PVOID DeviceMap;
    PVOID EtwDataSource;
    PVOID FreeTebHint;
    union
    {
        HARDWARE_PTE PageDirectoryPte;
        UINT64 Filler;
    };
    PVOID Session;
    UCHAR ImageFileName[16];
    LIST_ENTRY JobLinks;
    PVOID LockedPagesList;
    LIST_ENTRY ThreadListHead;
    PVOID SecurityPort;
    PVOID PaeTop;
    ULONG ActiveThreads;
    ULONG ImagePathHash;
    ULONG DefaultHardErrorProcessing;
    LONG LastThreadExitStatus;
    PPEB Peb;
    EX_FAST_REF PrefetchTrace;
    LARGE_INTEGER ReadOperationCount;
    LARGE_INTEGER WriteOperationCount;
    LARGE_INTEGER OtherOperationCount;
    LARGE_INTEGER ReadTransferCount;
    LARGE_INTEGER WriteTransferCount;
    LARGE_INTEGER OtherTransferCount;
    ULONG CommitChargeLimit;
    ULONG CommitChargePeak;
    PVOID AweInfo;
    SE_AUDIT_PROCESS_CREATION_INFO SeAuditProcessCreationInfo;
    MMSUPPORT Vm;
    LIST_ENTRY MmProcessLinks;
    ULONG ModifiedPageCount;
    ULONG Flags2;
    ULONG FILLER_HACK_HACK_HACK_HACK;
    ULONG JobNotReallyActive: 1;
    ULONG AccountingFolded: 1;
    ULONG NewProcessReported: 1;
    ULONG ExitProcessReported: 1;
    ULONG ReportCommitChanges: 1;
    ULONG LastReportMemory: 1;
    ULONG ReportPhysicalPageChanges: 1;
    ULONG HandleTableRundown: 1;
    ULONG NeedsHandleRundown: 1;
    ULONG RefTraceEnabled: 1;
    ULONG NumaAware: 1;
    ULONG ProtectedProcess: 1;
    ULONG DefaultPagePriority: 3;
    ULONG PrimaryTokenFrozen: 1;
    ULONG ProcessVerifierTarget: 1;
    ULONG StackRandomizationDisabled: 1;
    ULONG Flags;
    ULONG CreateReported: 1;
    ULONG NoDebugInherit: 1;
    ULONG ProcessExiting: 1;
    ULONG ProcessDelete: 1;
    ULONG Wow64SplitPages: 1;
    ULONG VmDeleted: 1;
    ULONG OutswapEnabled: 1;
    ULONG Outswapped: 1;
    ULONG ForkFailed: 1;
    ULONG Wow64VaSpace4Gb: 1;
    ULONG AddressSpaceInitialized: 2;
    ULONG SetTimerResolution: 1;
    ULONG BreakOnTermination: 1;
    ULONG DeprioritizeViews: 1;
    ULONG WriteWatch: 1;
    ULONG ProcessInSession: 1;
    ULONG OverrideAddressSpace: 1;
    ULONG HasAddressSpace: 1;
    ULONG LaunchPrefetched: 1;
    ULONG InjectInpageErrors: 1;
    ULONG VmTopDown: 1;
    ULONG ImageNotifyDone: 1;
    ULONG PdeUpdateNeeded: 1;
    ULONG VdmAllowed: 1;
    ULONG SmapAllowed: 1;
    ULONG ProcessInserted: 1;
    ULONG DefaultIoPriority: 3;
    ULONG SparePsFlags1: 2;
    LONG ExitStatus;
    SHORT Spare7;
    union
    {
        struct
        {
            UCHAR SubSystemMinorVersion;
            UCHAR SubSystemMajorVersion;
        };
        SHORT SubSystemVersion;
    };
    UCHAR PriorityClass;
    MM_AVL_TABLE VadRoot;
    ULONG Cookie;
    ALPC_PROCESS_CONTEXT AlpcContext;
} EPROCESS2, *PEPROCESS2;

typedef struct _EXCEPTION_REGISTRATION_RECORD
{
    struct _EXCEPTION_REGISTRATION_RECORD *Next;
    EXCEPTION_DISPOSITION *Handler;
} EXCEPTION_REGISTRATION_RECORD, *PEXCEPTION_REGISTRATION_RECORD;

typedef struct _KTRAP_FRAME
{
    ULONG DbgEbp;
    ULONG DbgEip;
    ULONG DbgArgMark;
    ULONG DbgArgPointer;
    USHORT TempSegCs;
    UCHAR Logging;
    UCHAR Reserved;
    ULONG TempEsp;
    ULONG Dr0;
    ULONG Dr1;
    ULONG Dr2;
    ULONG Dr3;
    ULONG Dr6;
    ULONG Dr7;
    ULONG SegGs;
    ULONG SegEs;
    ULONG SegDs;
    ULONG Edx;
    ULONG Ecx;
    ULONG Eax;
    ULONG PreviousPreviousMode;
    PEXCEPTION_REGISTRATION_RECORD ExceptionList;
    ULONG SegFs;
    ULONG Edi;
    ULONG Esi;
    ULONG Ebx;
    ULONG Ebp;
    ULONG ErrCode;
    ULONG Eip;
    ULONG SegCs;
    ULONG EFlags;
    ULONG HardwareEsp;
    ULONG HardwareSegSs;
    ULONG V86Es;
    ULONG V86Ds;
    ULONG V86Fs;
    ULONG V86Gs;
} KTRAP_FRAME, *PKTRAP_FRAME;

typedef struct _KTHREAD2
{
    DISPATCHER_HEADER Header;
    UINT64 CycleTime;
    ULONG HighCycleTime;
    UINT64 QuantumTarget;
    PVOID InitialStack;
    PVOID StackLimit;
    PVOID KernelStack;
    ULONG ThreadLock;
    union
    {
        KAPC_STATE ApcState;
        UCHAR ApcStateFill[23];
    };
    CHAR Priority;
    USHORT NextProcessor;
    USHORT DeferredProcessor;
    ULONG ApcQueueLock;
    ULONG ContextSwitches;
    UCHAR State;
    UCHAR NpxState;
    UCHAR WaitIrql;
    CHAR WaitMode;
    LONG WaitStatus;
    union
    {
        PKWAIT_BLOCK WaitBlockList;
        PKGATE GateObject;
    };
    union
    {
      ULONG KernelStackResident: 1;
      ULONG ReadyTransition: 1;
      ULONG ProcessReadyQueue: 1;
      ULONG WaitNext: 1;
      ULONG SystemAffinityActive: 1;
      ULONG Alertable: 1;
      ULONG GdiFlushActive: 1;
      ULONG Reserved: 25;
      LONG MiscFlags;
    };
    UCHAR WaitReason;
    UCHAR SwapBusy;
    UCHAR Alerted[2];
    union
    {
        LIST_ENTRY WaitListEntry;
        SINGLE_LIST_ENTRY SwapListEntry;
    };
    PKQUEUE Queue;
    ULONG WaitTime;
    union
    {
        struct
        {
            SHORT KernelApcDisable;
            SHORT SpecialApcDisable;
        };
        ULONG CombinedApcDisable;
    };
    PVOID Teb;
    union
    {
        KTIMER Timer;
        UCHAR TimerFill[40];
    };
    union
    {
        ULONG AutoAlignment: 1;
        ULONG DisableBoost: 1;
        ULONG EtwStackTraceApc1Inserted: 1;
        ULONG EtwStackTraceApc2Inserted: 1;
        ULONG CycleChargePending: 1;
        ULONG CalloutActive: 1;
        ULONG ApcQueueable: 1;
        ULONG EnableStackSwap: 1;
        ULONG GuiThread: 1;
        ULONG ReservedFlags: 23;
        LONG ThreadFlags;
    };
    union
    {
        KWAIT_BLOCK WaitBlock[4];
        struct
        {
            UCHAR WaitBlockFill0[23];
            UCHAR IdealProcessor;
        };
        struct
        {
            UCHAR WaitBlockFill1[47];
            CHAR PreviousMode;
        };
        struct
        {
            UCHAR WaitBlockFill2[71];
            UCHAR ResourceIndex;
        };
        UCHAR WaitBlockFill3[95];
    };
    UCHAR LargeStack;
    LIST_ENTRY QueueListEntry;
    PKTRAP_FRAME TrapFrame;
    PVOID FirstArgument;
    union
    {
        PVOID CallbackStack;
        ULONG CallbackDepth;
    };
    PVOID ServiceTable;
    UCHAR ApcStateIndex;
    CHAR BasePriority;
    CHAR PriorityDecrement;
    UCHAR Preempted;
    UCHAR AdjustReason;
    CHAR AdjustIncrement;
    UCHAR Spare01;
    CHAR Saturation;
    ULONG SystemCallNumber;
    ULONG Spare02;
    ULONG UserAffinity;
    PKPROCESS Process;
    ULONG Affinity;
    PKAPC_STATE ApcStatePointer[2];
    union
    {
        KAPC_STATE SavedApcState;
        UCHAR SavedApcStateFill[23];
    };
    CHAR FreezeCount;
    CHAR SuspendCount;
    UCHAR UserIdealProcessor;
    UCHAR Spare03;
    UCHAR Iopl;
    PVOID Win32Thread;
    PVOID StackBase;
    union
    {
        KAPC SuspendApc;
        struct
        {
            UCHAR SuspendApcFill0[1];
            CHAR Spare04;
        };
        struct
        {
            UCHAR SuspendApcFill1[3];
            UCHAR QuantumReset;
        };
        struct
        {
            UCHAR SuspendApcFill2[4];
            ULONG KernelTime;
        };
        struct
        {
            UCHAR SuspendApcFill3[36];
            /* PKPRCB WaitPrcb; */
            PVOID WaitPrcb;
        };
        struct
        {
            UCHAR SuspendApcFill4[40];
            PVOID LegoData;
        };
        UCHAR SuspendApcFill5[47];
    };
    UCHAR PowerState;
    ULONG UserTime;
    union
    {
        KSEMAPHORE SuspendSemaphore;
        UCHAR SuspendSemaphorefill[20];
    };
    ULONG SListFaultCount;
    LIST_ENTRY ThreadListEntry;
    LIST_ENTRY MutantListHead;
    PVOID SListFaultAddress;
    PVOID MdlForLockedTeb;
} KTHREAD2, *PKTHREAD2;

typedef struct _TERMINATION_PORT
{
    struct _TERMINATION_PORT *Next;
    PVOID Port;
} TERMINATION_PORT, *PTERMINATION_PORT;

typedef struct _PS_CLIENT_SECURITY_CONTEXT
{
    union
    {
        ULONG ImpersonationData;
        PVOID ImpersonationToken;
        ULONG ImpersonationLevel: 2;
        ULONG EffectiveOnly: 1;
    };
} PS_CLIENT_SECURITY_CONTEXT, *PPS_CLIENT_SECURITY_CONTEXT;

typedef struct _ETHREAD2
{
    KTHREAD2 Tcb;
    LARGE_INTEGER CreateTime;
    union
    {
        LARGE_INTEGER ExitTime;
        LIST_ENTRY KeyedWaitChain;
    };
    union
    {
        LONG ExitStatus;
        PVOID OfsChain;
    };
    union
    {
        LIST_ENTRY PostBlockList;
        struct
        {
            PVOID ForwardLinkShadow;
            PVOID StartAddress;
        };
    };
    union
    {
        PTERMINATION_PORT TerminationPort;
        PETHREAD ReaperLink;
        PVOID KeyedWaitValue;
        PVOID Win32StartParameter;
    };
    ULONG ActiveTimerListLock;
    LIST_ENTRY ActiveTimerListHead;
    CLIENT_ID Cid;
    union
    {
        KSEMAPHORE KeyedWaitSemaphore;
        KSEMAPHORE AlpcWaitSemaphore;
    };
    PS_CLIENT_SECURITY_CONTEXT ClientSecurity;
    LIST_ENTRY IrpList;
    ULONG TopLevelIrp;
    PDEVICE_OBJECT DeviceToVerify;
    /* _PSP_RATE_APC *RateControlApc; */
    PVOID RateControlApc;
    PVOID Win32StartAddress;
    PVOID SparePtr0;
    LIST_ENTRY ThreadListEntry;
    EX_RUNDOWN_REF RundownProtect;
    EX_PUSH_LOCK ThreadLock;
    ULONG ReadClusterSize;
    LONG MmLockOrdering;
    ULONG CrossThreadFlags;
    ULONG Terminated: 1;
    ULONG ThreadInserted: 1;
    ULONG HideFromDebugger: 1;
    ULONG ActiveImpersonationInfo: 1;
    ULONG SystemThread: 1;
    ULONG HardErrorsAreDisabled: 1;
    ULONG BreakOnTermination: 1;
    ULONG SkipCreationMsg: 1;
    ULONG SkipTerminationMsg: 1;
    ULONG CopyTokenOnOpen: 1;
    ULONG ThreadIoPriority: 3;
    ULONG ThreadPagePriority: 3;
    ULONG RundownFail: 1;
    ULONG SameThreadPassiveFlags;
    ULONG ActiveExWorker: 1;
    ULONG ExWorkerCanWaitUser: 1;
    ULONG MemoryMaker: 1;
    ULONG ClonedThread: 1;
    ULONG KeyedEventInUse: 1;
    ULONG RateApcState: 2;
    ULONG SelfTerminate: 1;
    ULONG SameThreadApcFlags;
    ULONG Spare: 1;
    ULONG StartAddressInvalid: 1;
    ULONG EtwPageFaultCalloutActive: 1;
    ULONG OwnsProcessWorkingSetExclusive: 1;
    ULONG OwnsProcessWorkingSetShared: 1;
    ULONG OwnsSystemWorkingSetExclusive: 1;
    ULONG OwnsSystemWorkingSetShared: 1;
    ULONG OwnsSessionWorkingSetExclusive: 1;
    ULONG OwnsSessionWorkingSetShared: 1;
    ULONG OwnsProcessAddressSpaceExclusive: 1;
    ULONG OwnsProcessAddressSpaceShared: 1;
    ULONG SuppressSymbolLoad: 1;
    ULONG Prefetching: 1;
    ULONG OwnsDynamicMemoryShared: 1;
    ULONG OwnsChangeControlAreaExclusive: 1;
    ULONG OwnsChangeControlAreaShared: 1;
    ULONG PriorityRegionActive: 4;
    UCHAR CacheManagerActive;
    UCHAR DisablePageFaultClustering;
    UCHAR ActiveFaultCount;
    ULONG AlpcMessageId;
    union
    {
        PVOID AlpcMessage;
        ULONG AlpcReceiveAttributeSet;
    };
    LIST_ENTRY AlpcWaitListEntry;
    ULONG CacheManagerCount;
} ETHREAD2, *PETHREAD2;

#endif