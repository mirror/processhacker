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
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using System.Drawing;

namespace ProcessHacker
{
    public partial class NetworkList : UserControl
    {
        private NetworkProvider _provider;
        private HighlightingContext _highlightingContext;
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
            listNetwork.ListViewItemSorter = new SortedListComparer(listNetwork);
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
                    _provider.DictionaryAdded -= new NetworkProvider.ProviderDictionaryAdded(provider_DictionaryAdded);
                    _provider.DictionaryModified -= new NetworkProvider.ProviderDictionaryModified(provider_DictionaryModified);
                    _provider.DictionaryRemoved -= new NetworkProvider.ProviderDictionaryRemoved(provider_DictionaryRemoved);
                    _provider.Updated -= new NetworkProvider.ProviderUpdateOnce(provider_Updated);
                }

                _provider = value;
                
                listNetwork.Items.Clear();

                if (_provider != null)
                {
                    foreach (Win32.NetworkConnection item in _provider.Dictionary.Values)
                    {
                        provider_DictionaryAdded(item);
                    }

                    _provider.UseInvoke = true;
                    _provider.Invoke = new NetworkProvider.ProviderInvokeMethod(this.BeginInvoke);
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

        private void provider_Updated()
        {
            _highlightingContext.Tick();
        }  

        private void provider_DictionaryAdded(Win32.NetworkConnection item)
        {
            if (listNetwork.Groups[item.PID.ToString()] == null)
            {
                string header = Win32.GetNameFromPID(item.PID) + " (" + item.PID.ToString() + ")";

                // find the right place to put the group
                bool inserted = false;

                for (int i = 0; i < listNetwork.Groups.Count; i++)
                {
                    if (listNetwork.Groups[i].Header.CompareTo(header) >= 0)
                    {
                        listNetwork.Groups.Insert(i, new ListViewGroup(item.PID.ToString(), header));
                        inserted = true;
                        break;
                    }
                }

                if (!inserted)
                {
                    // either we don't have any groups or we should add it at the end
                    listNetwork.Groups.Add(new ListViewGroup(item.PID.ToString(), header));
                }
            }

            HighlightedListViewItem litem = new HighlightedListViewItem(_highlightingContext);

            litem.Name = item.ID;
            litem.Group = listNetwork.Groups[item.PID.ToString()];

            if (item.Local != null && item.Local.ToString() != "0.0.0.0:0")
                litem.Text = item.Local.ToString();
            else
                litem.Text = "";

            if (item.Remote != null && item.Remote.ToString() != "0.0.0.0:0")
                litem.SubItems.Add(new ListViewItem.ListViewSubItem(litem, item.Remote.ToString()));
            else
                litem.SubItems.Add(new ListViewItem.ListViewSubItem(litem, ""));

            litem.SubItems.Add(new ListViewItem.ListViewSubItem(litem, item.Protocol.ToString().ToUpper()));
            litem.SubItems.Add(new ListViewItem.ListViewSubItem(litem, item.State != 0 ? item.State.ToString() : ""));

            listNetwork.Items.Add(litem);
        }

        private void provider_DictionaryModified(Win32.NetworkConnection oldItem, Win32.NetworkConnection newItem)
        {
            lock (listNetwork)
            {
                ListViewItem litem = listNetwork.Items[newItem.ID];

                if (litem == null)
                    return;

                if (newItem.Local != null && newItem.Local.ToString() != "0.0.0.0:0")
                {
                    if (newItem.LocalString != null)
                        litem.SubItems[0].Text = newItem.LocalString + ":" + newItem.Local.Port.ToString() +
                            " (" + newItem.Local.ToString() + ")";
                    else
                        litem.SubItems[0].Text = newItem.Local.ToString();
                }

                if (newItem.Remote != null && newItem.Remote.ToString() != "0.0.0.0:0")
                {
                    if (newItem.RemoteString != null)
                        litem.SubItems[1].Text = newItem.RemoteString + ":" + newItem.Remote.Port.ToString() +
                            " (" + newItem.Remote.ToString() + ")";
                    else
                        litem.SubItems[1].Text = newItem.Remote.ToString();
                }

                litem.SubItems[3].Text = newItem.State != 0 ? newItem.State.ToString() : "";
                listNetwork.Sort();
            }
        }

        private void provider_DictionaryRemoved(Win32.NetworkConnection item)
        {
            int index = listNetwork.Items[item.ID].Index;
            bool selected = listNetwork.Items[item.ID].Selected;
            int selectedCount = listNetwork.SelectedItems.Count;
            ListViewItem litem = listNetwork.Items[item.ID];

            litem.Remove();
        }
    }
}
