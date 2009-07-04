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
using ProcessHacker.Native.Security;

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

    // x86 only
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
    public struct DbgKmCreateProcess
    {
        public int SubSystemKey;
        public IntPtr FileHandle;
        public IntPtr BaseOfImage;
        public int DebugInfoFileOffset;
        public int DebugInfoSize;
        public DbgKmCreateThread InitialThread;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DbgKmCreateThread
    {
        public int SubSystemKey;
        public IntPtr StartAddress;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DbgKmException
    {
        public ExceptionRecord ExceptionRecord;
        public int FirstChance;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DbgKmExitProcess
    {
        public NtStatus ExitStatus;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DbgKmExitThread
    {
        public NtStatus ExitStatus;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DbgKmLoadDll
    {
        public IntPtr FileHandle;
        public IntPtr BaseOfDll;
        public int DebugInfoFileOffset;
        public int DebugInfoSize;
        public IntPtr NamePointer;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DbgKmUnloadDll
    {
        public IntPtr BaseAddress;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DbgUiCreateProcess
    {
        public IntPtr HandleToProcess;
        public IntPtr HandleToThread;
        public DbgKmCreateProcess NewProcess;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DbgUiCreateThread
    {
        public IntPtr HandleToThread;
        public DbgKmCreateThread NewThread;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DbgUiWaitStateChange
    {
        // Overlapping objects and non-objects. Must manually marshal.
        //[StructLayout(LayoutKind.Explicit, Pack = 1)]
        //public struct StateInfoUnion
        //{
        //    [FieldOffset(0)]
        //    public DbgKmException Exception;
        //    [FieldOffset(0)]
        //    public DbgUiCreateThread CreateThread;
        //    [FieldOffset(0)]
        //    public DbgUiCreateProcess CreateProcess;
        //    [FieldOffset(0)]
        //    public DbgKmExitThread ExitThread;
        //    [FieldOffset(0)]
        //    public DbgKmExitProcess ExitProcess;
        //    [FieldOffset(0)]
        //    public DbgKmLoadDll LoadDll;
        //    [FieldOffset(0)]
        //    public DbgKmUnloadDll UnloadDll;
        //}

        public DbgState NewState;
        public ClientId AppClientId;
        //public StateInfoUnion StateInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct EventBasicInformation
    {
        public EventType EventType;
        public int EventState;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ExceptionRecord
    {
        public NtStatus ExceptionCode;
        public int ExceptionFlags;
        public IntPtr ExceptionRecordPtr;
        public IntPtr ExceptionAddress;
        public int NumberParameters;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Win32.ExceptionMaximumParameters)]
        public IntPtr[] ExceptionInformation;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FileAccessInformation
    {
        public FileAccess AccessFlags;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FileAlignmentInformation
    {
        public FileAlignment AlignmentRequirement;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FileAllInformation
    {
        public FileBasicInformation BasicInformation;
        public FileStandardInformation StandardInformation;
        public FileInternalInformation InternalInformation;
        public FileEaInformation EaInformation;
        public FileAccessInformation AccessInformation;
        public FilePositionInformation PositionInformation;
        public FileModeInformation ModeInformation;
        public FileAlignmentInformation AlignmentInformation;
        public FileNameInformation NameInformation;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FileBasicInformation
    {
        public long CreationTime;
        public long LastAccessTime;
        public long LastWriteTime;
        public long ChangeTime;
        public int FileAttributes;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FileDirectoryInformation
    {
        public int NextEntryOffset;
        public int FileIndex;
        public long CreationTime;
        public long LastAccessTime;
        public long LastWriteTime;
        public long ChangeTime;
        public long EndOfFile;
        public long AllocationSize;
        public int FileAttributes;
        public int FileNameLength;
        public char FileName;
        // File name string follows (WCHAR).
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FileEaInformation
    {
        public int EaSize;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FileFsAttributeInformation
    {
        public int FileSystemAttributes;
        public int MaximumComponentNameLength;
        public int FileSystemNameLength;
        public char FileSystemName;
        // File system name string follows (WCHAR).
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FileFsLabelInformation
    {
        public int VolumeLabelLength;
        public char VolumeLabel;
        // Volume label string follows (WCHAR).
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FileFsVolumeInformation
    {
        public long VolumeCreationTime;
        public int VolumeSerialNumber;
        public int VolumeLabelLength;
        [MarshalAs(UnmanagedType.I1)]
        public bool SupportsObjects;
        public char VolumeLabel;
        // Volume label string follows (WCHAR).
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FileInternalInformation
    {
        public long IndexNumber;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FileModeInformation
    {
        public int Mode;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FileNameInformation
    {
        public int FileNameLength;
        public char FileName;
        // File name string follows (WCHAR).
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FileNamesInformation
    {
        public int NextEntryOffset;
        public int FileIndex;
        public int FileNameLength;
        public char FileName;
        // File name string follows (WCHAR).
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FilePipeInformation
    {
        public int ReadMode;
        public int CompletionMode;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FilePipeLocalInformation
    {
        public int NamedPipeType;
        public int NamedPipeConfiguration;
        public int MaximumInstances;
        public int CurrentInstances;
        public int InboundQuota;
        public int ReadDataAvailable;
        public int OutboundQuota;
        public int WriteQuotaAvailable;
        public int NamedPipeState;
        public int NamedPipeEnd;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FilePositionInformation
    {
        public long CurrentByteOffset;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FileStandardInformation
    {
        public long AllocationSize;
        public long EndOfFile;
        public int NumberOfLinks;
        [MarshalAs(UnmanagedType.I1)]
        public bool DeletePending;
        [MarshalAs(UnmanagedType.I1)]
        public bool Directory;
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
    public struct GenericMapping<T>
        where T : struct
    {
        public GenericMapping(T read, T write, T execute, T all)
        {
            this.GenericRead = read;
            this.GenericWrite = write;
            this.GenericExecute = execute;
            this.GenericAll = all;
        }

        public T GenericRead;
        public T GenericWrite;
        public T GenericExecute;
        public T GenericAll;
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
    public struct IoCompletionBasicInformation
    {
        public int Depth;
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
        public IoStatusBlock(NtStatus status)
            : this(status, IntPtr.Zero)
        { }

        public IoStatusBlock(NtStatus status, IntPtr information)
        {
            this.Pointer = IntPtr.Zero;
            this.Status = status;
            this.Information = information;
        }

        public IoStatusBlock(IntPtr pointer)
            : this(pointer, IntPtr.Zero)
        { }

        public IoStatusBlock(IntPtr pointer, IntPtr information)
        {
            this.Status = NtStatus.Success;
            this.Pointer = pointer;
            this.Information = information;
        }

        [FieldOffset(0)]
        public NtStatus Status;
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

    [StructLayout(LayoutKind.Sequential)]
    public struct KeyBasicInformation
    {
        public LargeInteger LastWriteTime;
        public int TitleIndex;
        public int NameLength;
        public char Name;
        // Variable length string follows.
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct KeyCachedInformation
    {
        public LargeInteger LastWriteTime;
        public int TitleIndex;
        public int SubKeys;
        public int MaxNameLen;
        public int Values;
        public int MaxValueNameLen;
        public int MaxValueDataLen;
        public int NameLength;
        public char Name;
        // Variable length string follows.
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct KeyFlagsInformation
    {
        public int UserFlags;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct KeyFullInformation
    {
        public LargeInteger LastWriteTime;
        public int TitleIndex;
        public int ClassOffset;
        public int ClassLength;
        public int SubKeys;
        public int MaxNameLen;
        public int MaxClassLen;
        public int Values;
        public int MaxValueNameLen;
        public int MaxValueDataLen;
        public char Class;
        // Variable length string follows.
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct KeyNameInformation
    {
        public int NameLength;
        public char Name;
        // Variable length string follows.
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct KeyNodeInformation
    {
        public LargeInteger LastWriteTime;
        public int TitleIndex;
        public int ClassOffset;
        public int ClassLength;
        public int NameLength;
        public char Name;
        // Variable length string follows.
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct KeyUserFlagsInformation
    {
        public int UserFlags;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct KeyValueBasicInformation
    {
        public int TitleIndex;
        public int Type;
        public int NameLength;
        public char Name;
        // Variable length string follows.
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct KeyValueEntry
    {
        public IntPtr ValueName; // pointer to UNICODE_STRING
        public int DataLength;
        public int DataOffset;
        public int Type;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct KeyValueFullInformation
    {
        public int TitleIndex;
        public int Type;
        public int DataOffset;
        public int DataLength;
        public int NameLength;
        public char Name;
        // Variable length string follows.
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct KeyValuePartialInformation
    {
        public int TitleIndex;
        public int Type;
        public int DataLength;
        public byte Data;
        // Variable length data follows.
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct KeyWriteTimeInformation
    {
        public LargeInteger LastWriteTime;
    }

    [StructLayout(LayoutKind.Explicit, Size = 8)]
    public struct LargeInteger
    {
        [FieldOffset(0)]
        public long QuadPart;
        [FieldOffset(0)]
        public uint LowPart;
        [FieldOffset(4)]
        public int HighPart;

        public static implicit operator long(LargeInteger li)
        {
            return li.QuadPart;
        }
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

    /// <summary>
    /// Represents a locally unique identifier (LUID), a value which 
    /// is unique on the currently running system.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Pack = 4)]
    public struct Luid
    {
        public static readonly Luid Empty = new Luid();
        public static readonly Luid System = new Luid(0x3e7, 0);
        public static readonly Luid AnonymousLogon = new Luid(0x3e6, 0);
        public static readonly Luid LocalService = new Luid(0x3e5, 0);
        public static readonly Luid NetworkService = new Luid(0x3e4, 0);

        /// <summary>
        /// Creates a LUID from a single 64-bit value.
        /// </summary>
        /// <param name="quadPart">The value.</param>
        public Luid(long quadPart)
        {
            this.LowPart = 0;
            this.HighPart = 0;
            this.QuadPart = quadPart;
        }

        /// <summary>
        /// Creates a LUID from two 32-bit values.
        /// </summary>
        /// <param name="lowPart">The low 32 bits of the LUID.</param>
        /// <param name="highPart">The high 32 bits of the LUID.</param>
        public Luid(uint lowPart, int highPart)
        {
            this.QuadPart = 0;
            this.LowPart = lowPart;
            this.HighPart = highPart;
        }

        /// <summary>
        /// The 64-bit value of the LUID.
        /// </summary>
        [FieldOffset(0)]
        public long QuadPart;
        /// <summary>
        /// The low 32 bits of the LUID.
        /// </summary>
        [FieldOffset(0)]
        public uint LowPart;
        /// <summary>
        /// The high 32 bits of the LUID.
        /// </summary>
        [FieldOffset(4)]
        public int HighPart;

        /// <summary>
        /// Allocates a locally unique identifier (LUID) from 
        /// the kernel.
        /// </summary>
        /// <returns>A new LUID.</returns>
        public static Luid Allocate()
        {
            NtStatus status;
            Luid luid;

            if ((status = Win32.NtAllocateLocallyUniqueId(out luid)) >= NtStatus.Error)
                Win32.ThrowLastError(status);

            return luid;
        }

        public override string ToString()
        {
            return this.QuadPart.ToString("x");
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
    public struct Peb
    {
        [MarshalAs(UnmanagedType.I1)]
        public bool InheritedAddressSpace;
        [MarshalAs(UnmanagedType.I1)]
        public bool ReadImageFileExecOptions;
        [MarshalAs(UnmanagedType.I1)]
        public bool BeingDebugged;
        [MarshalAs(UnmanagedType.I1)]
        public bool BitField;
        public IntPtr Mutant;

        public IntPtr ImageBaseAddress;
        public IntPtr Ldr; // ptr to PebLdrData
        public IntPtr ProcessParameters; // ptr to RtlUserProcessParameters
        public IntPtr SubSystemData;
        public IntPtr ProcessHeap;
        public IntPtr FastPebLock;
        public IntPtr AtlThunkSListPtr;
        public IntPtr SparePrt2;
        public int EnvironmentUpdateCount;
        public IntPtr KernelCallbackTable;
        public int SystemReserved;
        public int SpareUlong;
        public IntPtr FreeList;
        public int TlsExpansionCounter;
        public IntPtr TlsBitmap;
        public unsafe fixed int TlsBitmapBits[2];
        public IntPtr ReadOnlySharedMemoryBase;
        public IntPtr ReadOnlySharedMemoryHeap;
        public IntPtr ReadOnlyStaticServerData;
        public IntPtr AnsiCodePageData;
        public IntPtr OemCodePageData;
        public IntPtr UnicodeCaseTableData;

        public int NumberOfProcessors;
        public int NtGlobalFlag;

        public long CriticalSectionTimeout;
        public IntPtr HeapSegmentReserve;
        public IntPtr HeapSegmentCommit;
        public IntPtr HeapDeCommitTotalFreeThreshold;
        public IntPtr HeapDeCommitFreeBlockThreshold;

        public int NumberOfHeaps;
        public int MaximumNumberOfHeaps;
        public IntPtr ProcessHeaps;

        public IntPtr GdiSharedHandleTable;
        public IntPtr ProcessStarterHelper;
        public int GdiDCAttributeList;
        public IntPtr LoaderLock;

        public int OSMajorVersion;
        public int OSMinorVersion;
        public short OSBuildNumber;
        public short OSCSDVersion;
        public int OSPlatformId;
        public int ImageSubsystem;
        public int ImageSubsystemMajorVersion;
        public int ImageSubsystemMinorVersion;
        public IntPtr ImageProcessAffinityMask;
        public unsafe fixed byte GdiHandleBuffer[Win32.GdiHandleBufferSize];
        public IntPtr PostProcessInitRoutine;

        public IntPtr TlsExpansionBitmap;
        public unsafe fixed int TlsExpansionBitmapBits[32];

        public int SessionId;

        public long AppCompatFlags;
        public long AppCompatFlagsUser;
        public IntPtr pShimData;
        public IntPtr AppCompatInfo;

        public UnicodeString CSDVersion;

        public IntPtr ActivationContextData;
        public IntPtr ProcessAssemblyStorageMap;
        public IntPtr SystemDefaultActivationContextData;
        public IntPtr SystemAssemblyStorageMap;

        public IntPtr MinimumStackCommit;

        public IntPtr FlsCallback;
        public ListEntry FlsListHead;
        public IntPtr FlsBitmap;
        public unsafe fixed int FlsBitmapBits[Win32.FlsMaximumAvailable / (sizeof(int) * 8)];
        public int FlsHighIndex;
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

    [StructLayout(LayoutKind.Sequential)]
    public struct PortMessageStruct
    {
        public short DataLength;
        public short TotalLength;
        public PortMessageType Type;
        public short DataInfoOffset;
        public ClientId ClientId;
        public int MessageId;
        public IntPtr ClientViewSize;
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
    public struct PrivilegeSetStruct
    {
        public int Count;
        public PrivilegeSetFlags Flags;
        [MarshalAs(UnmanagedType.ByValArray)]
        public LuidAndAttributes[] Privileges;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ProcessBasicInformation
    {
        public NtStatus ExitStatus;
        public IntPtr PebBaseAddress;
        public IntPtr AffinityMask;
        public int BasePriority;
        public IntPtr UniqueProcessId;
        public IntPtr InheritedFromUniqueProcessId;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ProcessHandleTracingEnable
    {
        public int Flags; // No flags. Set to 0.
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ProcessHandleTracingEnableEx
    {
        public int Flags; // No flags. Set to 0.
        public int TotalSlots;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ProcessHandleTracingEntry
    {
        public IntPtr Handle;
        public ClientId ClientId;
        public HandleTraceType Type;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Win32.ProcessHandleTracingMaxStacks)]
        public IntPtr[] Stacks;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ProcessHandleTracingQuery
    {
        public IntPtr Handle;
        public int TotalTraces;
        public char HandleTrace;
        // An array of ProcessHandleTracingEntry structures follows.
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
    public struct RtlDebugInformation
    {
        public IntPtr SectionHandleClient;
        public IntPtr ViewBaseClient;
        public IntPtr ViewBaseTarget;
        public IntPtr ViewBaseDelta;
        public IntPtr EventPairClient;
        public IntPtr EventPairTarget;
        public IntPtr TargetProcessId;
        public IntPtr TargetThreadHandle;
        public int Flags;
        public IntPtr OffsetFree;
        public IntPtr CommitSize;
        public IntPtr ViewSize;
        public IntPtr Modules;
        public IntPtr BackTraces;
        public IntPtr Heaps;
        public IntPtr Locks;
        public IntPtr SpecificHeap;
        public IntPtr TargetProcessHandle;
#if _X64
        public unsafe fixed long Reserved[6];
#else
        public unsafe fixed int Reserved[6];
#endif
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RtlHandleTable
    {
        public int MaximumNumberOfHandles;
        public int SizeOfHandleTableEntry;
        public int Reserved1;
        public int Reserved2;
        public IntPtr FreeHandles;
        public IntPtr CommittedHandles;
        public IntPtr UnCommittedHandles;
        public IntPtr MaxReservedHandles;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RtlProcessBacktraceInformation
    {
        public IntPtr SymbolicBackTrace; // PCHAR, always NULL.
        public int TraceCount;
        public ushort Index;
        public ushort Depth;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Win32.MaxStackDepth)]
        public IntPtr[] BackTrace;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RtlProcessBacktraces
    {
        public int CommittedMemory;
        public int ReservedMemory;
        public int NumberOfBackTraceLookups;
        public int NumberOfBackTraces;
        public char BackTraces; // RtlProcessBacktraceInformation[] BackTraces
        // Array of RtlProcessBacktraceInformation structures follows.
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
    public struct SidStruct
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

        public Sid ToSid()
        {
            return new Sid(this);
        }
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
        public IntPtr StackBase; // 16
        public IntPtr StackLimit;
        public IntPtr Win32StartAddress;
        public IntPtr TebAddress; // Vista+
        public IntPtr Unused1;
        public IntPtr Unused2;
        public IntPtr Unused3;
    }

    [StructLayout(LayoutKind.Sequential)]
    public partial struct SystemHandleInformation
    {
        public int ProcessId;
        public byte ObjectTypeNumber;
        public HandleFlags Flags;
        public short Handle;
        public IntPtr Object;
        public int GrantedAccess;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SystemLoadAndCallImage
    {
        public UnicodeString ModuleName;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SystemObjectInformation
    {
        public int NextEntryOffset;
        public IntPtr Object;
        public IntPtr CreatorUniqueProcess;
        public ushort CreatorBackTraceIndex;
        public ushort Flags;
        public int PointerCount;
        public int HandleCount;
        public uint PagedPoolCharge;
        public uint NonPagedPoolCharge;
        public IntPtr ExclusiveProcessId;
        public IntPtr SecurityDescriptor;
        public ObjectNameInformation NameInfo;
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
    public struct SystemPagefileInformation
    {
        public int NextEntryOffset;
        public int TotalSize;
        public int TotalInUse;
        public int PeakUsage;
        public UnicodeString PageFileName;
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
        public int ProcessId; // should be IntPtr
        public int InheritedFromProcessId; // should be IntPtr
        public int HandleCount;
        public int SessionId;
        public IntPtr PageDirectoryBase;
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
        public IntPtr StartAddress;
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
        public NtStatus ExitStatus;
        public IntPtr TebBaseAddress;
        public ClientId ClientId;
        public IntPtr AffinityMask;
        public int Priority;
        public int BasePriority;
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
        public TokenGroups(Sid[] sids)
        {
            this.GroupCount = sids.Length;
            this.Groups = new SidAndAttributes[sids.Length];

            for (int i = 0; i < sids.Length; i++)
                this.Groups[i] = sids[i].ToSidAndAttributes();
        }

        public int GroupCount;

        [MarshalAs(UnmanagedType.ByValArray)]
        public SidAndAttributes[] Groups;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TokenOwner
    {
        public TokenOwner(Sid owner)
        {
            this.Owner = owner;
        }

        public IntPtr Owner;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TokenPrimaryGroup
    {
        public TokenPrimaryGroup(Sid primaryGroup)
        {
            this.PrimaryGroup = primaryGroup;
        }

        public IntPtr PrimaryGroup;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TokenPrivileges
    {
        public TokenPrivileges(PrivilegeSet privileges)
        {
            this = privileges.ToTokenPrivileges();
        }

        public int PrivilegeCount;

        [MarshalAs(UnmanagedType.ByValArray)]
        public LuidAndAttributes[] Privileges;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct TokenSource
    {
        public TokenSource(string sourceName, Luid sourceIdentifier)
        {
            if (sourceName.Length > 8)
                throw new ArgumentException("Source name must be equal to or less than 8 characters long.");

            this.SourceName = sourceName;
            this.SourceIdentifier = sourceIdentifier;
        }

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
        public string SourceName;

        public Luid SourceIdentifier;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TokenStatistics
    {
        public Luid TokenId;
        public Luid AuthenticationId;
        public long ExpirationTime;
        public TokenType TokenType;
        public SecurityImpersonationLevel ImpersonationLevel;
        public int DynamicCharged;
        public int DynamicAvailable;
        public int GroupCount;
        public int PrivilegeCount;
        public Luid ModifiedId;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TokenUser
    {
        public TokenUser(Sid user)
        {
            this.User = user.ToSidAndAttributes();
        }

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
            if (this.Length == 0)
                return "";

            return Marshal.PtrToStringUni(this.Buffer, this.Length / 2);
        }

        public string Read(ProcessHandle processHandle)
        {
            if (this.Length == 0)
                return "";

            byte[] strData = processHandle.ReadMemory(this.Buffer, this.Length);
            GCHandle strDataHandle = GCHandle.Alloc(strData, GCHandleType.Pinned);

            try
            {
                return Marshal.PtrToStringUni(strDataHandle.AddrOfPinnedObject(), this.Length / 2);
            }
            finally
            {
                strDataHandle.Free();
            }
        }

        public bool StartsWith(UnicodeString unicodeString, bool caseInsensitive)
        {
            return Win32.RtlPrefixUnicodeString(ref this, ref unicodeString, caseInsensitive);
        }

        public bool StartsWith(UnicodeString unicodeString)
        {
            return this.StartsWith(unicodeString, false);
        }

        public override string ToString()
        {
            return this.Read();
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct VmCounters
    {
        // These all should be IntPtrs...
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
        // These all should be IntPtrs...
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
