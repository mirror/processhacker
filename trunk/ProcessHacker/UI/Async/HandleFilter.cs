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
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Collections;

namespace ProcessHacker.FormHelper
{
    public sealed class HandleFilter : AsyncOperation
    {
        private const int BufferSize = 100;

        public delegate void MatchListViewEvent(List<ListViewItem> item);
        public delegate void MatchProgressEvent(int currentValue,int count);
        public event MatchListViewEvent MatchListView;
        public event MatchProgressEvent MatchProgress;
        private string strFilter;
        private List<ListViewItem> listViewItemContainer = new List<ListViewItem>(BufferSize);
        private Dictionary<int, bool> isCurrentSessionIdCache = new Dictionary<int, bool>();

        public HandleFilter(ISynchronizeInvoke isi, string strFilter)
            : base(isi)
        { 
            this.strFilter=strFilter;  
        }       

        protected override void DoWork()
        {
            DoFilter(strFilter);
            if (CancelRequested)
            {
                AcknowledgeCancel();
            }
        }

        private void DoFilter(string strFilter)
        {
            // Stop if cancel
            if (!CancelRequested)
            {
                Win32.SYSTEM_HANDLE_INFORMATION[] handles = null;
                handles = Win32.EnumHandles();
                Dictionary<int, Win32.ProcessHandle> processHandles = new Dictionary<int, Win32.ProcessHandle>();

                for (int i = 0; i < handles.Length; i++)
                {
                    // Check for cancellation here too,
                    // otherwise the user might have to wait for much time                    
                    if (CancelRequested) return;

                    if (i % 20 == 0)
                        OnMatchProgress(i, handles.Length);

                    Win32.SYSTEM_HANDLE_INFORMATION handle = handles[i];

                    CompareHandlerBestNameWithFilterString(processHandles, handle, strFilter);
                    // test Exception 
                    //if (i > 2000) throw new Exception("test");
                }
                OnMatchListView(null);
                foreach (Win32.ProcessHandle phandle in processHandles.Values)
                    phandle.Dispose();
            }
        }

        private void CompareHandlerBestNameWithFilterString(Dictionary<int, Win32.ProcessHandle> processHandles, Win32.SYSTEM_HANDLE_INFORMATION currhandle, string strFilter)
        {
            try
            {
                if (Program.KPH == null)
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
                        new Win32.ProcessHandle(currhandle.ProcessId, Win32.PROCESS_RIGHTS.PROCESS_DUP_HANDLE));

                Win32.ObjectInformation info = Win32.GetHandleInfo(processHandles[currhandle.ProcessId], currhandle);

                if (!info.BestName.ToLower().Contains(strFilter.ToLower()))
                    return;

                CallMatchListView(currhandle, info);
            }
            catch
            {
                return;
            }
        }

        private void CallMatchListView(Win32.SYSTEM_HANDLE_INFORMATION handle, Win32.ObjectInformation info)
        {
            ListViewItem item = new ListViewItem();
            item.Name = handle.ProcessId.ToString() + " " + handle.Handle.ToString();
            item.Text = Program.ProcessProvider.Dictionary[handle.ProcessId].Name +
                " (" + handle.ProcessId.ToString() + ")";
            item.Tag = handle.ProcessId;
            item.SubItems.Add(new ListViewItem.ListViewSubItem(item, info.TypeName));
            item.SubItems.Add(new ListViewItem.ListViewSubItem(item, info.BestName));
            item.SubItems.Add(new ListViewItem.ListViewSubItem(item, "0x" + handle.Handle.ToString("x")));
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



