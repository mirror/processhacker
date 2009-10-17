/*
 * Process Hacker - 
 *   processes and system performance information provider
 *
 * Copyright (C) 2009 Flavio Erlich 
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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using ProcessHacker.Common;
using ProcessHacker.Common.Messaging;
using ProcessHacker.Native;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Image;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;

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

        public int RunId;
        public int Pid;

        public Icon Icon;
        public Icon LargeIcon;
        public string CmdLine;
        public float CpuUsage;
        public string FileName;
        public FileVersionInfo VersionInfo;
        public string Name;
        public string Username;
        public string JobName;
        public string Integrity;
        public int IntegrityLevel;
        public SystemProcessInformation Process;
        public DateTime CreateTime;

        public TokenElevationType ElevationType;
        public bool HasParent;
        public bool IsBeingDebugged;
        public bool IsDotNet;
        public bool IsElevated;
        public bool IsInJob;
        public bool IsInSignificantJob;
        public bool IsPacked;
        public bool IsPosix;
        public bool IsWow64;
        public int SessionId;
        public int ParentPid;

        public VerifyResult VerifyResult;
        public string VerifySignerName;
        public int ImportFunctions;
        public int ImportModules;

        public bool JustProcessed;
        public int ProcessingAttempts;

        public ProcessHandle ProcessQueryHandle;

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
        public class ProcessQueryMessage : Message
        {
            public int Stage;
            public int Pid;
            public string FileName;
            public TokenElevationType ElevationType;
            public bool IsElevated;
            public string Integrity;
            public int IntegrityLevel;
            public string JobName;
            public bool IsInJob;
            public bool IsInSignificantJob;
            public bool IsWow64;
            public Icon Icon;
            public Icon LargeIcon;
            public FileVersionInfo VersionInfo;
            public string CmdLine;

            public bool IsDotNet;
            public bool IsPacked;
            public bool IsPosix;

            public VerifyResult VerifyResult;
            public string VerifySignerName;
            public int ImportFunctions;
            public int ImportModules;
        }

        public delegate void ProcessQueryDelegate(int stage, int pid);

        public event ProcessQueryDelegate ProcessQueryComplete;
        public event ProcessQueryDelegate ProcessQueryReceived;

        private SystemBasicInformation _system;
        public SystemBasicInformation System
        {
            get { return _system; }
        }

        private SystemPerformanceInformation _performance;
        public SystemPerformanceInformation Performance
        {
            get { return _performance; }
        }

        private int _processorPerfArraySize;
        private MemoryAlloc _processorPerfBuffer;
        private SystemProcessorPerformanceInformation[] _processorPerfArray;
        public SystemProcessorPerformanceInformation[] ProcessorPerfArray
        {
            get { return _processorPerfArray; }
        }

        private SystemProcessorPerformanceInformation _processorPerf;
        public SystemProcessorPerformanceInformation ProcessorPerf
        {
            get { return _processorPerf; }
        }

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

        private delegate ProcessQueryMessage QueryProcessDelegate(int pid, string fileName, bool useCache);

        private MessageQueue _messageQueue = new MessageQueue();
        private Dictionary<string, VerifyResult> _fileResults = new Dictionary<string, VerifyResult>();
        private DeltaManager<SystemStats, long> _longDeltas = 
            new DeltaManager<SystemStats, long>(Subtractor.Int64Subtractor, EnumComparer<SystemStats>.Instance);
        private DeltaManager<string, long> _cpuDeltas = new DeltaManager<string, long>(Subtractor.Int64Subtractor);
        private HistoryManager<bool, DateTime> _timeHistory = new HistoryManager<bool, DateTime>();
        private HistoryManager<SystemStats, long> _longHistory = 
            new HistoryManager<SystemStats, long>(EnumComparer<SystemStats>.Instance);
        private HistoryManager<string, float> _floatHistory = new HistoryManager<string, float>();
        private HistoryManager<bool, string> _mostUsageHistory = new HistoryManager<bool, string>();

        private SystemProcess _dpcs = new SystemProcess()
        {
            Name = "DPCs",
            Process = new SystemProcessInformation()
            {
                ProcessId = -2,
                InheritedFromProcessId = 0,
                SessionId = -1
            }
        };

        private SystemProcess _interrupts = new SystemProcess()
        {
            Name = "Interrupts",
            Process = new SystemProcessInformation()
            {
                ProcessId = -3,
                InheritedFromProcessId = 0,
                SessionId = -1
            }
        };

        public ProcessSystemProvider()
            : base()
        {
            this.Name = this.GetType().Name;
            this.ProviderUpdate += new ProviderUpdateOnce(UpdateOnce);

            // Add the file processing results listener.
            _messageQueue.AddListener(
                new MessageQueueListener<ProcessQueryMessage>((message) =>
                    {
                        if (this.Dictionary.ContainsKey(message.Pid))
                        {
                            ProcessItem item = this.Dictionary[message.Pid];

                            this.FillPqResult(item, message);
                            item.JustProcessed = true;
                        }
                    }));

            SystemBasicInformation basic;
            int retLen;

            Win32.NtQuerySystemInformation(SystemInformationClass.SystemBasicInformation, out basic,
                Marshal.SizeOf(typeof(SystemBasicInformation)), out retLen);
            _system = basic;
            _processorPerfArraySize = Marshal.SizeOf(typeof(SystemProcessorPerformanceInformation)) *
                _system.NumberOfProcessors;
            _processorPerfBuffer = new MemoryAlloc(_processorPerfArraySize);
            _processorPerfArray = new SystemProcessorPerformanceInformation[_system.NumberOfProcessors];

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

        private void UpdateProcessorPerf()
        {
            int retLen;

            Win32.NtQuerySystemInformation(SystemInformationClass.SystemProcessorPerformanceInformation,
                _processorPerfBuffer, _processorPerfArraySize, out retLen);

            _processorPerf = new SystemProcessorPerformanceInformation();

            // Thanks to:
            // http://www.netperf.org/svn/netperf2/trunk/src/netcpu_ntperf.c
            // for the critical information:
            // "KernelTime needs to be fixed-up; it includes both idle & true kernel time".
            // This is why I love free software.
            for (int i = 0; i < _processorPerfArray.Length; i++)
            {
                var cpuPerf = _processorPerfBuffer.ReadStruct<SystemProcessorPerformanceInformation>(i);

                cpuPerf.KernelTime -= cpuPerf.IdleTime + cpuPerf.DpcTime + cpuPerf.InterruptTime;
                _processorPerf.DpcTime += cpuPerf.DpcTime;
                _processorPerf.IdleTime += cpuPerf.IdleTime;
                _processorPerf.InterruptCount += cpuPerf.InterruptCount;
                _processorPerf.InterruptTime += cpuPerf.InterruptTime;
                _processorPerf.KernelTime += cpuPerf.KernelTime;
                _processorPerf.UserTime += cpuPerf.UserTime;
                _processorPerfArray[i] = cpuPerf;
            }
        }

        private void UpdatePerformance()
        {
            int retLen;

            Win32.NtQuerySystemInformation(SystemInformationClass.SystemPerformanceInformation,
                 out _performance, Marshal.SizeOf(typeof(SystemPerformanceInformation)), out retLen);
        }

        private ProcessQueryMessage QueryProcessStage1(int pid, string fileName, bool forced)
        {
            return QueryProcessStage1(pid, fileName, forced, true);
        }

        /// <summary>
        /// Stage 1 Process Querying - gets the process file name, icon and command line.
        /// </summary>
        private ProcessQueryMessage QueryProcessStage1(int pid, string fileName, bool forced, bool addToQueue)
        {
            ProcessQueryMessage fpResult = new ProcessQueryMessage();

            fpResult.Pid = pid;
            fpResult.Stage = 0x1;

            if (fileName == null)
                fileName = this.GetFileName(pid);

            if (fileName == null)
                Logging.Log(Logging.Importance.Warning, "Could not get file name for PID " + pid.ToString());

            fpResult.FileName = fileName;

            try
            {
                using (var queryLimitedHandle = new ProcessHandle(pid, Program.MinProcessQueryRights))
                {
                    try
                    {
                        // Get a handle to the process' token and get its 
                        // elevation type, and integrity.

                        using (var thandle = queryLimitedHandle.GetToken(TokenAccess.Query))
                        {
                            try { fpResult.ElevationType = thandle.GetElevationType(); }
                            catch { }
                            try { fpResult.IsElevated = thandle.IsElevated(); }
                            catch { }

                            // Try to get the integrity level.
                            try
                            {
                                fpResult.Integrity = thandle.GetIntegrity(out fpResult.IntegrityLevel);
                            }
                            catch
                            { }
                        }
                    }
                    catch
                    { }

                    // Is the process running under WOW64?
                    if (IntPtr.Size == 8)
                    {
                        try
                        {
                            fpResult.IsWow64 = queryLimitedHandle.IsWow64();
                        }
                        catch
                        { }
                    }

                    // Get the process' job if we have KProcessHacker. 
                    // Otherwise, don't do anything.

                    if (KProcessHacker.Instance != null)
                    {
                        try
                        {
                            var jhandle = queryLimitedHandle.GetJobObject(JobObjectAccess.Query);

                            if (jhandle != null)
                            {
                                using (jhandle)
                                {
                                    var limits = jhandle.GetBasicLimitInformation();

                                    fpResult.IsInJob = true;
                                    fpResult.JobName = jhandle.GetObjectName();

                                    // This is what Process Explorer does...
                                    if (limits.LimitFlags != JobObjectLimitFlags.SilentBreakawayOk)
                                    {
                                        fpResult.IsInSignificantJob = true;
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Logging.Log(ex);
                            fpResult.IsInJob = false;
                            fpResult.IsInSignificantJob = false;
                        }
                    }
                    else
                    {
                        try { fpResult.IsInJob = queryLimitedHandle.IsInJob(); }
                        catch { }
                    }
                }
            }
            catch
            { }

            if (fileName != null)
            {
                try
                {
                    fpResult.Icon = FileUtils.GetFileIcon(fileName);
                    fpResult.LargeIcon = FileUtils.GetFileIcon(fileName, true);
                }
                catch
                { }

                try
                {
                    fpResult.VersionInfo = FileVersionInfo.GetVersionInfo(fileName);
                }
                catch
                { }
            }

            if (pid > 4)
            {
                try
                {
                    using (var phandle = new ProcessHandle(pid,
                        Program.MinProcessQueryRights | Program.MinProcessReadMemoryRights))
                    {
                        fpResult.CmdLine = phandle.GetCommandLine();
                        fpResult.IsPosix = phandle.IsPosix();
                    }
                }
                catch
                { }
            }

            if (addToQueue)
                _messageQueue.Enqueue(fpResult);

            WorkQueue.GlobalQueueWorkItemTag(
                new QueryProcessDelegate(this.QueryProcessStage1a),
                "process-stage1a",
                pid, fileName, forced
                );
            WorkQueue.GlobalQueueWorkItemTag(
                new QueryProcessDelegate(this.QueryProcessStage2),
                "process-stage2",
                pid, fileName, forced
                );

            if (this.ProcessQueryComplete != null)
                this.ProcessQueryComplete(fpResult.Stage, pid);

            return fpResult;
        }

        /// <summary>
        /// Stage 1A Process Querying - gets whether the process is managed.
        /// </summary>
        private ProcessQueryMessage QueryProcessStage1a(int pid, string fileName, bool forced)
        {
            ProcessQueryMessage fpResult = new ProcessQueryMessage();

            fpResult.Pid = pid;
            fpResult.Stage = 0x1a;

            if (pid > 4)
            {
                try
                {
                    var publish = new Debugger.Core.Wrappers.CorPub.ICorPublish();
                    Debugger.Core.Wrappers.CorPub.ICorPublishProcess process = null;

                    try
                    {
                        process = publish.GetProcess(pid);
                        fpResult.IsDotNet = process.IsManaged;
                    }
                    finally
                    {
                        if (process != null)
                        {
                            Debugger.Wrappers.ResourceManager.ReleaseCOMObject(process, process.GetType());
                        }
                    }
                }
                catch
                { }
            }

            _messageQueue.Enqueue(fpResult);

            if (this.ProcessQueryComplete != null)
                this.ProcessQueryComplete(fpResult.Stage, pid);

            return fpResult;
        }

        /// <summary>
        /// Stage 2 Process Querying - gets whether the process file is packed or signed.
        /// </summary>
        private ProcessQueryMessage QueryProcessStage2(int pid, string fileName, bool forced)
        {
            ProcessQueryMessage fpResult = new ProcessQueryMessage();

            fpResult.Pid = pid;
            fpResult.Stage = 0x2;
            fpResult.IsPacked = false;

            if (fileName == null)
                return null;

            // Find out if it's packed.
            // An image is packed if:
            // 1. It references less than 3 libraries
            // 2. It imports less than 5 functions
            // or:
            // 1. The function-to-library ratio is lower than 4
            //   (on average less than 4 functions are imported from each library)
            // 2. It references more than 3 libraries but less than 14 libraries.
            if (fileName != null && (Settings.Instance.VerifySignatures || forced))
            {
                try
                {
                    using (var mappedImage = new MappedImage(fileName))
                    {
                        int libraryTotal = mappedImage.Imports.Count;
                        int funcTotal = 0;

                        for (int i = 0; i < mappedImage.Imports.Count; i++)
                            funcTotal += mappedImage.Imports[i].Count;

                        fpResult.ImportModules = libraryTotal;
                        fpResult.ImportFunctions = funcTotal;

                        if (
                            libraryTotal < 3 && funcTotal < 5 ||
                            ((float)funcTotal / libraryTotal < 4) && libraryTotal > 3 && libraryTotal < 30
                            )
                            fpResult.IsPacked = true;
                    }
                }
                catch (AccessViolationException)
                {
                    if (pid > 4)
                        fpResult.IsPacked = true;
                }
                catch
                { }
            }

            try
            {
                if (Settings.Instance.VerifySignatures || forced)
                {
                    if (fileName != null)
                    {
                        string uniName = global::System.IO.Path.GetFullPath(fileName).ToLowerInvariant();

                        // No lock needed; verify results are never removed, only added.
                        if (!forced && _fileResults.ContainsKey(uniName))
                        {
                            fpResult.VerifyResult = _fileResults[uniName];
                        }
                        else
                        {
                            try
                            {
                                fpResult.VerifyResult = Cryptography.VerifyFile(fileName);
                            }
                            catch
                            {
                                fpResult.VerifyResult = VerifyResult.NoSignature;
                            }

                            if (!_fileResults.ContainsKey(uniName))
                                _fileResults.Add(uniName, fpResult.VerifyResult);
                            else
                                _fileResults[uniName] = fpResult.VerifyResult;
                        }

                        //if (fpResult.VerifyResult != VerifyResult.NoSignature)
                        //    fpResult.VerifySignerName = Cryptography.GetFileSubjectValue(fileName, "CN");
                    }
                }
            }
            catch
            { }

            _messageQueue.Enqueue(fpResult);

            if (this.ProcessQueryComplete != null)
                this.ProcessQueryComplete(fpResult.Stage, pid);

            return fpResult;
        }

        private string GetFileName(int pid)
        {
            string fileName = null;

            if (pid != 4)
            {
                try
                {
                    using (var phandle = new ProcessHandle(pid, Program.MinProcessQueryRights))
                    {
                        // First try to get the native file name, to prevent PEB
                        // file name spoofing.
                        try
                        {
                            fileName = FileUtils.GetFileName(phandle.GetImageFileName());
                        }
                        catch
                        { }

                        // If we couldn't get it or we couldn't resolve the \Device prefix,
                        // we'll use the Win32 variant.
                        if ((fileName == null || fileName.StartsWith("\\")) &&
                            OSVersion.HasWin32ImageFileName)
                        {
                            try
                            {
                                fileName = phandle.GetImageFileNameWin32();
                            }
                            catch
                            { }
                        }
                    }
                }
                catch
                { }

                if (fileName == null || fileName.StartsWith("\\Device\\"))
                {
                    try
                    {
                        using (var phandle =
                            new ProcessHandle(pid, ProcessAccess.QueryInformation | ProcessAccess.VmRead))
                        {
                            // We can try to use the PEB.
                            try
                            {
                                fileName = FileUtils.GetFileName(
                                    FileUtils.GetFileName(phandle.GetPebString(PebOffset.ImagePathName)));
                            }
                            catch
                            { }

                            // If all else failed, we get the main module file name.
                            try
                            {
                                fileName = phandle.GetMainModule().FileName;
                            }
                            catch
                            { }
                        }
                    }
                    catch
                    { }
                }
            }
            else
            {
                try
                {
                    fileName = Windows.KernelFileName;
                }
                catch
                { }
            }

            return fileName;
        }

        public void QueueProcessQuery(int pid)
        {
            WorkQueue.GlobalQueueWorkItemTag(
                new QueryProcessDelegate(this.QueryProcessStage1),
                "process-stage1",
                pid, this.Dictionary[pid].FileName, true
                );
        }

        private void FillPqResult(ProcessItem item, ProcessQueryMessage result)
        {
            if (result.Stage == 0x1)
            {
                item.FileName = result.FileName;
                item.ElevationType = result.ElevationType;
                item.IsElevated = result.IsElevated;
                item.Integrity = result.Integrity;
                item.IntegrityLevel = result.IntegrityLevel;
                item.IsWow64 = result.IsWow64;
                item.IsInJob = result.IsInJob;
                item.JobName = result.JobName;
                item.IsInSignificantJob = result.IsInSignificantJob;
                item.Icon = result.Icon;
                item.LargeIcon = result.LargeIcon;
                item.VersionInfo = result.VersionInfo;
                item.CmdLine = result.CmdLine;
                item.IsPosix = result.IsPosix;
            }
            else if (result.Stage == 0x1a)
            {
                item.IsDotNet = result.IsDotNet;

                if (item.IsDotNet)
                    item.IsPacked = false;
            }
            else if (result.Stage == 0x2)
            {
                item.IsPacked = (item.IsDotNet || result.IsDotNet) ? false : result.IsPacked;
                item.VerifyResult = result.VerifyResult;
                item.VerifySignerName = result.VerifySignerName;
                item.ImportFunctions = result.ImportFunctions;
                item.ImportModules = result.ImportModules;
            }
            else
            {
                Logging.Log(Logging.Importance.Warning, "Unknown stage " + result.Stage.ToString("x"));
            }

            if (this.ProcessQueryReceived != null)
                this.ProcessQueryReceived(result.Stage, result.Pid);
        }

        private void UpdateOnce()
        {
            this.UpdatePerformance();
            this.UpdateProcessorPerf();
            //this.UpdateFrozenWindows();

            if (this.RunCount % 3 == 0)
                FileUtils.RefreshFileNamePrefixes();

            var tsProcesses = new Dictionary<int, IntPtr>();
            var procs = Windows.GetProcesses();
            Dictionary<int, ProcessItem> newdictionary = new Dictionary<int, ProcessItem>(this.Dictionary);
            Win32.WtsEnumProcessesFastData wtsEnumData = new Win32.WtsEnumProcessesFastData();

            _longDeltas.Update(SystemStats.CpuKernel, _processorPerf.KernelTime);
            long sysKernelTime = _longDeltas[SystemStats.CpuKernel];

            _longDeltas.Update(SystemStats.CpuUser, _processorPerf.UserTime);
            long sysUserTime = _longDeltas[SystemStats.CpuUser];

            _longDeltas.Update(SystemStats.CpuOther,
                _processorPerf.IdleTime + _processorPerf.DpcTime + _processorPerf.InterruptTime);
            long otherTime = _longDeltas[SystemStats.CpuOther];

            if (sysKernelTime + sysUserTime + otherTime == 0)
            {
                Logging.Log(Logging.Importance.Warning, "Total systimes are 0, returning!");
                return;
            }

            _longDeltas.Update(SystemStats.IoRead, _performance.IoReadTransferCount);
            _longDeltas.Update(SystemStats.IoWrite, _performance.IoWriteTransferCount);
            _longDeltas.Update(SystemStats.IoOther, _performance.IoOtherTransferCount);

            if (_processorPerf.KernelTime != 0 && _processorPerf.UserTime != 0)
            {
                this.CurrentCpuKernelUsage = (float)sysKernelTime / (sysKernelTime + sysUserTime + otherTime);
                this.CurrentCpuUserUsage = (float)sysUserTime / (sysKernelTime + sysUserTime + otherTime);

                _floatHistory.Update("Kernel", this.CurrentCpuKernelUsage);
                _floatHistory.Update("User", this.CurrentCpuUserUsage);
                _floatHistory.Update("Other", (float)otherTime / (sysKernelTime + sysUserTime + otherTime));
            }

            for (int i = 0; i < this.System.NumberOfProcessors; i++)
            {
                long cpuKernelTime = _cpuDeltas.Update(i.ToString() + " Kernel", _processorPerfArray[i].KernelTime);
                long cpuUserTime = _cpuDeltas.Update(i.ToString() + " User", _processorPerfArray[i].UserTime);
                long cpuOtherTime = _cpuDeltas.Update(i.ToString() + " Other",
                    _processorPerfArray[i].IdleTime + _processorPerfArray[i].DpcTime +
                    _processorPerfArray[i].InterruptTime);
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
            _longHistory.Update(SystemStats.Commit, (long)_performance.CommittedPages * _system.PageSize);
            _longHistory.Update(SystemStats.PhysicalMemory,
                (long)(_system.NumberOfPhysicalPages - _performance.AvailablePages) * _system.PageSize);

            // set System Idle Process CPU time
            if (procs.ContainsKey(0))
            {
                SystemProcess proc = procs[0];
                proc.Process.KernelTime = _processorPerf.IdleTime;
                procs[0] = proc;
            }

            // add fake processes (DPCs and Interrupts)
            _dpcs.Process.KernelTime = _processorPerf.DpcTime;
            procs.Add(-2, _dpcs);

            _interrupts.Process.KernelTime = _processorPerf.InterruptTime;
            procs.Add(-3, _interrupts);

            float mostCPUUsage = 0;
            long mostIOActivity = 0;

            // look for dead processes
            foreach (int pid in Dictionary.Keys)
            {
                if (!procs.ContainsKey(pid))
                {
                    ProcessItem item = this.Dictionary[pid];

                    this.OnDictionaryRemoved(item);

                    if (item.ProcessQueryHandle != null)
                        item.ProcessQueryHandle.Dispose();

                    if (item.Icon != null)
                        Win32.DestroyIcon(item.Icon.Handle);
                    if (item.LargeIcon != null)
                        Win32.DestroyIcon(item.LargeIcon.Handle);

                    // Remove process protection if needed.
                    if (KProcessHacker.Instance != null)
                    {
                        try
                        {
                            using (var phandle = new ProcessHandle(pid, Program.MinProcessQueryRights))
                                KProcessHacker.Instance.ProtectRemove(phandle);
                        }
                        catch
                        { }
                    }

                    newdictionary.Remove(pid);
                }
            }

            // Receive any processing results.
            _messageQueue.Listen();

            // look for new processes
            foreach (int pid in procs.Keys)
            {
                var processInfo = procs[pid].Process;

                if (!Dictionary.ContainsKey(pid))
                {
                    ProcessItem item = new ProcessItem();

                    // Set up basic process information.
                    item.RunId = this.RunCount;
                    item.Pid = pid;
                    item.Process = processInfo;
                    item.SessionId = processInfo.SessionId;
                    item.ProcessingAttempts = 1;

                    item.Name = procs[pid].Name;

                    // Create the delta and history managers.
                    item.DeltaManager = new DeltaManager<ProcessStats, long>(
                        Subtractor.Int64Subtractor, EnumComparer<ProcessStats>.Instance);
                    item.DeltaManager.Add(ProcessStats.CpuKernel, processInfo.KernelTime);
                    item.DeltaManager.Add(ProcessStats.CpuUser, processInfo.UserTime);
                    item.DeltaManager.Add(ProcessStats.IoRead, (long)processInfo.IoCounters.ReadTransferCount);
                    item.DeltaManager.Add(ProcessStats.IoWrite, (long)processInfo.IoCounters.WriteTransferCount);
                    item.DeltaManager.Add(ProcessStats.IoOther, (long)processInfo.IoCounters.OtherTransferCount);
                    item.FloatHistoryManager = 
                        new HistoryManager<ProcessStats, float>(EnumComparer<ProcessStats>.Instance);
                    item.LongHistoryManager =
                        new HistoryManager<ProcessStats, long>(EnumComparer<ProcessStats>.Instance);
                    item.FloatHistoryManager.Add(ProcessStats.CpuKernel);
                    item.FloatHistoryManager.Add(ProcessStats.CpuUser);
                    item.LongHistoryManager.Add(ProcessStats.IoReadOther);
                    item.LongHistoryManager.Add(ProcessStats.IoRead);
                    item.LongHistoryManager.Add(ProcessStats.IoWrite);
                    item.LongHistoryManager.Add(ProcessStats.IoOther);
                    item.LongHistoryManager.Add(ProcessStats.PrivateMemory);
                    item.LongHistoryManager.Add(ProcessStats.WorkingSet);

                    // HACK: Shouldn't happen, but it does - sometimes 
                    // the process name is null.
                    if (item.Name == null)
                    {
                        try
                        {
                            using (var phandle = 
                                new ProcessHandle(pid, ProcessAccess.QueryInformation | ProcessAccess.VmRead))
                                item.Name = phandle.GetMainModule().BaseName;
                        }
                        catch
                        {
                            item.Name = "";
                        }
                    }

                    // Get the process' creation time and check the 
                    // parent process ID.

                    try
                    {
                        item.CreateTime = DateTime.FromFileTime(processInfo.CreateTime);
                    }
                    catch
                    { }

                    if (pid > 0)
                    {
                        item.ParentPid = processInfo.InheritedFromProcessId;
                        item.HasParent = true;

                        if (!procs.ContainsKey(item.ParentPid) || item.ParentPid == pid)
                        {
                            item.HasParent = false;
                        }
                        else if (procs.ContainsKey(item.ParentPid))
                        {
                            // Check the parent's creation time to see if it's actually the parent.
                            ulong parentStartTime = (ulong)procs[item.ParentPid].Process.CreateTime;
                            ulong thisStartTime = (ulong)processInfo.CreateTime;

                            if (parentStartTime > thisStartTime)
                                item.HasParent = false;
                        }

                        // Get the process' token's username.

                        try
                        {
                            using (var queryLimitedHandle = new ProcessHandle(pid, Program.MinProcessQueryRights))
                            {
                                try
                                {
                                    using (var thandle = queryLimitedHandle.GetToken(TokenAccess.Query))
                                    {
                                        try
                                        {
                                            using (var sid = thandle.GetUser())
                                                item.Username = sid.GetFullName(true);
                                        }
                                        catch
                                        { }
                                    }
                                }
                                catch
                                { }
                            }
                        }
                        catch
                        { }

                        // Get a process handle with QUERY_INFORMATION access, and 
                        // see if it's being debugged.

                        try
                        {
                            item.ProcessQueryHandle = new ProcessHandle(pid, ProcessAccess.QueryInformation);

                            try
                            {
                                item.IsBeingDebugged = item.ProcessQueryHandle.IsBeingDebugged();
                            }
                            catch
                            { }
                        }
                        catch
                        { }
                    }

                    // Update the process name if it's a fake process.

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

                    // If this is not the first run, we process the item immediately.
                    if (this.RunCount > 0)
                    {
                        this.FillPqResult(item, this.QueryProcessStage1(pid, null, false, false));
                    }
                    else
                    {
                        if (pid > 0)
                        {
                            WorkQueue.GlobalQueueWorkItemTag(
                                new QueryProcessDelegate(this.QueryProcessStage1),
                                "process-stage1",
                                pid, item.FileName, false);
                        }
                    }

                    // Set the username for System Idle Process and System.
                    if (pid == 0 || pid == 4)
                    {
                        // TODO: Potential localization problem. Need to create 
                        // a well-known SID and use that.
                        item.Username = "NT AUTHORITY\\SYSTEM";
                    }

                    // If we didn't get a username, try to use Terminal Services 
                    // to get the SID of the process' token's user.
                    if (pid > 4 && item.Username == null)
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
                            item.Username = Sid.FromPointer(tsProcesses[pid]).GetFullName(true);
                        }
                        catch
                        { }
                    }

                    newdictionary.Add(pid, item);
                    this.OnDictionaryAdded(item);
                }
                // look for modified processes
                else
                {
                    ProcessItem item = this.Dictionary[pid];
                    bool fullUpdate = false;

                    // Update process performance information.

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
                    item.LongHistoryManager.Update(ProcessStats.PrivateMemory, processInfo.VirtualMemoryCounters.PrivatePageCount.ToInt64());
                    item.LongHistoryManager.Update(ProcessStats.WorkingSet, processInfo.VirtualMemoryCounters.WorkingSetSize.ToInt64());

                    // Update the struct.
                    item.Process = processInfo;

                    // Update CPU usage, and update PIDs with most activity.

                    try
                    {
                        item.CpuUsage = (float)
                            (item.DeltaManager[ProcessStats.CpuUser] + 
                            item.DeltaManager[ProcessStats.CpuKernel]) * 100 /
                            (sysKernelTime + sysUserTime + otherTime);

                        // HACK.

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

                    // Determine whether the process is being debugged.

                    if (item.ProcessQueryHandle != null)
                    {
                        try
                        {
                            bool isBeingDebugged = item.ProcessQueryHandle.IsBeingDebugged();

                            if (isBeingDebugged != item.IsBeingDebugged)
                            {
                                item.IsBeingDebugged = isBeingDebugged;
                                fullUpdate = true;
                            }
                        }
                        catch
                        { }
                    }

                    // Processes sometimes mistakenly get labeled as packed. 
                    // Try again if it is packed.
                    if (pid > 0)
                    {
                        if (item.IsPacked && item.ProcessingAttempts < 3)
                        {
                            WorkQueue.GlobalQueueWorkItemTag(
                                new QueryProcessDelegate(this.QueryProcessStage2),
                                "process-stage2",
                                pid, item.FileName, true
                                );
                            item.ProcessingAttempts++;
                        }
                    }

                    if (item.JustProcessed)
                        fullUpdate = true;

                    // If we need a full update, call the dictionary modified 
                    // event so the process tree updates the process' 
                    // highlighting color.
                    if (fullUpdate)
                    {
                        this.OnDictionaryModified(null, item);
                    }

                    item.JustProcessed = false;
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
                    "R+O: " + Utils.FormatSize(
                    newdictionary[this.PIDWithMostIoActivity].LongHistoryManager[ProcessStats.IoReadOther][0]) +
                    ", W: " + Utils.FormatSize(
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
