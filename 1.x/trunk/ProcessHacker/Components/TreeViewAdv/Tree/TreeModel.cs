using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace Aga.Controls.Tree
{
	/// <summary>
	/// Provides a simple ready to use implementation of ITreeModel. Warning: this class is not optimized 
	/// to work with big amount of data. In this case create you own implementation of ITreeModel, and pay attention
	/// on GetChildren and IsLeaf methods.
	/// </summary>
	public class TreeModel : ITreeModel
	{
	    public Node Root { get; private set; }

	    public Collection<Node> Nodes
		{
			get { return this.Root.Nodes; }
		}

		public TreeModel()
		{
			this.Root = new Node 
            {
                Model = this
            };
		}

		public TreePath GetPath(Node node)
		{
			if (node == this.Root)
				return TreePath.Empty;
		    
            Stack<object> stack = new Stack<object>();
		   
            while (node != this.Root)
		    {
		        stack.Push(node);
		        node = node.Parent;
		    }

		    return new TreePath(stack.ToArray());
		}

		public Node FindNode(TreePath path)
		{
			if (path.IsEmpty())
				return this.Root;

            return FindNode(this.Root, path, 0);
		}

		private Node FindNode(Node root, TreePath path, int level)
		{
            foreach (Node node in root.Nodes)
            {
                if (node == path.FullPath[level])
                {
                    if (level == path.FullPath.Length - 1)
                        return node;

                    return FindNode(node, path, level + 1);
                }
            }
		    return null;
		}

		#region ITreeModel Members

		public System.Collections.IEnumerable GetChildren(TreePath treePath)
		{
			Node node = FindNode(treePath);

            if (node != null)
            {
                foreach (Node n in node.Nodes)
                    yield return n;
            }
            else
                yield break;
		}

		public bool IsLeaf(TreePath treePath)
		{
			Node node = FindNode(treePath);
			if (node != null)
				return node.IsLeaf;
		
            throw new ArgumentException("treePath");
		}

		public event EventHandler<TreeModelEventArgs> NodesChanged;
		internal void OnNodesChanged(TreeModelEventArgs args)
		{
			if (NodesChanged != null)
				NodesChanged(this, args);
		}

		public event EventHandler<TreePathEventArgs> StructureChanged;
		public void OnStructureChanged(TreePathEventArgs args)
		{
			if (StructureChanged != null)
				StructureChanged(this, args);
		}

		public event EventHandler<TreeModelEventArgs> NodesInserted;
		internal void OnNodeInserted(Node parent, int index, Node node)
		{
			if (NodesInserted != null)
			{
				TreeModelEventArgs args = new TreeModelEventArgs(GetPath(parent), new int[] { index }, new object[] { node });
				NodesInserted(this, args);
			}

		}

		public event EventHandler<TreeModelEventArgs> NodesRemoved;
		internal void OnNodeRemoved(Node parent, int index, Node node)
		{
			if (NodesRemoved != null)
			{
				TreeModelEventArgs args = new TreeModelEventArgs(GetPath(parent), new int[] { index }, new object[] { node });
				NodesRemoved(this, args);
			}
		}

		#endregion
	}
}
