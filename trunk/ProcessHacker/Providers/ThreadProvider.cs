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
using System.Windows.Forms;
using ProcessHacker.Native;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;
using ProcessHacker.Symbols;

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
        public uint StartAddressI;
        public string StartAddress;
        public Symbols.SymbolResolveLevel StartAddressLevel;
        public KWaitReason WaitReason;
        public bool IsGuiThread;
        public bool JustResolved;

        public ThreadHandle ThreadQueryLimitedHandle;
    }

    public class ThreadProvider : Provider<int, ThreadItem>
    {
        private class ResolveResult
        {
            public int Tid;
            public string Symbol;
            public SymbolResolveLevel ResolveLevel;
        }

        public delegate void LoadingStateChangedDelegate(bool loading);
        private delegate void ResolveThreadStartAddressDelegate(int tid, ulong startAddress);

        public event LoadingStateChangedDelegate LoadingStateChanged;

        private ProcessHandle _processHandle;
        private ProcessAccess _processAccess;
        private SymbolProvider _symbols;
        private int _pid;
        private int _loading = 0;
        private Queue<ResolveResult> _resolveResults = new Queue<ResolveResult>();
        private EventWaitHandle _moduleLoadCompletedEvent = new EventWaitHandle(false, EventResetMode.ManualReset);
        private bool _waitedForLoad = false;

        public ThreadProvider(int pid)
            : base()
        {
            this.Name = this.GetType().Name;
            _pid = pid;

            this.ProviderUpdate += new ProviderUpdateOnce(UpdateOnce);
            this.Disposed += ThreadProvider_Disposed;

            try
            {
                // Try to get a good process handle we can use the same handle for stack walking.
                try
                {
                    _processAccess = ProcessAccess.QueryInformation | ProcessAccess.VmRead;
                    _processHandle = new ProcessHandle(_pid, _processAccess);
                }
                catch
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

                try
                {
                    // Needed (maybe) to display the EULA
                    Win32.SymbolServerSetOptions(SymbolServerOption.Unattended, 0);
                }
                catch (Exception ex)
                {
                    Logging.Log(ex);
                }

                // start loading symbols; avoid the UI blocking on the dbghelp call lock
                WorkQueue.GlobalQueueWorkItem(new Action(() =>
                {
                    _symbols = new SymbolProvider(_processHandle);

                    SymbolProvider.Options = SymbolOptions.DeferredLoads |
                        (Properties.Settings.Default.DbgHelpUndecorate ? SymbolOptions.UndName : 0);

                    if (Properties.Settings.Default.DbgHelpSearchPath != "")
                        _symbols.SearchPath = Properties.Settings.Default.DbgHelpSearchPath;

                    try
                    {
                        if (_pid != 4)
                        {
                            using (var phandle =
                                new ProcessHandle(_pid, Program.MinProcessQueryRights | Program.MinProcessReadMemoryRights))
                            {
                                foreach (var module in phandle.GetModules())
                                {
                                    try
                                    {
                                        _symbols.LoadModule(module.FileName, module.BaseAddress, module.Size);
                                    }
                                    catch (Exception ex)
                                    {
                                        Logging.Log(ex);
                                    }
                                }
                            }
                        }
                        else
                        {
                            // hack for drivers, whose sizes never load properly because of dbghelp.dll's dumb guessing
                            _symbols.PreloadModules = true;

                            // load driver symbols
                            foreach (var module in Windows.GetKernelModules())
                            {
                                try
                                {
                                    _symbols.LoadModule(module.FileName, module.BaseAddress);
                                }
                                catch (Exception ex)
                                {
                                    Logging.Log(ex);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logging.Log(ex);
                    }

                    lock (_moduleLoadCompletedEvent)
                    {
                        if (!_moduleLoadCompletedEvent.SafeWaitHandle.IsClosed)
                            _moduleLoadCompletedEvent.Set();
                    }
                }));
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
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

        private void ThreadProvider_Disposed(IProvider provider)
        {
            if (_symbols != null)
                _symbols.Dispose();
            if (_processHandle != null)
                _processHandle.Dispose();
            _symbols = null;

            lock (_moduleLoadCompletedEvent)
                _moduleLoadCompletedEvent.Close();

            foreach (int tid in this.Dictionary.Keys)
            {
                ThreadItem item = this.Dictionary[tid];

                if (item.ThreadQueryLimitedHandle != null)
                    item.ThreadQueryLimitedHandle.Dispose();
            }
        }

        private void ResolveThreadStartAddress(int tid, ulong startAddress)
        {
            ResolveResult result = new ResolveResult();

            result.Tid = tid;

            if (!_moduleLoadCompletedEvent.SafeWaitHandle.IsClosed)
            {
                try
                {
                    _moduleLoadCompletedEvent.WaitOne();
                }
                catch
                { }
            }

            if (_symbols == null)
                return;

            try
            {
                Interlocked.Increment(ref _loading);

                if (this.LoadingStateChanged != null)
                    this.LoadingStateChanged(Thread.VolatileRead(ref _loading) > 0);

                try
                {
                    result.Symbol = _symbols.GetSymbolFromAddress(startAddress, out result.ResolveLevel);

                    lock (_resolveResults)
                        _resolveResults.Enqueue(result);
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
            this.QueueThreadResolveStartAddress(tid, this.Dictionary[tid].StartAddressI);
        }

        public void QueueThreadResolveStartAddress(int tid, ulong startAddress)
        {
            WorkQueue.GlobalQueueWorkItem(
                new ResolveThreadStartAddressDelegate(this.ResolveThreadStartAddress),
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
                return (new System.IO.FileInfo(fileName)).Name + "+0x" +
                    (startAddress - modBase).ToString("x");
            }
        }

        private void UpdateOnce()
        {
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

                    this.CallDictionaryRemoved(item);
                    newdictionary.Remove(tid);
                }
            }

            lock (_resolveResults)
            {
                while (_resolveResults.Count > 0)
                {
                    var result = _resolveResults.Dequeue();

                    if (result.Symbol != null)
                    {
                        this.Dictionary[result.Tid].StartAddress = result.Symbol;
                        this.Dictionary[result.Tid].StartAddressLevel = result.ResolveLevel;
                        this.Dictionary[result.Tid].JustResolved = true;
                    }
                }
            }

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
                            item.PriorityI = (int)item.ThreadQueryLimitedHandle.GetPriorityLevel();
                            item.Priority = item.ThreadQueryLimitedHandle.GetPriorityLevel().ToString();
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
                            item.StartAddressI = KProcessHacker.Instance.GetThreadStartAddress(item.ThreadQueryLimitedHandle);
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
                                item.StartAddressI = (uint)thandle.GetWin32StartAddress();
                            }
                        }
                        catch
                        { }
                    }

                    if (!_waitedForLoad)
                    {
                        _waitedForLoad = true;

                        try
                        {
                            if (_moduleLoadCompletedEvent.WaitOne(0, false))
                            {
                                item.StartAddress = this.GetThreadBasicStartAddress(
                                    item.StartAddressI, out item.StartAddressLevel);
                            }
                        }
                        catch
                        { }
                    }

                    if (string.IsNullOrEmpty(item.StartAddress))
                    {
                        item.StartAddress = "0x" + item.StartAddressI.ToString("x8");
                        item.StartAddressLevel = SymbolResolveLevel.Address;
                    }

                    this.QueueThreadResolveStartAddress(tid, item.StartAddressI);

                    newdictionary.Add(tid, item);
                    this.CallDictionaryAdded(item);
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
                        newitem.PriorityI = (int)newitem.ThreadQueryLimitedHandle.GetPriorityLevel();
                        newitem.Priority = newitem.ThreadQueryLimitedHandle.GetPriorityLevel().ToString();
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
                        if (_moduleLoadCompletedEvent.WaitOne(0, false))
                        {
                            newitem.StartAddress = this.GetThreadBasicStartAddress(
                                newitem.StartAddressI, out newitem.StartAddressLevel);
                        }

                        // If we couldn't resolve it to a module+offset, 
                        // use the StartAddress (instead of the Win32StartAddress)
                        // and queue the resolve again.
                        if (
                            item.StartAddressLevel == SymbolResolveLevel.Address &&
                            item.JustResolved)
                        {
                            if (item.StartAddressI != (uint)t.StartAddress)
                            {
                                item.StartAddressI = (uint)t.StartAddress;
                                this.QueueThreadResolveStartAddress(tid, item.StartAddressI);
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
                        this.CallDictionaryModified(item, newitem);
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
