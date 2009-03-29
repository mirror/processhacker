using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Samples;
using System.Windows.Forms;

namespace ProcessHacker.UI
{
    public static class ProcessActions
    {
        private static bool Prompt(IWin32Window window, int[] pids, string[] names, 
            string action, string content, bool promptOnlyIfDangerous)
        {
            string name = "the selected process(es)";

            if (pids.Length == 1)
                name = names[0];
            else
                name = "the selected processes";

            bool dangerous = false;

            foreach (int pid in pids)
            {
                if (Misc.IsDangerousPid(pid))
                {
                    dangerous = true;
                    break;
                }
            }

            if (promptOnlyIfDangerous && !dangerous)
                return true;

            DialogResult result = DialogResult.No;

            if (Program.WindowsVersion == WindowsVersion.Vista)
            {
                TaskDialog td = new TaskDialog();

                td.WindowTitle = "Process Hacker";
                td.MainInstruction = "Do you want to " + action + " " + name + "?";
                td.Content = content;

                if (dangerous)
                {
                    td.MainIcon = TaskDialogIcon.Warning;
                    td.Content = "You are about to " + action + " one or more system processes. " +
                        "Doing so will cause system instability. Are you sure you want to continue?";
                }

                if (pids.Length > 1)
                {
                    td.ExpandFooterArea = true;
                    td.ExpandedInformation = "Processes:\r\n";

                    for (int i = 0; i < pids.Length; i++)
                    {
                        td.ExpandedInformation += names[i] + " (PID " + pids[i].ToString() + ")\r\n";
                    }

                    td.ExpandedInformation = td.ExpandedInformation.Trim();
                }

                td.Buttons = new TaskDialogButton[]
                {
                    new TaskDialogButton((int)DialogResult.Yes, char.ToUpper(action[0]) + action.Substring(1)),
                    new TaskDialogButton((int)DialogResult.No, "Cancel")
                };
                td.DefaultButton = (int)DialogResult.No;

                result = (DialogResult)td.Show(window);
            }
            else if (Program.WindowsVersion == WindowsVersion.XP)
            {
                result = MessageBox.Show("Are you sure you want to " + action + " " + name + "?",
                    "Process Hacker", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);
            }

            return result == DialogResult.Yes;
        }

        public static void Terminate(IWin32Window window, int[] pids, string[] names, bool prompt)
        {
            if (Program.ElevationType == Win32.TOKEN_ELEVATION_TYPE.TokenElevationTypeLimited &&
                Program.KPH == null)
            {
                try
                {
                    foreach (int pid in pids)
                    {
                        using (Win32.ProcessHandle phandle = new Win32.ProcessHandle(pid,
                            Win32.PROCESS_RIGHTS.PROCESS_TERMINATE))
                        { }
                    }
                }
                catch
                {
                    string objects = "";

                    foreach (int pid in pids)
                        objects += pid + ",";

                    Program.StartProcessHackerAdmin("-e -type process -action terminate -obj \"" +
                        objects + "\" -hwnd " + window.Handle.ToString(), null, window.Handle);

                    return;
                }
            }

            if (prompt && !Prompt(window, pids, names, "terminate",
                "Terminating a process will cause unsaved data to be lost. " +
                "Terminating a system process will cause system instability. " +
                "Are you sure you want to continue?", false))
                return;

            for (int i = 0; i < pids.Length; i++)
            {
                try
                {
                    using (Win32.ProcessHandle phandle = new Win32.ProcessHandle(pids[i],
                        Win32.PROCESS_RIGHTS.PROCESS_TERMINATE))
                        phandle.Terminate();
                }
                catch (Exception ex)
                {
                    DialogResult r = MessageBox.Show("Could not terminate process \"" + names[i] +
                        "\" with PID " + pids[i].ToString() + ":\n\n" +
                        ex.Message, "Process Hacker", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);

                    if (r == DialogResult.Cancel)
                        return;
                }
            }
        }

        public static void Suspend(IWin32Window window, int[] pids, string[] names, bool prompt)
        {
            if (Program.ElevationType == Win32.TOKEN_ELEVATION_TYPE.TokenElevationTypeLimited &&
                Program.KPH == null)
            {
                try
                {
                    foreach (int pid in pids)
                    {
                        using (Win32.ProcessHandle phandle = new Win32.ProcessHandle(pid,
                            Win32.PROCESS_RIGHTS.PROCESS_SUSPEND_RESUME))
                        { }
                    }
                }
                catch
                {
                    string objects = "";

                    foreach (int pid in pids)
                        objects += pid + ",";

                    Program.StartProcessHackerAdmin("-e -type process -action suspend -obj \"" +
                        objects + "\" -hwnd " + window.Handle.ToString(), null, window.Handle);

                    return;
                }
            }

            if (prompt && !Prompt(window, pids, names, "suspend",
                "Suspending a process will pause its execution. " +
                "Suspending a system process will cause system instability. " +
                "Are you sure you want to continue?", true))
                return;

            for (int i = 0; i < pids.Length; i++)
            {
                try
                {
                    using (Win32.ProcessHandle phandle = new Win32.ProcessHandle(pids[i],
                        Win32.PROCESS_RIGHTS.PROCESS_SUSPEND_RESUME))
                        phandle.Suspend();
                }
                catch (Exception ex)
                {
                    DialogResult r = MessageBox.Show("Could not suspend process \"" + names[i] +
                        "\" with PID " + pids[i].ToString() + ":\n\n" +
                        ex.Message, "Process Hacker", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);

                    if (r == DialogResult.Cancel)
                        return;
                }
            }
        }

        public static void Resume(IWin32Window window, int[] pids, string[] names, bool prompt)
        {
            if (Program.ElevationType == Win32.TOKEN_ELEVATION_TYPE.TokenElevationTypeLimited &&
                Program.KPH == null)
            {
                try
                {
                    foreach (int pid in pids)
                    {
                        using (Win32.ProcessHandle phandle = new Win32.ProcessHandle(pid,
                            Win32.PROCESS_RIGHTS.PROCESS_SUSPEND_RESUME))
                        { }
                    }
                }
                catch
                {
                    string objects = "";

                    foreach (int pid in pids)
                        objects += pid + ",";

                    Program.StartProcessHackerAdmin("-e -type process -action resume -obj \"" +
                        objects + "\" -hwnd " + window.Handle.ToString(), null, window.Handle);

                    return;
                }
            }

            if (prompt && !Prompt(window, pids, names, "resume",
                "Resuming a process will begin its execution. " +
                "Resuming a system process may lead to system instability. " +
                "Are you sure you want to continue?", true))
                return;

            for (int i = 0; i < pids.Length; i++)
            {
                try
                {
                    using (Win32.ProcessHandle phandle = new Win32.ProcessHandle(pids[i],
                        Win32.PROCESS_RIGHTS.PROCESS_SUSPEND_RESUME))
                        phandle.Resume();
                }
                catch (Exception ex)
                {
                    DialogResult r = MessageBox.Show("Could not resume process \"" + names[i] +
                        "\" with PID " + pids[i].ToString() + ":\n\n" +
                        ex.Message, "Process Hacker", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);

                    if (r == DialogResult.Cancel)
                        return;
                }
            }
        }
    }
}
