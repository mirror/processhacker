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

                    Win32Error result = _taskbar.HrInit();

                    if (Failed(result))
                        throw Marshal.GetExceptionForHR((int)result);
                }

                return _taskbar;
            }
        }

        public static void ActivateTab(IntPtr hwnd)
        {
            Win32Error result = Taskbar.ActivateTab(hwnd);

            if (Failed(result))
                throw Marshal.GetExceptionForHR((int)result);
        }

        public static void AddTab(IntPtr hwnd)
        {
            Win32Error result = Taskbar.AddTab(hwnd);

            if (Failed(result))
                throw Marshal.GetExceptionForHR((int)result);
        }

        public static void DeleteTab(IntPtr hwnd)
        {
            Win32Error result = Taskbar.DeleteTab(hwnd);

            if (Failed(result))
                throw Marshal.GetExceptionForHR((int)result);
        }

        public static void SetActivateAlt(IntPtr hwnd)
        {
            Win32Error result = Taskbar.SetActiveAlt(hwnd);

            if (Failed(result))
                throw Marshal.GetExceptionForHR((int)result);
        }

        public static void MarkFullscreenWindow(IntPtr hwnd, bool fullscreen)
        {
            Win32Error result = Taskbar.MarkFullscreenWindow(hwnd, fullscreen);

            if (Failed(result))
                throw Marshal.GetExceptionForHR((int)result);
        }

        public static void SetProgressValue(ulong completed, ulong total)
        {
            if (OSVersion.HasExtendedTaskbar)
            {
                Win32Error result = Taskbar.SetProgressValue(Program.HackerWindowHandle, completed, total);

                if (Failed(result))
                    throw Marshal.GetExceptionForHR((int)result);
            }
        }

        public static void SetProgressState(TaskbarNative.TBPFlag flags)
        {
            if (OSVersion.HasExtendedTaskbar)
            {
                Win32Error result = Taskbar.SetProgressState(Program.HackerWindowHandle, flags);

                if (Failed(result))
                    throw Marshal.GetExceptionForHR((int)result);
            }
        }

        public static void RegisterTab(IntPtr hwndTab, IntPtr hwndMDI)
        {
            if (OSVersion.HasExtendedTaskbar)
            {
                Win32Error result = Taskbar.RegisterTab(hwndTab, hwndMDI);

                if (Failed(result))
                    throw Marshal.GetExceptionForHR((int)result);
            }
        }

        public static void UnregisterTab(IntPtr hwndTab)
        {
            if (OSVersion.HasExtendedTaskbar)
            {
                Win32Error result = Taskbar.UnregisterTab(hwndTab);

                if (Failed(result))
                    throw Marshal.GetExceptionForHR((int)result);
            }
        }

        public static void SetTabOrder(IntPtr hwndTab, IntPtr hwndInsertBefore)
        {
            if (OSVersion.HasExtendedTaskbar)
            {
                Win32Error result = Taskbar.SetTabOrder(hwndTab, hwndInsertBefore);

                if (Failed(result))
                    throw Marshal.GetExceptionForHR((int)result);
            }
        }

        public static void SetTabActive(IntPtr hwndTab, IntPtr hwndInsertBefore, TaskbarNative.TBATFlag tbatFlags)
        {
            if (OSVersion.HasExtendedTaskbar)
            {
                Win32Error result = Taskbar.SetTabActive(hwndTab, hwndInsertBefore, tbatFlags);

                if (Failed(result))
                    throw Marshal.GetExceptionForHR((int)result);
            }
        }

        public static void ThumbBarAddButtons(IntPtr hwnd, uint cButtons, TaskbarNative.THUMBBUTTON[] pButton)
        {
            if (OSVersion.HasExtendedTaskbar)
            {
                Win32Error result = Taskbar.ThumbBarAddButtons(hwnd, cButtons, pButton);

                if (Failed(result))
                    throw Marshal.GetExceptionForHR((int)result);
            }
        }

        public static void ThumbBarUpdateButtons(IntPtr hwnd, uint cButtons, TaskbarNative.THUMBBUTTON[] pButton)
        {
            if (OSVersion.HasExtendedTaskbar)
            {
                Win32Error result = Taskbar.ThumbBarUpdateButtons(hwnd, cButtons, pButton);

                if (Failed(result))
                    throw Marshal.GetExceptionForHR((int)result);
            }
        }

        public static void ThumbBarSetImageList(IntPtr hwnd, IntPtr himl)
        {
            if (OSVersion.HasExtendedTaskbar)
            {
                Win32Error result = Taskbar.ThumbBarSetImageList(hwnd, himl);

                if (Failed(result))
                    throw Marshal.GetExceptionForHR((int)result);
            }
        }

        public static void SetOverlayIcon(Icon icon, string pszDescription)
        {
            if (OSVersion.HasExtendedTaskbar)
            {
                Win32Error result = Taskbar.SetOverlayIcon(Program.HackerWindowHandle, icon != null ? icon.Handle : IntPtr.Zero, pszDescription);

                if (Failed(result))
                    throw Marshal.GetExceptionForHR((int)result);
            }
        }

        public static void SetThumbnailTooltip(IntPtr hwnd, string pszTip)
        {
            if (OSVersion.HasExtendedTaskbar)
            {
                Win32Error result = Taskbar.SetThumbnailTooltip(hwnd, pszTip);

                if (Failed(result))
                    throw Marshal.GetExceptionForHR((int)result);
            }
        }

        public static void SetThumbnailClip(IntPtr hwnd, ref Rect prcClip)
        {
            if (OSVersion.HasExtendedTaskbar)
            {
                Win32Error result = Taskbar.SetThumbnailClip(hwnd, ref prcClip);

                if (Failed(result))
                    throw Marshal.GetExceptionForHR((int)result);
            }
        }

        public static bool Failed(Win32Error hResult)
        {
            if (hResult != Win32Error.Success)
                return true;
            else
                return false;
        }
    }
}
