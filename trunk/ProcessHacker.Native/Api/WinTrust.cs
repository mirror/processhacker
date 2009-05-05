namespace Security.WinTrust
{
    using System;
    using System.Runtime.InteropServices;

    #region WinTrustData struct field enums
    public enum WinTrustDataUIChoice : uint
    {
        All = 1,
        None = 2,
        NoBad = 3,
        NoGood = 4
    }

    public enum WinTrustDataRevocationChecks : uint
    {
        None = 0x00000000,
        WholeChain = 0x00000001
    }

    public enum WinTrustDataChoice : uint
    {
        File = 1,
        Catalog = 2,
        Blob = 3,
        Signer = 4,
        Certificate = 5
    }

    public enum WinTrustDataStateAction : uint
    {
        Ignore = 0x00000000,
        Verify = 0x00000001,
        Close = 0x00000002,
        AutoCache = 0x00000003,
        AutoCacheFlush = 0x00000004
    }

    [FlagsAttribute]
    public enum WinTrustDataProvFlags : uint
    {
        UseIe4TrustFlag = 0x00000001,
        NoIe4ChainFlag = 0x00000002,
        NoPolicyUsageFlag = 0x00000004,
        RevocationCheckNone = 0x00000010,
        RevocationCheckEndCert = 0x00000020,
        RevocationCheckChain = 0x00000040,
        RevocationCheckChainExcludeRoot = 0x00000080,
        SaferFlag = 0x00000100,
        HashOnlyFlag = 0x00000200,
        UseDefaultOsverCheck = 0x00000400,
        LifetimeSigningFlag = 0x00000800,
        CacheOnlyUrlRetrieval = 0x00001000      // affects CRL retrieval and AIA retrieval
    }

    public enum WinTrustDataUIContext : uint
    {
        Execute = 0,
        Install = 1
    }
    #endregion

    #region WinTrust structures

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public class WinTrustCatalogInfo
    {
        private int _size;
        private int _catalogVersion;
        private IntPtr _catalogFilePath;
        private IntPtr _memberTag;
        private IntPtr _memberFilePath;
        private int _memberFile;
        private byte[] _calculatedFileHash;
        private int _calculatedFileHashSize;
        private int _catalogContext;

        public WinTrustCatalogInfo()
        {

        }

        ~WinTrustCatalogInfo()
        {
            Marshal.FreeCoTaskMem(_catalogFilePath);
            Marshal.FreeCoTaskMem(_memberTag);
            Marshal.FreeCoTaskMem(_memberFilePath);
        }

        public int Size
        {
            set { _size = value; }
            get { return _size; }
        }
        public int CatalogVersion
        {
            set { _catalogVersion = value; }
            get { return _catalogVersion; }
        }
        public string CatalogFilePath
        {
            set { _catalogFilePath = Marshal.StringToCoTaskMemAuto(value); ; }
            get { return Marshal.PtrToStringAuto(_catalogFilePath); }
        }
        public string MemberTag
        {
            set { _memberTag = Marshal.StringToCoTaskMemAuto(value); }
            get { return Marshal.PtrToStringAuto(_memberTag); }
        }
        public string MemberFilePath
        {
            set { _memberFilePath = Marshal.StringToCoTaskMemAuto(value); }
            get { return Marshal.PtrToStringAuto(_memberFilePath); }
        }
        public int MemberFile
        {
            set { _memberFile = value; }
            get { return _memberFile; }
        }
        public byte[] CalculatedFileHash
        {
            set { _calculatedFileHash = value; }
            get { return _calculatedFileHash; }
        }
        public int CalculatedFileHashSize
        {
            set { _calculatedFileHashSize = value; }
            get { return _calculatedFileHashSize; }
        }
        public int CatalogContext
        {
            set { _catalogContext = value; }
            get { return _catalogContext; }
        }

    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public class CatalogInfo
    {
        private int _size;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        private string _catalogFile;

        public CatalogInfo()
        {
            _size = 0;
        }
        
        public int Size
        {
            set { _size = value; }
            get { return _size; }
        }
        public string CatalogFile
        {
            set { _catalogFile = value; }
            get { return _catalogFile; }
        }

    }

    
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public class WinTrustFileInfo
    {
        private UInt32 _structSize;
        private IntPtr _pszFilePath;
        private IntPtr _hFile;
        private IntPtr _pgKnownSubject;

        public WinTrustFileInfo()
        { 
            _structSize = (UInt32)Marshal.SizeOf(typeof(WinTrustFileInfo));
            _pszFilePath = IntPtr.Zero;
            _hFile = IntPtr.Zero;
            _pgKnownSubject = IntPtr.Zero;
        }

        public WinTrustFileInfo(String filePath)
        {
            _structSize = (UInt32)Marshal.SizeOf(typeof(WinTrustFileInfo));
            _hFile = IntPtr.Zero;
            _pgKnownSubject = IntPtr.Zero;
            _pszFilePath = Marshal.StringToCoTaskMemUni(filePath);
        }
        ~WinTrustFileInfo()
        {
        }

        public UInt32 StructSize
        {
            set { _structSize = value; }
            get { return _structSize; } 
        }
        public String pszFilePath
        {
            set { _pszFilePath = Marshal.StringToCoTaskMemUni(value); }
            get { return Marshal.PtrToStringUni(_pszFilePath); } 
        }
        public IntPtr hFile
        {
            set { _hFile = value; }
            get { return _hFile; } 
        }
        public IntPtr pgKnownSubject
        {
            set { _pgKnownSubject = value; }
            get { return _pgKnownSubject; }
        }
    }


    
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public class WinTrustData
    {
        private UInt32 _structSize;
        private IntPtr _policyCallbackData;
        private IntPtr _sIPClientData;
        // required: UI choice
        private WinTrustDataUIChoice _uIChoice;
        // required: certificate revocation check options
        private WinTrustDataRevocationChecks _revocationChecks;
        // required: which structure is being passed in?
        private WinTrustDataChoice _unionChoice;
        // individual file
        private IntPtr _unionData;
        private WinTrustDataStateAction _stateAction;
        private IntPtr _stateData;
        private String _uRLReference;
        private WinTrustDataProvFlags _provFlags;
        private WinTrustDataUIContext _uIContext;

        public WinTrustData()
        { 
             _structSize = (UInt32)Marshal.SizeOf(typeof(WinTrustData));
             _policyCallbackData = IntPtr.Zero;
             _sIPClientData = IntPtr.Zero;
             _uIChoice = WinTrustDataUIChoice.None;
             _revocationChecks = WinTrustDataRevocationChecks.None;
             _unionChoice = WinTrustDataChoice.File;
             _unionData = IntPtr.Zero;
             _stateAction = WinTrustDataStateAction.Ignore;
             _stateData = IntPtr.Zero;
             _uRLReference = null;
             _provFlags = WinTrustDataProvFlags.SaferFlag;
             _uIContext = WinTrustDataUIContext.Execute;
        }
        public WinTrustData(String _fileName)
        {
            _structSize = (UInt32)Marshal.SizeOf(typeof(WinTrustData));
            _policyCallbackData = IntPtr.Zero;
            _sIPClientData = IntPtr.Zero;
            _uIChoice = WinTrustDataUIChoice.None;
            _revocationChecks = WinTrustDataRevocationChecks.None;
            _unionChoice = WinTrustDataChoice.File;

            WinTrustFileInfo wtfiData = new WinTrustFileInfo(_fileName);
            _unionData = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(WinTrustFileInfo)));
            Marshal.StructureToPtr(wtfiData, _unionData, false);
                        
            _stateAction = WinTrustDataStateAction.Ignore;
            _stateData = IntPtr.Zero;
            _uRLReference = null;
            _provFlags = WinTrustDataProvFlags.SaferFlag;
            _uIContext = WinTrustDataUIContext.Execute;

        }
        ~WinTrustData()
        {
            Marshal.FreeCoTaskMem(_unionData);
        }

        public UInt32 StructSize
        {
            set { _structSize = value; }
            get { return _structSize; }
        }
        public IntPtr PolicyCallbackData
        {
            set { _policyCallbackData = value; }
            get { return _policyCallbackData; }
        }
        public IntPtr SIPClientData
        {
            set { _sIPClientData = value; }
            get { return _sIPClientData; }
        }
        public WinTrustDataUIChoice UIChoice
        {
            set { _uIChoice = value; }
            get { return _uIChoice; }
        }
        public WinTrustDataRevocationChecks RevocationChecks
        {
            set { _revocationChecks = value; }
            get { return _revocationChecks; }
        }
        public WinTrustDataChoice UnionChoice
        {
            set { _unionChoice = value; }
            get { return _unionChoice; }
        }
        public Object UnionData
        {
            set 
            {
                WinTrustFileInfo wtfiData = value as WinTrustFileInfo;
                if (wtfiData != null)
                {
                    _unionData = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(WinTrustFileInfo)));
                    Marshal.StructureToPtr(wtfiData, _unionData, false);
                    return;
                }

                WinTrustCatalogInfo wtci = value as WinTrustCatalogInfo;
                if (wtci != null)
                {
                    _unionData = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(WinTrustCatalogInfo)));
                    Marshal.StructureToPtr(wtci, _unionData, false);
                    return;
                }

                _unionData = (IntPtr)value;
            }
        }
        public WinTrustDataStateAction StateAction
        {
            set { _stateAction = value; }
            get { return _stateAction; }
        }
        public IntPtr StateData
        {
            set { _stateData = value; }
            get { return _stateData; }
        }
        public String URLReference
        {
            set { _uRLReference = value; }
            get { return _uRLReference; }
        }
        public WinTrustDataProvFlags ProvFlags
        {
            set { _provFlags = value; }
            get { return _provFlags; }
        }
        public WinTrustDataUIContext UIContext
        {
            set { _uIContext = value; }
            get { return _uIContext; }
        }
    }
    #endregion

    public enum WinVerifyTrustResult : uint
    {
        Success = 0,
        ProviderUnknown = 0x800b0001,           // The trust provider is not recognized on this system
        ActionUnknown = 0x800b0002,             // The trust provider does not support the specified action
        SubjectFormUnknown = 0x800b0003,        // The trust provider does not support the form specified for the subject
        SubjectNotTrusted = 0x800b0004,          // The subject failed the specified verification action
        //NoSignature = 0x800b0100,
        //Expired = 0x800b0101,
        //Revoked = 0x800b010c,
        //Distrust = 0x800b0111,
        //SecuritySettings = 0x80092026
    }
}