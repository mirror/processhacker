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

using System;
using System.Runtime.InteropServices;

namespace ProcessHacker.Native.Api
{
    public enum AddressMode : int
    {
        AddrMode1616,
        AddrMode1632,
        AddrModeReal,
        AddrModeFlat
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
    public enum CreationFlags : uint
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

    [Flags]
    public enum HeapEntry32Flags : int
    {
        Fixed = 0x00000001,
        Free = 0x00000002,
        Moveable = 0x00000004
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

    public enum MachineType : uint
    {
        I386 = 0x014c,
        Ia64 = 0x0200,
        Amd64 = 0x8664
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
        Listening,
        SynSent,
        SynReceived,
        Established,
        FinWait1,
        FinWait2,
        CloseWait,
        Closing,
        LastAck,
        TimeWait,
        DeleteTcb
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

    public enum PeekMessageFlags : int
    {
        NoRemove = 0,
        Remove = 1,
        NoYield = 2,
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

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct ProfileInformation
    {
        public int Size;
        public int Flags;
        public string UserName;
        public string ProfilePath;
        public string DefaultPath;
        public string ServerName;
        public string PolicyPath;
        public int ProfileHandle;
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

    public enum ShowWindowType : uint
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

    public enum SiObjectInfoFlags : int
    {
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
        MayWrite = 0x10000000,
        All = EditPerms | EditOwner | EditAudits
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

    public enum SymbolServerOption
    {
        Callback = 0x01,
        Unattended = 0x20,
        ParentWin = 0x80,
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

        BcmSetShield = 0x160c,

        App = 0x8000
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
    public enum WtProvFlags : int
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

    public enum WtRevocationChecks : int
    {
        None = 0,
        WholeChain = 1
    }

    public enum WtStateAction
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
