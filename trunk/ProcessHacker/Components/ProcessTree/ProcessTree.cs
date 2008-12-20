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
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Aga.Controls.Tree;

namespace ProcessHacker
{
    public partial class ProcessTree : UserControl
    {
        ProcessProvider _provider;
        ProcessTreeModel _treeModel;
        public new event KeyEventHandler KeyDown;
        public new event MouseEventHandler MouseDown;
        public new event MouseEventHandler MouseUp;
        public event EventHandler SelectionChanged;

        public ProcessTree()
        {
            InitializeComponent();

            treeProcesses.KeyDown += new KeyEventHandler(ProcessTree_KeyDown);
            treeProcesses.MouseDown += new MouseEventHandler(treeProcesses_MouseDown);
            treeProcesses.MouseUp += new MouseEventHandler(treeProcesses_MouseUp);

            nodeName.ToolTipProvider = new ProcessTooltips(this);
        }

        private void treeProcesses_SelectionChanged(object sender, EventArgs e)
        {
            if (this.SelectionChanged != null)
                this.SelectionChanged(sender, e);
        }

        private void treeProcesses_MouseUp(object sender, MouseEventArgs e)
        {
            if (this.MouseUp != null)
                this.MouseUp(sender, e);
        }

        private void treeProcesses_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.MouseDown != null)
                this.MouseDown(sender, e);
        }

        private void ProcessTree_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.KeyDown != null)
                this.KeyDown(sender, e);
        }

        private void treeProcesses_ColumnClicked(object sender, TreeColumnEventArgs e)
        {
            if (e.Column.SortOrder == SortOrder.None)  
            {
                e.Column.SortOrder = SortOrder.Descending;
            }
            else if (e.Column.SortOrder == SortOrder.Descending) 
            {
                e.Column.SortOrder = SortOrder.Ascending;  
            }
            else
            {
                e.Column.SortOrder = SortOrder.None;
            }

            _treeModel.CallStructureChanged(new TreePathEventArgs(new TreePath()));

            treeProcesses.Root.ExpandAll();
            this.RefreshItems();
        }

        #region Properties

        public override bool Focused
        {
            get
            {
                return treeProcesses.Focused;
            }
        }

        public override ContextMenu ContextMenu
        {
            get { return treeProcesses.ContextMenu; }
            set { treeProcesses.ContextMenu = value; }
        }

        public override ContextMenuStrip ContextMenuStrip
        {
            get { return treeProcesses.ContextMenuStrip; }
            set { treeProcesses.ContextMenuStrip = value; }
        }

        public TreeViewAdv Tree
        {
            get { return treeProcesses; }
        }

        public ProcessTreeModel Model
        {
            get { return _treeModel; }
        }

        public ProcessProvider Provider
        {
            get { return _provider; }
            set
            {
                if (_provider != null)
                {
                    _provider.DictionaryAdded -= new Provider<int, ProcessItem>.ProviderDictionaryAdded(provider_DictionaryAdded);
                    _provider.DictionaryModified -= new Provider<int, ProcessItem>.ProviderDictionaryModified(provider_DictionaryModified);
                    _provider.DictionaryRemoved -= new Provider<int, ProcessItem>.ProviderDictionaryRemoved(provider_DictionaryRemoved);
                }

                _provider = value;

                treeProcesses.Model = _treeModel = new ProcessTreeModel(this);

                if (_provider != null)
                {
                    foreach (ProcessItem item in _provider.Dictionary.Values)
                    {
                        provider_DictionaryAdded(item);
                    }

                    _provider.UseInvoke = true;
                    _provider.Invoke = new Provider<int, ProcessItem>.ProviderInvokeMethod(this.BeginInvoke);
                    _provider.DictionaryAdded += new Provider<int, ProcessItem>.ProviderDictionaryAdded(provider_DictionaryAdded);
                    _provider.DictionaryModified += new Provider<int, ProcessItem>.ProviderDictionaryModified(provider_DictionaryModified);
                    _provider.DictionaryRemoved += new Provider<int, ProcessItem>.ProviderDictionaryRemoved(provider_DictionaryRemoved);
                }
            }
        }

        #endregion

        #region Core Process List

        private void PerformDelayed(int delay, MethodInvoker action)
        {
            Timer t = new Timer();

            t.Tick += new EventHandler(delegate(object o, EventArgs args)
            {
                t.Enabled = false;
                action();
            });

            t.Interval = delay;
            t.Enabled = true;
        }

        private Color GetProcessColor(ProcessItem p)
        {
            if (Program.HackerWindow.ProcessServices.ContainsKey(p.PID) &&
                Program.HackerWindow.ProcessServices[p.PID].Count > 0)
                return Properties.Settings.Default.ColorServiceProcesses;
            else if (p.IsBeingDebugged)
                return Properties.Settings.Default.ColorBeingDebugged;
            else if (p.Username == "NT AUTHORITY\\SYSTEM")
                return Properties.Settings.Default.ColorSystemProcesses;
            else if (p.Username == System.Security.Principal.WindowsIdentity.GetCurrent().Name)
                return Properties.Settings.Default.ColorOwnProcesses;
            else
                return SystemColors.Window;
        }

        private void provider_DictionaryAdded(ProcessItem item)
        {
            _treeModel.Add(item);

            TreeNodeAdv node = this.FindTreeNode(item.PID);

            if (node != null)
            {
                if (_provider.RunCount > 1)
                {
                    node.State = TreeNodeAdv.NodeState.New;
                    this.PerformDelayed(Properties.Settings.Default.HighlightingDuration,
                        new MethodInvoker(delegate { node.State = TreeNodeAdv.NodeState.Normal; }));
                }

                node.BackColor = GetProcessColor(item);
                node.ExpandAll();
            }
        }

        private void provider_DictionaryModified(ProcessItem oldItem, ProcessItem newItem)
        {
            _treeModel.Modify(oldItem, newItem);

            TreeNodeAdv node = this.FindTreeNode(newItem.PID);

            if (node != null)
            {
                node.BackColor = GetProcessColor(newItem);
            }

            if (_treeModel.GetSortColumn() != "")
                _treeModel.CallStructureChanged(new TreePathEventArgs(new TreePath()));
        }

        private void provider_DictionaryRemoved(ProcessItem item)
        {
            TreeNodeAdv node = this.FindTreeNode(item.PID);

            node.State = TreeNodeAdv.NodeState.Removed;
            this.PerformDelayed(Properties.Settings.Default.HighlightingDuration,
                new MethodInvoker(delegate { _treeModel.Remove(item); }));
        }

        public void RefreshItems()
        {
            foreach (TreeNodeAdv node in treeProcesses.AllNodes)
            {
                try
                {
                    ProcessNode pNode = this.FindNode(node);
                    ProcessItem item = _provider.Dictionary[pNode.PID];

                    node.BackColor = this.GetProcessColor(item);
                }
                catch
                { }
            }
        }

        public Dictionary<int, ProcessNode> Nodes
        {
            get { return _treeModel.Nodes; }
        }

        public TreeNodeAdv FindTreeNode(int PID)
        {
            return treeProcesses.FindNode(_treeModel.GetPath(_treeModel.Nodes[PID]));
        }

        public TreeNodeAdv FindTreeNode(ProcessNode node)
        {
            return this.FindTreeNode(node.PID);
        }

        public ProcessNode FindNode(TreeNodeAdv node)
        {
            return treeProcesses.GetPath(node).LastNode as ProcessNode;
        }

        #endregion

        #region Interfacing

        public void BeginUpdate()
        {
            treeProcesses.BeginUpdate();
        }

        public void EndUpdate()
        {
            treeProcesses.EndUpdate();
        }

        public IEnumerable<TreeNodeAdv> TreeNodes
        {
            get { return treeProcesses.AllNodes; }
        }

        public System.Collections.ObjectModel.ReadOnlyCollection<TreeNodeAdv> SelectedTreeNodes
        {
            get { return treeProcesses.SelectedNodes; }
        }

        public System.Collections.ObjectModel.ReadOnlyCollection<ProcessNode> SelectedNodes
        {
            get
            {
                List<ProcessNode> nodes = new List<ProcessNode>();

                foreach (TreeNodeAdv node in treeProcesses.SelectedNodes)
                    nodes.Add(treeProcesses.GetPath(node).LastNode as ProcessNode);

                System.Collections.ObjectModel.ReadOnlyCollection<ProcessNode> collection =
                    new System.Collections.ObjectModel.ReadOnlyCollection<ProcessNode>(nodes);

                return collection;
            }
        }

        #endregion
    }
}
