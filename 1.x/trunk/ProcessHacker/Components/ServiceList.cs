/*
 * Process Hacker - 
 *   service list
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
using System.Reflection;
using System.Windows.Forms;
using ProcessHacker.Common;
using ProcessHacker.Common.Ui;
using ProcessHacker.Native.Api;
using ProcessHacker.UI;

namespace ProcessHacker.Components
{
    public partial class ServiceList : UserControl
    {
        private ServiceProvider _provider;
        private int _runCount = 0;
        private HighlightingContext _highlightingContext;
        private List<ListViewItem> _needsAdd = new List<ListViewItem>();
        private bool _needsSort = false;
        public new event KeyEventHandler KeyDown;
        public new event MouseEventHandler MouseDown;
        public new event MouseEventHandler MouseUp;
        public new event EventHandler DoubleClick;
        public event EventHandler SelectedIndexChanged;

        public ServiceList()
        {
            InitializeComponent();

            _highlightingContext = new HighlightingContext(listServices);
            listServices.SetTheme("explorer");
            listServices.KeyDown += new KeyEventHandler(ServiceList_KeyDown);
            listServices.MouseDown += new MouseEventHandler(listServices_MouseDown);
            listServices.MouseUp += new MouseEventHandler(listServices_MouseUp);
            listServices.DoubleClick += new EventHandler(listServices_DoubleClick);
            listServices.SelectedIndexChanged += new System.EventHandler(listServices_SelectedIndexChanged);
            listServices.ListViewItemSorter = new SortedListViewComparer(listServices);
        }

        private void listServices_DoubleClick(object sender, EventArgs e)
        {
            if (this.DoubleClick != null)
                this.DoubleClick(sender, e);
        }  

        private void listServices_MouseUp(object sender, MouseEventArgs e)
        {
            if (this.MouseUp != null)
                this.MouseUp(sender, e);
        }

        private void listServices_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.MouseDown != null)
                this.MouseDown(sender, e);
        }

        private void listServices_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (this.SelectedIndexChanged != null)
                this.SelectedIndexChanged(sender, e);
        }

        private void ServiceList_KeyDown(object sender, KeyEventArgs e)
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
                    BindingFlags.NonPublic | BindingFlags.Instance).GetValue(listServices, null);
            }
            set
            {
                typeof(ListView).GetProperty("DoubleBuffered",
                    BindingFlags.NonPublic | BindingFlags.Instance).SetValue(listServices, value, null);
            }
        }

        public override bool Focused
        {
            get
            {
                return listServices.Focused;
            }
        }

        public override ContextMenu ContextMenu
        {
            get { return listServices.ContextMenu; }
            set { listServices.ContextMenu = value; }
        }

        public override ContextMenuStrip ContextMenuStrip
        {
            get { return listServices.ContextMenuStrip; }
            set { listServices.ContextMenuStrip = value; }
        }

        public ListView List
        {
            get { return listServices; }
        }

        public ServiceProvider Provider
        {
            get { return _provider; }
            set
            {
                if (_provider != null)
                {
                    _provider.DictionaryAdded -= new ServiceProvider.ProviderDictionaryAdded(provider_DictionaryAdded);
                    _provider.DictionaryModified -= new ServiceProvider.ProviderDictionaryModified(provider_DictionaryModified);
                    _provider.DictionaryRemoved -= new ServiceProvider.ProviderDictionaryRemoved(provider_DictionaryRemoved);
                    _provider.Updated -= new ServiceProvider.ProviderUpdateOnce(provider_Updated);
                }

                _provider = value;
                
                listServices.Items.Clear();

                if (_provider != null)
                {
                    //_provider.InterlockedExecute(new MethodInvoker(() =>
                    //{
                        _provider.DictionaryAdded += new ServiceProvider.ProviderDictionaryAdded(provider_DictionaryAdded);
                        _provider.DictionaryModified += new ServiceProvider.ProviderDictionaryModified(provider_DictionaryModified);
                        _provider.DictionaryRemoved += new ServiceProvider.ProviderDictionaryRemoved(provider_DictionaryRemoved);
                        _provider.Updated += new ServiceProvider.ProviderUpdateOnce(provider_Updated);

                        foreach (ServiceItem item in _provider.Dictionary.Values)
                        {
                            provider_DictionaryAdded(item);
                        }
                    //}));
                }
            }
        }

        #endregion

        #region Interfacing

        public void BeginUpdate()
        {
            listServices.BeginUpdate();
        }

        public void EndUpdate()
        {
            listServices.EndUpdate();
        }

        public ListView.ListViewItemCollection Items
        {
            get { return listServices.Items; }
        }

        public ListView.SelectedListViewItemCollection SelectedItems
        {
            get { return listServices.SelectedItems; }
        }

        #endregion

        private void provider_Updated()
        {
            lock (_needsAdd)
            {
                if (_needsAdd.Count > 0)
                {
                    this.BeginInvoke(new MethodInvoker(() =>
                    {
                        lock (_needsAdd)
                        {
                            listServices.Items.AddRange(_needsAdd.ToArray());
                            _needsAdd.Clear();
                            _needsAdd.TrimExcess();
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
                            listServices.Sort();
                            _needsSort = false;
                        }
                    }));
            }

            _runCount++;
        }

        public void AddItem(ServiceItem item)
        {
            provider_DictionaryAdded(item);
        }

        public void UpdateItems()
        {
            provider_Updated();
        }

        private void provider_DictionaryAdded(ServiceItem item)
        {
            HighlightedListViewItem litem = new HighlightedListViewItem(_highlightingContext,
                item.RunId > 0 && _runCount > 0);

            litem.Name = item.Status.ServiceName;
            litem.Text = item.Status.ServiceName;
            litem.SubItems.Add(new ListViewItem.ListViewSubItem(litem,
                item.Status.DisplayName));
            litem.SubItems.Add(new ListViewItem.ListViewSubItem(litem,
                item.Status.ServiceStatusProcess.ServiceType.ToString()));
            litem.SubItems.Add(new ListViewItem.ListViewSubItem(litem,
                item.Status.ServiceStatusProcess.CurrentState.ToString()));
            litem.SubItems.Add(new ListViewItem.ListViewSubItem(litem,
                item.Config.StartType.ToString()));
            litem.SubItems.Add(new ListViewItem.ListViewSubItem(litem,
                item.Status.ServiceStatusProcess.ProcessID == 0 ? "" :
                item.Status.ServiceStatusProcess.ProcessID.ToString()));

            if ((item.Status.ServiceStatusProcess.ServiceType & ServiceType.InteractiveProcess) != 0)
                litem.ImageKey = "Interactive";
            else if (item.Status.ServiceStatusProcess.ServiceType == ServiceType.Win32OwnProcess ||
                item.Status.ServiceStatusProcess.ServiceType == ServiceType.Win32ShareProcess)
                litem.ImageKey = "Win32";
            else if (item.Status.ServiceStatusProcess.ServiceType == ServiceType.FileSystemDriver)
                litem.ImageKey = "FS";
            else
                litem.ImageKey = "Driver";

            lock (_needsAdd)
                _needsAdd.Add(litem);
        }

        private void provider_DictionaryModified(ServiceItem oldItem, ServiceItem newItem)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new ServiceProvider.ProviderDictionaryModified(provider_DictionaryModified), oldItem, newItem);
                return;
            }

            lock (listServices)
            {
                ListViewItem litem = listServices.Items[newItem.Status.ServiceName];

                if (litem == null)
                    return;

                litem.SubItems[1].Text = newItem.Status.DisplayName;
                litem.SubItems[2].Text = newItem.Status.ServiceStatusProcess.ServiceType.ToString();
                litem.SubItems[3].Text = newItem.Status.ServiceStatusProcess.CurrentState.ToString();
                litem.SubItems[4].Text = newItem.Config.StartType.ToString();
                litem.SubItems[5].Text = newItem.Status.ServiceStatusProcess.ProcessID == 0 ? "" :
                    newItem.Status.ServiceStatusProcess.ProcessID.ToString();
                _needsSort = true;
            }
        }

        private void provider_DictionaryRemoved(ServiceItem item)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new ServiceProvider.ProviderDictionaryRemoved(provider_DictionaryRemoved), item);
                return;
            }

            lock (listServices)
                listServices.Items[item.Status.ServiceName].Remove();
        }
    }
}
