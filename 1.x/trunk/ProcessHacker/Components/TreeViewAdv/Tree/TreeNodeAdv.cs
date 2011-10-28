/*
 * Modified by wj32.
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Aga.Controls.Tree
{
	[Serializable]
	public sealed class TreeNodeAdv : ISerializable
	{
        static TreeNodeAdv()
        {
            StateColors.Add(NodeState.Normal, SystemColors.Window);
            StateColors.Add(NodeState.New, Color.Green);
            StateColors.Add(NodeState.Removed, Color.Red);
        }

		#region NodeCollection
		private class NodeCollection : Collection<TreeNodeAdv>
		{
			private TreeNodeAdv _owner;

			public NodeCollection(TreeNodeAdv owner)
			{
				_owner = owner;
			}

			protected override void ClearItems()
			{
				while (this.Count != 0)
					this.RemoveAt(this.Count - 1);
			}

			protected override void InsertItem(int index, TreeNodeAdv item)
			{
				if (item == null)
					throw new ArgumentNullException("item");

				if (item.Parent != _owner)
				{
					if (item.Parent != null)
						item.Parent.Nodes.Remove(item);
					item._parent = _owner;
					item._index = index;
					for (int i = index; i < Count; i++)
						this[i]._index++;
					base.InsertItem(index, item);
				}
			}

			protected override void RemoveItem(int index)
			{
				TreeNodeAdv item = this[index];
				item._parent = null;
				item._index = -1;
				for (int i = index + 1; i < Count; i++)
					this[i]._index--;
				base.RemoveItem(index);
			}

			protected override void SetItem(int index, TreeNodeAdv item)
			{
				if (item == null)
					throw new ArgumentNullException("item");
				RemoveAt(index);
				InsertItem(index, item);
			}
		}
		#endregion

		#region Properties

	    internal TreeViewAdv Tree { get; private set; }

	    private int _row;
		internal int Row
		{
			get { return _row; }
			set { _row = value; }
		}

		private int _index = -1;
		public int Index
		{
			get
			{
				return _index;
			}
        }

        public enum NodeState
        {
            Normal, New, Removed
        }

        public static Dictionary<NodeState, Color> StateColors = new Dictionary<NodeState, Color>();

        private NodeState _state = NodeState.Normal;

        public NodeState State
        {
            get { return _state; }
            set
            {
                _state = value;
                if (_automaticForeColor)
                    _autoForeColor = GetForeColor(this.BackColor);
            }
        }

        private Color _backColor = Color.White;
        public Color BackColor
        {
            get
            {
                if (_state != NodeState.Normal)
                    return StateColors[_state];

                return _backColor;
            }
            set
            {
                _backColor = value;

                if (_automaticForeColor)
                    _autoForeColor = GetForeColor(this.BackColor);
            }
        }

        private Color _autoForeColor = Color.Black;
        private Color _foreColor = SystemColors.ControlText;
        public Color ForeColor
        {
            get
            {
                if (_automaticForeColor)
                    return _autoForeColor;
                else
                    return _foreColor;
            }
            set { _foreColor = value; }
        }

        private bool _automaticForeColor = true;
        public bool AutomaticForeColor
        {
            get { return _automaticForeColor; }
            set
            {
                _automaticForeColor = value;
                if (_automaticForeColor)
                    _autoForeColor = GetForeColor(this.BackColor);
            }
        }

        public static Color GetForeColor(Color color)
        {
            if (color.GetBrightness() > 0.4)
                return Color.Black;
            else
                return Color.White;
        }

		private bool _isSelected;
		public bool IsSelected
		{
			get { return _isSelected; }
			set 
			{
				if (_isSelected != value)
				{
					if (this.Tree.IsMyNode(this))
					{
						//_tree.OnSelectionChanging
						if (value)
						{
							if (!this.Tree.Selection.Contains(this))
								this.Tree.Selection.Add(this);

							if (this.Tree.Selection.Count == 1)
								this.Tree.CurrentNode = this;
						}
						else
							this.Tree.Selection.Remove(this);
						this.Tree.UpdateView();
						this.Tree.OnSelectionChanged();
					}
					_isSelected = value;
				}
			}
		}

		/// <summary>
		/// Returns true if all parent nodes of this node are expanded.
		/// </summary>
		internal bool IsVisible
		{
			get
			{
				TreeNodeAdv node = _parent;
				while (node != null)
				{
					if (!node.IsExpanded)
						return false;
					node = node.Parent;
				}
				return true;
			}
		}

		private bool _isLeaf;
		public bool IsLeaf
		{
			get { return _isLeaf; }
			internal set { _isLeaf = value; }
		}

		private bool _isExpandedOnce;
		public bool IsExpandedOnce
		{
			get { return _isExpandedOnce; }
			internal set { _isExpandedOnce = value; }
		}

		private bool _isExpanded;
		public bool IsExpanded
		{
            get { return _isExpanded; }
            set 
			{
				if (value)
					Expand();
				else
					Collapse();
			}
		}

		internal void AssignIsExpanded(bool value)
		{
			_isExpanded = value;
		}

		private TreeNodeAdv _parent;
		public TreeNodeAdv Parent
		{
			get { return _parent; }
		}

		public int Level
		{
			get
			{
				if (_parent == null)
					return 0;
				else
					return _parent.Level + 1;
			}
		}

		public TreeNodeAdv NextNode
		{
			get
			{
				if (_parent != null)
				{
					int index = Index;
					if (index < _parent.Nodes.Count - 1)
						return _parent.Nodes[index + 1];
				}
				return null;
			}
		}

		internal TreeNodeAdv BottomNode
		{
			get
			{
				TreeNodeAdv parent = this.Parent;
				if (parent != null)
				{
					if (parent.NextNode != null)
						return parent.NextNode;
					else
						return parent.BottomNode;
				}
				return null;
			}
		}

		internal TreeNodeAdv NextVisibleNode
		{
			get
			{
				if (IsExpanded && Nodes.Count > 0)
					return Nodes[0];
				else
				{
					TreeNodeAdv nn = NextNode;
					if (nn != null)
						return nn;
					else
						return BottomNode;
				}
			}
		}

		public bool CanExpand
		{
			get
			{
				return (Nodes.Count > 0 || (!IsExpandedOnce && !IsLeaf));
			}
		}

		private object _tag;
		public object Tag
		{
			get { return _tag; }
		}

		private Collection<TreeNodeAdv> _nodes;
		internal Collection<TreeNodeAdv> Nodes
		{
			get { return _nodes; }
		}

		private ReadOnlyCollection<TreeNodeAdv> _children;
		public ReadOnlyCollection<TreeNodeAdv> Children
		{
			get
			{
				return _children;
			}
		}

		private int? _rightBounds;
		internal int? RightBounds
		{
			get { return _rightBounds; }
			set { _rightBounds = value; }
		}

		private int? _height;
		internal int? Height
		{
			get { return _height; }
			set { _height = value; }
		}

		private bool _isExpandingNow;
		internal bool IsExpandingNow
		{
			get { return _isExpandingNow; }
			set { _isExpandingNow = value; }
		}

		#endregion

        public TreeNodeAdv(object tag): this(null, tag)
		{
		}

		internal TreeNodeAdv(TreeViewAdv tree, object tag)
		{
			_row = -1;
			this.Tree = tree;
			_nodes = new NodeCollection(this);
			_children = new ReadOnlyCollection<TreeNodeAdv>(_nodes);
			_tag = tag;
        }

        public override string ToString()
        {
            if (Tag != null)
				return Tag.ToString();
            
            return base.ToString();
        }

	    public void Collapse()
        {
			if (_isExpanded)
				Collapse(true);
        }

        public void CollapseAll()
        {
            Collapse(false);
        }

        public void Collapse(bool ignoreChildren)
        {
			SetIsExpanded(false, ignoreChildren);
        }

        public void EnsureVisible()
        {
            TreeNodeAdv parent = this.Parent;

            while (parent != this.Tree.Root)
            {
                parent.Expand();
                parent = parent.Parent;
            }

            this.Tree.ScrollTo(this);
        }

        public void EnsureVisible2()
        {
            TreeNodeAdv parent = this.Parent;

            while (parent != this.Tree.Root)
            {
                parent.Expand();
                parent = parent.Parent;
            }

            this.Tree.ScrollTo2(this);
        }

		public void Expand()
		{
			if (!_isExpanded)
				Expand(true);
		}

		public void ExpandAll()
		{
			Expand(false);
		}

		public void Expand(bool ignoreChildren)
        {
			SetIsExpanded(true, ignoreChildren);
		}

		private void SetIsExpanded(bool value, bool ignoreChildren)
		{
			if (this.Tree == null)
			{
				_isExpanded = value;
				if (!ignoreChildren)
					this.Tree.SetIsExpandedRecursive(this, value);
			}
			else
				this.Tree.SetIsExpanded(this, value, ignoreChildren);
		}

		#region ISerializable Members

        private TreeNodeAdv(SerializationInfo info, StreamingContext context): this(null, null)
        {
			int nodesCount = 0;
			nodesCount = info.GetInt32("NodesCount");
			_isExpanded = info.GetBoolean("IsExpanded");
			_tag = info.GetValue("Tag", typeof(object));

			for (int i = 0; i < nodesCount; i++)
			{
				TreeNodeAdv child = (TreeNodeAdv)info.GetValue("Child" + i, typeof(TreeNodeAdv));
				Nodes.Add(child);
			}

        }

		[SecurityPermission(SecurityAction.Demand, SerializationFormatter=true)]
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("IsExpanded", IsExpanded);
			info.AddValue("NodesCount", Nodes.Count);
			if ((Tag != null) && Tag.GetType().IsSerializable)
				info.AddValue("Tag", Tag, Tag.GetType());

			for (int i = 0; i < Nodes.Count; i++)
				info.AddValue("Child" + i, Nodes[i], typeof(TreeNodeAdv));

		}

		#endregion
	}
}
