/*
 * Process Hacker - 
 *   handle filter
 * 
 * Copyright (C) 2008 Dean
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
using System.ComponentModel;
using System.Windows.Forms;
using ProcessHacker.Common;
using ProcessHacker.Native;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Debugging;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;

namespace ProcessHacker.FormHelper
{
    public sealed class HandleFilter : AsyncOperation
    {
        private const int BufferSize = 50;

        public delegate void MatchListViewEvent(List<ListViewItem> item);
        public delegate void MatchProgressEvent(int currentValue, int count);
        public event MatchListViewEvent MatchListView;
        public event MatchProgressEvent MatchProgress;
        private string strFilter;
        private string strFilterLower;
        private IntPtr intPtrFilter;
        private List<ListViewItem> listViewItemContainer = new List<ListViewItem>(BufferSize);
        private Dictionary<int, bool> isCurrentSessionIdCache = new Dictionary<int, bool>();

        public HandleFilter(ISynchronizeInvoke isi, string strFilter)
            : base(isi)
        {
            this.strFilter = strFilter;
            strFilterLower = strFilter.ToLowerInvariant();

            try
            {
                ulong address = (ulong)BaseConverter.ToNumberParse(strFilter);

                intPtrFilter = address.ToIntPtr();
            }
            catch
            { }
        }

        protected override void DoWork()
        {
            DoFilter();
            if (CancelRequested)
            {
                AcknowledgeCancel();
            }
        }

        private void DoFilter()
        {
            // Stop if cancel
            if (!CancelRequested)
            {
                var handles = Windows.GetHandles();
                Dictionary<int, ProcessHandle> processHandles = new Dictionary<int, ProcessHandle>();

                // Find handles
                for (int i = 0; i < handles.Length; i++)
                {
                    // Check for cancellation here too,
                    // otherwise the user might have to wait for much time                    
                    if (CancelRequested) return;

                    if (i % 20 == 0)
                        OnMatchProgress(i, handles.Length);

                    var handle = handles[i];

                    CompareHandleBestNameWithFilter(processHandles, handle);
                    // test Exception 
                    //if (i > 2000) throw new Exception("test");
                }

                foreach (ProcessHandle phandle in processHandles.Values)
                    phandle.Dispose();

                // Find DLLs and mapped files
                var processes = Windows.GetProcesses();

                foreach (var process in processes)
                {
                    try
                    {
                        // Modules
                        using (var phandle = new ProcessHandle(process.Key,
                            Program.MinProcessQueryRights | Program.MinProcessReadMemoryRights))
                        {
                            phandle.EnumModules((module) =>
                            {
                                if (module.FileName.ToLowerInvariant().Contains(strFilterLower))
                                    this.CallDllMatchListView(process.Key, module);
                                return true;
                            });
                        }

                        // Memory
                        using (var phandle = new ProcessHandle(process.Key,
                            ProcessAccess.QueryInformation | Program.MinProcessReadMemoryRights))
                        {
                            phandle.EnumMemory((region) =>
                            {
                                if (region.Type != MemoryType.Mapped)
                                    return true;

                                string name = phandle.GetMappedFileName(region.BaseAddress);

                                if (name != null && name.ToLowerInvariant().Contains(strFilterLower))
                                    this.CallMappedFileMatchListView(process.Key, region.BaseAddress, name);

                                return true;
                            });
                        }

                        // WOW64 Modules
                        if (OSVersion.Architecture == OSArch.Amd64)
                        {
                            using (DebugBuffer buffer = new DebugBuffer())
                            {
                                buffer.Query(
                                    process.Key,
                                    RtlQueryProcessDebugFlags.Modules32 |
                                    RtlQueryProcessDebugFlags.NonInvasive
                                    );

                                buffer.EnumModules((module) =>
                                    {
                                        if (module.FileName.ToLowerInvariant().Contains(strFilterLower))
                                            this.CallDllMatchListView(process.Key, module);
                                        return true;
                                    });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logging.Log(ex);
                    }
                }

                OnMatchListView(null);
            }
        }

        private void CompareHandleBestNameWithFilter(
            Dictionary<int, ProcessHandle> processHandles,
            SystemHandleEntry currhandle)
        {
            try
            {
                // Don't get handles from processes in other session 
                // if we don't have KPH to reduce freezes.
                if (KProcessHacker.Instance == null)
                {
                    try
                    {
                        if (isCurrentSessionIdCache.ContainsKey(currhandle.ProcessId))
                        {
                            if (!isCurrentSessionIdCache[currhandle.ProcessId])
                                return;
                        }
                        else
                        {
                            bool isCurrentSessionId = Win32.GetProcessSessionId(currhandle.ProcessId) == Program.CurrentSessionId;

                            isCurrentSessionIdCache.Add(currhandle.ProcessId, isCurrentSessionId);

                            if (!isCurrentSessionId)
                                return;
                        }
                    }
                    catch
                    {
                        return;
                    }
                }

                if (!processHandles.ContainsKey(currhandle.ProcessId))
                    processHandles.Add(currhandle.ProcessId,
                        new ProcessHandle(currhandle.ProcessId, Program.MinProcessGetHandleInformationRights));

                var info = currhandle.GetHandleInfo(processHandles[currhandle.ProcessId]);

                if (intPtrFilter == IntPtr.Zero && string.IsNullOrEmpty(info.BestName))
                    return;

                if (
                    (info.BestName != null && info.BestName.ToLowerInvariant().Contains(strFilterLower)) ||
                    (intPtrFilter != IntPtr.Zero && currhandle.Object == intPtrFilter)
                    )
                {
                    CallHandleMatchListView(currhandle, info);
                }
            }
            catch
            {
                return;
            }
        }

        private void CallHandleMatchListView(SystemHandleEntry handle, ObjectInformation info)
        {
            ListViewItem item = new ListViewItem();
            item.Name = handle.ProcessId.ToString() + " " + handle.Handle.ToString();
            item.Text = Program.ProcessProvider.Dictionary[handle.ProcessId].Name +
                " (" + handle.ProcessId.ToString() + ")";
            item.Tag = handle;
            item.SubItems.Add(new ListViewItem.ListViewSubItem(item, info.TypeName));
            item.SubItems.Add(new ListViewItem.ListViewSubItem(item, info.BestName));
            item.SubItems.Add(new ListViewItem.ListViewSubItem(item, "0x" + handle.Handle.ToString("x")));
            OnMatchListView(item);
        }

        private void CallDllMatchListView(int pid, ILoadedModule module)
        {
            ListViewItem item = new ListViewItem();
            item.Name = pid.ToString() + " " + module.BaseAddress.ToString();
            item.Text = Program.ProcessProvider.Dictionary[pid].Name +
                " (" + pid.ToString() + ")";
            item.Tag = pid;
            item.SubItems.Add(new ListViewItem.ListViewSubItem(item, "DLL"));
            item.SubItems.Add(new ListViewItem.ListViewSubItem(item, module.FileName));
            item.SubItems.Add(new ListViewItem.ListViewSubItem(item, Utils.FormatAddress(module.BaseAddress)));
            OnMatchListView(item);
        }

        private void CallMappedFileMatchListView(int pid, IntPtr address, string fileName)
        {
            ListViewItem item = new ListViewItem();
            item.Name = pid.ToString() + " " + address.ToString();
            item.Text = Program.ProcessProvider.Dictionary[pid].Name +
                " (" + pid.ToString() + ")";
            item.Tag = pid;
            item.SubItems.Add(new ListViewItem.ListViewSubItem(item, "Mapped File"));
            item.SubItems.Add(new ListViewItem.ListViewSubItem(item, fileName));
            item.SubItems.Add(new ListViewItem.ListViewSubItem(item, Utils.FormatAddress(address)));
            OnMatchListView(item);
        }

        private void OnMatchListView(ListViewItem item)
        {
            if (item == null)
            {
                if (listViewItemContainer.Count > 0)
                    FireAsync(MatchListView, listViewItemContainer);
                return;
            }

            listViewItemContainer.Add(item);

            if (listViewItemContainer.Count >= BufferSize)
            {
                List<ListViewItem> items = listViewItemContainer;

                FireAsync(MatchListView, items);
                listViewItemContainer = new List<ListViewItem>(BufferSize);
            }
        }

        private void OnMatchProgress(int currentValue, int allValue)
        {
            FireAsync(MatchProgress, currentValue, allValue);
        }
    }
}
