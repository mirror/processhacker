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
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace ProcessHacker
{
    public class ThreadItem : ICloneable
    {
        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public int TID;

        public long ContextSwitches;
        public long ContextSwitchesDelta;
        public ulong Cycles;
        public ulong CyclesDelta;
        public string Priority;
        public uint StartAddressI;
        public string StartAddress;
        public Win32.KWAIT_REASON WaitReason;
        public bool IsGuiThread;
        public bool JustResolved;

        public Win32.ThreadHandle ThreadQueryLimitedHandle;
    }

    public class ThreadProvider : Provider<int, ThreadItem>
    {
        public delegate void LoadingStateChangedDelegate(bool loading);
        private delegate void ResolveThreadStartAddressDelegate(int tid, long startAddress);

        public event LoadingStateChangedDelegate LoadingStateChanged;

        private Win32.ProcessHandle _processHandle;
        private Symbols _symbols;
        private int _pid;
        private int _loading = 0;
        private Queue<KeyValuePair<int, string>> _resolveResults = new Queue<KeyValuePair<int,string>>();

        public ThreadProvider(int PID)
            : base()
        {
            _pid = PID;

            if (!Win32.ProcessesWithThreads.ContainsKey(_pid))
                Win32.ProcessesWithThreads.Add(_pid, null);

            this.ProviderUpdate += new ProviderUpdateOnce(UpdateOnce);
            this.Killed += new MethodInvoker(ThreadProvider_Killed);

            try
            {
                _processHandle = new Win32.ProcessHandle(_pid, Program.MinProcessQueryRights);
                _symbols = new Symbols(_processHandle);

                Symbols.Options = Win32.SYMBOL_OPTIONS.DeferredLoads | 
                    (Properties.Settings.Default.DbgHelpUndecorate ? Win32.SYMBOL_OPTIONS.UndName : 0);

                try
                {
                    // Needed to display the EULA
                    Win32.SymbolServerSetOptions(Win32.SYMBOL_SERVER_OPTION.Unattended, 0);
                    Win32.SymbolServerSetOptions(Win32.SYMBOL_SERVER_OPTION.ParentWin, Program.HackerWindow.Handle.ToInt64());
                }
                catch
                { }

                if (Properties.Settings.Default.DbgHelpSearchPath != "")
                    _symbols.SearchPath = Properties.Settings.Default.DbgHelpSearchPath;

                // start loading symbols
                ThreadPool.QueueUserWorkItem(new WaitCallback(o =>
                {
                    try
                    {
                        if (_pid != 4)
                        {
                            using (var phandle =
                                new Win32.ProcessHandle(_pid, Program.MinProcessQueryRights | Program.MinProcessReadMemoryRights))
                            {
                                foreach (var module in phandle.GetModules())
                                {
                                    try
                                    {
                                        _symbols.LoadModule(module.FileName, module.BaseAddress.ToInt32(), module.Size);
                                    }
                                    catch
                                    { }
                                }
                            }
                        }
                        else
                        {
                            // load driver symbols
                            foreach (var module in Win32.EnumKernelModules())
                            {
                                try
                                {
                                    _symbols.LoadModule(module.FileName, module.BaseAddress);
                                }
                                catch
                                { }
                            }
                        }
                    }
                    catch
                    { }
                }));
            }
            catch
            { }
        }

        private void ThreadProvider_Killed()
        {
            if (_symbols != null)
                _symbols.Dispose();
            if (_processHandle != null)
                _processHandle.Dispose();

            if (Win32.ProcessesWithThreads.ContainsKey(_pid))
                Win32.ProcessesWithThreads.Remove(_pid);

            foreach (int tid in this.Dictionary.Keys)
            {
                ThreadItem item = this.Dictionary[tid];

                if (item.ThreadQueryLimitedHandle != null)
                    item.ThreadQueryLimitedHandle.Dispose();
            }
        }

        private void ResolveThreadStartAddress(int tid, long startAddress)
        {
            string name = null;

            try
            {
                _loading++;

                if (this.LoadingStateChanged != null)
                    this.LoadingStateChanged(_loading > 0);

                try
                {
                    name = _symbols.GetSymbolFromAddress(startAddress);

                    lock (_resolveResults)
                        _resolveResults.Enqueue(new KeyValuePair<int, string>(tid, name));
                }
                catch
                { }
            }
            finally
            {
                _loading--;

                if (this.LoadingStateChanged != null)
                    this.LoadingStateChanged(_loading > 0);
            }
        }

        private void UpdateOnce()
        {
            Dictionary<int, Win32.SYSTEM_THREAD_INFORMATION> threads =
                Program.HackerWindow.ProcessProvider.Dictionary[_pid].Threads;
            Dictionary<int, ThreadItem> newdictionary = new Dictionary<int, ThreadItem>(this.Dictionary);

            if (threads == null)
                threads = new Dictionary<int, Win32.SYSTEM_THREAD_INFORMATION>();

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

                    if (result.Value != null)
                    {
                        this.Dictionary[result.Key].StartAddress = result.Value;
                        this.Dictionary[result.Key].JustResolved = true;
                    }
                }
            }

            // look for new threads
            foreach (int tid in threads.Keys)
            {
                Win32.SYSTEM_THREAD_INFORMATION t = threads[tid];

                if (!Dictionary.ContainsKey(tid))
                {
                    ThreadItem item = new ThreadItem();

                    item.TID = tid;
                    item.ContextSwitches = t.ContextSwitchCount;
                    item.WaitReason = t.WaitReason;

                    try
                    {
                        item.ThreadQueryLimitedHandle = new Win32.ThreadHandle(tid, Program.MinThreadQueryRights);

                        try
                        {
                            item.Priority = item.ThreadQueryLimitedHandle.GetPriorityLevel().ToString();
                        }
                        catch
                        { }

                        if (Program.KPH != null)
                        {
                            try
                            {
                                item.IsGuiThread = Program.KPH.KphGetThreadWin32Thread(item.ThreadQueryLimitedHandle) != 0;
                            }
                            catch
                            { }
                        }

                        if (Program.WindowsVersion != WindowsVersion.XP)
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

                    if (Program.KPH != null && item.ThreadQueryLimitedHandle != null)
                    {
                        try
                        {
                            item.StartAddressI = Program.KPH.GetThreadWin32StartAddress(item.ThreadQueryLimitedHandle);
                        }
                        catch
                        { }
                    }
                    else
                    {
                        try
                        {
                            using (Win32.ThreadHandle thandle =
                                new Win32.ThreadHandle(tid, Win32.THREAD_RIGHTS.THREAD_QUERY_INFORMATION))
                            {
                                int retLen;

                                Win32.ZwQueryInformationThread(thandle,
                                    Win32.THREAD_INFORMATION_CLASS.ThreadQuerySetWin32StartAddress,
                                    out item.StartAddressI, 4, out retLen);
                            }
                        }
                        catch
                        { }
                    }

                    try
                    {
                        long modBase;
                        string fileName = _symbols.GetModuleFromAddress(item.StartAddressI, out modBase);

                        if (fileName == null)
                            item.StartAddress = "0x" + item.StartAddressI.ToString("x");
                        else
                            item.StartAddress = (new System.IO.FileInfo(fileName)).Name + "+0x" + 
                                (item.StartAddressI - modBase).ToString("x");
                    }
                    catch
                    { }

                    (new ResolveThreadStartAddressDelegate(this.ResolveThreadStartAddress)).BeginInvoke(
                        tid, item.StartAddressI, r => { }, null);

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
                        newitem.Priority = newitem.ThreadQueryLimitedHandle.GetPriorityLevel().ToString();
                    }
                    catch
                    { }

                    if (Program.KPH != null)
                    {
                        try
                        {
                            newitem.IsGuiThread = Program.KPH.KphGetThreadWin32Thread(newitem.ThreadQueryLimitedHandle) != 0;
                        }
                        catch
                        { }
                    }

                    if (Program.WindowsVersion != WindowsVersion.XP)
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

        public Symbols Symbols
        {
            get { return _symbols; }
        }

        public int PID
        {
            get { return _pid; }
        }
    }
}
