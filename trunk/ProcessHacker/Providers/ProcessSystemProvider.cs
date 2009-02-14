/*
 * Process Hacker - 
 *   processes and system performance information provider
 * 
 * Copyright (C) 2008-2009 wj32
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
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;

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

        public Win32.VerifyResult VerifyResult;
        public int ImportFunctions;
        public int ImportModules;

        public bool JustProcessed;

        public Win32.TokenHandle TokenQueryHandle;
        public Win32.ProcessHandle ProcessQueryHandle;
        public Win32.ProcessHandle ProcessQueryLimitedHandle;
        public Win32.ProcessHandle ProcessQueryLimitedVmReadHandle;
    }

    public class ProcessSystemProvider : Provider<int, ProcessItem>
    {
        struct FileProcessResult
        {
            public int PID;
            public bool IsDotNet;
            public bool IsPacked;
            public Win32.VerifyResult VerifyResult;
            public int ImportFunctions;
            public int ImportModules;
        }

        private Dictionary<string, Win32.VerifyResult> _fileResults = new Dictionary<string, Win32.VerifyResult>();
        private Queue<FileProcessResult> _fpResults = new Queue<FileProcessResult>();
        private long _lastOtherTime;
        private long _lastSysTime;

        private delegate void ProcessFileDelegate(int pid, string fileName);

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
                    var cpuPerf = data.ReadStruct<Win32.SYSTEM_PROCESSOR_PERFORMANCE_INFORMATION>(i);
                    
                    cpuPerf.KernelTime -= cpuPerf.IdleTime + cpuPerf.DpcTime + cpuPerf.InterruptTime;
                    newSums.DpcTime += cpuPerf.DpcTime;
                    newSums.IdleTime += cpuPerf.IdleTime;
                    newSums.InterruptCount += cpuPerf.InterruptCount;
                    newSums.InterruptTime += cpuPerf.InterruptTime;
                    newSums.KernelTime += cpuPerf.KernelTime;
                    newSums.UserTime += cpuPerf.UserTime;
                    this.ProcessorPerfArray[i] = cpuPerf;
                }

                this.ProcessorPerf = newSums;
            }
        }

        private void UpdatePerformance()
        {
            int retLen;
            Win32.SYSTEM_PERFORMANCE_INFORMATION performance = new Win32.SYSTEM_PERFORMANCE_INFORMATION();

            Win32.ZwQuerySystemInformation(Win32.SYSTEM_INFORMATION_CLASS.SystemPerformanceInformation,
                ref performance, Marshal.SizeOf(performance), out retLen);
            this.Performance = performance;
        }

        private void ProcessFile(int pid, string fileName)
        {
            FileProcessResult fpResult = new FileProcessResult();

            fpResult.PID = pid;
            fpResult.IsPacked = false;

            // find out if it's packed
            // an image is packed if:
            // 1. it references less than 3 libraries
            // 2. it imports less than 5 functions
            // or:
            // 1. the function-to-library ratio is lower than 4
            //   (on average less than 4 functions are imported from each library)
            // 2. it references more than 3 libraries
            if (fileName != null)
            {
                try
                {
                    var peFile = new PE.PEFile(fileName);

                    if (peFile.ImportData != null)
                    {
                        int libraryTotal = peFile.ImportData.ImportLookupTable.Count;
                        int funcTotal = 0;

                        foreach (var i in peFile.ImportData.ImportLookupTable)
                            funcTotal += i.Count;

                        fpResult.ImportModules = libraryTotal;
                        fpResult.ImportFunctions = funcTotal;

                        if (
                            libraryTotal < 3 && funcTotal < 5 ||
                            ((float)funcTotal / libraryTotal < 4) && libraryTotal > 3
                            )
                            fpResult.IsPacked = true;
                    }
                }
                catch (System.IO.EndOfStreamException)
                {
                    if (pid > 4)
                        fpResult.IsPacked = true;
                }
                catch (PE.PEException)
                {
                    if (pid > 4)
                        fpResult.IsPacked = true;
                }
                catch
                { }
            }

            try
            {
                if (Properties.Settings.Default.VerifySignatures)
                {
                    if (fileName != null)
                    {
                        string uniName = (new System.IO.FileInfo(fileName)).FullName.ToLower();

                        lock (_fileResults)
                        {
                            if (_fileResults.ContainsKey(uniName))
                            {
                                fpResult.VerifyResult = _fileResults[uniName];
                            }
                            else
                            {
                                try
                                {
                                    fpResult.VerifyResult = Win32.VerifyFile(fileName);
                                }
                                catch
                                {
                                    fpResult.VerifyResult = Win32.VerifyResult.NoSignature;
                                }

                                _fileResults.Add(uniName, fpResult.VerifyResult);
                            }
                        }
                    }
                }
            }
            catch
            { }

            // find out if it's a .NET process (we'll just see if it has loaded mscoree.dll)
            if (fpResult.IsPacked)
            {
                try
                {
                    var modDict = new Dictionary<string, string>();

                    foreach (var m in Dictionary[pid].ProcessQueryLimitedVmReadHandle.GetModules())
                    {
                        if (!modDict.ContainsKey(m.BaseName.ToLower()))
                            modDict.Add(m.BaseName.ToLower(), m.FileName);
                    }

                    if (modDict.ContainsKey("mscoree.dll") &&
                        modDict["mscoree.dll"].ToLower() == (Environment.SystemDirectory + "\\mscoree.dll").ToLower())
                    {
                        fpResult.IsDotNet = true;
                        // .NET processes also look like they're packed
                        fpResult.IsPacked = false;
                    }
                }
                catch
                { }
            }
            
            lock (_fpResults)
                _fpResults.Enqueue(fpResult);
        }

        private void UpdateOnce()
        {
            this.UpdatePerformance();
            this.UpdateProcessorPerf();

            if (this.RunCount % 3 == 0)
                Win32.RefreshDriveDevicePrefixes();

            Dictionary<int, int> tsProcesses = new Dictionary<int, int>();
            Dictionary<int, Win32.SystemProcess> procs = Win32.EnumProcesses();
            Dictionary<int, ProcessItem> newdictionary = new Dictionary<int, ProcessItem>(this.Dictionary);
            Win32.WtsEnumProcessesFastData wtsEnumData = new Win32.WtsEnumProcessesFastData();

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

                    if (item.ProcessQueryLimitedVmReadHandle != null)
                        item.ProcessQueryLimitedVmReadHandle.Dispose();

                    if (item.TokenQueryHandle != null)
                        item.TokenQueryHandle.Dispose();

                    if (item.Icon != null)
                        Win32.DestroyIcon(item.Icon.Handle);

                    newdictionary.Remove(pid);
                }
            }

            lock (_fpResults)
            {
                while (_fpResults.Count > 0)
                {
                    var result = _fpResults.Dequeue();

                    // Dictionary may contain items newdictionary doesn't contain, 
                    // because we just removed terminated processes. However, 
                    // the look-for-modified-processes section relies on Dictionary!
                    if (Dictionary.ContainsKey(result.PID))
                    {
                        var item = Dictionary[result.PID];

                        item.IsDotNet = result.IsDotNet;
                        item.IsPacked = result.IsPacked;
                        item.VerifyResult = result.VerifyResult;
                        item.ImportFunctions = result.ImportFunctions;
                        item.ImportModules = result.ImportModules;
                        item.JustProcessed = true;

                        Dictionary[result.PID] = item;
                    }
                }
            }

            // look for new processes
            foreach (int pid in procs.Keys)
            {
                Win32.SYSTEM_PROCESS_INFORMATION processInfo = procs[pid].Process;

                if (!Dictionary.ContainsKey(pid))
                {
                    Process p = null;
                    ProcessItem item = new ProcessItem();

                    if (pid >= 0)
                    {
                        try { p = Process.GetProcessById(pid); }
                        catch { }
                    }

                    item.PID = pid;
                    item.LastTime = processInfo.KernelTime + processInfo.UserTime;
                    item.MemoryUsage = processInfo.VirtualMemoryCounters.PrivateBytes;
                    item.Process = processInfo;
                    item.SessionId = processInfo.SessionId;
                    item.Threads = procs[pid].Threads;

                    item.Name = procs[pid].Name;

                    // HACK: Shouldn't happen, but it does.
                    if (item.Name == null)
                    {
                        try
                        {
                            item.Name = p.MainModule.ModuleName;
                        }
                        catch
                        {
                            item.Name = "";
                        }
                    }

                    if (pid > 0)
                    {
                        item.ParentPID = processInfo.InheritedFromProcessId;
                        item.HasParent = true;

                        if (!procs.ContainsKey(item.ParentPID))
                        {
                            item.HasParent = false;
                        }
                        else if (procs.ContainsKey(item.ParentPID))
                        {
                            // check the parent's creation time to see if it's actually the parent
                            long parentStartTime = procs[item.ParentPID].Process.CreateTime;
                            long thisStartTime = processInfo.CreateTime;

                            if (parentStartTime > thisStartTime)
                                item.HasParent = false;
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

                            try { item.IsInJob = item.ProcessQueryLimitedHandle.IsInJob(); }
                            catch { }
                        }
                        catch
                        { }
                    }

                    if (pid > 0)
                    {
                        if (pid != 4)
                        {
                            if (item.ProcessQueryLimitedHandle != null)
                            {
                                // first try to get the native file name, to prevent PEB
                                // file name spoofing.
                                try
                                {
                                    item.FileName =
                                        Win32.DeviceFileNameToDos(item.ProcessQueryLimitedHandle.GetNativeImageFileName());
                                }
                                catch
                                { }

                                // if we couldn't get it or we couldn't resolve the \Device prefix,
                                // we'll just use the normal method (which only works on Vista).
                                if ((item.FileName == null || item.FileName.StartsWith("\\Device\\")) && 
                                    Program.WindowsVersion == "Vista")
                                {
                                    try
                                    {
                                        item.FileName = item.ProcessQueryLimitedHandle.GetImageFileName();
                                    }
                                    catch
                                    { }
                                }
                            }

                            // if all else failed, we go for the .NET method.
                            if (item.FileName == null || item.FileName.StartsWith("\\Device\\"))
                            {
                                try
                                {
                                    item.FileName = Misc.GetRealPath(p.MainModule.FileName);
                                }
                                catch
                                { }
                            }
                        }
                        else
                        {
                            item.FileName = Misc.GetKernelFileName();
                        }

                        if (item.FileName != null)
                        {
                            try
                            {
                                item.Icon = (Icon)Win32.GetFileIcon(item.FileName);
                            }
                            catch
                            { }
                        }
                    }

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

                    if (pid > 0)
                    {
                        (new ProcessFileDelegate(this.ProcessFile)).BeginInvoke(pid, item.FileName,
                            r => { }, null);
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
                            wtsEnumData = Win32.TSEnumProcessesFast();

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

                    if (Win32.ProcessesWithThreads.ContainsKey(pid))
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

                    if (newitem.FileName != null && newitem.Icon == null && newitem.IconAttempts < 3)
                    {
                        try
                        {
                            newitem.Icon = (Icon)Win32.GetFileIcon(newitem.FileName);
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

                    //if (item.TokenQueryHandle != null)
                    //{
                    //    try
                    //    {
                    //        newitem.IsVirtualizationEnabled = item.TokenQueryHandle.IsVirtualizationEnabled();
                    //    }
                    //    catch
                    //    { }
                    //}

                    if (newitem.MemoryUsage != item.MemoryUsage ||
                        newitem.CPUUsage != item.CPUUsage || 
                        newitem.IsBeingDebugged != item.IsBeingDebugged ||
                        //newitem.IsVirtualizationEnabled != item.IsVirtualizationEnabled ||
                        newitem.JustProcessed
                        )
                    {
                        newitem.JustProcessed = false;
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

            if (wtsEnumData.Memory != null)
                wtsEnumData.Memory.Dispose();
        }
    }
}
