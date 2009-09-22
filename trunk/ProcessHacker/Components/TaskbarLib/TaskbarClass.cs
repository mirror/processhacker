using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using ProcessHacker;
using ProcessHacker.Native;
using ProcessHacker.Native.Api;
using TaskbarLib;
using System.Runtime.InteropServices;

namespace TaskbarLib
{
    public class TaskbarClass
    {
        #region Native

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
            void SetThumbnailTooltip(IntPtr hwnd, [MarshalAs(UnmanagedType.LPWStr)] string pszTip);

            [PreserveSig]
            void SetThumbnailClip(IntPtr hwnd, ref Rect prcClip);

            #endregion

            #region "ITaskbarList4"

            void SetTabProperties(IntPtr hwndTab, STPFLAG stpFlags);

            #endregion
        }

        [Flags()]
        public enum TBATFLAG
        {
            USEMDITHUMBNAIL = 1,
            USEMDILIVEPREVIEW = 2
        }
        [Flags()]
        public enum TBPFlag
        {
            NOPROGRESS = 0,
            INDETERMINATE = 1,
            NORMAL = 2,
            ERROR = 4,
            PAUSED = 8
        }
        [Flags()]
        public enum STPFLAG
        {
            NONE = 0,
            USEAPPTHUMBNAILALWAYS = 1,
            USEAPPTHUMBNAILWHENACTIVE = 2,
            USEAPPPEEKALWAYS = 4,
            USEAPPPEEKWHENACTIVE = 8,
        }
        [Flags()]
        public enum THBMASK
        {
            THB_BITMAP = 0x1,
            THB_ICON = 0x2,
            THB_TOOLTIP = 0x4,
            THB_FLAGS = 0x8
        }

        [Flags()]
        public enum THBFLAGS
        {
            THBF_ENABLED = 0x00000000,
            THBF_DISABLED = 0x00000001,
            THBF_DISMISSONCLICK = 0x00000002,
            THBF_NOBACKGROUND = 0x00000004,
            THBF_HIDDEN = 0x00000008,
            THBF_NONINTERACTIVE = 0x00000010
        }

        [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Auto)]
        public struct THUMBBUTTON
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

        #endregion

        private static object _taskbarInstanceLock = new object();
        private static ITaskbarList _taskbar = null;

        private static ITaskbarList Taskbar
        {
            get
            {
                if (_taskbar == null)
                {
                    lock (_taskbarInstanceLock)
                    {
                        if (_taskbar == null)
                        {
                            _taskbar = (ITaskbarList)new TaskbarList();
                            _taskbar.HrInit();
                        }
                    }
                }

                return _taskbar;
            }
        }

        public static void ActivateTab(IntPtr hwnd)
        {
            Taskbar.ActivateTab(hwnd);
        }

        public static void AddTab(IntPtr hwnd)
        {
            Taskbar.AddTab(hwnd);
        }

        public static void DeleteTab(IntPtr hwnd)
        {
            Taskbar.DeleteTab(hwnd);
        }

        public static void SetActivateAlt(IntPtr hwnd)
        {
            Taskbar.SetActiveAlt(hwnd);
        }

        public static void MarkFullscreenWindow(IntPtr hwnd, bool fullscreen)
        {
            Taskbar.MarkFullscreenWindow(hwnd, fullscreen);
        }

        public static void SetProgressValue(ulong completed, ulong total)
        {
            if (OSVersion.HasExtendedTaskbar)
                Taskbar.SetProgressValue(Program.HackerWindowHandle, completed, total);
        }

        public static void SetProgressState(TBPFlag flags)
        {
            if (OSVersion.HasExtendedTaskbar)
                Taskbar.SetProgressState(Program.HackerWindowHandle, flags);
        }

        public static void RegisterTab(IntPtr hwndTab, IntPtr hwndMDI)
        {
            if (OSVersion.HasExtendedTaskbar)
                Taskbar.RegisterTab(hwndTab, hwndMDI);
        }

        public static void UnregisterTab(IntPtr hwndTab)
        {
            if (OSVersion.HasExtendedTaskbar)
                Taskbar.UnregisterTab(hwndTab);
        }

        public static void SetTabOrder(IntPtr hwndTab, IntPtr hwndInsertBefore)
        {
            if (OSVersion.HasExtendedTaskbar)
                Taskbar.SetTabOrder(hwndTab, hwndInsertBefore);
        }

        public static void SetTabActive(IntPtr hwndTab, IntPtr hwndInsertBefore, TBATFLAG tbatFlags)
        {
            if (OSVersion.HasExtendedTaskbar)
                Taskbar.SetTabActive(hwndTab, hwndInsertBefore, tbatFlags);
        }

        public static void ThumbBarAddButtons(IntPtr hwnd, uint cButtons, THUMBBUTTON[] pButton)
        {
            if (OSVersion.HasExtendedTaskbar)
            {
                HRESULT result;

                result = Taskbar.ThumbBarAddButtons(hwnd, cButtons, pButton);

                if (Failed(result))
                    throw Marshal.GetExceptionForHR((int)result);
            }
        }

        public static void ThumbBarUpdateButtons(IntPtr hwnd, uint cButtons, THUMBBUTTON[] pButton)
        {
            if (OSVersion.HasExtendedTaskbar)
            {
                HRESULT result;

                result = Taskbar.ThumbBarUpdateButtons(hwnd, cButtons, pButton);

                if (Failed(result))
                    throw Marshal.GetExceptionForHR((int)result);
            }
        }

        public static void ThumbBarSetImageList(IntPtr hwnd, IntPtr himl)
        {
            if (OSVersion.HasExtendedTaskbar)
                Taskbar.ThumbBarSetImageList(hwnd, himl);
        }

        public static void SetOverlayIcon(Icon icon, string pszDescription)
        {
            if (OSVersion.HasExtendedTaskbar)
                Taskbar.SetOverlayIcon(Program.HackerWindowHandle, icon != null ? icon.Handle : IntPtr.Zero, pszDescription);
        }

        public static void SetThumbnailTooltip(IntPtr hwnd, string pszTip)
        {
            if (OSVersion.HasExtendedTaskbar)
                Taskbar.SetThumbnailTooltip(hwnd, pszTip);
        }

        public static void SetThumbnailClip(IntPtr hwnd, ref Rect prcClip)
        {
            if (OSVersion.HasExtendedTaskbar)
                Taskbar.SetThumbnailClip(hwnd, ref prcClip);
        }

        /// <summary>
        /// HRESULT - Succeeded
        /// </summary>
        /// <param name="hresult">The error code.</param>
        /// <returns>True if the error code indicates success.</returns>
        public static bool Succeeded(HRESULT hresult)
        {
            return ((int)hresult >= 0);
        }

        /// <summary>
        /// HRESULT - Failed
        /// </summary>
        /// <param name="hResult">The error code.</param>
        /// <returns>True if the error code indicates failure.</returns>
        public static bool Failed(HRESULT hResult)
        {
            return ((int)hResult < 0);
        }
    }
}
