/*
 * Process Hacker - 
 *   windows API structs
 * 
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.ComponentModel;

namespace ProcessHacker
{
    public partial class Win32
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct ADDRESS64
        {
            public long Offset;
            public short Segment;
            public ADDRESS_MODE Mode;
        }
        
        [StructLayout(LayoutKind.Sequential)]
        public struct CLIENT_ID
        {
            public int UniqueProcess;
            public int UniqueThread;
        }

        // NOTE: This x86 CONTEXT ONLY!!!
        [StructLayout(LayoutKind.Sequential)]
        public struct CONTEXT
        {
            public CONTEXT_FLAGS ContextFlags;

            public int Dr0;
            public int Dr1;
            public int Dr2;
            public int Dr3;
            public int Dr4;
            public int Dr5;
            public int Dr6;
            public int Dr7;

            [MarshalAs(UnmanagedType.Struct)]
            public FLOATING_SAVE_AREA FloatSave;

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

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAXIMUM_SUPPORTED_EXTENSION)]
            public byte[] ExtendedRegisters;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ENUM_SERVICE_STATUS
        {
            [MarshalAs(UnmanagedType.LPTStr)]
            public string ServiceName;

            [MarshalAs(UnmanagedType.LPTStr)]
            public string DisplayName;

            [MarshalAs(UnmanagedType.Struct)]
            public SERVICE_STATUS ServiceStatus;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ENUM_SERVICE_STATUS_PROCESS
        {
            [MarshalAs(UnmanagedType.LPTStr)]
            public string ServiceName;

            [MarshalAs(UnmanagedType.LPTStr)]
            public string DisplayName;

            [MarshalAs(UnmanagedType.Struct)]
            public SERVICE_STATUS_PROCESS ServiceStatusProcess;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct EVENT_BASIC_INFORMATION
        {
            public EVENT_TYPE EventType;
            public int EventState;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct FLOATING_SAVE_AREA
        {
            public int ControlWord;
            public int StatusWord;
            public int TagWord;
            public int ErrorOffset;
            public int ErrorSelector;
            public int DataOffset;
            public int DataSelector;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = SIZE_OF_80387_REGISTERS)]
            public byte[] RegisterArea;

            public int Cr0NpxState;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct FPO_DATA
        {
            public int ulOffStart;
            public int cbProcSize;
            public int cdwLocals;
            public short cdwParams;

            public long Part1;
            public long Part2;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct GENERIC_MAPPING
        {
            public STANDARD_RIGHTS GenericRead;
            public STANDARD_RIGHTS GenericWrite;
            public STANDARD_RIGHTS GenericExecute;
            public STANDARD_RIGHTS GenericAll;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct GUID
        {
            public uint Data1;
            public ushort Data2;
            public ushort Data3;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] Data4;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HEAPENTRY32
        {
            public int dwSize;
            public int hHandle;
            public int dwAddress;
            public int dwBlockSize;
            public HEAPENTRY32FLAGS dwFlags;
            public int dwLockCount;
            public int dwResvd;
            public int th32ProcessID;
            public int th32HeapID;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HEAPLIST32
        {
            public int dwSize;
            public int th32ProcessID;
            public int th32HeapID;
            public int dwFlags;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct IO_COUNTERS
        {
            public ulong ReadOperationCount;
            public ulong WriteOperationCount;
            public ulong OtherOperationCount;
            public ulong ReadTransferCount;
            public ulong WriteTransferCount;
            public ulong OtherTransferCount;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct KDHELP64
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
        public struct LSA_UNICODE_STRING
        {
            public ushort Length;
            public ushort MaximumLength;

            [MarshalAs(UnmanagedType.LPWStr)]
            public string Buffer;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct LUID
        {
            public int LowPart;
            public int HighPart;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct LUID_AND_ATTRIBUTES
        {
            public LUID Luid;
            public SE_PRIVILEGE_ATTRIBUTES Attributes;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MEMORY_BASIC_INFORMATION
        {
            public int BaseAddress;
            public int AllocationBase;
            public MEMORY_PROTECTION AllocationProtect;
            public int RegionSize;
            public MEMORY_STATE State;
            public MEMORY_PROTECTION Protect;
            public MEMORY_TYPE Type;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MIB_TCPROW
        {
            public MIB_TCP_STATE State;
            public uint LocalAddress;
            public int LocalPort;
            public uint RemoteAddress;
            public int RemotePort;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MIB_TCPROW_OWNER_PID
        {
            public MIB_TCP_STATE State;
            public uint LocalAddress;
            public int LocalPort;
            public uint RemoteAddress;
            public int RemotePort;
            public int OwningProcessId;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MIB_TCPSTATS
        {
            public int RtoAlgorithm;
            public int RtoMin;
            public int RtoMax;
            public int MaxConn;
            public int ActiveOpens;
            public int PassiveOpens;
            public int AttemptFails;
            public int EstabResets;
            public int CurrEstab;
            public int InSegs;
            public int OutSegs;
            public int RetransSegs;
            public int InErrs;
            public int OutRsts;
            public int NumConns;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MIB_TCPTABLE
        {
            public int NumEntries;
            public MIB_TCPROW[] Table;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MIB_TCPTABLE_OWNER_PID
        {
            public int NumEntries;
            public MIB_TCPROW_OWNER_PID[] Table;
        }

        //http://msdn.microsoft.com/en-us/library/aa366889(VS.85).aspx
        //UDPRow  
        [StructLayout(LayoutKind.Sequential)]
        public struct MIB_UDPROW
        {
            public uint LocalAddress;
            public int LocalPort;
        }

        //UDPTable   
        [StructLayout(LayoutKind.Sequential)]
        public struct MIB_UDPTABLE
        {
            public int NumEntries;
            public MIB_UDPROW[] Table;
        }

        //UDPRow And OwnerPID  
        [StructLayout(LayoutKind.Sequential)]
        public struct MIB_UDPROW_OWNER_PID
        {
            public uint LocalAddress;
            public int LocalPort;
            public int OwningProcessId;
        }

        //UDPStats
        [StructLayout(LayoutKind.Sequential)]
        public struct MIB_UDPSTATS
        {
            public int InDatagrams;
            public int NoPorts;
            public int InErrors;
            public int OutDatagrams;
            public int NumAddrs;
        }
        //later
        //MIB_UDP6ROW,MIB_UDP6TABLE,MIB_UDP6ROW_OWNER_PID,MIB_UDP6TABLE_OWNER_PID...  

        //UDPRowTable And OwnerPID     
        [StructLayout(LayoutKind.Sequential)]
        public struct MIB_UDPTABLE_OWNER_PID
        {
            public int NumEntries;
            public MIB_UDPROW_OWNER_PID[] Table;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MODULEENTRY32
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

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szExePath;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MUTANT_BASIC_INFORMATION
        {
            public int CurrentCount;
            public byte OwnedByCaller;
            public byte AbandonedState;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct OBJECT_ATTRIBUTES
        {
            public int Length;
            public int RootDirectory;
            public IntPtr ObjectName;
            public uint Attributes;
            public int SecurityDescriptor;
            public int SecurityQualityOfService;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct OBJECT_BASIC_INFORMATION
        {
            public uint Attributes;
            public STANDARD_RIGHTS GrantedAccess;
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
        public struct OBJECT_NAME_INFORMATION
        {
            public UNICODE_STRING Name;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct OBJECT_TYPE_INFORMATION
        {
            public UNICODE_STRING Name;
            public uint ObjectCount;
            public uint HandleCount;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public uint[] Reserved1;

            public uint PeakObjectCount;
            public uint PeakHandleCount;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public uint[] Reserved2;

            public STANDARD_RIGHTS InvalidAttributes;
            public GENERIC_MAPPING GenericMapping;
            public STANDARD_RIGHTS ValidAccess;
            public byte SecurityRequired;
            public byte MaintainHandleCount;
            public ushort MaintainTypeList;

            public POOL_TYPE PoolType;
            public uint PagedPoolUsage;
            public uint NonPagedPoolUsage;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PERFORMANCE_INFORMATION
        {
            public int Size;

            // I've made this an array because we already have this 
            // stuff from SYSTEM_PERFORMANCE_INFORMATION. I just want 
            // counts below.
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
            public int[] IDontCare;

            public int HandlesCount;
            public int ProcessCount;
            public int ThreadCount;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POOLED_USAGE_AND_LIMITS
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
        public struct PROCESS_BASIC_INFORMATION
        {
            public int ExitStatus;
            public int PebBaseAddress;
            public int AffinityMask;
            public int BasePriority;
            public int UniqueProcessId;
            public int InheritedFromUniqueProcessId;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PROCESSENTRY32
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
        public struct PROCESS_INFORMATION
        {
            int hProcess;
            int hThread;
            int dwProcessId;
            int dwThreadId;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct QUERY_SERVICE_CONFIG
        {
            public SERVICE_TYPE ServiceType;
            public SERVICE_START_TYPE StartType;
            public SERVICE_ERROR_CONTROL ErrorControl;

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
        public struct QUOTA_LIMITS
        {
            public int PagedPoolLimit;
            public int NonPagedPoolLimit;
            public int MinimumWorkingSetSize;
            public int MaximumWorkingSetSizse;
            public int PagefileLimit;
            public long TimeLimit;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SC_ACTION
        {
            public SC_ACTION_TYPE Type;
            public int Delay;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SECTION_BASIC_INFORMATION
        {
            public int Unknown;
            public SECTION_ATTRIBUTES SectionAttributes;
            public long SectionSize;
        }  

        [StructLayout(LayoutKind.Sequential)]
        public struct SECTION_IMAGE_INFORMATION
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
        public struct SERVICE_STATUS
        {
            public SERVICE_TYPE ServiceType;
            public SERVICE_STATE CurrentState;
            public SERVICE_ACCEPT ControlsAccepted;
            public int Win32ExitCode;
            public int ServiceSpecificExitCode;
            public int CheckPoint;
            public int WaitHint;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SERVICE_STATUS_PROCESS
        {
            public SERVICE_TYPE ServiceType;
            public SERVICE_STATE CurrentState;
            public SERVICE_ACCEPT ControlsAccepted;
            public int Win32ExitCode;
            public int ServiceSpecificExitCode;
            public int CheckPoint;
            public int WaitHint;
            public int ProcessID;
            public SERVICE_FLAGS ServiceFlags;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SHELLEXECUTEINFO
        {
            public int cbSize;
            public int fMask;
            public int hWnd;
            public string lpVerb;
            public string lpFile;
            public string lpParameters;
            public string lpDirectory;
            public int nShow;
            public int hInstApp;

            public int lpIDList;
            public string lpClass;
            public int hkeyClass;
            public int dwHotKey;
            public int hIcon;
            public int hProcess;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SHFILEINFO
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
        public struct SID_AND_ATTRIBUTES
        {
            public int SID; // ptr to a SID object
            public SID_ATTRIBUTES Attributes;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct STACKFRAME64
        {
            public ADDRESS64 AddrPC;
            public ADDRESS64 AddrReturn;
            public ADDRESS64 AddrFrame;
            public ADDRESS64 AddrStack;
            public ADDRESS64 AddrBStore;

            public int FuncTableEntry;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public long[] Params;

            public int Far;
            public int Virtual;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public long[] Reserved;

            KDHELP64 KdHelp;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct STARTUPINFO
        {
            int cb;
            [MarshalAs(UnmanagedType.LPTStr)]
            string lpReserved;
            [MarshalAs(UnmanagedType.LPTStr)]
            string lpDesktop;
            [MarshalAs(UnmanagedType.LPTStr)]
            string lpTitle;
            int dwX;
            int dwY;
            int dwXSize;
            int dwYSize;
            int dwXCountChars;
            int dwYCountChars;
            int dwFillAttribute;
            int dwFlags;
            short wShowWindow;
            short cbReserved2;
            byte lpReserved2;
            int hStdInput;
            int hStdOutput;
            int hStdError;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SYMBOL_INFO
        {
            public int SizeOfStruct;
            public int TypeIndex;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public long[] Reserved;

            public int Index;
            public int Size;
            public long ModBase;
            public SYMBOL_FLAGS Flags;
            public long Value;
            public long Address;
            public int Register;
            public int Scope;
            public int Tag;
            public int NameLen;
            public int MaxNameLen;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = SYMBOL_NAME_MAXSIZE)]
            public string Name;
        } 

        [StructLayout(LayoutKind.Sequential)]
        public struct SYSTEM_BASIC_INFORMATION
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
        public struct SYSTEM_CACHE_INFORMATION
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
        public struct SYSTEM_HANDLE_INFORMATION
        {
            public int ProcessId;
            public byte ObjectTypeNumber;
            public SYSTEM_HANDLE_FLAGS Flags;
            public short Handle;
            public int Object;
            public STANDARD_RIGHTS GrantedAccess;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SYSTEM_LOAD_AND_CALL_IMAGE
        {
            public UNICODE_STRING ModuleName;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SYSTEM_OBJECT_TYPE_INFORMATION
        {
            public uint NextEntryOffset;
            public uint ObjectCount;
            public uint HandleCount;
            public uint TypeNumber;
            public uint InvalidAttributes;
            public GENERIC_MAPPING GenericMapping;
            public STANDARD_RIGHTS ValidAccessMask;
            public POOL_TYPE PoolType;
            public byte Unknown;

            [MarshalAs(UnmanagedType.Struct)]
            public UNICODE_STRING Name;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SYSTEM_PERFORMANCE_INFORMATION
        {
            /// <summary>
            /// The total idle time of all processors in units of 100-nanoseconds.
            /// </summary>
            public long IdleTime;

            /// <summary>
            /// Total bytes read by calls to ZwReadFile.
            /// </summary>
            public long IoReadTransferCount;

            /// <summary>
            /// Total bytes written by calls to ZwWriteFile.
            /// </summary>
            public long IoWriteTransferCount;

            /// <summary>
            /// Total bytes transferred by other I/O operations.
            /// </summary>
            public long IoOtherTransferCount;

            /// <summary>
            /// Number of calls to ZwReadFile.
            /// </summary>
            public int IoReadOperationCount;

            /// <summary>
            /// Number of calls to ZwWriteFile.
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
        public struct SYSTEM_PROCESS_INFORMATION
        {
            public int NextEntryOffset;
            public int NumberOfThreads;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public long[] Reserved1;

            public long CreateTime;
            public long UserTime;
            public long KernelTime;
            public UNICODE_STRING ImageName;
            public int BasePriority;
            public int ProcessId;
            public int InheritedFromProcessId;
            public int HandleCount;
            public int SessionId;
            public int PageDirectoryBase;
            public VM_COUNTERS_EX VirtualMemoryCounters;
            public IO_COUNTERS IoCounters;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SYSTEM_PROCESSOR_PERFORMANCE_INFORMATION
        {
            public long IdleTime;
            public long KernelTime;
            public long UserTime;
            public long DpcTime;
            public long InterruptTime;
            public int InterruptCount;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SYSTEM_THREAD_INFORMATION
        {
            public long KernelTime;
            public long UserTime;
            public long CreateTime;
            public int WaitTime;
            public int StartAddress;
            public CLIENT_ID ClientId;
            public int Priority;
            public int BasePriority;
            public int ContextSwitchCount;
            public int State;
            public KWAIT_REASON WaitReason;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct THREAD_BASIC_INFORMATION
        {
            public int ExitStatus;
            public int TebBaseAddress;
            public CLIENT_ID ClientId;
            public int AffinityMask;
            public int Priority;
            public int BasePriority;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct THREADENTRY32
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
        public struct TOKEN_GROUPS
        {
            public uint GroupCount;

            [MarshalAs(UnmanagedType.ByValArray)]
            public SID_AND_ATTRIBUTES[] Groups;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct TOKEN_PRIVILEGES
        {
            public uint PrivilegeCount;

            [MarshalAs(UnmanagedType.ByValArray)]
            public LUID_AND_ATTRIBUTES[] Privileges;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct TOKEN_SOURCE
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
            public string SourceName;

            public LUID SourceIdentifier;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct TOKEN_USER
        {
            public SID_AND_ATTRIBUTES User;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct UNICODE_STRING
        {
            public ushort Length;
            public ushort MaximumLength;

            public int Buffer;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct VM_COUNTERS
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
        public struct VM_COUNTERS_EX
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
        public struct WINTRUST_DATA
        {
            public int Size;
            public int PolicyCallbackData;
            public int SIPClientData;
            public int UIChoice;
            public int RevocationChecks;
            public int UnionChoice;
            public IntPtr File;
            public int StateAction;
            public int StateData;
            public int URLReference;
            public int ProvFlags;
            public int UIContext;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WINTRUST_FILE_INFO
        {
            public int Size;
            public IntPtr FilePath;
            public int FileHandle;
            public int KnownSubject;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WTS_CLIENT_DISPLAY
        {
            public int HorizontalResolution;
            public int VerticalResolution;
            public int ColorDepth;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WTS_PROCESS_INFO
        {
            public int SessionID;
            public int ProcessID;

            [MarshalAs(UnmanagedType.LPTStr)]
            public string ProcessName;

            public int SID;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WTS_SESSION_INFO
        {
            public int SessionID;

            [MarshalAs(UnmanagedType.LPTStr)]
            public string WinStationName;

            WTS_CONNECTSTATE_CLASS State;
        }
    }
}
