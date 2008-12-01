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
    public struct ProcessItem
    {
        public int PID;
        public Process Process;

        public Icon Icon;
        public string Name;
        public string MemoryUsage;
        public string Username;
        public string UsernameWithDomain;
    }

    public class ProcessProvider : Provider<int, ProcessItem>
    {
        public ProcessProvider()
            : base()
        {      
            this.ProviderUpdate += new ProviderUpdateOnce(UpdateOnce);   
        }

        private void UpdateOnce()
        {
            Process[] processes = Process.GetProcesses();
            Dictionary<int, Win32.WTS_PROCESS_INFO> tsProcesses = new Dictionary<int, Win32.WTS_PROCESS_INFO>();
            List<int> pids = new List<int>();
            Dictionary<int, ProcessItem> newdictionary = new Dictionary<int, ProcessItem>();

            foreach (Win32.WTS_PROCESS_INFO process in Win32.TSEnumProcesses())
                tsProcesses.Add(process.ProcessID, process);

            foreach (int key in Dictionary.Keys)
                newdictionary.Add(key, Dictionary[key]);

            foreach (Process p in processes)
                pids.Add(p.Id);

            // look for dead processes
            foreach (int pid in Dictionary.Keys)
            {
                if (!pids.Contains(pid))
                {                 
                    this.CallDictionaryRemoved(this.Dictionary[pid]);
                    newdictionary.Remove(pid);
                }
            }

            // look for new processes
            foreach (Process p in processes)
            {
                if (!Dictionary.ContainsKey(p.Id))
                {
                    ProcessItem item = new ProcessItem();

                    item.PID = p.Id;
                    item.Process = p;

                    try
                    {
                        item.Icon = (Icon)Win32.GetProcessIcon(p).Clone();
                    }
                    catch
                    { }

                    try
                    {
                        if (p.Id == 0)
                            item.Name = "System Idle Process";
                        else
                            item.Name = p.MainModule.ModuleName;
                    }
                    catch
                    {
                        item.Name = Win32.GetNameFromPID(p.Id);

                        if (item.Name == "(error)" || item.Name == "(unknown)")
                        {
                            try
                            {
                                item.Name = "(" + p.ProcessName + ")";
                            }
                            catch
                            {
                                item.Name = "(unknown)";
                            }
                        }
                    }

                    try
                    {
                        item.MemoryUsage = Misc.GetNiceSizeName(p.PrivateMemorySize64);
                    }
                    catch
                    { }

                    try
                    {
                        //item.Username = Win32.GetProcessUsername(p.Handle.ToInt32(),
                        //    Properties.Settings.Default.ShowAccountDomains);
                        //item.UsernameWithDomain = Win32.GetProcessUsername(p.Handle.ToInt32(),
                        //    true);
                        item.Username = Win32.GetAccountName(tsProcesses[p.Id],
                            Properties.Settings.Default.ShowAccountDomains);
                        item.UsernameWithDomain = Win32.GetAccountName(tsProcesses[p.Id], true);
                    }
                    catch
                    {
                        item.Username = "(" + Win32.GetLastErrorMessage() + ")";
                        item.UsernameWithDomain = "(" + Win32.GetLastErrorMessage() + ")";
                    }

                    newdictionary.Add(p.Id, item);
                    this.CallDictionaryAdded(item);
                }
                // look for modified processes
                else
                {
                    ProcessItem item = Dictionary[p.Id];
                    ProcessItem newitem = new ProcessItem();

                    newitem.Icon = item.Icon;
                    newitem.Name = item.Name;
                    newitem.PID = item.PID;
                    newitem.Process = item.Process;

                    try
                    {
                        newitem.MemoryUsage = Misc.GetNiceSizeName(p.PrivateMemorySize64);
                    }
                    catch
                    { }

                    try
                    {
                        //newitem.Username = Win32.GetProcessUsername(p.Handle.ToInt32(),
                        //    Properties.Settings.Default.ShowAccountDomains);
                        //newitem.UsernameWithDomain = Win32.GetProcessUsername(p.Handle.ToInt32(),
                        //    true);
                        newitem.Username = Win32.GetAccountName(tsProcesses[p.Id],
                            Properties.Settings.Default.ShowAccountDomains);
                        newitem.UsernameWithDomain = Win32.GetAccountName(tsProcesses[p.Id], true);
                    }
                    catch
                    {
                        newitem.Username = "(" + Win32.GetLastErrorMessage() + ")";
                        newitem.UsernameWithDomain = "(" + Win32.GetLastErrorMessage() + ")";
                    }

                    if (newitem.MemoryUsage != item.MemoryUsage ||
                        newitem.Username != item.Username)
                    {
                        newdictionary[p.Id] = newitem;
                        this.CallDictionaryModified(item, newitem);
                    }
                }
            }

            Dictionary = newdictionary;
        }
    }
}
