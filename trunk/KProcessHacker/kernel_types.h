#ifndef _KERNEL_TYPES_H
#define _KERNEL_TYPES_H

#include <ntddk.h>

typedef struct _KGDTENTRY
{
    SHORT LimitLow;
    SHORT BaseLow;
    ULONG HighWord;
} KGDTENTRY, *PKGDTENTRY;

typedef struct _KIDTENTRY
{
    SHORT Offset;
    SHORT Selector;
    SHORT Access;
    SHORT ExtendedOffset;
} KIDTENTRY, *PKIDTENTRY;

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

typedef struct _HANDLE_TRACE_DB_ENTRY
{
    CLIENT_ID ClientId;
    PVOID Handle;
    ULONG Type;
    VOID *StackTrace[16];
} HANDLE_TRACE_DB_ENTRY, *PHANDLE_TRACE_DB_ENTRY;

typedef struct _HANDLE_TRACE_DEBUG_INFO
{
    LONG RefCount;
    ULONG TableSize;
    ULONG BitMaskFlags;
    FAST_MUTEX CloseCompactionLock;
    ULONG CurrentStackIndex;
    HANDLE_TRACE_DB_ENTRY TraceDb[1];
} HANDLE_TRACE_DEBUG_INFO, *PHANDLE_TRACE_DEBUG_INFO;

typedef struct _HANDLE_TABLE_ENTRY_INFO
{
    ULONG AuditMask;
} HANDLE_TABLE_ENTRY_INFO, *PHANDLE_TABLE_ENTRY_INFO;

typedef struct _HANDLE_TABLE_ENTRY
{
    union
    {
        PVOID Object;
        ULONG ObAttributes;
        PHANDLE_TABLE_ENTRY_INFO InfoTable;
        ULONG Value;
    };
    union
    {
        ULONG GrantedAccess;
        struct
        {
            SHORT GrantedAccessIndex;
            SHORT CreatorBackTraceIndex;
        };
        LONG NextFreeTableEntry;
    };
} HANDLE_TABLE_ENTRY, *PHANDLE_TABLE_ENTRY;

typedef struct _HANDLE_TABLE
{
    ULONG TableCode;
    PEPROCESS QuotaProcess;
    PVOID UniqueProcessId;
    EX_PUSH_LOCK HandleLock;
    LIST_ENTRY HandleTableList;
    EX_PUSH_LOCK HandleContentionEvent;
    PHANDLE_TRACE_DEBUG_INFO DebugInfo;
    LONG ExtraInfoPages;
    ULONG Flags;
    ULONG StrictFIFO: 1;
    LONG FirstFreeHandle;
    PHANDLE_TABLE_ENTRY LastFreeHandleEntry;
    LONG HandleCount;
    ULONG NextHandleNeedingPool;
} HANDLE_TABLE, *PHANDLE_TABLE;

/* from ReactOS */
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
    ULONGLONG CycleTime;
} KPROCESS2, *PKPROCESS2;

typedef struct _EX_FAST_REF
{
    union
    {
        PVOID Object;
        ULONG RefCnt: 3;
        ULONG Value;
    };
} EX_FAST_REF, *PEX_FAST_REF;

typedef struct _MMADDRESS_NODE
{
    ULONG u1;
    struct _MMADDRESS_NODE *LeftChild;
    struct _MMADDRESS_NODE *RightChild;
    ULONG StartingVpn;
    ULONG EndingVpn;
} MMADDRESS_NODE, *PMMADDRESS_NODE;

typedef struct _MM_AVL_TABLE
{
    MMADDRESS_NODE BalancedRoot;
    ULONG DepthOfTree: 5;
    ULONG Unused: 3;
    ULONG NumberGenericTableElements: 24;
    PVOID NodeHint;
    PVOID NodeFreeHint;
} MM_AVL_TABLE, *PMM_AVL_TABLE;

typedef struct _HARDWARE_PTE
{
    union
    {
        ULONG Valid: 1;
        ULONG Write: 1;
        ULONG Owner: 1;
        ULONG WriteThrough: 1;
        ULONG CacheDisable: 1;
        ULONG Accessed: 1;
        ULONG Dirty: 1;
        ULONG LargePage: 1;
        ULONG Global: 1;
        ULONG CopyOnWrite: 1;
        ULONG Prototype: 1;
        ULONG reserved0: 1;
        ULONG PageFrameNumber: 26;
        ULONG reserved1: 26;
        ULONG LowPart;
    };
    ULONG HighPart;
} HARDWARE_PTE, *PHARDWARE_PTE;

typedef struct _SE_AUDIT_PROCESS_CREATION_INFO
{
    POBJECT_NAME_INFORMATION ImageFileName;
} SE_AUDIT_PROCESS_CREATION_INFO, *PSE_AUDIT_PROCESS_CREATION_INFO;

typedef struct _MMSUPPORT_FLAGS
{
    ULONG SessionSpace: 1;
    ULONG ModwriterAttached: 1;
    ULONG TrimHard: 1;
    ULONG MaximumWorkingSetHard: 1;
    ULONG ForceTrim: 1;
    ULONG MinimumWorkingSetHard: 1;
    ULONG SessionMaster: 1;
    ULONG TrimmerAttached: 1;
    ULONG TrimmerDetaching: 1;
    ULONG Reserved: 7;
    ULONG MemoryPriority: 8;
    ULONG WsleDeleted: 1;
    ULONG VmExiting: 1;
    ULONG Available: 6;
} MMSUPPORT_FLAGS, *PMMSUPPORT_FLAGS;

typedef struct _MMSUPPORT
{
    LIST_ENTRY WorkingSetExpansionLinks;
    SHORT LastTrimStamp;
    SHORT NextPageColor;
    MMSUPPORT_FLAGS Flags;
    ULONG PageFaultCount;
    ULONG PeakWorkingSetSize;
    ULONG Spare0;
    ULONG MinimumWorkingSetSize;
    ULONG MaximumWorkingSetSize;
    /* PMMWSL VmWorkingSetList; */
    PVOID VmWorkingSetList;
    ULONG Claim;
    ULONG Spare[1];
    ULONG WorkingSetPrivateSize;
    ULONG WorkingSetSizeOverhead;
    ULONG WorkingSetSize;
    PKEVENT ExitEvent;
    EX_PUSH_LOCK WorkingSetMutex;
    PVOID AccessLog;
} MMSUPPORT, *PMMSUPPORT;

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

typedef struct _OBJECT_TYPE_INITIALIZER
{
	USHORT Length;
	UCHAR ObjectTypeFlags;
	ULONG CaseInsensitive: 1;
	ULONG UnnamedObjectsOnly: 1;
	ULONG UseDefaultObject: 1;
	ULONG SecurityRequired: 1;
	ULONG MaintainHandleCount: 1;
	ULONG MaintainTypeList: 1;
	ULONG ObjectTypeCode;
	ULONG InvalidAttributes;
	GENERIC_MAPPING GenericMapping;
	ULONG ValidAccessMask;
	POOL_TYPE PoolType;
    ULONG DefaultPagedPoolCharge;
    ULONG DefaultNonPagedPoolCharge;
    PVOID DumpProcedure;
    PVOID OpenProcedure;
    PVOID CloseProcedure;
    PVOID DeleteProcedure;
    PVOID ParseProcedure;
    PVOID SecurityProcedure;
    PVOID QueryNameProcedure;
    PVOID OkayToCloseProcedure;
} OBJECT_TYPE_INITIALIZER, *POBJECT_TYPE_INITIALIZER;

typedef struct _OBJECT_TYPE
{
    ERESOURCE Mutex;
    LIST_ENTRY TypeList;
    UNICODE_STRING Name;
    PVOID DefaultObject;
    ULONG Index;
    ULONG TotalNumberOfObjects;
    ULONG TotalNumberOfHandles;
    ULONG HighWaterNumberOfObjects;
    ULONG HighWaterNumberOfHandles;
    OBJECT_TYPE_INITIALIZER TypeInfo;
    ULONG Key;
    EX_PUSH_LOCK ObjectLocks[32];
} OBJECT_TYPE;

typedef struct _AUX_ACCESS_DATA
{
    /* PPRIVILEGE_SET PrivilegeSet; */
    PVOID PrivilegeSet;
    GENERIC_MAPPING GenericMapping;
    ULONG Reserved;
} AUX_ACCESS_DATA, *PAUX_ACCESS_DATA;

#endif
