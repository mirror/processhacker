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
using System.Security;
using System.Text;
using ProcessHacker.Native.Api;

namespace TaskbarLib.Interop
{
    [Flags]
    internal enum KnownDestCategory
    {
        FREQUENT = 1,
        RECENT
    }
   
    [Flags]
    internal enum AppDocListType
    {
        ADLT_RECENT = 0,
        ADLT_FREQUENT
    }
        
    [Flags]
    internal enum Tbatflag
    {
        TBATF_USEMDITHUMBNAIL = 0x1,
        TBATF_USEMDILIVEPREVIEW = 0x2
    }
  
    [Flags]
    internal enum ThumbnailButtonMask
    {
        Bitmap = 0x1,
        Icon = 0x2,
        Tooltip = 0x4,
        Flags = 0x8
    }
   
    [Flags]
    internal enum ThumbnailButtonFlags
    {
        ENABLED = 0,
        DISABLED = 0x1,
        DISMISSONCLICK = 0x2,
        NOBACKGROUND = 0x4,
        HIDDEN = 0x8
    }

    [Flags]
    internal enum SIGDN : uint
    {
        SIGDN_NORMALDISPLAY = 0x00000000,           // SHGDN_NORMAL
        SIGDN_PARENTRELATIVEPARSING = 0x80018001,   // SHGDN_INFOLDER | SHGDN_FORPARSING
        SIGDN_DESKTOPABSOLUTEPARSING = 0x80028000,  // SHGDN_FORPARSING
        SIGDN_PARENTRELATIVEEDITING = 0x80031001,   // SHGDN_INFOLDER | SHGDN_FOREDITING
        SIGDN_DESKTOPABSOLUTEEDITING = 0x8004c000,  // SHGDN_FORPARSING | SHGDN_FORADDRESSBAR
        SIGDN_FILESYSPATH = 0x80058000,             // SHGDN_FORPARSING
        SIGDN_URL = 0x80068000,                     // SHGDN_FORPARSING
        SIGDN_PARENTRELATIVEFORADDRESSBAR = 0x8007c001,     // SHGDN_INFOLDER | SHGDN_FORPARSING | SHGDN_FORADDRESSBAR
        SIGDN_PARENTRELATIVE = 0x80080001           // SHGDN_INFOLDER
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct POINT
    {
        internal int X;
        internal int Y;

        internal POINT(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal struct THUMBBUTTON
    {
        [MarshalAs(UnmanagedType.U4)]
        public ThumbnailButtonMask dwMask;
        public int iId;
        public int iBitmap;
        public IntPtr hIcon;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szTip;
        [MarshalAs(UnmanagedType.U4)]
        public ThumbnailButtonFlags dwFlags;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct PropertyKey
    {
        public Guid fmtid;
        public uint pid;

        public PropertyKey(Guid fmtid, uint pid)
        {
            this.fmtid = fmtid;
            this.pid = pid;
        }

        public static PropertyKey PKEY_Title = new PropertyKey(new Guid("F29F85E0-4FF9-1068-AB91-08002B27B3D9"), 2);
        public static PropertyKey PKEY_AppUserModel_ID = new PropertyKey(new Guid("9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3"), 5);
        public static PropertyKey PKEY_AppUserModel_IsDestListSeparator = new PropertyKey(new Guid("9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3"), 6);
        public static PropertyKey PKEY_AppUserModel_RelaunchCommand = new PropertyKey(new Guid("9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3"), 2);
        public static PropertyKey PKEY_AppUserModel_RelaunchDisplayNameResource = new PropertyKey(new Guid("9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3"), 4);
        public static PropertyKey PKEY_AppUserModel_RelaunchIconResource = new PropertyKey(new Guid("9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3"), 3);
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct CALPWSTR
    {
        [FieldOffset(0)]
        internal uint cElems;
        [FieldOffset(4)]
        internal IntPtr pElems;
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct PropVariant : IDisposable
    {
        [FieldOffset(0)]
        private ushort vt;
        [FieldOffset(8)]
        private IntPtr pointerValue;
        [FieldOffset(8)]
        private byte byteValue;
        [FieldOffset(8)]
        private long longValue;
        [FieldOffset(8)]
        private short boolValue;
        [MarshalAs(UnmanagedType.Struct)]
        [FieldOffset(8)]
        private CALPWSTR calpwstr;

        public VarEnum VarType
        {
            get { return (VarEnum)vt; }
        }

        public void SetValue(String val)
        {
            this.Clear();
            this.vt = (ushort)VarEnum.VT_LPWSTR;
            this.pointerValue = Marshal.StringToCoTaskMemUni(val);
        }

        public void SetValue(bool val)
        {
            this.Clear();
            this.vt = (ushort)VarEnum.VT_BOOL;
            this.boolValue = val ? (short)-1 : (short)0;
        }

        public string GetValue()
        {
            return Marshal.PtrToStringUni(this.pointerValue);
        }

        public void Clear()
        {
           HResult clearResult = UnsafeNativeMethods.PropVariantClear(ref this);
           clearResult.ThrowIf();
        }

        public void Dispose()
        {
            Marshal.FreeCoTaskMem(this.pointerValue);
        }
    }

    [SuppressUnmanagedCodeSecurity]
    internal static class SafeNativeMethods
    {
        //Obviously, these GUIDs shouldn't be modified.  The reason they
        //are not readonly is that they are passed with 'ref' to various
        //native methods.
        public static Guid IID_IObjectArray = new Guid("92CA9DCD-5622-4BBA-A805-5E9F541BD8C9");
        public const string IID_IObjectCollection = "5632B1A4-E38A-400A-928A-D4CD63230295";
        public static Guid IID_IPropertyStore = new Guid("886D8EEB-8CF2-4446-8D02-CDBA1DBDCF99");
        public static Guid IID_IUnknown = new Guid("00000000-0000-0000-C000-000000000046");

        public const int DWM_SIT_DISPLAYFRAME = 0x00000001;
        public const int DWMWA_FORCE_ICONIC_REPRESENTATION = 7;
        public const int DWMWA_HAS_ICONIC_BITMAP = 10;

        public const int WA_ACTIVE = 1;
        public const int WA_CLICKACTIVE = 2;

        public const int SC_CLOSE = 0xF060;

        // Thumbbutton WM_COMMAND notification
        public const uint THBN_CLICKED = 0x1800;
    }

    [SuppressUnmanagedCodeSecurity]
    internal static class UnsafeNativeMethods
    {
        public static readonly uint WM_TaskbarButtonCreated = RegisterWindowMessage("TaskbarButtonCreated");

        [DllImport("ole32.dll")]
        public static extern HResult PropVariantClear(ref PropVariant pvar);

        [DllImport("dwmapi.dll")]
        public static extern HResult DwmSetIconicThumbnail(IntPtr hwnd, IntPtr hbitmap, uint flags);
       
        [DllImport("dwmapi.dll")]
        public static extern HResult DwmSetIconicLivePreviewBitmap(IntPtr hwnd, IntPtr hbitmap, ref POINT ptClient, uint flags);
       
        [DllImport("dwmapi.dll")]
        public static extern HResult DwmSetIconicLivePreviewBitmap(IntPtr hwnd, IntPtr hbitmap, IntPtr ptClient, uint flags);
       
        [DllImport("dwmapi.dll")]
        internal static extern HResult DwmSetWindowAttribute(IntPtr hwnd, uint dwAttributeToSet, ref int pvAttributeValue, uint cbAttribute);
      
        [DllImport("dwmapi.dll")]
        public static extern HResult DwmInvalidateIconicBitmaps(IntPtr hwnd);
       
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern uint RegisterWindowMessage(string lpString);

        [DllImport("shell32.dll")]
        public static extern HResult SetCurrentProcessExplicitAppUserModelID([MarshalAs(UnmanagedType.LPWStr)] string AppID);
       
        [DllImport("shell32.dll")]
        public static extern HResult GetCurrentProcessExplicitAppUserModelID([Out(), MarshalAs(UnmanagedType.LPWStr)] out string AppID);

        [DllImport("shell32.dll")]
        public static extern HResult SHGetPropertyStoreForWindow(IntPtr hwnd, ref Guid iid /*IID_IPropertyStore*/, [Out(), MarshalAs(UnmanagedType.Interface)] out IPropertyStore propertyStore);
       
        [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern HResult SHCreateItemFromParsingName(string path, /* The following parameter is not used - binding context. */ IntPtr pbc, ref Guid riid, [MarshalAs(UnmanagedType.Interface)] out IShellItem shellItem);
    }
}