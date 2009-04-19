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
using System.Collections.ObjectModel;

namespace ProcessHacker
{
    public enum ProcessStats
    {
        CpuKernel, CpuUser, IoRead, IoWrite, IoOther, IoReadOther, PrivateMemory, WorkingSet
    }

    public class ProcessItem : ICloneable
    {
        public object Clone()
        {
            return base.MemberwiseClone();
        }

        public int Pid;

        public Icon Icon;
        public string CmdLine;
        public float CpuUsage;
        public string FileName;
        public FileVersionInfo VersionInfo;
        public string Name;
        public string Username;
        public string JobName;
        public Win32.SYSTEM_PROCESS_INFORMATION Process;
        public Dictionary<int, Win32.SYSTEM_THREAD_INFORMATION> Threads;

        public Win32.TOKEN_ELEVATION_TYPE ElevationType;
        public bool IsBeingDebugged;
        public bool IsDotNet;
        public bool IsElevated;
        public bool IsInJob;
        public bool IsInSignificantJob;
        public bool IsPacked;
        public int SessionId;
        public bool HasParent;
        public int ParentPid;

        public Win32.VerifyResult VerifyResult;
        public int ImportFunctions;
        public int ImportModules;

        public bool JustProcessed;
        public int ProcessingAttempts;

        public Win32.ProcessHandle ProcessQueryHandle;

        public bool FullUpdate;
        public DeltaManager<ProcessStats, long> DeltaManager;
        public HistoryManager<ProcessStats, float> FloatHistoryManager;
        public HistoryManager<ProcessStats, long> LongHistoryManager;
    }

    public enum SystemStats
    {
        CpuKernel, CpuUser, CpuOther, IoRead, IoWrite, IoOther, IoReadOther, Commit, PhysicalMemory
    }

    public class ProcessSystemProvider : Provider<int, ProcessItem>
    {
        public class FileProcessResult
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
        private DeltaManager<SystemStats, long> _longDeltas = new DeltaManager<SystemStats, long>(Subtractor.Int64Subtractor);
        private DeltaManager<string, long> _cpuDeltas = new DeltaManager<string, long>(Subtractor.Int64Subtractor);
        private HistoryManager<bool, DateTime> _timeHistory = new HistoryManager<bool, DateTime>();
        private HistoryManager<SystemStats, long> _longHistory = new HistoryManager<SystemStats, long>();
        private HistoryManager<string, float> _floatHistory = new HistoryManager<string, float>();
        private HistoryManager<bool, string> _mostUsageHistory = new HistoryManager<bool, string>();

        private delegate void ProcessFileDelegate(int pid, string fileName, bool useCache);

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

            _timeHistory.Add(false);

            _mostUsageHistory = new HistoryManager<bool, string>();
            _mostUsageHistory.Add(false);
            _mostUsageHistory.Add(true);

            _longDeltas.Add(SystemStats.CpuKernel, this.ProcessorPerf.KernelTime);
            _longDeltas.Add(SystemStats.CpuUser, this.ProcessorPerf.UserTime);
            _longDeltas.Add(SystemStats.CpuOther,
                this.ProcessorPerf.IdleTime + this.ProcessorPerf.DpcTime + this.ProcessorPerf.InterruptTime);
            _longDeltas.Add(SystemStats.IoRead, this.Performance.IoReadTransferCount);
            _longDeltas.Add(SystemStats.IoWrite, this.Performance.IoWriteTransferCount);
            _longDeltas.Add(SystemStats.IoOther, this.Performance.IoOtherTransferCount);

            _floatHistory.Add("Kernel");
            _floatHistory.Add("User");
            _floatHistory.Add("Other");

            for (int i = 0; i < this.System.NumberOfProcessors; i++)
            {                                
                _cpuDeltas.Add(i.ToString() + " Kernel", this.ProcessorPerfArray[i].KernelTime);
                _cpuDeltas.Add(i.ToString() + " User", this.ProcessorPerfArray[i].UserTime);
                _cpuDeltas.Add(i.ToString() + " Other", 
                    this.ProcessorPerfArray[i].IdleTime + this.ProcessorPerfArray[i].DpcTime + 
                    this.ProcessorPerfArray[i].InterruptTime);
                _floatHistory.Add(i.ToString() + " Kernel");
                _floatHistory.Add(i.ToString() + " User");
                _floatHistory.Add(i.ToString() + " Other");   
            }

            _longHistory.Add(SystemStats.IoRead);
            _longHistory.Add(SystemStats.IoWrite);
            _longHistory.Add(SystemStats.IoOther);
            _longHistory.Add(SystemStats.IoReadOther);
            _longHistory.Add(SystemStats.Commit);
            _longHistory.Add(SystemStats.PhysicalMemory);
        }

        public Win32.SYSTEM_BASIC_INFORMATION System { get; private set; }
        public Win32.SYSTEM_PERFORMANCE_INFORMATION Performance { get; private set; }
        public Win32.SYSTEM_PROCESSOR_PERFORMANCE_INFORMATION ProcessorPerf { get; private set; }
        public Win32.SYSTEM_PROCESSOR_PERFORMANCE_INFORMATION[] ProcessorPerfArray { get; private set; }
        public float CurrentCpuKernelUsage { get; private set; }
        public float CurrentCpuUserUsage { get; private set; }
        public float CurrentCpuUsage { get { return this.CurrentCpuKernelUsage + this.CurrentCpuUserUsage; } }
        public int PIDWithMostIoActivity { get; private set; }
        public int PIDWithMostCpuUsage { get; private set; }
        public DeltaManager<string, long> CpuDeltas { get { return _cpuDeltas; } }
        public DeltaManager<SystemStats, long> LongDeltas { get { return _longDeltas; } }
        public HistoryManager<string, float> FloatHistory { get { return _floatHistory; } }
        public HistoryManager<SystemStats, long> LongHistory { get { return _longHistory; } }
        public ReadOnlyCollection<DateTime> TimeHistory { get { return _timeHistory[false]; } }
        public ReadOnlyCollection<string> MostCpuHistory { get { return _mostUsageHistory[false]; } }
        public ReadOnlyCollection<string> MostIoHistory { get { return _mostUsageHistory[true]; } }

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

        private void ProcessFile(int pid, string fileName, bool forced)
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
            // 2. it references more than 3 libraries but less than 14 libraries.
            if (fileName != null && (Properties.Settings.Default.VerifySignatures || forced))
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
                            ((float)funcTotal / libraryTotal < 4) && libraryTotal > 3 && libraryTotal < 14
                            )
                            fpResult.IsPacked = true;

                        // Only one import from mscoree.dll means that it's a .NET program.
                        if (libraryTotal == 1 && 
                            peFile.ImportData.ImportDirectoryTable[0].Name.Equals("mscoree.dll", 
                            StringComparison.InvariantCultureIgnoreCase))
                            fpResult.IsPacked = false;
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
                if (Properties.Settings.Default.VerifySignatures || forced)
                {
                    if (fileName != null)
                    {
                        string uniName = (new System.IO.FileInfo(fileName)).FullName.ToLower();

                        lock (_fileResults)
                        {
                            if (!forced && _fileResults.ContainsKey(uniName))
                            {
                                fpResult.VerifyResult = _fileResults[uniName];
                            }
                            else
                            {
                                try
                                {
                                    fpResult.VerifyResult = Win32.VerifyFile(fileName);
                                    //fpResult.VerifyResult = NProcessHacker.PhvVerifyFile(fileName);
                                }
                                catch
                                {
                                    fpResult.VerifyResult = Win32.VerifyResult.NoSignature;
                                }

                                if (!_fileResults.ContainsKey(uniName))
                                    _fileResults.Add(uniName, fpResult.VerifyResult);
                                else
                                    _fileResults[uniName] = fpResult.VerifyResult;
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

            Program.CollectGarbage();
        }

        public void QueueFileProcessing(int pid)
        {
            (new ProcessFileDelegate(this.ProcessFile)).BeginInvoke(pid, this.Dictionary[pid].FileName, true,
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

            _longDeltas.Update(SystemStats.CpuKernel, this.ProcessorPerf.KernelTime);
            long sysKernelTime = _longDeltas[SystemStats.CpuKernel];

            _longDeltas.Update(SystemStats.CpuUser, this.ProcessorPerf.UserTime);
            long sysUserTime = _longDeltas[SystemStats.CpuUser];

            _longDeltas.Update(SystemStats.CpuOther, 
                this.ProcessorPerf.IdleTime + this.ProcessorPerf.DpcTime + this.ProcessorPerf.InterruptTime);
            long otherTime = _longDeltas[SystemStats.CpuOther];

            if (sysKernelTime + sysUserTime + otherTime == 0)
                return;

            _longDeltas.Update(SystemStats.IoRead, this.Performance.IoReadTransferCount);
            _longDeltas.Update(SystemStats.IoWrite, this.Performance.IoWriteTransferCount);
            _longDeltas.Update(SystemStats.IoOther, this.Performance.IoOtherTransferCount);

            if (this.ProcessorPerf.KernelTime != 0 && this.ProcessorPerf.UserTime != 0)
            {
                this.CurrentCpuKernelUsage = (float)sysKernelTime / (sysKernelTime + sysUserTime + otherTime);
                this.CurrentCpuUserUsage = (float)sysUserTime / (sysKernelTime + sysUserTime + otherTime);

                _floatHistory.Update("Kernel", this.CurrentCpuKernelUsage);
                _floatHistory.Update("User", this.CurrentCpuUserUsage);
                _floatHistory.Update("Other", (float)otherTime / (sysKernelTime + sysUserTime + otherTime));
            }

            for (int i = 0; i < this.System.NumberOfProcessors; i++)
            {
                long cpuKernelTime = _cpuDeltas.Update(i.ToString() + " Kernel", this.ProcessorPerfArray[i].KernelTime);
                long cpuUserTime = _cpuDeltas.Update(i.ToString() + " User", this.ProcessorPerfArray[i].UserTime);
                long cpuOtherTime = _cpuDeltas.Update(i.ToString() + " Other",
                    this.ProcessorPerfArray[i].IdleTime + this.ProcessorPerfArray[i].DpcTime +
                    this.ProcessorPerfArray[i].InterruptTime);
                _floatHistory.Update(i.ToString() + " Kernel", 
                    (float)cpuKernelTime / (cpuKernelTime + cpuUserTime + cpuOtherTime));
                _floatHistory.Update(i.ToString() + " User",
                    (float)cpuUserTime / (cpuKernelTime + cpuUserTime + cpuOtherTime));
                _floatHistory.Update(i.ToString() + " Other",
                    (float)cpuOtherTime / (cpuKernelTime + cpuUserTime + cpuOtherTime));
            }

            if (this.RunCount < 3)
            {
                _longDeltas[SystemStats.IoRead] = 0;
                _longDeltas[SystemStats.IoWrite] = 0;
                _longDeltas[SystemStats.IoOther] = 0;
            }

            _longHistory.Update(SystemStats.IoRead, _longDeltas[SystemStats.IoRead]);
            _longHistory.Update(SystemStats.IoWrite, _longDeltas[SystemStats.IoWrite]);
            _longHistory.Update(SystemStats.IoOther, _longDeltas[SystemStats.IoOther]);
            _longHistory.Update(SystemStats.IoReadOther, 
                _longDeltas[SystemStats.IoRead] + _longDeltas[SystemStats.IoOther]);
            _longHistory.Update(SystemStats.Commit, (long)this.Performance.CommittedPages * this.System.PageSize);
            _longHistory.Update(SystemStats.PhysicalMemory,
                (long)(this.System.NumberOfPhysicalPages - this.Performance.AvailablePages) * this.System.PageSize);

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
            long mostIOActivity = 0;

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
                        ProcessItem item = this.Dictionary[result.PID];

                        item.IsDotNet = result.IsDotNet;
                        item.IsPacked = result.IsPacked;
                        item.VerifyResult = result.VerifyResult;
                        item.ImportFunctions = result.ImportFunctions;
                        item.ImportModules = result.ImportModules;
                        item.JustProcessed = true;
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

                    item.Pid = pid;
                    item.Process = processInfo;
                    item.SessionId = processInfo.SessionId;
                    item.Threads = procs[pid].Threads;
                    item.ProcessingAttempts = 1;

                    item.Name = procs[pid].Name;

                    item.DeltaManager = new DeltaManager<ProcessStats, long>(Subtractor.Int64Subtractor);
                    item.DeltaManager.Add(ProcessStats.CpuKernel, processInfo.KernelTime);
                    item.DeltaManager.Add(ProcessStats.CpuUser, processInfo.UserTime);
                    item.DeltaManager.Add(ProcessStats.IoRead, (long)processInfo.IoCounters.ReadTransferCount);
                    item.DeltaManager.Add(ProcessStats.IoWrite, (long)processInfo.IoCounters.WriteTransferCount);
                    item.DeltaManager.Add(ProcessStats.IoOther, (long)processInfo.IoCounters.OtherTransferCount);
                    item.FloatHistoryManager = new HistoryManager<ProcessStats, float>();
                    item.LongHistoryManager = new HistoryManager<ProcessStats, long>();
                    item.FloatHistoryManager.Add(ProcessStats.CpuKernel);
                    item.FloatHistoryManager.Add(ProcessStats.CpuUser);
                    item.LongHistoryManager.Add(ProcessStats.IoReadOther);
                    item.LongHistoryManager.Add(ProcessStats.IoRead);
                    item.LongHistoryManager.Add(ProcessStats.IoWrite);
                    item.LongHistoryManager.Add(ProcessStats.IoOther);
                    item.LongHistoryManager.Add(ProcessStats.PrivateMemory);
                    item.LongHistoryManager.Add(ProcessStats.WorkingSet);

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
                        item.ParentPid = processInfo.InheritedFromProcessId;
                        item.HasParent = true;

                        if (!procs.ContainsKey(item.ParentPid))
                        {
                            item.HasParent = false;
                        }
                        else if (procs.ContainsKey(item.ParentPid))
                        {
                            // check the parent's creation time to see if it's actually the parent
                            ulong parentStartTime = (ulong)procs[item.ParentPid].Process.CreateTime;
                            ulong thisStartTime = (ulong)processInfo.CreateTime;

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

                                if (Program.KPH != null)
                                {
                                    try
                                    {
                                        using (var jhandle = queryLimitedHandle.GetJob(Win32.JOB_OBJECT_RIGHTS.JOB_OBJECT_QUERY))
                                        {
                                            var limits = jhandle.GetBasicLimitInformation();

                                            item.IsInJob = true;
                                            item.JobName = jhandle.GetHandleName();

                                            if (limits.LimitFlags != Win32.JOB_OBJECT_LIMIT_FLAGS.JOB_OBJECT_LIMIT_SILENT_BREAKAWAY_OK)
                                            {
                                                item.IsInSignificantJob = true;
                                            }
                                        }
                                    }
                                    catch
                                    {
                                        item.IsInJob = false;
                                        item.IsInSignificantJob = false;
                                    }
                                }
                                else
                                {
                                    try { item.IsInJob = queryLimitedHandle.IsInJob(); }
                                    catch { }
                                }
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
                                    Version.HasWin32ImageFileName)
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
                        item.ParentPid = 0;
                        item.HasParent = true;
                    }
                    else if (pid == -3)
                    {
                        item.ParentPid = 0;
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
                        (new ProcessFileDelegate(this.ProcessFile)).BeginInvoke(pid, item.FileName, false,
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
                            Program.MinProcessQueryRights | Program.MinProcessReadMemoryRights))
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
                    ProcessItem item = this.Dictionary[pid];

                    item.DeltaManager.Update(ProcessStats.CpuKernel, processInfo.KernelTime);
                    item.DeltaManager.Update(ProcessStats.CpuUser, processInfo.UserTime);
                    item.DeltaManager.Update(ProcessStats.IoRead, (long)processInfo.IoCounters.ReadTransferCount);
                    item.DeltaManager.Update(ProcessStats.IoWrite, (long)processInfo.IoCounters.WriteTransferCount);
                    item.DeltaManager.Update(ProcessStats.IoOther, (long)processInfo.IoCounters.OtherTransferCount);

                    item.FloatHistoryManager.Update(ProcessStats.CpuKernel,
                        (float)item.DeltaManager[ProcessStats.CpuKernel] /
                        (sysKernelTime + sysUserTime + otherTime));
                    item.FloatHistoryManager.Update(ProcessStats.CpuUser,
                        (float)item.DeltaManager[ProcessStats.CpuUser] /
                        (sysKernelTime + sysUserTime + otherTime));
                    item.LongHistoryManager.Update(ProcessStats.IoRead, item.DeltaManager[ProcessStats.IoRead]);
                    item.LongHistoryManager.Update(ProcessStats.IoWrite, item.DeltaManager[ProcessStats.IoWrite]);
                    item.LongHistoryManager.Update(ProcessStats.IoOther, item.DeltaManager[ProcessStats.IoOther]);
                    item.LongHistoryManager.Update(ProcessStats.IoReadOther,
                        item.DeltaManager[ProcessStats.IoRead] + item.DeltaManager[ProcessStats.IoOther]);
                    item.LongHistoryManager.Update(ProcessStats.PrivateMemory, processInfo.VirtualMemoryCounters.PrivateBytes);
                    item.LongHistoryManager.Update(ProcessStats.WorkingSet, processInfo.VirtualMemoryCounters.WorkingSetSize);

                    item.Process = processInfo;
                    item.FullUpdate = false;
                    item.JustProcessed = false;

                    if (Win32.ProcessesWithThreads.ContainsKey(pid))
                        item.Threads = procs[pid].Threads;

                    try
                    {
                        item.CpuUsage = (float)
                            (item.DeltaManager[ProcessStats.CpuUser] + 
                            item.DeltaManager[ProcessStats.CpuKernel]) * 100 /
                            (sysKernelTime + sysUserTime + otherTime);

                        if (item.CpuUsage > 400.0f)
                            item.CpuUsage /= 8.0f;
                        else if (item.CpuUsage > 200.0f)
                            item.CpuUsage /= 4.0f;
                        else if (item.CpuUsage > 100.0f)
                            item.CpuUsage /= 2.0f;

                        if (pid != 0 && item.CpuUsage > mostCPUUsage)
                        {
                            mostCPUUsage = item.CpuUsage;
                            this.PIDWithMostCpuUsage = pid;
                        }

                        if (pid != 0 && (item.LongHistoryManager[ProcessStats.IoReadOther][0] +
                            item.LongHistoryManager[ProcessStats.IoWrite][0]) > mostIOActivity)
                        {
                            mostIOActivity = item.LongHistoryManager[ProcessStats.IoReadOther][0] +
                                item.LongHistoryManager[ProcessStats.IoWrite][0];
                            this.PIDWithMostIoActivity = pid;
                        }
                    }
                    catch
                    { }

                    if (item.ProcessQueryHandle != null)
                    {
                        try
                        {
                            item.IsBeingDebugged = item.ProcessQueryHandle.IsBeingDebugged();
                            item.FullUpdate = true;
                        }
                        catch
                        { }
                    }

                    if (pid > 0)
                    {
                        if (item.IsPacked && item.ProcessingAttempts < 3)
                        {
                            (new ProcessFileDelegate(this.ProcessFile)).BeginInvoke(pid, item.FileName, true,
                                r => { }, null);
                            item.ProcessingAttempts++;
                        }
                    }

                    if (item.FullUpdate)
                        this.CallDictionaryModified(null, item);
                }
            }

            try
            {
                _mostUsageHistory.Update(false, newdictionary[this.PIDWithMostCpuUsage].Name + ": " +
                    newdictionary[this.PIDWithMostCpuUsage].CpuUsage.ToString("N2") + "%");
            }
            catch
            {
                _mostUsageHistory.Update(false, "");
            }

            try
            {
                _mostUsageHistory.Update(true, newdictionary[this.PIDWithMostIoActivity].Name + ": " +
                    "R+O: " + Misc.GetNiceSizeName(
                    newdictionary[this.PIDWithMostIoActivity].LongHistoryManager[ProcessStats.IoReadOther][0]) +
                    ", W: " + Misc.GetNiceSizeName(
                    newdictionary[this.PIDWithMostIoActivity].LongHistoryManager[ProcessStats.IoWrite][0]));
            }
            catch
            {
                _mostUsageHistory.Update(true, "");
            }

            _timeHistory.Update(false, DateTime.Now);

            Dictionary = newdictionary;

            if (wtsEnumData.Memory != null)
                wtsEnumData.Memory.Dispose();
        }
    }
}
