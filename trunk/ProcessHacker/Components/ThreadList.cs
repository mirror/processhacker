/*
 * Process Hacker
 * 
 * Copyright (C) 2008 wj32
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Reflection;
using System.Windows.Forms;
using System.Diagnostics;

namespace ProcessHacker
{
    public partial class ThreadList : UserControl
    {
        ThreadProvider _provider;
        public new event KeyEventHandler KeyDown;
        public new event MouseEventHandler MouseDown;
        public new event MouseEventHandler MouseUp;
        public new event EventHandler DoubleClick;
        public event EventHandler SelectedIndexChanged;

        public ThreadList()
        {
            InitializeComponent();

            listThreads.KeyDown += new KeyEventHandler(ThreadList_KeyDown);
            listThreads.MouseDown += new MouseEventHandler(listThreads_MouseDown);
            listThreads.MouseUp += new MouseEventHandler(listThreads_MouseUp);
            listThreads.DoubleClick += new EventHandler(listThreads_DoubleClick);
            listThreads.SelectedIndexChanged += new System.EventHandler(listThreads_SelectedIndexChanged);
        }

        private void listThreads_DoubleClick(object sender, EventArgs e)
        {
            if (this.DoubleClick != null)
                this.DoubleClick(sender, e);
        }

        private void listThreads_MouseUp(object sender, MouseEventArgs e)
        {
            if (this.MouseUp != null)
                this.MouseUp(sender, e);
        }

        private void listThreads_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.MouseDown != null)
                this.MouseDown(sender, e);
        }

        private void listThreads_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (this.SelectedIndexChanged != null)
                this.SelectedIndexChanged(sender, e);
        }

        private void ThreadList_KeyDown(object sender, KeyEventArgs e)
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
                    BindingFlags.NonPublic | BindingFlags.Instance).GetValue(listThreads, null);
            }
            set
            {
                typeof(ListView).GetProperty("DoubleBuffered",
                    BindingFlags.NonPublic | BindingFlags.Instance).SetValue(listThreads, value, null);
            }
        }

        public override bool Focused
        {
            get
            {
                return listThreads.Focused;
            }
        }

        public override ContextMenu ContextMenu
        {
            get { return listThreads.ContextMenu; }
            set { listThreads.ContextMenu = value; }
        }

        public override ContextMenuStrip ContextMenuStrip
        {
            get { return listThreads.ContextMenuStrip; }
            set { listThreads.ContextMenuStrip = value; }
        }

        public ListView List
        {
            get { return listThreads; }
        }

        public ThreadProvider Provider
        {
            get { return _provider; }
            set
            {
                if (_provider != null)
                {
                    _provider.DictionaryAdded -= new ProviderDictionaryAdded(provider_DictionaryAdded);
                    _provider.DictionaryModified -= new ProviderDictionaryModified(provider_DictionaryModified);
                    _provider.DictionaryRemoved -= new ProviderDictionaryRemoved(provider_DictionaryRemoved);
                }

                _provider = value;

                listThreads.Items.Clear();

                if (_provider != null)
                {
                    foreach (ThreadItem item in _provider.Dictionary.Values)
                    {
                        provider_DictionaryAdded(item);
                    }

                    _provider.UseInvoke = true;
                    _provider.Invoke = new ProviderInvokeMethod(this.BeginInvoke);
                    _provider.DictionaryAdded += new ProviderDictionaryAdded(provider_DictionaryAdded);
                    _provider.DictionaryModified += new ProviderDictionaryModified(provider_DictionaryModified);
                    _provider.DictionaryRemoved += new ProviderDictionaryRemoved(provider_DictionaryRemoved);
                }
            }
        }

        #endregion

        #region Core Thread List

        private void provider_DictionaryAdded(object item)
        {
            lock (listThreads)
            {
                ThreadItem titem = (ThreadItem)item;
                HighlightedListViewItem litem = new HighlightedListViewItem();

                litem.Name = titem.TID.ToString();
                litem.Text = titem.TID.ToString();
                litem.SubItems.Add(new ListViewItem.ListViewSubItem(litem, titem.State));
                litem.SubItems.Add(new ListViewItem.ListViewSubItem(litem, titem.CPUTime));
                litem.SubItems.Add(new ListViewItem.ListViewSubItem(litem, titem.Priority));

                listThreads.Items.Add(litem);
            }
        }

        private void provider_DictionaryModified(object oldItem, object newItem)
        {
            lock (listThreads)
            {
                ThreadItem titem = (ThreadItem)newItem;
                ListViewItem litem = listThreads.Items[titem.TID.ToString()];

                litem.SubItems[1].Text = titem.State;
                litem.SubItems[2].Text = titem.CPUTime;
                litem.SubItems[3].Text = titem.Priority;
            }
        }

        private void provider_DictionaryRemoved(object item)
        {
            lock (listThreads)
            {
                ThreadItem titem = (ThreadItem)item;
                int index = listThreads.Items[titem.TID.ToString()].Index;
                bool selected = listThreads.Items[titem.TID.ToString()].Selected;
                int selectedCount = listThreads.SelectedItems.Count;

                listThreads.Items[titem.TID.ToString()].Remove();

                if (selected && selectedCount == 1)
                {
                    if (listThreads.Items.Count == 0)
                    { }
                    else if (index > (listThreads.Items.Count - 1))
                    {
                        listThreads.Items[listThreads.Items.Count - 1].Selected = true;
                    }
                    else
                    {
                        listThreads.Items[index].Selected = true;
                    }
                }
            }
        }

        #endregion

        #region Interfacing

        public void BeginUpdate()
        {
            listThreads.BeginUpdate();
        }

        public void EndUpdate()
        {
            listThreads.EndUpdate();
        }

        public ListView.ListViewItemCollection Items
        {
            get { return listThreads.Items; }
        }

        public ListView.SelectedListViewItemCollection SelectedItems
        {
            get { return listThreads.SelectedItems; }
        }

        #endregion
    }
}
