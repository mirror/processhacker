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

/* This file contains structure declarations for the Native API.
 * Structures shared between the Native API and Win32 are placed 
 * in this file.
 */

using System;
using System.Runtime.InteropServices;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;
using ProcessHacker.Native.Security.AccessControl;

namespace ProcessHacker.Native.Api
{
    [StructLayout(LayoutKind.Sequential)]
    public struct AccessAllowedAceStruct
    {
        public AceHeader Header;
        public int Mask;
        public int SidStart;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct AccessAllowedObjectAceStruct
    {
        public AceHeader Header;
        public int Mask;
        public ObjectAceFlags Flags;
        public Guid ObjectType;
        public Guid InheritedObjectType;
        public int SidStart;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct AccessDeniedAceStruct
    {
        public AceHeader Header;
        public int Mask;
        public int SidStart;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct AccessDeniedObjectAceStruct
    {
        public AceHeader Header;
        public int Mask;
        public ObjectAceFlags Flags;
        public Guid ObjectType;
        public Guid InheritedObjectType;
        public int SidStart;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct AceData
    {
        public AceType AceType;
        public byte InheritFlags;
        public AceFlags AceFlags;
        public int Mask;
        public IntPtr Sid; // Sid**
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct AceHeader
    {
        public AceType AceType;
        public AceFlags AceFlags;
        public ushort AceSize;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct AclRevisionInformation
    {
        public int AclRevision;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct AclSizeInformation
    {
        public int AceCount;
        public int AclBytesInUse;
        public int AclBytesFree;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct AclStruct
    {
        public byte AclRevision;
        public byte Sbz1;
        public ushort AclSize;
        public ushort AceCount;
        public ushort Sbz2;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct AnsiString : IDisposable
    {
        public AnsiString(string str)
        {
            UnicodeString unicodeStr;

            unicodeStr = new UnicodeString(str);
            this = unicodeStr.ToAnsiString();
            unicodeStr.Dispose();
        }

        public ushort Length;
        public ushort MaximumLength;
        public IntPtr Buffer;

        public void Dispose()
        {
            if (this.Buffer == IntPtr.Zero)
                return;

            Win32.RtlFreeAnsiString(ref this);
            this.Buffer = IntPtr.Zero;
        }

        public UnicodeString ToUnicodeString()
        {
            NtStatus status;
            UnicodeString unicodeStr = new UnicodeString();

            if ((status = Win32.RtlAnsiStringToUnicodeString(ref unicodeStr, ref this, true)) >= NtStatus.Error)
                Win32.Throw(status);

            return unicodeStr;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct BaseCreateProcessMsg
    {
        public IntPtr ProcessHandle;
        public IntPtr ThreadHandle;
        public ClientId ClientId;
        public ClientId DebuggerClientId;
        public ProcessCreationFlags CreationFlags;
        public int IsVdm;
        public IntPtr VdmHandle;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct BaseCreateThreadMsg
    {
        public IntPtr ThreadHandle;
        public ClientId ClientId;
    }

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

    [StructLayout(LayoutKind.Sequential)]
    public struct CompoundAccessAllowedAceStruct
    {
        public AceHeader Header;
        public int Mask;
        public CompoundAceType CompoundAceType;
        public ushort Reserved;
        public int SidStart;
    }

    /// <summary>
    /// x86 context
    /// </summary>
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

    /// <summary>
    /// AMD64 context.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct ContextAmd64
    {
        public long P1Home;
        public long P2Home;
        public long P3Home;
        public long P4Home;
        public long P5Home;
        public long P6Home;

        public ContextFlagsAmd64 ContextFlags;
        public int MxCsr;

        public ushort SegCs;
        public ushort SegDs;
        public ushort SegEs;
        public ushort SegFs;
        public ushort SegGs;
        public ushort SegSs;
        public int EFlags;

        public long Dr0;
        public long Dr1;
        public long Dr2;
        public long Dr3;
        public long Dr6;
        public long Dr7;

        public long Rax;
        public long Rcx;
        public long Rdx;
        public long Rbx;
        public long Rsp;
        public long Rbp;
        public long Rsi;
        public long Rdi;
        public long R8;
        public long R9;
        public long R10;
        public long R11;
        public long R12;
        public long R13;
        public long R14;
        public long R15;

        public long Rip;

        public XmmSaveArea32 FltSave;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 26)]
        public M128A[] VectorRegister;
        public long VectorControl;

        public long DebugControl;
        public long LastBranchToRip;
        public long LastBranchFromRip;
        public long LastExceptionToRip;
        public long LastExceptionFromRip;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CsrApiMsg
    {
        public static readonly int ApiMessageDataOffset =
            Marshal.OffsetOf(typeof(CsrApiMsg), "ApiMessageData").ToInt32();

        public PortMessageStruct Header;
        public IntPtr CaptureBuffer; // CsrCaptureHeader*
        public int ApiNumber;
        public int ReturnValue;
        public int Reserved;
        public int ApiMessageData;
        // API message data follows.
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CsrCaptureHeader
    {
        public static readonly int MessagePointerOffsetsOffset =
            Marshal.OffsetOf(typeof(CsrCaptureHeader), "MessagePointerOffsets").ToInt32();

        public int Length;
        public IntPtr RelatedCaptureBuffer; // CsrCaptureBuffer*
        public int CountMessagePointers;
        public IntPtr FreeSpace;
        public IntPtr MessagePointerOffsets;
        // Array of ULONG_PTRs follows.
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
    public struct EnlistmentBasicInformation
    {
        public Guid EnlistmentId;
        public Guid TransactionId;
        public Guid ResourceManagerId;
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
        public FileAttributes FileAttributes;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FileCompletionInformation
    {
        public IntPtr Port;
        public IntPtr Key;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FileDirectoryInformation
    {
        public static int FileNameOffset = 
            Marshal.OffsetOf(typeof(FileDirectoryInformation), "FileName").ToInt32();

        public int NextEntryOffset;
        public int FileIndex;
        public long CreationTime;
        public long LastAccessTime;
        public long LastWriteTime;
        public long ChangeTime;
        public long EndOfFile;
        public long AllocationSize;
        public FileAttributes FileAttributes;
        public int FileNameLength;
        public short FileName;
        // File name string follows (WCHAR).
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FileDispositionInformation
    {
        [MarshalAs(UnmanagedType.I1)]
        public bool DeleteFile;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FileEaInformation
    {
        public int EaSize;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FileEndOfFileInformation
    {
        public long EndOfFile;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FileFsAttributeInformation
    {
        public int FileSystemAttributes;
        public int MaximumComponentNameLength;
        public int FileSystemNameLength;
        public short FileSystemName;
        // File system name string follows (WCHAR).
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FileFsLabelInformation
    {
        public int VolumeLabelLength;
        public short VolumeLabel;
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
        public short VolumeLabel;
        // Volume label string follows (WCHAR).
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FileInternalInformation
    {
        public long IndexNumber;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FileMailslotQueryInformation
    {
        public int MaximumMessageSize;
        public int MailslotQuota;
        public int NextMessageSize;
        public int MessagesAvailable;
        public long ReadTimeout;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FileMailslotSetInformation
    {
        public long ReadTimeout;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FileModeInformation
    {
        public FileObjectFlags Mode;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FileNameInformation
    {
        public static int FileNameOffset = 
            Marshal.OffsetOf(typeof(FileNameInformation), "FileName").ToInt32();

        public int FileNameLength;
        public short FileName;
        // File name string follows (WCHAR).
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FileNamesInformation
    {
        public int NextEntryOffset;
        public int FileIndex;
        public int FileNameLength;
        public short FileName;
        // File name string follows (WCHAR).
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FileNotifyInformation
    {
        public static int FileNameOffset = 
            Marshal.OffsetOf(typeof(FileNotifyInformation), "FileName").ToInt32();

        public int NextEntryOffset;
        public FileNotifyAction Action;
        public int FileNameLength;
        public short FileName;
        // Unicode file name string follows.
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FilePipeInformation
    {
        public PipeType ReadMode;
        public PipeCompletionMode CompletionMode;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FilePipeLocalInformation
    {
        public PipeType NamedPipeType;
        public PipeConfiguration NamedPipeConfiguration;
        public int MaximumInstances;
        public int CurrentInstances;
        public int InboundQuota;
        public int ReadDataAvailable;
        public int OutboundQuota;
        public int WriteQuotaAvailable;
        public PipeState NamedPipeState;
        public PipeEnd NamedPipeEnd;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FilePipePeekBuffer
    {
        public static readonly int DataOffset =
            Marshal.OffsetOf(typeof(FilePipePeekBuffer), "Data").ToInt32();

        public PipeState NamedPipeState;
        public int ReadDataAvailable;
        public int NumberOfMessages;
        public int MessageLength;
        public byte Data;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FilePipeWaitForBuffer
    {
        public static readonly int NameOffset =
            Marshal.OffsetOf(typeof(FilePipeWaitForBuffer), "Name").ToInt32();

        public long Timeout;
        public int NameLength;
        [MarshalAs(UnmanagedType.I1)]
        public bool TimeoutSpecified;
        public short Name;
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
    public struct FileStreamInformation
    {
        public static int StreamNameOffset = 
            Marshal.OffsetOf(typeof(FileStreamInformation), "StreamName").ToInt32();

        public int NextEntryOffset;
        public int StreamNameLength;
        public long StreamSize;
        public long StreamAllocationSize;
        public short StreamName;
        // Stream name string follows (WCHAR).
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
    public struct ImageBaseRelocation
    {
        public int VirtualAddress;
        public int SizeOfBlock;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ImageBoundForwarderRef
    {
        public int TimeDateStamp;
        public short OffsetModuleName;
        public short Reserved;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ImageBoundImportDescriptor
    {
        public int TimeDateStamp;
        public short OffsetModuleName;
        public short NumberOfModuleForwarderRefs;
        public ImageBoundForwarderRef ForwarderRefs;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ImageDataDirectory
    {
        public int VirtualAddress;
        public int Size;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ImageExportDirectory
    {
        public int Characteristics;
        public int TimeDateStamp;
        public short MajorVersion;
        public short MinorVersion;
        public int Name;
        public int Base;
        public int NumberOfFunctions;
        public int NumberOfNames;
        public int AddressOfFunctions;
        public int AddressOfNames;
        public int AddressOfNameOrdinals;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ImageFileHeader
    {
        public MachineType Machine;
        public short NumberOfSections;
        public int TimeDateStamp;
        public int PointerToSymbolTable;
        public int NumberOfSymbols;
        public short SizeOfOptionalHeader;
        public ImageCharacteristics Characteristics;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ImageImportByName
    {
        public short Hint;
        public byte Name;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ImageImportDescriptor
    {
        public int OriginalFirstThunk; // also Characteristics
        public int TimeDateStamp;
        public int ForwarderChain;
        public int Name;
        public int FirstThunk;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ImageLoadConfigDirectory
    {
        public int Size;
        public int TimeDateStamp;
        public short MajorVersion;
        public short MinorVersion;
        public int GlobalFlagsClear;
        public int GlobalFlagsSet;
        public int CriticalSectionDefaultTimeout;
        public int DeCommitFreeBlockThreshold;
        public int DeCommitTotalFreeThreshold;
        public int LockPrefixTable;
        public int MaximumAllocationSize;
        public int VirtualMemoryThreshold;
        public int ProcessHeapFlags;
        public int ProcessAffinityMask;
        public short CsdVersion;
        public short Reserved1;
        public int EditList;
        public int SecurityCookie;
        public int SEHandlerTable;
        public int SEHandlerCount;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ImageLoadConfigDirectory64
    {
        public int Size;
        public int TimeDateStamp;
        public short MajorVersion;
        public short MinorVersion;
        public int GlobalFlagsClear;
        public int GlobalFlagsSet;
        public int CriticalSectionDefaultTimeout;
        public long DeCommitFreeBlockThreshold;
        public long DeCommitTotalFreeThreshold;
        public long LockPrefixTable;
        public long MaximumAllocationSize;
        public long VirtualMemoryThreshold;
        public long ProcessAffinityMask;
        public int ProcessHeapFlags;
        public short CsdVersion;
        public short Reserved1;
        public long EditList;
        public long SecurityCookie;
        public long SEHandlerTable;
        public long SEHandlerCount;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ImageNtHeaders
    {
        public int Signature;
        public ImageFileHeader FileHeader;
        public ImageOptionalHeader OptionalHeader;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ImageOptionalHeader
    {
        public short Magic;
        public byte MajorLinkerVersion;
        public byte MinorLinkerVersion;
        public int SizeOfCode;
        public int SizeOfInitializedData;
        public int SizeOfUninitializedData;
        public int AddressOfEntryPoint;
        public int BaseOfCode;
        public int BaseOfData;
        public int ImageBase;
        public int SectionAlignment;
        public int FileAlignment;
        public short MajorOperatingSystemVersion;
        public short MinorOperatingSystemVersion;
        public short MajorImageVersion;
        public short MinorImageVersion;
        public short MajorSubsystemVersion;
        public short MinorSubsystemVersion;
        public int Win32VersionValue;
        public int SizeOfImage;
        public int SizeOfHeaders;
        public int CheckSum;
        public ImageSubsystem Subsystem;
        public ImageDllCharacteristics DllCharacteristics;
        public int SizeOfStackReserve;
        public int SizeOfStackCommit;
        public int SizeOfHeapReserve;
        public int SizeOfHeapCommit;
        public int LoaderFlags;
        public int NumberOfRvaAndSizes;
        public ImageDataDirectory DataDirectory;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ImageOptionalHeader64
    {
        public short Magic;
        public byte MajorLinkerVersion;
        public byte MinorLinkerVersion;
        public int SizeOfCode;
        public int SizeOfInitializedData;
        public int SizeOfUninitializedData;
        public int AddressOfEntryPoint;
        public int BaseOfCode;
        public long ImageBase;
        public int SectionAlignment;
        public int FileAlignment;
        public short MajorOperatingSystemVersion;
        public short MinorOperatingSystemVersion;
        public short MajorImageVersion;
        public short MinorImageVersion;
        public short MajorSubsystemVersion;
        public short MinorSubsystemVersion;
        public int Win32VersionValue;
        public int SizeOfImage;
        public int SizeOfHeaders;
        public int CheckSum;
        public ImageSubsystem Subsystem;
        public ImageDllCharacteristics DllCharacteristics;
        public long SizeOfStackReserve;
        public long SizeOfStackCommit;
        public long SizeOfHeapReserve;
        public long SizeOfHeapCommit;
        public int LoaderFlags;
        public int NumberOfRvaAndSizes;
        public ImageDataDirectory DataDirectory;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ImageRelocation
    {
        public int VirtualAddress;
        public int SymbolTableIndex;
        public short Type;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct ImageSectionHeader
    {
        public fixed byte Name[8];
        public int Misc; // PhysicalAddress, VirtualSize
        public int VirtualAddress;
        public int SizeOfRawData;
        public int PointerToRawData;
        public int PointerToRelocations;
        public int PointerToLinenumbers;
        public short NumberOfRelocations;
        public short NumberOfLinenumbers;
        public ImageSectionFlags Characteristics;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ImageThunkData
    {
        public int ForwarderString; // byte*
        public int Function; // int*
        public int Ordinal;
        public int AddressOfData; // ImageImportByName*
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ImageThunkData64
    {
        public long ForwarderString; // byte*
        public long Function; // int*
        public long Ordinal;
        public long AddressOfData; // ImageImportByName*
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

    [StructLayout(LayoutKind.Sequential)]
    public struct IoStatusBlock
    {
        public IoStatusBlock(NtStatus status)
            : this(status, IntPtr.Zero)
        { }

        public IoStatusBlock(NtStatus status, IntPtr information)
        {
            this.Pointer = IntPtr.Zero;
            this.Information = information;
            this.Status = status;
        }

        public IoStatusBlock(IntPtr pointer)
            : this(pointer, IntPtr.Zero)
        { }

        public IoStatusBlock(IntPtr pointer, IntPtr information)
        {
            this.Pointer = pointer;
            this.Information = information;
        }

        public IntPtr Pointer;
        public IntPtr Information;

        public unsafe NtStatus Status
        {
            get
            {
                fixed (IoStatusBlock* thisPtr = &this)
                    return *(NtStatus*)&thisPtr->Pointer;
            }
            set
            {
                fixed (IoStatusBlock* thisPtr = &this)
                    *(NtStatus*)&thisPtr->Pointer = value;
            }
        }
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
        public short Name;
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
        public short Name;
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
        public short Class;
        // Variable length string follows.
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct KeyNameInformation
    {
        public int NameLength;
        public short Name;
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
        public short Name;
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
        public short Name;
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
        public short Name;
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

    [StructLayout(LayoutKind.Sequential)]
    public struct KnownAceStruct
    {
        public AceHeader Header;
        public int Mask;
        public int SidStart;
    }

    [StructLayout(LayoutKind.Explicit, Size = 12)]
    public struct KSystemTime
    {
        [FieldOffset(0)]
        public uint LowPart;
        [FieldOffset(4)]
        public int High1Time;
        [FieldOffset(8)]
        public int High2Time;

        [FieldOffset(0)]
        public long QuadPart;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct KtmObjectCursor
    {
        public Guid LastQuery;
        public int ObjectIdCount;
        public byte ObjectIds;
        // Array of Guids follows.
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 4)]
    public struct KUserSharedData
    {
        public static readonly int TickCountOffset = 
            Marshal.OffsetOf(typeof(KUserSharedData), "TickCount").ToInt32();
        public static readonly int TickCountMultiplierOffset =
            Marshal.OffsetOf(typeof(KUserSharedData), "TickCountMultiplier").ToInt32();

        public int TickCountLowDeprecated;
        public int TickCountMultiplier;
        public KSystemTime InterruptTime;
        public KSystemTime SystemTime;
        public KSystemTime TimeZoneBias;
        public ushort ImageNumberLow;
        public ushort ImageNumberHigh;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string NtSystemRoot;

        public int MaxStackTraceDepth;
        public int CryptoExponent;
        public int TimeZoneId;
        public int LargePageMinimum;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
        public int[] Reserved2;

        public WinNtProductType NtProductType;
        [MarshalAs(UnmanagedType.U1)]
        public bool ProductTypeIsValid;

        public int NtMajorVersion;
        public int NtMinorVersion;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Win32.ProcessorFeatureMax)]
        public byte[] ProcessorFeatures;

        public int Reserved1;
        public int Reserved3;
        public int TimeSlip;
        public AlternativeArchitectureType AlternativeArchitecture;
        public int Padding1;
        public long SystemExpirationDate;
        public SuiteType SuiteMask;
        [MarshalAs(UnmanagedType.U1)]
        public bool KdDebuggerEnabled;
        public byte NXSupportPolicy;
        public int ActiveConsoleId;
        public int DismountCount;
        public int ComPlusPackage;
        public int LastSystemRITEventTickCount;
        public int NumberOfPhysicalPages;
        [MarshalAs(UnmanagedType.U1)]
        public bool SafeBootMode;
        public int TraceLogging;
        public int Padding3;

        public long TestRetInstruction;
        public int SystemCall;
        public int SystemCallReturn;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public long[] SystemCallPad;

        public KSystemTime TickCount;

        public int Cookie;
    }

    [StructLayout(LayoutKind.Explicit, Size = 8)]
    public struct LargeInteger
    {
        public static implicit operator long(LargeInteger li)
        {
            return li.QuadPart;
        }

        public LargeInteger(long quadPart)
        {
            this.LowPart = 0;
            this.HighPart = 0;
            this.QuadPart = quadPart;
        }

        [FieldOffset(0)]
        public long QuadPart;
        [FieldOffset(0)]
        public uint LowPart;
        [FieldOffset(4)]
        public int HighPart;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct LdrDataTableEntry
    {
        public static readonly int LoadCountOffset =
            Marshal.OffsetOf(typeof(LdrDataTableEntry), "LoadCount").ToInt32();

        public ListEntry InLoadOrderLinks;
        public ListEntry InMemoryOrderLinks;
        public ListEntry InInitializationOrderLinks;
        public IntPtr DllBase;
        public IntPtr EntryPoint;
        public int SizeOfImage;
        public UnicodeString FullDllName;
        public UnicodeString BaseDllName;
        public LdrpDataTableEntryFlags Flags;
        public short LoadCount;
        public short TlsIndex;
        public ListEntry HashTableEntry;
        public int TimeDateStamp;
        public IntPtr EntryPointActivationContext;
        public IntPtr PatchInformation;
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
    public struct Luid : IEquatable<Luid>, IEquatable<long>
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
                Win32.Throw(status);

            return luid;
        }

        public bool Equals(Luid other)
        {
            return this.QuadPart == other.QuadPart;
        }

        public bool Equals(long other)
        {
            return this.QuadPart == other;
        }

        public override int GetHashCode()
        {
            return this.QuadPart.GetHashCode();
        }

        public long ToLong()
        {
            return this.QuadPart;
        }

        public override string ToString()
        {
            return this.QuadPart.ToString("x");
        }

        public uint ToUInt32()
        {
            return this.LowPart;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct M128A
    {
        public ulong Low;
        public long High;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MessageResourceEntry
    {
        public static readonly int TextOffset = Marshal.OffsetOf(typeof(MessageResourceEntry), "Text").ToInt32();

        public ushort Length;
        public MessageResourceFlags Flags;
        public byte Text;
        // ANSI/Unicode string follows.
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MutantBasicInformation
    {
        public int CurrentCount;
        [MarshalAs(UnmanagedType.U1)]
        public bool OwnedByCaller;
        [MarshalAs(UnmanagedType.U1)]
        public bool AbandonedState;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MutantOwnerInformation
    {
        public ClientId ClientId;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct NtTib
    {
        public IntPtr ExceptionList; // ExceptionRegistrationRecord*
        public IntPtr StackBase;
        public IntPtr StackLimit;
        public IntPtr SubSystemTib;
        public IntPtr FiberData;
        public IntPtr ArbitraryUserPointer;
        public IntPtr Self; // NtTib*
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ObjectAttributes : IDisposable
    {
        public ObjectAttributes(
            string objectName,
            ObjectFlags attributes,
            NativeHandle rootDirectory)
            : this(objectName, attributes, rootDirectory, null, null)
        { }

        public ObjectAttributes(
            string objectName,
            ObjectFlags attributes,
            NativeHandle rootDirectory,
            SecurityDescriptor securityDescriptor,
            SecurityQualityOfService? securityQos
            )
        {
            this.Length = Marshal.SizeOf(typeof(ObjectAttributes));
            this.RootDirectory = IntPtr.Zero;
            this.ObjectName = IntPtr.Zero;
            this.SecurityDescriptor = IntPtr.Zero;
            this.SecurityQualityOfService = IntPtr.Zero;

            // Object name
            if (objectName != null)
            {
                UnicodeString unicodeString = new UnicodeString(objectName);
                IntPtr unicodeStringMemory = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(UnicodeString)));

                Marshal.StructureToPtr(unicodeString, unicodeStringMemory, false);
                this.ObjectName = unicodeStringMemory;
            }

            // Object flags
            this.Attributes = attributes;

            // Root directory
            if (rootDirectory != null)
                this.RootDirectory = rootDirectory;

            // Security descriptor
            this.SecurityDescriptor = securityDescriptor ?? IntPtr.Zero;

            // Security QOS
            if (securityQos.HasValue)
            {
                this.SecurityQualityOfService = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SecurityQualityOfService)));
                Marshal.StructureToPtr(securityQos.Value, this.SecurityQualityOfService, false);
            }
        }

        public int Length;
        public IntPtr RootDirectory;
        public IntPtr ObjectName;
        public ObjectFlags Attributes;
        public IntPtr SecurityDescriptor;
        public IntPtr SecurityQualityOfService;

        public void Dispose()
        {
            // Object name
            if (this.ObjectName != IntPtr.Zero)
            {
                UnicodeString unicodeString =
                    (UnicodeString)Marshal.PtrToStructure(this.ObjectName, typeof(UnicodeString));

                unicodeString.Dispose();
                Marshal.FreeHGlobal(this.ObjectName);

                this.ObjectName = IntPtr.Zero;
            }

            // Security QOS
            if (this.SecurityQualityOfService != null)
            {
                Marshal.FreeHGlobal(this.SecurityQualityOfService);
                this.SecurityQualityOfService = IntPtr.Zero;
            }
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
        public static readonly int ImageSubsystemOffset =
            Marshal.OffsetOf(typeof(Peb), "ImageSubsystem").ToInt32();
        public static readonly int LdrOffset =
            Marshal.OffsetOf(typeof(Peb), "Ldr").ToInt32();
        public static readonly int ProcessHeapOffset = 
            Marshal.OffsetOf(typeof(Peb), "ProcessHeap").ToInt32();
        public static readonly int ProcessParametersOffset = 
            Marshal.OffsetOf(typeof(Peb), "ProcessParameters").ToInt32();

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
        public IntPtr Ldr; // PebLdrData*
        public IntPtr ProcessParameters; // RtlUserProcessParameters*
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
        [MarshalAs(UnmanagedType.I1)]
        public bool Initialized;
        public IntPtr SsHandle;
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
        public static int PrivilegesOffset =
            Marshal.OffsetOf(typeof(PrivilegeSetStruct), "Privileges").ToInt32();

        public int Count;
        public PrivilegeSetFlags Flags;
        public LuidAndAttributes Privileges;
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
    public struct ProcessForegroundBackground
    {
        [MarshalAs(UnmanagedType.I1)]
        public bool Foreground;
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
        public static readonly int HandleTraceOffset =
            Marshal.OffsetOf(typeof(ProcessHandleTracingQuery), "HandleTrace").ToInt32();

        public IntPtr Handle;
        public int TotalTraces;
        public ProcessHandleTracingEntry HandleTrace;
        // An array of ProcessHandleTracingEntry structures follows.
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ProcessPriorityClassStruct
    {
        //the type char is set by the CLR runtime to marshal as Ansi (single byte) for some insane reason...
        public char Foreground;
        public char PriorityClass;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RemotePortView
    {
        public int Length;
        public IntPtr ViewSize;
        public IntPtr ViewBase;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ResourceManagerBasicInformation
    {
        public static readonly int DescriptionOffset =
            Marshal.OffsetOf(typeof(ResourceManagerBasicInformation), "Description").ToInt32();

        /// <summary>
        /// The GUID assigned to the resource manager.
        /// </summary>
        public Guid ResourceManagerId;

        /// <summary>
        /// The length, in bytes, of the resource manager description string.
        /// </summary>
        public int DescriptionLength;

        /// <summary>
        /// The first byte of the description string.
        /// </summary>
        public byte Description; // wchar[]
        // Description string follows.
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RtlAceData
    {
        public AceType AceType;
        public AceFlags InheritFlags;
        public AceFlags AceFlags;
        public int Mask;
        public IntPtr Sid; // Sid**
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RtlBitmap
    {
        public int SizeOfBitMap;
        public IntPtr Buffer; // int*
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RtlBitmapRun
    {
        public int StartingIndex;
        public int NumberOfBits;
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
        public IntPtr Modules; // RtlProcessModules*
        public IntPtr BackTraces; // RtlProcessBackTraces*
        public IntPtr Heaps; // RtlProcessHeaps*
        public IntPtr Locks; // RtlProcessLocks*
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
    public struct RtlHeapInformation
    {
        public IntPtr BaseAddress;
        public int Flags;
        public ushort EntryOverhead;
        public ushort CreatorBackTraceIndex;
        public IntPtr BytesAllocated;
        public IntPtr BytesCommitted;
        public int NumberOfTags;
        public int NumberOfEntries;
        public int NumberOfPseudoTags;
        public int PseudoTagGranularity;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public int[] Reserved;
        public IntPtr Tags;
        public IntPtr Entries;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RtlProcessBackTraceInformation
    {
        public IntPtr SymbolicBackTrace; // PCHAR, always NULL.
        public int TraceCount;
        public ushort Index;
        public ushort Depth;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Win32.MaxStackDepth)]
        public IntPtr[] BackTrace;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RtlProcessBackTraces
    {
        public int CommittedMemory;
        public int ReservedMemory;
        public int NumberOfBackTraceLookups;
        public int NumberOfBackTraces;
        public byte BackTraces; // RtlProcessBackTraceInformation[] BackTraces
        // Array of RtlProcessBackTraceInformation structures follows.
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RtlProcessHeaps
    {
        public static readonly int HeapsOffset =
            Marshal.OffsetOf(typeof(RtlProcessHeaps), "Heaps").ToInt32();

        public int NumberOfHeaps;
        public RtlHeapInformation Heaps;
        // Array of RtlHeapInformation structures follows.
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RtlProcessLockInformation
    {
        public IntPtr Address;
        public RtlLockType Type;
        public ushort CreatorBackTraceInformation;

        public IntPtr OwningThread; // TID
        public int LockCount;
        public int ContentionCount;
        public int EntryCount;

        // Valid for critical sections
        public int RecursionCount;

        // Valid for resources
        public int NumberOfWaitingShared;
        public int NumberOfWaitingExclusive;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RtlProcessLocks
    {
        public int NumberOfLocks;
        // RtlProcessLockInformation[] Locks
        // Array of RtlProcessLockInformation structures follows.
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct RtlProcessModuleInformation
    {
        public IntPtr Section; // empty
        public IntPtr MappedBase;
        public IntPtr ImageBase;
        public int ImageSize;
        public LdrpDataTableEntryFlags Flags;
        public ushort LoadOrderIndex;
        public ushort InitOrderIndex;
        public ushort LoadCount;
        public ushort OffsetToFileName;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public char[] FullPathName;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RtlProcessModules
    {
        public static readonly int ModulesOffset =
            Marshal.OffsetOf(typeof(RtlProcessModules), "Modules").ToInt32();

        public int NumberOfModules;
        public RtlProcessModuleInformation Modules;
        // Array of RtlProcessModuleInformation structures follows.
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
        public static readonly int CurrentDirectoryOffset =
            Marshal.OffsetOf(typeof(RtlUserProcessParameters), "CurrentDirectory").ToInt32();
        public static readonly int DllPathOffset =
            Marshal.OffsetOf(typeof(RtlUserProcessParameters), "DllPath").ToInt32();
        public static readonly int ImagePathNameOffset =
            Marshal.OffsetOf(typeof(RtlUserProcessParameters), "ImagePathName").ToInt32();
        public static readonly int CommandLineOffset =
            Marshal.OffsetOf(typeof(RtlUserProcessParameters), "CommandLine").ToInt32();
        public static readonly int EnvironmentOffset =
            Marshal.OffsetOf(typeof(RtlUserProcessParameters), "Environment").ToInt32();
        public static readonly int WindowTitleOffset =
            Marshal.OffsetOf(typeof(RtlUserProcessParameters), "WindowTitle").ToInt32();
        public static readonly int DesktopInfoOffset =
            Marshal.OffsetOf(typeof(RtlUserProcessParameters), "DesktopInfo").ToInt32();
        public static readonly int ShellInfoOffset =
            Marshal.OffsetOf(typeof(RtlUserProcessParameters), "ShellInfo").ToInt32();
        public static readonly int RuntimeDataOffset =
            Marshal.OffsetOf(typeof(RtlUserProcessParameters), "RuntimeData").ToInt32();
        public static readonly int CurrentDirectoriesOffset =
            Marshal.OffsetOf(typeof(RtlUserProcessParameters), "CurrentDirectories").ToInt32();

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

        public StartupFlags WindowFlags;
        public int ShowWindowFlags;
        public UnicodeString WindowTitle;
        public UnicodeString DesktopInfo;
        public UnicodeString ShellInfo;
        public UnicodeString RuntimeData;

        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        //public RtlDriveLetterCurDir[] CurrentDirectories;
        public RtlDriveLetterCurDir CurrentDirectories;
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
    public struct SecurityDescriptorStruct
    {
        public byte Revision;
        public byte Sbz1;
        public SecurityDescriptorControlFlags Control;
        public IntPtr Owner; // Sid*
        public IntPtr Group; // Sid*
        public IntPtr Sacl; // Acl*
        public IntPtr Dacl; // Acl*
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SecurityDescriptorRelativeStruct
    {
        public byte Revision;
        public byte Sbz1;
        public SecurityDescriptorControlFlags Control;
        public int Owner;
        public int Group;
        public int Sacl;
        public int Dacl;
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
    public struct SystemAlarmAceStruct
    {
        public AceHeader Header;
        public int Mask;
        public int SidStart;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SystemAuditAceStruct
    {
        public AceHeader Header;
        public int Mask;
        public int SidStart;
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
        public IntPtr MinimumUserModeAddress;
        public IntPtr MaximumUserModeAddress;
        public IntPtr ActiveProcessorsAffinityMask;
        public byte NumberOfProcessors;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SystemCacheInformation
    {
        /// <summary>
        /// The size of the system working set, in bytes.
        /// </summary>
        public IntPtr SystemCacheWsSize;
        public IntPtr SystemCacheWsPeakSize;
        public int SystemCacheWsFaults;

        /// <summary>
        /// Measured in pages.
        /// </summary>
        public IntPtr SystemCacheWsMinimum;

        /// <summary>
        /// Measured in pages.
        /// </summary>
        public IntPtr SystemCacheWsMaximum;
        public IntPtr TransitionSharedPages;
        public IntPtr TransitionSharedPagesPeak;
        public int TransitionRePurposeCount;
        public int Flags;
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
    public struct SystemHandleEntry
    {
        public int ProcessId;
        public byte ObjectTypeNumber;
        public HandleFlags Flags;
        public short Handle;
        public IntPtr Object;
        public int GrantedAccess;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SystemHandleInformation
    {
        public static readonly int HandlesOffset =
            Marshal.OffsetOf(typeof(SystemHandleInformation), "Handles").ToInt32();

        public int NumberOfHandles;
        public SystemHandleEntry Handles;
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
        public static readonly int Size = Marshal.SizeOf(typeof(SystemPerformanceInformation));

        /// <summary>
        /// The total idle time of all processors in units of 100-nanoseconds.
        /// </summary>
        public long IdleProcessTime;
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
        public int PageFaultCount;
        /// <summary>
        /// The number of copy-on-write page faults.
        /// </summary>
        public int CopyOnWriteCount;
        /// <summary>
        /// The number of soft page faults.
        /// </summary>
        public int TransitionCount;
        /// <summary>
        /// Something that the Native API reference book doesn't have.
        /// </summary>
        public int CacheTransitionCount;
        /// <summary>
        /// The number of demand zero faults.
        /// </summary>
        public int DemandZeroCount;
        /// <summary>
        /// The number of pages read from disk to resolve page faults.
        /// </summary>
        public int PageReadCount;
        /// <summary>
        /// The number of read operations initiated to resolve page faults.
        /// </summary>
        public int PageReadIoCount;
        public int CacheReadCount;
        public int CacheIoCount;
        /// <summary>
        /// The number of pages written to the system's pagefiles.
        /// </summary>
        public int DirtyPagesWriteCount;
        /// <summary>
        /// The number of write operations performed on the system's pagefiles.
        /// </summary>
        public int DirtyWriteIoCount;
        /// <summary>
        /// The number of pages written to mapped files.
        /// </summary>
        public int MappedPagesWriteCount;
        /// <summary>
        /// The number of write operations performed on mapped files.
        /// </summary>
        public int MappedWriteIoCount;
        /// <summary>
        /// The number of pages used by the paged pool.
        /// </summary>
        public int PagedPoolPages;
        /// <summary>
        /// The number of pages used by the non-paged pool.
        /// </summary>
        public int NonPagedPoolPages;
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
        public int ResidentSystemCodePage;
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
        public int NonPagedPoolLookasideHits;
        /// <summary>
        /// The number of times an allocation could be statisfied by one of the 
        /// small paged lookaside lists.
        /// </summary>
        public int PagedPoolLookasideHits;
        /// <summary>
        /// The number of pages available for use by the paged pool.
        /// </summary>
        public int AvailablePagedPoolPages;
        /// <summary>
        /// The number of pages of the system cache in physical memory.
        /// </summary>
        public int ResidentSystemCachePage;
        /// <summary>
        /// The number of pages of the paged pool in physical memory.
        /// </summary>
        public int ResidentPagedPoolPage;
        /// <summary>
        /// The number of pages of pageable driver code and data in physical memory.
        /// </summary>
        public int ResidentSystemDriverPage;
        /// <summary>
        /// The number of asynchronous fast read operations.
        /// </summary>
        public int CcFastReadNoWait;
        /// <summary>
        /// The number of synchronous fast read operations.
        /// </summary>
        public int CcFastReadWait;
        /// <summary>
        /// The number of fast read operations not possible because of resource 
        /// conflicts.
        /// </summary>
        public int CcFastReadResourceMiss;
        public int CcFastReadNotPossible;
        public int CcFastMdlReadNoWait;
        public int CcFastMdlReadWait;
        public int CcFastMdlReadResourceMiss;
        public int CcFastMdlReadNotPossible;
        public int CcMapDataNoWait;
        public int CcMapDataWait;
        public int CcMapDataNoWaitMiss;
        public int CcMapDataWaitMiss;
        public int CcPinMappedDataCount;
        public int CcPinReadNoWait;
        public int CcPinReadWait;
        public int CcPinReadNoWaitMiss;
        public int CcPinReadWaitMiss;
        public int CcCopyReadNoWait;
        public int CcCopyReadWait;
        public int CcCopyReadNoWaitMiss;
        public int CcCopyReadWaitMiss;
        public int CcMdlReadNoWait;
        public int CcMdlReadWait;
        public int CcMdlReadNoWaitMiss;
        public int CcMdlReadWaitMiss;
        public int CcReadAheadIos;
        public int CcLazyWriteIos;
        public int CcLazyWritePages;
        public int CcDataFlushes;
        public int CcDataPages;
        public int ContextSwitches;
        public int FirstLevelTbFills;
        public int SecondLevelTbFills;
        public int SystemCalls;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SystemProcessInformation
    {
        public int NextEntryOffset;
        public int NumberOfThreads;
        public long SpareLi1;
        public long SpareLi2;
        public long SpareLi3;
        public long CreateTime; // 8
        public long UserTime;
        public long KernelTime;
        public UnicodeString ImageName;
        public int BasePriority;
        private IntPtr _processId;
        private IntPtr _inheritedFromProcessId;
        public int HandleCount;
        public int SessionId;
        public IntPtr PageDirectoryBase;
        public VmCountersEx VirtualMemoryCounters;
        public IoCounters IoCounters;

        public int ProcessId
        {
            get { return _processId.ToInt32(); }
            set { _processId = value.ToIntPtr(); }
        }

        public int InheritedFromProcessId
        {
            get { return _inheritedFromProcessId.ToInt32(); }
            set { _inheritedFromProcessId = value.ToIntPtr(); }
        }
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
    public struct SystemTimeOfDayInformation
    {
        public long BootTime;
        public long CurrentTime;
        public long TimeZoneBias;
        public int TimeZoneId;
        public int Reserved;
        public long BootTimeBias;
        public long SleepTimeBias;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct Teb
    {
        public NtTib NtTib;
        public IntPtr EnvironmentPointer;
        public ClientId ClientId;
        public IntPtr ActiveRpcHandle;
        public IntPtr ThreadLocalStoragePointer;
        public IntPtr ProcessEnvironmentBlock; // Peb*
        public Win32Error LastErrorValue;
        public int CountOfOwnedCriticalSections;
        public IntPtr CsrClientThread;
        public IntPtr Win32ThreadInfo;
        public fixed int User32Reserved[26];
        public fixed int UserReserved[5];
        public IntPtr Wow32Reserved;
        public int CurrentLocale;
        public int FpSoftwareStatusRegister;
        // Variable size part follows
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
    public struct TmBasicInformation
    {
        public Guid TmIdentity;
        public long VirtualClock;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TmLogInformation
    {
        public Guid LogIdentity;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TmLogPathInformation
    {
        public static readonly int LogPathOffset = Marshal.OffsetOf(typeof(TmLogPathInformation), "LogPath").ToInt32();

        /// <summary>
        /// The length, in bytes, of the log path string.
        /// </summary>
        public int LogPathLength;

        /// <summary>
        /// The first byte of the log path string.
        /// </summary>
        public short LogPath; // wchar[]
        // Log path follows.
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TmRecoveryInformation
    {
        public long LastRecoveredLsn;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TokenDefaultDacl
    {
        public TokenDefaultDacl(Acl defaultDacl)
        {
            this.DefaultDacl = defaultDacl ?? IntPtr.Zero;
        }

        public IntPtr DefaultDacl; // Acl*
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TokenGroups
    {
        public static readonly int GroupsOffset =
            Marshal.OffsetOf(typeof(TokenGroups), "Groups").ToInt32();

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
            this.Owner = owner ?? IntPtr.Zero;
        }

        public IntPtr Owner;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TokenPrimaryGroup
    {
        public TokenPrimaryGroup(Sid primaryGroup)
        {
            this.PrimaryGroup = primaryGroup ?? IntPtr.Zero;
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
    public struct TransactionBasicInformation
    {
        public Guid TransactionId;
        public TransactionState State;
        public TransactionOutcome Outcome;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TransactionEnlistmentPair
    {
        public Guid EnlistmentId;
        public Guid ResourceManagerId;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TransactionEnlistmentsInformation
    {
        public int NumberOfEnlistments;
        public byte EnlistmentPair; // TransactionEnlistmentPair[]
        // Array of TransactionEnlistmentPair structures follows.
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TransactionNotification
    {
        public IntPtr TransactionKey;
        public NotificationMask Notification; // Original name: TransactionNotification
        public long TmVirtualClock;
        public int ArgumentLength;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TransactionPropertiesInformation
    {
        public static readonly int DescriptionOffset =
            Marshal.OffsetOf(typeof(TransactionPropertiesInformation), "Description").ToInt32();

        public int IsolationLevel;
        public int IsolationFlags;
        public long Timeout;
        public TransactionOutcome Outcome;

        /// <summary>
        /// The length, in bytes, of the description string.
        /// </summary>
        public int DescriptionLength;

        /// <summary>
        /// The first byte of the description string.
        /// </summary>
        public byte Description; // wchar[]
        // Description string follows.
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct UnicodeString : IComparable<UnicodeString>, IEquatable<UnicodeString>, IDisposable
    {
        public UnicodeString(string str)
        {
            if (str != null)
            {
                UnicodeString newString;

                if (!Win32.RtlCreateUnicodeString(out newString, str))
                    throw new OutOfMemoryException();

                this = newString;
            }
            else
            {
                this.Length = 0;
                this.MaximumLength = 0;
                this.Buffer = IntPtr.Zero;
            }
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
                Win32.Throw(status);

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

        public override int GetHashCode()
        {
            return this.Hash();
        }

        public int Hash(HashStringAlgorithm algorithm, bool caseInsensitive)
        {
            NtStatus status;
            int hash;

            if ((status = Win32.RtlHashUnicodeString(ref this,
                caseInsensitive, algorithm, out hash)) >= NtStatus.Error)
                Win32.Throw(status);

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

        public AnsiString ToAnsiString()
        {
            NtStatus status;
            AnsiString ansiStr = new AnsiString();

            if ((status = Win32.RtlUnicodeStringToAnsiString(ref ansiStr, ref this, true)) >= NtStatus.Error)
                Win32.Throw(status);

            return ansiStr;
        }

        public override string ToString()
        {
            return this.Read();
        }

        public AnsiString ToUpperAnsiString()
        {
            NtStatus status;
            AnsiString ansiStr = new AnsiString();

            if ((status = Win32.RtlUpcaseUnicodeStringToAnsiString(ref ansiStr, ref this, true)) >= NtStatus.Error)
                Win32.Throw(status);

            return ansiStr;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct VmCounters
    {
        public IntPtr PeakVirtualSize;
        public IntPtr VirtualSize;
        public int PageFaultCount;
        public IntPtr PeakWorkingSetSize;
        public IntPtr WorkingSetSize;
        public IntPtr QuotaPeakPagedPoolUsage;
        public IntPtr QuotaPagedPoolUsage;
        public IntPtr QuotaPeakNonPagedPoolUsage;
        public IntPtr QuotaNonPagedPoolUsage;
        public IntPtr PagefileUsage;
        public IntPtr PeakPagefileUsage;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct VmCountersEx
    {
        public IntPtr PeakVirtualSize;
        public IntPtr VirtualSize;
        public int PageFaultCount;
        public IntPtr PeakWorkingSetSize;
        public IntPtr WorkingSetSize;
        public IntPtr QuotaPeakPagedPoolUsage;
        public IntPtr QuotaPagedPoolUsage;
        public IntPtr QuotaPeakNonPagedPoolUsage;
        public IntPtr QuotaNonPagedPoolUsage;
        public IntPtr PagefileUsage;
        public IntPtr PeakPagefileUsage;
        public IntPtr PrivatePageCount;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct VmCountersEx64
    {
        public VmCountersEx64(VmCountersEx vm)
        {
            this.PeakVirtualSize = vm.PeakVirtualSize.ToInt64();
            this.VirtualSize = vm.VirtualSize.ToInt64();
            this.PageFaultCount = vm.PageFaultCount;
            this.PeakWorkingSetSize = vm.PeakWorkingSetSize.ToInt64();
            this.WorkingSetSize = vm.WorkingSetSize.ToInt64();
            this.QuotaPeakPagedPoolUsage = vm.QuotaPeakPagedPoolUsage.ToInt64();
            this.QuotaPagedPoolUsage = vm.QuotaPagedPoolUsage.ToInt64();
            this.QuotaPeakNonPagedPoolUsage = vm.QuotaPeakNonPagedPoolUsage.ToInt64();
            this.QuotaNonPagedPoolUsage = vm.QuotaNonPagedPoolUsage.ToInt64();
            this.PagefileUsage = vm.PagefileUsage.ToInt64();
            this.PeakPagefileUsage = vm.PeakPagefileUsage.ToInt64();
            this.PrivatePageCount = vm.PrivatePageCount.ToInt64();
        }

        public long PeakVirtualSize;
        public long VirtualSize;
        public int PageFaultCount;
        public long PeakWorkingSetSize;
        public long WorkingSetSize;
        public long QuotaPeakPagedPoolUsage;
        public long QuotaPagedPoolUsage;
        public long QuotaPeakNonPagedPoolUsage;
        public long QuotaNonPagedPoolUsage;
        public long PagefileUsage;
        public long PeakPagefileUsage;
        public long PrivatePageCount;

        public VmCountersEx ToVmCountersEx()
        {
            return new VmCountersEx()
            {
                PeakVirtualSize = this.PeakVirtualSize.ToIntPtr(),
                VirtualSize = this.VirtualSize.ToIntPtr(),
                PageFaultCount = this.PageFaultCount,
                PeakWorkingSetSize = this.PeakWorkingSetSize.ToIntPtr(),
                WorkingSetSize = this.WorkingSetSize.ToIntPtr(),
                QuotaPeakPagedPoolUsage = this.QuotaPeakPagedPoolUsage.ToIntPtr(),
                QuotaPagedPoolUsage = this.QuotaPagedPoolUsage.ToIntPtr(),
                QuotaPeakNonPagedPoolUsage = this.QuotaPeakNonPagedPoolUsage.ToIntPtr(),
                QuotaNonPagedPoolUsage = this.QuotaNonPagedPoolUsage.ToIntPtr(),
                PagefileUsage = this.PagefileUsage.ToIntPtr(),
                PeakPagefileUsage = this.PeakPagefileUsage.ToIntPtr(),
                PrivatePageCount = this.PrivatePageCount.ToIntPtr()
            };
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct XmmSaveArea32
    {
        public ushort ControlWord;
        public ushort StatusWord;
        public byte TagWord;
        public byte Reserved1;
        public ushort ErrorOpcode;
        public int ErrorOffset;
        public ushort ErrorSelector;
        public ushort Reserved2;
        public int DataOffset;
        public ushort DataSelector;
        public ushort Reserved3;
        public int MxCsr;
        public int MxCsrMask;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public M128A[] FloatRegisters;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public M128A[] XmmRegisters;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 96)]
        public byte[] Reserved4;
    }
}
