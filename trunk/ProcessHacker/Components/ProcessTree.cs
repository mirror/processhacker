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
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using System.Drawing;
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

                treeProcesses.Model = _treeModel = new ProcessTreeModel();

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

            if (this.FindTreeNode(item.PID) != null)
                this.FindTreeNode(item.PID).ExpandAll();
        }

        private void provider_DictionaryModified(ProcessItem oldItem, ProcessItem newItem)
        {
            _treeModel.Modify(oldItem, newItem);
        }

        private void provider_DictionaryRemoved(ProcessItem item)
        {
            _treeModel.Remove(item);
        }

        public void RefreshItems()
        {
            //lock (listProcesses)
            //{
            //    foreach (ListViewItem litem in listProcesses.Items)
            //    {
            //        try
            //        {
            //            ProcessItem item = _provider.Dictionary[int.Parse(litem.Name)];

            //            (litem as HighlightedListViewItem).NormalColor = this.GetProcessColor(item);
            //            litem.SubItems[4].Text = this.GetBestUsername(item.Username,
            //                Properties.Settings.Default.ShowAccountDomains);
            //        }
            //        catch
            //        { }
            //    }
            //}
        }

        public Dictionary<int, ProcessNode> Nodes
        {
            get { return _treeModel.Nodes; }
        }

        public TreeNodeAdv FindTreeNode(int PID)
        {
            return treeProcesses.FindNode(_treeModel.GetPath(_treeModel.Nodes[PID]));
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

    public class ProcessTreeModel : ITreeModel
    {
        private Dictionary<int, ProcessNode> _processes = new Dictionary<int,ProcessNode>();
        private List<ProcessNode> _roots = new List<ProcessNode>();

        public void Add(ProcessItem item)
        {
            ProcessNode itemNode = new ProcessNode(item);

            // find this process' parent
            if (_processes.ContainsKey(item.ParentPID))
                _processes[item.ParentPID].Children.Add(itemNode);
            else
                _roots.Add(itemNode);

            _processes.Add(item.PID, itemNode);

            ProcessNode[] rootNodes = _roots.ToArray();

            // find this process' children
            foreach (ProcessNode node in rootNodes)
            {
                if (node.PPID == item.PID)
                {
                    _roots.Remove(node);
                    itemNode.Children.Add(node);
                }
            }

            this.StructureChanged(this, new TreePathEventArgs(new TreePath()));
        }

        public void Modify(ProcessItem oldItem, ProcessItem newItem)
        {
            _processes[newItem.PID].ProcessItem = newItem;

            ProcessNode node = _processes[newItem.PID];

            this.NodesChanged(this, new TreeModelEventArgs(this.GetPath(
                _processes.ContainsKey(node.PPID) ? _processes[node.PPID] : null), 
                new object[] { node }));
        }

        public void Remove(ProcessItem item)
        {
            ProcessNode targetNode = _processes[item.PID];
            ProcessNode[] nodes = _roots.ToArray();

            foreach (ProcessNode node in nodes)
            {
                if (node.PID == item.PID)
                {
                    _roots.Remove(node);
                    this.MoveChildrenToRoot(node);
                    break;
                }
                else if (_processes.ContainsKey(targetNode.PPID))
                {
                    ProcessNode foundNode = _processes[targetNode.PPID];

                    if (foundNode != null)
                    {
                        foundNode.Children.Remove(targetNode);
                        this.MoveChildrenToRoot(targetNode);
                        break;
                    }
                }
            }

            _processes.Remove(item.PID);

            this.StructureChanged(this, new TreePathEventArgs(new TreePath()));
        }

        public TreePath GetPath(ProcessNode node)
        {
            if (node == null)
                return TreePath.Empty;
            else
            {
                Stack<ProcessNode> stack = new Stack<ProcessNode>();

                while (true)
                {
                    stack.Push(node);

                    if (_processes.ContainsKey(node.PPID))
                    {
                        ProcessNode newNode = _processes[node.PPID];

                        if (newNode == node)
                            break;

                        node = newNode;
                    }
                    else
                        break;
                }

                return new TreePath(stack.ToArray());
            }
        }

        public void MoveChildrenToRoot(ProcessNode node)
        {
            ProcessNode[] children = node.Children.ToArray();

            _roots.AddRange(children);
        }

        public Dictionary<int, ProcessNode> Nodes
        {
            get { return _processes; }
        }

        #region ITreeModel Members

        public System.Collections.IEnumerable GetChildren(TreePath treePath)
        {
            if (treePath.IsEmpty())
                return _roots;
            else
                return (treePath.LastNode as ProcessNode).Children;
        }

        public bool IsLeaf(TreePath treePath)
        {
            if (treePath.IsEmpty())
                return false;
            else
                return (treePath.LastNode as ProcessNode).Children.Count == 0;
        }

        public event EventHandler<TreeModelEventArgs> NodesChanged;

        public event EventHandler<TreeModelEventArgs> NodesInserted;

        public event EventHandler<TreeModelEventArgs> NodesRemoved;

        public event EventHandler<TreePathEventArgs> StructureChanged;

        #endregion
    }

    public class ProcessNode : Node
    {
        private List<ProcessNode> _children = new List<ProcessNode>();
        private ProcessItem _pitem;
        private bool _wasNoIcon = false;
        private Bitmap _icon;

        public ProcessNode(ProcessItem pitem)
        {
            _pitem = pitem;
            this.Tag = pitem.PID;

            if (_pitem.Icon == null)
            {
                _wasNoIcon = true;
                _icon = global::ProcessHacker.Properties.Resources.Process_small.ToBitmap();
            }
            else
            {
                _icon = _pitem.Icon.ToBitmap();
            }
        }

        public ProcessItem ProcessItem
        {
            get { return _pitem; }
            set
            {
                _pitem = value;

                if (_wasNoIcon && _pitem.Icon != null)
                {
                    _icon = _pitem.Icon.ToBitmap();
                    _wasNoIcon = false;
                }
            }
        }

        public List<ProcessNode> Children
        {
            get { return _children; }
        }

        public string Name
        {
            get { return _pitem.Name; }
        }

        public int PID
        {
            get { return _pitem.PID; }
        }

        public int PPID
        {
            get { return _pitem.ParentPID; }
        }

        public string Memory
        {
            get { return _pitem.MemoryUsage; }
        }

        public string CPU
        {
            get { return _pitem.CPUUsage.ToString("F2"); }
        }

        private string GetBestUsername(string username, bool includeDomain)
        {
            if (!username.Contains("\\"))
                return username;

            string[] split = username.Split(new char[] { '\\' }, 2);
            string domain = split[0];
            string user = split[1];

            if (includeDomain)
                return domain + "\\" + user;
            else
                return user;
        }

        public string Username
        {
            get { return this.GetBestUsername(_pitem.Username, Properties.Settings.Default.ShowAccountDomains); }
        }

        public Bitmap Icon
        {
            get { return _icon; }
        }
    }

    public class ProcessTooltips : IToolTipProvider
    {
        private ProcessTree _tree;

        public ProcessTooltips(ProcessTree owner)
        {
            _tree = owner;
        }

        public string GetToolTip(TreeNodeAdv node, Aga.Controls.Tree.NodeControls.NodeControl nodeControl)
        {
            try
            {
                ProcessNode pNode = _tree.Tree.GetPath(node).LastNode as ProcessNode;

                string filename = "";

                if (pNode.PID == 4)
                {
                    filename = Misc.GetKernelFileName();
                }
                else
                {
                    filename = pNode.ProcessItem.Process.MainModule.FileName;
                }

                FileVersionInfo info = FileVersionInfo.GetVersionInfo(
                    Misc.GetRealPath(filename));

                string fileText = (pNode.ProcessItem.CmdLine != null ? (pNode.ProcessItem.CmdLine + "\n\n") : "") + info.FileName + "\n" +
                    info.FileDescription + " (" + info.FileVersion + ")\n" +
                    info.CompanyName;

                if (!Program.HackerWindow.ProcessServices.ContainsKey(pNode.PID))
                {
                    return fileText;
                }
                else
                {
                    string servicesText = "";

                    foreach (string service in Program.HackerWindow.ProcessServices[pNode.PID])
                    {
                        if (Program.HackerWindow.ServiceProvider.Dictionary.ContainsKey(service))
                        {
                            if (Program.HackerWindow.ServiceProvider.Dictionary[service].Status.DisplayName != "")
                                servicesText += service + " (" + 
                                Program.HackerWindow.ServiceProvider.Dictionary[service].Status.DisplayName + ")\n";
                            else
                                servicesText += service + "\n";
                        }
                        else
                        {
                            servicesText += service + "\n";
                        }
                    }

                    return fileText + "\n\nServices:\n" + servicesText.TrimEnd('\n');
                }
            }
            catch
            { }

            return string.Empty;
        }
    }
}
