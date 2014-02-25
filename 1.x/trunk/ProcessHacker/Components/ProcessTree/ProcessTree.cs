/*
 * Process Hacker - 
 *   process tree control
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
using System.Windows.Forms;
using Aga.Controls.Tree;
using ProcessHacker.Common;
using ProcessHacker.Native;
using ProcessHacker.Native.Api;

namespace ProcessHacker
{
    public partial class ProcessTree : UserControl
    {
        private ProcessSystemProvider _provider;
        private ProcessTreeModel _treeModel;
        private ProcessToolTipProvider _tooltipProvider;
        private int _runCount = 0;
        public new event KeyEventHandler KeyDown;
        public new event MouseEventHandler MouseDown;
        public new event MouseEventHandler MouseUp;
        public new event EventHandler DoubleClick;
        public event EventHandler SelectionChanged;
        public event EventHandler<TreeNodeAdvMouseEventArgs> NodeMouseDoubleClick;
        private object _listLock = new object();
        private bool _draw = true;
        private bool _dumpMode = false;

        public ProcessTree()
        {
            InitializeComponent();

            var column = new TreeColumn("CPU History", 60);

            column.IsVisible = false;
            column.MinColumnWidth = 10;
            treeProcesses.Columns.Add(column);
            treeProcesses.NodeControls.Add(new ProcessHacker.Components.NodePlotter()
            {
                DataPropertyName = "CpuHistory",
                ParentColumn = column
            });

            column = new TreeColumn("I/O History", 60);
            column.IsVisible = false;
            column.MinColumnWidth = 10;
            treeProcesses.Columns.Add(column);
            treeProcesses.NodeControls.Add(new ProcessHacker.Components.NodePlotter()
            {
                DataPropertyName = "IoHistory",
                ParentColumn = column
            });

            treeProcesses.KeyDown += new KeyEventHandler(ProcessTree_KeyDown);
            treeProcesses.MouseDown += new MouseEventHandler(treeProcesses_MouseDown);
            treeProcesses.MouseUp += new MouseEventHandler(treeProcesses_MouseUp);
            treeProcesses.DoubleClick += new EventHandler(treeProcesses_DoubleClick);

            nodeName.ToolTipProvider = _tooltipProvider = new ProcessToolTipProvider(this);

            // make it draw when we want it to draw :)
            treeProcesses.BeginUpdate();
        }

        private void treeProcesses_DoubleClick(object sender, EventArgs e)
        {
            if (this.DoubleClick != null)
                this.DoubleClick(sender, e);
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

        private void treeProcesses_NodeMouseDoubleClick(object sender, TreeNodeAdvMouseEventArgs e)
        {
            if (this.NodeMouseDoubleClick != null)
                this.NodeMouseDoubleClick(sender, e);
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

        public bool Draw
        {
            get { return _draw; }
            set { _draw = value; }
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

        public ProcessSystemProvider Provider
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
                }

                _provider = value;
                treeProcesses.Model = _treeModel = new ProcessTreeModel(this);

                if (_provider != null)
                {
                    // Do an interlocked execute so that we don't get corrupted state.
                    //_provider.InterlockedExecute(new MethodInvoker(() =>
                    //    {
                            _provider.DictionaryAdded += provider_DictionaryAdded;
                            _provider.DictionaryModified += provider_DictionaryModified;
                            _provider.DictionaryRemoved += provider_DictionaryRemoved;
                            _provider.Updated += provider_Updated;

                            treeProcesses.BeginUpdate();
                            treeProcesses.BeginCompleteUpdate();

                            foreach (ProcessItem item in _provider.Dictionary.Values)
                            {
                                provider_DictionaryAdded(item);
                            }

                            treeProcesses.EndCompleteUpdate();
                            treeProcesses.EndUpdate();
                        //}));
                }
            }
        }

        public ProcessToolTipProvider TooltipProvider
        {
            get { return _tooltipProvider; }
        }

        public bool DumpMode
        {
            get { return _dumpMode; }
            set { _dumpMode = value; }
        }

        public IDictionary<int, ProcessItem> DumpProcesses
        {
            get;
            set;
        }

        public IDictionary<int, List<string>> DumpProcessServices
        {
            get;
            set;
        }

        public IDictionary<string, ServiceItem> DumpServices
        {
            get;
            set;
        }

        public string DumpUserName
        {
            get;
            set;
        }

        #endregion

        private void provider_Updated()
        {
            if (_draw)
            {
                this.BeginInvoke(new MethodInvoker(delegate
                {
                    if (_treeModel.GetSortColumn() != "")
                    {
                        _treeModel.CallStructureChanged(new TreePathEventArgs(new TreePath()));
                    }

                    //treeProcesses.InvalidateNodeControlCache();
                    treeProcesses.Invalidate();
                }));
            }

            _runCount++;
        }

        private void PerformDelayed(int delay, MethodInvoker action)
        {
            Timer t = new Timer();

            t.Tick += new EventHandler(delegate(object o, EventArgs args)
            {
                t.Enabled = false;
                action();
                t.Dispose();
            });

            t.Interval = delay;
            t.Enabled = true;
        }

        private Color GetProcessColor(ProcessItem p)
        {
            if (Settings.Instance.UseColorDebuggedProcesses && p.IsBeingDebugged)
                return Settings.Instance.ColorDebuggedProcesses;
            else if (Settings.Instance.UseColorElevatedProcesses &&
                p.ElevationType == TokenElevationType.Full)
                return Settings.Instance.ColorElevatedProcesses;
            else if (Settings.Instance.UseColorPosixProcesses &&
                p.IsPosix)
                return Settings.Instance.ColorPosixProcesses;
            else if (Settings.Instance.UseColorWow64Processes &&
                p.IsWow64)
                return Settings.Instance.ColorWow64Processes;
            else if (Settings.Instance.UseColorJobProcesses && p.IsInSignificantJob)
                return Settings.Instance.ColorJobProcesses;
            else if (Settings.Instance.UseColorPackedProcesses &&
                Settings.Instance.VerifySignatures &&
                p.Name != null &&
                Program.ImposterNames.Contains(p.Name.ToLowerInvariant()) &&
                p.VerifyResult != VerifyResult.Trusted &&
                p.VerifyResult != VerifyResult.Unknown &&
                p.FileName != null)
                return Settings.Instance.ColorPackedProcesses;
            else if (Settings.Instance.UseColorPackedProcesses &&
                Settings.Instance.VerifySignatures &&
                p.VerifyResult != VerifyResult.Trusted &&
                p.VerifyResult != VerifyResult.NoSignature &&
                p.VerifyResult != VerifyResult.Unknown)
                return Settings.Instance.ColorPackedProcesses;
            else if (Settings.Instance.UseColorDotNetProcesses && p.IsDotNet)
                return Settings.Instance.ColorDotNetProcesses;
            else if (Settings.Instance.UseColorPackedProcesses && p.IsPacked)
                return Settings.Instance.ColorPackedProcesses;
            else if (_dumpMode && Settings.Instance.UseColorServiceProcesses &&
                DumpProcessServices.ContainsKey(p.Pid) && DumpProcessServices[p.Pid].Count > 0)
                return Settings.Instance.ColorServiceProcesses;
            else if (!_dumpMode && Settings.Instance.UseColorServiceProcesses &&
                Program.HackerWindow.ProcessServices.ContainsKey(p.Pid) &&
                Program.HackerWindow.ProcessServices[p.Pid].Count > 0)
                return Settings.Instance.ColorServiceProcesses;
            else if (Settings.Instance.UseColorSystemProcesses && p.Username == "NT AUTHORITY\\SYSTEM")
                return Settings.Instance.ColorSystemProcesses;
            else if (_dumpMode && Settings.Instance.UseColorOwnProcesses && p.Username == DumpUserName)
                return Settings.Instance.ColorOwnProcesses;
            else if (!_dumpMode && Settings.Instance.UseColorOwnProcesses && p.Username == Program.CurrentUsername)
                return Settings.Instance.ColorOwnProcesses;
            else
                return SystemColors.Window;
        }

        public void AddItem(ProcessItem item)
        {
            provider_DictionaryAdded(item);
        }

        public void UpdateItems()
        {
            provider_Updated();
        }

        private void provider_DictionaryAdded(ProcessItem item)
        {
            this.BeginInvoke(new MethodInvoker(delegate
            {
                lock (_listLock)
                {
                    _treeModel.Add(item);

                    TreeNodeAdv node = this.FindTreeNode(item.Pid);

                    if (node != null)
                    {
                        if (item.RunId > 0 && _runCount > 0)
                        {
                            node.State = TreeNodeAdv.NodeState.New;
                            this.PerformDelayed(Settings.Instance.HighlightingDuration,
                                new MethodInvoker(delegate
                            {
                                node.State = TreeNodeAdv.NodeState.Normal;
                                treeProcesses.Invalidate();
                            }));
                        }

                        node.BackColor = this.GetProcessColor(item);
                        node.ExpandAll();
                    }
                }
            }));
        }

        private void provider_DictionaryModified(ProcessItem oldItem, ProcessItem newItem)
        {
            this.BeginInvoke(new MethodInvoker(delegate
            {
                lock (_listLock)
                {
                    TreeNodeAdv node = this.FindTreeNode(newItem.Pid);

                    if (node != null)
                    {
                        node.BackColor = this.GetProcessColor(newItem);
                    }

                    _treeModel.Nodes[newItem.Pid].ProcessItem = newItem;
                }
            }));
        }

        private void provider_DictionaryRemoved(ProcessItem item)
        {
            this.BeginInvoke(new MethodInvoker(delegate
            {
                lock (_listLock)
                {
                    TreeNodeAdv node = this.FindTreeNode(item.Pid);

                    if (node != null)
                    {
                        //if (this.StateHighlighting)
                        //{
                            node.State = TreeNodeAdv.NodeState.Removed;
                            this.PerformDelayed(Settings.Instance.HighlightingDuration,
                                new MethodInvoker(delegate
                            {
                                try
                                {
                                    _treeModel.Remove(item);
                                    this.RefreshItems();
                                }
                                catch (Exception ex)
                                {
                                    Logging.Log(ex);
                                }
                            }));
                        //}
                        //else
                        //{
                        //    _treeModel.Remove(item);
                        //}

                        treeProcesses.Invalidate();
                    }
                }
            }));
        }

        public void RefreshItems()
        {
            lock (_listLock)
            {
                foreach (TreeNodeAdv node in treeProcesses.AllNodes)
                {
                    try
                    {
                        ProcessNode pNode = this.FindNode(node);
                        IDictionary<int, ProcessItem> processes;

                        if (!_dumpMode)
                            processes = _provider.Dictionary;
                        else
                            processes = DumpProcesses;

                        // May not be in the dictionary if the process has terminated but 
                        // the node is still being highlighted.
                        if (processes.ContainsKey(pNode.Pid))
                        {
                            ProcessItem item = processes[pNode.Pid];

                            node.BackColor = this.GetProcessColor(item);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logging.Log(ex);
                    }
                }
            }
        }

        public Dictionary<int, ProcessNode> Nodes
        {
            get { return _treeModel.Nodes; }
        }

        public TreeNodeAdv FindTreeNode(int pid)
        {
            if (_treeModel.Nodes.ContainsKey(pid))
                return treeProcesses.FindNode(_treeModel.GetPath(_treeModel.Nodes[pid]));
            else
                return null;
        }

        public TreeNodeAdv FindTreeNode(ProcessNode node)
        {
            return this.FindTreeNode(node.Pid);
        }

        public ProcessNode FindNode(TreeNodeAdv node)
        {
            return treeProcesses.GetPath(node).LastNode as ProcessNode;
        }

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
