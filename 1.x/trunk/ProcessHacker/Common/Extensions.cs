/*
 * Process Hacker - 
 *   long extensions
 * 
 * Copyright (C) 2009 Dean
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
using System.Windows.Forms;
using ProcessHacker.Native;
using ProcessHacker.Common;
using ProcessHacker.Native.Api;
using System.Net;

namespace ProcessHacker
{
    public static class EnumExtensions
    {
        public static Int32 ToInt(this Enum code)
        {
            //do not cast (int)
            return Convert.ToInt32(code);
        }
    }

    public static class ExceptionExtensions
    {
        public static void LogEx(this Exception ex, Boolean show, Boolean log, String operation)
        {
            HackerEvent.Log.Ex(show, log, operation, ex);
        }
    }

    public static class FormExtensions
    {
        public static void FocusWindow(this Form f)
        {
            if (f.InvokeRequired)
            {
                f.BeginInvoke(new MethodInvoker(delegate { f.FocusWindow(); }));
                return;
            }

            //TODO: fix crossthread exception under some instances
            try
            {
                f.Visible = true; // just in case it's hidden right now   

                if (f.WindowState == FormWindowState.Minimized)
                    f.WindowState = FormWindowState.Normal;

                f.Activate();
            }
            catch { }
        }

        public static void AddEscapeToClose(this Form f)
        {
            f.KeyPreview = true;
            f.KeyDown += (sender, e) =>
            {
                if (e.KeyCode == Keys.Escape)
                {
                    f.Close();
                    e.Handled = true;
                }
            };
        }

        public static void SetTopMost(this Form f)
        {
            if (Program.HackerWindow.TopMost)
                f.TopMost = true;
        }

        /// <summary>
        /// Floats the window on top of the main Process Hacker window.
        /// </summary>
        /// <param name="f">The form to float.</param>
        /// <remarks>
        /// Always call this method before calling InitializeComponent in order for the 
        /// parent to be restored properly.
        /// </remarks>
        public static void SetPhParent(this Form f)
        {
            f.SetPhParent(true);
        }

        public static void SetPhParent(this Form f, Boolean hideInTaskbar)
        {
            if (ProcessHacker.Settings.Instance.FloatChildWindows)
            {
                if (hideInTaskbar)
                    f.ShowInTaskbar = false;

                Program.HackerWindow.AddOwnedForm(f);

                //.net has a full 32/64bit support or these apis using Owner property, 
                //either we roll our own (using both 32/64 apis!) or we use the .net methods.

                //IntPtr oldParent = Win32.SetWindowLongPtr(f.Handle, GetWindowLongOffset.HwndParent, Program.HackerWindowHandle);
                //f.FormClosing += (sender, e) => Win32.SetWindowLongPtr(f.Handle, GetWindowLongOffset.HwndParent, oldParent);
                //  ^ ??
            }
        }

        /// <summary>
        /// Selects all of the specified nodes.
        /// </summary>
        /// <param name="items">The nodes.</param>
        public static void SelectAll(this IEnumerable<Aga.Controls.Tree.TreeNodeAdv> nodes)
        {
            foreach (Aga.Controls.Tree.TreeNodeAdv node in nodes)
                node.IsSelected = true;
        }

        /// <summary>
        /// Controls whether the UAC shield icon is displayed on the specified button.
        /// </summary>
        /// <param name="button">The button to modify.</param>
        /// <param name="show">Whether to show the UAC shield icon.</param>
        private static void SetShieldIconInternal(Button button, Boolean show)
        {
            if (OSVersion.IsAboveOrEqual(WindowsVersion.Vista))
            {
                Win32.SendMessage(button.Handle,
                    WindowMessage.BcmSetShield, 0, show ? 1 : 0);
            }
        }

        /// <summary>
        /// Controls whether the UAC shield icon is displayed on the button.
        /// </summary>
        /// <param name="visible">Whether the shield icon is visible.</param>
        public static void SetShieldIcon(this Button button, Boolean visible)
        {
            SetShieldIconInternal(button, visible);
        }

        /// <summary>
        /// Sets the theme of a control.
        /// </summary>
        /// <param name="control">The control to modify.</param>
        /// <param name="theme">A name of a theme.</param>
        public static void SetTheme(this Control control, String theme)
        {
            // Don't set on XP, doesn't look better than without SetWindowTheme.
            if (OSVersion.IsAboveOrEqual(WindowsVersion.Vista))
            {
                Win32.SetWindowTheme(control.Handle, theme, null);
            }
        }

        public static void UpdateWindowMenu(this Form f, Menu windowMenuItem)
        {
            windowMenuItem.MenuItems.DisposeAndClear();

            MenuItem item;

            item = new MenuItem("&Always On Top");
            item.Tag = f.Handle;
            item.Click += new EventHandler(delegate
            {
                if (!f.Handle.IsGreaterThanZero())
                    return;

                f.Invoke(new MethodInvoker(delegate
                {
                    if (f != Program.HackerWindow)
                    {
                        Program.HackerWindow.TopMost = !Program.HackerWindow.TopMost;
                    }

                    f.TopMost = !f.TopMost;
                }));

                UpdateWindowMenu(f, windowMenuItem);
            });

            item.Checked = f.TopMost;

            windowMenuItem.MenuItems.Add(item);

            item = new MenuItem("&Close");
            item.Tag = f.Handle;
            item.Click += new EventHandler(delegate
            {
                if (!f.Handle.IsGreaterThanZero())
                    return;

                f.Invoke(new MethodInvoker(delegate { f.Close(); }));
            });

            windowMenuItem.MenuItems.Add(item);
        }
    }

    public static class Int32Extensions
    {
        public static Int32 ToInt(this Decimal value)
        {
            return Convert.ToInt32(value);
        }
    }

    public static class IntPtrExtensions
    {
        public static IntPtr And(this IntPtr ptr, int value)
        {
            if (IntPtr.Size == sizeof(Int32))
                return new IntPtr(ptr.ToInt32() & value);
            else
                return new IntPtr(ptr.ToInt64() & value);
        }

        public static IntPtr And(this IntPtr ptr, IntPtr value)
        {
            if (IntPtr.Size == sizeof(Int32))
                return new IntPtr(ptr.ToInt32() & value.ToInt32());
            else
                return new IntPtr(ptr.ToInt64() & value.ToInt64());
        }

        public static int CompareTo(this IntPtr ptr, IntPtr ptr2)
        {
            if (ptr.ToUInt64() > ptr2.ToUInt64())
                return 1;
            if (ptr.ToUInt64() < ptr2.ToUInt64())
                return -1;
            return 0;
        }

        public static bool IsGreaterThanZero(this IntPtr ptr)
        {
            return ptr.CompareTo(IntPtr.Zero) > 0;
        }

        public static IntPtr Not(this IntPtr ptr)
        {
            if (IntPtr.Size == sizeof(Int32))
                return new IntPtr(~ptr.ToInt32());
            else
                return new IntPtr(~ptr.ToInt64());
        }

        public static IntPtr Or(this IntPtr ptr, IntPtr value)
        {
            if (IntPtr.Size == sizeof(Int32))
                return new IntPtr(ptr.ToInt32() | value.ToInt32());
            else
                return new IntPtr(ptr.ToInt64() | value.ToInt64());
        }

        public static uint ToUInt32(this IntPtr ptr)
        {
            // Avoid sign-extending the pointer - we want it zero-extended.
            unsafe
            {
                void* voidPtr = (void*)ptr;

                return (uint)voidPtr;
            }
        }

        public static ulong ToUInt64(this IntPtr ptr)
        {
            // Avoid sign-extending the pointer - we want it zero-extended.
            unsafe
            {
                void* voidPtr = (void*)ptr;

                return (ulong)voidPtr;
            }
        }

        public static IntPtr ToIntPtr(this int value)
        {
            return new IntPtr(value);
        }

        public static IntPtr ToIntPtr(this uint value)
        {
            unchecked
            {
                return new IntPtr((int)value);
            }
        }

        public static IntPtr ToIntPtr(this long value)
        {
            unchecked
            {
                if (value > 0 && value <= 0xffffffff)
                    return new IntPtr((int)value);
            }

            return new IntPtr(value);
        }

        public static IntPtr ToIntPtr(this ulong value)
        {
            unchecked
            {
                return ((long)value).ToIntPtr();
            }
        }

        public static IntPtr Xor(this IntPtr ptr, IntPtr value)
        {
            if (IntPtr.Size == sizeof(Int32))
                return new IntPtr(ptr.ToInt32() ^ value.ToInt32());
            else
                return new IntPtr(ptr.ToInt64() ^ value.ToInt64());
        }
    }

    public static class ListViewExtensions
    {
        /// <summary>
        /// Adds Ctrl+C and Ctrl+A shortcuts to the specified ListView.
        /// </summary>
        /// <param name="lv">The ListView to modify.</param>
        public static void AddShortcuts(this ListView lv)
        {
            lv.AddShortcuts(null);
        }

        /// <summary>
        /// Adds Ctrl+C and Ctrl+A shortcuts to the specified ListView.
        /// </summary>
        /// <param name="lv">The ListView to modify.</param>
        /// <param name="retrieveVirtualItem">A virtual item handler, if any.</param>
        public static void AddShortcuts(this ListView lv, RetrieveVirtualItemEventHandler retrieveVirtualItem)
        {
            lv.KeyDown +=
                (sender, e) =>
                {
                    if (e.Control && e.KeyCode == Keys.A)
                    {
                        if (retrieveVirtualItem != null)
                        {
                            for (int i = 0; i < lv.VirtualListSize; i++)
                                if (!lv.SelectedIndices.Contains(i))
                                    lv.SelectedIndices.Add(i);
                        }
                        else
                        {
                            lv.Items.SelectAll();
                        }
                    }

                    if (e.Control && e.KeyCode == Keys.C)
                    {
                        ProcessHacker.UI.GenericViewMenu.ListViewCopy(lv, -1, retrieveVirtualItem);
                    }
                };
        }
    }

    public static class LongExtensions
    {
        /// <summary>
        /// Gets the largest value in the specified array of longs.
        /// </summary>
        /// <param name="source">The source array.</param>
        /// <returns>The largest value in the specified array.</returns>
        public static long Max(this IEnumerable<long> source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            long max = 0;
            bool afterFirst = false;

            foreach (long number in source)
            {
                if (afterFirst)
                {
                    if (number > max)
                        max = number;
                }
                else
                {
                    max = number;
                    afterFirst = true;
                }
            }

            return max;
        }

        /// <summary>
        /// Takes a number of elements from the specified array.
        /// </summary>
        /// <param name="source">The list to process.</param>
        /// <param name="count">The number of elements to take.</param>
        /// <returns>
        /// A new list containing the first <paramref name="count" /> 
        /// elements from the specified array.
        /// </returns>
        public static IList<long> Take(this IList<long> source, int count)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            // If we're trying to take more than we have, return the original set.
            if (count >= source.Count)
                return source;

            // Create a new list containing the elements.
            IList<long> newList = new List<long>();

            for (int i = 0; i < count; i++)
                newList.Add(source[i]);

            return newList;
        }
    }

    public static class NetworkExtensions
    {
        public static bool IsEmpty(this IPEndPoint endPoint)
        {
            return endPoint.Address.GetAddressBytes().IsEmpty() && endPoint.Port == 0;
        }
    }
}