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

        public static void ActivateTab(IntPtr hWnd)
        {
            HResult result = Taskbar.ActivateTab(hWnd);

            result.ThrowIf();
        }

        public static void AddTab(IntPtr hWnd)
        {
            HResult result = Taskbar.AddTab(hWnd);

            result.ThrowIf();
        }

        public static void DeleteTab(IntPtr hWnd)
        {
            HResult result = Taskbar.DeleteTab(hWnd);

            result.ThrowIf();
        }

        public static void SetActivateAlt(IntPtr hWnd)
        {
            HResult result = Taskbar.SetActiveAlt(hWnd);

            result.ThrowIf();
        }

        public static void MarkFullscreenWindow(IntPtr hWnd, bool fullscreen)
        {
            HResult result = Taskbar.MarkFullscreenWindow(hWnd, fullscreen);

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

        public static void SetProgressState(TaskbarNative.TaskbarProgressFlags flags)
        {
            if (OSVersion.HasExtendedTaskbar)
            {
                HResult result = Taskbar.SetProgressState(Program.HackerWindowHandle, flags);

                result.ThrowIf();
            }
        }

        public static void RegisterTab(IntPtr hWndTab, IntPtr hWndMdi)
        {
            if (OSVersion.HasExtendedTaskbar)
            {
                HResult result = Taskbar.RegisterTab(hWndTab, hWndMdi);

                result.ThrowIf();
            }
        }

        public static void UnregisterTab(IntPtr hWndTab)
        {
            if (OSVersion.HasExtendedTaskbar)
            {
                HResult result = Taskbar.UnregisterTab(hWndTab);

                result.ThrowIf();
            }
        }

        public static void SetTabOrder(IntPtr hWndTab, IntPtr hWndInsertBefore)
        {
            if (OSVersion.HasExtendedTaskbar)
            {
                HResult result = Taskbar.SetTabOrder(hWndTab, hWndInsertBefore);

                result.ThrowIf();
            }
        }

        public static void SetTabActive(IntPtr hWndTab, IntPtr hWndInsertBefore, TaskbarNative.TabActiveFlags tbatFlags)
        {
            if (OSVersion.HasExtendedTaskbar)
            {
                HResult result = Taskbar.SetTabActive(hWndTab, hWndInsertBefore, tbatFlags);

                result.ThrowIf();
            }
        }

        public static void ThumbBarAddButtons(IntPtr hWnd, int count, TaskbarNative.ThumbButton[] buttons)
        {
            if (OSVersion.HasExtendedTaskbar)
            {
                HResult result = Taskbar.ThumbBarAddButtons(hWnd, count, buttons);

                result.ThrowIf();
            }
        }

        public static void ThumbBarUpdateButtons(IntPtr hWnd, int count, TaskbarNative.ThumbButton[] buttons)
        {
            if (OSVersion.HasExtendedTaskbar)
            {
                HResult result = Taskbar.ThumbBarUpdateButtons(hWnd, count, buttons);

                result.ThrowIf();
            }
        }

        public static void ThumbBarSetImageList(IntPtr hWnd, IntPtr imageListHandle)
        {
            if (OSVersion.HasExtendedTaskbar)
            {
                HResult result = Taskbar.ThumbBarSetImageList(hWnd, imageListHandle);

                result.ThrowIf();
            }
        }

        public static void SetOverlayIcon(Icon icon, string description)
        {
            if (OSVersion.HasExtendedTaskbar)
            {
                HResult result = Taskbar.SetOverlayIcon(
                    Program.HackerWindowHandle,
                    icon != null ? icon.Handle : IntPtr.Zero,
                    description
                    );

                result.ThrowIf();
            }
        }

        public static void SetThumbnailTooltip(IntPtr hWnd, string tooltipText)
        {
            if (OSVersion.HasExtendedTaskbar)
            {
                HResult result = Taskbar.SetThumbnailTooltip(hWnd, tooltipText);

                result.ThrowIf();
            }
        }

        public static void SetThumbnailClip(IntPtr hWnd, ref Rect clipRectangle)
        {
            if (OSVersion.HasExtendedTaskbar)
            {
                HResult result = Taskbar.SetThumbnailClip(hWnd, ref clipRectangle);

                result.ThrowIf();
            }
        }
    }
}
