/*
 * Process Hacker - 
 *   native API enumerations
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

/* This file contains enumeration declarations for the Native API.
 * Enumerations shared between the Native API and Win32 are placed 
 * in this file.
 */

using System;
using System.Collections.Generic;

namespace ProcessHacker.Native.Api
{
    [Flags]
    public enum AceFlags : byte
    {
        ObjectInherit = 0x1,
        ContainerInherit = 0x2,
        NoPropagateInherit = 0x4,
        InheritOnly = 0x8,
        Inherited = 0x10,
        Valid = 0x1f,

        // For SystemAudit and SystemAlarm ACEs.
        SuccessfulAccess = 0x40,
        FailedAccess = 0x80
    }

    public enum AceType : byte
    {
        //Mininum = 0x0,
        AccessAllowed = 0x0,
        AccessDenied = 0x1,
        SystemAudit = 0x2,
        SystemAlarm = 0x3,
        //MaximumV2 = 0x3,

        AccessAllowedCompound = 0x4,
        //MaximumV3 = 0x4,

        //MinimumObject = 0x5,
        AccessAllowedObject = 0x5,
        AccessDeniedObject = 0x6,
        SystemAuditObject = 0x7,
        SystemAlarmObject = 0x8,
        //MaximumObject = 0x8,
        //MaximumV4 = 0x8,
        //Maximum = 0x8,

        AccessAllowedCallback = 0x9,
        AccessDeniedCallback = 0xa,
        AccessAllowedCallbackObject = 0xb,
        AccessDeniedCallbackObject = 0xc,
        SystemAuditCallback = 0xd,
        SystemAlarmCallback = 0xe,
        SystemAuditCallbackObject = 0xf,
        SystemAlarmCallbackObject = 0x10,
        //MaximumV5 = 0x10
    }

    public enum AclInformationClass : int
    {
        AclRevisionInformation = 1,
        AclSizeInformation
    }

    public enum AlternativeArchitectureType : int
    {
        StandardDesign,
        Nec98x86,
        EndAlternatives
    }

    public enum BaseSrvApiNumber : int
    {
        BasepCreateProcess = Win32.BaseSrvFirstApiNumber,
        BasepCreateThread,
        BasepGetTempFile,
        BasepExitProcess,
        BasepDebugProcess,
        BasepCheckVDM,
        BasepUpdateVDMEntry,
        BasepGetNextVDMCommand,
        BasepExitVDM,
        BasepIsFirstVDM,
        BasepGetVDMExitCode,
        BasepSetReenterCount,
        BasepSetProcessShutdownParam,
        BasepGetProcessShutdownParam,
        BasepNlsSetUserInfo,
        BasepNlsSetMultipleUserInfo,
        BasepNlsCreateSortSection,
        BasepNlsPreserveSection,
        BasepSetVDMCurDirs,
        BasepGetVDMCurDirs,
        BasepBatNotification,
        BasepRegisterWowExec,
        BasepSoundSentryNotification,
        BasepRefreshIniFileMapping,
        BasepDefineDosDevice,
        BasepMaxApiNumber
    }

    public enum CompoundAceType : ushort
    {
        Impersonation = 1
    }

    /// <summary>
    /// Generic context-related flags.
    /// </summary>
    [Flags]
    public enum ContextFlagsGeneric : uint
    {
        // Context architecture
        I386 = 0x00010000,
        I486 = 0x00010000,
        Amd64 = 0x00100000,

        // Context flags
        Control = 0x00000001,
        Integer = 0x00000002,
        Segments = 0x00000004,
        FloatingPoint = 0x00000008,
        DebugRegisters = 0x00000010,
        ExtendedRegisters = 0x00000020,
    }

    /// <summary>
    /// x86 context.
    /// </summary>
    [Flags]
    public enum ContextFlags : uint
    {
        I386 = ContextFlagsGeneric.I386,
        I486 = ContextFlagsGeneric.I486,

        Control = I386 | ContextFlagsGeneric.Control,
        Integer = I386 | ContextFlagsGeneric.Integer,
        Segments = I386 | ContextFlagsGeneric.Segments,
        FloatingPoint = I386 | ContextFlagsGeneric.FloatingPoint,
        DebugRegisters = I386 | ContextFlagsGeneric.DebugRegisters,
        ExtendedRegisters = I386 | ContextFlagsGeneric.ExtendedRegisters,

        Full = Control | Integer | Segments,
        All = Control | Integer | Segments | FloatingPoint | DebugRegisters | ExtendedRegisters
    }

    /// <summary>
    /// AMD64 context.
    /// </summary>
    [Flags]
    public enum ContextFlagsAmd64 : uint
    {
        Amd64 = ContextFlagsGeneric.Amd64,

        Control = Amd64 | ContextFlagsGeneric.Control,
        Integer = Amd64 | ContextFlagsGeneric.Integer,
        Segments = Amd64 | ContextFlagsGeneric.Segments,
        FloatingPoint = Amd64 | ContextFlagsGeneric.FloatingPoint,
        DebugRegisters = Amd64 | ContextFlagsGeneric.DebugRegisters,

        Full = Control | Integer | FloatingPoint,
        All = Control | Integer | Segments | FloatingPoint | DebugRegisters,

        ExceptionActive = 0x08000000,
        ServiceActive = 0x10000000,
        ExceptionRequest = 0x40000000,
        ExceptionReporting = 0x80000000
    }

    [Flags]
    public enum CrmProtocolOptions : int
    {
        ExplicitMarshalOnly = 0x1,
        DynamicMarshalInfo = 0x2,
        MaximumOption = 0x3
    }

    [Flags]
    public enum DebugObjectFlags : uint
    {
        KillOnClose = 0x1
    }

    [Flags]
    public enum DebugObjectInformationClass : int
    {
        DebugObjectFlags,
        MaxDebugObjectInfoClass
    }

    public enum DeviceControlAccess : int
    {
        Any = 0,
        Special = Any,
        Read = 1,
        Write = 2
    }

    public enum DeviceControlMethod : int
    {
        Buffered = 0,
        InDirect = 1,
        OutDirect = 2,
        Neither = 3
    }

    public enum DeviceType : int
    {
        Beep = 0x1,
        CdRom = 0x2,
        CdRomFileSystem = 0x3,
        Controller = 0x4,
        DataLink = 0x5,
        Dfs = 0x6,
        Disk = 0x7,
        DiskFileSystem = 0x8,
        FileSystem = 0x9,
        InportPort = 0xa,
        Keyboard = 0xb,
        Mailslot = 0xc,
        MidiIn = 0xd,
        MidiOut = 0xe,
        Mouse = 0xf,
        MultiUncProvider = 0x10,
        NamedPipe = 0x11,
        Network = 0x12,
        NetworkBrowser = 0x13,
        NetworkFileSystem = 0x14,
        Null = 0x15,
        ParallelPort = 0x16,
        PhysicalNetCard = 0x17,
        Printer = 0x18,
        Scanner = 0x19,
        SerialMousePort = 0x1a,
        SerialPort = 0x1b,
        Screen = 0x1c,
        Sound = 0x1d,
        Streams = 0x1e,
        Tape = 0x1f,
        TapeFileSystem = 0x20,
        Transport = 0x21,
        Unknown = 0x22,
        Video = 0x23,
        VirtualDisk = 0x24,
        WaveIn = 0x25,
        WaveOut = 0x26,
        EightZeroFourTwoPort = 0x27,
        NetworkRedirector = 0x28,
        Battery = 0x29,
        BusExtender = 0x2a,
        Modem = 0x2b,
        Vdm = 0x2c,
        MassStorage = 0x2d,
        Smb = 0x2e,
        Ks = 0x2f,
        Changer = 0x30,
        SmartCard = 0x31,
        Acpi = 0x32,
        Dvd = 0x33,
        FullscreenVideo = 0x34,
        DfsFileSystem = 0x35,
        DfsVolume = 0x36,
        Serenum = 0x37,
        TermSrv = 0x38,
        KSec = 0x39,
        Fips = 0x3a,
        Infiniband = 0x3b,

        MountMgr = 'm',
        MountMgrDevice = 'M'
    }

    [Flags]
    public enum DbgState : int
    {
        DbgIdle,
        DbgReplyPending,
        DbgCreateThreadStateChange,
        DbgCreateProcessStateChange,
        DbgExitThreadStateChange,
        DbgExitProcessStateChange,
        DbgExceptionStateChange,
        DbgBreakpointStateChange,
        DbgSingleStepStateChange,
        DbgLoadDllStateChange,
        DbgUnloadDllStateChange
    }

    [Flags]
    public enum DuplicateOptions : int
    {
        CloseSource = 0x1,
        SameAccess = 0x2,
        SameAttributes = 0x4
    }

    public enum EnlistmentInformationClass : int
    {
        EnlistmentBasicInformation,
        EnlistmentRecoveryInformation,
        EnlistmentFullInformation
    }

    [Flags]
    public enum EnlistmentOptions : int
    {
        Superior = 0x1,
        MaximumOption = 0x1
    }

    public enum EventInformationClass : int
    {
        EventBasicInformation
    }

    public enum EventType : int
    {
        NotificationEvent,
        SynchronizationEvent
    }

    public enum FileAlignment : int
    {
        Byte = 0x0,
        Word = 0x1,
        Long = 0x3,
        Quad = 0x7,
        Octa = 0xf,
        ThirtyTwoByte = 0x1f,
        SixtyFourByte = 0x3f,
        OneHundredAndTwentyEightByte = 0x7f,
        TwoHundredAndFiftySixByte = 0xff,
        FiveHundredAndTwelveByte = 0x1ff
    }

    [Flags]
    public enum FileAttributes : uint
    {
        ReadOnly = 0x1,
        Hidden = 0x2,
        System = 0x4,

        Directory = 0x10,
        Archive = 0x20,
        Device = 0x40,
        Normal = 0x80,

        Temporary = 0x100,
        SparseFile = 0x200,
        ReparsePoint = 0x400,
        Compressed = 0x800,

        Offline = 0x1000,
        NotContextIndexed = 0x2000,
        Encrypted = 0x4000
    }

    [Flags]
    public enum FileCreateOptions : uint
    {
        DirectoryFile = 0x1,
        WriteThrough = 0x2,
        SequentialOnly = 0x4,
        NoIntermediateBuffering = 0x8,

        SynchronousIoAlert = 0x10,
        SynchronousIoNonAlert = 0x20,
        NonDirectoryFile = 0x40,
        CreateTreeConnection = 0x80,

        CompleteIfOpLocked = 0x100,
        NoEaKnowledge = 0x200,
        OpenForRecovery = 0x400,
        RandomAccess = 0x800,

        DeleteOnClose = 0x1000,
        OpenByFileId = 0x2000,
        OpenForBackupIntent = 0x4000,
        NoCompression = 0x8000,

        ReserveOpFilter = 0x100000,
        OpenReparsePoint = 0x200000,
        OpenNoRecall = 0x400000,
        OpenForFreeSpaceQuery = 0x800000,

        CopyStructuredStorage = 0x41,
        StructuredStorage = 0x441,

        ValidOptionFlags = 0xffffff,
        ValidPipeOptionFlags = 0x32,
        ValidMailslotOptionFlags = 0x32,
        ValidSetFlags = 0x36
    }

    public enum FileCreationDisposition : int
    {
        Supersede = 0x0,
        Open = 0x1,
        Create = 0x2,
        OpenIf = 0x3,
        Overwrite = 0x4,
        OverwriteIf = 0x5
    }

    public enum FileInformationClass : int
    {
        FileDirectoryInformation = 1, // dir
        FileFullDirectoryInformation, // dir
        FileBothDirectoryInformation, // dir
        FileBasicInformation,
        FileStandardInformation,
        FileInternalInformation,
        FileEaInformation,
        FileAccessInformation,
        FileNameInformation,
        FileRenameInformation, // 10
        FileLinkInformation,
        FileNamesInformation, // dir
        FileDispositionInformation,
        FilePositionInformation,
        FileFullEaInformation,
        FileModeInformation,
        FileAlignmentInformation,
        FileAllInformation,
        FileAllocationInformation,
        FileEndOfFileInformation, // 20
        FileAlternateNameInformation,
        FileStreamInformation,
        FilePipeInformation,
        FilePipeLocalInformation,
        FilePipeRemoteInformation,
        FileMailslotQueryInformation,
        FileMailslotSetInformation,
        FileCompressionInformation,
        FileObjectIdInformation, // dir
        FileCompletionInformation, // 30
        FileMoveClusterInformation,
        FileQuotaInformation,
        FileReparsePointInformation,
        FileNetworkOpenInformation,
        FileAttributeTagInformation,
        FileTrackingInformation,
        FileIdBothDirectoryInformation, // dir
        FileIdFullDirectoryInformation, // dir
        FileValidDataLengthInformation,
        FileShortNameInformation, // 40
        FileIoCompletionNotificationInformation,
        FileIoStatusBlockRangeInformation,
        FileIoPriorityHintInformation,
        FileSfioReserveInformation,
        FileSfioVolumeInformation, 
        FileHardLinkInformation,
        FileProcessIdsUsingFileInformation,
        FileNormalizedNameInformation,
        FileNetworkPhysicalNameInformation,
        FileIdGlobalTxDirectoryInformation, // 50
        FileMaximumInformation
    }

    public enum FileIoStatus : int
    {
        Superseded = 0,
        Opened = 1,
        Created = 2,
        Overwritten = 3,
        Exists = 4,
        DoesNotExist = 5
    }

    public enum FileNotifyAction : int
    {
        Added = 0x1,
        Removed = 0x2,
        Modified = 0x3,
        RenamedOldName = 0x4,
        RenamedNewName = 0x5,
        AddedStream = 0x6,
        RemovedStream = 0x7,
        ModifiedStream = 0x8,
        RemovedByDelete = 0x9,
        IdNotTunnelled = 0xa,
        TunnelledIdCollision = 0xb
    }

    [Flags]
    public enum FileNotifyFlags : int
    {
        FileName = 0x1,
        DirName = 0x2,
        Name = 0x3,
        Attributes = 0x4,
        Size = 0x8,
        LastWrite = 0x10,
        LastAccess = 0x20,
        Creation = 0x40,
        Ea = 0x80,
        Security = 0x100,
        StreamName = 0x200,
        StreamSize = 0x400,
        StreamWrite = 0x800,
        Valid = 0xfff
    }

    [Flags]
    public enum FileObjectFlags : int
    {
        FileOpen = 0x00000001,
        SynchronousIo = 0x00000002,
        AlertableIo = 0x00000004,
        NoIntermediateBuffering = 0x00000008,
        WriteThrough = 0x00000010,
        SequentialOnly = 0x00000020,
        CacheSupported = 0x00000040,
        NamedPipe = 0x00000080,
        StreamFile = 0x00000100,
        MailSlot = 0x00000200,
        GenerateAuditOnClose = 0x00000400,
        QueueIrpToThread = GenerateAuditOnClose,
        DirectDeviceOpen = 0x00000800,
        FileModified = 0x00001000,
        FileSizeChanged = 0x00002000,
        CleanupComplete = 0x00004000,
        TemporaryFile = 0x00008000,
        DeleteOnClose = 0x00010000,
        OpenedCaseSensitivity = 0x00020000,
        HandleCreated = 0x00040000,
        FileFastIoRead = 0x00080000,
        RandomAccess = 0x00100000,
        FileOpenCancelled = 0x00200000,
        VolumeOpen = 0x00400000,
        RemoteOrigin = 0x01000000,
        SkipCompletionPort = 0x02000000,
        SkipSetEvent = 0x04000000,
        SkipSetFastIo = 0x08000000
    }

    [Flags]
    public enum FileShareMode : uint
    {
        Exclusive = 0x0,
        Read = 0x1,
        Write = 0x2,
        Delete = 0x4,

        ReadWrite = Read | Write,
        ReadWriteDelete = Read | Write | Delete
    }

    public enum FsInformationClass : int
    {
        FileFsVolumeInformation = 1,
        FileFsLabelInformation,
        FileFsSizeInformation,
        FileFsDeviceInformation,
        FileFsAttributeInformation, 
        FileFsControlInformation,
        FileFsFullSizeInformation,
        FileFsObjectIdInformation,
        FileFsDriverPathInformation,
        FileFsVolumeFlagsInformation, // 10
        FileFsMaximumInformation
    }

    [Flags]
    public enum HandleFlags : byte
    {
        ProtectFromClose = 0x1,
        Inherit = 0x2,
        AuditObjectClose = 0x4
    }

    public enum HandleTraceType : int
    {
        Open = 1,
        Close = 2,
        BadRef = 3
    }

    [Flags]
    public enum HashStringAlgorithm : int
    {
        Default = 0,
        X65599 = 1,
        Invalid = -1
    }

    [Flags]
    public enum HeapFlags : uint
    {
        NoSerialize = 0x00000001,
        Growable = 0x00000002,
        GenerateExceptions = 0x00000004,
        ZeroMemory = 0x00000008,
        ReallocInPlaceOnly = 0x00000010,
        TailCheckingEnabled = 0x00000020,
        FreeCheckingEnabled = 0x00000040,
        DisableCoalesceOnFree = 0x00000080,

        CreateAlign16 = 0x00010000,
        CreateEnableTracing = 0x00020000,
        CreateEnableExecute = 0x00040000,

        SettableUserValue = 0x00000100,
        SettableUserFlag1 = 0x00000200,
        SettableUserFlag2 = 0x00000400,
        SettableUserFlag3 = 0x00000800,
        SettableUserFlags = 0x00000e00,

        Class0 = 0x00000000, // Process heap
        Class1 = 0x00001000, // Private heap
        Class2 = 0x00002000, // Kernel heap
        Class3 = 0x00003000, // GDI heap
        Class4 = 0x00004000, // User heap
        Class5 = 0x00005000, // Console heap
        Class6 = 0x00006000, // User desktop heap
        Class7 = 0x00007000, // CSRSS shared heap
        Class8 = 0x00008000, // CSR port heap
        ClassMask = 0x0000f000
    }

    public enum ImageBaseRelocationType : short
    {
        /// <summary>
        /// The base relocation is skipped. This type can be used to pad a block.
        /// </summary>
        Absolute = 0,

        /// <summary>
        /// The base relocation adds the high 16 bits of the difference to the 16-bit 
        /// field at offset. The 16-bit field represents the high value of a 32-bit word.
        /// </summary>
        High = 1,

        /// <summary>
        /// The base relocation adds the low 16 bits of the difference to the 16-bit 
        /// field at offset. The 16-bit field represents the low half of a 32-bit word. 
        /// </summary>
        Low = 2,

        /// <summary>
        /// The base relocation applies all 32 bits of the difference to the 32-bit 
        /// field at offset.
        /// </summary>
        HighLow = 3,

        /// <summary>
        /// The base relocation adds the high 16 bits of the difference to the 16-bit 
        /// field at offset. The 16-bit field represents the high value of a 32-bit word. 
        /// The low 16 bits of the 32-bit value are stored in the 16-bit word that follows 
        /// this base relocation. This means that this base relocation occupies two slots.
        /// </summary>
        HighAdj = 4,

        /// <summary>
        /// The base relocation applies to a MIPS jump instruction.
        /// </summary>
        MipsJmpAddr = 5,

        /// <summary>
        /// The base relocation applies to a MIPS16 jump instruction.
        /// </summary>
        MipsJmpAddr16 = 9,
        Ia64Imm64 = 9,

        /// <summary>
        /// The base relocation applies the difference to the 64-bit field at offset.
        /// </summary>
        Dir16 = 10
    }

    [Flags]
    public enum ImageCharacteristics : ushort
    {
        /// <summary>
        /// Image only, Windows CE, and Windows NT® and later. This indicates that the file does 
        /// not contain base relocations and must therefore be loaded at its preferred base address. 
        /// If the base address is not available, the loader reports an error. The default behavior 
        /// of the linker is to strip base relocations from executable (EXE) files.
        /// </summary>
        RelocsStripped = 0x0001,

        /// <summary>
        /// Image only. This indicates that the image file is valid and can be run. If this flag 
        /// is not set, it indicates a linker error.
        /// </summary>
        ExecutableImage = 0x0002,

        /// <summary>
        /// COFF line numbers have been removed. This flag is deprecated and should be zero.
        /// </summary>
        LineNumsStripped = 0x0004,

        /// <summary>
        /// COFF symbol table entries for local symbols have been removed. This flag is deprecated 
        /// and should be zero.
        /// </summary>
        LocalSymsStripped = 0x0008,

        /// <summary>
        /// Obsolete. Aggressively trim working set. This flag is deprecated for Windows 2000 and later 
        /// and must be zero.
        /// </summary>
        AggressiveWsTrim = 0x0010,

        /// <summary>
        /// Application can handle > 2 GB addresses.
        /// </summary>
        LargeAddressAware = 0x0020,

        /// <summary>
        /// This flag is reserved for future use.
        /// </summary>
        Reserved = 0x0040,

        /// <summary>
        /// Little endian: the least significant bit (LSB) precedes the most significant bit (MSB) in 
        /// memory. This flag is deprecated and should be zero.
        /// </summary>
        BytesReversedLo = 0x0080,

        /// <summary>
        /// Machine is based on a 32-bit-word architecture.
        /// </summary>
        ThirtyTwoBitMachine = 0x0100,

        /// <summary>
        /// Debugging information is removed from the image file.
        /// </summary>
        DebugStripped = 0x0200,

        /// <summary>
        /// If the image is on removable media, fully load it and copy it to the swap file.
        /// </summary>
        RemovableRunFromSwap = 0x0400,

        /// <summary>
        /// If the image is on network media, fully load it and copy it to the swap file.
        /// </summary>
        NetRunFromSwap = 0x0800,

        /// <summary>
        /// The image file is a system file, not a user program.
        /// </summary>
        System = 0x1000,

        /// <summary>
        /// The image file is a dynamic-link library (DLL). Such files are considered 
        /// executable files for almost all purposes, although they cannot be directly run.
        /// </summary>
        DLL = 0x2000,

        /// <summary>
        /// The file should be run only on a uniprocessor machine.
        /// </summary>
        UPSystemOnly = 0x4000,

        /// <summary>
        /// Big endian: the MSB precedes the LSB in memory. This flag is deprecated and should be zero.
        /// </summary>
        BytesReversedHi = 0x8000
    }

    [Flags]
    public enum ImageDllCharacteristics : ushort
    {
        DynamicBase = 0x0040,
        ForceIntegrity = 0x0080,
        NxCompat = 0x0100,
        NoIsolation = 0x0200,
        NoSeh = 0x0400,
        NoBind = 0x0800,
        WdmDriver = 0x2000,
        TerminalServerAware = 0x8000
    }

    public enum ImageI386RelocationType : short
    {
        Absolute = 0x0,
        Dir16 = 0x1,
        Rel16 = 0x2,
        Dir32 = 0x6,
        Dir32Nb = 0x7,
        Seg12 = 0x9,
        Section = 0xa,
        SecRel = 0xb,
        Token = 0xc,
        SecRel7 = 0xd,
        Rel32 = 0x14
    }

    [Flags]
    public enum ImageSectionFlags : uint
    {
        /// <summary>
        /// Reserved for future use.
        /// </summary>
        Reserved1 = 0x00000000,

        /// <summary>
        /// Reserved for future use.
        /// </summary>
        Reserved2 = 0x00000001,

        /// <summary>
        /// Reserved for future use.
        /// </summary>
        Reserved3 = 0x00000002,

        /// <summary>
        /// Reserved, must be zero.
        /// </summary>
        Reserved4 = 0x00000004,

        /// <summary>
        /// The section should not be padded to the next boundary. 
        /// This flag is obsolete and is replaced by IMAGE_SCN_ALIGN_1BYTES. 
        /// This is valid only for object files.
        /// </summary>
        NoPad = 0x00000008,

        /// <summary>
        /// The section contains executable code.
        /// </summary>
        Code = 0x00000020,

        /// <summary>
        /// The section contains initialized data.
        /// </summary>
        InitializedData = 0x00000040,

        /// <summary>
        /// The section contains uninitialized data.
        /// </summary>
        UninitializedData = 0x00000080,

        /// <summary>
        /// Reserved for future use.
        /// </summary>
        Other = 0x00000100,

        /// <summary>
        /// The section contains comments or other information. The 
        /// .drectve section has this type. This is valid for object 
        /// files only.
        /// </summary>
        Info = 0x00000200,

        /// <summary>
        /// Reserved for future use.
        /// </summary>
        Reserved5 = 0x00000400,

        /// <summary>
        /// The section will not become part of the image. This is valid 
        /// only for object files.
        /// </summary>
        Remove = 0x00000800,

        /// <summary>
        /// The section contains COMDAT data. 
        /// </summary>
        COMDAT = 0x00001000,

        /// <summary>
        /// The section contains data referenced through the global pointer (GP).
        /// </summary>
        GPRel = 0x00008000,

        /// <summary>
        /// Reserved for future use.
        /// </summary>
        MemoryPurgeable = 0x00010000,

        /// <summary>
        /// Reserved for future use.
        /// </summary>
        Memory16Bit = 0x00020000,

        /// <summary>
        /// Reserved for future use.
        /// </summary>
        MemoryLocked = 0x00040000,

        /// <summary>
        /// Reserved for future use.
        /// </summary>
        MemoryPeload = 0x00080000,

        Align1Bytes = 0x00100000,
        Align2Bytes = 0x00200000,
        Align4Bytes = 0x00300000,
        Align8Bytes = 0x00400000,
        Align16Bytes = 0x00500000,
        Align32Bytes = 0x00600000,
        Align64Bytes = 0x00700000,
        Align128Bytes = 0x00800000,
        Align256Bytes = 0x00900000,
        Align512Bytes = 0x00a00000,
        Align1024Bytes = 0x00b00000,
        Align2048Bytes = 0x00c00000,
        Align4096Bytes = 0x00d00000,
        Align8192Bytes = 0x00e00000,

        /// <summary>
        /// The section contains extended relocations.
        /// </summary>
        NRelocOvfl = 0x01000000,

        /// <summary>
        /// The section can be discarded as needed.
        /// </summary>
        MemoryDiscardable = 0x02000000,

        /// <summary>
        /// The section cannot be cached.
        /// </summary>
        MemoryNotCached = 0x04000000,

        /// <summary>
        /// The section is not pageable.
        /// </summary>
        MemoryNotPaged = 0x08000000,

        /// <summary>
        /// The section can be shared in memory.
        /// </summary>
        MemoryShared = 0x10000000,

        /// <summary>
        /// The section can be executed as code.
        /// </summary>
        MemoryExecute = 0x20000000,

        /// <summary>
        /// The section can be read.
        /// </summary>
        MemoryRead = 0x40000000,

        /// <summary>
        /// The section can be written to.
        /// </summary>
        MemoryWrite = 0x80000000
    }

    public enum ImageSubsystem : short
    {
        Unknown = 0,
        Native = 1,
        WindowsGui = 2,
        WindowsCui = 3,
        OS2Cui = 5,
        PosixCui = 7,
        NativeWindows = 8,
        WindowsCeGui = 9,
        EfiApplication = 10,
        EfiBootServiceDriver = 11,
        EfiRuntimeDriver = 12,
        EfiRom = 13,
        Xbox = 14,
        WindowsBootApplication = 16
    }

    public enum IoCompletionInformationClass : int
    {
        IoCompletionBasicInformation
    }

    public enum IoReparseTag : uint
    {
        ReservedZero = 0x0,
        ReservedOne = 0x1,

        MountPoint = 0xa0000003,
        Hsm = 0xc0000004,
        DriveExtender = 0x80000005,
        Hsm2 = 0x80000006,
        Sis = 0x80000007,
        Wim = 0x80000008,
        Csv = 0x80000009,
        Dfs = 0x8000000a,
        FilterManager = 0x8000000b,
        SymLink = 0xa000000c,
        IisCache = 0xa0000010,
        Dfsr = 0x80000012
    }

    [Flags]
    public enum JobObjectBasicUiRestrictions : uint
    {
        Handles = 0x1,
        ReadClipboard = 0x2,
        WriteClipboard = 0x4,
        SystemParameters = 0x8,
        DisplaySettings = 0x10,
        GlobalAtoms = 0x20,
        Desktop = 0x40,
        ExitWindows = 0x80
    }

    public enum JobObjectInformationClass : int
    {
        JobObjectBasicAccountingInformation = 1,
        JobObjectBasicLimitInformation,
        JobObjectBasicProcessIdList,
        JobObjectBasicUIRestrictions,
        JobObjectSecurityLimitInformation,
        JobObjectEndOfJobTimeInformation,
        JobObjectAssociateCompletionPortInformation,
        JobObjectBasicAndIoAccountingInformation,
        JobObjectExtendedLimitInformation,
        JobObjectJobSetInformation
    }

    [Flags]
    public enum JobObjectLimitFlags : uint
    {
        WorkingSet = 0x1,
        ProcessTime = 0x2,
        JobTime = 0x4,
        ActiveProcess = 0x8,
        Affinity = 0x10,
        PriorityClass = 0x20,
        PreserveJobTime = 0x40,
        SchedulingClass = 0x80,
        ProcessMemory = 0x100,
        JobMemory = 0x200,
        DieOnUnhandledException = 0x400,
        BreakawayOk = 0x800,
        SilentBreakawayOk = 0x1000,
        KillOnJobClose = 0x2000,
    }

    [Flags]
    public enum KeyCreationDisposition : int
    {
        CreatedNewKey = 0x1,
        OpenedExistingKey = 0x2
    }

    public enum KeyInformationClass : int
    {
        KeyBasicInformation,
        KeyNodeInformation,
        KeyFullInformation,
        KeyNameInformation,
        KeyCachedInformation,
        KeyFlagsInformation,
        MaxKeyInfoClass
    }

    public enum KeySetInformationClass : int
    {
        KeyWriteTimeInformation,
        KeyUserFlagsInformation,
        MaxKeySetInfoClass
    }

    public enum KProcessorMode : byte
    {
        KernelMode = 0,
        UserMode = 1
    }

    public enum KProfileSource : int
    {
        ProfileTime,
        ProfileAlignmentFixup,
        ProfileTotalIssues,
        ProfilePipelineDry,
        ProfileLoadInstructions,
        ProfilePipelineFrozen,
        ProfileBranchInstructions,
        ProfileTotalNonissues,
        ProfileDcacheMisses,
        ProfileIcacheMisses,
        ProfileCacheMisses,
        ProfileBranchMispredictions,
        ProfileStoreInstructions,
        ProfileFpInstructions,
        ProfileIntegerInstructions,
        Profile2Issue,
        Profile3Issue,
        Profile4Issue,
        ProfileSpecialInstructions,
        ProfileTotalCycles,
        ProfileIcacheIssues,
        ProfileDcacheAccesses,
        ProfileMemoryBarrierCycles,
        ProfileLoadLinkedIssues,
        ProfileMaximum
    }

    public enum KtmObjectType : int
    {
        Transaction,
        TransactionManager,
        ResourceManager,
        Enlistment,
        Invalid
    }

    public enum KWaitReason : int
    {
        Executive = 0,
        FreePage = 1,
        PageIn = 2,
        PoolAllocation = 3,
        DelayExecution = 4,
        Suspended = 5,
        UserRequest = 6,
        WrExecutive = 7,
        WrFreePage = 8,
        WrPageIn = 9,
        WrPoolAllocation = 10,
        WrDelayExecution = 11,
        WrSuspended = 12,
        WrUserRequest = 13,
        WrEventPair = 14,
        WrQueue = 15,
        WrLpcReceive = 16,
        WrLpcReply = 17,
        WrVirtualMemory = 18,
        WrPageOut = 19,
        WrRendezvous = 20,
        Spare2 = 21,
        Spare3 = 22,
        Spare4 = 23,
        Spare5 = 24,
        WrCalloutStack = 25,
        WrKernel = 26,
        WrResource = 27,
        WrPushLock = 28,
        WrMutex = 29,
        WrQuantumEnd = 30,
        WrDispatchInt = 31,
        WrPreempted = 32,
        WrYieldExecution = 33,
        WrFastMutex = 34,
        WrGuardedMutex = 35,
        WrRundown = 36,
        MaximumWaitReason = 37
    }

    [Flags]
    public enum LdrpDataTableEntryFlags : uint
    {
        StaticLink = 0x00000002,
        ImageDll = 0x00000004,
        Flag0x8 = 0x00000008,
        Flag0x10 = 0x00000010,
        LoadInProgress = 0x00001000,
        UnloadInProgress = 0x00002000,
        EntryProcessed = 0x00004000,
        EntryInserted = 0x00008000,
        CurrentLoad = 0x00010000,
        FailedBuiltInLoad = 0x00020000,
        DontCallForThreads = 0x00040000,
        ProcessAttachCalled = 0x00080000,
        DebugSymbolsLoaded = 0x00100000,
        ImageNotAtBase = 0x00200000,
        CorImage = 0x00400000,
        CorOwnsUnmap = 0x00800000,
        SystemMapped = 0x01000000,
        ImageVerifying = 0x02000000,
        DriverDependentDll = 0x04000000,
        EntryNative = 0x08000000,
        Redirected = 0x10000000,
        NonPagedDebugInfo = 0x20000000,
        MmLoaded = 0x40000000,
        CompatDatabaseProcessed = 0x80000000
    }

    /// <summary>
    /// Specifies an executable's target CPU type.
    /// </summary>
    public enum MachineType : ushort
    {
        /// <summary>
        /// Assumed to be applicable to any machine type.
        /// </summary>
        Unknown = 0x0,

        /// <summary>
        /// Matsushita AM33.
        /// </summary>
        Am33 = 0x1d3,

        /// <summary>
        /// x64.
        /// </summary>
        Amd64 = 0x8664,

        /// <summary>
        /// ARM little-endian.
        /// </summary>
        Arm = 0x1c0,

        /// <summary>
        /// EFI byte code.
        /// </summary>
        Ebc = 0xebc,

        /// <summary>
        /// Intel 386 or later processors and compatible processors.
        /// </summary>
        I386 = 0x14c,

        /// <summary>
        /// Intel Itanium processor family.
        /// </summary>
        Ia64 = 0x200,

        /// <summary>
        /// Mitsubishi M32R little endian.
        /// </summary>
        M32R = 0x9041,

        /// <summary>
        /// MIPS16.
        /// </summary>
        Mips16 = 0x266,

        /// <summary>
        /// MIPS with FPU.
        /// </summary>
        MipsFpu = 0x366,

        /// <summary>
        /// MIPS16 with FPU.
        /// </summary>
        MipsFpu16 = 0x466,

        /// <summary>
        /// PowerPC little-endian.
        /// </summary>
        PowerPc = 0x1f0,

        /// <summary>
        /// PowerPC with floating point support.
        /// </summary>
        PowerPcFp = 0x1f1,

        /// <summary>
        /// MIPS little-endian.
        /// </summary>
        R4000 = 0x166,

        /// <summary>
        /// Hitachi SH3.
        /// </summary>
        Sh3 = 0x1a2,

        /// <summary>
        /// Hitachi SH3 DSP.
        /// </summary>
        Sh3Dsp = 0x1a3,

        /// <summary>
        /// Hitachi SH4.
        /// </summary>
        Sh4 = 0x1a6,

        /// <summary>
        /// Hitachi SH5.
        /// </summary>
        Sh5 = 0x1a8,

        /// <summary>
        /// Thumb.
        /// </summary>
        Thumb = 0x1c2,

        /// <summary>
        /// MIPS little-endian WCE v2.
        /// </summary>
        WceMipsv2 = 0x169
    }

    public enum MediaType : int
    {
        Unknown,
        F5_1Pt2_512,
        F3_1Pt44_512,
        F3_2Pt88_512,
        F3_20Pt8_512,
        F3_720_512,
        F5_360_512,
        F5_320_512,
        F5_320_1024,
        F5_180_512,
        F5_160_512,
        RemovableMedia,
        FixedMedia,
        F3_120M_512,
        F3_640_512,
        F5_640_512,
        F5_720_512,
        F3_1Pt2_512,
        F3_1Pt23_1024,
        F5_1Pt23_1024,
        F3_128Mb_512,
        F3_230Mb_512,
        F8_256_128,
        F3_200Mb_512,
        F3_240M_512,
        F3_32M_512,

        DDS_4mm = 0x20,
        MiniQic,
        Travan,
        QIC,
        MP_8mm,
        AME_8mm,
        AIT1_8mm,
        DLT,
        NCTP,
        IBM_3480,
        IBM_3490E,
        IBM_Magstar_3590,
        IBM_Magstar_MP,
        STK_DATA_D3,
        SONY_DTF,
        DV_6mm,
        DMI,
        SONY_D2,
        CLEANER_CARTRIDGE,
        CD_ROM,
        CD_R,
        CD_RW,
        DVD_ROM,
        DVD_R,
        DVD_RW,
        MO_3_RW,
        MO_5_WO,
        MO_5_RW,
        MO_5_LIMDOW,
        PC_5_WO,
        PC_5_RW,
        PD_5_RW,
        ABL_5_WO,
        PINNACLE_APEX_5_RW,
        SONY_12_WO,
        PHILIPS_12_WO,
        HITACHI_12_WO,
        CYGNET_12_WO,
        KODAK_14_WO,
        MO_NFR_525,
        NIKON_12_RW,
        IOMEGA_ZIP,
        IOMEGA_JAZ,
        SYQUEST_EZ135,
        SYQUEST_EZFLYER,
        SYQUEST_SYJET,
        AVATAR_F2,
        MP2_8mm,
        DST_S,
        DST_M,
        DST_L,
        VXATape_1,
        VXATape_2,
        STK_9840,
        LTO_Ultrium,
        LTO_Accelis,
        DVD_RAM,
        AIT_8mm,
        ADR_1,
        ADR_2,
        STK_9940,
        SAIT,
        VXATape
    }

    [Flags]
    public enum MemExecuteOptions : int
    {
        ExecuteDisable = 0x1,
        ExecuteEnable = 0x2,
        DisableThunkEmulation = 0x4,
        Permanent = 0x8
    }

    [Flags]
    public enum MemoryFlags : uint
    {
        Commit = 0x1000,
        Reserve = 0x2000,
        Decommit = 0x4000,
        Release = 0x8000,
        Free = 0x10000,
        Private = 0x20000,
        Mapped = 0x40000,
        Reset = 0x80000,
        TopDown = 0x100000,
        WriteWatch = 0x200000,
        Physical = 0x400000,
        LargePages = 0x20000000,
        DosLimit = 0x40000000,
        FourMbPages = 0x80000000
    }

    public enum MemoryInformationClass : int
    {
        MemoryBasicInformation,
        MemoryWorkingSetInformation,
        MemoryMappedFilenameInformation,
        MemoryRegionInformation,
        MemoryWorkingSetExInformation
    }

    public enum MemoryMapType : int
    {
        Process = 1,
        System = 2
    }

    [Flags]
    public enum MemoryProtection : uint
    {
        AccessDenied = 0x0,
        Execute = 0x10,
        ExecuteRead = 0x20,
        ExecuteReadWrite = 0x40,
        ExecuteWriteCopy = 0x80,
        Guard = 0x100,
        NoCache = 0x200,
        WriteCombine = 0x400,
        NoAccess = 0x01,
        ReadOnly = 0x02,
        ReadWrite = 0x04,
        WriteCopy = 0x08
    }

    [Flags]
    public enum MessageResourceFlags : ushort
    {
        Unicode = 0x1
    }

    public enum MutantInformationClass : int
    {
        MutantBasicInformation,
        MutantOwnerInformation
    }

    [Flags]
    public enum NotificationMask : uint
    {
        Mask = 0x3fffffff,
        PrePrepare = 0x00000001,
        Prepare = 0x00000002,
        Commit = 0x00000004,
        Rollback = 0x00000008,
        PrePrepareComplete = 0x00000010,
        PrepareComplete = 0x00000020,
        CommitComplete = 0x00000040,
        RollbackComplete = 0x00000080,
        Recover = 0x00000100,
        SinglePhaseComplete = 0x00000200,
        DelegateCommit = 0x00000400,
        RecoverQuery = 0x00000800,
        EnlistPrePrepare = 0x00001000,
        LastRecover = 0x00002000,
        Indoubt = 0x00004000,
        PropagatePull = 0x00008000,
        PropagatePush = 0x00010000,
        Marshal = 0x00020000,
        EnlistMask = 0x00040000,
        RmDisconnected = 0x01000000,
        TmOnline = 0x02000000,
        CommitRequest = 0x04000000,
        Promote = 0x08000000,
        PromoteNew = 0x10000000,
        RequestOutcome = 0x20000000,

        // For filter manager use only. DO NOT SPECIFY.
        CommitFinalize = 0x40000000
    }

    [Flags]
    public enum ObjectAceFlags : uint
    {
        ObjectTypePresent = 0x1,
        InheritedObjectTypePresent = 0x2
    }

    [Flags]
    public enum ObjectFlags : uint
    {
        Inherit = 0x2,
        Permanent = 0x10,
        Exclusive = 0x20,
        CaseInsensitive = 0x40,
        OpenIf = 0x80,
        OpenLink = 0x100,
        KernelHandle = 0x200,
        ForceAccessCheck = 0x400,
        ValidAttributes = 0x7f2
    }

    public enum ObjectInformationClass : int
    {
        ObjectBasicInformation = 0,
        ObjectNameInformation = 1,
        ObjectTypeInformation = 2,
        ObjectTypesInformation = 3,
        ObjectHandleFlagInformation = 4,
        ObjectSessionInformation = 5
    }

    public enum PartitionType : byte
    {
        Unused = 0x0,
        Fat12 = 0x1,
        Xenix1 = 0x2,
        Xenix2 = 0x3,
        Fat16 = 0x4,
        Extended = 0x5,
        Huge = 0x6,
        Ifs = 0x7,
        Os2BootMgr = 0xa,
        Fat32 = 0xb,
        Fat32XInt13 = 0xc,
        XInt13 = 0xe,
        XInt13Extended = 0xf,
        Prep = 0x41,
        Ldm = 0x42,
        Unix = 0x63,

        Ntft = 0x80
    }

    public enum PipeEnd : int
    {
        Client = 0,
        Server = 1
    }

    public enum PipeCompletionMode : int
    {
        Queue = 0,
        Complete = 1
    }

    public enum PipeConfiguration : int
    {
        Inbound = 0,
        Outbound = 1,
        FullDuplex = 2
    }

    public enum PipeState : int
    {
        Disconnected = 1,
        Listening = 2,
        Connected = 3,
        Closing = 4
    }

    public enum PipeType : int
    {
        ByteStream = 0,
        Message = 1
    }

    public enum PortMessageType : short
    {
        Request = 1,
        Reply = 2,
        Datagram = 3,
        LostReply = 4,
        PortClosed = 5,
        ClientDied = 6,
        Exception = 7,
        DebugEvent = 8,
        ErrorEvent = 9,
        ConnectionRequest = 10
    }

    [Flags]
    public enum PrivilegeSetFlags : int
    {
        AnyNecessary = 0,
        AllNecessary = 0x1
    }

    public enum ProcessInformationClass : int
    {
        ProcessBasicInformation, // 0
        ProcessQuotaLimits,
        ProcessIoCounters,
        ProcessVmCounters,
        ProcessTimes,
        ProcessBasePriority,
        ProcessRaisePriority,
        ProcessDebugPort,
        ProcessExceptionPort,
        ProcessAccessToken,
        ProcessLdtInformation, // 10
        ProcessLdtSize,
        ProcessDefaultHardErrorMode,
        ProcessIoPortHandlers,
        ProcessPooledUsageAndLimits,
        ProcessWorkingSetWatch,
        ProcessUserModeIOPL,
        ProcessEnableAlignmentFaultFixup,
        ProcessPriorityClass,
        ProcessWx86Information,
        ProcessHandleCount, // 20
        ProcessAffinityMask,
        ProcessPriorityBoost,
        ProcessDeviceMap,
        ProcessSessionInformation,
        ProcessForegroundInformation,
        ProcessWow64Information,
        ProcessImageFileName,
        ProcessLUIDDeviceMapsEnabled,
        ProcessBreakOnTermination,
        ProcessDebugObjectHandle, // 30
        ProcessDebugFlags,
        ProcessHandleTracing,
        ProcessIoPriority,
        ProcessExecuteFlags,
        ProcessResourceManagement,
        ProcessCookie,
        ProcessImageInformation,
        ProcessCycleTime,
        ProcessPagePriority,
        ProcessInstrumentationCallback, // 40
        ProcessThreadStackAllocation,
        ProcessWorkingSetWatchEx,
        ProcessImageFileNameWin32,
        ProcessImageFileMapping,
        ProcessAffinityUpdateMode,
        ProcessMemoryAllocationMode,
        ProcessGroupInformation,
        ProcessTokenVirtualizationEnabled,
        ProcessConsoleHostProcess,
        ProcessWindowInformation, // 50
        MaxProcessInfoClass
    }

    public enum ProcessPriorityClass
    {
        Unknown = 0,
        Idle = 1,
        Normal = 2,
        High = 3,
        RealTime = 4,
        BelowNormal = 5,
        AboveNormal = 6
    }

    [Flags]
    public enum RegHiveFormat : int
    {
        Standard = 0x1,
        Latest = 0x2,
        NoCompression = 0x4
    }

    [Flags]
    public enum RegNotifyFilter : int
    {
        Name = 0x1,
        Attributes = 0x2,
        LastSet = 0x4,
        Security = 0x8,
        Legal = Name | Attributes | LastSet | Security
    }

    [Flags]
    public enum RegOptions : int
    {
        Reserved = 0x0,
        NonVolatile = 0x0,
        Volatile = 0x1,
        CreateLink = 0x2,
        BackupRestore = 0x4,
        OpenLink = 0x8,
        Legal = Reserved | NonVolatile | Volatile | CreateLink | BackupRestore | OpenLink
    }

    [Flags]
    public enum RegRestoreFlags : int
    {
        WholeHiveVolatile = 0x1,
        RefreshHive = 0x2,
        NoLazyFlush = 0x4,
        ForceRestore = 0x8
    }

    [Flags]
    public enum RegUnloadFlags : int
    {
        ForceUnload = 0x1
    }

    public enum ResourceManagerInformationClass : int
    {
        ResourceManagerBasicInformation,
        ResourceManagerCompletionInformation,
        ResourceManagerFullInformation
    }

    [Flags]
    public enum ResourceManagerOptions : int
    {
        Volatile = 0x1,
        Communication = 0x2,
        MaximumOption = 0x3
    }

    [Flags]
    public enum RtlAcquirePrivilegeFlags : int
    {
        Revert = 0x1,
        Process = 0x2
    }

    [Flags]
    public enum RtlDuplicateUnicodeStringFlags : int
    {
        NullTerminate = 0x1,
        AllocateNullString = 0x2
    }

    public enum RtlLockType : ushort
    {
        CriticalSection = 0,
        Resource = 1
    }

    [Flags]
    public enum RtlQueryProcessDebugFlags : uint
    {
        Modules = 0x00000001,
        BackTraces = 0x00000002,
        HeapSummary = 0x00000004,
        HeapTags = 0x00000008,
        HeapEntries = 0x00000010,
        Locks = 0x00000020,
        Modules32 = 0x00000040,

        NonInvasive = 0x80000000
    }

    [Flags]
    public enum RtlUserProcessFlags : uint
    {
        ParamsNormalized = 0x00000001,
        ProfileUser = 0x00000002,
        ProfileKernel = 0x00000004,
        ProfileServer = 0x00000008,
        Reserve1Mb = 0x00000020,
        Reserve16Mb = 0x00000040,
        CaseSensitive = 0x00000080,
        DisableHeapDecommit = 0x00000100,
        DllRedirectionLocal = 0x00001000,
        AppManifestPresent = 0x00002000,
        ImageKeyMissing = 0x00004000,
        OptInProcess = 0x00020000
    }

    [Flags]
    public enum SectionAttributes : uint
    {
        Based = 0x200000,
        NoChange = 0x400000,
        File = 0x800000,
        Image = 0x1000000,
        Reserve = 0x4000000,
        Commit = 0x8000000,
        NoCache = 0x10000000,
        Global = 0x20000000,
        LargePages = 0x80000000
    }

    [Flags]
    public enum SectionInformationClass : int
    {
        SectionBasicInformation,
        SectionImageInformation
    }

    public enum SectionInherit : int
    {
        ViewShare = 1,
        ViewUnmap = 2
    }

    [Flags]
    public enum SecurityDescriptorControlFlags : ushort
    {
        OwnerDefaulted = 0x0001,
        GroupDefaulted = 0x0002,
        DaclPresent = 0x0004,
        DaclDefaulted = 0x0008,
        SaclPresent = 0x0010,
        SaclDefaulted = 0x0020,
        DaclUntrusted = 0x0040,
        ServerSecurity = 0x0080,
        DaclAutoInheritReq = 0x0100,
        SaclAutoInheritReq = 0x0200,
        DaclAutoInherited = 0x0400,
        SaclAutoInherited = 0x0800,
        DaclProtected = 0x1000,
        SaclProtected = 0x2000,
        RmControlValid = 0x4000,
        SelfRelative = 0x8000
    }

    public enum SecurityImpersonationLevel : int
    {
        SecurityAnonymous,
        SecurityIdentification,
        SecurityImpersonation,
        SecurityDelegation
    }

    [Flags]
    public enum SecurityInformation : uint
    {
        Owner = 0x00000001,
        Group = 0x00000002,
        Dacl = 0x00000004,
        Sacl = 0x00000008,
        Label = 0x00000010,

        ProtectedDacl = 0x80000000,
        ProtectedSacl = 0x40000000,
        UnprotectedDacl = 0x20000000,
        UnprotectedSacl = 0x10000000
    }

    public enum SemaphoreInformationClass : int
    {
        SemaphoreBasicInformation
    }

    [Flags]
    public enum SidAttributes : uint
    {
        Mandatory = 0x00000001,
        EnabledByDefault = 0x00000002,
        Enabled = 0x00000004,
        Owner = 0x00000008,
        UseForDenyOnly = 0x00000010,
        Integrity = 0x00000020,
        IntegrityEnabled = 0x00000040,
        LogonId = 0xc0000000,
        Resource = 0x20000000
    }

    public enum SidNameUse : int
    {
        User = 1,
        Group,
        Domain,
        Alias,
        WellKnownGroup,
        DeletedAccount,
        Invalid,
        Unknown,
        Computer,
        Label
    }

    [Flags]
    public enum SiRequested : uint
    {
        OwnerSecurityInformation = 0x1,
        GroupSecurityInformation = 0x2,
        DaclSecurityInformation = 0x4,
        SaclSecurityInformation = 0x8,
        LabelSecurityInformation = 0x10
    }

    [Flags]
    public enum SuiteType : uint
    {
        SmallBusiness = 0x00000001,
        Enterprise = 0x00000002,
        BackOffice = 0x00000004,
        Communications = 0x00000008,
        Terminal = 0x00000010,
        SmallBusinessRestricted = 0x00000020,
        EmbeddedNt = 0x00000040,
        DataCenter = 0x00000080,
        SingleUserTs = 0x00000100,
        Personal = 0x00000200,
        Blade = 0x00000400,
        EmbeddedRestricted = 0x00000800,
        SecurityAppliance = 0x00001000,
        StorageServer = 0x00002000,
        ComputeServer = 0x00004000,

        WorkstationNt = 0x40000000,
        ServerNt = 0x80000000
    }

    public enum SystemInformationClass : int
    {
        SystemBasicInformation,
        SystemProcessorInformation,
        SystemPerformanceInformation,
        SystemTimeOfDayInformation,
        SystemPathInformation,
        SystemProcessInformation,
        SystemCallCountInformation,
        SystemDeviceInformation,
        SystemProcessorPerformanceInformation,
        SystemFlagsInformation,
        SystemCallTimeInformation, // 10
        SystemModuleInformation,
        SystemLocksInformation,
        SystemStackTraceInformation,
        SystemPagedPoolInformation,
        SystemNonPagedPoolInformation,
        SystemHandleInformation,
        SystemObjectInformation,
        SystemPageFileInformation,
        SystemVdmInstemulInformation,
        SystemVdmBopInformation, // 20
        SystemFileCacheInformation,
        SystemPoolTagInformation,
        SystemInterruptInformation,
        SystemDpcBehaviorInformation,
        SystemFullMemoryInformation,
        SystemLoadGdiDriverInformation,
        SystemUnloadGdiDriverInformation,
        SystemTimeAdjustmentInformation,
        SystemSummaryMemoryInformation,
        SystemMirrorMemoryInformation, // 30
        SystemPerformanceTraceInformation,
        SystemCrashDumpInformation,
        SystemExceptionInformation,
        SystemCrashDumpStateInformation,
        SystemKernelDebuggerInformation,
        SystemContextSwitchInformation,
        SystemRegistryQuotaInformation,
        SystemExtendServiceTableInformation, // used to be SystemLoadAndCallImage
        SystemPrioritySeparation,
        SystemVerifierAddDriverInformation, // 40
        SystemVerifierRemoveDriverInformation,
        SystemProcessorIdleInformation,
        SystemLegacyDriverInformation,
        SystemCurrentTimeZoneInformation,
        SystemLookasideInformation,
        SystemTimeSlipNotification,
        SystemSessionCreate,
        SystemSessionDetach,
        SystemSessionInformation,
        SystemRangeStartInformation, // 50
        SystemVerifierInformation,
        SystemVerifierThunkExtend,
        SystemSessionProcessInformation,
        SystemLoadGdiDriverInSystemSpace,
        SystemNumaProcessorMap,
        SystemPrefetcherInformation,
        SystemExtendedProcessInformation,
        SystemRecommendedSharedDataAlignment,
        SystemComPlusPackage,
        SystemNumaAvailableMemory, // 60
        SystemProcessorPowerInformation,
        SystemEmulationBasicInformation,
        SystemEmulationProcessorInformation,
        SystemExtendedHandleInformation,
        SystemLostDelayedWriteInformation,
        SystemBigPoolInformation,
        SystemSessionPoolTagInformation,
        SystemSessionMappedViewInformation,
        SystemHotpatchInformation,
        SystemObjectSecurityMode, // 70
        SystemWatchdogTimerHandler, // doesn't seem to be implemented
        SystemWatchdogTimerInformation,
        SystemLogicalProcessorInformation,
        SystemWow64SharedInformation,
        SystemRegisterFirmwareTableInformationHandler,
        SystemFirmwareTableInformation,
        SystemModuleInformationEx,
        SystemVerifierTriageInformation,
        SystemSuperfetchInformation,
        SystemMemoryListInformation, // 80
        SystemFileCacheInformationEx,
        SystemNotImplemented19,
        SystemProcessorDebugInformation,
        SystemVerifierInformation2,
        SystemNotImplemented20,
        SystemRefTraceInformation,
        SystemSpecialPoolTag, // MmSpecialPoolTag, then MmSpecialPoolCatchOverruns != 0
        SystemProcessImageName,
        SystemNotImplemented21,
        SystemBootEnvironmentInformation, // 90
        SystemEnlightenmentInformation,
        SystemVerifierInformationEx,
        SystemNotImplemented22,
        SystemNotImplemented23,
        SystemCovInformation,
        SystemNotImplemented24,
        SystemNotImplemented25,
        SystemPartitionInformation,
        SystemSystemDiskInformation, // this and SystemPartitionInformation both call IoQuerySystemDeviceName
        SystemPerformanceDistributionInformation, // 100
        SystemNumaProximityNodeInformation,
        SystemTimeZoneInformation2,
        SystemCodeIntegrityInformation,
        SystemNotImplemented26,
        SystemUnknownInformation, // No symbols for this case, very strange...
        SystemVaInformation // 106, calls MmQuerySystemVaInformation
    }

    public enum ThreadInformationClass : uint
    {
        ThreadBasicInformation,
        ThreadTimes,
        ThreadPriority,
        ThreadBasePriority,
        ThreadAffinityMask,
        ThreadImpersonationToken,
        ThreadDescriptorTableEntry,
        ThreadEnableAlignmentFaultFixup,
        ThreadEventPair,
        ThreadQuerySetWin32StartAddress,
        ThreadZeroTlsCell, // 10
        ThreadPerformanceCount,
        ThreadAmILastThread,
        ThreadIdealProcessor,
        ThreadPriorityBoost,
        ThreadSetTlsArrayAddress,
        ThreadIsIoPending,
        ThreadHideFromDebugger,
        ThreadBreakOnTermination,
        ThreadSwitchLegacyState,
        ThreadIsTerminated, // 20
        ThreadLastSystemCall,
        ThreadIoPriority,
        ThreadCycleTime,
        ThreadPagePriority,
        ThreadActualBasePriority,
        ThreadTebInformation,
        ThreadCSwitchMon,
        ThreadCSwitchPmu,
        ThreadWow64Context,
        ThreadGroupInformation, // 30
        ThreadUmsInformation,
        ThreadCounterProfiling,
        ThreadIdealProcessorEx,
        MaxThreadInfoClass
    }

    public enum TimerInformationClass : int
    {
        TimerBasicInformation
    }

    public enum TimerType : int
    {
        NotificationTimer,
        SynchronizationTimer
    }

    public enum TmInformationClass : int
    {
        TransactionManagerBasicInformation,
        TransactionManagerLogInformation,
        TransactionManagerLogPathInformation,
        TransactionManagerOnlineProbeInformation,
        TransactionManagerRecoveryInformation
    }

    [Flags]
    public enum TmOptions : int
    {
        Volatile = 0x1,
        CommitDefault = 0x0,
        CommitSystemVolume = 0x2,
        CommitSystemHives = 0x4,
        CommitLowest = 0x8,
        CorruptForRecovery = 0x10,
        CorruptForProgress = 0x20,
        MaximumOption = 0x3f
    }

    public enum TokenElevationType : int
    {
        Default = 1,
        Full,
        Limited
    }

    public enum TokenInformationClass
    {
        TokenUser = 1,
        TokenGroups,
        TokenPrivileges,
        TokenOwner,
        TokenPrimaryGroup,
        TokenDefaultDacl,
        TokenSource,
        TokenType,
        TokenImpersonationLevel,
        TokenStatistics, // 10
        TokenRestrictedSids,
        TokenSessionId,
        TokenGroupsAndPrivileges,
        TokenSessionReference,
        TokenSandBoxInert,
        TokenAuditPolicy,
        TokenOrigin,
        TokenElevationType,
        TokenLinkedToken,
        TokenElevation, // 20
        TokenHasRestrictions,
        TokenAccessInformation,
        TokenVirtualizationAllowed,
        TokenVirtualizationEnabled,
        TokenIntegrityLevel,
        TokenUIAccess,
        TokenMandatoryPolicy,
        TokenLogonSid,
        MaxTokenInfoClass  // MaxTokenInfoClass should always be the last enum
    }

    public enum TokenType : int
    {
        Primary = 1,
        Impersonation
    }

    public enum TransactionInformationClass : int
    {
        TransactionBasicInformation,
        TransactionPropertiesInformation,
        TransactionEnlistmentInformation,
        TransactionFullInformation
    }

    [Flags]
    public enum TransactionOptions : int
    {
        DoNotPromote = 0x1,
        MaximumOption = 0x1
    }

    public enum TransactionOutcome : int
    {
        Undetermined = 1,
        Committed,
        Aborted
    }

    public enum TransactionState : int
    {
        Normal = 1,
        Indoubt,
        CommittedNotify
    }

    public enum WaitType : int
    {
        WaitAll,
        WaitAny
    }

    public enum WellKnownSidType : int
    {
        WinNullSid = 0,
        WinWorldSid = 1,
        WinLocalSid = 2,
        WinCreatorOwnerSid = 3,
        WinCreatorGroupSid = 4,
        WinCreatorOwnerServerSid = 5,
        WinCreatorGroupServerSid = 6,
        WinNtAuthoritySid = 7,
        WinDialupSid = 8,
        WinNetworkSid = 9,
        WinBatchSid = 10,
        WinInteractiveSid = 11,
        WinServiceSid = 12,
        WinAnonymousSid = 13,
        WinProxySid = 14,
        WinEnterpriseControllersSid = 15,
        WinSelfSid = 16,
        WinAuthenticatedUserSid = 17,
        WinRestrictedCodeSid = 18,
        WinTerminalServerSid = 19,
        WinRemoteLogonIdSid = 20,
        WinLogonIdsSid = 21,
        WinLocalSystemSid = 22,
        WinLocalServiceSid = 23,
        WinNetworkServiceSid = 24,
        WinBuiltinDomainSid = 25,
        WinBuiltinAdministratorsSid = 26,
        WinBuiltinUsersSid = 27,
        WinBuiltinGuestsSid = 28,
        WinBuiltinPowerUsersSid = 29,
        WinBuiltinAccountOperatorsSid = 30,
        WinBuiltinSystemOperatorsSid = 31,
        WinBuiltinPrintOperatorsSid = 32,
        WinBuiltinBackupOperatorsSid = 33,
        WinBuiltinReplicatorSid = 34,
        WinBuiltinPreWindows2000CompatibleAccessSid = 35,
        WinBuiltinRemoteDesktopUsersSid = 36,
        WinBuiltinNetworkConfigurationOperatorsSid = 37,
        WinAccountAdministratorSid = 38,
        WinAccountGuestSid = 39,
        WinAccountKrbtgtSid = 40,
        WinAccountDomainAdminsSid = 41,
        WinAccountDomainUsersSid = 42,
        WinAccountDomainGuestsSid = 43,
        WinAccountComputersSid = 44,
        WinAccountControllersSid = 45,
        WinAccountCertAdminsSid = 46,
        WinAccountSchemaAdminsSid = 47,
        WinAccountEnterpriseAdminsSid = 48,
        WinAccountPolicyAdminsSid = 49,
        WinAccountRasAndIasServersSid = 50,
        WinNTLMAuthenticationSid = 51,
        WinDigestAuthenticationSid = 52,
        WinSChannelAuthenticationSid = 53,
        WinThisOrganizationSid = 54,
        WinOtherOrganizationSid = 55,
        WinBuiltinIncomingForestTrustBuildersSid = 56,
        WinBuiltinPerfMonitoringUsersSid = 57,
        WinBuiltinPerfLoggingUsersSid = 58,
        WinBuiltinAuthorizationAccessSid = 59,
        WinBuiltinTerminalServerLicenseServersSid = 60,
        WinBuiltinDCOMUsersSid = 61,
        WinBuiltinIUsersSid = 62,
        WinIUserSid = 63,
        WinBuiltinCryptoOperatorsSid = 64,
        WinUntrustedLabelSid = 65,
        WinLowLabelSid = 66,
        WinMediumLabelSid = 67,
        WinHighLabelSid = 68,
        WinSystemLabelSid = 69,
        WinWriteRestrictedCodeSid = 70,
        WinCreatorOwnerRightsSid = 71,
        WinCacheablePrincipalsGroupSid = 72,
        WinNonCacheablePrincipalsGroupSid = 73,
        WinEnterpriseReadonlyControllersSid = 74,
        WinAccountReadonlyControllersSid = 75,
        WinBuiltinEventLogReadersGroup = 76,
        WinNewEnterpriseReadonlyControllersSid = 77,
        WinBuiltinCertSvcDComAccessGroup = 78
    }

    public enum WinNtProductType : int
    {
        WinNt = 1,
        LanManNt,
        Server
    }

    [Flags]
    public enum WtFlags : uint
    {
        ExecuteDefault = 0x0,
        ExecuteInIoThread = 0x1,
        ExecuteInUiThread = 0x2,
        ExecuteInWaitThread = 0x4,
        ExecuteOnlyOnce = 0x8,
        ExecuteLongFunction = 0x10,
        ExecuteInTimerThread = 0x20,
        ExecuteInPersistentIoThread = 0x40,
        ExecuteInPersistentThread = 0x80,
        TransferImpersonation = 0x100
    }
}
