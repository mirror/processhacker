/*
 * Process Hacker - 
 *   windows API structs
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
using ProcessHacker.Native.Objects;

namespace ProcessHacker.Native.Api
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Address64
    {
        public ulong Offset;
        public ushort Segment;
        public AddressMode Mode;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct CatalogInfo
    {
        public int Size;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string CatalogFile;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ClientId
    {
        public int UniqueProcess;
        public int UniqueThread;
    }

    // NOTE: This x86 CONTEXT ONLY!!!
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
    public struct EnumServiceStatus
    {
        [MarshalAs(UnmanagedType.LPTStr)]
        public string ServiceName;

        [MarshalAs(UnmanagedType.LPTStr)]
        public string DisplayName;

        [MarshalAs(UnmanagedType.Struct)]
        public ServiceStatus ServiceStatus;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct EnumServiceStatusProcess
    {
        [MarshalAs(UnmanagedType.LPTStr)]
        public string ServiceName;

        [MarshalAs(UnmanagedType.LPTStr)]
        public string DisplayName;

        [MarshalAs(UnmanagedType.Struct)]
        public ServiceStatusProcess ServiceStatusProcess;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct EventBasicInformation
    {
        public EventType EventType;
        public int EventState;
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
    public struct FileTime
    {
        public uint LowDateTime;
        public uint HighDateTime;

        public static implicit operator long(FileTime fileTime)
        {
            LargeInteger integer = new LargeInteger();
            integer.LowPart = fileTime.LowDateTime;
            integer.HighPart = fileTime.HighDateTime;
            return integer.QuadPart;
        }
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
    public struct FpoData
    {
        public int ulOffStart;
        public int cbProcSize;
        public int cdwLocals;
        public short cdwParams;

        public long Part1;
        public long Part2;
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
    public struct HeapEntry32
    {
        public int dwSize;
        public int hHandle;
        public int dwAddress;
        public int dwBlockSize;
        public HeapEntry32Flags dwFlags;
        public int dwLockCount;
        public int dwResvd;
        public int th32ProcessID;
        public int th32HeapID;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct HeapList32
    {
        public int dwSize;
        public int th32ProcessID;
        public IntPtr th32HeapID;
        public int dwFlags;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct ImagehlpLine64
    {
        public int SizeOfStruct;
        public int Key;
        public int LineNumber;
        public string FileName;
        public long Address;
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

    [StructLayout(LayoutKind.Sequential)]
    public struct KdHelp64
    {
        public long Thread;
        public int ThCallbackStack;
        public int ThCallbackBSTore;
        public int NextCallback;
        public int FramePointer;
        public long KiCallUserMode;
        public long KeUserCallbackDispatcher;
        public long SystemRangeStart;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public long[] Reserved;
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
            int status;
            Luid luid = new Luid();

            if ((status = Win32.NtAllocateLocallyUniqueId(ref luid)) < 0)
                Win32.ThrowLastError(status);

            return luid;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct LuidAndAttributes
    {
        public Luid Luid;
        public SePrivilegeAttributes Attributes;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MemoryBasicInformation
    {
        public IntPtr BaseAddress;
        public IntPtr AllocationBase;
        public MemoryProtection AllocationProtect;
        public int RegionSize;
        public MemoryState State;
        public MemoryProtection Protect;
        public MemoryType Type;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 16)]
    public struct MemoryBasicInformation64
    {
        public IntPtr BaseAddress;
        public IntPtr AllocationBase;
        public MemoryProtection AllocationProtect;
        private int _alignment1;
        public ulong RegionSize;
        public MemoryState State;
        public MemoryProtection Protect;
        public MemoryType Type;
        private int _alignment2;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MibTcpRow
    {
        public MibTcpState State;
        public uint LocalAddress;
        public int LocalPort;
        public uint RemoteAddress;
        public int RemotePort;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MibTcpRowOwnerPid
    {
        public MibTcpState State;
        public uint LocalAddress;
        public int LocalPort;
        public uint RemoteAddress;
        public int RemotePort;
        public int OwningProcessId;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MibTcpStats
    {
        public uint RtoAlgorithm;
        public uint RtoMin;
        public uint RtoMax;
        public uint MaxConn;
        public uint ActiveOpens;
        public uint PassiveOpens;
        public uint AttemptFails;
        public uint EstabResets;
        public uint CurrEstab;
        public uint InSegs;
        public uint OutSegs;
        public uint RetransSegs;
        public uint InErrs;
        public uint OutRsts;
        public uint NumConns;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MibTcpTable
    {
        public int NumEntries;
        public MibTcpRow[] Table;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MibTcpTableOwnerPid
    {
        public int NumEntries;
        public MibTcpRowOwnerPid[] Table;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MibUdpRow
    {
        public uint LocalAddress;
        public int LocalPort;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MibUdpTable
    {
        public int NumEntries;
        public MibUdpRow[] Table;
    }
 
    [StructLayout(LayoutKind.Sequential)]
    public struct MibUdpRowOwnerPid
    {
        public uint LocalAddress;
        public int LocalPort;
        public int OwningProcessId;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MibUdpStats
    {
        public int InDatagrams;
        public int NoPorts;
        public int InErrors;
        public int OutDatagrams;
        public int NumAddrs;
    }
 
    [StructLayout(LayoutKind.Sequential)]
    public struct MibUdpTableOwnerPid
    {
        public int NumEntries;
        public MibUdpRowOwnerPid[] Table;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ModuleEntry32
    {
        public int dwSize;
        public int th32ModuleID;
        public int th32ProcessID;
        public int GlblcntUsage;
        public int ProccntUsage;
        public int modBaseAddr;
        public int modBaseSize;
        public int hModule;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string szModule;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string szExePath;

        public int dwFlags;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ModuleInfo
    {
        public IntPtr BaseOfDll;
        public int SizeOfImage;
        public IntPtr EntryPoint;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MonitorInformation
    {
        public uint Size;
        public Rectangle MonitorRectangle;
        public Rectangle WorkRectangle;
        public uint Flags;
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

        public static ObjectAttributes Create(
            string objectName,
            ObjectFlags attributes,
            DirectoryHandle rootDirectory
            )
        {
            ObjectAttributes oa = new ObjectAttributes();

            oa.Length = Marshal.SizeOf(oa);

            if (objectName != null)
            {
                UnicodeString unicodeString = UnicodeString.Create(objectName);
                IntPtr unicodeStringMemory = Marshal.AllocHGlobal(Marshal.SizeOf(unicodeString));

                Marshal.StructureToPtr(unicodeString, unicodeStringMemory, false);
                oa.ObjectName = unicodeStringMemory;
            }

            oa.Attributes = attributes;

            if (rootDirectory != null)
                oa.RootDirectory = rootDirectory;

            return oa;
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
    public struct PerformanceInformation
    {
        public int Size;
        public int CommitTotal;
        public int CommitLimit;
        public int CommitPeak;
        public int PhysicalTotal;
        public int PhysicalAvailable;
        public int SystemCache;
        public int KernelTotal;
        public int KernelPaged;
        public int KernelNonPaged;
        public int PageSize;
        public int HandlesCount;
        public int ProcessCount;
        public int ThreadCount;
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
    public struct ProcessEntry32
    {
        public int dwSize;
        public int cntUsage;
        public int th32ProcessID;
        public int th32DefaultHeapID;
        public int th32ModuleID;
        public int cntThreads;
        public int th32ParentProcessID;
        public int pcPriClassBase;
        public int dwFlags;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szExeFile;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ProcessInformation
    {
        public IntPtr hProcess;
        public IntPtr hThread;
        public int dwProcessId;
        public int dwThreadId;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct QueryServiceConfig
    {
        public ServiceType ServiceType;
        public ServiceStartType StartType;
        public ServiceErrorControl ErrorControl;

        [MarshalAs(UnmanagedType.LPTStr)]
        public string BinaryPathName;

        [MarshalAs(UnmanagedType.LPTStr)]
        public string LoadOrderGroup;

        public int TagID;
        public int Dependencies; // pointer to a string array

        [MarshalAs(UnmanagedType.LPTStr)]
        public string ServiceStartName;

        [MarshalAs(UnmanagedType.LPTStr)]
        public string DisplayName;
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
    public struct ScAction
    {
        public ScActionType Type;
        public int Delay;
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
        public int EntryPoint;
        public int StackZeroBits;
        public int StackReserved;
        public int StackCommit;
        public int ImageSubsystem;
        public short SubSystemVersionLow;
        public short SubSystemVersionHigh;
        public int Unknown1;
        public int ImageCharacteristics;
        public int ImageMachineType;
        public int Unknown2;
        public int Unknown3;
        public int Unknown4;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ServiceDescription
    {
        [MarshalAs(UnmanagedType.LPWStr)]
        public string Description;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ServiceStatus
    {
        public ServiceType ServiceType;
        public ServiceState CurrentState;
        public ServiceAccept ControlsAccepted;
        public int Win32ExitCode;
        public int ServiceSpecificExitCode;
        public int CheckPoint;
        public int WaitHint;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ServiceStatusProcess
    {
        public ServiceType ServiceType;
        public ServiceState CurrentState;
        public ServiceAccept ControlsAccepted;
        public int Win32ExitCode;
        public int ServiceSpecificExitCode;
        public int CheckPoint;
        public int WaitHint;
        public int ProcessID;
        public ServiceFlags ServiceFlags;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ShellExecuteInfo
    {
        public int cbSize;
        public uint fMask;
        public IntPtr hWnd;
        public string lpVerb;
        public string lpFile;
        public string lpParameters;
        public string lpDirectory;
        public ShowWindowType nShow;
        public IntPtr hInstApp;

        public IntPtr lpIDList;
        public string lpClass;
        public IntPtr hkeyClass;
        public uint dwHotKey;
        public IntPtr hIcon;
        public IntPtr hProcess;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ShFileInfo
    {
        public IntPtr hIcon;
        public IntPtr iIcon;
        public uint dwAttributes;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szDisplayName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
        public string szTypeName;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SidAndAttributes
    {
        public IntPtr SID; // ptr to a SID object
        public SidAttributes Attributes;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct StackFrame64
    {
        public Address64 AddrPC;
        public Address64 AddrReturn;
        public Address64 AddrFrame;
        public Address64 AddrStack;
        public Address64 AddrBStore;

        public int FuncTableEntry;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public long[] Params;

        public int Far;
        public int Virtual;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public long[] Reserved;

        KdHelp64 KdHelp;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct StartupInfo
    {
        public int Size;
        public int Reserved;

        [MarshalAs(UnmanagedType.LPWStr)]
        public string Desktop;

        [MarshalAs(UnmanagedType.LPWStr)]
        public string Title;

        public int X;
        public int Y;
        public int XSize;
        public int YSize;
        public int XCountChars;
        public int YCountChars;
        public int FillAttribute;
        public StartupFlags Flags;

        public ShowWindowType ShowWindow;
        public short Reserved2;
        public int Reserved3;

        public int StdInputHandle;
        public int StdOutputHandle;
        public int StdErrorHandle;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SymbolInfo
    {
        public int SizeOfStruct;
        public int TypeIndex;
        public unsafe fixed long Reserved[2];
        public int Index;
        public int Size;
        public ulong ModBase;
        public SymbolFlags Flags;
        public long Value;
        public long Address;
        public int Register;
        public int Scope;
        public int Tag;
        public int NameLen;
        public int MaxNameLen;
        public char Name;
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
    public struct ThreadEntry32
    {
        public int dwSize;
        public int cntUsage;
        public int th32ThreadID;
        public int th32OwnerProcessID;
        public int tpBasePri;
        public int tpDeltaPri;
        public int dwFlags;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szExeFile;
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
    public struct UnicodeString : IDisposable
    {
        public ushort Length;
        public ushort MaximumLength;
        public IntPtr Buffer;

        public static UnicodeString Create(string str)
        {
            UnicodeString unicodeString = new UnicodeString();

            unicodeString.Buffer = Marshal.StringToHGlobalUni(str);
            unicodeString.Length = (ushort)(str.Length * 2);
            unicodeString.MaximumLength = unicodeString.Length;

            return unicodeString;
        }

        public void Dispose()
        {
            if (this.Buffer == IntPtr.Zero)
                return;

            Marshal.FreeHGlobal(this.Buffer);
        }

        public string Read()
        {
            return Utils.ReadUnicodeString(this);
        }

        public string Read(ProcessHandle processHandle)
        {
            return Utils.ReadUnicodeString(processHandle, this);
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

    [StructLayout(LayoutKind.Sequential)]
    public struct WindowClass
    {
        public int Styles;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public Win32.WndProcDelegate WindowsProc;
        private int ExtraClassData;
        private int ExtraWindowData;
        public IntPtr InstanceHandle;
        public IntPtr IconHandle;
        public IntPtr CursorHandle;
        public IntPtr backgroundBrush;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string MenuName;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string ClassName;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct WintrustCatalogInfo
    {
        public int Size;
        public int CatalogVersion;
        public string CatalogFilePath;
        public string MemberTag;
        public string MemberFilePath;
        public IntPtr MemberFile;
        public byte[] CalculatedFileHash;
        public int CalculatedFileHashSize;
        public IntPtr CatalogContext;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct WintrustData
    {
        public int Size;
        public IntPtr PolicyCallbackData;
        public IntPtr SIPClientData;
        public int UIChoice;
        public WtRevocationChecks RevocationChecks;
        public int UnionChoice;
        public IntPtr UnionData;
        public int StateAction;
        public IntPtr StateData;
        public IntPtr URLReference;
        public WtProvFlags ProvFlags;
        public int UIContext;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct WintrustFileInfo
    {
        public int Size;
        public IntPtr FilePath;
        public IntPtr FileHandle;
        public IntPtr KnownSubject;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct WtsClientDisplay
    {
        public int HorizontalResolution;
        public int VerticalResolution;
        public int ColorDepth;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct WtsProcessInfo
    {
        public int SessionId;
        public int ProcessId;
        public IntPtr ProcessName;
        public IntPtr Sid;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct WtsSessionInfo
    {
        public int SessionID;
        public string WinStationName;
        WtsConnectStateClass State;
    }
}
