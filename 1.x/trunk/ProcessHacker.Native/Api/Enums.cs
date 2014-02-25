/*
 * Process Hacker - 
 *   windows API enums
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

/* This file contains enumeration declarations for the Win32 API.
 * 
 * All enumerations which do not belong in any other category 
 * are placed in this file.
 */

using System;

namespace ProcessHacker.Native.Api
{
    public enum AddressMode : int
    {
        AddrMode1616,
        AddrMode1632,
        AddrModeReal,
        AddrModeFlat
    }

    public enum AiFamily : int
    {
        /// <summary>
        /// The address family is unspecified.
        /// </summary>
        Unspecified = 0,
        /// <summary>
        /// The Internet Protocol version 4 (IPv4) address family.
        /// </summary>
        INet = 2,
        /// <summary>
        /// The NetBIOS address family. This address family is only supported 
        /// if a Windows Sockets provider for NetBIOS is installed.
        /// </summary>
        NetBios = 17,
        /// <summary>
        /// The Internet Protocol version 6 (IPv6) address family.
        /// </summary>
        INet6 = 23,
        /// <summary>
        /// The Infrared Data Association (IrDA) address family. This address 
        /// family is only supported if the computer has an infrared port and 
        /// driver installed.
        /// </summary>
        IrDA = 26,
        /// <summary>
        /// The Bluetooth address family. This address family is only supported 
        /// if a Bluetooth adapter is installed on Windows Server 2003 or later.
        /// </summary>
        Bth = 32
    }

    [Flags]
    public enum AllocFlags : uint
    {
        LHnd = 0x42,
        LMemFixed = 0x0,
        LMemMoveable = 0x2,
        LMemZeroInit = 0x40,
        LPtr = 0x40,
        NonZeroLHnd = LMemMoveable,
        NonZeroLPtr = LMemFixed
    }

    [Flags]
    public enum CredPackFlags : uint
    {
        ProtectedCredentials = 0x1,
        WowBuffer = 0x2,
        GenericCredentials = 0x4
    }

    [Flags]
    public enum CredUiFlags : uint
    {
        IncorrectPassword = 0x1,
        DoNotPersist = 0x2,
        RequestAdministrator = 0x4,
        ExcludeCertificates = 0x8,
        RequireCertificate = 0x10,
        ShowSaveCheckBox = 0x40,
        AlwaysShowUi = 0x80,
        RequireSmartcard = 0x100,
        PasswordOnlyOk = 0x200,
        ValidateUsername = 0x400,
        CompleteUsername = 0x800,
        Persist = 0x1000,
        ServerCredential = 0x4000,
        ExpectConfirmation = 0x20000,
        GenericCredentials = 0x40000,
        UsernameTargetCredentials = 0x80000,
        KeepUsername = 0x100000
    }

    [Flags]
    public enum CredUiWinFlags : uint
    {
        Generic = 0x1,
        CheckBox = 0x2,
        AuthPackageOnly = 0x10,
        InCredOnly = 0x20,
        EnumerateAdmins = 0x100,
        EnumerateCurrentUser = 0x200,
        SecurePrompt = 0x1000,
        Pack32Wow = 0x10000000
    }

    public enum DepFlags : uint
    {
        Disable = 0x00000000,
        Enable = 0x00000001,
        DisableAtlThunkEmulation = 0x00000002
    }

    public enum DepSystemPolicyType : int
    {
        AlwaysOff = 0,
        AlwaysOn,
        OptIn,
        OptOut
    }

    [Flags]
    public enum ExitWindowsFlags : uint
    {
        Logoff = 0x0,
        Poweroff = 0x8,
        Reboot = 0x2,
        RestartApps = 0x40,
        Shutdown = 0x1,
        Force = 0x4,
        ForceIfHung = 0x10
    }

    public enum FileCreationDispositionWin32 : uint
    {
        /// <summary>
        /// Creates a new file. The function fails if the specified file already exists.
        /// </summary>
        CreateNew = 1,
        /// <summary>
        /// Creates a new file. If the file exists, the function overwrites the file and clears the existing attributes.
        /// </summary>
        CreateAlways = 2,
        /// <summary>
        /// Opens the file. The function fails if the file does not exist. 
        /// </summary>
        OpenExisting = 3,
        /// <summary>
        /// Opens the file, if it exists. If the file does not exist, the function creates the file.
        /// </summary>
        OpenAlways = 4,
        /// <summary>
        /// Opens the file. Once opened, the file is truncated so that its size is zero bytes. 
        /// The function fails if the file does not exist.
        /// </summary>
        TruncateExisting = 5
    }

    public enum GdiBlendMode : int
    {
        Black = 1,
        NotMergePen,
        MaskNotPen,
        NotCopyPen,
        MaskPenNot,
        Not,
        XorPen,
        NotMaskPen,
        MaskPen,
        NotXorPen,
        Nop,
        MergeNotPen,
        CopyPen,
        MergePenNot,
        MergePen,
        White,
        Last
    }

    public enum GdiPenStyle : int
    {
        Solid = 0,
        Dash,
        Dot,
        DashDot,
        DashDotDot,
        Null,
        InsideFrame,
        UserStyle,
        Alternate
    }

    public enum GdiStockObject : int
    {
        WhiteBrush = 0,
        LightGrayBrush,
        GrayBrush,
        DarkGrayBrush,
        BlackBrush,
        NullBrush,
        WhitePen,
        BlackPen,
        NullPen,
        OemFixedFont,
        AnsiFixedFont,
        AnsiVarFont,
        SystemFont,
        DeviceDefaultFont,
        DefaultPalette,
        SystemFixedFont,
        DefaultGuiFont,
        DcBrush,
        DcPen
    }

    public enum GetWindowLongOffset : int
    {
        WndProc = -4,
        HInstance = -6,
        HwndParent = -8,
        Id = -12,
        Style = -16,
        ExStyle = -20,
        UserData = -21
    }

    [Flags]
    public enum HeapEntry32Flags : int
    {
        Fixed = 0x00000001,
        Free = 0x00000002,
        Moveable = 0x00000004
    }

    public enum LoadImageType : int
    {
        Bitmap = 0,
        Icon = 1,
        Cursor = 2
    }

    public enum LogonFlags : uint
    {
        LogonWithProfile = 1,
        NetCredentialsOnly = 2
    }

    public enum LogonType : uint
    {
        Interactive = 2,
        Network = 3,
        Batch = 4,
        Service = 5,
        Unlock = 7,
        NetworkCleartext = 8,
        NewCredentials = 9
    }

    public enum LogonProvider : uint
    {
        Default = 0,
        WinNT35 = 1,
        WinNT40 = 2,
        WinNT50 = 3
    }

    [Flags]
    public enum MemoryState : uint
    {
        Commit = 0x1000,
        Reserve = 0x2000,

        /// <summary>
        /// Decommits memory, putting it into the reserved state.
        /// </summary>
        Decommit = 0x4000,

        /// <summary>
        /// Frees memory, putting it into the freed state.
        /// </summary>
        Release = 0x8000,
        Free = 0x10000,
        Reset = 0x80000,
        Physical = 0x400000,
        LargePages = 0x20000000
    }

    public enum MemoryType : int
    {
        Image = 0x1000000,
        Mapped = 0x40000,
        Private = 0x20000
    }

    public enum MibTcpState : int
    {
        Closed = 1,
        Listening = 2,
        SynSent = 3,
        SynReceived = 4,
        Established = 5,
        FinWait1 = 6,
        FinWait2 = 7,
        CloseWait = 8,
        Closing = 9,
        LastAck = 10,
        TimeWait = 11,
        DeleteTcb = 12
    }

    public enum MinidumpType : uint
    {
        Normal = 0x00000000,
        WithDataSegs = 0x00000001,
        WithFullMemory = 0x00000002,
        WithHandleData = 0x00000004,
        FilterMemory = 0x00000008,
        ScanMemory = 0x00000010,
        WithUnloadedModules = 0x00000020,
        WithIndirectlyReferencedMemory = 0x00000040,
        FilterModulePaths = 0x00000080,
        WithProcessThreadData = 0x00000100,
        WithPrivateReadWriteMemory = 0x00000200,
        WithoutOptionalData = 0x00000400,
        WithFullMemoryInfo = 0x00000800,
        WithThreadInfo = 0x00001000,
        WithCodeSegs = 0x00002000,
        WithoutAuxiliaryState = 0x00004000,
        WithFullAuxiliaryState = 0x00008000
    }

    [Flags]
    public enum ModuleFilterFlags : int
    {
        Default = 0x0,
        x32Bit = 0x01,
        x64Bit = 0x02,
        All = 0x03
    }

    public enum PeekMessageFlags : int
    {
        NoRemove = 0,
        Remove = 1,
        NoYield = 2,
    }

    [Flags]
    public enum PipeAccessMode : uint
    {
        Inbound = 0x1,
        Outbound = 0x2,
        Duplex = 0x3,
        FirstPipeInstance = 0x80000,
        WriteThrough = 0x80000000,
        Overlapped = 0x40000000,
        WriteDac = 0x40000,
        WriteOwner = 0x80000,
        AccessSystemSecurity = 0x01000000
    }

    [Flags]
    public enum PipeMode : uint
    {
        TypeByte = 0x0,
        TypeMessage = 0x4,
        ReadModeByte = 0x0,
        ReadModeMessage = 0x2,
        Wait = 0x0,
        NoWait = 0x1,
        AcceptRemoteClients = 0x0,
        RejectRemoteClients = 0x8
    }

    public enum PoolType : uint
    {
        NonPagedPool,
        PagedPool,
        NonPagedPoolMustSucceed,
        DontUseThisType,
        NonPagedPoolCacheAligned,
        PagedPoolCacheAligned,
        NonPagedPoolCacheAlignedMustS
    }

    public enum PrivateNamespaceFlags : int
    {
        Destroy = 0x1
    }

    [Flags]
    public enum ProcessCreationFlags : uint
    {
        DebugProcess = 0x1,
        DebugOnlyThisProcess = 0x2,
        CreateSuspended = 0x4,
        DetachedProcess = 0x8,
        CreateNewConsole = 0x10,
        NormalPriorityClass = 0x20,
        IdlePriorityClass = 0x40,
        HighPriorityClass = 0x80,
        RealtimePriorityClass = 0x100,
        CreateNewProcessGroup = 0x200,
        CreateUnicodeEnvironment = 0x400,
        CreateSeparateWowVdm = 0x800,
        CreateSharedWowVdm = 0x1000,
        CreateForceDos = 0x2000,
        BelowNormalPriorityClass = 0x4000,
        AboveNormalPriorityClass = 0x8000,
        StackSizeParamIsAReservation = 0x10000,
        InheritCallerPriority = 0x20000,
        CreateProtectedProcess = 0x40000,
        ExtendedStartupInfoPresent = 0x80000,
        ProcessModeBackgroundBegin = 0x100000,
        ProcessModeBackgroundEnd = 0x200000,
        CreateBreakawayFromJob = 0x1000000,
        CreatePreserveCodeAuthzLevel = 0x2000000,
        CreateDefaultErrorMode = 0x4000000,
        CreateNoWindow = 0x8000000,
        ProfileUser = 0x10000000,
        ProfileKernel = 0x20000000,
        ProfileServer = 0x40000000,
        CreateIgnoreSystemDefault = 0x80000000
    }

    public enum ProcessPriorityClassWin32 : int
    {
        Idle = 0x40,
        Normal = 0x20,
        High = 0x80,
        RealTime = 0x100,
        BelowNormal = 0x4000,
        AboveNormal = 0x8000
    }

    [Flags]
    public enum RedrawWindowFlags
    {
        Invalidate = 0x0001,
        InternalPaint = 0x0002,
        Erase = 0x0004,

        Validate = 0x0008,
        NoInternalPaint = 0x0010,
        NoErase = 0x0020,

        NoChildren = 0x0040,
        AllChildren = 0x0080,

        UpdateNow = 0x0100,
        EraseNow = 0x0200,

        Frame = 0x0400,
        NoFrame = 0x0800
    }

    [Flags]
    public enum RunFileDialogFlags : uint
    {
        /// <summary>
        /// Don't use any of the flags (only works alone)
        /// </summary>
        None = 0x0000,
        /// <summary>
        /// Removes the browse button
        /// </summary>
        NoBrowse = 0x0001,
        /// <summary>
        /// No default item selected
        /// </summary>
        NoDefault = 0x0002,
        /// <summary>
        /// Calculates the working directory from the file name
        /// </summary>
        CalcDirectory = 0x0004,
        /// <summary>
        /// Removes the edit box label
        /// </summary>
        NoLabel = 0x0008,
        /// <summary>
        /// Removes the separate memory space checkbox (Windows NT only)
        /// </summary>
        NoSeparateMemory = 0x0020
    }

    public enum ScActionType : int
    {
        None = 0,
        Reboot = 2,
        Restart = 1,
        RunCommand = 3
    }

    public enum SeObjectType : int
    {
        Unknown = 0,
        FileObject,
        Service,
        Printer,
        RegistryKey,
        LmShare,
        KernelObject,
        WindowObject,
        DsObject,
        DsObjectAll,
        ProviderDefinedObject,
        WmiGuidObject,
        RegistryWow6432Key
    }

    [Flags]
    public enum SePrivilegeAttributes : uint
    {
        Disabled = 0x00000000,
        EnabledByDefault = 0x00000001,
        Enabled = 0x00000002,
        Removed = 0x00000004,
        UsedForAccess = 0x80000000
    }

    public enum ServiceAccept : uint
    {
        NetBindChange = 0x10,
        ParamChange = 0x8,
        PauseContinue = 0x2,
        PreShutdown = 0x100,
        Shutdown = 0x4,
        Stop = 0x1,
        HardwareProfileChange = 0x20,
        PowerEvent = 0x40,
        SessionChange = 0x80
    }

    public enum ServiceControl : uint
    {
        Continue = 0x3,
        Interrogate = 0x4,
        NetBindAdd = 0x7,
        NetBindDisable = 0xa,
        NetBindEnable = 0x9,
        NetBindRemove = 0x8,
        ParamChange = 0x6,
        Pause = 0x2,
        Stop = 0x1
    }

    public enum ServiceErrorControl : uint
    {
        Critical = 0x3,
        Ignore = 0x0,
        Normal = 0x1,
        Severe = 0x2
    }

    public enum ServiceFlags : uint
    {
        None = 0,
        RunsInSystemProcess = 0x1
    }

    public enum ServiceInfoLevel : uint
    {
        Description = 1,
        FailureActions = 2,
        DelayedAutoStartInfo = 3,
        FailureActionsFlag = 4,
        SidInfo = 5,
        RequiredPrivilegesInfo = 6,
        PreShutdownInfo = 7,
        TriggerInfo = 8,
        PreferredNode = 9
    }

    public enum ServiceQueryState : uint
    {
        Active = 1,
        Inactive = 2,
        All = 3
    }

    [Flags]
    public enum ServiceQueryType : uint
    {
        Driver = 0xb,
        Win32 = 0x30
    }

    public enum ServiceStartType : uint
    {
        AutoStart = 0x2,
        BootStart = 0x0,
        DemandStart = 0x3,
        Disabled = 0x4,
        SystemStart = 0x1
    }

    public enum ServiceState : uint
    {
        ContinuePending = 0x5,
        PausePending = 0x6,
        Paused = 0x7,
        Running = 0x4,
        StartPending = 0x2,
        StopPending = 0x3,
        Stopped = 0x1
    }

    [Flags]
    public enum ServiceType : uint
    {
        FileSystemDriver = 0x2,
        KernelDriver = 0x1,
        Win32OwnProcess = 0x10,
        Win32ShareProcess = 0x20,
        InteractiveProcess = 0x100
    }

    public enum ShowWindowType : int
    {
        Hide = 0,
        ShowNormal = 1,
        Normal = 1,
        ShowMinimized = 2,
        ShowMaximized = 3,
        Maximize = 3,
        ShowNoActivate = 4,
        Show = 5,
        Minimize = 6,
        ShowMinNoActive = 7,
        ShowNa = 8,
        Restore = 9,
        ShowDefault = 10,
        ForceMinimize = 11,
        Max = 11
    }

    [Flags]
    public enum SiAccessFlags : int
    {
        Specific = 0x00010000,
        General = 0x00020000,
        Container = 0x00040000,
        Property = 0x00080000
    }

    public enum SiCallbackMessage : uint
    {
        Release = 1,
        Create = 2,
        InitDialog = WindowMessage.User + 1
    }

    [Flags]
    public enum SiObjectInfoFlags : int
    {        
        EditAll = EditPerms | EditOwner | EditAudits,
        EditPerms = 0x00000000,
        EditOwner = 0x00000001,
        EditAudits = 0x00000002,
        Container = 0x00000004,
        ReadOnly = 0x00000008,
        Advanced = 0x00000010,
        Reset = 0x00000020,
        OwnerReadOnly = 0x00000040,
        EditProperties = 0x00000080,
        Recurse = 0x00000100,
        NoAclProtect = 0x00000200,
        NoTreeApply = 0x00000400,
        PageTitle = 0x00000800,
        ServerIsDc = 0x00001000,
        ResetDaclTree = 0x00004000,
        ResetSaclTree = 0x00008000,
        ObjectGuid = 0x00010000,
        EditEffective = 0x00020000,
        ResetDacl = 0x00040000,
        ResetSacl = 0x00080000,
        ResetOwner = 0x00100000,
        NoAdditionalPermission = 0x00200000,
        ViewOnly = 0x00400000,
        PermsElevationRequired = 0x01000000,
        AuditsElevationRequired = 0x02000000,
        OwnerElevationRequested = 0x04000000,
        MayWrite = 0x10000000
    }

    public enum SiPageType : int
    {
        Perm,
        AdvPerm,
        Audit,
        Owner,
        Effective,
        TakeOwnership
    }

    [Flags]
    public enum SmtoFlags : int
    {
        Normal = 0x0,
        Block = 0x1,
        AbortIfHung = 0x2,
        NoTimeoutIfNotHung = 0x8,
        ErrorOnExit = 0x20
    }

    [Flags]
    public enum SnapshotFlags : uint
    {
        HeapList = 0x00000001,
        Process = 0x00000002,
        Thread = 0x00000004,
        Module = 0x00000008,
        Module32 = 0x00000010,
        Inherit = 0x80000000,
        All = 0x0000001f
    }

    [Flags]
    public enum StartupFlags : uint
    {
        UseShowWindow = 0x1,
        UseSize = 0x2,
        UsePosition = 0x4,
        UseCountChars = 0x8,
        UseFillAttribute = 0x10,
        RunFullScreen = 0x20,
        ForceOnFeedback = 0x40,
        ForceOffFeedback = 0x80,
        UseStdHandles = 0x100,
        UseHotkey = 0x200
    }

    [Flags]
    public enum SymbolFlags : int
    {
        ClrToken = 0x00040000,
        Constant = 0x00000100,
        Export = 0x00000200,
        Forwarder = 0x00000400,
        FrameRel = 0x00000020,
        Function = 0x00000800,
        IlRel = 0x00010000,
        Local = 0x00000080,
        Metadata = 0x00020000,
        Parameter = 0x00000040,
        Register = 0x00000008,
        RegRel = 0x00000010,
        Slot = 0x00008000,
        Thunk = 0x00002000,
        TlsRel = 0x00004000,
        ValuePresent = 0x00000001,
        Virtual = 0x00001000
    }

    [Flags]
    public enum SymbolOptions : uint
    {
        AllowAbsoluteSymbols = 0x00000800,
        AllowZeroAddress = 0x01000000,
        AutoPublics = 0x00010000,
        CaseInsensitive = 0x00000001,
        Debug = 0x80000000,
        DeferredLoads = 0x00000004,
        DisableSymSrvAutodetect = 0x02000000,
        ExactSymbols = 0x00000400,
        FailCriticalErrors = 0x00000200,
        FavorCompressed = 0x00800000,
        FlatDirectory = 0x00400000,
        IgnoreCvRec = 0x00000080,
        IgnoreImageDir = 0x00200000,
        IgnoreNtSymPath = 0x00001000,
        Include32BitModules = 0x00002000,
        LoadAnything = 0x00000040,
        LoadLines = 0x00000010,
        NoCpp = 0x00000008,
        NoImageSearch = 0x00020000,
        NoPrompts = 0x00080000,
        NoPublics = 0x00008000,
        NoUnqualifiedLoads = 0x00000100,
        Overwrite = 0x00100000,
        PublicsOnly = 0x00004000,
        Secure = 0x00040000,
        UndName = 0x00000002
    }

    [Flags]
    public enum SymbolServerOption
    {
        Callback = 0x01,
        Unattended = 0x20,
        ParentWin = 0x80,
    }

    public enum TcpConnectionOffloadState 
    {
        InHost = 0,
        Offloading = 1,
        Offloaded = 2,
        Uploading = 3,
        Max = 4 
    }

    public enum TcpTableClass : int
    {
        BasicListener,
        BasicConnections,
        BasicAll,
        OwnerPidListener,
        OwnerPidConnections,
        OwnerPidAll,
        OwnerModuleListener,
        OwnerModuleConnections,
        OwnerModuleAll
    }
    
    public enum UipiFilterFlag : uint
    {
        Add = 1,
        Remove = 2
    }

    public enum UdpTableClass : int
    {
        Basic,
        OwnerPid,
        OwnerModule
    }

    public enum WaitResult : uint
    {
        Object0 = 0x0,
        Abandoned = 0x80,
        Timeout = 0x102,
        Failed = 0xffffffff
    }

    [Flags]
    public enum Win32HandleFlags : int
    {
        Inherit = 0x1,
        ProtectFromClose = 0x2
    }

    public enum WindowMessage : uint
    {
        Null = 0x00,
        Create = 0x01,
        Destroy = 0x02,
        Move = 0x03,
        Size = 0x05,
        Activate = 0x06,
        SetFocus = 0x07,
        KillFocus = 0x08,
        Enable = 0x0a,
        SetRedraw = 0x0b,
        SetText = 0x0c,
        GetText = 0x0d,
        GetTextLength = 0x0e,
        Paint = 0x0f,
        Close = 0x10,
        QueryEndSession = 0x11,
        Quit = 0x12,
        QueryOpen = 0x13,
        EraseBkgnd = 0x14,
        SysColorChange = 0x15,
        EndSession = 0x16,
        SystemError = 0x17,
        ShowWindow = 0x18,
        CtlColor = 0x19,
        WinIniChange = 0x1a,
        SettingChange = 0x1a,
        DevModeChange = 0x1b,
        ActivateApp = 0x1c,
        FontChange = 0x1d,
        TimeChange = 0x1e,
        CancelMode = 0x1f,
        SetCursor = 0x20,
        MouseActivate = 0x21,
        ChildActivate = 0x22,
        QueueSync = 0x23,
        GetMinMaxInfo = 0x24,
        PaintIcon = 0x26,
        IconEraseBkgnd = 0x27,
        NextDlgCtl = 0x28,
        SpoolerStatus = 0x2a,
        DrawIcon = 0x2b,
        MeasureItem = 0x2c,
        DeleteItem = 0x2d,
        VKeyToItem = 0x2e,
        CharToItem = 0x2f,

        SetFont = 0x30,
        GetFont = 0x31,
        SetHotkey = 0x32,
        GetHotkey = 0x33,
        QueryDragIcon = 0x37,
        CompareItem = 0x39,
        Compacting = 0x41,
        WindowPosChanging = 0x46,
        WindowPosChanged = 0x47,
        Power = 0x48,
        CopyData = 0x4a,
        CancelJournal = 0x4b,
        Notify = 0x4e,
        InputLangChangeRequest = 0x50,
        InputLangChange = 0x51,
        TCard = 0x52,
        Help = 0x53,
        UserChanged = 0x54,
        NotifyFormat = 0x55,
        ContextMenu = 0x7b,
        StyleChanging = 0x7c,
        StyleChanged = 0x7d,
        DisplayChange = 0x7e,
        GetIcon = 0x7f,
        SetIcon = 0x80,

        NcCreate = 0x81,
        NcDestroy = 0x82,
        NcCalcSize = 0x83,
        NcHitTest = 0x84,
        NcPaint = 0x85,
        NcActivate = 0x86,
        GetDlgCode = 0x87,
        NcMouseMove = 0xa0,
        NcLButtonDown = 0xa1,
        NcLButtonUp = 0xa2,
        NcLButtonDblClk = 0xa3,
        NcRButtonDown = 0xa4,
        NcRButtonUp = 0xa5,
        NcRButtonDblClk = 0xa6,
        NcMButtonDown = 0xa7,
        NcMButtonUp = 0xa8,
        NcMButtonDblClk = 0xa9,

        KeyDown = 0x100,
        KeyUp = 0x101,
        Char = 0x102,
        DeadChar = 0x103,
        SysKeyDown = 0x104,
        SysKeyUp = 0x105,
        SysChar = 0x106,
        SysDeadChar = 0x107,

        ImeStartComposition = 0x10d,
        ImeEndComposition = 0x10e,
        ImeComposition = 0x10f,
        ImeKeyLast = 0x10f,

        InitDialog = 0x110,
        Command = 0x111,
        SysCommand = 0x112,
        Timer = 0x113,
        HScroll = 0x114,
        VScroll = 0x115,
        InitMenu = 0x116,
        InitMenuPopup = 0x117,
        MenuSelect = 0x11f,
        MenuChar = 0x120,
        EnterIdle = 0x121,

        CtlColorMsgBox = 0x132,
        CtlColorEdit = 0x133,
        CtlColorListBox = 0x134,
        CtlColorBtn = 0x135,
        CtlColorDlg = 0x136,
        CtlColorScrollbar = 0x137,
        CtlColorStatic = 0x138,

        MouseMove = 0x200,
        LButtonDown = 0x201,
        LButtonUp = 0x202,
        LButtonDblClk = 0x203,
        RButtonDown = 0x204,
        RButtonUp = 0x205,
        RButtonDblClk = 0x206,
        MButtonDown = 0x207,
        MButtonUp = 0x208,
        MButtonDblClk = 0x209,
        MouseWheel = 0x20a,

        ParentNotify = 0x210,
        EnterMenuLoop = 0x211,
        ExitMenuLoop = 0x212,
        NextMenu = 0x213,
        Sizing = 0x214,
        CaptureChanged = 0x215,
        Moving = 0x216,
        PowerBroadcast = 0x218,
        DeviceChange = 0x219,

        MdiCreate = 0x220,
        MdiDestroy = 0x221,
        MdiActivate = 0x222,
        MdiRestore = 0x223,
        MdiNext = 0x224,
        MdiMaximize = 0x225,
        MdiTile = 0x226,
        MdiCascade = 0x227,
        MdiIconArrange = 0x228,
        MdiGetActive = 0x229,
        MdiSetMenu = 0x230,
        EnterSizeMove = 0x231,
        ExitSizeMove = 0x232,
        DropFiles = 0x233,
        MdiRefreshMenu = 0x234,

        ImeSetContext = 0x281,
        ImeNotify = 0x282,
        ImeControl = 0x283,
        ImeCompositionFull = 0x284,
        ImeSelect = 0x285,
        ImeChar = 0x286,
        ImeKeyDown = 0x290,
        ImeKeyUp = 0x291,

        NcMouseHover = 0x2a0,
        MouseHover = 0x2a1,
        NcMouseLeave = 0x2a2,
        MouseLeave = 0x2a3,

        WtsSessionChange = 0x2b1,

        TabletFirst = 0x2c0,
        TabletLast = 0x2df,

        Cut = 0x300,
        Copy = 0x301,
        Paste = 0x302,
        Clear = 0x303,
        Undo = 0x304,

        RenderFormat = 0x305,
        RenderAllFormats = 0x306,
        DestroyClipboard = 0x307,
        DrawClipboard = 0x308,
        PaintClipboard = 0x309,
        VScrollClipboard = 0x30a,
        SizeClipboard = 0x30b,
        AskCbFormatName = 0x30c,
        ChangeCbChain = 0x30d,
        HScrollClipboard = 0x30e,
        QueryNewPalette = 0x30f,
        PaletteIsChanging = 0x310,
        PaletteChanged = 0x311,

        Hotkey = 0x312,
        Print = 0x317,
        PrintClient = 0x318,

        DwmSendIconicThumbnail = 0x323,
        DwmSendIconicLivePreviewBitmap = 0x326,

        HandheldFirst = 0x358,
        HandheldLast = 0x35f,
        PenWinFirst = 0x380,
        PenWinLast = 0x38f,
        CoalesceFirst = 0x390,
        CoalesceLast = 0x39f,
        DdeInitiate = 0x3e0,
        DdeTerminate = 0x3e1,
        DdeAdvise = 0x3e2,
        DdeUnadvise = 0x3e3,
        DdeAck = 0x3e4,
        DdeData = 0x3e5,
        DdeRequest = 0x3e6,
        DdePoke = 0x3e7,
        DdeExecute = 0x3e8,
        
        User = 0x400,

        Reflect = User + 0x1c00,

        BcmSetShield = 0x160c,

        App = 0x8000
    }

    [Flags]
    public enum WindowPlacementFlags : int
    {
        SetMinPosition = 0x1,
        RestoreToMaximized = 0x2,
        AsyncWindowPlacement = 0x4
    }

    public enum WindowStyles : uint
    {
        Overlapped = 0x00000000,
        Popup = 0x80000000,
        Child = 0x40000000,
        Minimize = 0x20000000,
        Visible = 0x10000000,
        Disabled = 0x08000000,
        ClipSiblings = 0x04000000,
        ClipChildren = 0x02000000,
        Maximize = 0x01000000,
        Caption = 0x00C00000, /* WindowStyles.Border | WindowStyles.DialogFrame */
        Border = 0x00800000,
        DialogFrame = 0x00400000,
        VerticalScroll = 0x00200000,
        HorizontalScroll = 0x00100000,
        SystemMenu = 0x00080000,
        ThickFrame = 0x00040000,
        Group = 0x00020000,
        TabStop = 0x00010000,
        MinimizeBox = 0x00020000,
        MaximizeBox = 0x00010000
    }

    [Flags]
    public enum WtdProvFlags : int
    {
        RevocationCheckNone = 0x10,
        RevocationCheckEndCert = 0x20,
        RevocationCheckChain = 0x40,
        RevocationCheckChainExcludeRoot = 0x80,
        Safer = 0x100,
        HashOnly = 0x200,
        UseDefaultOsVerCheck = 0x800,
        CacheOnlyUrlRetrieval = 0x1000
    }

    public enum WtdRevocationChecks : int
    {
        None = 0,
        WholeChain = 1
    }

    public enum WtdStateAction
    {
        Ignore = 0,
        Verify = 1,
        Close = 2,
        AutoCache = 3,
        AutoCacheFlush = 4
    }

    public enum WtsConnectStateClass : int
    {
        Active,
        Connected,
        ConnectQuery,
        Shadow,
        Disconnected,
        Idle,
        Listen,
        Reset,
        Down,
        Init
    }

    public enum WtsInformationClass : int
    {
        InitialProgram,
        ApplicationName,
        WorkingDirectory,
        OemId,
        SessionId,
        UserName,
        WinStationName,
        DomainName,
        ConnectState,
        ClientBuildNumber,
        ClientName,
        ClientDirectory,
        ClientProductId,
        ClientHardwareId,
        ClientAddress,
        ClientDisplay,
        ClientProtocolType,
        IdleTime,
        LogonTime,
        IncomingBytes,
        OutgoingBytes,
        IncomingFrames,
        OutgoingFrames
    }

    public enum WtsNotificationFlags : int
    {
        ThisSession = 0x0,
        AllSessions = 0x1
    }

    public enum WtsSessionChangeEvent : int
    {
        ConsoleConnect = 1,
        ConsoleDisconnect,
        RemoteConnect,
        RemoteDisconnect,
        SessionLogon,
        SessionLogoff,
        SessionLock,
        SessionUnlock,
        RemoteControl
    }

    public enum WtsShutdownFlags : int
    {
        Logoff = 0x1,
        Shutdown = 0x2,
        Reboot = 0x4,
        Poweroff = 0x8,
        FastReboot = 0x10
    }
}
