/*
 * Process Hacker - 
 *   ITreeModel implementation for the process tree
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
using Aga.Controls.Tree;
using System.Windows.Forms;

namespace ProcessHacker
{
    public class ProcessTreeModel : ITreeModel
    {
        private ProcessTree _tree;
        private Dictionary<int, ProcessNode> _processes = new Dictionary<int, ProcessNode>();
        private List<ProcessNode> _roots = new List<ProcessNode>();

        public ProcessTreeModel(ProcessTree tree)
        {
            _tree = tree;
        }

        public void Add(ProcessItem item)
        {
            ProcessNode itemNode = new ProcessNode(item);

            // find this process' parent
            if (item.HasParent && _processes.ContainsKey(item.ParentPid))
                _processes[item.ParentPid].Children.Add(itemNode);
            else
                _roots.Add(itemNode);

            _processes.Add(item.PID, itemNode);

            ProcessNode[] rootNodes = _roots.ToArray();

            // find this process' children
            foreach (ProcessNode node in rootNodes)
            {
                if (node.ProcessItem.HasParent && node.PPID == item.PID)
                {
                    _roots.Remove(node);
                    itemNode.Children.Add(node);
                }
            }

            this.StructureChanged(this, new TreePathEventArgs(new TreePath()));
        }

        public void Modify(ProcessItem oldItem, ProcessItem newItem)
        {
            ProcessNode node = _processes[newItem.PID];

            node.ProcessItem = newItem;

            //if (node.ProcessItem.HasParent && node.PPID != -1)
            //    this.NodesChanged(this, new TreeModelEventArgs(this.GetPath(
            //        _processes.ContainsKey(node.PPID) ? _processes[node.PPID] : null),
            //        new object[] { node }));
        }

        public void Remove(ProcessItem item)
        {
            ProcessNode targetNode = _processes[item.PID];
            ProcessNode[] nodes = _roots.ToArray();
            ProcessNode[] targetChildren = null;

            targetNode.Dispose();

            foreach (ProcessNode node in nodes)
            {
                if (node.PID == item.PID)
                {
                    _roots.Remove(node);
                    this.MoveChildrenToRoot(node);
                    break;
                }
                else if (targetNode.ProcessItem.HasParent && _processes.ContainsKey(targetNode.PPID))
                {
                    ProcessNode foundNode = _processes[targetNode.PPID];

                    if (foundNode != null)
                    {
                        foundNode.Children.Remove(targetNode);
                        targetChildren = targetNode.Children.ToArray();
                        this.MoveChildrenToRoot(targetNode);
                        break;
                    }
                }
            }

            _processes.Remove(item.PID);
            this.StructureChanged(this, new TreePathEventArgs(new TreePath()));

            foreach (ProcessNode n in targetChildren)
            {
                try
                {
                    _tree.FindTreeNode(n).ExpandAll();
                }
                catch
                { }
            }

            _tree.Invalidate();
        }

        public TreePath GetPath(ProcessNode node)
        {
            if (this.GetSortColumn() != "")
                return new TreePath(node);

            if (node == null)
                return TreePath.Empty;
            else
            {
                ProcessNode currentNode = node;
                Stack<ProcessNode> stack = new Stack<ProcessNode>();

                while (true)
                {
                    stack.Push(currentNode);

                    if (currentNode.ProcessItem.HasParent && _processes.ContainsKey(currentNode.PPID))
                    {
                        ProcessNode newNode = _processes[currentNode.PPID];

                        if (newNode == currentNode)
                            break;

                        currentNode = newNode;
                    }
                    else
                    {
                        break;
                    }
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

        public string GetSortColumn()
        {
            foreach (TreeColumn column in _tree.Tree.Columns)
                if (column.SortOrder != SortOrder.None)
                    return column.Header.ToLower();

            return "";
        }

        public SortOrder GetSortOrder()
        {
            foreach (TreeColumn column in _tree.Tree.Columns)
                if (column.SortOrder != SortOrder.None)
                    return column.SortOrder;

            return SortOrder.None;
        }

        public int ModifySort(int sortResult, SortOrder order)
        {
            if (order == SortOrder.Ascending)
                return sortResult * -1;
            else if (order == SortOrder.Descending)
                return sortResult;
            else
                return 0;
        }

        public System.Collections.IEnumerable GetChildren(TreePath treePath)
        {
            if (this.GetSortColumn() != "")
            {
                List<ProcessNode> nodes = new List<ProcessNode>();
                string sortC = this.GetSortColumn();
                SortOrder sortO = this.GetSortOrder();

                nodes.AddRange(_processes.Values);

                nodes.Sort(new Comparison<ProcessNode>(delegate(ProcessNode n1, ProcessNode n2)
                    {
                        if (sortC == "name")
                            return ModifySort(n1.Name.CompareTo(n2.Name), sortO);
                        else if (sortC == "pid")
                            return ModifySort(n1.PID.CompareTo(n2.PID), sortO);
                        else if (sortC == "pvt. memory")
                            return ModifySort(n1.ProcessItem.Process.VirtualMemoryCounters.PrivateBytes.CompareTo(
                                n2.ProcessItem.Process.VirtualMemoryCounters.PrivateBytes), sortO);
                        else if (sortC == "working set")
                            return ModifySort(n1.ProcessItem.Process.VirtualMemoryCounters.WorkingSetSize.CompareTo(
                                n2.ProcessItem.Process.VirtualMemoryCounters.WorkingSetSize), sortO);
                        else if (sortC == "peak working set")
                            return ModifySort(n1.ProcessItem.Process.VirtualMemoryCounters.PeakWorkingSetSize.CompareTo(
                                n2.ProcessItem.Process.VirtualMemoryCounters.PeakWorkingSetSize), sortO);
                        else if (sortC == "virtual size")
                            return ModifySort(n1.ProcessItem.Process.VirtualMemoryCounters.VirtualSize.CompareTo(
                                n2.ProcessItem.Process.VirtualMemoryCounters.VirtualSize), sortO);
                        else if (sortC == "peak virtual size")
                            return ModifySort(n1.ProcessItem.Process.VirtualMemoryCounters.PeakVirtualSize.CompareTo(
                                n2.ProcessItem.Process.VirtualMemoryCounters.PeakVirtualSize), sortO);
                        else if (sortC == "pagefile usage")
                            return ModifySort(n1.ProcessItem.Process.VirtualMemoryCounters.PagefileUsage.CompareTo(
                                n2.ProcessItem.Process.VirtualMemoryCounters.PagefileUsage), sortO);
                        else if (sortC == "peak pagefile usage")
                            return ModifySort(n1.ProcessItem.Process.VirtualMemoryCounters.PeakPagefileUsage.CompareTo(
                                n2.ProcessItem.Process.VirtualMemoryCounters.PeakPagefileUsage), sortO);
                        else if (sortC == "page faults")
                            return ModifySort(n1.ProcessItem.Process.VirtualMemoryCounters.PageFaultCount.CompareTo(
                                n2.ProcessItem.Process.VirtualMemoryCounters.PageFaultCount), sortO);
                        else if (sortC == "cpu")
                            return ModifySort(n1.ProcessItem.CpuUsage.CompareTo(n2.ProcessItem.CpuUsage), sortO);
                        else if (sortC == "username")
                            return ModifySort(n1.Username.CompareTo(n2.Username), sortO);
                        else if (sortC == "session id")
                            return ModifySort(n1.ProcessItem.SessionId.CompareTo(n2.ProcessItem.SessionId), sortO);
                        else if (sortC == "base priority")
                            return ModifySort(n1.ProcessItem.Process.BasePriority.CompareTo(
                                n2.ProcessItem.Process.BasePriority), sortO);
                        else if (sortC == "description")
                            return ModifySort(n1.Description.CompareTo(n2.Description), sortO);
                        else if (sortC == "company")
                            return ModifySort(n1.Company.CompareTo(n2.Company), sortO);
                        else if (sortC == "file name")
                            return ModifySort(n1.FileName.CompareTo(n2.FileName), sortO);
                        else if (sortC == "command line")
                            return ModifySort(n1.CommandLine.CompareTo(n2.CommandLine), sortO);
                        else if (sortC == "threads")
                            return ModifySort(n1.ProcessItem.Process.NumberOfThreads.CompareTo(
                                n2.ProcessItem.Process.NumberOfThreads), sortO);
                        else if (sortC == "handles")
                            return ModifySort(n1.ProcessItem.Process.HandleCount.CompareTo(
                                n2.ProcessItem.Process.HandleCount), sortO);
                        else if (sortC == "gdi handles")
                            return ModifySort(n1.GdiHandlesNumber.CompareTo(n2.GdiHandlesNumber), sortO);
                        else if (sortC == "user handles")
                            return ModifySort(n1.UserHandlesNumber.CompareTo(n2.UserHandlesNumber), sortO);
                        else if (sortC == "i/o total")
                            return ModifySort(n1.IoTotalNumber.CompareTo(n2.IoTotalNumber), sortO);
                        else if (sortC == "i/o ro")
                            return ModifySort(n1.IoReadOtherNumber.CompareTo(n2.IoReadOtherNumber), sortO);
                        else if (sortC == "i/o w")
                            return ModifySort(n1.IoWriteNumber.CompareTo(n2.IoWriteNumber), sortO);
                        else
                            return 0;
                    }));

                return nodes;
            }

            if (treePath.IsEmpty())
                return _roots;
            else
                return (treePath.LastNode as ProcessNode).Children;
        }

        public bool IsLeaf(TreePath treePath)
        {
            if (this.GetSortColumn() != "")
                return true;

            if (treePath.IsEmpty())
                return false;
            else
                return (treePath.LastNode as ProcessNode).Children.Count == 0;
        }

        public event EventHandler<TreeModelEventArgs> NodesChanged;

        public event EventHandler<TreeModelEventArgs> NodesInserted;

        public event EventHandler<TreeModelEventArgs> NodesRemoved;

        public event EventHandler<TreePathEventArgs> StructureChanged;

        public void CallStructureChanged(TreePathEventArgs args)
        {
            this.StructureChanged(this, args);
        }
    }
}
