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
        public string Priority;
        public uint StartAddressI;
        public string StartAddress;
        public Win32.KWAIT_REASON WaitReason;

        public Win32.ThreadHandle ThreadQueryLimitedHandle;
    }

    public class ThreadProvider : Provider<int, ThreadItem>
    {
        private SymbolProvider _symbols = new SymbolProvider();
        private int _pid;

        public ThreadProvider(int PID)
            : base()
        {
            _pid = PID;

            if (!Win32.ProcessesWithThreads.ContainsKey(_pid))
                Win32.ProcessesWithThreads.Add(_pid, null);

            this.ProviderUpdate += new ProviderUpdateOnce(UpdateOnce);
            this.Killed += new MethodInvoker(ThreadProvider_Killed);

            // start loading symbols
            ThreadPool.QueueUserWorkItem(new WaitCallback(o =>
            {
                try
                {
                    if (_pid != 4)
                    {
                        using (var phandle =
                            new Win32.ProcessHandle(_pid, Program.MinProcessQueryRights |
                                Win32.PROCESS_RIGHTS.PROCESS_VM_READ))
                        {
                            foreach (var module in phandle.GetModules())
                            {
                                try
                                {
                                    _symbols.LoadSymbolsFromLibrary(module.FileName, (uint)module.BaseAddress.ToInt32());
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
                                _symbols.LoadSymbolsFromLibrary(module.FileName, module.BaseAddress);
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

        private void ThreadProvider_Killed()
        {
            if (Win32.ProcessesWithThreads.ContainsKey(_pid))
                Win32.ProcessesWithThreads.Remove(_pid);

            foreach (int tid in this.Dictionary.Keys)
            {
                ThreadItem item = this.Dictionary[tid];

                if (item.ThreadQueryLimitedHandle != null)
                    item.ThreadQueryLimitedHandle.Dispose();
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
                    }
                    catch
                    { }

                    try
                    {
                        using (Win32.ThreadHandle thandle =
                            new Win32.ThreadHandle(tid, Win32.THREAD_RIGHTS.THREAD_QUERY_INFORMATION))
                        {
                            int retLen;

                            Win32.ZwQueryInformationThread(thandle.Handle,
                                Win32.THREAD_INFORMATION_CLASS.ThreadQuerySetWin32StartAddress,
                                out item.StartAddressI, 4, out retLen);
                        }

                        item.StartAddress = _symbols.GetNameFromAddress(item.StartAddressI);
                    }
                    catch { }

                    newdictionary.Add(tid, item);
                    this.CallDictionaryAdded(item);
                }
                // look for modified threads
                else
                {
                    ThreadItem item = Dictionary[tid];
                    ThreadItem newitem = item.Clone() as ThreadItem;

                    newitem.ContextSwitchesDelta = t.ContextSwitchCount - newitem.ContextSwitches;
                    newitem.ContextSwitches = t.ContextSwitchCount;
                    newitem.WaitReason = t.WaitReason;

                    try
                    {
                        newitem.Priority = newitem.ThreadQueryLimitedHandle.GetPriorityLevel().ToString();
                    }
                    catch
                    { }

                    try
                    {
                        SymbolProvider.FoundLevel level;

                        string symName = _symbols.GetNameFromAddress(newitem.StartAddressI, out level);

                        if (level != SymbolProvider.FoundLevel.Address)
                            newitem.StartAddress = symName;
                    }
                    catch { }

                    if (                  
                        newitem.ContextSwitches != item.ContextSwitches || 
                        newitem.ContextSwitchesDelta != item.ContextSwitchesDelta ||
                        newitem.Priority != item.Priority || 
                        newitem.StartAddress != item.StartAddress ||
                        newitem.WaitReason != item.WaitReason
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

        public int PID
        {
            get { return _pid; }
        }
    }
}
