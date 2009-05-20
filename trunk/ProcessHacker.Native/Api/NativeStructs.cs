/*
 * Process Hacker - 
 *   native API structs
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
using ProcessHacker.Native.Objects;

namespace ProcessHacker.Native.Api
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ClientId
    {
        public ClientId(int processId, int threadId)
        {
            this.UniqueProcess = new IntPtr(processId);
            this.UniqueThread = new IntPtr(threadId);
        }

        public IntPtr UniqueProcess;
        public IntPtr UniqueThread;

        public int ProcessId { get { return this.UniqueProcess.ToInt32(); } }
        public int ThreadId { get { return this.UniqueThread.ToInt32(); } }
    }

    // NOTE: This x86 CONTEXT ONLY
    [StructLayout(LayoutKind.Sequential)]
    public struct Context
    {
        public ContextFlags ContextFlags;

        public int Dr0;
        public int Dr1;
        public int Dr2;
        public int Dr3;
        public int Dr6;
        public int Dr7;

        [MarshalAs(UnmanagedType.Struct)]
        public FloatingSaveArea FloatSave;

        public int SegGs;
        public int SegFs;
        public int SegEs;
        public int SegDs;

        public int Edi;
        public int Esi;
        public int Ebx;
        public int Edx;
        public int Ecx;
        public int Eax;

        public int Ebp;
        public int Eip;
        public int SegCs;
        public int EFlags;
        public int Esp;
        public int SegSs;

        public unsafe fixed byte ExtendedRegisters[Win32.MaximumSupportedExtension];
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct EventBasicInformation
    {
        public EventType EventType;
        public int EventState;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FloatingSaveArea
    {
        public int ControlWord;
        public int StatusWord;
        public int TagWord;
        public int ErrorOffset;
        public int ErrorSelector;
        public int DataOffset;
        public int DataSelector;

        public unsafe fixed byte RegisterArea[Win32.SizeOf80387Registers];

        public int Cr0NpxState;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct GenericMapping
    {
        public int GenericRead;
        public int GenericWrite;
        public int GenericExecute;
        public int GenericAll;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct InitialTeb
    {
        public struct OldInitialTebStruct
        {
            public IntPtr OldStackBase;
            public IntPtr OldStackLimit;
        }

        public OldInitialTebStruct OldInitialTeb;
        public IntPtr StackBase;
        public IntPtr StackLimit;
        public IntPtr StackAllocationBase;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct IoCounters
    {
        public ulong ReadOperationCount;
        public ulong WriteOperationCount;
        public ulong OtherOperationCount;
        public ulong ReadTransferCount;
        public ulong WriteTransferCount;
        public ulong OtherTransferCount;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct IoStatusBlock
    {
        [FieldOffset(0)]
        public NtStatus status;
        [FieldOffset(0)]
        public IntPtr Pointer;

        // HACK, offset is 8 on x64
        [FieldOffset(4)]
        public IntPtr Information;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct JobObjectBasicAccountingInformation
    {
        public long TotalUserTime;
        public long TotalKernelTime;
        public long ThisPeriodTotalUserTime;
        public long ThisPeriodTotalKernelTime;
        public int TotalPageFaultCount;
        public int TotalProcesses;
        public int ActiveProcesses;
        public int TotalTerminatedProcesses;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct JobObjectBasicAndIoAccountingInformation
    {
        public JobObjectBasicAccountingInformation BasicInfo;
        public IoCounters IoInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct JobObjectBasicLimitInformation
    {
        public long PerProcessUserTimeLimit;
        public long PerJobUserTimeLimit;
        public JobObjectLimitFlags LimitFlags;
        public int MinimumWorkingSetSize;
        public int MaximumWorkingSetSize;
        public int ActiveProcessLimit;
        public int Affinity;
        public int PriorityClass;
        public int SchedulingClass;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct JobObjectBasicProcessIdList
    {
        public int NumberOfAssignedProcesses;
        public int NumberOfProcessIdsInList;
        /* an array follows */
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct JobObjectEndOfJobTimeInformation
    {
        public int EndOfJobTimeAction; // 0: Terminate, 1: Post
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct JobObjectExtendedLimitInformation
    {
        public JobObjectBasicLimitInformation BasicLimitInformation;
        public IoCounters IoInfo;
        public int ProcessMemoryLimit;
        public int JobMemoryLimit;
        public int PeakProcessMemoryUsed;
        public int PeakJobMemoryUsed;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct JobSetArray
    {
        public IntPtr JobHandle;
        public uint MemberLevel;
        public int Flags; // Unused
    }

    [StructLayout(LayoutKind.Explicit, Size = 8)]
    public struct LargeInteger
    {
        [FieldOffset(0)]
        public Int64 QuadPart;
        [FieldOffset(0)]
        public UInt32 LowPart;
        [FieldOffset(4)]
        public UInt32 HighPart;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct LdrModule
    {
        public ListEntry InLoadOrderModuleList;
        public ListEntry InMemoryOrderModuleList;
        public ListEntry InInitializationOrderModuleList;
        public IntPtr BaseAddress;
        public IntPtr EntryPoint;
        public int SizeOfImage;
        public UnicodeString FullDllName;
        public UnicodeString BaseDllName;
        public int Flags;
        public short LoadCount;
        public short TlsIndex;
        public ListEntry HashTableEntry;
        public int TimeDateStamp;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ListEntry
    {
        public IntPtr Flink;
        public IntPtr Blink;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Luid
    {
        public int LowPart;
        public int HighPart;

        public Luid Allocate()
        {
            NtStatus status;
            Luid luid;

            if ((status = Win32.NtAllocateLocallyUniqueId(out luid)) >= NtStatus.Error)
                Win32.ThrowLastError(status);

            return luid;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MutantBasicInformation
    {
        public int CurrentCount;
        public byte OwnedByCaller;
        public byte AbandonedState;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ObjectAttributes : IDisposable
    {
        public ObjectAttributes(
            string objectName,
            ObjectFlags attributes,
            DirectoryHandle rootDirectory
            )
        {
            this.Length = Marshal.SizeOf(typeof(ObjectAttributes));
            this.RootDirectory = IntPtr.Zero;
            this.ObjectName = IntPtr.Zero;
            this.SecurityDescriptor = IntPtr.Zero;
            this.SecurityQualityOfService = IntPtr.Zero;

            if (objectName != null)
            {
                UnicodeString unicodeString = new UnicodeString(objectName);
                IntPtr unicodeStringMemory = Marshal.AllocHGlobal(Marshal.SizeOf(unicodeString));

                Marshal.StructureToPtr(unicodeString, unicodeStringMemory, false);
                this.ObjectName = unicodeStringMemory;
            }

            this.Attributes = attributes;

            if (rootDirectory != null)
                this.RootDirectory = rootDirectory;
        }

        public int Length;
        public IntPtr RootDirectory;
        public IntPtr ObjectName;
        public ObjectFlags Attributes;
        public IntPtr SecurityDescriptor;
        public IntPtr SecurityQualityOfService;

        public void Dispose()
        {
            if (this.ObjectName == IntPtr.Zero)
                return;

            UnicodeString unicodeString =
                (UnicodeString)Marshal.PtrToStructure(this.ObjectName, typeof(UnicodeString));

            unicodeString.Dispose();
            Marshal.FreeHGlobal(this.ObjectName);

            this.ObjectName = IntPtr.Zero;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ObjectBasicInformation
    {
        public uint Attributes;
        public int GrantedAccess;
        public uint HandleCount;
        public uint PointerCount;
        public uint PagedPoolUsage;
        public uint NonPagedPoolUsage;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public uint[] Reserved;

        public uint NameInformationLength;
        public uint TypeInformationLength;
        public uint SecurityDescriptorLength;
        public ulong CreateTime;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ObjectDirectoryInformation
    {
        public UnicodeString Name;
        public UnicodeString TypeName;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ObjectNameInformation
    {
        public UnicodeString Name;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ObjectTypeInformation
    {
        public UnicodeString Name;
        public int TotalNumberOfObjects;
        public int TotalNumberOfHandles;
        public int TotalPagedPoolUsage;
        public int TotalNonPagedPoolUsage;
        public int TotalNamePoolUsage;
        public int TotalHandleTableUsage;
        public int HighWaterNumberOfObjects;
        public int HighWaterNumberOfHandles;
        public int HighWaterPagedPoolUsage;
        public int HighWaterNonPagedPoolUsage;
        public int HighWaterNamePoolUsage;
        public int HighWaterHandleTableUsage;
        public int InvalidAttributes;
        public GenericMapping GenericMapping;
        public int ValidAccess;
        public byte SecurityRequired;
        public byte MaintainHandleCount;
        public ushort MaintainTypeList;
        public PoolType PoolType;
        public int PagedPoolUsage;
        public int NonPagedPoolUsage;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PebLdrData
    {
        public int Length;
        public char Initialized;
        public int SsHandle;
        public ListEntry InLoadOrderModuleList;
        public ListEntry InMemoryOrderModuleList;
        public ListEntry InInitializationOrderModuleList;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PooledUsageAndLimits
    {
        public int PeakPagedPoolUsage;
        public int PagedPoolUsage;
        public int PagedPoolLimit;
        public int PeakNonPagedPoolUsage;
        public int NonPagedPoolUsage;
        public int NonPagedPoolLimit;
        public int PeakPagefileUsage;
        public int PagefileUsage;
        public int PagefileLimit;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct PortMessage
    {
        [FieldOffset(0)]
        public short DataLength;
        [FieldOffset(2)]
        public short TotalLength;
        [FieldOffset(0)]
        public int Length;

        [FieldOffset(4)]
        public short Type;
        [FieldOffset(6)]
        public short DataInfoOffset;
        [FieldOffset(4)]
        public int ZeroInit;

        [FieldOffset(8)]
        public ClientId ClientId;
        [FieldOffset(8)]
        public double DoNotUseThisField;

        [FieldOffset(16)]
        public int MessageId;

        [FieldOffset(20)]
        public IntPtr ClientViewSize;
        [FieldOffset(20)]
        public int CallbackId;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PortView
    {
        public int Length;
        public IntPtr SectionHandle;
        public int SectionOffset;
        public IntPtr ViewSize;
        public IntPtr ViewBase;
        public IntPtr ViewRemoteBase;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ProcessBasicInformation
    {
        public int ExitStatus;
        public IntPtr PebBaseAddress;
        public int AffinityMask;
        public int BasePriority;
        public int UniqueProcessId;
        public int InheritedFromUniqueProcessId;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct QuotaLimits
    {
        public int PagedPoolLimit;
        public int NonPagedPoolLimit;
        public int MinimumWorkingSetSize;
        public int MaximumWorkingSetSizse;
        public int PagefileLimit;
        public long TimeLimit;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RemotePortView
    {
        public int Length;
        public IntPtr ViewSize;
        public IntPtr ViewBase;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RtlUserProcessInformation
    {
        public int Length;
        public IntPtr Process;
        public IntPtr Thread;
        public ClientId ClientId;
        public SectionImageInformation ImageInformation;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RtlUserProcessParameters
    {
        public struct CurDir
        {
            public UnicodeString DosPath;
            public IntPtr Handle;
        }

        public struct RtlDriveLetterCurDir
        {
            public ushort Flags;
            public ushort Length;
            public uint TimeStamp;
            public IntPtr DosPath;
        }

        public int MaximumLength;
        public int Length;

        public RtlUserProcessFlags Flags;
        public int DebugFlags;

        public IntPtr ConsoleHandle;
        public int ConsoleFlags;
        public IntPtr StandardInput;
        public IntPtr StandardOutput;
        public IntPtr StandardError;

        public CurDir CurrentDirectory;
        public UnicodeString DllPath;
        public UnicodeString ImagePathName;
        public UnicodeString CommandLine;
        public IntPtr Environment;

        public int StartingX;
        public int StartingY;
        public int CountX;
        public int CountY;
        public int CountCharsX;
        public int CountCharsY;
        public int FillAttribute;

        public int WindowFlags;
        public int ShowWindowFlags;
        public UnicodeString WindowTitle;
        public UnicodeString DesktopInfo;
        public UnicodeString ShellInfo;
        public UnicodeString RuntimeData;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public RtlDriveLetterCurDir[] CurrentDirectories;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SectionBasicInformation
    {
        public int Unknown;
        public SectionAttributes SectionAttributes;
        public long SectionSize;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SectionImageInformation
    {
        public IntPtr TransferAddress;
        public int StackZeroBits;
        public IntPtr StackReserved;
        public IntPtr StackCommit;
        public int ImageSubsystem;
        public short SubSystemVersionLow;
        public short SubSystemVersionHigh;
        public int GpValue;
        public short ImageCharacteristics;
        public short DllCharacteristics;
        public int ImageMachineType;
        [MarshalAs(UnmanagedType.I1)]
        public bool ImageContainsCode;
        [MarshalAs(UnmanagedType.I1)]
        public bool Spare1;
        public int LoaderFlags;
        public int ImageFileSize;
        public int Reserved;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SecurityQualityOfService
    {
        public SecurityQualityOfService(
            SecurityImpersonationLevel impersonationLevel,
            bool dynamicTracking,
            bool effectiveOnly
            )
        {
            this.Length = Marshal.SizeOf(typeof(SecurityQualityOfService));
            this.ImpersonationLevel = impersonationLevel;
            this.ContextTrackingMode = dynamicTracking;
            this.EffectiveOnly = effectiveOnly;
        }

        public int Length;
        public SecurityImpersonationLevel ImpersonationLevel;
        [MarshalAs(UnmanagedType.I1)]
        public bool ContextTrackingMode; // True for dynamic tracking, false for static tracking
        [MarshalAs(UnmanagedType.I1)]
        public bool EffectiveOnly;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SemaphoreBasicInformation
    {
        public int CurrentCount;
        public int MaximumCount;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Sid
    {
        public byte Revision;
        public byte SubAuthorityCount;
        public SidIdentifierAuthority IdentifierAuthority;

        // Array of ULONG follows
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SidAndAttributes
    {
        public IntPtr Sid; // ptr to a SID object
        public SidAttributes Attributes;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SidIdentifierAuthority
    {
        public unsafe fixed byte Value[6];
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SystemBasicInformation
    {
        public int Reserved;
        public int TimerResolution;
        public int PageSize;
        public int NumberOfPhysicalPages;
        public int LowestPhysicalPageNumber;
        public int HighestPhysicalPageNumber;
        public int AllocationGranularity;
        public int MinimumUserModeAddress;
        public int MaximumUserModeAddress;
        public int ActiveProcessorsAffinityMask;
        public byte NumberOfProcessors;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SystemCacheInformation
    {
        /// <summary>
        /// The size of the system working set, in bytes.
        /// </summary>
        public int SystemCacheWsSize;
        public int SystemCacheWsPeakSize;
        public int SystemCacheWsFaults;

        /// <summary>
        /// Measured in pages.
        /// </summary>
        public int SystemCacheWsMinimum;

        /// <summary>
        /// Measured in pages.
        /// </summary>
        public int SystemCacheWsMaximum;
        public int TransitionSharedPages;
        public int TransitionSharedPagesPeak;
        public int Reserved1;
        public int Reserved2;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SystemExtendedThreadInformation
    {
        public SystemThreadInformation ThreadInfo;
        public int StackBase; // 16
        public int StackLimit;
        public int Win32StartAddress;
        public int TebAddress; // Vista+
        public int Unused1;
        public int Unused2;
        public int Unused3;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SystemLoadAndCallImage
    {
        public UnicodeString ModuleName;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SystemObjectTypeInformation
    {
        public int NextEntryOffset;
        public UnicodeString Name;
        public int ObjectCount;
        public int HandleCount;
        public int TypeNumber;
        public int InvalidAttributes;
        public GenericMapping GenericMapping;
        public int ValidAccessMask;
        public PoolType PoolType;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SystemPerformanceInformation
    {
        /// <summary>
        /// The total idle time of all processors in units of 100-nanoseconds.
        /// </summary>
        public long IdleTime;

        /// <summary>
        /// Total bytes read by calls to NtReadFile.
        /// </summary>
        public long IoReadTransferCount;

        /// <summary>
        /// Total bytes written by calls to NtWriteFile.
        /// </summary>
        public long IoWriteTransferCount;

        /// <summary>
        /// Total bytes transferred by other I/O operations.
        /// </summary>
        public long IoOtherTransferCount;

        /// <summary>
        /// Number of calls to NtReadFile.
        /// </summary>
        public int IoReadOperationCount;

        /// <summary>
        /// Number of calls to NtWriteFile.
        /// </summary>
        public int IoWriteOperationCount;

        /// <summary>
        /// Number of calls to other I/O functions.
        /// </summary>
        public int IoOtherOperationCount;

        /// <summary>
        /// The number of pages of physical memory available.
        /// </summary>
        public int AvailablePages;

        /// <summary>
        /// The number of pages of committed virtual memory.
        /// </summary>
        public int CommittedPages;

        /// <summary>
        /// The number of pages of virtual memory that could be committed 
        /// without extending the system's pagefiles.
        /// </summary>
        public int CommitLimit;

        /// <summary>
        /// The peak number of pages of committed virtual memory.
        /// </summary>
        public int PeakCommitment;

        /// <summary>
        /// The total number of soft and hard page faults.
        /// </summary>
        public int PageFaults;

        /// <summary>
        /// The number of copy-on-write page faults.
        /// </summary>
        public int CopyOnWriteFaults;

        /// <summary>
        /// The number of soft page faults.
        /// </summary>
        public int TransitionFaults;

        /// <summary>
        /// Something that the Native API reference book doesn't have.
        /// </summary>
        public int CacheTransitionFaults;

        /// <summary>
        /// The number of demand zero faults.
        /// </summary>
        public int DemandZeroFaults;

        /// <summary>
        /// The number of pages read from disk to resolve page faults.
        /// </summary>
        public int PagesRead;

        /// <summary>
        /// The number of read operations initiated to resolve page faults.
        /// </summary>
        public int PagesReadIos;

        public int CacheRead;
        public int CacheReadIos;

        /// <summary>
        /// The number of pages written to the system's pagefiles.
        /// </summary>
        public int PagefilePagesWritten;

        /// <summary>
        /// The number of write operations performed on the system's pagefiles.
        /// </summary>
        public int PagefilePagesWriteIos;

        /// <summary>
        /// The number of pages written to mapped files.
        /// </summary>
        public int MappedFilePagesWritten;

        /// <summary>
        /// The number of write operations performed on mapped files.
        /// </summary>
        public int MappedFilePageWriteIos;

        /// <summary>
        /// The number of pages used by the paged pool.
        /// </summary>
        public int PagedPoolUsage;

        /// <summary>
        /// The number of pages used by the non-paged pool.
        /// </summary>
        public int NonPagedPoolUsage;

        /// <summary>
        /// The number of allocations made from the paged pool.
        /// </summary>
        public int PagedPoolAllocs;

        /// <summary>
        /// The number of allocations returned to the paged pool.
        /// </summary>
        public int PagedPoolFrees;

        /// <summary>
        /// The number of allocations made from the non-paged pool.
        /// </summary>
        public int NonPagedPoolAllocs;

        /// <summary>
        /// The number of allocations returned to the non-paged pool.
        /// </summary>
        public int NonPagedPoolFrees;

        /// <summary>
        /// The number of available System Page Table Entries.
        /// </summary>
        public int FreeSystemPtes;

        /// <summary>
        /// The number of pages of pageable OS code and data in physical 
        /// memory.
        /// </summary>
        public int SystemCodePages;

        /// <summary>
        /// The number of pages of pageable driver code and data.
        /// </summary>
        public int TotalSystemDriverPages;

        /// <summary>
        /// The number of pages of OS driver code and data.
        /// </summary>
        public int TotalSystemCodePages;

        /// <summary>
        /// The number of times an allocation could be statisfied by one of the 
        /// small non-paged lookaside lists.
        /// </summary>
        public int SmallNonPagedPoolLookasideListAllocateHits;

        /// <summary>
        /// The number of times an allocation could be statisfied by one of the 
        /// small paged lookaside lists.
        /// </summary>
        public int SmallPagedPoolLookasideAllocateHits;

        public int Reserved3;

        /// <summary>
        /// The number of pages of the system cache in physical memory.
        /// </summary>
        public int SystemCachePages;

        /// <summary>
        /// The number of pages of the paged pool in physical memory.
        /// </summary>
        public int PagedPoolPages;

        /// <summary>
        /// The number of pages of pageable driver code and data in physical memory.
        /// </summary>
        public int SystemDriverPages;

        /// <summary>
        /// The number of asynchronous fast read operations.
        /// </summary>
        public int FastReadNoWait;

        /// <summary>
        /// The number of synchronous fast read operations.
        /// </summary>
        public int FastReadWait;

        /// <summary>
        /// The number of fast read operations not possible because of resource 
        /// conflicts.
        /// </summary>
        public int FastReadResourceMiss;

        /// <remarks>
        /// Google Books won't let me read the page containing the description 
        /// for this field!
        /// </remarks>
        public int FastReadNotPossible;

        /// <remarks>
        /// Google Books won't let me read the page containing the description 
        /// for this field!
        /// </remarks>
        public int FastMdlReadNoWait;

        /// <remarks>
        /// Google Books won't let me read the page containing the description 
        /// for this field!
        /// </remarks>
        public int FastMdlReadWait;

        /// <remarks>
        /// Google Books won't let me read the page containing the description 
        /// for this field!
        /// </remarks>
        public int FastMdlReadResourceMiss;

        /// <remarks>
        /// Google Books won't let me read the page containing the description 
        /// for this field!
        /// </remarks>
        public int FastMdlReadNotPossible;

        /// <remarks>
        /// Google Books won't let me read the page containing the description 
        /// for this field!
        /// </remarks>
        public int MapDataNoWait;

        /// <remarks>
        /// Google Books won't let me read the page containing the description 
        /// for this field!
        /// </remarks>
        public int MapDataWait;

        /// <remarks>
        /// Google Books won't let me read the page containing the description 
        /// for this field!
        /// </remarks>
        public int MapDataNoWaitMiss;

        /// <remarks>
        /// Google Books won't let me read the page containing the description 
        /// for this field!
        /// </remarks>
        public int MapDataWaitMiss;

        /// <remarks>
        /// Google Books won't let me read the page containing the description 
        /// for this field!
        /// </remarks>
        public int PinMappedDataCount;

        /// <remarks>
        /// Google Books won't let me read the page containing the description 
        /// for this field!
        /// </remarks>
        public int PinReadNoWait;

        /// <remarks>
        /// Google Books won't let me read the page containing the description 
        /// for this field!
        /// </remarks>
        public int PinReadWait;

        /// <remarks>
        /// Google Books won't let me read the page containing the description 
        /// for this field!
        /// </remarks>
        public int PinReadNoWaitMiss;

        /// <remarks>
        /// Google Books won't let me read the page containing the description 
        /// for this field!
        /// </remarks>
        public int PinReadWaitMiss;

        /// <remarks>
        /// Google Books won't let me read the page containing the description 
        /// for this field!
        /// </remarks>
        public int CopyReadNoWait;

        /// <remarks>
        /// Google Books won't let me read the page containing the description 
        /// for this field!
        /// </remarks>
        public int CopyReadWait;

        /// <remarks>
        /// Google Books won't let me read the page containing the description 
        /// for this field!
        /// </remarks>
        public int CopyReadNoWaitMiss;

        /// <remarks>
        /// Google Books won't let me read the page containing the description 
        /// for this field!
        /// </remarks>
        public int CopyReadWaitMiss;

        /// <remarks>
        /// Google Books won't let me read the page containing the description 
        /// for this field!
        /// </remarks>
        public int MdlReadNoWait;

        /// <remarks>
        /// Google Books won't let me read the page containing the description 
        /// for this field!
        /// </remarks>
        public int MdlReadWait;

        /// <remarks>
        /// Google Books won't let me read the page containing the description 
        /// for this field!
        /// </remarks>
        public int MdlReadNoWaitMiss;

        /// <remarks>
        /// Google Books won't let me read the page containing the description 
        /// for this field!
        /// </remarks>
        public int MdlReadWaitMiss;

        /// <remarks>
        /// Google Books won't let me read the page containing the description 
        /// for this field!
        /// </remarks>
        public int ReadAheadIos;

        /// <remarks>
        /// Google Books won't let me read the page containing the description 
        /// for this field!
        /// </remarks>
        public int LazyWriteIos;

        /// <remarks>
        /// Google Books won't let me read the page containing the description 
        /// for this field!
        /// </remarks>
        public int LazyWritePages;

        /// <remarks>
        /// Google Books won't let me read the page containing the description 
        /// for this field!
        /// </remarks>
        public int DataFlushes;

        /// <remarks>
        /// Google Books won't let me read the page containing the description 
        /// for this field!
        /// </remarks>
        public int DataPages;

        /// <summary>
        /// The total number of context switches.
        /// </summary>
        public int ContextSwitches;

        /// <summary>
        /// The number of first level translation buffer fills.
        /// </summary>
        public int FirstLevelTbFills;

        /// <summary>
        /// The number of second level translation buffer fills.
        /// </summary>
        public int SecondLevelTbFills;

        /// <summary>
        /// The number of system calls executed.
        /// </summary>
        public int SystemCalls;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SystemProcessInformation
    {
        public int NextEntryOffset;
        public int NumberOfThreads;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public long[] Spare;

        public long CreateTime; // 8
        public long UserTime;
        public long KernelTime;
        public UnicodeString ImageName;
        public int BasePriority;
        public int ProcessId;
        public int InheritedFromProcessId;
        public int HandleCount;
        public int SessionId;
        public int PageDirectoryBase;
        public VmCountersEx VirtualMemoryCounters;
        public IoCounters IoCounters;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SystemProcessorPerformanceInformation
    {
        public long IdleTime;
        public long KernelTime;
        public long UserTime;
        public long DpcTime;
        public long InterruptTime;
        public int InterruptCount;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SystemSessionProcessInformation
    {
        public int SessionId;
        public int BufferLength;
        public IntPtr Buffer;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SystemThreadInformation
    {
        public long KernelTime;
        public long UserTime;
        public long CreateTime;
        public int WaitTime;
        public int StartAddress;
        public ClientId ClientId;
        public int Priority;
        public int BasePriority;
        public int ContextSwitchCount; // 12
        public int State; // 13
        public KWaitReason WaitReason; // 14
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ThreadBasicInformation
    {
        public uint ExitStatus;
        public IntPtr TebBaseAddress;
        public ClientId ClientId;
        public uint AffinityMask;
        public uint Priority;
        public uint BasePriority;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TimerBasicInformation
    {
        public LargeInteger RemainingTime;
        [MarshalAs(UnmanagedType.I1)]
        public bool TimerState;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TokenGroups
    {
        public uint GroupCount;

        [MarshalAs(UnmanagedType.ByValArray)]
        public SidAndAttributes[] Groups;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TokenPrivileges
    {
        public uint PrivilegeCount;

        [MarshalAs(UnmanagedType.ByValArray)]
        public LuidAndAttributes[] Privileges;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct TokenSource
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
        public string SourceName;

        public Luid SourceIdentifier;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TokenUser
    {
        public SidAndAttributes User;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct UnicodeString : IComparable<UnicodeString>, IEquatable<UnicodeString>, IDisposable
    {
        public UnicodeString(string str)
        {
            UnicodeString newString;

            if (!Win32.RtlCreateUnicodeString(out newString, str))
                throw new OutOfMemoryException();

            this.Length = newString.Length;
            this.MaximumLength = newString.MaximumLength;
            this.Buffer = newString.Buffer;
        }

        public ushort Length;
        public ushort MaximumLength;
        public IntPtr Buffer;

        public int CompareTo(UnicodeString unicodeString, bool caseInsensitive)
        {
            return Win32.RtlCompareUnicodeString(ref this, ref unicodeString, caseInsensitive);
        }

        public int CompareTo(UnicodeString unicodeString)
        {
            return this.CompareTo(unicodeString, false);
        }

        public void Dispose()
        {
            if (this.Buffer == IntPtr.Zero)
                return;

            Win32.RtlFreeUnicodeString(ref this);
            this.Buffer = IntPtr.Zero;
        }

        /// <summary>
        /// Copies the string to a newly allocated string.
        /// </summary>
        public UnicodeString Duplicate()
        {
            NtStatus status;
            UnicodeString newString;

            if ((status = Win32.RtlDuplicateUnicodeString(
                RtlDuplicateUnicodeStringFlags.AllocateNullString |
                RtlDuplicateUnicodeStringFlags.NullTerminate,
                ref this, out newString)) >= NtStatus.Error)
                Win32.ThrowLastError(status);

            return newString;
        }

        public bool Equals(UnicodeString unicodeString, bool caseInsensitive)
        {
            return Win32.RtlEqualUnicodeString(ref this, ref unicodeString, caseInsensitive);
        }

        public bool Equals(UnicodeString unicodeString)
        {
            return this.Equals(unicodeString, false);
        }

        public int Hash(HashStringAlgorithm algorithm, bool caseInsensitive)
        {
            NtStatus status;
            int hash;

            if ((status = Win32.RtlHashUnicodeString(ref this,
                caseInsensitive, algorithm, out hash)) >= NtStatus.Error)
                Win32.ThrowLastError(status);

            return hash;
        }

        public int Hash(HashStringAlgorithm algorithm)
        {
            return this.Hash(algorithm, false);
        }

        public int Hash()
        {
            return this.Hash(HashStringAlgorithm.Default);
        }

        public override int GetHashCode()
        {
            return this.Hash();
        }

        public string Read()
        {
            return Utils.ReadUnicodeString(this);
        }

        public string Read(ProcessHandle processHandle)
        {
            return Utils.ReadUnicodeString(processHandle, this);
        }

        public bool StartsWith(UnicodeString unicodeString, bool caseInsensitive)
        {
            return Win32.RtlPrefixUnicodeString(ref this, ref unicodeString, caseInsensitive);
        }

        public bool StartsWith(UnicodeString unicodeString)
        {
            return this.StartsWith(unicodeString, false);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct VmCounters
    {
        public int PeakVirtualSize;
        public int VirtualSize;
        public int PageFaultCount;
        public int PeakWorkingSetSize;
        public int WorkingSetSize;
        public int QuotaPeakPagedPoolUsage;
        public int QuotaPagedPoolUsage;
        public int QuotaPeakNonPagedPoolUsage;
        public int QuotaNonPagedPoolUsage;
        public int PagefileUsage;
        public int PeakPagefileUsage;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct VmCountersEx
    {
        public int PeakVirtualSize;
        public int VirtualSize;
        public int PageFaultCount;
        public int PeakWorkingSetSize;
        public int WorkingSetSize;
        public int QuotaPeakPagedPoolUsage;
        public int QuotaPagedPoolUsage;
        public int QuotaPeakNonPagedPoolUsage;
        public int QuotaNonPagedPoolUsage;
        public int PagefileUsage;
        public int PeakPagefileUsage;
        public int PrivateBytes;
    }
}
