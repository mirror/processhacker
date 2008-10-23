/*
 * Process Hacker
 * 
 * Copyright (C) 2008 wj32
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace ProcessHacker
{
    public partial class HackerWindow : Form
    {
        Hashtable processMemoryUsage = new Hashtable();
        Hashtable processUsername = new Hashtable();
        Hashtable processTotalMilliseconds = new Hashtable();

        bool processListUpdatedOnce = false;

        /// <summary>
        /// Finds new, removed and modified processes.
        /// Also calls Windows APIs which take a long time.
        /// Runs on its own thread.
        /// </summary>
        public void ProcessListUpdater()
        {
            while (true)
            {
                Process[] processes = Process.GetProcesses();
                List<int> newpids = new List<int>();
                List<int> runningpids = new List<int>();

                lock (pids)
                {
                    // look for new processes
                    foreach (Process p in processes)
                    {
                        if (!pids.Contains(p.Id))
                        {
                            UpdateTask task = new UpdateTask();
                            Win32.SHFILEINFO shinfo = new Win32.SHFILEINFO();
                            IntPtr icon;
                            string memoryUsage = "";
                            string username = "";

                            task.Type = UpdateTaskType.Add;
                            task.Process = p;

                            try
                            {
                                if (Win32.SHGetFileInfo(Misc.GetRealPath(p.MainModule.FileName), 0, ref shinfo,
                                      (uint)Marshal.SizeOf(shinfo),
                                       Win32.SHGFI_ICON |
                                       Win32.SHGFI_SMALLICON) == 0)
                                {
                                    task.SmallIcon = 0;
                                }
                                else
                                {
                                    icon = shinfo.hIcon;

                                    this.Invoke(new AddIconCallback(AddIcon), System.Drawing.Icon.FromHandle(icon));

                                    task.SmallIcon = imageIndex++;
                                }
                            }
                            catch
                            {
                                task.SmallIcon = 0;
                            }

                            try
                            {
                                memoryUsage = Misc.GetNiceSizeName(p.PrivateMemorySize64);
                            }
                            catch { }

                            try
                            {
                                username = Win32.GetProcessUsername(p.Handle.ToInt32(), 
                                    Properties.Settings.Default.ShowProcessDomains);
                            }
                            catch { }

                            try { processMemoryUsage.Add(p.Id, memoryUsage); }
                            catch { }
                            try { processUsername.Add(p.Id, username); }     
                            catch { }
                            try { processTotalMilliseconds.Add(p.Id, 0); }  
                            catch { }
                            
                            lock (processQueue)
                                processQueue.Enqueue(task);

                            pids.Add(p.Id);
                        }

                        runningpids.Add(p.Id);
                    }

                    // clone
                    foreach (int pid in pids)
                        newpids.Add(pid);

                    // look for processes that are no longer running
                    foreach (int pid in pids)
                    {
                        if (runningpids.Contains(pid))
                            continue;

                        UpdateTask task = new UpdateTask();

                        task.Type = UpdateTaskType.Remove;
                        task.PID = pid;

                        lock (processQueue)
                            processQueue.Enqueue(task);

                        processMemoryUsage.Remove(pid);
                        processUsername.Remove(pid);
                        processTotalMilliseconds.Remove(pid);
                        newpids.Remove(pid);
                    }

                    pids = newpids;

                    // look for processes with different information (like memory usage)
                    foreach (int pid in pids)
                    {
                        try
                        {
                            Process p = Process.GetProcessById(pid);

                            if ((string)processMemoryUsage[p.Id] != Misc.GetNiceSizeName(p.PrivateMemorySize64))
                            {
                                UpdateTask task = new UpdateTask();

                                task.Type = UpdateTaskType.Modify;
                                task.PID = pid;

                                processMemoryUsage[p.Id] = Misc.GetNiceSizeName(p.PrivateMemorySize64);

                                lock (processQueue)
                                    processQueue.Enqueue(task);
                            }
                        }
                        catch { }
                    }
                }

                Thread.Sleep(RefreshInterval);
            }
        }

        /// <summary>
        /// Performs tasks on the queue.
        /// Runs on the main thread.
        /// </summary>
        private void ProcessQueueUpdated()
        {
            lock (processQueue)
            {
                while (processQueue.Count > 0)
                {
                    UpdateTask task = processQueue.Dequeue();

                    if (task.Type == UpdateTaskType.Add)
                    {
                        ListViewItem item = new ListViewItem();
                        string memoryUsage = "";
                        string processUsername = "";

                        lock (processMemoryUsage)
                        {
                            try
                            {
                                memoryUsage = processMemoryUsage[task.Process.Id].ToString();
                            }
                            catch { }
                        }

                        lock (processUsername)
                        {
                            try
                            {
                                processUsername = processUsername[task.Process.Id].ToString();
                            }
                            catch { }
                        }

                        item.SubItems.Add(new ListViewItem.ListViewSubItem());
                        item.SubItems.Add(new ListViewItem.ListViewSubItem());
                        item.SubItems.Add(new ListViewItem.ListViewSubItem());

                        try
                        {
                            item.SubItems[0].Text = task.Process.MainModule.ModuleName;
                        }
                        catch
                        {
                            item.SubItems[0].Text = Win32.GetNameFromPID(task.Process.Id);

                            if (item.SubItems[0].Text == "(error)" || item.SubItems[0].Text == "(unknown)")
                            {
                                try
                                {
                                    item.SubItems[0].Text = "(" + task.Process.ProcessName + ")";
                                }
                                catch
                                {
                                    item.SubItems[0].Text = "(unknown)";
                                }
                            }
                        }

                        try
                        {
                            item.ToolTipText =
                                FileVersionInfo.GetVersionInfo(
                                Misc.GetRealPath(task.Process.MainModule.FileName)).FileDescription + " - " +
                                Misc.GetRealPath(task.Process.MainModule.FileName);
                        }
                        catch
                        {

                        }

                        item.SubItems[1].Text = task.Process.Id.ToString();
                        item.SubItems[2].Text = memoryUsage;
                        item.SubItems[3].Text = processUsername;

                        item.ImageIndex = task.SmallIcon;

                        listProcesses.Items.Add(item);

                        item.Selected = false;
                    }
                    else if (task.Type == UpdateTaskType.Modify)
                    {
                        foreach (ListViewItem item in listProcesses.Items)
                        {
                            if (task.PID.ToString() == item.SubItems[1].Text)
                            {
                                if (processMemoryUsage.ContainsKey(task.PID))
                                    item.SubItems[2].Text = processMemoryUsage[task.PID].ToString();

                                break;
                            }
                        }
                    }
                    else if (task.Type == UpdateTaskType.Remove)
                    {
                        foreach (ListViewItem item in listProcesses.Items)
                        {
                            int index = item.Index;

                            if (item.SubItems[1].Text == task.PID.ToString())
                            {
                                bool selected = item.Selected;

                                item.Remove();

                                if (selected)
                                {
                                    DeselectAll(listProcesses);

                                    if (listProcesses.Items.Count == 0)
                                    {
                                        listModules.Items.Clear();
                                        listThreads.Items.Clear();
                                    }
                                    else if (index < listProcesses.Items.Count)
                                    {
                                        listProcesses.Items[index].Selected = true;
                                    }
                                    else if (index - 1 >= 0)
                                    {
                                        listProcesses.Items[index - 1].Selected = true;
                                    }

                                    UpdateProcessExtra();
                                }

                                break;
                            }
                        }
                    }
                }
            }

            if (!processListUpdatedOnce)
            {
                try
                {
                    this.Invoke(new MethodInvoker(delegate { Cursor = Cursors.Default; listProcesses.EndUpdate(); }));
                    processListUpdatedOnce = true;
                }
                catch
                { }
            }
        }
    }
}
