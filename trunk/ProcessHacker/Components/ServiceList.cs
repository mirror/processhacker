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
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using System.Drawing;

namespace ProcessHacker
{
    public partial class ServiceList : UserControl
    {
        ServiceProvider _provider;
        public new event KeyEventHandler KeyDown;
        public new event MouseEventHandler MouseDown;
        public new event MouseEventHandler MouseUp;
        public event EventHandler SelectedIndexChanged;

        public ServiceList()
        {
            InitializeComponent();

            listServices.KeyDown += new KeyEventHandler(ProcessList_KeyDown);
            listServices.MouseDown += new MouseEventHandler(listProcesses_MouseDown);
            listServices.MouseUp += new MouseEventHandler(listProcesses_MouseUp);
            listServices.SelectedIndexChanged += new System.EventHandler(listProcesses_SelectedIndexChanged);
        }

        private void listProcesses_MouseUp(object sender, MouseEventArgs e)
        {
            if (this.MouseUp != null)
                this.MouseUp(sender, e);
        }

        private void listProcesses_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.MouseDown != null)
                this.MouseDown(sender, e);
        }

        private void listProcesses_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (this.SelectedIndexChanged != null)
                this.SelectedIndexChanged(sender, e);
        }

        private void ProcessList_KeyDown(object sender, KeyEventArgs e)
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
                    _provider.DictionaryAdded -= new ProviderDictionaryAdded(provider_DictionaryAdded);
                    _provider.DictionaryModified -= new ProviderDictionaryModified(provider_DictionaryModified);
                    _provider.DictionaryRemoved -= new ProviderDictionaryRemoved(provider_DictionaryRemoved);
                }

                _provider = value;
                
                listServices.Items.Clear();

                if (_provider != null)
                {
                    foreach (Win32.ENUM_SERVICE_STATUS_PROCESS item in _provider.Dictionary.Values)
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

        #region Core Process List

        private void provider_DictionaryAdded(object item)
        {
            Win32.ENUM_SERVICE_STATUS_PROCESS sitem = (Win32.ENUM_SERVICE_STATUS_PROCESS)item;
            HighlightedListViewItem litem = new HighlightedListViewItem();

            litem.Name = sitem.ServiceName;
            litem.Text = sitem.ServiceName;
            litem.SubItems.Add(new ListViewItem.ListViewSubItem(litem, sitem.DisplayName));
            litem.SubItems.Add(new ListViewItem.ListViewSubItem(litem, sitem.ServiceStatusProcess.ServiceType.ToString()));
            litem.SubItems.Add(new ListViewItem.ListViewSubItem(litem, sitem.ServiceStatusProcess.CurrentState.ToString()));
            litem.SubItems.Add(new ListViewItem.ListViewSubItem(litem, sitem.ServiceStatusProcess.ProcessID.ToString()));

            if (sitem.ServiceStatusProcess.ServiceType == Win32.SERVICE_TYPE.Win32OwnProcess ||
                sitem.ServiceStatusProcess.ServiceType == Win32.SERVICE_TYPE.Win32ShareProcess)
                litem.ImageKey = "Win32";
            else if (sitem.ServiceStatusProcess.ServiceType == Win32.SERVICE_TYPE.InteractiveProcess)
                litem.ImageKey = "Interactive";
            else
                litem.ImageKey = "Driver";

            listServices.Items.Add(litem);
        }

        private void provider_DictionaryModified(object item)
        {
            try
            {
                Win32.ENUM_SERVICE_STATUS_PROCESS sitem = (Win32.ENUM_SERVICE_STATUS_PROCESS)item;
                ListViewItem litem = listServices.Items[sitem.ServiceName];

                litem.SubItems[1].Text = sitem.DisplayName;
                litem.SubItems[2].Text = sitem.ServiceStatusProcess.ServiceType.ToString();
                litem.SubItems[3].Text = sitem.ServiceStatusProcess.CurrentState.ToString();
                litem.SubItems[4].Text = sitem.ServiceStatusProcess.ProcessID.ToString();
            }
            catch
            { }
        }

        private void provider_DictionaryRemoved(object item)
        {
            try
            {
                Win32.ENUM_SERVICE_STATUS_PROCESS sitem = (Win32.ENUM_SERVICE_STATUS_PROCESS)item;
                int index = listServices.Items[sitem.ServiceName].Index;
                bool selected = listServices.Items[sitem.ServiceName].Selected;
                int selectedCount = listServices.SelectedItems.Count;
                ListViewItem litem = listServices.Items[sitem.ServiceName];

                litem.Remove();

                if (selected && selectedCount == 1)
                {
                    if (listServices.Items.Count == 0)
                    { }
                    else if (index > (listServices.Items.Count - 1))
                    {
                        listServices.Items[listServices.Items.Count - 1].Selected = true;
                    }
                    else
                    {
                        listServices.Items[index].Selected = true;
                    }
                }
            }
            catch
            { }
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
    }
}
