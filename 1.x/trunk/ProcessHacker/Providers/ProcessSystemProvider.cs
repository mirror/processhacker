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
    public class ImageVersionInfo
    {
        public ImageVersionInfo()
        { }

        public ImageVersionInfo(FileVersionInfo info)
        {
            this.CompanyName = info.CompanyName;
            this.FileDescription = info.FileDescription;
            this.FileName = info.FileName;
            this.FileVersion = info.FileVersion;
            this.ProductName = info.ProductName;
        }

        public string CompanyName { get; set; }
        public string FileDescription { get; set; }
        public string FileName { get; set; }
        public string FileVersion { get; set; }
        public string ProductName { get; set; }
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
        public ImageVersionInfo VersionInfo;
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

        public Int64Delta CpuKernelDelta;
        public Int64Delta CpuUserDelta;
        public Int64Delta IoReadDelta;
        public Int64Delta IoWriteDelta;
        public Int64Delta IoOtherDelta;

        public CircularBuffer<float> CpuKernelHistory;
        public CircularBuffer<float> CpuUserHistory;
        public CircularBuffer<long> IoReadHistory;
        public CircularBuffer<long> IoWriteHistory;
        public CircularBuffer<long> IoOtherHistory;
        public CircularBuffer<long> IoReadOtherHistory;
        public CircularBuffer<long> PrivateMemoryHistory;
        public CircularBuffer<long> WorkingSetHistory;
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
            public ImageVersionInfo VersionInfo;
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
        public int PidWithMostIoActivity { get; private set; }
        public int PidWithMostCpuUsage { get; private set; }

        public Int64Delta CpuKernelDelta { get { return _cpuKernelDelta; } }
        public Int64Delta CpuUserDelta { get { return _cpuUserDelta; } }
        public Int64Delta CpuOtherDelta { get { return _cpuOtherDelta; } }
        public Int64Delta[] CpuKernelDeltas { get { return _cpuKernelDeltas; } }
        public Int64Delta[] CpuUserDeltas { get { return _cpuUserDeltas; } }
        public Int64Delta[] CpuOtherDeltas { get { return _cpuOtherDeltas; } }
        public Int64Delta IoReadDelta { get { return _ioReadDelta; } }
        public Int64Delta IoWriteDelta { get { return _ioWriteDelta; } }
        public Int64Delta IoOtherDelta { get { return _ioOtherDelta; } }

        public int HistoryMaxSize { get { return _historyMaxSize; } set { _historyMaxSize = value; } }
        public IList<long> IoReadHistory { get { return _ioReadHistory; } }
        public IList<long> IoWriteHistory { get { return _ioWriteHistory; } }
        public IList<long> IoOtherHistory { get { return _ioOtherHistory; } }
        public IList<long> IoReadOtherHistory { get { return _ioReadOtherHistory; } }
        public IList<float> CpuKernelHistory { get { return _cpuKernelHistory; } }
        public IList<float> CpuUserHistory { get { return _cpuUserHistory; } }
        public IList<float> CpuOtherHistory { get { return _cpuOtherHistory; } }
        public IList<float>[] CpusKernelHistory { get { return _cpusKernelHistory; } }
        public IList<float>[] CpusUserHistory { get { return _cpusUserHistory; } }
        public IList<float>[] CpusOtherHistory { get { return _cpusOtherHistory; } }
        public IList<long> CommitHistory { get { return _commitHistory; } }
        public IList<long> PhysicalMemoryHistory { get { return _physicalMemoryHistory; } }
        public IList<DateTime> TimeHistory { get { return _timeHistory; } }
        public IList<string> MostCpuHistory { get { return _cpuMostUsageHistory; } }
        public IList<string> MostIoHistory { get { return _ioMostUsageHistory; } }

        private delegate ProcessQueryMessage QueryProcessDelegate(int pid, string fileName, bool useCache);

        private MessageQueue _messageQueue = new MessageQueue();
        private Dictionary<string, VerifyResult> _fileResults = new Dictionary<string, VerifyResult>();

        private Int64Delta _ioReadDelta;
        private Int64Delta _ioWriteDelta;
        private Int64Delta _ioOtherDelta;
        private Int64Delta _cpuKernelDelta;
        private Int64Delta _cpuUserDelta;
        private Int64Delta _cpuOtherDelta;
        private Int64Delta[] _cpuKernelDeltas;
        private Int64Delta[] _cpuUserDeltas;
        private Int64Delta[] _cpuOtherDeltas;

        private int _historyMaxSize = 100;
        private CircularBuffer<long> _ioReadHistory;
        private CircularBuffer<long> _ioWriteHistory;
        private CircularBuffer<long> _ioOtherHistory;
        private CircularBuffer<long> _ioReadOtherHistory;
        private CircularBuffer<float> _cpuKernelHistory;
        private CircularBuffer<float> _cpuUserHistory;
        private CircularBuffer<float> _cpuOtherHistory;
        private CircularBuffer<float>[] _cpusKernelHistory;
        private CircularBuffer<float>[] _cpusUserHistory;
        private CircularBuffer<float>[] _cpusOtherHistory;
        private CircularBuffer<long> _commitHistory;
        private CircularBuffer<long> _physicalMemoryHistory;
        private CircularBuffer<DateTime> _timeHistory;
        private CircularBuffer<string> _cpuMostUsageHistory;
        private CircularBuffer<string> _ioMostUsageHistory;

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

            // Initialize the deltas

            _cpuKernelDelta = new Int64Delta(this.ProcessorPerf.KernelTime);
            _cpuUserDelta = new Int64Delta(this.ProcessorPerf.UserTime);
            _cpuOtherDelta = new Int64Delta(
                this.ProcessorPerf.IdleTime + this.ProcessorPerf.DpcTime + this.ProcessorPerf.InterruptTime);
            _ioReadDelta = new Int64Delta(this.Performance.IoReadTransferCount);
            _ioWriteDelta = new Int64Delta(this.Performance.IoWriteTransferCount);
            _ioOtherDelta = new Int64Delta(this.Performance.IoOtherTransferCount);

            // Initialize history

            _cpuKernelHistory = new CircularBuffer<float>(_historyMaxSize);
            _cpuUserHistory = new CircularBuffer<float>(_historyMaxSize);
            _cpuOtherHistory = new CircularBuffer<float>(_historyMaxSize);
            _ioReadHistory = new CircularBuffer<long>(_historyMaxSize);
            _ioWriteHistory = new CircularBuffer<long>(_historyMaxSize);
            _ioOtherHistory = new CircularBuffer<long>(_historyMaxSize);
            _ioReadOtherHistory = new CircularBuffer<long>(_historyMaxSize);
            _commitHistory = new CircularBuffer<long>(_historyMaxSize);
            _physicalMemoryHistory = new CircularBuffer<long>(_historyMaxSize);
            _timeHistory = new CircularBuffer<DateTime>(_historyMaxSize);
            _ioMostUsageHistory = new CircularBuffer<string>(_historyMaxSize);
            _cpuMostUsageHistory = new CircularBuffer<string>(_historyMaxSize);

            // Initialize deltas and history for the CPUs

            _cpuKernelDeltas = new Int64Delta[this.System.NumberOfProcessors];
            _cpuUserDeltas = new Int64Delta[this.System.NumberOfProcessors];
            _cpuOtherDeltas = new Int64Delta[this.System.NumberOfProcessors];

            _cpusKernelHistory = new CircularBuffer<float>[this.System.NumberOfProcessors];
            _cpusUserHistory = new CircularBuffer<float>[this.System.NumberOfProcessors];
            _cpusOtherHistory = new CircularBuffer<float>[this.System.NumberOfProcessors];

            for (int i = 0; i < this.System.NumberOfProcessors; i++)
            {
                Int64Delta.Update(ref _cpuKernelDeltas[i], this.ProcessorPerfArray[i].KernelTime);
                Int64Delta.Update(ref _cpuUserDeltas[i], this.ProcessorPerfArray[i].UserTime);
                Int64Delta.Update(ref _cpuOtherDeltas[i],
                    this.ProcessorPerfArray[i].IdleTime + this.ProcessorPerfArray[i].DpcTime +
                    this.ProcessorPerfArray[i].InterruptTime);

                _cpusKernelHistory[i] = new CircularBuffer<float>(_historyMaxSize);
                _cpusUserHistory[i] = new CircularBuffer<float>(_historyMaxSize);
                _cpusOtherHistory[i] = new CircularBuffer<float>(_historyMaxSize);
            }
        }

        public SystemProcess DpcsProcess
        {
            get { return _dpcs; }
        }

        public SystemProcess InterruptsProcess
        {
            get { return _interrupts; }
        }

        private void UpdateCb<T>(CircularBuffer<T> cb, T value)
        {
            if (cb.Size != _historyMaxSize)
                cb.Resize(_historyMaxSize);

            cb.Add(value);
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
                 out _performance, SystemPerformanceInformation.Size, out retLen);
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
                    if (OSVersion.Architecture == OSArch.Amd64)
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
                    fpResult.VersionInfo = new ImageVersionInfo(FileVersionInfo.GetVersionInfo(fileName));
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
                    fpResult.IsDotNet = PhUtils.IsDotNetProcess(pid);
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

            // Don't process the file if it is too big (above 32MB).
            try
            {
                if ((new global::System.IO.FileInfo(fileName)).Length > 32 * 1024 * 1024)
                    return null;
            }
            catch
            {
                return null;
            }

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
                                fpResult.VerifyResult = Cryptography.VerifyFile(fileName, out fpResult.VerifySignerName);
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

        protected override void Update()
        {
            this.UpdatePerformance();
            this.UpdateProcessorPerf();

            if (this.RunCount % 3 == 0)
                FileUtils.RefreshFileNamePrefixes();

            Dictionary<int, IntPtr> tsProcesses = null;
            var procs = Windows.GetProcesses();
            Dictionary<int, ProcessItem> newdictionary = new Dictionary<int, ProcessItem>(this.Dictionary);
            Win32.WtsEnumProcessesFastData wtsEnumData = new Win32.WtsEnumProcessesFastData();

            _cpuKernelDelta.Update(_processorPerf.KernelTime);
            _cpuUserDelta.Update(_processorPerf.UserTime);
            _cpuOtherDelta.Update(
                _processorPerf.IdleTime + _processorPerf.DpcTime + _processorPerf.InterruptTime);

            long sysKernelTime = _cpuKernelDelta.Delta;
            long sysUserTime = _cpuUserDelta.Delta;
            long otherTime = _cpuOtherDelta.Delta;

            if (sysKernelTime + sysUserTime + otherTime == 0)
            {
                Logging.Log(Logging.Importance.Warning, "Total systimes are 0, returning!");
                return;
            }

            _ioReadDelta.Update(_performance.IoReadTransferCount);
            _ioWriteDelta.Update(_performance.IoWriteTransferCount);
            _ioOtherDelta.Update(_performance.IoOtherTransferCount);

            if (_processorPerf.KernelTime != 0 && _processorPerf.UserTime != 0)
            {
                this.CurrentCpuKernelUsage = (float)sysKernelTime / (sysKernelTime + sysUserTime + otherTime);
                this.CurrentCpuUserUsage = (float)sysUserTime / (sysKernelTime + sysUserTime + otherTime);

                UpdateCb(_cpuKernelHistory, this.CurrentCpuKernelUsage);
                UpdateCb(_cpuUserHistory, this.CurrentCpuUsage);
                UpdateCb(_cpuOtherHistory, (float)otherTime / (sysKernelTime + sysUserTime + otherTime));
            }

            for (int i = 0; i < this.System.NumberOfProcessors; i++)
            {
                Int64Delta.Update(ref _cpuKernelDeltas[i], _processorPerfArray[i].KernelTime);
                Int64Delta.Update(ref _cpuUserDeltas[i], _processorPerfArray[i].UserTime);
                Int64Delta.Update(ref _cpuOtherDeltas[i],
                    _processorPerfArray[i].IdleTime + _processorPerfArray[i].DpcTime +
                    _processorPerfArray[i].InterruptTime);

                long cpuKernelTime = _cpuKernelDeltas[i].Delta;
                long cpuUserTime = _cpuUserDeltas[i].Delta;
                long cpuOtherTime = _cpuOtherDeltas[i].Delta;

                UpdateCb(_cpusKernelHistory[i], 
                    (float)cpuKernelTime / (cpuKernelTime + cpuUserTime + cpuOtherTime));
                UpdateCb(_cpusUserHistory[i],
                    (float)cpuUserTime / (cpuKernelTime + cpuUserTime + cpuOtherTime));
                UpdateCb(_cpusOtherHistory[i],
                    (float)cpuOtherTime / (cpuKernelTime + cpuUserTime + cpuOtherTime));
            }

            // Prevent a massive spike in the I/O graph when the program is starting.
            if (this.RunCount < 3)
            {
                _ioReadDelta.Update(_ioReadDelta.Value);
                _ioWriteDelta.Update(_ioWriteDelta.Value);
                _ioOtherDelta.Update(_ioOtherDelta.Value);
            }

            UpdateCb(_ioReadHistory, _ioReadDelta.Delta);
            UpdateCb(_ioWriteHistory, _ioWriteDelta.Delta);
            UpdateCb(_ioOtherHistory, _ioOtherDelta.Delta);
            UpdateCb(_ioReadOtherHistory, _ioReadDelta.Delta + _ioOtherDelta.Delta);
            UpdateCb(_commitHistory, (long)_performance.CommittedPages * _system.PageSize);
            UpdateCb(_physicalMemoryHistory,
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

                    item.CpuKernelDelta = new Int64Delta(processInfo.KernelTime);
                    item.CpuUserDelta = new Int64Delta(processInfo.UserTime);
                    item.IoReadDelta = new Int64Delta((long)processInfo.IoCounters.ReadTransferCount);
                    item.IoWriteDelta = new Int64Delta((long)processInfo.IoCounters.WriteTransferCount);
                    item.IoOtherDelta = new Int64Delta((long)processInfo.IoCounters.OtherTransferCount);

                    item.CpuKernelHistory = new CircularBuffer<float>(_historyMaxSize);
                    item.CpuUserHistory = new CircularBuffer<float>(_historyMaxSize);
                    item.IoReadHistory = new CircularBuffer<long>(_historyMaxSize);
                    item.IoWriteHistory = new CircularBuffer<long>(_historyMaxSize);
                    item.IoOtherHistory = new CircularBuffer<long>(_historyMaxSize);
                    item.IoReadOtherHistory = new CircularBuffer<long>(_historyMaxSize);
                    item.PrivateMemoryHistory = new CircularBuffer<long>(_historyMaxSize);
                    item.WorkingSetHistory = new CircularBuffer<long>(_historyMaxSize);

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
                        if (tsProcesses == null)
                        {
                            // Delay loading until this point.
                            tsProcesses = new Dictionary<int, IntPtr>();
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

                    item.CpuKernelDelta.Update(processInfo.KernelTime);
                    item.CpuUserDelta.Update(processInfo.UserTime);
                    item.IoReadDelta.Update((long)processInfo.IoCounters.ReadTransferCount);
                    item.IoWriteDelta.Update((long)processInfo.IoCounters.WriteTransferCount);
                    item.IoOtherDelta.Update((long)processInfo.IoCounters.OtherTransferCount);

                    UpdateCb(item.CpuKernelHistory,
                        (float)item.CpuKernelDelta.Delta /
                        (sysKernelTime + sysUserTime + otherTime));
                    UpdateCb(item.CpuUserHistory,
                        (float)item.CpuUserDelta.Delta /
                        (sysKernelTime + sysUserTime + otherTime));
                    UpdateCb(item.IoReadHistory, item.IoReadDelta.Delta);
                    UpdateCb(item.IoWriteHistory, item.IoWriteDelta.Delta);
                    UpdateCb(item.IoOtherHistory, item.IoOtherDelta.Delta);
                    UpdateCb(item.IoReadOtherHistory,
                        item.IoReadDelta.Delta + item.IoOtherDelta.Delta);
                    UpdateCb(item.PrivateMemoryHistory,
                        processInfo.VirtualMemoryCounters.PrivatePageCount.ToInt64());
                    UpdateCb(item.WorkingSetHistory,
                        processInfo.VirtualMemoryCounters.WorkingSetSize.ToInt64());

                    // Update the struct.
                    item.Process = processInfo;

                    // Update CPU usage, and update PIDs with most activity.

                    try
                    {
                        item.CpuUsage = (float)
                            (item.CpuUserDelta.Delta + item.CpuKernelDelta.Delta) * 100 /
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
                            this.PidWithMostCpuUsage = pid;
                        }

                        if (pid != 0 && (item.IoReadDelta.Delta + item.IoWriteDelta.Delta) > mostIOActivity)
                        {
                            mostIOActivity = item.IoReadDelta.Delta + item.IoWriteDelta.Delta;
                            this.PidWithMostIoActivity = pid;
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
                UpdateCb(_cpuMostUsageHistory, newdictionary[this.PidWithMostCpuUsage].Name + ": " +
                    newdictionary[this.PidWithMostCpuUsage].CpuUsage.ToString("N2") + "%");
            }
            catch
            {
                UpdateCb(_cpuMostUsageHistory, "");
            }

            try
            {
                UpdateCb(_ioMostUsageHistory, newdictionary[this.PidWithMostIoActivity].Name + ": " +
                    "R+O: " + Utils.FormatSize(
                    newdictionary[this.PidWithMostIoActivity].IoReadOtherHistory[0]) +
                    ", W: " + Utils.FormatSize(
                    newdictionary[this.PidWithMostIoActivity].IoWriteHistory[0]));
            }
            catch
            {
                UpdateCb(_ioMostUsageHistory, "");
            }

            UpdateCb(_timeHistory, DateTime.Now);

            Dictionary = newdictionary;

            if (wtsEnumData.Memory != null)
                wtsEnumData.Memory.Dispose();
        }
    }
}
