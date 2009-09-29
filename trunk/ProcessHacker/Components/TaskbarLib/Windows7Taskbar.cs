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
using System.Drawing;
using System.Runtime.InteropServices;
using TaskbarLib.Interop;
using ProcessHacker.Native;
using ProcessHacker;
using ProcessHacker.Native.Api;
using System.Windows.Forms;

namespace TaskbarLib
{
    /// <summary>
    /// The primary coordinator of the Windows 7 taskbar-related activities.
    /// For additional functionality, see the JumpListManager
    /// and ThumbButtonManager classes.
    /// </summary>
    public static class Windows7Taskbar
    {
        #region Infrastructure

        // Use thread static to work around COM threading issues.
        [ThreadStatic]
        private static ITaskbarList3 _taskbarList;
        internal static ITaskbarList3 TaskbarList
        {
            get
            {
                if (_taskbarList == null)
                {
                    _taskbarList = (ITaskbarList3)new CTaskbarList();
                    HResult result = _taskbarList.HrInit();

                    result.ThrowIf();
                }
                return _taskbarList;
            }
        }

        /// <summary>
        /// Creates a jump list manager for this form.
        /// </summary>
        /// <returns>An object of type <see cref="JumpListManager"/>
        /// that can be used to manage the application's jump list.</returns>
        public static JumpListManager CreateJumpListManager()
        {
            return new JumpListManager();
        }

        /// <summary>
        /// Creates a taskbar thumbnail button manager for this form.
        /// </summary>
        /// <returns>An object of type <see cref="ThumbButtonManager"/>
        /// that can be used to add and manage thumbnail toolbar buttons.</returns>
        public static ThumbButtonManager CreateThumbButtonManager()
        {
            return new ThumbButtonManager(Program.HackerWindowHandle);
        }

        private static IPropertyStore InternalGetWindowPropertyStore(IntPtr hwnd)
        {
            IPropertyStore propStore;
            int rc = UnsafeNativeMethods.SHGetPropertyStoreForWindow(hwnd, ref SafeNativeMethods.IID_IPropertyStore, out propStore);
            if (rc != 0)
                throw Marshal.GetExceptionForHR(rc);

            return propStore;
        }

        private static void InternalEnableCustomWindowPreview(IntPtr hwnd, bool enable)
        {
            int t = enable ? 1 : 0;

            int rc;
            rc = UnsafeNativeMethods.DwmSetWindowAttribute(hwnd, SafeNativeMethods.DWMWA_HAS_ICONIC_BITMAP, ref t, 4);
            if (rc != 0)
                throw Marshal.GetExceptionForHR(rc);

            rc = UnsafeNativeMethods.DwmSetWindowAttribute(hwnd, SafeNativeMethods.DWMWA_FORCE_ICONIC_REPRESENTATION, ref t, 4);
            if (rc != 0)
                throw Marshal.GetExceptionForHR(rc);
        }

        #endregion

        #region Application Id

        /// <summary>
        /// Gets Taskbar application id.
        /// </summary>
        /// <returns>The application id of that window.</returns>
        public static string GetAppId()
        {
            IPropertyStore propStore = InternalGetWindowPropertyStore(Program.HackerWindowHandle);

            PropVariant pv;
            HResult getValueResult = propStore.GetValue(ref PropertyKey.PKEY_AppUserModel_ID, out pv);
            getValueResult.ThrowIf();

            Marshal.ReleaseComObject(propStore);

            return pv.GetValue(); 
        }

        /// <summary>
        /// Sets the window's application id.
        /// </summary>
        /// <param name="appId">The application id.</param>
        public static void SetAppId(string appId)
        {
            IPropertyStore propStore = InternalGetWindowPropertyStore(Program.HackerWindowHandle);

            PropVariant pv = new PropVariant();
            pv.SetValue(appId);

            HResult setValueResult = propStore.SetValue(ref PropertyKey.PKEY_AppUserModel_ID, ref pv);
            setValueResult.ThrowIf();

            Marshal.ReleaseComObject(propStore);
        }

        /// <summary>
        /// Sets the current process' explicit application user model id.
        /// </summary>
        /// <param name="appId">The application id.</param>
        public static void SetCurrentProcessAppId(string appId)
        {
           HResult setProcessAppUserModeIDResult = UnsafeNativeMethods.SetCurrentProcessExplicitAppUserModelID(appId);
           setProcessAppUserModeIDResult.ThrowIf();
        }

        /// <summary>
        /// Gets the current process' explicit application user model id.
        /// </summary>
        /// <returns>The application id.</returns>
        public static string GetCurrentProcessAppId()
        {
            string appId;
            HResult getProcessAppUserModeIDResult = UnsafeNativeMethods.GetCurrentProcessExplicitAppUserModelID(out appId);
            getProcessAppUserModeIDResult.ThrowIf();
            return appId;
        }

        #endregion

        #region DWM Iconic Thumbnail and Peek Bitmap

        /// <summary>
        /// Indicates that the specified window requests the DWM
        /// to demand live preview (thumbnail and peek) mode when necessary
        /// instead of relying on default preview.
        /// </summary>
        public static void EnableCustomWindowPreview()
        {
            InternalEnableCustomWindowPreview(Program.HackerWindowHandle, true);
        }

        /// <summary>
        /// Indicates that the specified window does not require the DWM
        /// to demand live preview (thumbnail and peek) mode when necessary,
        /// i.e. this window relies on default preview.
        /// </summary>
        public static void DisableCustomWindowPreview()
        {
            InternalEnableCustomWindowPreview(Program.HackerWindowHandle, false);
        }

        /// <summary>
        /// Sets the specified iconic thumbnail for the specified window.
        /// This is typically done in response to a DWM message.
        /// </summary>
        /// <param name="bitmap">The thumbnail bitmap.</param>
        public static void SetIconicThumbnail(Bitmap bitmap)
        {
            int rc = UnsafeNativeMethods.DwmSetIconicThumbnail(Program.HackerWindowHandle, bitmap.GetHbitmap(), SafeNativeMethods.DWM_SIT_DISPLAYFRAME);
            if (rc != 0)
                throw Marshal.GetExceptionForHR(rc);
        }

        /// <summary>
        /// Sets the specified peek (live preview) bitmap for the specified
        /// window.  This is typically done in response to a DWM message.
        /// </summary>
        /// <param name="bitmap">The thumbnail bitmap.</param>
        /// <param name="displayFrame">Whether to display a standard window
        /// frame around the bitmap.</param>
        public static void SetPeekBitmap(Bitmap bitmap, bool displayFrame)
        {
            int rc = UnsafeNativeMethods.DwmSetIconicLivePreviewBitmap(Program.HackerWindowHandle, bitmap.GetHbitmap(), IntPtr.Zero, displayFrame ? SafeNativeMethods.DWM_SIT_DISPLAYFRAME : (uint)0);
            
            if (rc != 0)
                throw Marshal.GetExceptionForHR(rc);
        }
      
        /// <summary>
        /// Sets the specified peek (live preview) bitmap for the specified
        /// window.  This is typically done in response to a DWM message.
        /// </summary>
        /// <param name="hwnd">The window handle.</param>
        /// <param name="bitmap">The thumbnail bitmap.</param>
        /// <param name="offset">The client area offset at which to display
        /// the specified bitmap.  The rest of the parent window will be
        /// displayed as "remembered" by the DWM.</param>
        /// <param name="displayFrame">Whether to display a standard window
        /// frame around the bitmap.</param>
        public static void SetPeekBitmap(Bitmap bitmap, Point offset, bool displayFrame)
        {
            var nativePoint = new POINT(offset.X, offset.Y);
            int rc = UnsafeNativeMethods.DwmSetIconicLivePreviewBitmap(Program.HackerWindowHandle, bitmap.GetHbitmap(), ref nativePoint, displayFrame ? SafeNativeMethods.DWM_SIT_DISPLAYFRAME : (uint)0);
            if (rc != 0)
                throw Marshal.GetExceptionForHR(rc);
        }

        #endregion

        #region Taskbar Overlay Icon

        /// <summary>
        /// Draws the specified overlay icon on the specified window's
        /// taskbar button.
        /// </summary>
        /// <param name="icon">The overlay icon.</param>
        /// <param name="description">The overlay icon description.</param>
        public static void SetTaskbarOverlayIcon(Icon icon, string description)
        {
            HResult result = TaskbarList.SetOverlayIcon(
                Program.HackerWindowHandle, icon == null ? IntPtr.Zero : icon.Handle, description);
           
            result.ThrowIf();
        }

        public static void SetTaskbarOverlayIcon(this Form form, Icon icon, string description)
        {
            HResult result = TaskbarList.SetOverlayIcon(
                form.Handle, icon == null ? IntPtr.Zero : icon.Handle, description);

            result.ThrowIf();
        }

        #endregion

        #region Taskbar Progress Bar

        /// <summary>
        /// Sets the progress bar in the containing form's taskbar button
        /// to this progress bar's progress.
        /// </summary>
        /// <param name="progressBar">The progress bar.</param>
        public static void SetTaskbarProgress(this Form form, ProgressBar progressBar)
        {
            try
            {
                ulong maximum = Convert.ToUInt64(progressBar.Maximum);
                ulong progress = Convert.ToUInt64(progressBar.Value);

                HResult stateResult = TaskbarList.SetProgressState(form.Handle, (uint)ThumbnailProgressState.Normal);
                stateResult.ThrowIf();

                HResult valueResult = TaskbarList.SetProgressValue(form.Handle, progress, maximum);
                valueResult.ThrowIf();
            }
            catch (System.ObjectDisposedException)
            { /* This expcetion will occour when trying to access the form Handle upon FormClosing event */ }
        }

        /// <summary>
        /// Sets the progress bar in the containing form's taskbar button
        /// to this toolstrip progress bar's progress.
        /// </summary>
        /// <param name="progressBar">The progress bar.</param>
        public static void SetTaskbarProgress(this Form form, ToolStripProgressBar progressBar)
        {
            ulong maximum = Convert.ToUInt64(progressBar.Maximum);
            ulong progress = Convert.ToUInt64(progressBar.Value);

            HResult stateResult = TaskbarList.SetProgressState(form.Handle, (uint)ThumbnailProgressState.Normal);
            stateResult.ThrowIf();

            HResult valueResult = TaskbarList.SetProgressValue(form.Handle, progress, maximum);
            valueResult.ThrowIf();
        }

        /// <summary>
        /// Sets the progress state of the specified window's
        /// taskbar button.
        /// </summary>
        /// <param name="hwnd">The window handle.</param>
        /// <param name="state">The progress state.</param>
        public static void SetTaskbarProgressState(this Form form, ThumbnailProgressState state)
        {
            HResult result = TaskbarList.SetProgressState(form.Handle, (uint)state);
            result.ThrowIf();
        }

        /// <summary>
        /// Represents the thumbnail progress bar state.
        /// </summary>
        public enum ThumbnailProgressState
        {
            /// <summary>
            /// No progress is displayed.
            /// </summary>
            NoProgress = 0,
            /// <summary>
            /// The progress is indeterminate (marquee).
            /// </summary>
            Indeterminate = 0x1,
            /// <summary>
            /// Normal progress is displayed.
            /// </summary>
            Normal = 0x2,
            /// <summary>
            /// An error occurred (red).
            /// </summary>
            Error = 0x4,
            /// <summary>
            /// The operation is paused (yellow).
            /// </summary>
            Paused = 0x8
        }

        #endregion

        #region Taskbar Thumbnails

        /// <summary>
        /// Specifies that only a portion of the window's client area
        /// should be used in the window's thumbnail.
        /// </summary>
        /// <param name="hwnd">The window.</param>
        /// <param name="clipRect">The rectangle that specifies the clipped region.</param>
        private static void SetThumbnailClip(this Form form, Rectangle clipRect)
        {
            RECT rect = new RECT(clipRect.Left, clipRect.Top, clipRect.Right, clipRect.Bottom);
            HResult result = TaskbarList.SetThumbnailClip(form.Handle, ref rect);
        }

        /// <summary>
        /// Sets the specified window's thumbnail tooltip.
        /// </summary>
        /// <param name="hwnd">The window.</param>
        /// <param name="tooltip">The tooltip text.</param>
        private static void SetThumbnailTooltip(this Form form, string tooltip)
        {
            HResult result = TaskbarList.SetThumbnailTooltip(form.Handle, tooltip);
        }

        #endregion

        #region Miscellaneous

        /// <summary>
        /// Allow the taskbar and DWM-related windows messages
        /// through the Windows UIPI mechanism.  
        /// Calling this method is not required unless the process is elevated.
        /// </summary>
        public static void AllowWindowMessagesThroughUipi()
        {
            // If it's Windows 7 or above and we're elevated.
            if (OSVersion.HasTaskDialogs && Program.ElevationType == TokenElevationType.Full)
            {
                Win32.ChangeWindowMessageFilter((WindowMessage)UnsafeNativeMethods.WM_TaskbarButtonCreated, UipiFilterFlag.Add);
                Win32.ChangeWindowMessageFilter(WindowMessage.DwmSendIconicThumbnail, UipiFilterFlag.Add);
                Win32.ChangeWindowMessageFilter(WindowMessage.DwmSendIconicLivePreviewBitmap, UipiFilterFlag.Add);
                Win32.ChangeWindowMessageFilter(WindowMessage.Command, UipiFilterFlag.Add);
                Win32.ChangeWindowMessageFilter(WindowMessage.SysCommand, UipiFilterFlag.Add);
                Win32.ChangeWindowMessageFilter(WindowMessage.Activate, UipiFilterFlag.Add);
            }
        }

        /// <summary>
        /// The WM_TaskbarButtonCreated message number.
        /// </summary>
        public static uint TaskbarButtonCreatedMessage
        {
            get { return UnsafeNativeMethods.WM_TaskbarButtonCreated; }
        }

        #endregion

    }
}