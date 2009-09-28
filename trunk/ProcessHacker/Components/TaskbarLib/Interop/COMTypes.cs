using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Runtime.CompilerServices;

namespace TaskbarLib.Interop
{
    #region "Interface Classes"

    [ComImportAttribute()]
    [GuidAttribute("86C14003-4D6B-4EF3-A7B4-0506663B2E68")]
    [ClassInterfaceAttribute(ClassInterfaceType.None)]
    internal class CApplicationDestinations { }

    [ComImportAttribute()]
    [GuidAttribute("86BEC222-30F2-47E0-9F25-60D11CD75C28")]
    [ClassInterfaceAttribute(ClassInterfaceType.None)]
    internal class CApplicationDocumentLists { }

    [ComImportAttribute()]
    [GuidAttribute("56FDF344-FD6D-11d0-958A-006097C9A090")]
    [ClassInterfaceAttribute(ClassInterfaceType.None)]
    internal class CTaskbarList { }

    [ComImportAttribute()]
    [GuidAttribute("00021401-0000-0000-C000-000000000046")]
    [ClassInterfaceAttribute(ClassInterfaceType.None)]
    internal class CShellLink { }

    [ComImportAttribute()]
    [GuidAttribute("77F10CF0-3DB5-4966-B520-B7C54FD35ED6")]
    [ClassInterfaceAttribute(ClassInterfaceType.None)]
    internal class CDestinationList { }

    [ComImportAttribute()]
    [GuidAttribute("2D3468C1-36A7-43B6-AC24-D3F02FD9607A")]
    [ClassInterfaceAttribute(ClassInterfaceType.None)]
    internal class CEnumerableObjectCollection { }

    #endregion

    #region "Interfaces"
 
    [ComImportAttribute()]
    [GuidAttribute("92CA9DCD-5622-4BBA-A805-5E9F541BD8C9")]
    [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IObjectArray
    {
        void GetCount(out uint cObjects);
        void GetAt(uint iIndex, ref Guid riid, [Out(), MarshalAs(UnmanagedType.Interface)] out object ppvObject);
    }

    [ComImportAttribute()]
    [GuidAttribute(SafeNativeMethods.IID_IObjectCollection)]
    [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IObjectCollection
    {
        // IObjectArray
        [PreserveSig]
        void GetCount(out uint cObjects);
        [PreserveSig]
        void GetAt(uint iIndex, ref Guid riid, [Out(), MarshalAs(UnmanagedType.Interface)] out object ppvObject);

        // IObjectCollection
        void AddObject([MarshalAs(UnmanagedType.Interface)] object pvObject);
        void AddFromArray([MarshalAs(UnmanagedType.Interface)] IObjectArray poaSource);
        void RemoveObject(uint uiIndex);
        void Clear();
    }

    [ComImportAttribute()]
    [GuidAttribute("886d8eeb-8cf2-4446-8d02-cdba1dbdcf99")]
    [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IPropertyStore
    {
        void GetCount(out UInt32 cProps);
        void GetAt(UInt32 iProp, [MarshalAs(UnmanagedType.Struct)] out PropertyKey pkey);
        void GetValue([In, MarshalAs(UnmanagedType.Struct)] ref PropertyKey pkey, [Out(), MarshalAs(UnmanagedType.Struct)] out PropVariant pv);
        void SetValue([In, MarshalAs(UnmanagedType.Struct)] ref PropertyKey pkey, [In, MarshalAs(UnmanagedType.Struct)] ref PropVariant pv);
        void Commit();
    }

    [ComImportAttribute()]
    [GuidAttribute("6332DEBF-87B5-4670-90C0-5E57B408A49E")]
    [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ICustomDestinationList
    {
        void SetAppID([MarshalAs(UnmanagedType.LPWStr)] string pszAppID);
        void BeginList(out uint cMaxSlots, ref Guid riid, [Out(), MarshalAs(UnmanagedType.Interface)] out object ppvObject);
        void AppendCategory([MarshalAs(UnmanagedType.LPWStr)] string pszCategory, [MarshalAs(UnmanagedType.Interface)] IObjectArray poa);
        void AppendKnownCategory([MarshalAs(UnmanagedType.I4)] KnownDestCategory category);
        void AddUserTasks([MarshalAs(UnmanagedType.Interface)] IObjectArray poa);
        void CommitList();
        void GetRemovedDestinations(ref Guid riid,            [Out(), MarshalAs(UnmanagedType.Interface)] out object ppvObject);
        void DeleteList([MarshalAs(UnmanagedType.LPWStr)] string pszAppID);
        void AbortList();
    }

    [ComImportAttribute()]
    [GuidAttribute("12337D35-94C6-48A0-BCE7-6A9C69D4D600")]
    [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IApplicationDestinations
    {
        void SetAppID([MarshalAs(UnmanagedType.LPWStr)] string pszAppID);
        void RemoveDestination([MarshalAs(UnmanagedType.Interface)] object pvObject);
        void RemoveAllDestinations();
    }

    [ComImportAttribute()]
    [GuidAttribute("3C594F9F-9F30-47A1-979A-C9E83D3D0A06")]
    [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IApplicationDocumentLists
    {
        void SetAppID([MarshalAs(UnmanagedType.LPWStr)] string pszAppID);
        void GetList([MarshalAs(UnmanagedType.I4)] AppDocListType listtype, uint cItemsDesired, ref Guid riid, [Out(), MarshalAs(UnmanagedType.Interface)] out object ppvObject);
    }

    [ComImportAttribute()]
    [GuidAttribute("ea1afb91-9e28-4b86-90e9-9e9f8a5eefaf")]
    [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ITaskbarList3
    {
        // ITaskbarList
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

        // ITaskbarList2
        [PreserveSig]
        void MarkFullscreenWindow(IntPtr hwnd, [MarshalAs(UnmanagedType.Bool)] bool fFullscreen);

        // ITaskbarList3
        void SetProgressValue(IntPtr hwnd, UInt64 ullCompleted, UInt64 ullTotal);
        void SetProgressState(IntPtr hwnd, uint tbpFlags);
        void RegisterTab(IntPtr hwndTab, IntPtr hwndMDI);
        void UnregisterTab(IntPtr hwndTab);
        void SetTabOrder(IntPtr hwndTab, IntPtr hwndInsertBefore);
        void SetTabActive(IntPtr hwndTab, IntPtr hwndMDI, TBATFLAG tbatFlags);
        void ThumbBarAddButtons(IntPtr hwnd, uint cButtons, [MarshalAs(UnmanagedType.LPArray)] THUMBBUTTON[] pButtons);
        void ThumbBarUpdateButtons(IntPtr hwnd, uint cButtons, [MarshalAs(UnmanagedType.LPArray)] THUMBBUTTON[] pButtons);
        void ThumbBarSetImageList(IntPtr hwnd, IntPtr himl);
        void SetOverlayIcon(IntPtr hwnd, IntPtr hIcon, [MarshalAs(UnmanagedType.LPWStr)] string pszDescription);
        void SetThumbnailTooltip(IntPtr hwnd, [MarshalAs(UnmanagedType.LPWStr)] string pszTip);
        void SetThumbnailClip(IntPtr hwnd, ref RECT prcClip);
    }

    [ComImportAttribute()]
    [GuidAttribute("43826D1E-E718-42EE-BC55-A1E261C37BFE")]
    [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IShellItem
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void BindToHandler([In, MarshalAs(UnmanagedType.Interface)] IntPtr pbc, [In] ref Guid bhid, [In] ref Guid riid, out IntPtr ppv);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetParent([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetDisplayName([In] SIGDN sigdnName, [MarshalAs(UnmanagedType.LPWStr)] out string ppszName);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetAttributes([In] uint sfgaoMask, out uint psfgaoAttribs);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Compare([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi, [In] uint hint, out int piOrder);
    }

    [ComImportAttribute()]
    [GuidAttribute("000214F9-0000-0000-C000-000000000046")]
    [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IShellLinkW
    {
        void GetPath([Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile, int cchMaxPath, IntPtr pfd, uint fFlags);
        void GetIDList(out IntPtr ppidl);
        void SetIDList(IntPtr pidl);
        void GetDescription([Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile, int cchMaxName);
        void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string pszName);
        void GetWorkingDirectory([Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszDir, int cchMaxPath);
        void SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string pszDir);
        void GetArguments([Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszArgs, int cchMaxPath);
        void SetArguments([MarshalAs(UnmanagedType.LPWStr)] string pszArgs);
        void GetHotKey(out short wHotKey);
        void SetHotKey(short wHotKey);
        void GetShowCmd(out uint iShowCmd);
        void SetShowCmd(uint iShowCmd);
        void GetIconLocation([Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszIconPath, int cchIconPath, out int iIcon);
        void SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string pszIconPath, int iIcon);
        void SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string pszPathRel, uint dwReserved);
        void Resolve(IntPtr hwnd, uint fFlags);
        void SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);
    }

    #endregion
}