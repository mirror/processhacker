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

        public Icon Icon;
        public string CmdLine;
        public float CPUUsage;
        public long MemoryUsage;
        public string Name;
        public string Username;

        public Win32.TOKEN_ELEVATION_TYPE ElevationType;
        public bool IsElevated;
        public bool IsBeingDebugged;
        public bool IsVirtualizationEnabled;
        public ulong LastTime;
        public int SessionId;
        public int ParentPID;
        public int IconAttempts;

        public Win32.TokenHandle TokenQueryHandle;
        public Win32.ProcessHandle ProcessQueryHandle;
        public Win32.ProcessHandle ProcessQueryLimitedHandle;
    }

    public class ProcessProvider : Provider<int, ProcessItem>
    {
        private ulong _lastSysTime;

        public ProcessProvider()
            : base()
        {      
            this.ProviderUpdate += new ProviderUpdateOnce(UpdateOnce);

            ulong[] systemTimes = Win32.GetSystemTimes();

            _lastSysTime = systemTimes[1] / 10000 + systemTimes[2] / 10000;
        }

        private void UpdateOnce()
        {
            Process[] processes = Process.GetProcesses();
            Dictionary<int, Win32.WTS_PROCESS_INFO> tsProcesses = new Dictionary<int, Win32.WTS_PROCESS_INFO>();
            Dictionary<int, Process> procs = new Dictionary<int, Process>();
            Dictionary<int, ProcessItem> newdictionary = new Dictionary<int, ProcessItem>(this.Dictionary);
            Win32.WtsEnumProcessesData wtsEnumData = Win32.TSEnumProcesses();

            ulong[] systemTimes = Win32.GetSystemTimes();
            ulong thisSysTime = systemTimes[1] / 10000 + systemTimes[2] / 10000;
            ulong sysTime = thisSysTime - _lastSysTime;

            _lastSysTime = thisSysTime;

            foreach (Win32.WTS_PROCESS_INFO process in wtsEnumData.Processes)
                tsProcesses.Add(process.ProcessID, process);

            foreach (Process p in processes)
                procs.Add(p.Id, p);

            // look for dead processes
            foreach (int pid in Dictionary.Keys)
            {
                if (!procs.ContainsKey(pid))
                {
                    ProcessItem item = this.Dictionary[pid];

                    this.CallDictionaryRemoved(item);

                    if (item.ProcessQueryHandle != null)
                        item.ProcessQueryHandle.Dispose();

                    if (item.ProcessQueryLimitedHandle != null)
                        item.ProcessQueryLimitedHandle.Dispose();

                    if (item.TokenQueryHandle != null)
                        item.TokenQueryHandle.Dispose();

                    newdictionary.Remove(pid);
                }
            }

            // look for new processes
            foreach (int pid in procs.Keys)
            {
                Process p = procs[pid];

                if (!Dictionary.ContainsKey(pid))
                { 
                    ProcessItem item = new ProcessItem();

                    item.PID = pid;

                    try
                    {
                        item.SessionId = Win32.GetProcessSessionId(pid);
                    }
                    catch
                    {
                        item.SessionId = -1;
                    }

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
                        item.Name = Win32.GetNameFromPID(pid);

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
                        item.MemoryUsage = p.PrivateMemorySize64;
                    }
                    catch
                    { }

                    try
                    {
                        item.ProcessQueryHandle = new Win32.ProcessHandle(pid, Win32.PROCESS_RIGHTS.PROCESS_QUERY_INFORMATION);
                        
                        try
                        {
                            item.IsBeingDebugged = item.ProcessQueryHandle.IsBeingDebugged();
                        }
                        catch
                        { }
                    }
                    catch
                    { }

                    try
                    {
                        item.ProcessQueryLimitedHandle =  new Win32.ProcessHandle(pid, Program.MinProcessQueryRights);
                        
                        try
                        {
                            ulong[] times = Win32.GetProcessTimes(item.ProcessQueryLimitedHandle);

                            item.LastTime = times[2] / 10000 + times[3] / 10000;
                        }
                        catch
                        { }

                        try
                        {
                            item.TokenQueryHandle = item.ProcessQueryLimitedHandle.GetToken(Win32.TOKEN_RIGHTS.TOKEN_QUERY);

                            try { item.Username = item.TokenQueryHandle.GetUsername(true); }
                            catch { }
                            try { item.ElevationType = item.TokenQueryHandle.GetElevationType(); }
                            catch { }
                            try { item.IsElevated = item.TokenQueryHandle.IsElevated(); }
                            catch { }
                            try { item.IsVirtualizationEnabled = item.TokenQueryHandle.IsVirtualizationEnabled(); }
                            catch { }
                        }
                        catch
                        { }

                        try
                        {
                            item.ParentPID = item.ProcessQueryLimitedHandle.GetParentPID();
                        }
                        catch
                        {
                            item.ParentPID = -1;
                        }
                    }
                    catch
                    { }

                    if (pid == 0 || pid == 4)
                    {
                        item.Username = "NT AUTHORITY\\SYSTEM";
                    }

                    if (item.Username == null)
                    {
                        try
                        {
                            item.Username = Win32.GetAccountName(tsProcesses[pid].SID, true);
                        }
                        catch
                        { }
                    }

                    if (pid == 0)
                        item.LastTime = systemTimes[0] / 10000;

                    try
                    {
                        using (Win32.ProcessHandle phandle =
                            new Win32.ProcessHandle(pid,
                                Program.MinProcessQueryRights | Win32.PROCESS_RIGHTS.PROCESS_VM_READ))
                            item.CmdLine = phandle.GetCommandLine();
                    }
                    catch
                    { }

                    newdictionary.Add(pid, item);
                    this.CallDictionaryAdded(item);
                }
                // look for modified processes
                else
                {
                    ProcessItem item = Dictionary[pid];
                    ProcessItem newitem = new ProcessItem();

                    newitem.CmdLine = item.CmdLine;
                    newitem.ElevationType = item.ElevationType;
                    newitem.Icon = item.Icon;
                    newitem.IconAttempts = item.IconAttempts;
                    newitem.IsElevated = item.IsElevated;
                    newitem.IsVirtualizationEnabled = item.IsVirtualizationEnabled;
                    newitem.Name = item.Name;
                    newitem.ParentPID = item.ParentPID;
                    newitem.PID = item.PID;
                    newitem.SessionId = item.SessionId;
                    newitem.Username = item.Username;

                    newitem.ProcessQueryHandle = item.ProcessQueryHandle;
                    newitem.ProcessQueryLimitedHandle = item.ProcessQueryLimitedHandle;
                    newitem.TokenQueryHandle = item.TokenQueryHandle;

                    try
                    {
                        newitem.MemoryUsage = p.PrivateMemorySize64;
                    }
                    catch
                    { }

                    if (newitem.Icon == null && newitem.IconAttempts < 5)
                    {
                        try
                        {
                            newitem.Icon = (Icon)Win32.GetProcessIcon(p).Clone();
                        }
                        catch
                        { }

                        newitem.IconAttempts++;
                    }

                    try
                    {
                        newitem.IsBeingDebugged = item.ProcessQueryHandle.IsBeingDebugged();
                    }
                    catch
                    { }

                    try
                    {
                        newitem.IsVirtualizationEnabled = item.TokenQueryHandle.IsVirtualizationEnabled();
                    }
                    catch
                    { }

                    try
                    {
                        ulong[] times = Win32.GetProcessTimes(item.ProcessQueryLimitedHandle);

                        newitem.LastTime = times[2] / 10000 + times[3] / 10000;
                        newitem.CPUUsage = ((float)(newitem.LastTime - item.LastTime) * 100 / sysTime);
                    }
                    catch
                    { }

                    if (pid == 0)
                    {
                        newitem.LastTime = systemTimes[0] / 10000;
                        newitem.CPUUsage = ((float)(newitem.LastTime - item.LastTime) * 100 / sysTime);
                    }

                    if (newitem.MemoryUsage != item.MemoryUsage ||
                        newitem.CPUUsage != item.CPUUsage || 
                        newitem.IsBeingDebugged != item.IsBeingDebugged)
                    {
                        newdictionary[pid] = newitem;
                        this.CallDictionaryModified(item, newitem);
                    }
                }
            }

            Dictionary = newdictionary;
        }
    }
}
