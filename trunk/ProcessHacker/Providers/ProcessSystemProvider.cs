/*
 * Process Hacker
 * 
 * Copyright (C) 2008-2009 wj32
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
        public string FileDescription;
        public string FileName;
        public long MemoryUsage;
        public string Name;
        public string Username;
        public Win32.SYSTEM_PROCESS_INFORMATION Process;
        public Dictionary<int, Win32.SYSTEM_THREAD_INFORMATION> Threads;

        public Win32.TOKEN_ELEVATION_TYPE ElevationType;
        public bool IsBeingDebugged;
        public bool IsDotNet;
        public bool IsElevated;
        public bool IsInJob;
        public bool IsPacked;
        public bool IsVirtualizationEnabled;
        public long LastTime;
        public int SessionId;
        public bool HasParent;
        public int ParentPID;
        public int IconAttempts;

        public Win32.TokenHandle TokenQueryHandle;
        public Win32.ProcessHandle ProcessQueryHandle;
        public Win32.ProcessHandle ProcessQueryLimitedHandle;
        public Win32.ProcessHandle ProcessQueryLimitedVmReadHandle;
    }

    public class ProcessSystemProvider : Provider<int, ProcessItem>
    {
        private long _lastOtherTime;
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
            this.ProcessorPerfArray = new Win32.SYSTEM_PROCESSOR_PERFORMANCE_INFORMATION[this.System.NumberOfProcessors];

            this.UpdateProcessorPerf();
            _lastSysTime = this.ProcessorPerf.KernelTime + this.ProcessorPerf.UserTime;
            _lastOtherTime = this.ProcessorPerf.IdleTime;
        }

        public Win32.SYSTEM_BASIC_INFORMATION System { get; private set; }
        public Win32.SYSTEM_PERFORMANCE_INFORMATION Performance { get; private set; }
        public Win32.SYSTEM_PROCESSOR_PERFORMANCE_INFORMATION ProcessorPerf { get; private set; }
        public Win32.SYSTEM_PROCESSOR_PERFORMANCE_INFORMATION[] ProcessorPerfArray { get; private set; }
        public float CurrentCPUUsage { get; private set; }
        public int PIDWithMostCPUUsage { get; private set; }

        public bool PerformanceEnabled { get; set; }

        private void UpdateProcessorPerf()
        {
            using (MemoryAlloc data =
                new MemoryAlloc(Marshal.SizeOf(typeof(Win32.SYSTEM_PROCESSOR_PERFORMANCE_INFORMATION)) *
                this.ProcessorPerfArray.Length))
            {
                int retLen;

                Win32.ZwQuerySystemInformation(Win32.SYSTEM_INFORMATION_CLASS.SystemProcessorTimes,
                    data, data.Size, out retLen);

                var newSums = new Win32.SYSTEM_PROCESSOR_PERFORMANCE_INFORMATION();

                // Thanks to:
                // http://www.netperf.org/svn/netperf2/trunk/src/netcpu_ntperf.c
                // for the critical information:
                // "KernelTime needs to be fixed-up; it includes both idle & true kernel time".
                // This is why I love free software.
                for (int i = 0; i < this.ProcessorPerfArray.Length; i++)
                {
                    this.ProcessorPerfArray[i] = 
                        data.ReadStruct<Win32.SYSTEM_PROCESSOR_PERFORMANCE_INFORMATION>(i);

                    this.ProcessorPerfArray[i].KernelTime -= this.ProcessorPerfArray[i].IdleTime + 
                        this.ProcessorPerfArray[i].DpcTime + this.ProcessorPerfArray[i].InterruptTime;
                    newSums.DpcTime += this.ProcessorPerfArray[i].DpcTime;
                    newSums.IdleTime += this.ProcessorPerfArray[i].IdleTime;
                    newSums.InterruptCount += this.ProcessorPerfArray[i].InterruptCount;
                    newSums.InterruptTime += this.ProcessorPerfArray[i].InterruptTime;
                    newSums.KernelTime += this.ProcessorPerfArray[i].KernelTime;
                    newSums.UserTime += this.ProcessorPerfArray[i].UserTime;
                }

                this.ProcessorPerf = newSums;
            }
        }

        public void UpdatePerformance()
        {
            if (!this.PerformanceEnabled)
                return;

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

            long thisOtherTime = this.ProcessorPerf.IdleTime + this.ProcessorPerf.DpcTime + this.ProcessorPerf.InterruptTime;
            long otherTime = thisOtherTime - _lastOtherTime;

            _lastOtherTime = thisOtherTime;

            // set System Idle Process CPU time
            if (procs.ContainsKey(0))
            {
                Win32.SystemProcess proc = procs[0];

                proc.Process.KernelTime = this.ProcessorPerf.IdleTime;
                procs.Remove(0);
                procs.Add(0, proc);
            }

            procs.Add(-2, new Win32.SystemProcess()
            {
                Name = "DPCs",
                Process = new Win32.SYSTEM_PROCESS_INFORMATION()
                {
                    ProcessId = -2,
                    InheritedFromProcessId = 0,
                    KernelTime = this.ProcessorPerf.DpcTime,
                    SessionId = -1
                }
            });

            procs.Add(-3, new Win32.SystemProcess()
            {
                Name = "Interrupts",
                Process = new Win32.SYSTEM_PROCESS_INFORMATION()
                {
                    ProcessId = -3,
                    InheritedFromProcessId = 0,
                    KernelTime = this.ProcessorPerf.InterruptTime,
                    SessionId = -1
                }
            });

            float mostCPUUsage = 0;

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
                Process p = null;

                try { p = Process.GetProcessById(pid); }
                catch { }

                if (!Dictionary.ContainsKey(pid))
                {
                    ProcessItem item = new ProcessItem();

                    item.PID = pid;
                    item.LastTime = processInfo.KernelTime + processInfo.UserTime;
                    item.MemoryUsage = processInfo.VirtualMemoryCounters.PrivateBytes;
                    item.Process = processInfo;
                    item.SessionId = processInfo.SessionId;
                    item.Threads = procs[pid].Threads;

                    item.Name = procs[pid].Name;

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
                        item.FileName = Misc.GetRealPath(p.MainModule.FileName);
                    }
                    catch
                    { }

                    try
                    {
                        item.ProcessQueryLimitedHandle = new Win32.ProcessHandle(pid, Program.MinProcessQueryRights);

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

                        if (item.FileName == null)
                        {
                            try
                            {
                                item.FileName = item.ProcessQueryLimitedHandle.GetImageFileName();
                            }
                            catch
                            { }
                        }

                        try { item.IsInJob = item.ProcessQueryLimitedHandle.IsInJob(); }
                        catch { }

                        try
                        {
                            item.ParentPID = item.ProcessQueryLimitedHandle.GetParentPID();
                            item.HasParent = true;

                            if (!procs.ContainsKey(item.ParentPID))
                            {
                                item.HasParent = false;
                            }
                            else if (pid > 4 && item.ParentPID == 0)
                            {
                                // the PID is 0 for processes we got Access Denied on, 
                                // but they don't really have System Idle Process 
                                // as their parent.
                                item.ParentPID = -1;
                                item.HasParent = false;
                            }
                            else
                            {
                                // check the parent's creation time to see if it's actually the parent
                                long parentStartTime = procs[item.ParentPID].Process.CreateTime;
                                long thisStartTime = processInfo.CreateTime;

                                if (parentStartTime > thisStartTime)
                                    item.HasParent = false;
                            }
                        }
                        catch
                        {
                            item.ParentPID = -1;
                            item.HasParent = false;
                        }
                    }
                    catch
                    { }

                    try
                    {
                        item.Icon = (Icon)Win32.GetFileIcon(item.FileName).Clone();
                    }
                    catch
                    { }

                    if (pid == 0)
                    {
                        item.Name = "System Idle Process";
                        item.FileDescription = "System Idle Process";
                    }
                    else if (pid == 4)
                    {
                        item.FileDescription = "Windows Kernel";
                    }
                    else if (pid == -2)
                    {
                        item.FileDescription = "Deferred Procedure Calls";
                        item.ParentPID = 0;
                        item.HasParent = true;
                    }
                    else if (pid == -3)
                    {
                        item.FileDescription = "Hardware Interrupts";
                        item.ParentPID = 0;
                        item.HasParent = true;
                    }
                    else
                    {
                        try
                        {
                            item.FileDescription = FileVersionInfo.GetVersionInfo(item.FileName).FileDescription;
                        }
                        catch
                        { }
                    }

                    item.IsPacked = false;
                    // find out if it's packed - if it has less than 3 referenced DLLs and 5 function imports
                    try
                    {
                        var peFile = new PE.PEFile(item.FileName);
                        int funcTotal = 0;

                        foreach (var i in peFile.ImportData.ImportLookupTable)
                            funcTotal += i.Count;

                        if (peFile.ImportData.ImportDirectoryTable.Count < 3 &&
                            funcTotal < 5)
                            item.IsPacked = true;
                    }
                    catch
                    {
                        // we can't read it, so...
                        if (pid > 4)
                            item.IsPacked = true;
                    }

                    // find out if it's a .NET process (we'll just see if it has loaded mscoree.dll)
                    if (item.IsPacked)
                    {
                        var modDict = new Dictionary<string, string>();

                        try
                        {
                            foreach (ProcessModule m in p.Modules)
                                modDict.Add(m.ModuleName.ToLower(), m.FileName);
                        }
                        catch
                        { }

                        if (modDict.ContainsKey("mscoree.dll") &&
                            modDict["mscoree.dll"].ToLower() == (Environment.SystemDirectory + "\\mscoree.dll").ToLower())
                        {
                            item.IsDotNet = true;
                            // .NET processes also look like they're packed
                            item.IsPacked = false;
                        }
                    }

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
                    newitem.MemoryUsage = processInfo.VirtualMemoryCounters.PrivateBytes;
                    newitem.Process = processInfo;
                    newitem.Threads = procs[pid].Threads;

                    try
                    {
                        newitem.CPUUsage = (float)(newitem.LastTime - item.LastTime) * 100 / (sysTime + otherTime);

                        if (pid != 0 && newitem.CPUUsage > mostCPUUsage)
                        {
                            mostCPUUsage = newitem.CPUUsage;
                            this.PIDWithMostCPUUsage = pid;
                        }
                    }
                    catch
                    { }

                    if (newitem.Icon == null && newitem.IconAttempts < 5)
                    {
                        try
                        {
                            newitem.Icon = (Icon)Win32.GetFileIcon(newitem.FileName).Clone();
                        }
                        catch
                        { }

                        newitem.IconAttempts++;
                    }

                    if (item.ProcessQueryHandle != null)
                    {
                        try
                        {
                            newitem.IsBeingDebugged = item.ProcessQueryHandle.IsBeingDebugged();
                        }
                        catch
                        { }
                    }

                    if (item.ProcessQueryHandle != null)
                    {
                        try
                        {
                            newitem.IsVirtualizationEnabled = item.TokenQueryHandle.IsVirtualizationEnabled();
                        }
                        catch
                        { }
                    }

                    if (newitem.MemoryUsage != item.MemoryUsage ||
                        newitem.CPUUsage != item.CPUUsage || 
                        newitem.IsBeingDebugged != item.IsBeingDebugged ||
                        newitem.IsVirtualizationEnabled != item.IsVirtualizationEnabled)
                    {                                           
                        newdictionary[pid] = newitem;
                        this.CallDictionaryModified(item, newitem);
                    }
                    else if (Win32.ProcessesWithThreads.ContainsKey(pid))
                    {
                        newdictionary[pid] = newitem;
                    }
                }
            }

            if (thisSysTime != 0)
                this.CurrentCPUUsage = (float)sysTime / (sysTime + otherTime);

            Dictionary = newdictionary;

            wtsEnumData.Memory.Dispose();
        }
    }
}
