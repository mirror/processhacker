/*
 * Process Hacker - 
 *   ProcessHacker Taskbar Extensions
 * 
 * Copyright (C) 2009 dmex
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
 * 
 */

using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Runtime.CompilerServices;
using ProcessHacker.Native.Api;

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
        [PreserveSig]
        HResult GetCount(out uint cObjects);
        [PreserveSig]
        HResult GetAt(uint iIndex, ref Guid riid, [Out(), MarshalAs(UnmanagedType.Interface)] out object ppvObject);
    }

    [ComImportAttribute()]
    [GuidAttribute(SafeNativeMethods.IID_IObjectCollection)]
    [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IObjectCollection
    {
        // IObjectArray  
        [PreserveSig]
        HResult GetCount(out uint cObjects);
        [PreserveSig]
        HResult GetAt(uint iIndex, ref Guid riid, [Out(), MarshalAs(UnmanagedType.Interface)] out object ppvObject);

        // IObjectCollection
        [PreserveSig]
        HResult AddObject([MarshalAs(UnmanagedType.Interface)] object pvObject);
        [PreserveSig]
        HResult AddFromArray([MarshalAs(UnmanagedType.Interface)] IObjectArray poaSource);
        [PreserveSig]
        HResult RemoveObject(uint uiIndex);
        [PreserveSig]
        HResult Clear();
    }

    [ComImportAttribute()]
    [GuidAttribute("886d8eeb-8cf2-4446-8d02-cdba1dbdcf99")]
    [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IPropertyStore
    {
        [PreserveSig]
        HResult GetCount(out UInt32 cProps);
        [PreserveSig]
        HResult GetAt(UInt32 iProp, [MarshalAs(UnmanagedType.Struct)] out PropertyKey pkey);
        [PreserveSig]
        HResult GetValue([In, MarshalAs(UnmanagedType.Struct)] ref PropertyKey pkey, [Out(), MarshalAs(UnmanagedType.Struct)] out PropVariant pv);
        [PreserveSig]
        HResult SetValue([In, MarshalAs(UnmanagedType.Struct)] ref PropertyKey pkey, [In, MarshalAs(UnmanagedType.Struct)] ref PropVariant pv);
        [PreserveSig]
        HResult Commit();
    }

    [ComImportAttribute()]
    [GuidAttribute("6332DEBF-87B5-4670-90C0-5E57B408A49E")]
    [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ICustomDestinationList
    {
        [PreserveSig]
        HResult SetAppID([MarshalAs(UnmanagedType.LPWStr)] string pszAppID);
        [PreserveSig]
        HResult BeginList(out uint cMaxSlots, ref Guid riid, [Out(), MarshalAs(UnmanagedType.Interface)] out object ppvObject);
        [PreserveSig]
        HResult AppendCategory([MarshalAs(UnmanagedType.LPWStr)] string pszCategory, [MarshalAs(UnmanagedType.Interface)] IObjectArray poa);
        HResult AppendKnownCategory([MarshalAs(UnmanagedType.I4)] KnownDestCategory category);
        [PreserveSig]
        HResult AddUserTasks([MarshalAs(UnmanagedType.Interface)] IObjectArray poa);
        [PreserveSig]
        HResult CommitList();
        [PreserveSig]
        HResult GetRemovedDestinations(ref Guid riid, [Out(), MarshalAs(UnmanagedType.Interface)] out object ppvObject);
        [PreserveSig]
        HResult DeleteList([MarshalAs(UnmanagedType.LPWStr)] string pszAppID);
        [PreserveSig]
        HResult AbortList();
    }

    [ComImportAttribute()]
    [GuidAttribute("12337D35-94C6-48A0-BCE7-6A9C69D4D600")]
    [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IApplicationDestinations
    {
        [PreserveSig]
        HResult SetAppID([MarshalAs(UnmanagedType.LPWStr)] string pszAppID);
        [PreserveSig]
        HResult RemoveDestination([MarshalAs(UnmanagedType.Interface)] object pvObject);
        [PreserveSig]
        HResult RemoveAllDestinations();
    }

    [ComImportAttribute()]
    [GuidAttribute("3C594F9F-9F30-47A1-979A-C9E83D3D0A06")]
    [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IApplicationDocumentLists
    {
        [PreserveSig]
        HResult SetAppID([MarshalAs(UnmanagedType.LPWStr)] string pszAppID);
        [PreserveSig]
        HResult GetList([MarshalAs(UnmanagedType.I4)] AppDocListType listtype, uint cItemsDesired, ref Guid riid, [Out(), MarshalAs(UnmanagedType.Interface)] out object ppvObject);
    }

    [ComImportAttribute()]
    [GuidAttribute("ea1afb91-9e28-4b86-90e9-9e9f8a5eefaf")]
    [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ITaskbarList3
    {
        // ITaskbarList
        [PreserveSig]
        HResult HrInit();
        [PreserveSig]
        HResult AddTab(IntPtr hwnd);
        [PreserveSig]
        HResult DeleteTab(IntPtr hwnd);
        [PreserveSig]
        HResult ActivateTab(IntPtr hwnd);
        [PreserveSig]
        HResult SetActiveAlt(IntPtr hwnd);

        // ITaskbarList2
        [PreserveSig]
        HResult MarkFullscreenWindow(IntPtr hwnd, [MarshalAs(UnmanagedType.Bool)] bool fFullscreen);

        // ITaskbarList3
        [PreserveSig]
        HResult SetProgressValue(IntPtr hwnd, UInt64 ullCompleted, UInt64 ullTotal);
        [PreserveSig]
        HResult SetProgressState(IntPtr hwnd, uint tbpFlags);
        [PreserveSig]
        HResult RegisterTab(IntPtr hwndTab, IntPtr hwndMDI);
        [PreserveSig]
        HResult UnregisterTab(IntPtr hwndTab);
        [PreserveSig]
        HResult SetTabOrder(IntPtr hwndTab, IntPtr hwndInsertBefore);
        [PreserveSig]
        HResult SetTabActive(IntPtr hwndTab, IntPtr hwndMDI, Tbatflag tbatFlags);
        [PreserveSig]
        HResult ThumbBarAddButtons(IntPtr hwnd, int cButtons, [MarshalAs(UnmanagedType.LPArray)] THUMBBUTTON[] pButtons);
        [PreserveSig]
        HResult ThumbBarUpdateButtons(IntPtr hwnd, int cButtons, [MarshalAs(UnmanagedType.LPArray)] THUMBBUTTON[] pButtons);
        [PreserveSig]
        HResult ThumbBarSetImageList(IntPtr hwnd, IntPtr himl);
        [PreserveSig]
        HResult SetOverlayIcon(IntPtr hwnd, IntPtr hIcon, [MarshalAs(UnmanagedType.LPWStr)] string pszDescription);
        [PreserveSig]
        HResult SetThumbnailTooltip(IntPtr hwnd, [MarshalAs(UnmanagedType.LPWStr)] string pszTip);
        [PreserveSig]
        HResult SetThumbnailClip(IntPtr hwnd, ref Rect prcClip);
    }

    [ComImportAttribute()]
    [GuidAttribute("43826D1E-E718-42EE-BC55-A1E261C37BFE")]
    [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IShellItem
    {
        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [PreserveSig]
        HResult BindToHandler([In, MarshalAs(UnmanagedType.Interface)] IntPtr pbc, [In] ref Guid bhid, [In] ref Guid riid, out IntPtr ppv);

        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [PreserveSig]
        HResult GetParent([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] 
        [PreserveSig]
        HResult GetDisplayName([In] SIGDN sigdnName, [MarshalAs(UnmanagedType.LPWStr)] out string ppszName);

        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [PreserveSig]
        HResult GetAttributes([In] uint sfgaoMask, out uint psfgaoAttribs);

        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [PreserveSig]
        HResult Compare([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi, [In] uint hint, out int piOrder);
    }

    [ComImportAttribute()]
    [GuidAttribute("000214F9-0000-0000-C000-000000000046")]
    [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IShellLinkW
    {
        [PreserveSig]
        HResult GetPath([Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile, int cchMaxPath, IntPtr pfd, uint fFlags);
        [PreserveSig]
        HResult GetIDList(out IntPtr ppidl);
        [PreserveSig]
        HResult SetIDList(IntPtr pidl);
        [PreserveSig]
        HResult GetDescription([Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile, int cchMaxName);
        [PreserveSig]
        HResult SetDescription([MarshalAs(UnmanagedType.LPWStr)] string pszName);
        [PreserveSig]
        HResult GetWorkingDirectory([Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszDir, int cchMaxPath);
        [PreserveSig]
        HResult SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string pszDir);
        [PreserveSig]
        HResult GetArguments([Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszArgs, int cchMaxPath);
        [PreserveSig]
        HResult SetArguments([MarshalAs(UnmanagedType.LPWStr)] string pszArgs);
        [PreserveSig]
        HResult GetHotKey(out short wHotKey);
        [PreserveSig]
        HResult SetHotKey(short wHotKey);
        [PreserveSig]
        HResult GetShowCmd(out uint iShowCmd);
        [PreserveSig]
        HResult SetShowCmd(uint iShowCmd);
        [PreserveSig]
        HResult GetIconLocation([Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszIconPath, int cchIconPath, out int iIcon);
        [PreserveSig]
        HResult SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string pszIconPath, int iIcon);
        [PreserveSig]
        HResult SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string pszPathRel, uint dwReserved);
        [PreserveSig]
        HResult Resolve(IntPtr hwnd, uint fFlags);
        [PreserveSig]
        HResult SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);
    }

    #endregion
}