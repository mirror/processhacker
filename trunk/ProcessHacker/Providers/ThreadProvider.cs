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
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace ProcessHacker
{
    public struct ThreadItem
    {
        public int TID;
        public ProcessThread Thread;

        public string CPUTime;
        public string Priority;
        public string State;
    }

    public class ThreadProvider : Provider<int, ThreadItem>
    {
        private int _pid;

        public ThreadProvider(int PID)
            : base()
        {
            _pid = PID;

            this.ProviderUpdate += new ProviderUpdateOnce(UpdateOnce);   
        }

        private void UpdateOnce()
        {
            Process process = Process.GetProcessById(_pid);
            ProcessThreadCollection threads = process.Threads;
            List<int> tids = new List<int>();
            Dictionary<int, ThreadItem> newdictionary = new Dictionary<int, ThreadItem>();

            foreach (int key in Dictionary.Keys)
                newdictionary.Add(key, Dictionary[key]);

            foreach (ProcessThread t in threads)
                tids.Add(t.Id);

            // look for dead threads
            foreach (int tid in Dictionary.Keys)
            {
                if (!tids.Contains(tid))
                {
                    this.CallDictionaryRemoved(this.Dictionary[tid]);
                    newdictionary.Remove(tid);
                }
            }

            // look for new threads
            foreach (ProcessThread t in threads)
            {
                if (!Dictionary.ContainsKey(t.Id))
                {
                    ThreadItem item = new ThreadItem();

                    item.TID = t.Id;
                    item.Thread = t;

                    try { item.State = t.ThreadState.ToString(); }
                    catch { }
                    try { item.CPUTime = Misc.GetNiceTimeSpan(t.TotalProcessorTime); }
                    catch { }
                    try { item.Priority = t.PriorityLevel.ToString(); }
                    catch { }

                    newdictionary.Add(t.Id, item);
                    this.CallDictionaryAdded(item);
                }
                // look for modified threads
                else
                {
                    ThreadItem item = Dictionary[t.Id];
                    ThreadItem newitem = new ThreadItem();

                    newitem.TID = item.TID;
                    newitem.Thread = item.Thread;

                    try { newitem.State = t.ThreadState.ToString(); }
                    catch { }
                    try { newitem.CPUTime = Misc.GetNiceTimeSpan(t.TotalProcessorTime); }
                    catch { }
                    try { newitem.Priority = t.PriorityLevel.ToString(); }
                    catch { }

                    if (newitem.State != item.State ||
                        newitem.CPUTime != item.CPUTime ||
                        newitem.Priority != item.Priority)
                    {
                        newdictionary[t.Id] = newitem;
                        this.CallDictionaryModified(item, newitem);
                    }
                }
            }

            Dictionary = newdictionary;
        }
    }
}
