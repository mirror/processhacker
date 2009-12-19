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

/* This file contains structure declarations for the Win32 API.
 * 
 * All structures which do not belong in any other category 
 * are placed in this file.
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
    public struct CertContext
    {
        public int CertEncodingType;
        public IntPtr CertEncoded;
        public int CertEncodedSize;
        public IntPtr CertInfo; // CertInfo*
        public IntPtr CertStore;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CertInfo
    {
        public int Version;
        public CryptoApiBlob SerialNumber;
        public CryptAlgorithmIdentifier SignatureAlgorithm;
        public CryptoApiBlob Issuer;
        public Filetime NotBefore;
        public Filetime NotAfter;
        public CryptoApiBlob Subject;

        // More...
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CMsgSignerInfo
    {
        public int Version;
        public CryptoApiBlob Issuer;
        public CryptoApiBlob SerialNumber;
        public CryptAlgorithmIdentifier HashAlgorithm;
        public CryptAlgorithmIdentifier HashEncryptionAlgorithm;
        public CryptoApiBlob EncryptedHash;
        public CryptAttributes AuthAttrs;
        public CryptAttributes UnauthAttrs;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct CredUiInfo
    {
        public int Size;
        public IntPtr Parent;
        public string MessageText;
        public string CaptionText;
        public IntPtr Banner;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct CryptAlgorithmIdentifier
    {
        public string ObjId;
        public CryptoApiBlob Parameters;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CryptAttributes
    {
        public int Count;
        public IntPtr Attributes; // CryptAttribute*
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CryptoApiBlob
    {
        public int Size;
        public IntPtr Data;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CryptProviderCert
    {
        public int Size;
        public IntPtr Cert;
        public bool Commercial;
        public bool TrustedRoot;
        public bool SelfSigned;
        public bool TestCert;
        public int RevokedReason;
        public int Confidence;
        public int Error;
        public IntPtr TrustListContext; // CtlContext*
        public bool TrustListSignerCert;
        public IntPtr CtlContext; // CtlContext*
        public int CtlError;
        public bool IsCyclic;
        public IntPtr ChainElement; // CertChainElement*
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CryptProviderSgnr
    {
        public int Size;
        public Filetime VerifyAsOf;
        public int CertChainCount;
        public IntPtr CertChain; // CryptProviderCert*
        public int SignerType;
        public IntPtr Signer; // CMsgSignerInfo*
        public int Error;
        public int CounterSignersCount;
        public IntPtr CounterSigners; // CryptProviderSgnr*
        public IntPtr ChainContext;
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
    public struct Filetime
    {
        public int LowDateTime;
        public int HighDateTime;
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
        public IntPtr hHandle;
        public IntPtr dwAddress;
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
        public IntPtr Key;
        public int LineNumber;
        public string FileName;
        public ulong Address;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct INet6Address
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        [FieldOffset(0)]
        public byte[] Bytes;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        [FieldOffset(0)]
        public ushort[] Words;
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
        public long KiUserExceptionDispatcher;
        public long StackBase;
        public long StackLimit;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public long[] Reserved;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct LoadedImage
    {
        public IntPtr ModuleName;
        public IntPtr FileHandle;
        public IntPtr MappedAddress;
        public IntPtr FileHeader; // ImageNtHeaders32*
        public IntPtr LastRvaSection; // ImageSectionHeader*
        public int NumberOfSections;
        public IntPtr Sections; // ImageSectionHeader*
        public int Characteristics;
        [MarshalAs(UnmanagedType.U1)]
        public bool SystemImage;
        [MarshalAs(UnmanagedType.U1)]
        public bool DosImage;
        [MarshalAs(UnmanagedType.U1)]
        public bool ReadOnly;
        public byte Version;
        public ListEntry Links;
        public int SizeOfImage;
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
        public IntPtr RegionSize;
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
    public struct MibTcp6Row 
    {
        public MibTcpState State;
        public uint LocalAddress;
        public uint LocalScopeId;
        public int LocalPort;
        public uint RemoteAddr;
        public int RemoteScopeId;
        public int RemotePort;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MibTcp6Row2
    {
        public INet6Address LocalAddr;
        public uint LocalScopeId;
        public int LocalPort;
        public INet6Address RemoteAddr;
        public uint RemoteScopeId;
        public int RemotePort;
        public MibTcpState State;
        public int OwningPid;
        public TcpConnectionOffloadState OffloadState;
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

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct MibTcp6RowOwnerPid
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] LocalAddress;
        public uint LocalScopeId;
        public int LocalPort;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] RemoteAddress;
        public uint RemoteScopeId;
        public int RemotePort;
        public MibTcpState State;
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
    public struct MibTcp6Table
    {
        public int NumEntries;
        public MibTcp6Row[] Table;
    }
        
    [StructLayout(LayoutKind.Sequential)]
    public struct MibTcpTableOwnerPid
    {
        public int NumEntries;
        public MibTcpRowOwnerPid[] Table;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MibTcp6TableOwnerPid
    {
        public int NumEntries;
        public MibTcp6RowOwnerPid[] Table;
    }
    
    [StructLayout(LayoutKind.Sequential)]
    public struct MibUdpRow
    {
        public uint LocalAddress;
        public int LocalPort;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MibUdp6Row
    {
        public INet6Address LocalAddress;
        public uint LocalScopeId;
        public int LocalPort;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MibUdpTable
    {
        public uint NumEntries;
        public MibUdpRow[] Table;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MibUdp6Table
    {
        public int NumEntries;
        public MibUdp6Row[] Table;
    }
 
    [StructLayout(LayoutKind.Sequential)]
    public struct MibUdpRowOwnerPid
    {
        public uint LocalAddress;
        public int LocalPort;
        public int OwningProcessId;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct MibUdp6RowOwnerPid
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] LocalAddress;
        public uint LocalScopeId;
        public int LocalPort;
        public int OwningProcessId;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct MibUdp6RowOwnerModule
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] LocalAddress;
        public uint LocalScopeId;
        public int LocalPort;
        public int OwningProcessId;
        public long CreateTimestamp;
        public int Flags;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public long[] OwningModuleInfo;
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
    public struct MibUdp6TableOwnerPid
    {
        public int NumEntries;
        public MibUdp6RowOwnerPid[] Table;
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
        public IntPtr CommitTotal;
        public IntPtr CommitLimit;
        public IntPtr CommitPeak;
        public IntPtr PhysicalTotal;
        public IntPtr PhysicalAvailable;
        public IntPtr SystemCache;
        public IntPtr KernelTotal;
        public IntPtr KernelPaged;
        public IntPtr KernelNonPaged;
        public IntPtr PageSize;
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
        public IntPtr ProcessHandle;
        public IntPtr ThreadHandle;
        public int ProcessId;
        public int ThreadId;
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

        public Rectangle ToRectangle()
        {
            return Rectangle.FromLTRB(this.Left, this.Top, this.Right, this.Bottom);
        }

        public Rect(int left, int top, int right, int bottom)
        {
            this.Left = left;
            this.Top = top;
            this.Right = right;
            this.Bottom = bottom;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ScAction
    {
        public ScActionType Type;
        public int Delay;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct SecPkgInfo
    {
        public int Capabilities;
        public ushort Version;
        public ushort RpcId;
        public int MaxToken;
        public string Name;
        public string Comment;
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

        public IntPtr FuncTableEntry;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public long[] Params;

        public int Far;
        public int Virtual;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public long[] Reserved;

        public KdHelp64 KdHelp;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct StartupInfo
    {
        public int Size;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string Reserved;
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
        public short ShowWindow;
        public short Reserved2;
        public IntPtr Reserved3;
        public IntPtr StdInputHandle;
        public IntPtr StdOutputHandle;
        public IntPtr StdErrorHandle;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SymbolInfo
    {
        public static readonly int NameOffset = Marshal.OffsetOf(typeof(SymbolInfo), "Name").ToInt32();

        public int SizeOfStruct;
        public int TypeIndex;
        public unsafe fixed long Reserved[2];
        public int Index;
        public int Size;
        public ulong ModBase;
        public SymbolFlags Flags;
        public long Value;
        public ulong Address;
        public int Register;
        public int Scope;
        public int Tag;
        public int NameLen;
        public int MaxNameLen;
        public byte Name;
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
        public WndProcDelegate WindowsProc;
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

    [StructLayout(LayoutKind.Sequential)]
    public struct WindowPlacement
    {
        public int Length;
        public WindowPlacementFlags Flags;
        public ShowWindowType ShowState;
        public Point MinPosition;
        public Point MaxPosition;
        public Rect NormalPosition;
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
        public WtdRevocationChecks RevocationChecks;
        public int UnionChoice;
        public IntPtr UnionData;
        public WtdStateAction StateAction;
        public IntPtr StateData;
        public IntPtr URLReference;
        public WtdProvFlags ProvFlags;
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
