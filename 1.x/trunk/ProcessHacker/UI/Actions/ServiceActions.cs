/*
 * Process Hacker - 
 *   service actions
 * 
 * Copyright (C) 2009 wj32
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
using System.Windows.Forms;
using ProcessHacker.Components;
using ProcessHacker.Native;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;

namespace ProcessHacker.UI.Actions
{
    public static class ServiceActions
    {
        private static bool Prompt(IWin32Window window, string service, string action, string content, TaskDialogIcon icon)
        {
            DialogResult result = DialogResult.No;

            if (OSVersion.HasTaskDialogs)
            {
                TaskDialog td = new TaskDialog();

                td.WindowTitle = "Process Hacker";
                td.MainInstruction = "Do you want to " + action + " " + service + "?";
                td.MainIcon = icon;
                td.Content = content;

                td.Buttons = new TaskDialogButton[]
                {
                    new TaskDialogButton((int)DialogResult.Yes, char.ToUpper(action[0]) + action.Substring(1)),
                    new TaskDialogButton((int)DialogResult.No, "Cancel")
                };
                td.DefaultButton = (int)DialogResult.No;

                result = (DialogResult)td.Show(window);
            }
            else
            {
                result = MessageBox.Show("Are you sure you want to " + action + " " + service + "?",
                    "Process Hacker", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);
            }

            return result == DialogResult.Yes;
        }

        private static bool ElevateIfRequired(IWin32Window window, string service,
            ServiceAccess access, string action)
        {
            if (Settings.Instance.ElevationLevel == (int)ElevationLevel.Never)
                return false;

            if (OSVersion.HasUac && Program.ElevationType == TokenElevationType.Limited)
            {
                try
                {
                    using (var shandle = new ServiceHandle(service, access))
                    { }
                }
                catch (WindowsException ex)
                {
                    DialogResult result;

                    if (Settings.Instance.ElevationLevel == (int)ElevationLevel.Elevate)
                    {
                        result = DialogResult.Yes;
                    }
                    else
                    {
                        TaskDialog td = new TaskDialog();

                        td.WindowTitle = "Process Hacker";
                        td.MainIcon = TaskDialogIcon.Warning;
                        td.MainInstruction = "Do you want to elevate the action?";
                        td.Content = "The action cannot be performed in the current security context. " +
                            "Do you want Process Hacker to prompt for the appropriate credentials and elevate the action?";

                        td.ExpandedInformation = "Error: " + ex.Message + " (0x" + ex.ErrorCode.ToString("x") + ")";
                        td.ExpandFooterArea = true;

                        td.Buttons = new TaskDialogButton[]
                        {
                            new TaskDialogButton((int)DialogResult.Yes, "Elevate\nPrompt for credentials and elevate the action."),
                            new TaskDialogButton((int)DialogResult.No, "Continue\nAttempt to perform the action without elevation.")
                        };
                        td.CommonButtons = TaskDialogCommonButtons.Cancel;
                        td.UseCommandLinks = true;
                        td.Callback = (taskDialog, args, userData) =>
                            {
                                if (args.Notification == TaskDialogNotification.Created)
                                {
                                    taskDialog.SetButtonElevationRequiredState((int)DialogResult.Yes, true);
                                }

                                return false;
                            };

                        result = (DialogResult)td.Show(window);
                    }

                    if (result == DialogResult.Yes)
                    {
                        Program.StartProcessHackerAdmin("-e -type service -action " + action + " -obj \"" +
                            service + "\" -hwnd " + window.Handle.ToString(), null, window.Handle);

                        return true;
                    }
                    else if (result == DialogResult.No)
                    {
                        return false;
                    }
                    else if (result == DialogResult.Cancel)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static void Start(IWin32Window window, string service, bool prompt)
        {
            if (ElevateIfRequired(window, service, ServiceAccess.Start, "start"))
                return;

            if (prompt && !Prompt(window, service, "start",
                "", TaskDialogIcon.None))
                return;

            try
            {
                using (var shandle = new ServiceHandle(service, ServiceAccess.Start))
                    shandle.Start();
            }
            catch (Exception ex)
            {
                DialogResult r = MessageBox.Show(window, "Could not start the service \"" + service +
                    "\":\n\n" +
                    ex.Message, "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void Continue(IWin32Window window, string service, bool prompt)
        {
            if (ElevateIfRequired(window, service, ServiceAccess.PauseContinue, "continue"))
                return;

            if (prompt && !Prompt(window, service, "continue",
                "", TaskDialogIcon.None))
                return;

            try
            {
                using (var shandle = new ServiceHandle(service, ServiceAccess.PauseContinue))
                    shandle.Control(ServiceControl.Continue);
            }
            catch (Exception ex)
            {
                DialogResult r = MessageBox.Show(window, "Could not continue the service \"" + service +
                    "\":\n\n" +
                    ex.Message, "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void Pause(IWin32Window window, string service, bool prompt)
        {
            if (ElevateIfRequired(window, service, ServiceAccess.PauseContinue, "pause"))
                return;

            if (prompt && !Prompt(window, service, "pause",
                "", TaskDialogIcon.None))
                return;

            try
            {
                using (var shandle = new ServiceHandle(service, ServiceAccess.PauseContinue))
                    shandle.Control(ServiceControl.Pause);
            }
            catch (Exception ex)
            {
                DialogResult r = MessageBox.Show(window, "Could not pause the service \"" + service +
                    "\":\n\n" +
                    ex.Message, "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void Stop(IWin32Window window, string service, bool prompt)
        {
            if (ElevateIfRequired(window, service, ServiceAccess.Stop, "stop"))
                return;

            if (prompt && !Prompt(window, service, "stop",
                "", TaskDialogIcon.None))
                return;

            try
            {
                using (var shandle = new ServiceHandle(service, ServiceAccess.Stop))
                    shandle.Control(ServiceControl.Stop);
            }
            catch (Exception ex)
            {
                DialogResult r = MessageBox.Show(window, "Could not stop the service \"" + service +
                    "\":\n\n" +
                    ex.Message, "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void Delete(IWin32Window window, string service, bool prompt)
        {
            if (ElevateIfRequired(window, service, (ServiceAccess)StandardRights.Delete, "delete"))
                return;

            if (prompt && !Prompt(window, service, "delete",
                "Deleting a service can prevent the system from starting or functioning properly. " +
                "Are you sure you want to continue?", TaskDialogIcon.Warning))
                return;

            try
            {
                using (var shandle = new ServiceHandle(service, (ServiceAccess)StandardRights.Delete))
                    shandle.Delete();
            }
            catch (Exception ex)
            {
                DialogResult r = MessageBox.Show(window, "Could not delete the service \"" + service +
                    "\":\n\n" +
                    ex.Message, "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
