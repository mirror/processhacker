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
    public partial class ModuleList : UserControl
    {
        ModuleProvider _provider;
        public new event KeyEventHandler KeyDown;
        public new event MouseEventHandler MouseDown;
        public new event MouseEventHandler MouseUp;
        public new event EventHandler DoubleClick;
        public event EventHandler SelectedIndexChanged;

        public ModuleList()
        {
            InitializeComponent();

            listModules.KeyDown += new KeyEventHandler(ModuleList_KeyDown);
            listModules.MouseDown += new MouseEventHandler(listModules_MouseDown);
            listModules.MouseUp += new MouseEventHandler(listModules_MouseUp);
            listModules.DoubleClick += new EventHandler(listModules_DoubleClick);
            listModules.SelectedIndexChanged += new System.EventHandler(listModules_SelectedIndexChanged);
        }

        private void listModules_DoubleClick(object sender, EventArgs e)
        {
            if (this.DoubleClick != null)
                this.DoubleClick(sender, e);
        }

        private void listModules_MouseUp(object sender, MouseEventArgs e)
        {
            if (this.MouseUp != null)
                this.MouseUp(sender, e);
        }

        private void listModules_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.MouseDown != null)
                this.MouseDown(sender, e);
        }

        private void listModules_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (this.SelectedIndexChanged != null)
                this.SelectedIndexChanged(sender, e);
        }

        private void ModuleList_KeyDown(object sender, KeyEventArgs e)
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
                    BindingFlags.NonPublic | BindingFlags.Instance).GetValue(listModules, null);
            }
            set
            {
                typeof(ListView).GetProperty("DoubleBuffered",
                    BindingFlags.NonPublic | BindingFlags.Instance).SetValue(listModules, value, null);
            }
        }

        public override bool Focused
        {
            get
            {
                return listModules.Focused;
            }
        }

        public override ContextMenu ContextMenu
        {
            get { return listModules.ContextMenu; }
            set { listModules.ContextMenu = value; }
        }

        public override ContextMenuStrip ContextMenuStrip
        {
            get { return listModules.ContextMenuStrip; }
            set { listModules.ContextMenuStrip = value; }
        }

        public ListView List
        {
            get { return listModules; }
        }

        public ModuleProvider Provider
        {
            get { return _provider; }
            set
            {
                if (_provider != null)
                {
                    _provider.DictionaryAdded -= new ModuleProvider.ProviderDictionaryAdded(provider_DictionaryAdded);
                    _provider.DictionaryRemoved -= new ModuleProvider.ProviderDictionaryRemoved(provider_DictionaryRemoved);
                }

                _provider = value;

                listModules.Items.Clear();

                if (_provider != null)
                {
                    foreach (ModuleItem item in _provider.Dictionary.Values)
                    {
                        provider_DictionaryAdded(item);
                    }

                    _provider.UseInvoke = true;
                    _provider.Invoke = new ModuleProvider.ProviderInvokeMethod(this.BeginInvoke);
                    _provider.DictionaryAdded += new ModuleProvider.ProviderDictionaryAdded(provider_DictionaryAdded);
                    _provider.DictionaryRemoved += new ModuleProvider.ProviderDictionaryRemoved(provider_DictionaryRemoved);
                }
            }
        }

        #endregion

        #region Core Module List

        private void provider_DictionaryAdded(ModuleItem item)
        {
            HighlightedListViewItem litem = new HighlightedListViewItem();

            litem.Name = item.BaseAddress.ToString();
            litem.Text = item.Name;
            litem.SubItems.Add(new ListViewItem.ListViewSubItem(litem, "0x" + item.BaseAddress.ToString("x8")));
            litem.SubItems.Add(new ListViewItem.ListViewSubItem(litem, Misc.GetNiceSizeName(item.Size)));
            litem.SubItems.Add(new ListViewItem.ListViewSubItem(litem, item.FileDescription));
            litem.ToolTipText = item.FileName;

            listModules.Items.Add(litem);
        }

        private void provider_DictionaryRemoved(ModuleItem item)
        {
            listModules.Items[item.BaseAddress.ToString()].Remove();
        }

        #endregion

        #region Interfacing

        public void BeginUpdate()
        {
            listModules.BeginUpdate();
        }

        public void EndUpdate()
        {
            listModules.EndUpdate();
        }

        public ListView.ListViewItemCollection Items
        {
            get { return listModules.Items; }
        }

        public ListView.SelectedListViewItemCollection SelectedItems
        {
            get { return listModules.SelectedItems; }
        }

        #endregion
    }
}
