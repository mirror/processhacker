namespace TaskbarLib
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [GuidAttribute("56FDF344-FD6D-11d0-958A-006097C9A090")]
    [ClassInterfaceAttribute(ClassInterfaceType.None)]
    [ComImportAttribute()]
    internal class TaskbarList 
    { }

    /// <summary>
    /// ITaskbarList COM Interface
    /// </summary>
    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("C43DC798-95D1-4BEA-9030-BB99E2983A1A")]
    internal interface ITaskbarList
    {
        #region "ITaskbarList"

        /// <summary>
        /// Initializes the taskbar list object. 
        /// This method must be called before any other ITaskbarList methods can be called. 
        /// </summary>
        [PreserveSig]
        void HrInit();
        
        [PreserveSig]
        void AddTab(IntPtr hwnd);
        
        [PreserveSig]
        void DeleteTab(IntPtr hwnd);
       
        [PreserveSig]
        void ActivateTab(IntPtr hwnd);
       
        [PreserveSig]
        void SetActiveAlt(IntPtr hwnd);

        #endregion

        #region "ITaskbarList2"

        [PreserveSig]
        void MarkFullscreenWindow(IntPtr hwnd, [MarshalAs(UnmanagedType.Bool)] bool fFullscreen);

        #endregion

        #region "ITaskbarList3"

        [PreserveSig]
        void SetProgressValue(IntPtr hwnd, UInt64 ullCompleted, UInt64 ullTotal);
        
        [PreserveSig]
        void SetProgressState(IntPtr hwnd, TBPFlag tbpFlags);
        
        [PreserveSig]
        void RegisterTab(IntPtr hwndTab, IntPtr hwndMDI);
        
        [PreserveSig]
        void UnregisterTab(IntPtr hwndTab);
        
        [PreserveSig]
        void SetTabOrder(IntPtr hwndTab, IntPtr hwndInsertBefore);
        
        [PreserveSig]
        void SetTabActive(IntPtr hwndTab, IntPtr hwndInsertBefore, TBATFLAG dwReserved);
        
        [PreserveSig]
        HRESULT ThumbBarAddButtons(IntPtr hwnd, uint cButtons, [MarshalAs(UnmanagedType.LPArray)] THUMBBUTTON[] pButtons);
        
        [PreserveSig]
        HRESULT ThumbBarUpdateButtons(IntPtr hwnd, uint cButtons, [MarshalAs(UnmanagedType.LPArray)] THUMBBUTTON[] pButtons);
       
        [PreserveSig]
        void ThumbBarSetImageList(IntPtr hwnd, IntPtr himl);

        [PreserveSig]
        void SetOverlayIcon(IntPtr hwnd, IntPtr hIcon, [MarshalAs(UnmanagedType.LPWStr)] string pszDescription);

        [PreserveSig]
        void SetThumbnailTooltip(IntPtr hwnd,[MarshalAs(UnmanagedType.LPWStr)] string pszTip);

        [PreserveSig]
        void SetThumbnailClip(IntPtr hwnd, ref RECT prcClip);

        #endregion

        #region "ITaskbarList4"
        
        void SetTabProperties(IntPtr hwndTab, STPFLAG stpFlags);

        #endregion
    }

    [Flags()]
    internal enum TBATFLAG
    {
        USEMDITHUMBNAIL = 1,
        USEMDILIVEPREVIEW = 2
    }
    [Flags()]
    internal enum TBPFlag
    {     
        NOPROGRESS = 0,
        INDETERMINATE = 1,
        NORMAL = 2,
        ERROR = 4,
        PAUSED = 8
    }
    [Flags()]
    internal enum STPFLAG
    {
        NONE = 0,
        USEAPPTHUMBNAILALWAYS = 1,
        USEAPPTHUMBNAILWHENACTIVE = 2,
        USEAPPPEEKALWAYS = 4,
        USEAPPPEEKWHENACTIVE = 8,
    }
    [Flags()]
    internal enum THBMASK
    {
        THB_BITMAP = 0x1,
        THB_ICON = 0x2,
        THB_TOOLTIP = 0x4,
        THB_FLAGS = 0x8
    }
    [Flags()]
    internal enum THBFLAGS
    {
        THBF_ENABLED = 0x00000000,
        THBF_DISABLED = 0x00000001,
        THBF_DISMISSONCLICK = 0x00000002,
        THBF_NOBACKGROUND = 0x00000004,
        THBF_HIDDEN = 0x00000008,
        THBF_NONINTERACTIVE = 0x00000010
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Auto)]
    internal struct THUMBBUTTON
    {
        //WPARAM value for a THUMBBUTTON being clicked.
        internal const int THBN_CLICKED = 0x1800;

        [MarshalAs(UnmanagedType.U4)]
        internal THBMASK dwMask;
        internal uint iId;
        internal uint iBitmap;
        internal IntPtr hIcon;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        internal string szTip;
        [MarshalAs(UnmanagedType.U4)]
        internal THBFLAGS dwFlags;
    }

    /// <summary>
    /// HRESULT Wrapper
    /// </summary>
    public enum HRESULT : uint
    {
        S_FALSE = 0x0001,
        S_OK = 0x0000,

        E_INVALIDARG = 0x80070057,
        E_OUTOFMEMORY = 0x8007000E, 
        E_NOINTERFACE = 0x80004002,
        E_FAIL = 0x80004005,
        E_ELEMENTNOTFOUND = 0x80070490,

        TYPE_E_ELEMENTNOTFOUND = 0x8002802B,
        NO_OBJECT = 0x800401E5,
        ERROR_CANCELLED = 1223,
        RESOURCE_IN_USE = 0x800700AA,
    }
}