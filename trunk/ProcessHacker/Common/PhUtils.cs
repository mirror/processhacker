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
using System.Drawing;
using System.Net;
using System.Windows.Forms;
using Aga.Controls.Tree;
using ProcessHacker.Components;
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
                        GenericViewMenu.ListViewCopy(lv, -1, retrieveVirtualItem);
                    }
                };
        }

        /// <summary>
        /// Gets whether the specified process is a system process.
        /// </summary>
        /// <param name="pid">The PID of a process to check.</param>
        /// <returns>Whether the process is a system process.</returns>
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
        /// Formats an error message.
        /// </summary>
        /// <param name="operation">
        /// The operation being performed, e.g. "Unable to X"
        /// </param>
        /// <param name="ex">The exception to use.</param>
        /// <returns>A formatted error message.</returns>
        private static string FormatException(string operation, Exception ex)
        {
            if (!string.IsNullOrEmpty(operation))
                return operation + ": " + ex.Message;
            else
                return ex.Message;
        }

        public static string FormatPriorityClass(ProcessPriorityClass priorityClass)
        {
            switch (priorityClass)
            {
                case ProcessPriorityClass.AboveNormal:
                    return "Above Normal";
                case ProcessPriorityClass.BelowNormal:
                    return "Below Normal";
                case ProcessPriorityClass.High:
                    return "High";
                case ProcessPriorityClass.Idle:
                    return "Idle";
                case ProcessPriorityClass.Normal:
                    return "Normal";
                case ProcessPriorityClass.RealTime:
                    return "Realtime";
                case ProcessPriorityClass.Unknown:
                default:
                    return "";
            }
        }

        /// <summary>
        /// Gets an appropriate foreground color to be displayed on top of a 
        /// specified background color.
        /// </summary>
        /// <param name="backColor">The background color.</param>
        /// <returns>
        /// Black if the background color's brightness is above 0.4, otherwise 
        /// White.
        /// </returns>
        public static Color GetForeColor(Color backColor)
        {
            if (backColor.GetBrightness() > 0.4)
                return Color.Black;
            else
                return Color.White;
        }

        public static string GetIntegrity(this TokenHandle tokenHandle, out int integrityLevel)
        {
            var groups = tokenHandle.GetGroups();
            string integrity = null;

            integrityLevel = 0;

            for (int i = 0; i < groups.Length; i++)
            {
                if ((groups[i].Attributes & SidAttributes.IntegrityEnabled) != 0)
                {
                    integrity = groups[i].GetFullName(false).Replace(" Mandatory Level", "");

                    if (integrity == "Untrusted")
                        integrityLevel = 0;
                    else if (integrity == "Low")
                        integrityLevel = 1;
                    else if (integrity == "Medium")
                        integrityLevel = 2;
                    else if (integrity == "High")
                        integrityLevel = 3;
                    else if (integrity == "System")
                        integrityLevel = 4;
                    else if (integrity == "Installer")
                        integrityLevel = 5;
                }

                groups[i].Dispose();
            }

            return integrity;
        }

        public static bool IsEmpty(this IPEndPoint endPoint)
        {
            return endPoint.Address.GetAddressBytes().IsEmpty() && endPoint.Port == 0;
        }

        /// <summary>
        /// Opens a registry key in the Registry Editor.
        /// </summary>
        /// <param name="keyName">
        /// The path to the registry key, in either abbreviated (HKCU, HKLM, etc.) 
        /// or full (HKEY_CURRENT_USER, etc.) format.
        /// </param>
        public static void OpenKeyInRegedit(string keyName)
        {
            OpenKeyInRegedit(null, keyName);
        }

        /// <summary>
        /// Opens a registry key in the Registry Editor.
        /// </summary>
        /// <param name="window">
        /// The window in which the elevation dialog, if any, should be centered.
        /// </param>
        /// <param name="keyName">
        /// The path to the registry key, in either abbreviated (HKCU, HKLM, etc.) 
        /// or full (HKEY_CURRENT_USER, etc.) format.
        /// </param>
        public static void OpenKeyInRegedit(IWin32Window window, string keyName)
        {
            string lastKey = keyName;

            // Expand the abbreviations.
            if (lastKey.ToLowerInvariant().StartsWith("hkcu"))
                lastKey = "HKEY_CURRENT_USER" + lastKey.Substring(4);
            else if (lastKey.ToLowerInvariant().StartsWith("hku"))
                lastKey = "HKEY_USERS" + lastKey.Substring(3);
            else if (lastKey.ToLowerInvariant().StartsWith("hkcr"))
                lastKey = "HKEY_CLASSES_ROOT" + lastKey.Substring(4);
            else if (lastKey.ToLowerInvariant().StartsWith("hklm"))
                lastKey = "HKEY_LOCAL_MACHINE" + lastKey.Substring(4);

            // Set the last opened key in regedit config. Note that if we are on 
            // Vista, we need to append "Computer\" to the beginning.
            using (var regeditKey =
                Microsoft.Win32.Registry.CurrentUser.CreateSubKey(
                    @"Software\Microsoft\Windows\CurrentVersion\Applets\Regedit",
                    Microsoft.Win32.RegistryKeyPermissionCheck.ReadWriteSubTree
                    ))
            {
                if (OSVersion.IsAboveOrEqual(WindowsVersion.Vista))
                    regeditKey.SetValue("LastKey", "Computer\\" + lastKey);
                else
                    regeditKey.SetValue("LastKey", lastKey);
            }

            // If we have UAC and we aren't elevated, request that regedit be elevated 
            // and pass the window handle. This is so that we get the elevation 
            // dialog in the center of the specified window because it looks nice.
            // Also, this makes sure we don't throw an exception if the user denies 
            // elevation.
            if (OSVersion.HasUac && Program.ElevationType == TokenElevationType.Limited)
            {
                Program.StartProgramAdmin(
                    Environment.SystemDirectory + "\\..\\regedit.exe",
                    "",
                    null,
                    ShowWindowType.Normal,
                    window != null ? window.Handle : IntPtr.Zero
                    );
            }
            else
            {
                System.Diagnostics.Process.Start(Environment.SystemDirectory + "\\..\\regedit.exe");
            }
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

        /// <summary>
        /// Sets the theme of a control.
        /// </summary>
        /// <param name="control">The control to modify.</param>
        /// <param name="theme">A name of a theme.</param>
        public static void SetTheme(this Control control, string theme)
        {
            Win32.SetWindowTheme(control.Handle, theme, null);
        }

        /// <summary>
        /// Asks if the user wants to perform an action.
        /// </summary>
        /// <param name="verb">
        /// The action to be performed, e.g. "Terminate"
        /// </param>
        /// <param name="obj">
        /// The object for the action to be performed on, e.g. "the selected process"
        /// </param>
        /// <param name="message">
        /// Additional information to show to the user, e.g. "Terminating a process will ..."
        /// </param>
        /// <param name="warning">Whether the message is a warning.</param>
        /// <returns>Whether the user wants to continue with the operation.</returns>
        public static bool ShowConfirmMessage(string verb, string obj, string message, bool warning)
        {
            // Make the sure the verb is all lowercase.
            verb = verb.ToLower();

            // "terminate" -> "Terminate"
            string verbCaps = char.ToUpper(verb[0]) + verb.Substring(1);
            // "terminate", "the process" -> "terminate the process"
            string action = verb + " " + obj;

            // Example:
            // __________________________________________________
            // | Process Hacker                             _ O x|
            // |                                                 |
            // |    /\     Do you want to terminate the process? |
            // |   /||\                                          |
            // |  /_||_\   Terminating a process may result in...|
            // |           Are you sure you want to continue?    |
            // |                                                 |
            // |                       | Terminate |  | Cancel | | 
            // |_________________________________________________|

            if (OSVersion.HasTaskDialogs)
            {
                TaskDialog td = new TaskDialog();

                td.WindowTitle = "Process Hacker";
                td.MainIcon = warning ? TaskDialogIcon.Warning : TaskDialogIcon.None;
                td.MainInstruction = "Do you want to " + action + "?";

                if (!string.IsNullOrEmpty(message))
                    td.Content = message + " Are you sure you want to continue?";

                td.Buttons = new TaskDialogButton[]
                {
                    new TaskDialogButton((int)DialogResult.Yes, verbCaps),
                    new TaskDialogButton((int)DialogResult.No, "Cancel")
                };
                td.DefaultButton = (int)DialogResult.No;

                return td.Show(Form.ActiveForm) == (int)DialogResult.Yes;
            }
            else
            {
                return MessageBox.Show(
                    message + " Are you sure you want to " + action + "?",
                    "Process Hacker",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                    ) == DialogResult.Yes;
            }
        }

        /// <summary>
        /// Notifies the user of an error and asks whether the operation should 
        /// continue.
        /// </summary>
        /// <param name="operation">
        /// The operation being performed, e.g. "Unable to X"
        /// </param>
        /// <param name="ex">The exception to notify the user of.</param>
        /// <returns>
        /// True if the user wants to continue the operation, otherwise false.
        /// </returns>
        public static bool ShowContinueMessage(string operation, Exception ex)
        {
            return MessageBox.Show(
                FormatException(operation, ex),
                "Process Hacker",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Error
                ) == DialogResult.OK;
        }

        /// <summary>
        /// Displays an error message to the user.
        /// </summary>
        /// <param name="message">The message to show.</param>
        public static void ShowError(string message)
        {
            MessageBox.Show(message, "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Notifies the user of an error.
        /// </summary>
        /// <param name="operation">
        /// The operation being performed, e.g. "Unable to X"
        /// </param>
        /// <param name="ex">The exception to notify the user of.</param>
        public static void ShowException(string operation, Exception ex)
        {
            MessageBox.Show(FormatException(operation, ex), "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Displays information to the user.
        /// </summary>
        /// <param name="message">The message to show.</param>
        public static void ShowInformation(string message)
        {
            MessageBox.Show(message, "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Displays a warning to the user.
        /// </summary>
        /// <param name="message">The message to show.</param>
        public static void ShowWarning(string message)
        {
            MessageBox.Show(message, "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
