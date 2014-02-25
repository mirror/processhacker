/*
 * Process Hacker - 
 *   network list
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
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using ProcessHacker.Common;
using ProcessHacker.Common.Ui;
using ProcessHacker.UI;

namespace ProcessHacker.Components
{
    public partial class NetworkList : UserControl
    {
        private NetworkProvider _provider;
        private int _runCount = 0;
        private List<ListViewItem> _needsAdd = new List<ListViewItem>();
        private HighlightingContext _highlightingContext;
        private bool _needsSort = false;
        private bool _needsImageKeyReset = false;
        public new event KeyEventHandler KeyDown;
        public new event MouseEventHandler MouseDown;
        public new event MouseEventHandler MouseMove;
        public new event MouseEventHandler MouseUp;
        public new event EventHandler DoubleClick;
        public event EventHandler SelectedIndexChanged;

        public NetworkList()
        {
            InitializeComponent();

            _highlightingContext = new HighlightingContext(listNetwork);
            listNetwork.SetTheme("explorer");
            listNetwork.ListViewItemSorter = new SortedListViewComparer(listNetwork);
            listNetwork.KeyDown += new KeyEventHandler(NetworkList_KeyDown);
            listNetwork.MouseDown += new MouseEventHandler(listNetwork_MouseDown);
            listNetwork.MouseMove += new MouseEventHandler(listNetwork_MouseMove);
            listNetwork.MouseUp += new MouseEventHandler(listNetwork_MouseUp);
            listNetwork.DoubleClick += new EventHandler(listNetwork_DoubleClick);
            listNetwork.SelectedIndexChanged += new System.EventHandler(listNetwork_SelectedIndexChanged);
        }

        private void listNetwork_DoubleClick(object sender, EventArgs e)
        {
            if (this.DoubleClick != null)
                this.DoubleClick(sender, e);
        }  

        private void listNetwork_MouseUp(object sender, MouseEventArgs e)
        {
            if (this.MouseUp != null)
                this.MouseUp(sender, e);
        }

        private void listNetwork_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.MouseDown != null)
                this.MouseDown(sender, e);
        }

        private void listNetwork_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.MouseMove != null)
                this.MouseMove(sender, e);

            ListViewItem litem = listNetwork.GetItemAt(e.X, e.Y);

            if (litem != null)
            {
                NetworkItem item = (NetworkItem)litem.Tag;
                var tree = Program.HackerWindow.ProcessTree;

                if (tree.Model.Nodes.ContainsKey(item.Connection.Pid))
                    litem.ToolTipText = tree.Model.Nodes[item.Connection.Pid].GetTooltipText(tree.TooltipProvider);
            }
        }

        private void listNetwork_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (this.SelectedIndexChanged != null)
                this.SelectedIndexChanged(sender, e);
        }

        private void NetworkList_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.KeyDown != null)
                this.KeyDown(sender, e);
        }

        #region Properties

        public new bool DoubleBuffered
        {
            get
            {
                return (bool)typeof(ListView).GetProperty("DoubleBuffered",
                    BindingFlags.NonPublic | BindingFlags.Instance).GetValue(listNetwork, null);
            }
            set
            {
                typeof(ListView).GetProperty("DoubleBuffered",
                    BindingFlags.NonPublic | BindingFlags.Instance).SetValue(listNetwork, value, null);
            }
        }

        public override bool Focused
        {
            get
            {
                return listNetwork.Focused;
            }
        }

        public override ContextMenu ContextMenu
        {
            get { return listNetwork.ContextMenu; }
            set { listNetwork.ContextMenu = value; }
        }

        public override ContextMenuStrip ContextMenuStrip
        {
            get { return listNetwork.ContextMenuStrip; }
            set { listNetwork.ContextMenuStrip = value; }
        }

        public ListView List
        {
            get { return listNetwork; }
        }

        public NetworkProvider Provider
        {
            get { return _provider; }
            set
            {
                if (_provider != null)
                {
                    _provider.DictionaryAdded -= provider_DictionaryAdded;
                    _provider.DictionaryModified -= provider_DictionaryModified;
                    _provider.DictionaryRemoved -= provider_DictionaryRemoved;
                    _provider.Updated -= provider_Updated;
                    Program.ProcessProvider.ProcessQueryReceived -= ProcessProvider_FileProcessingReceived;
                }

                _provider = value;
                
                listNetwork.Items.Clear();

                if (_provider != null)
                {                       
                    Program.ProcessProvider.ProcessQueryReceived += ProcessProvider_FileProcessingReceived;

                    foreach (NetworkItem item in _provider.Dictionary.Values)
                    {
                        provider_DictionaryAdded(item);
                    }

                    _provider.DictionaryAdded += new NetworkProvider.ProviderDictionaryAdded(provider_DictionaryAdded);
                    _provider.DictionaryModified += new NetworkProvider.ProviderDictionaryModified(provider_DictionaryModified);
                    _provider.DictionaryRemoved += new NetworkProvider.ProviderDictionaryRemoved(provider_DictionaryRemoved);
                    _provider.Updated += new NetworkProvider.ProviderUpdateOnce(provider_Updated);
                }
            }
        }

        #endregion

        #region Interfacing

        public void BeginUpdate()
        {
            listNetwork.BeginUpdate();
        }

        public void EndUpdate()
        {
            listNetwork.EndUpdate();
        }

        public ListView.ListViewItemCollection Items
        {
            get { return listNetwork.Items; }
        }

        public ListView.SelectedListViewItemCollection SelectedItems
        {
            get { return listNetwork.SelectedItems; }
        }

        #endregion

        private void ProcessProvider_FileProcessingReceived(int stage, int pid)
        {
            if (stage == 0x1)
            {
                // We just got the icon for the process.
                this.BeginInvoke(new Action<int>(this.RefreshIcons), pid);
                _needsImageKeyReset = true;
            }
        }

        /// <summary>
        /// Invalidates the cached icon indicies used in the list view.
        /// </summary>
        /// <remarks>
        /// When the image key of a ListViewItem is set, it looks up 
        /// the index corresponding to the image key and uses that 
        /// instead of the image key. When an image is removed from 
        /// the image list, the indicies will be wrong.
        /// </remarks>
        private void ResetImageKeys()
        {
            lock (listNetwork)
            {
                foreach (ListViewItem lvItem in listNetwork.Items)
                {
                    string t = lvItem.ImageKey;

                    lvItem.ImageKey = "";
                    lvItem.ImageKey = t;
                }
            }
        }

        private void provider_Updated()
        {
            lock (_needsAdd)
            {
                if (_needsAdd.Count > 0)
                {
                    this.BeginInvoke(new MethodInvoker(() =>
                    {
                        lock (listNetwork)
                        {
                            lock (_needsAdd)
                            {
                                listNetwork.Items.AddRange(_needsAdd.ToArray());
                                _needsAdd.Clear();
                            }
                        }
                    }));
                }
            }

            _highlightingContext.Tick();

            if (_needsSort)
            {
                this.BeginInvoke(new MethodInvoker(() =>
                    {
                        if (_needsSort)
                        {
                            listNetwork.Sort();
                            _needsSort = false;
                        }
                    }));
            }

            if (_needsImageKeyReset)
            {
                this.BeginInvoke(new MethodInvoker(() =>
                    {
                        if (_needsImageKeyReset)
                        {
                            this.ResetImageKeys();
                            _needsImageKeyReset = false;
                        }
                    }));
            }

            _runCount++;
        }

        public void RefreshIcons()
        {
            this.RefreshIcons(0);
        }

        public void RefreshIcons(int searchPid)
        {
            lock (listNetwork)
            {
                foreach (ListViewItem item in listNetwork.Items)
                {
                    int pid = ((NetworkItem)item.Tag).Connection.Pid;

                    if (searchPid != 0)
                        if (pid != searchPid)
                            continue;
                    // If the item already has an icon, continue searching.
                    if (item.ImageKey != "generic_process")
                        continue;
                    // If the PID is System Idle Process, continue searching.
                    if (pid < 4)
                        continue;

                    if (Program.ProcessProvider.Dictionary.ContainsKey(pid) &&
                        Program.ProcessProvider.Dictionary[pid].Icon != null)
                    {
                        if (!imageList.Images.ContainsKey(pid.ToString()))
                            imageList.Images.Add(pid.ToString(),
                                Program.ProcessProvider.Dictionary[pid].Icon);

                        item.ImageKey = pid.ToString();
                    }
                }
            }
        }

        private void FillNetworkItemAddresses(ListViewItem litem, NetworkItem item)
        {
            if (item.Connection.Local != null && !item.Connection.Local.IsEmpty())
            {
                string addressString = item.Connection.Local.Address.ToString();

                if (item.LocalString != null && item.LocalString != addressString)
                {
                    litem.SubItems[1].Text = item.LocalString + " (" + addressString + ")";
                }
                else
                {
                    litem.SubItems[1].Text = addressString;
                }
            }

            if (item.Connection.Remote != null && !item.Connection.Remote.IsEmpty())
            {
                string addressString = item.Connection.Remote.Address.ToString();

                if (item.RemoteString != null && item.RemoteString != addressString)
                    litem.SubItems[3].Text = item.RemoteString + " (" + addressString + ")";
                else
                    litem.SubItems[3].Text = addressString;
            }
        }

        private void provider_DictionaryAdded(NetworkItem item)
        {
            HighlightedListViewItem litem = new HighlightedListViewItem(_highlightingContext, (int)item.Tag > 0 && _runCount > 0);

            litem.Name = item.Id;
            litem.Tag = item;

            Icon icon = null;

            if (Program.ProcessProvider.Dictionary.ContainsKey(item.Connection.Pid))
            {
                lock (listNetwork)
                {
                    if (imageList.Images.ContainsKey(item.Connection.Pid.ToString()))
                        imageList.Images.RemoveByKey(item.Connection.Pid.ToString());

                    icon = Program.ProcessProvider.Dictionary[item.Connection.Pid].Icon;
                }
            }

            if (icon != null)
            {
                lock (listNetwork)
                    imageList.Images.Add(item.Connection.Pid.ToString(), icon);

                litem.ImageKey = item.Connection.Pid.ToString();
            }
            else
            {
                litem.ImageKey = "generic_process";
            }

            if (item.Connection.Pid == 0)
            {
                litem.Text = "Waiting Connections";
            }
            else if (Program.ProcessProvider.Dictionary.ContainsKey(item.Connection.Pid))
            {
                litem.Text = Program.ProcessProvider.Dictionary[item.Connection.Pid].Name + 
                    " (" + item.Connection.Pid.ToString() + ")";
            }
            else
            {
                litem.Text = "Unknown Process (" + item.Connection.Pid.ToString() + ")";
            }

            if (item.Connection.Local != null && !item.Connection.Local.IsEmpty())
            {
                litem.SubItems.Add(new ListViewItem.ListViewSubItem(litem, item.Connection.Local.ToString()));
                litem.SubItems.Add(new ListViewItem.ListViewSubItem(litem, item.Connection.Local.Port.ToString()));
            }
            else
            {
                litem.SubItems.Add(new ListViewItem.ListViewSubItem(litem, ""));
                litem.SubItems.Add(new ListViewItem.ListViewSubItem(litem, ""));
            }

            if (item.Connection.Remote != null && !item.Connection.Remote.IsEmpty())
            {
                litem.SubItems.Add(new ListViewItem.ListViewSubItem(litem, item.Connection.Remote.ToString()));
                litem.SubItems.Add(new ListViewItem.ListViewSubItem(litem, item.Connection.Remote.Port.ToString()));
            }
            else
            {
                litem.SubItems.Add(new ListViewItem.ListViewSubItem(litem, ""));
                litem.SubItems.Add(new ListViewItem.ListViewSubItem(litem, ""));
            }

            this.FillNetworkItemAddresses(litem, item);

            litem.SubItems.Add(new ListViewItem.ListViewSubItem(litem, item.Connection.Protocol.ToString().ToUpper()));
            litem.SubItems.Add(new ListViewItem.ListViewSubItem(litem, item.Connection.State != 0 ? item.Connection.State.ToString() : ""));

            lock (_needsAdd)
                _needsAdd.Add(litem);
            _needsImageKeyReset = true;
        }

        private void provider_DictionaryModified(NetworkItem oldItem, NetworkItem newItem)
        {
            this.BeginInvoke(new MethodInvoker(() =>
                {
                    lock (listNetwork)
                    {
                        ListViewItem litem = listNetwork.Items[newItem.Id];

                        if (litem == null)
                            return;

                        this.FillNetworkItemAddresses(litem, newItem);

                        litem.SubItems[6].Text = newItem.Connection.State != 0 ? newItem.Connection.State.ToString() : "";
                        _needsSort = true;
                    }
                }));
        }

        private void provider_DictionaryRemoved(NetworkItem item)
        {
            this.BeginInvoke(new MethodInvoker(() =>
                {
                    lock (listNetwork)
                    {
                        if (!listNetwork.Items.ContainsKey(item.Id))
                            return;

                        ListViewItem litem = listNetwork.Items[item.Id];
                        bool imageStillUsed = false;

                        if (litem.ImageKey == "generic_process")
                        {
                            imageStillUsed = true;
                        }
                        else
                        {
                            foreach (ListViewItem lvItem in listNetwork.Items)
                            {
                                if (lvItem != litem && lvItem.ImageKey == item.Connection.Pid.ToString())
                                {
                                    imageStillUsed = true;
                                    break;
                                }
                            }
                        }

                        if (!imageStillUsed)
                        {
                            imageList.Images.RemoveByKey(item.Connection.Pid.ToString());

                            // Set the item's icon to generic_process, otherwise we are going to 
                            // get a blank space for the icon.
                            litem.ImageKey = "generic_process";
                            // Reset all the image keys (by now most items' icons have screwed up).
                            this.ResetImageKeys();
                        }

                        litem.Remove();
                    }
                }));
        }
    }
}
