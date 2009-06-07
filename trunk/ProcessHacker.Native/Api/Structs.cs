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
    public struct Rect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ScAction
    {
        public ScActionType Type;
        public int Delay;
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

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct SiAccess
    {
        public IntPtr Guid;
        public int Mask;
        public IntPtr Name; // string
        public SiAccessFlags Flags;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct SiInheritType
    {
        public IntPtr Guid;
        public int Flags;
        public IntPtr Name; // string
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct SiObjectInfo
    {
        public SiObjectInfoFlags Flags;
        public IntPtr Instance;
        public IntPtr ServerName; // string
        public IntPtr ObjectName; // string
        public IntPtr PageTitle; // string
        public Guid ObjectType;
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
    public struct WtsClientAddress
    {
        public int AddressFamily;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public byte[] Address;
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
        public WtsConnectStateClass State;
    }
}
