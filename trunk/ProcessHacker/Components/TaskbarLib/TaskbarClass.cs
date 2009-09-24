/*
 * Process Hacker - 
 *   Windows 7 taskbar extensions class
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
 */

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
        // Make this thread static because COM is not thread-safe and throws 
        // nasty exceptions if we try to use objects created on a different 
        // thread.
        [ThreadStatic]
        private static TaskbarNative.ITaskbarList _taskbar = null;

        private static TaskbarNative.ITaskbarList Taskbar
        {
            get
            {
                if (_taskbar == null)
                {
                    _taskbar = (TaskbarNative.ITaskbarList)new TaskbarNative.TaskbarList();
                    HResult result = _taskbar.HrInit();

                    result.ThrowIf();
                }

                return _taskbar;
            }
        }

        public static void ActivateTab(IntPtr hwnd)
        {
            HResult result = Taskbar.ActivateTab(hwnd);

            result.ThrowIf();
        }

        public static void AddTab(IntPtr hwnd)
        {
            HResult result = Taskbar.AddTab(hwnd);

            result.ThrowIf();
        }

        public static void DeleteTab(IntPtr hwnd)
        {
            HResult result = Taskbar.DeleteTab(hwnd);

            result.ThrowIf();
        }

        public static void SetActivateAlt(IntPtr hwnd)
        {
            HResult result = Taskbar.SetActiveAlt(hwnd);

            result.ThrowIf();
        }

        public static void MarkFullscreenWindow(IntPtr hwnd, bool fullscreen)
        {
            HResult result = Taskbar.MarkFullscreenWindow(hwnd, fullscreen);

            result.ThrowIf();
        }

        public static void SetProgressValue(ulong completed, ulong total)
        {
            if (OSVersion.HasExtendedTaskbar)
            {
                HResult result = Taskbar.SetProgressValue(Program.HackerWindowHandle, completed, total);

                result.ThrowIf();
            }
        }

        public static void SetProgressState(TaskbarNative.TBPFlag flags)
        {
            if (OSVersion.HasExtendedTaskbar)
            {
                HResult result = Taskbar.SetProgressState(Program.HackerWindowHandle, flags);

                result.ThrowIf();
            }
        }

        public static void RegisterTab(IntPtr hwndTab, IntPtr hwndMDI)
        {
            if (OSVersion.HasExtendedTaskbar)
            {
                HResult result = Taskbar.RegisterTab(hwndTab, hwndMDI);

                result.ThrowIf();
            }
        }

        public static void UnregisterTab(IntPtr hwndTab)
        {
            if (OSVersion.HasExtendedTaskbar)
            {
                HResult result = Taskbar.UnregisterTab(hwndTab);

                result.ThrowIf();
            }
        }

        public static void SetTabOrder(IntPtr hwndTab, IntPtr hwndInsertBefore)
        {
            if (OSVersion.HasExtendedTaskbar)
            {
                HResult result = Taskbar.SetTabOrder(hwndTab, hwndInsertBefore);

                result.ThrowIf();
            }
        }

        public static void SetTabActive(IntPtr hwndTab, IntPtr hwndInsertBefore, TaskbarNative.TBATFlag tbatFlags)
        {
            if (OSVersion.HasExtendedTaskbar)
            {
                HResult result = Taskbar.SetTabActive(hwndTab, hwndInsertBefore, tbatFlags);

                result.ThrowIf();
            }
        }

        public static void ThumbBarAddButtons(IntPtr hwnd, uint cButtons, TaskbarNative.THUMBBUTTON[] pButton)
        {
            if (OSVersion.HasExtendedTaskbar)
            {
                HResult result = Taskbar.ThumbBarAddButtons(hwnd, cButtons, pButton);

                result.ThrowIf();
            }
        }

        public static void ThumbBarUpdateButtons(IntPtr hwnd, uint cButtons, TaskbarNative.THUMBBUTTON[] pButton)
        {
            if (OSVersion.HasExtendedTaskbar)
            {
                HResult result = Taskbar.ThumbBarUpdateButtons(hwnd, cButtons, pButton);

                result.ThrowIf();
            }
        }

        public static void ThumbBarSetImageList(IntPtr hwnd, IntPtr himl)
        {
            if (OSVersion.HasExtendedTaskbar)
            {
                HResult result = Taskbar.ThumbBarSetImageList(hwnd, himl);

                result.ThrowIf();
            }
        }

        public static void SetOverlayIcon(Icon icon, string pszDescription)
        {
            if (OSVersion.HasExtendedTaskbar)
            {
                HResult result = Taskbar.SetOverlayIcon(Program.HackerWindowHandle, icon != null ? icon.Handle : IntPtr.Zero, pszDescription);

                result.ThrowIf();
            }
        }

        public static void SetThumbnailTooltip(IntPtr hwnd, string pszTip)
        {
            if (OSVersion.HasExtendedTaskbar)
            {
                HResult result = Taskbar.SetThumbnailTooltip(hwnd, pszTip);

                result.ThrowIf();
            }
        }

        public static void SetThumbnailClip(IntPtr hwnd, ref Rect prcClip)
        {
            if (OSVersion.HasExtendedTaskbar)
            {
                HResult result = Taskbar.SetThumbnailClip(hwnd, ref prcClip);

                result.ThrowIf();
            }
        }
    }
}
