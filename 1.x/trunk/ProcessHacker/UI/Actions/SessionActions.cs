using System;
using System.Windows.Forms;
using ProcessHacker.Common;
using ProcessHacker.Components;
using ProcessHacker.Native;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;

namespace ProcessHacker.UI.Actions
{
    public class SessionActions
    {
        private static bool Prompt(IWin32Window window, string name,
            string action, string content)
        {
            DialogResult result = DialogResult.No;

            if (OSVersion.HasTaskDialogs)
            {
                TaskDialog td = new TaskDialog();

                td.WindowTitle = "Process Hacker";
                td.MainInstruction = "Do you want to " + action + " " + name + "?";
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
                result = MessageBox.Show("Are you sure you want to " + action + " " + name + "?",
                    "Process Hacker", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);
            }

            return result == DialogResult.Yes;
        }

        private static void ElevateIfRequired(IWin32Window window, int session, string actionName, Action action)
        {
            if (Settings.Instance.ElevationLevel == (int)ElevationLevel.Never)
                return;

            try
            {
                action();
            }
            catch (WindowsException ex)
            {
                if (ex.ErrorCode == Win32Error.AccessDenied && 
                    OSVersion.HasUac &&
                    Program.ElevationType == ProcessHacker.Native.Api.TokenElevationType.Limited)
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
                        td.Content = "The action could not be performed in the current security context. " +
                            "Do you want Process Hacker to prompt for the appropriate credentials and elevate the action?";

                        td.ExpandedInformation = "Error: " + ex.Message + " (0x" + ex.ErrorCode.ToString("x") + ")";
                        td.ExpandFooterArea = true;

                        td.Buttons = new TaskDialogButton[]
                        {
                            new TaskDialogButton((int)DialogResult.Yes, "Elevate\nPrompt for credentials and elevate the action.")
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
                        Program.StartProcessHackerAdmin("-e -type session -action " + actionName + " -obj \"" +
                            session.ToString() + "\" -hwnd " + window.Handle.ToString(), null, window.Handle);
                    }
                }
                else
                {
                    PhUtils.ShowException("Unable to " + actionName + " the session", ex);
                }
            }
        }

        public static void Disconnect(IWin32Window window, int session, bool prompt)
        {
            if (prompt && !Prompt(window, "the session", "disconnect", ""))
                return;

            ElevateIfRequired(window, session, "disconnect", () =>
                {
                    TerminalServerHandle.GetCurrent().GetSession(session).Disconnect();
                });
        }

        public static void Logoff(IWin32Window window, int session, bool prompt)
        {
            if (prompt && !Prompt(window, "the session", "logoff", ""))
                return;

            ElevateIfRequired(window, session, "logoff", () =>
            {
                TerminalServerHandle.GetCurrent().GetSession(session).Logoff();
            });
        }
    }
}
