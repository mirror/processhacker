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
using System.Reflection;
using System.Windows.Forms;
using ProcessHacker.Native;
using ProcessHacker.UI;

namespace ProcessHacker.Components
{
    public partial class NetworkList : UserControl
    {
        private object _imageListLock = new object();
        private NetworkProvider _provider;
        private HighlightingContext _highlightingContext;
        private bool _needsSort = false;
        private bool _needsImageKeyReset = false;
        public new event KeyEventHandler KeyDown;
        public new event MouseEventHandler MouseDown;
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
                    Program.ProcessProvider.FileProcessingReceived += ProcessProvider_FileProcessingReceived;
                }

                _provider = value;
                
                listNetwork.Items.Clear();

                if (_provider != null)
                {
                    foreach (NetworkConnection item in _provider.Dictionary.Values)
                    {
                        provider_DictionaryAdded(item);
                    }

                    _provider.UseInvoke = true;
                    _provider.Invoke = new NetworkProvider.ProviderInvokeMethod(this.BeginInvoke);
                    _provider.DictionaryAdded += new NetworkProvider.ProviderDictionaryAdded(provider_DictionaryAdded);
                    _provider.DictionaryModified += new NetworkProvider.ProviderDictionaryModified(provider_DictionaryModified);
                    _provider.DictionaryRemoved += new NetworkProvider.ProviderDictionaryRemoved(provider_DictionaryRemoved);
                    _provider.Updated += new NetworkProvider.ProviderUpdateOnce(provider_Updated);
                    Program.ProcessProvider.FileProcessingReceived -= ProcessProvider_FileProcessingReceived;
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
            lock (_imageListLock)
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
            _highlightingContext.Tick();

            if (_needsSort)
            {
                listNetwork.Sort();
                _needsSort = false;
            }

            if (_needsImageKeyReset)
            {
                this.ResetImageKeys();
                _needsImageKeyReset = false;
            }
        }

        public void RefreshIcons()
        {
            this.RefreshIcons(0);
        }

        public void RefreshIcons(int searchPid)
        {
            lock (_imageListLock)
            {
                lock (listNetwork)
                {
                    foreach (ListViewItem item in listNetwork.Items)
                    {
                        int pid = (int)item.Tag;

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
                            Program.ProcessProvider.Dictionary[pid].Icon != null && 
                            !imageList.Images.ContainsKey(pid.ToString()))
                        {
                            imageList.Images.Add(pid.ToString(),
                                Program.ProcessProvider.Dictionary[pid].Icon);
                            item.ImageKey = pid.ToString();
                        }
                    }
                }
            }
        }

        private void provider_DictionaryAdded(NetworkConnection item)
        {
            HighlightedListViewItem litem = new HighlightedListViewItem(_highlightingContext);

            litem.Name = item.ID;
            litem.Tag = item.PID;

            if (Program.ProcessProvider.Dictionary.ContainsKey(item.PID))
            {
                lock (_imageListLock)
                {
                    if (imageList.Images.ContainsKey(item.PID.ToString()))
                        imageList.Images.RemoveByKey(item.PID.ToString());

                    var icon = Program.ProcessProvider.Dictionary[item.PID].Icon;

                    if (icon != null)
                    {
                        imageList.Images.Add(item.PID.ToString(), icon);
                        litem.ImageKey = item.PID.ToString();
                    }
                    else
                    {
                        litem.ImageKey = "generic_process";
                    }
                }
            }

            if (item.PID == 0)
                litem.Text = "Unknown Process";
            else if (Program.ProcessProvider.Dictionary.ContainsKey(item.PID))
                litem.Text = Program.ProcessProvider.Dictionary[item.PID].Name;
            else
                litem.Text = "(" + item.PID.ToString() + ")";

            if (item.Local != null && item.Local.ToString() != "0.0.0.0:0")
                litem.SubItems.Add(new ListViewItem.ListViewSubItem(litem, item.Local.ToString()));
            else
                litem.SubItems.Add(new ListViewItem.ListViewSubItem(litem, ""));

            if (item.Remote != null && item.Remote.ToString() != "0.0.0.0:0")
                litem.SubItems.Add(new ListViewItem.ListViewSubItem(litem, item.Remote.ToString()));
            else
                litem.SubItems.Add(new ListViewItem.ListViewSubItem(litem, ""));

            litem.SubItems.Add(new ListViewItem.ListViewSubItem(litem, item.Protocol.ToString().ToUpper()));
            litem.SubItems.Add(new ListViewItem.ListViewSubItem(litem, item.State != 0 ? item.State.ToString() : ""));

            listNetwork.Items.Add(litem);
            this.ResetImageKeys();
        }

        private void provider_DictionaryModified(NetworkConnection oldItem, NetworkConnection newItem)
        {
            lock (listNetwork)
            {
                ListViewItem litem = listNetwork.Items[newItem.ID];

                if (litem == null)
                    return;

                if (newItem.Local != null && newItem.Local.ToString() != "0.0.0.0:0")
                {
                    if (newItem.LocalString != null)
                        litem.SubItems[1].Text = newItem.LocalString + ":" + newItem.Local.Port.ToString() +
                            " (" + newItem.Local.ToString() + ")";
                    else
                        litem.SubItems[1].Text = newItem.Local.ToString();
                }

                if (newItem.Remote != null && newItem.Remote.ToString() != "0.0.0.0:0")
                {
                    if (newItem.RemoteString != null)
                        litem.SubItems[2].Text = newItem.RemoteString + ":" + newItem.Remote.Port.ToString() +
                            " (" + newItem.Remote.ToString() + ")";
                    else
                        litem.SubItems[2].Text = newItem.Remote.ToString();
                }

                litem.SubItems[4].Text = newItem.State != 0 ? newItem.State.ToString() : "";
                _needsSort = true;
            }
        }

        private void provider_DictionaryRemoved(NetworkConnection item)
        {
            int index = listNetwork.Items[item.ID].Index;
            bool selected = listNetwork.Items[item.ID].Selected;
            int selectedCount = listNetwork.SelectedItems.Count;
            ListViewItem litem = listNetwork.Items[item.ID];
            bool imageStillUsed = false;

            if (litem.ImageKey == "generic_process")
            {
                imageStillUsed = true;
            }
            else
            {
                lock (listNetwork)
                {
                    foreach (ListViewItem lvItem in listNetwork.Items)
                    {
                        if (lvItem != litem && lvItem.ImageKey == item.PID.ToString())
                        {
                            imageStillUsed = true;
                            break;
                        }
                    }
                }
            }

            lock (_imageListLock)
            {
                if (!imageStillUsed)
                {
                    imageList.Images.RemoveByKey(item.PID.ToString());
                }
            }

            // Do this outside of the lock; ResetImageKeys locks anyway
            if (!imageStillUsed)
            {
                // Set the item's icon to generic_process, otherwise we are going to 
                // get a blank space for the icon.
                litem.ImageKey = "generic_process";
                // Reset all the image keys (by now most items' icons have screwed up).
                this.ResetImageKeys();
            }

            lock (listNetwork)
                litem.Remove();
        }
    }
}
