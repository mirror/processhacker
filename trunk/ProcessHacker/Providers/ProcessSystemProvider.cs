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
using System.Runtime.InteropServices;

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
        public Win32.SYSTEM_PROCESS_INFORMATION Process;
        public Win32.SYSTEM_THREAD_INFORMATION[] Threads;

        public Win32.TOKEN_ELEVATION_TYPE ElevationType;
        public bool IsElevated;
        public bool IsBeingDebugged;
        public bool IsVirtualizationEnabled;
        public long LastTime;
        public int SessionId;
        public int ParentPID;
        public int IconAttempts;

        public Win32.TokenHandle TokenQueryHandle;
        public Win32.ProcessHandle ProcessQueryHandle;
        public Win32.ProcessHandle ProcessQueryLimitedHandle;
        public Win32.ProcessHandle ProcessQueryLimitedVmReadHandle;
    }

    public class ProcessSystemProvider : Provider<int, ProcessItem>
    {
        private long _lastSysTime;

        public ProcessSystemProvider()
            : base()
        {      
            this.ProviderUpdate += new ProviderUpdateOnce(UpdateOnce);

            Win32.SYSTEM_BASIC_INFORMATION basic = new Win32.SYSTEM_BASIC_INFORMATION();
            int retLen;

            Win32.ZwQuerySystemInformation(Win32.SYSTEM_INFORMATION_CLASS.SystemBasicInformation, ref basic,
                Marshal.SizeOf(basic), out retLen);
            this.System = basic;

            this.UpdateProcessorPerf();
            _lastSysTime = this.ProcessorPerf.KernelTime + this.ProcessorPerf.UserTime;
        }

        public Win32.SYSTEM_BASIC_INFORMATION System { get; private set; }
        public Win32.SYSTEM_PERFORMANCE_INFORMATION Performance { get; private set; }
        public Win32.SYSTEM_PROCESSOR_PERFORMANCE_INFORMATION ProcessorPerf { get; private set; }

        private void UpdateProcessorPerf()
        {
            int retLen;
            Win32.SYSTEM_PROCESSOR_PERFORMANCE_INFORMATION procPerf = new Win32.SYSTEM_PROCESSOR_PERFORMANCE_INFORMATION();

            Win32.ZwQuerySystemInformation(Win32.SYSTEM_INFORMATION_CLASS.SystemProcessorTimes,
                ref procPerf, Marshal.SizeOf(procPerf), out retLen);
            this.ProcessorPerf = procPerf;
        }

        private void UpdatePerformance()
        {
            int retLen;
            Win32.SYSTEM_PERFORMANCE_INFORMATION performance = new Win32.SYSTEM_PERFORMANCE_INFORMATION();

            Win32.ZwQuerySystemInformation(Win32.SYSTEM_INFORMATION_CLASS.SystemPerformanceInformation,
                ref performance, Marshal.SizeOf(performance), out retLen);
            this.Performance = performance;
        }

        private void UpdateOnce()
        {
            this.UpdatePerformance();
            this.UpdateProcessorPerf();

            Dictionary<int, int> tsProcesses = new Dictionary<int,int>();
            Dictionary<int, Win32.SystemProcess> procs = Win32.EnumProcesses();
            Dictionary<int, ProcessItem> newdictionary = new Dictionary<int, ProcessItem>(this.Dictionary);
            Win32.WtsEnumProcessesFastData wtsEnumData = Win32.TSEnumProcessesFast();

            long thisSysTime = this.ProcessorPerf.KernelTime + this.ProcessorPerf.UserTime;
            long sysTime = thisSysTime - _lastSysTime;

            _lastSysTime = thisSysTime;

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
                Win32.SYSTEM_PROCESS_INFORMATION processInfo = procs[pid].Process;
                Process p = Process.GetProcessById(pid);

                if (!Dictionary.ContainsKey(pid))
                {
                    ProcessItem item = new ProcessItem();

                    item.PID = pid;
                    item.LastTime = processInfo.KernelTime + processInfo.UserTime;
                    item.MemoryUsage = processInfo.VirtualMemoryCounters.PrivatePageCount;
                    item.Process = processInfo;
                    item.SessionId = processInfo.SessionId;

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
                            item.Name = procs[pid].Name;
                    }
                    catch
                    {
                        try
                        {
                            item.Name = p.MainModule.ModuleName;
                        }
                        catch
                        {
                            item.Name = Win32.GetNameFromPID(pid);
                        }
                    }

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
                            item.TokenQueryHandle = item.ProcessQueryLimitedHandle.GetToken(Win32.TOKEN_RIGHTS.TOKEN_QUERY);

                            try { item.Username = item.TokenQueryHandle.GetUser().GetName(true); }
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

                            // check the parent's creation time to see if it's actually the parent
                            try
                            {
                                DateTime thisStartTime = p.StartTime;
                                DateTime parentStartTime = Process.GetProcessById(item.ParentPID).StartTime;

                                if (parentStartTime > thisStartTime)
                                    item.ParentPID = -1; // parent was started later than child! it's a fake.
                            }
                            catch
                            { } // item.ParentPID = -1;
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
                        if (tsProcesses.Count == 0)
                        {
                            // delay loading until this point
                            for (int i = 0; i < wtsEnumData.PIDs.Length; i++)
                                tsProcesses.Add(wtsEnumData.PIDs[i], wtsEnumData.SIDs[i]);
                        }

                        try
                        {
                            item.Username = Win32.GetAccountName(tsProcesses[pid], true);
                        }
                        catch
                        { }
                    }

                    if (pid == 0)
                        item.LastTime = this.ProcessorPerf.IdleTime;

                    try
                    {
                        item.ProcessQueryLimitedVmReadHandle =
                            new Win32.ProcessHandle(pid,
                                Program.MinProcessQueryRights | Win32.PROCESS_RIGHTS.PROCESS_VM_READ);

                        item.CmdLine = item.ProcessQueryLimitedVmReadHandle.GetCommandLine();
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

                    newitem = item;
                    newitem.LastTime = processInfo.KernelTime + processInfo.UserTime;
                    newitem.MemoryUsage = processInfo.VirtualMemoryCounters.PrivatePageCount;

                    try
                    {
                        newitem.CPUUsage = ((float)(newitem.LastTime - item.LastTime) * 100 / sysTime) /
                            this.System.NumberOfProcessors;
                    }
                    catch
                    { }

                    if (pid == 0)
                    {
                        newitem.LastTime = this.ProcessorPerf.IdleTime;
                        newitem.CPUUsage = ((float)(newitem.LastTime - item.LastTime) * 100 / sysTime);
                    }

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

                    if (newitem.MemoryUsage != item.MemoryUsage ||
                        newitem.CPUUsage != item.CPUUsage || 
                        newitem.IsBeingDebugged != item.IsBeingDebugged ||
                        newitem.IsVirtualizationEnabled != item.IsVirtualizationEnabled)
                    {
                        newdictionary[pid] = newitem;
                        this.CallDictionaryModified(item, newitem);
                    }
                }
            }

            Dictionary = newdictionary;

            wtsEnumData.Memory.Dispose();
        }
    }
}
