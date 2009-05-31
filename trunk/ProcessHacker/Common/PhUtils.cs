/*
 * Process Hacker - 
 *   misc. functions
 * 
 * Copyright (C) 2008-2009 wj32
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
using Aga.Controls.Tree;
using ProcessHacker.Native;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;
using ProcessHacker.UI;

namespace ProcessHacker.Common
{
    public static class PhUtils
    {
        public static string[] DangerousNames = 
        {
            "csrss.exe", "dwm.exe", "logonui.exe", "lsass.exe", "lsm.exe", "services.exe",
            "smss.exe", "wininit.exe", "winlogon.exe"
        };

        public static void AddShortcuts(this ListView lv)
        {
            lv.AddShortcuts(null);
        }

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
                        GenericViewMenu.ListViewCopy(lv, -1, retrieveVirtualItem);
                    }
                };
        }

        public static bool IsDangerousPid(int pid)
        {
            if (pid == 4)
                return true;

            try
            {
                using (var phandle = new ProcessHandle(pid, OSVersion.MinProcessQueryInfoAccess))
                {
                    foreach (string s in DangerousNames)
                    {
                        if ((Environment.SystemDirectory + "\\" + s).Equals(
                            FileUtils.FixPath(FileUtils.DeviceFileNameToDos(phandle.GetNativeImageFileName())),
                            StringComparison.InvariantCultureIgnoreCase))
                        {
                            return true;
                        }
                    }
                }
            }
            catch
            { }

            return false;
        }

        /// <summary>
        /// Selects all of the specified nodes.
        /// </summary>
        /// <param name="items">The nodes.</param>
        public static void SelectAll(this IEnumerable<TreeNodeAdv> nodes)
        {
            foreach (TreeNodeAdv node in nodes)
                node.IsSelected = true;
        }

        /// <summary>
        /// Controls whether the UAC shield icon is displayed on the specified button.
        /// </summary>
        /// <param name="button">The button to modify.</param>
        /// <param name="show">Whether to show the UAC shield icon.</param>
        private static void SetShieldIconInternal(Button button, bool show)
        {
            Win32.SendMessage(button.Handle,
                WindowMessage.BcmSetShield, 0, show ? 1 : 0);
        }

        /// <summary>
        /// Controls whether the UAC shield icon is displayed on the button.
        /// </summary>
        /// <param name="visible">Whether the shield icon is visible.</param>
        public static void SetShieldIcon(this Button button, bool visible)
        {
            SetShieldIconInternal(button, visible);
        }

        public static void SetTheme(this Control control, string theme)
        {
            Win32.SetWindowTheme(control.Handle, theme, null);
        }
    }
}
