/*
 * Process Hacker - 
 *   thread provider
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
using System.Threading;
using ProcessHacker.Common;
using ProcessHacker.Common.Messaging;
using ProcessHacker.Common.Threading;
using ProcessHacker.Native;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;
using ProcessHacker.Native.Symbols;

namespace ProcessHacker
{
    public class ThreadItem : ICloneable
    {
        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public int RunId;
        public int Tid;

        public long ContextSwitches;
        public long ContextSwitchesDelta;
        public ulong Cycles;
        public ulong CyclesDelta;
        public int PriorityI;
        public string Priority;
        public IntPtr StartAddressI;
        public string StartAddress;
        public string FileName;
        public SymbolResolveLevel StartAddressLevel;
        public KWaitReason WaitReason;
        public bool IsGuiThread;
        public bool JustResolved;

        public ThreadHandle ThreadQueryLimitedHandle;
    }

    public class ThreadProvider : Provider<int, ThreadItem>
    {
        private class ResolveMessage : Message
        {
            public int Tid;
            public string Symbol;
            public string FileName;
            public SymbolResolveLevel ResolveLevel;
        }

        public delegate void LoadingStateChangedDelegate(bool loading);
        private delegate void ResolveThreadStartAddressDelegate(int tid, ulong startAddress);

        private static readonly WorkQueue _symbolsWorkQueue = new WorkQueue() { MaxWorkerThreads = 1 };

        public event LoadingStateChangedDelegate LoadingStateChanged;

        private ProcessHandle _processHandle;
        private ProcessAccess _processAccess;
        private SymbolProvider _symbols;
        private int _kernelSymbolsLoaded = 0;
        private int _pid;
        private int _loading = 0;
        private MessageQueue _messageQueue = new MessageQueue();
        private int _symbolsStartedLoading = 0;
        private FastEvent _moduleLoadCompletedEvent = new FastEvent(false);

        public ThreadProvider(int pid)
            : base()
        {
            this.Name = this.GetType().Name;
            _pid = pid;

            _messageQueue.AddListener(
                new MessageQueueListener<ResolveMessage>((message) =>
                {
                    if (message.Symbol != null)
                    {
                        this.Dictionary[message.Tid].StartAddress = message.Symbol;
                        this.Dictionary[message.Tid].FileName = message.FileName;
                        this.Dictionary[message.Tid].StartAddressLevel = message.ResolveLevel;
                        this.Dictionary[message.Tid].JustResolved = true;
                    }
                }));

            this.Disposed += ThreadProvider_Disposed;

            // Try to get a good process handle we can use the same handle for stack walking.
            try
            {
                _processAccess = ProcessAccess.QueryInformation | ProcessAccess.VmRead;
                _processHandle = new ProcessHandle(_pid, _processAccess);
            }
            catch
            {
                try
                {
                    if (KProcessHacker.Instance != null)
                    {
                        _processAccess = Program.MinProcessReadMemoryRights;
                        _processHandle = new ProcessHandle(_pid, _processAccess);
                    }
                    else
                    {
                        _processAccess = Program.MinProcessQueryRights;
                        _processHandle = new ProcessHandle(_pid, _processAccess);
                    }
                }
                catch (WindowsException ex)
                {
                    Logging.Log(ex);
                }
            }
        }

        public ProcessAccess ProcessAccess
        {
            get { return _processAccess; }
        }

        public ProcessHandle ProcessHandle
        {
            get { return _processHandle; }
        }

        public void LoadKernelSymbols()
        {
            this.LoadKernelSymbols(false);
        }

        public void LoadKernelSymbols(bool force)
        {
            // Ensure we only load kernel symbols once.
            if (Interlocked.CompareExchange(ref _kernelSymbolsLoaded, 1, 0) == 1)
                return;

            if (KProcessHacker.Instance != null || force)
                _symbols.LoadKernelModules();
        }

        private void LoadSymbols()
        {
            // Ensure we only load symbols once.
            if (Interlocked.CompareExchange(ref _symbolsStartedLoading, 1, 0) == 1)
                return;

            // Start loading symbols; avoid the UI blocking on the dbghelp call lock.
            _symbolsWorkQueue.QueueWorkItemTag(new Action(() =>
            {
                try
                {
                    // Needed (maybe) to display the EULA
                    Win32.SymbolServerSetOptions(SymbolServerOption.Unattended, 0);
                }
                catch (Exception ex)
                {
                    Logging.Log(ex);
                }

                try
                {
                    // Use the process handle if we have one, otherwise use the default ID generator.
                    if (_processHandle != null)
                        _symbols = new SymbolProvider(_processHandle);
                    else
                        _symbols = new SymbolProvider();

                    SymbolProvider.Options = SymbolOptions.DeferredLoads |
                        (Settings.Instance.DbgHelpUndecorate ? SymbolOptions.UndName : 0);

                    if (Settings.Instance.DbgHelpSearchPath != "")
                        _symbols.SearchPath = Settings.Instance.DbgHelpSearchPath;

                    try
                    {
                        if (_pid > 4)
                        {
                            using (var phandle =
                                new ProcessHandle(_pid, Program.MinProcessQueryRights | Program.MinProcessReadMemoryRights))
                            {
                                if (OSVersion.Architecture == OSArch.I386 || !phandle.IsWow64())
                                {
                                    // Load the process' modules.
                                    try { _symbols.LoadProcessModules(phandle); }
                                    catch { }
                                }
                                else
                                {
                                    // Load the process' WOW64 modules.
                                    try { _symbols.LoadProcessWow64Modules(_pid); }
                                    catch { }
                                }

                                // If the process is CSRSS we should load kernel modules 
                                // due to the presence of kernel-mode threads.
                                if (phandle.GetKnownProcessType() == KnownProcess.WindowsSubsystem)
                                    this.LoadKernelSymbols(true);
                            }
                        }
                        else
                        {
                            this.LoadKernelSymbols(true);
                        }
                    }
                    catch (WindowsException ex)
                    {
                        // Did we get Access Denied? At least load 
                        // kernel32.dll and ntdll.dll.
                        try
                        {
                            ProcessHandle.Current.EnumModules((module) =>
                            {
                                if (
                                    module.BaseName.Equals("kernel32.dll", StringComparison.OrdinalIgnoreCase) ||
                                    module.BaseName.Equals("ntdll.dll", StringComparison.OrdinalIgnoreCase)
                                    )
                                {
                                    _symbols.LoadModule(module.FileName, module.BaseAddress, module.Size);
                                }

                                return true;
                            });
                        }
                        catch (Exception ex2)
                        {
                            Logging.Log(ex2);
                        }

                        Logging.Log(ex);
                    }
                    catch (Exception ex)
                    {
                        Logging.Log(ex);
                    }
                }
                finally
                {
                    _moduleLoadCompletedEvent.Set();
                }
            }), "symbols-load");
        }

        private void ThreadProvider_Disposed(IProvider provider)
        {
            if (_symbols != null)
                _symbols.Dispose();
            if (_processHandle != null)
                _processHandle.Dispose();
            _symbols = null;

            foreach (int tid in this.Dictionary.Keys)
            {
                ThreadItem item = this.Dictionary[tid];

                if (item.ThreadQueryLimitedHandle != null)
                    item.ThreadQueryLimitedHandle.Dispose();
            }
        }

        private void ResolveThreadStartAddress(int tid, ulong startAddress)
        {
            ResolveMessage result = new ResolveMessage();

            result.Tid = tid;

            _moduleLoadCompletedEvent.Wait();

            if (_symbols == null)
                return;

            try
            {
                Interlocked.Increment(ref _loading);

                if (this.LoadingStateChanged != null)
                    this.LoadingStateChanged(Thread.VolatileRead(ref _loading) > 0);

                try
                {
                    SymbolFlags flags;
                    string fileName;

                    result.Symbol = _symbols.GetSymbolFromAddress(
                        startAddress,
                        out result.ResolveLevel,
                        out flags,
                        out fileName
                        );
                    result.FileName = fileName;
                    _messageQueue.Enqueue(result);
                }
                catch
                { }
            }
            finally
            {
                Interlocked.Decrement(ref _loading);

                if (this.LoadingStateChanged != null)
                    this.LoadingStateChanged(Thread.VolatileRead(ref _loading) > 0);
            }
        }

        public void QueueThreadResolveStartAddress(int tid)
        {
            this.QueueThreadResolveStartAddress(tid, this.Dictionary[tid].StartAddressI.ToUInt64());
        }

        public void QueueThreadResolveStartAddress(int tid, ulong startAddress)
        {
            _symbolsWorkQueue.QueueWorkItemTag(
                new ResolveThreadStartAddressDelegate(this.ResolveThreadStartAddress),
                "thread-resolve",
                tid, startAddress
                );
        }

        private string GetThreadBasicStartAddress(ulong startAddress, out SymbolResolveLevel level)
        {
            ulong modBase;
            string fileName = _symbols.GetModuleFromAddress(startAddress, out modBase);

            if (fileName == null)
            {
                level = SymbolResolveLevel.Address;
                return "0x" + startAddress.ToString("x");
            }
            else
            {
                level = SymbolResolveLevel.Module;
                return System.IO.Path.GetFileName(fileName) + "+0x" +
                    (startAddress - modBase).ToString("x");
            }
        }

        protected override void Update()
        {
            // Load symbols if they are not already loaded.
            this.LoadSymbols();

            var threads = Windows.GetProcessThreads(_pid);
            Dictionary<int, ThreadItem> newdictionary = new Dictionary<int, ThreadItem>(this.Dictionary);

            if (threads == null)
                threads = new Dictionary<int, SystemThreadInformation>();

            // look for dead threads
            foreach (int tid in Dictionary.Keys)
            {
                if (!threads.ContainsKey(tid))
                {
                    ThreadItem item = this.Dictionary[tid];

                    if (item.ThreadQueryLimitedHandle != null)
                        item.ThreadQueryLimitedHandle.Dispose();

                    this.OnDictionaryRemoved(item);
                    newdictionary.Remove(tid);
                }
            }

            // Get resolve results.
            _messageQueue.Listen();

            // look for new threads
            foreach (int tid in threads.Keys)
            {
                var t = threads[tid];

                if (!Dictionary.ContainsKey(tid))
                {
                    ThreadItem item = new ThreadItem();

                    item.RunId = this.RunCount;
                    item.Tid = tid;
                    item.ContextSwitches = t.ContextSwitchCount;
                    item.WaitReason = t.WaitReason;

                    try
                    {
                        item.ThreadQueryLimitedHandle = new ThreadHandle(tid, Program.MinThreadQueryRights);

                        try
                        {
                            item.PriorityI = (int)item.ThreadQueryLimitedHandle.GetBasePriorityWin32();
                            item.Priority = item.ThreadQueryLimitedHandle.GetBasePriorityWin32().ToString();
                        }
                        catch
                        { }

                        if (KProcessHacker.Instance != null)
                        {
                            try
                            {
                                item.IsGuiThread = KProcessHacker.Instance.KphGetThreadWin32Thread(item.ThreadQueryLimitedHandle) != 0;
                            }
                            catch
                            { }
                        }

                        if (OSVersion.HasCycleTime)
                        {
                            try
                            {
                                item.Cycles = item.ThreadQueryLimitedHandle.GetCycleTime();
                            }
                            catch
                            { }
                        }
                    }
                    catch
                    { }

                    if (KProcessHacker.Instance != null && item.ThreadQueryLimitedHandle != null)
                    {
                        try
                        {
                            item.StartAddressI =
                                KProcessHacker.Instance.GetThreadStartAddress(item.ThreadQueryLimitedHandle).ToIntPtr();
                        }
                        catch
                        { }
                    }
                    else
                    {
                        try
                        {
                            using (ThreadHandle thandle =
                                new ThreadHandle(tid, ThreadAccess.QueryInformation))
                            {
                                item.StartAddressI = thandle.GetWin32StartAddress();
                            }
                        }
                        catch
                        {
                            item.StartAddressI = t.StartAddress;
                        }
                    }

                    if (_moduleLoadCompletedEvent.Wait(0))
                    {
                        try
                        {
                            item.StartAddress = this.GetThreadBasicStartAddress(
                                item.StartAddressI.ToUInt64(), out item.StartAddressLevel);
                        }
                        catch
                        { }
                    }

                    if (string.IsNullOrEmpty(item.StartAddress))
                    {
                        item.StartAddress = Utils.FormatAddress(item.StartAddressI);
                        item.StartAddressLevel = SymbolResolveLevel.Address;
                    }

                    this.QueueThreadResolveStartAddress(tid, item.StartAddressI.ToUInt64());

                    newdictionary.Add(tid, item);
                    this.OnDictionaryAdded(item);
                }
                // look for modified threads
                else
                {
                    ThreadItem item = Dictionary[tid];
                    ThreadItem newitem = item.Clone() as ThreadItem;

                    newitem.JustResolved = false;
                    newitem.ContextSwitchesDelta = t.ContextSwitchCount - newitem.ContextSwitches;
                    newitem.ContextSwitches = t.ContextSwitchCount;
                    newitem.WaitReason = t.WaitReason;

                    try
                    {
                        newitem.PriorityI = (int)newitem.ThreadQueryLimitedHandle.GetBasePriorityWin32();
                        newitem.Priority = newitem.ThreadQueryLimitedHandle.GetBasePriorityWin32().ToString();
                    }
                    catch
                    { }

                    if (KProcessHacker.Instance != null)
                    {
                        try
                        {
                            newitem.IsGuiThread = KProcessHacker.Instance.KphGetThreadWin32Thread(newitem.ThreadQueryLimitedHandle) != 0;
                        }
                        catch
                        { }
                    }

                    if (OSVersion.HasCycleTime)
                    {
                        try
                        {
                            ulong thisCycles = newitem.ThreadQueryLimitedHandle.GetCycleTime();

                            newitem.CyclesDelta = thisCycles - newitem.Cycles;
                            newitem.Cycles = thisCycles;
                        }
                        catch
                        { }
                    }

                    if (newitem.StartAddressLevel == SymbolResolveLevel.Address)
                    {
                        if (_moduleLoadCompletedEvent.Wait(0))
                        {
                            newitem.StartAddress = this.GetThreadBasicStartAddress(
                                newitem.StartAddressI.ToUInt64(), out newitem.StartAddressLevel);
                        }

                        // If we couldn't resolve it to a module+offset, 
                        // use the StartAddress (instead of the Win32StartAddress)
                        // and queue the resolve again.
                        if (
                            item.StartAddressLevel == SymbolResolveLevel.Address &&
                            item.JustResolved)
                        {
                            if (item.StartAddressI != t.StartAddress)
                            {
                                item.StartAddressI = t.StartAddress;
                                this.QueueThreadResolveStartAddress(tid, item.StartAddressI.ToUInt64());
                            }
                        }
                    }

                    if (
                        newitem.ContextSwitches != item.ContextSwitches ||
                        newitem.ContextSwitchesDelta != item.ContextSwitchesDelta ||
                        newitem.Cycles != item.Cycles ||
                        newitem.CyclesDelta != item.CyclesDelta ||
                        newitem.IsGuiThread != item.IsGuiThread ||
                        newitem.Priority != item.Priority ||
                        newitem.StartAddress != item.StartAddress ||
                        newitem.WaitReason != item.WaitReason ||
                        item.JustResolved
                        )
                    {
                        newdictionary[tid] = newitem;
                        this.OnDictionaryModified(item, newitem);
                    }
                }
            }

            Dictionary = newdictionary;
        }

        public SymbolProvider Symbols
        {
            get { return _symbols; }
        }

        public int Pid
        {
            get { return _pid; }
        }
    }
}
