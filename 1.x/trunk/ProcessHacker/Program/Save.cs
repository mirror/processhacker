/*
 * Process Hacker - 
 *   save processes
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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;
using ProcessHacker.Common;
using ProcessHacker.Common.Threading;
using ProcessHacker.Native;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;

namespace ProcessHacker
{
    internal static class Save
    {
        private const int TabSize = 8;

        public static void SaveToFile(IWin32Window owner)
        {
            SaveFileDialog sfd = new SaveFileDialog();

            // HTML Files (*.htm;*.html)|*.htm;*.html
            sfd.Filter = "Process Hacker Dump Files (*.phi)|*.phi|Text Files (*.txt;*.log)|*.txt;*.log|" + 
                "Comma-separated values (*.csv)|*.csv|All Files (*.*)|*.*";

            //if (Program.HackerWindow.SelectedPid == -1)
            //{
            //    sfd.FileName = "Process List.txt";
            //}
            //else
            //{
            //    string processName = Windows.GetProcessName(Program.HackerWindow.SelectedPid);

            //    if (processName != null)
            //        sfd.FileName = processName + ".txt";
            //    else
            //        sfd.FileName = "Process Info.txt";
            //}
            sfd.FileName = "phdump-" + DateTime.Now.ToString("ddMMyy") + ".phi";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                FileInfo fi = new FileInfo(sfd.FileName);
                string ext = fi.Extension.ToLowerInvariant();

                if (ext == ".phi")
                {
                    ThreadTask dumpTask = new ThreadTask();

                    dumpTask.RunTask += delegate(object result, ref object param)
                    {
                        var mfs = Dump.BeginDump(fi.FullName, ProcessHacker.Native.Mfs.MfsOpenMode.OverwriteIf);

                        Dump.DumpProcesses(mfs, Program.ProcessProvider);
                        Dump.DumpServices(mfs);

                        mfs.Dispose();
                    };

                    ProgressWindow progressWindow = new ProgressWindow();

                    progressWindow.CloseButtonVisible = false;
                    progressWindow.ProgressBarStyle = ProgressBarStyle.Marquee;
                    progressWindow.ProgressText = "Creating the dump file...";

                    dumpTask.Completed += (result) =>
                    {
                        progressWindow.SetCompleted();

                        if (progressWindow.IsHandleCreated)
                            progressWindow.BeginInvoke(new MethodInvoker(progressWindow.Close));
                    };

                    dumpTask.Start();

                    if (dumpTask.Running)
                        progressWindow.ShowDialog(owner);

                    if (dumpTask.Exception != null)
                        PhUtils.ShowException("Unable to create the dump file", dumpTask.Exception);

                    return;
                }

                try
                {
                    using (StreamWriter sw = new StreamWriter(fi.FullName))
                    {
                        Program.HackerWindow.ProcessTree.Tree.ExpandAll();

                        if (ext == ".htm" || ext == ".html")
                        {

                        }
                        else if (ext == ".csv")
                        {
                            sw.Write(GetProcessTreeText(false));
                        }
                        else
                        {
                            sw.Write(GetEnvironmentInfo());
                            sw.WriteLine();
                            sw.Write(GetProcessTreeText(true));
                            sw.WriteLine();

                            if (Program.HackerWindow.SelectedPid != -1)
                            {
                                sw.Write(GetProcessDetailsText(Program.HackerWindow.SelectedPid));
                                sw.WriteLine();
                            }
                        }
                    }
                }
                catch (IOException ex)
                {
                    PhUtils.ShowException("Unable to save the process list", ex);
                }
            }
        }

        private static string GetEnvironmentInfo()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Process Hacker version " + Application.ProductVersion);
            sb.AppendLine(OSVersion.VersionString + " (" + OSVersion.BitsString + ")");

            return sb.ToString();
        }

        private static string GetProcessTreeText(bool tabs)
        {
            // This function builds a table of strings, formats it, and returns it.

            // The string builder which will contain the result.
            StringBuilder sb = new StringBuilder();
            // The number of rows in the table. We add 1 for the column headers.
            int items = Program.HackerWindow.ProcessTree.Tree.ItemCount + 1;
            // The number of columns.
            int columns = 0;
            // The column to column index map.
            Dictionary<TreeColumn, int> columnIndexMap = new Dictionary<TreeColumn, int>();
            // The table.
            string[][] str = new string[items][];

            // Create the column index map which will map columns to their corresponding 
            // columns in the table. Note that we cannot currently convert graph columns 
            // to text (so we ignore CPU History and I/O history).
            foreach (TreeColumn column in Program.HackerWindow.ProcessTree.Tree.Columns)
            {
                if (column.IsVisible && column.Header != "CPU History" && column.Header != "I/O History")
                {
                    columnIndexMap[column] = columns;
                    columns++;
                }
            }

            // At this point the columns variable will contain the number of columns.

            // Create the rows.
            for (int i = 0; i < items; i++)
                str[i] = new string[columns];

            // Populate the first row with the column headers.
            foreach (var column in Program.HackerWindow.ProcessTree.Tree.Columns)
            {
                if (columnIndexMap.ContainsKey(column))
                    str[0][columnIndexMap[column]] = column.Header;
            }

            // Go through the nodes in the process tree and populate each cell of the table.
            {
                int i = 0;

                // Go through each node.
                foreach (var node in Program.HackerWindow.ProcessTree.Tree.AllNodes)
                {
                    // Go through each node control, find the column which corresponds to it, 
                    // find the column index for the column, and fill in the cell.
                    foreach (var control in Program.HackerWindow.ProcessTree.Tree.NodeControls)
                    {
                        // Make sure the node control is visible, and make sure it's text.
                        if (!control.ParentColumn.IsVisible || !(control is BaseTextControl))
                            continue;

                        // Get the text contained in the node control.
                        string text = (control as BaseTextControl).GetLabel(node);
                        // Get the column index corresponding with the node control's column.
                        int columnIndex = columnIndexMap[control.ParentColumn];

                        // Fill in the cell.
                        str[i + 1][columnIndex] = 
                            // If this is the first column in the row, add some indentation.
                            (columnIndex == 0 ? (new string(' ', (node.Level - 1) * 2)) : "") + 
                            (text != null ? text : "");
                    }

                    i++;
                }
            }

            // Create the tab count array. This will contain the number of tabs needed 
            // to fill the biggest row cell in each column.
            // Note that this is ignored if tabs is false.
            int[] tabCount = new int[columns];

            for (int i = 0; i < items; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    int newCount = str[i][j].Length / TabSize;

                    // Replace the existing count if this tab count is bigger.
                    if (newCount > tabCount[j])
                        tabCount[j] = newCount;
                }
            }

            // Create the final string by going through each cell and appending the 
            // proper tab count (if we are using tabs). That will make sure each 
            // column is properly aligned.
            for (int i = 0; i < items; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (tabs)
                    {
                        // Append the cell contents.
                        sb.Append(str[i][j]);
                        // Append the proper tab count.
                        sb.Append('\t', tabCount[j] - str[i][j].Length / TabSize + 1);
                    }
                    else
                    {
                        // Append the quotes, escape and append the cell contents.
                        sb.Append("\"");
                        sb.Append(str[i][j].Replace("\"", "\\\""));
                        sb.Append("\"");

                        // Append the comma separator.
                        if (j != columns - 1)
                            sb.Append(",");
                    }
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }

        private static string GetProcessDetailsText(int pid)
        {
            // This function returns a string containing details about a process.

            // The string builder which will contain the result.
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Process PID " + pid.ToString() + ":");
            sb.AppendLine();

            try
            {
                using (var phandle = new ProcessHandle(pid, Program.MinProcessQueryRights))
                {
                    var fileName = phandle.GetImageFileName();

                    sb.AppendLine("Native file name: " + fileName);
                    fileName = FileUtils.GetFileName(fileName);
                    sb.AppendLine("DOS file name: " + fileName);

                    try
                    {
                        var fileInfo = FileVersionInfo.GetVersionInfo(fileName);

                        sb.AppendLine("Description: " + fileInfo.FileDescription);
                        sb.AppendLine("Company: " + fileInfo.CompanyName);
                        sb.AppendLine("Version: " + fileInfo.FileVersion);
                    }
                    catch (Exception ex2)
                    {
                        sb.AppendLine("Version info section failed! " + ex2.Message);
                    }

                    sb.AppendLine("Started: " + phandle.GetCreateTime().ToString());

                    var memoryInfo = phandle.GetMemoryStatistics();

                    sb.AppendLine("WS: " + Utils.FormatSize(memoryInfo.WorkingSetSize));
                    sb.AppendLine("Pagefile usage: " + Utils.FormatSize(memoryInfo.PagefileUsage));
                }
            }
            catch (Exception ex)
            {
                sb.AppendLine("Basic info section failed! " + ex.Message);
            }

            try
            {
                using (var phandle = new ProcessHandle(pid, Program.MinProcessQueryRights | ProcessAccess.VmRead))
                {
                    var commandLine = phandle.GetCommandLine();
                    var currentDirectory = phandle.GetPebString(PebOffset.CurrentDirectoryPath);

                    sb.AppendLine("Command line: " + commandLine);
                    sb.AppendLine("Current directory: " + currentDirectory);
                }
            }
            catch (Exception ex)
            {
                sb.AppendLine("PEB info section failed! " + ex.Message);
            }

            sb.AppendLine();
            sb.AppendLine("Modules:");
            sb.AppendLine();

            try
            {
                using (var phandle = new ProcessHandle(pid, Program.MinProcessQueryRights | ProcessAccess.VmRead))
                {
                    foreach (var module in phandle.GetModules())
                    {
                        sb.AppendLine(module.FileName);
                        sb.Append("    [0x" + module.BaseAddress.ToInt32().ToString("x") + ", ");
                        sb.AppendLine(Utils.FormatSize(module.Size) + "] ");
                        sb.AppendLine("    Flags: " + module.Flags.ToString());

                        try
                        {
                            var fileInfo = FileVersionInfo.GetVersionInfo(module.FileName);

                            sb.AppendLine("    Description: " + fileInfo.FileDescription);
                            sb.AppendLine("    Company: " + fileInfo.CompanyName);
                            sb.AppendLine("    Version: " + fileInfo.FileVersion);
                        }
                        catch (Exception ex2)
                        {
                            sb.AppendLine("    Version info failed! " + ex2.Message);
                        }

                        sb.AppendLine();
                    }
                }
            }
            catch (Exception ex)
            {
                sb.AppendLine("Modules section failed! " + ex.Message);
            }

            sb.AppendLine("Token:");
            sb.AppendLine();

            try
            {
                using (var phandle = new ProcessHandle(pid, Program.MinProcessQueryRights))
                using (var thandle = phandle.GetToken(TokenAccess.Query))
                {
                    sb.AppendLine("User: " + thandle.GetUser().GetFullName(true));
                    sb.AppendLine("Owner: " + thandle.GetOwner().GetFullName(true));
                    sb.AppendLine("Primary group: " + thandle.GetPrimaryGroup().GetFullName(true));

                    foreach (var group in thandle.GetGroups())
                    {
                        sb.AppendLine("Group " + group.GetFullName(true));
                    }

                    foreach (var privilege in thandle.GetPrivileges())
                    {
                        sb.AppendLine("Privilege " + privilege.Name + ": " + privilege.Attributes.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                sb.AppendLine("Token section failed! " + ex.Message);
            }

            sb.AppendLine();
            sb.AppendLine("Environment:");
            sb.AppendLine();

            try
            {
                using (var phandle = new ProcessHandle(pid, ProcessAccess.QueryInformation | ProcessAccess.VmRead))
                {
                    var vars = phandle.GetEnvironmentVariables();

                    foreach (var kvp in vars)
                    {
                        sb.AppendLine(kvp.Key + " = " + kvp.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                sb.AppendLine("Environment section failed! " + ex.Message);
            }

            sb.AppendLine();
            sb.AppendLine("Handles:");
            sb.AppendLine();

            try
            {
                using (var phandle = new ProcessHandle(pid, ProcessAccess.DupHandle))
                {
                    var handles = Windows.GetHandles();

                    foreach (var handle in handles)
                    {
                        if (handle.ProcessId != pid)
                            continue;

                        sb.Append("[0x" + handle.Handle.ToString("x") + ", ");

                        try
                        {
                            var info = handle.GetHandleInfo(phandle);

                            sb.Append(info.TypeName + "] ");
                            sb.AppendLine(!string.IsNullOrEmpty(info.BestName) ? info.BestName : "(no name)");
                        }
                        catch (Exception ex2)
                        {
                            sb.Append(handle.ObjectTypeNumber.ToString() + "] ");
                            sb.AppendLine("Error: " + ex2.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                sb.AppendLine("Handles section failed! " + ex.Message);
            }

            return sb.ToString();
        }
    }
}
