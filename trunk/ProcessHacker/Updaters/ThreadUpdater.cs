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

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using System;

namespace ProcessHacker
{
    public partial class HackerWindow : Form
    {
        Hashtable threadCPUTime = new Hashtable();
        Hashtable threadPriority = new Hashtable();
        Hashtable threadState = new Hashtable();
        Hashtable threadWaitReason = new Hashtable();

        /// <summary>
        /// Finds new, removed and modified threads to add to the task queue.
        /// Runs on its own thread.
        /// </summary>
        public void ThreadListUpdater()
        {
            while (true)
            {
                DoThreadListUpdate();

                Thread.Sleep(RefreshInterval);
            }
        }

        /// <summary>
        /// The actual updater.
        /// </summary>
        private void DoThreadListUpdate()
        {
            if (processSelectedItems == 1)
            {
                try
                {
                    Process process = Process.GetProcessById(processSelectedPID);
                    ProcessThreadCollection threads = process.Threads;
                    List<int> newtids = new List<int>();
                    List<int> runningtids = new List<int>();

                    lock (tids)
                    {
                        // if this is a different process, reset everything.
                        if (processSelectedPID != lastSelectedPID)
                        {
                            tids = new List<int>();
                            threadState = new Hashtable();
                            threadCPUTime = new Hashtable();
                            threadPriority = new Hashtable();
                            threadWaitReason = new Hashtable();
                            lastSelectedPID = processSelectedPID;
                        }

                        // look for new threads
                        foreach (ProcessThread t in threads)
                        {
                            if (!tids.Contains(t.Id))
                            {
                                UpdateTask task = new UpdateTask();

                                task.Type = UpdateTaskType.Add;
                                task.Thread = t;

                                threadState.Add(t.Id, t.ThreadState.ToString());
                                threadCPUTime.Add(t.Id, string.Format("{0:d3}:{1:d2}:{2:d3}",
                                t.TotalProcessorTime.Minutes,
                                t.TotalProcessorTime.Seconds,
                                t.TotalProcessorTime.Milliseconds));
                                threadPriority.Add(t.Id, t.PriorityLevel.ToString());
                                threadWaitReason.Add(t.Id, t.WaitReason.ToString());

                                lock (threadQueue)
                                    threadQueue.Enqueue(task);

                                tids.Add(t.Id);
                            }

                            string state = t.ThreadState.ToString();
                            string cputime = string.Format("{0:d3}:{1:d2}:{2:d3}",
                                t.TotalProcessorTime.Minutes,
                                t.TotalProcessorTime.Seconds,
                                t.TotalProcessorTime.Milliseconds);
                            string priority = t.PriorityLevel.ToString();
                            string waitreason = t.WaitReason.ToString();

                            if (threadState[t.Id].ToString() != state ||
                                threadCPUTime[t.Id].ToString() != cputime ||
                                threadPriority[t.Id].ToString() != priority ||
                                threadWaitReason[t.Id].ToString() != waitreason)
                            {
                                UpdateTask task = new UpdateTask();

                                task.Type = UpdateTaskType.Modify;
                                task.TID = t.Id;

                                threadState[t.Id] = state;
                                threadCPUTime[t.Id] = cputime;
                                threadPriority[t.Id] = priority;
                                threadWaitReason[t.Id] = waitreason;

                                lock (threadQueue)
                                    threadQueue.Enqueue(task);
                            }

                            runningtids.Add(t.Id);
                        }

                        // you can't clone a list for some reason...
                        foreach (int tid in tids)
                            newtids.Add(tid);

                        // look for threads which don't exist anymore
                        foreach (int tid in tids)
                        {
                            if (runningtids.Contains(tid))
                                continue;

                            UpdateTask task = new UpdateTask();

                            task.Type = UpdateTaskType.Remove;
                            task.TID = tid;

                            lock (threadQueue)
                                threadQueue.Enqueue(task);

                            threadState.Remove(tid);
                            threadCPUTime.Remove(tid);
                            threadPriority.Remove(tid);
                            threadWaitReason.Remove(tid);
                            newtids.Remove(tid);
                        }

                        tids = newtids;
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// Performs the tasks on the queue.
        /// Runs on the main thread.
        /// </summary>
        private void ThreadQueueUpdated()
        {
            lock (threadQueue)
            {
                while (threadQueue.Count > 0)
                {
                    UpdateTask task = threadQueue.Dequeue();

                    if (task.Type == UpdateTaskType.Add)
                    {
                        ListViewItem item = new ListViewItem();

                        try
                        {
                            item.SubItems.Add(new ListViewItem.ListViewSubItem());
                            item.SubItems.Add(new ListViewItem.ListViewSubItem());
                            item.SubItems.Add(new ListViewItem.ListViewSubItem());
                            item.SubItems.Add(new ListViewItem.ListViewSubItem());

                            item.SubItems[0].Text = task.Thread.Id.ToString();
                            item.SubItems[1].Text = task.Thread.ThreadState.ToString();
                            item.SubItems[2].Text = Misc.GetNiceTimeSpan(task.Thread.TotalProcessorTime);
                            item.SubItems[3].Text = task.Thread.PriorityLevel.ToString();
                            item.SubItems[4].Text = task.Thread.WaitReason.ToString();
                        }
                        catch
                        {
                            continue;
                        }

                        listThreads.Items.Add(item);
                    }
                    else if (task.Type == UpdateTaskType.Modify)
                    {
                        foreach (ListViewItem item in listThreads.Items)
                        {
                            if (item.SubItems[0].Text == task.TID.ToString())
                            {
                                if (threadState.ContainsKey(task.TID))
                                    item.SubItems[1].Text = threadState[task.TID].ToString();
                                if (threadCPUTime.ContainsKey(task.TID))
                                    item.SubItems[2].Text = threadCPUTime[task.TID].ToString();
                                if (threadPriority.ContainsKey(task.TID))
                                    item.SubItems[3].Text = threadPriority[task.TID].ToString();
                                if (threadWaitReason.ContainsKey(task.TID))
                                    item.SubItems[4].Text = threadWaitReason[task.TID].ToString();

                                break;
                            }
                        }
                    }
                    else if (task.Type == UpdateTaskType.Remove)
                    {
                        foreach (ListViewItem item in listThreads.Items)
                        {
                            if (item.SubItems[0].Text == task.TID.ToString())
                            {
                                item.Remove();

                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}
