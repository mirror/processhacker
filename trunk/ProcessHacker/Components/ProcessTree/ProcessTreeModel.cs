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
using Aga.Controls.Tree;

namespace ProcessHacker
{
    public class ProcessTreeModel : ITreeModel
    {
        private Dictionary<int, ProcessNode> _processes = new Dictionary<int, ProcessNode>();
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
}
