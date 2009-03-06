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
        public string FileName;
        public FileVersionInfo VersionInfo;
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
        public long LastTime;
        public int SessionId;
        public bool HasParent;
        public int ParentPID;

        public Win32.VerifyResult VerifyResult;
        public int ImportFunctions;
        public int ImportModules;

        public bool JustProcessed;
        public int ProcessingAttempts;

        public Win32.ProcessHandle ProcessQueryHandle;

        public bool FullUpdate;
    }

    public class ProcessSystemProvider : Provider<int, ProcessItem>
    {
        public struct FileProcessResult
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
        private long _lastSysKernelTime;
        private long _lastSysUserTime;

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
            _lastSysKernelTime = this.ProcessorPerf.KernelTime;
            _lastSysUserTime = this.ProcessorPerf.UserTime;
            _lastOtherTime = this.ProcessorPerf.IdleTime + this.ProcessorPerf.DpcTime + 
                this.ProcessorPerf.InterruptTime;
        }

        public Win32.SYSTEM_BASIC_INFORMATION System { get; private set; }
        public Win32.SYSTEM_PERFORMANCE_INFORMATION Performance { get; private set; }
        public Win32.SYSTEM_PROCESSOR_PERFORMANCE_INFORMATION ProcessorPerf { get; private set; }
        public Win32.SYSTEM_PROCESSOR_PERFORMANCE_INFORMATION[] ProcessorPerfArray { get; private set; }
        public float CurrentCPUKernelUsage { get; private set; }
        public float CurrentCPUUserUsage { get; private set; }
        public float CurrentCPUUsage { get { return this.CurrentCPUKernelUsage + this.CurrentCPUUserUsage; } }
        public int PIDWithMostCPUUsage { get; private set; }

        public Queue<FileProcessResult> FileProcessingQueue
        {
            get { return _fpResults; }
        }

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
                            if (false && _fileResults.ContainsKey(uniName))
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

                                //_fileResults.Add(uniName, fpResult.VerifyResult);
                            }
                        }
                    }
                }
            }
            catch
            { }

            if (pid > 4)
            {
                try
                {
                    var corpubPublishClass = new Debugger.Interop.CorPub.CorpubPublishClass();
                    Debugger.Interop.CorPub.ICorPublishProcess process = null;

                    try
                    {
                        int managed = 0;

                        corpubPublishClass.GetProcess((uint)pid, out process);
                        process.IsManaged(out managed);

                        if (managed > 0)
                        {
                            fpResult.IsPacked = false;
                            fpResult.IsDotNet = true;
                        }
                    }
                    finally
                    {
                        if (process != null)
                        {
                            Marshal.ReleaseComObject(process);
                        }
                    }
                }
                catch
                { }
            }

            lock (_fpResults)
                _fpResults.Enqueue(fpResult);
        }

        public void QueueFileProcessing(int pid)
        {
            (new ProcessFileDelegate(this.ProcessFile)).BeginInvoke(pid, this.Dictionary[pid].FileName,
                                r => { }, null);
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

            long thisSysKernelTime = this.ProcessorPerf.KernelTime;
            long sysKernelTime = thisSysKernelTime - _lastSysKernelTime;
            _lastSysKernelTime = thisSysKernelTime;

            long thisSysUserTime = this.ProcessorPerf.UserTime;
            long sysUserTime = thisSysUserTime - _lastSysUserTime;
            _lastSysUserTime = thisSysUserTime;

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
                    Win32.ProcessHandle queryLimitedHandle = null;

                    if (pid >= 0)
                    {
                        try { p = Process.GetProcessById(pid); }
                        catch { }

                        try { queryLimitedHandle = new Win32.ProcessHandle(pid, Program.MinProcessQueryRights); }
                        catch { }
                    }

                    item.PID = pid;
                    item.LastTime = processInfo.KernelTime + processInfo.UserTime;
                    item.Process = processInfo;
                    item.SessionId = processInfo.SessionId;
                    item.Threads = procs[pid].Threads;
                    item.ProcessingAttempts = 1;

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
                            if (queryLimitedHandle != null)
                            {
                                try
                                {
                                    using (var thandle = queryLimitedHandle.GetToken(Win32.TOKEN_RIGHTS.TOKEN_QUERY))
                                    {
                                        try { item.Username = thandle.GetUser().GetName(true); }
                                        catch { }
                                        try { item.ElevationType = thandle.GetElevationType(); }
                                        catch { }
                                        try { item.IsElevated = thandle.IsElevated(); }
                                        catch { }
                                    }
                                }
                                catch
                                { }

                                try { item.IsInJob = queryLimitedHandle.IsInJob(); }
                                catch { }
                            }
                        }
                        catch
                        { }
                    }

                    if (pid > 0)
                    {
                        if (pid != 4)
                        {
                            if (queryLimitedHandle != null)
                            {
                                // first try to get the native file name, to prevent PEB
                                // file name spoofing.
                                try
                                {
                                    item.FileName =
                                        Win32.DeviceFileNameToDos(queryLimitedHandle.GetNativeImageFileName());
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
                                        item.FileName = queryLimitedHandle.GetImageFileName();
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
                    }
                    else if (pid == -2)
                    {
                        item.ParentPID = 0;
                        item.HasParent = true;
                    }
                    else if (pid == -3)
                    {
                        item.ParentPID = 0;
                        item.HasParent = true;
                    }
                    else
                    {
                        try
                        {
                            item.VersionInfo = FileVersionInfo.GetVersionInfo(item.FileName);
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
                        using (var phandle = new Win32.ProcessHandle(pid,
                            Program.MinProcessQueryRights | Win32.PROCESS_RIGHTS.PROCESS_VM_READ))
                            item.CmdLine = phandle.GetCommandLine();

                    }
                    catch
                    { }

                    if (queryLimitedHandle != null)
                        queryLimitedHandle.Dispose();

                    newdictionary.Add(pid, item);
                    this.CallDictionaryAdded(item);
                }
                // look for modified processes
                else
                {
                    ProcessItem item = Dictionary[pid];
                    ProcessItem newitem = item;

                    newitem.LastTime = processInfo.KernelTime + processInfo.UserTime;
                    newitem.Process = processInfo;
                    newitem.FullUpdate = false;
                    newitem.JustProcessed = false;

                    if (Win32.ProcessesWithThreads.ContainsKey(pid))
                        newitem.Threads = procs[pid].Threads;

                    try
                    {
                        newitem.CPUUsage = (float)(newitem.LastTime - item.LastTime) * 100 /
                            (sysKernelTime + sysUserTime + otherTime);

                        if (newitem.CPUUsage > 400.0f)
                            newitem.CPUUsage /= 8.0f;
                        else if (newitem.CPUUsage > 200.0f)
                            newitem.CPUUsage /= 4.0f;
                        else if (newitem.CPUUsage > 100.0f)
                            newitem.CPUUsage /= 2.0f;

                        if (pid != 0 && newitem.CPUUsage > mostCPUUsage)
                        {
                            mostCPUUsage = newitem.CPUUsage;
                            this.PIDWithMostCPUUsage = pid;
                        }
                    }
                    catch
                    { }

                    if (item.ProcessQueryHandle != null)
                    {
                        try
                        {
                            newitem.IsBeingDebugged = item.ProcessQueryHandle.IsBeingDebugged();
                            newitem.FullUpdate = true;
                        }
                        catch
                        { }
                    }

                    if (pid > 0)
                    {
                        if (item.IsPacked && item.ProcessingAttempts < 5)
                        {
                            (new ProcessFileDelegate(this.ProcessFile)).BeginInvoke(pid, item.FileName,
                                r => { }, null);
                            newitem.ProcessingAttempts++;
                        }
                    }

                    newdictionary[pid] = newitem;
                    this.CallDictionaryModified(item, newitem);
                }
            }

            if (thisSysKernelTime != 0 && thisSysUserTime != 0)
            {
                this.CurrentCPUKernelUsage = (float)sysKernelTime / (sysKernelTime + sysUserTime + otherTime);
                this.CurrentCPUUserUsage = (float)sysUserTime / (sysKernelTime + sysUserTime + otherTime);
            }

            Dictionary = newdictionary;

            if (wtsEnumData.Memory != null)
                wtsEnumData.Memory.Dispose();
        }
    }
}
