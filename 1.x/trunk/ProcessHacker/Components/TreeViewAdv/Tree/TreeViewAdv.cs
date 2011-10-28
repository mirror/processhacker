/*
 * Modified by fliser.
 * Modified by wj32.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Aga.Controls.Tree.NodeControls;
using ProcessHacker;
using ProcessHacker.Common;
using ProcessHacker.Native;


namespace Aga.Controls.Tree
{
	public sealed partial class TreeViewAdv : Control
	{
		private const int LeftMargin = 7;
		internal const int ItemDragSensivity = 4;
	    private const int DividerWidth = 9;

	    private Pen _linePen;
		private bool _suspendUpdate;
        private bool _completeSuspendUpdate; // Overrides my (wj32's) hacks
		private bool _needFullUpdate;
		private bool _fireSelectionEvent;
		private NodePlusMinus _plusMinus;
		private Control _currentEditor;
		private EditableControl _currentEditorOwner;
		private ToolTip _toolTip;
		private DrawContext _measureContext;
		private TreeColumn _hotColumn;
		private IncrementalSearch _search;
        private Stack<bool> _suspendedStack = new Stack<bool>();

		#region Public Events

		[Category("Behavior")]
		public event EventHandler<TreeNodeAdvMouseEventArgs> NodeMouseClick;
		private void OnNodeMouseClick(TreeNodeAdvMouseEventArgs args)
		{
			if (NodeMouseClick != null)
				NodeMouseClick(this, args);
		}

		[Category("Behavior")]
		public event EventHandler<TreeNodeAdvMouseEventArgs> NodeMouseDoubleClick;
		private void OnNodeMouseDoubleClick(TreeNodeAdvMouseEventArgs args)
		{
			if (NodeMouseDoubleClick != null)
				NodeMouseDoubleClick(this, args);
		}

		[Category("Behavior")]
		public event EventHandler<TreeColumnEventArgs> ColumnWidthChanged;
		internal void OnColumnWidthChanged(TreeColumn column)
        {
			if (ColumnWidthChanged != null)
				ColumnWidthChanged(this, new TreeColumnEventArgs(column));
		}

		[Category("Behavior")]
		public event EventHandler<TreeColumnEventArgs> ColumnReordered;
		internal void OnColumnReordered(TreeColumn column)
		{
            this.InvalidateNodeControlCache();

			if (ColumnReordered != null)
				ColumnReordered(this, new TreeColumnEventArgs(column));
		}

		[Category("Behavior")]
		public event EventHandler<TreeColumnEventArgs> ColumnClicked;
		internal void OnColumnClicked(TreeColumn column)
		{
			if (ColumnClicked != null)
				ColumnClicked(this, new TreeColumnEventArgs(column));
		}

		[Category("Behavior")]
		public event EventHandler SelectionChanged;
		internal void OnSelectionChanged()
		{
			if (SuspendSelectionEvent)
				_fireSelectionEvent = true;
			else
			{
				_fireSelectionEvent = false;
				if (SelectionChanged != null)
					SelectionChanged(this, EventArgs.Empty);
			}
		}

		[Category("Behavior")]
		public event EventHandler<TreeViewAdvEventArgs> Collapsing;
		private void OnCollapsing(TreeNodeAdv node)
		{
			if (Collapsing != null)
				Collapsing(this, new TreeViewAdvEventArgs(node));
		}

		[Category("Behavior")]
		public event EventHandler<TreeViewAdvEventArgs> Collapsed;
		private void OnCollapsed(TreeNodeAdv node)
        {
            this.InvalidateNodeControlCache();

			if (Collapsed != null)
				Collapsed(this, new TreeViewAdvEventArgs(node));
		}

		[Category("Behavior")]
		public event EventHandler<TreeViewAdvEventArgs> Expanding;
		private void OnExpanding(TreeNodeAdv node)
		{
			if (Expanding != null)
				Expanding(this, new TreeViewAdvEventArgs(node));
		}

		[Category("Behavior")]
		public event EventHandler<TreeViewAdvEventArgs> Expanded;
		private void OnExpanded(TreeNodeAdv node)
        {
            this.InvalidateNodeControlCache();

			if (Expanded != null)
				Expanded(this, new TreeViewAdvEventArgs(node));
		}

        [Category("Behavior")]
        public event EventHandler GridLineStyleChanged;
		private void OnGridLineStyleChanged()
        {
			if (GridLineStyleChanged != null)
				GridLineStyleChanged(this, EventArgs.Empty);
        }

		#endregion

		public TreeViewAdv()
		{
			InitializeComponent();

			this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.Selectable | ControlStyles.CacheText, true);

            if (OSVersion.IsBelow(WindowsVersion.Vista))
            {
                this.ColumnHeaderHeight = ExplorerVisualStyle.VisualStylesEnabled ? 20 : 17;
            }
            else
            {
                this.ColumnHeaderHeight = ExplorerVisualStyle.VisualStylesEnabled ? 25 : 17;
            }

			//BorderStyle = BorderStyle.Fixed3D;
			_hScrollBar.Height = SystemInformation.HorizontalScrollBarHeight;
			_vScrollBar.Width = SystemInformation.VerticalScrollBarWidth;

			_rowLayout = new FixedRowHeightLayout(this, RowHeight);
			this.RowMap = new List<TreeNodeAdv>();
			this.Selection = new List<TreeNodeAdv>();
			this.SelectedNodes = new ReadOnlyCollection<TreeNodeAdv>(this.Selection);
			this.Columns = new TreeColumnCollection(this);
			_toolTip = new ToolTip();
			
			_measureContext = new DrawContext 
            {
                Font = this.Font, 
                Graphics = Graphics.FromImage(new Bitmap(1, 1))
            };

		    this.Input = new NormalInputState(this);
			_search = new IncrementalSearch(this);

			CreateNodes();
			CreatePens();

			ArrangeControls();

			_plusMinus = new NodePlusMinus(this);
			this.NodeControls = new NodeControlsCollection(this);

            Font = _font;
		}

		#region Public Methods

		public TreePath GetPath(TreeNodeAdv node)
		{
			if (node == this.Root)
				return TreePath.Empty;
		    
            Stack<object> stack = new Stack<object>();
		    
            while (node != this.Root && node != null)
		    {
		        stack.Push(node.Tag);

		        node = node.Parent;
		    }

		    return new TreePath(stack.ToArray());
		}

		public TreeNodeAdv GetNodeAt(Point point)
		{
			NodeControlInfo info = GetNodeControlInfoAt(point);
			return info.Node;
		}

		public NodeControlInfo GetNodeControlInfoAt(Point point)
		{
			if (point.X < 0 || point.Y < 0)
				return NodeControlInfo.Empty;

			int row = _rowLayout.GetRowAt(point);
			
            if (row < RowCount && row >= 0)
				return GetNodeControlInfoAt(this.RowMap[row], point);
				
            return NodeControlInfo.Empty;
		}

		private NodeControlInfo GetNodeControlInfoAt(TreeNodeAdv node, Point point)
		{
		    Rectangle rect = _rowLayout.GetRowBounds(FirstVisibleRow);
		    point.Y += (rect.Y - this.ColumnHeaderHeight);
		    point.X += OffsetX;

		    foreach (NodeControlInfo info in GetNodeControls(node))
		    {
		        if (info.Bounds.Contains(point))
		            return info;
		    }

		    return new NodeControlInfo(null, Rectangle.Empty, node);
		}

	    public void BeginUpdate()
		{
            _suspendedStack.Push(_suspendUpdate);
			_suspendUpdate = true;
			//SuspendSelectionEvent = true;
		}

        public void BeginCompleteUpdate()
        {
            _completeSuspendUpdate = true;
        }

		public void EndUpdate()
		{
			_suspendUpdate = _suspendedStack.Pop();

			if (_needFullUpdate && !_completeSuspendUpdate)
				FullUpdate();
			else
				UpdateView();
			//SuspendSelectionEvent = false;
		}

        public void EndCompleteUpdate()
        {
            _completeSuspendUpdate = false;
            FullUpdate();
        }

		public void ExpandAll()
		{
			this.Root.ExpandAll();
		}

		public void CollapseAll()
		{
			this.Root.CollapseAll();
		}

		/// <summary>
		/// Expand all parent nodes and scroll to the specified node
		/// </summary>
		public void EnsureVisible(TreeNodeAdv node)
		{
			if (node == null)
				throw new ArgumentNullException("node");

			if (!IsMyNode(node))
				throw new ArgumentException();

			TreeNodeAdv parent = node.Parent;
			
            while (parent != this.Root)
			{
				parent.IsExpanded = true;
				parent = parent.Parent;
			}

			ScrollTo(node);
		}

		/// <summary>
		/// Make node visible, scroll if needed. All parent nodes of the specified node must be expanded
		/// </summary>
		/// <param name="node"></param>
		public void ScrollTo(TreeNodeAdv node)
		{
			if (node == null)
				throw new ArgumentNullException("node");

			if (!IsMyNode(node))
				throw new ArgumentException();

			if (node.Row < 0)
				CreateRowMap();

            int row = FirstVisibleRow;

            if (node.Row < FirstVisibleRow)
            {
                row = node.Row;
            }
            else
            {
                int pageStart = _rowLayout.GetRowBounds(FirstVisibleRow).Top;
                int rowBottom = _rowLayout.GetRowBounds(node.Row).Bottom;
                if (rowBottom > pageStart + DisplayRectangle.Height - this.ColumnHeaderHeight)
                    row = _rowLayout.GetFirstRow(node.Row);
            }

            // wj32: Do the best we can, so don't bail out if the value is out of range.
            if (row < _vScrollBar.Minimum)
                row = _vScrollBar.Minimum;
            if (row > _vScrollBar.Maximum)
                row = _vScrollBar.Maximum;

            _vScrollBar.Value = row;
        }

        public void ScrollTo2(TreeNodeAdv node)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            if (!IsMyNode(node))
                throw new ArgumentException();

            if (node.Row < 0)
                CreateRowMap();

            int row = -1;

            if (node.Row < FirstVisibleRow)
            {
                row = node.Row;
                // Ugh, who wants the node at the TOP of the screen? Put it in the MIDDLE!
                row -= (this.Height / this.RowHeight) / 2;
            }
            else
            {
                int pageStart = _rowLayout.GetRowBounds(FirstVisibleRow).Top;
                int rowBottom = _rowLayout.GetRowBounds(node.Row).Bottom;
                if (rowBottom > pageStart + DisplayRectangle.Height - this.ColumnHeaderHeight)
                    row = _rowLayout.GetFirstRow(node.Row);

                // Ugh, who wants the node at the BOTTOM of the screen? Put it in the MIDDLE!
                row += (this.Height / this.RowHeight) / 2;
            }
            // wj32: Do the best we can, so don't bail out if the value is out of range.
            if (row < _vScrollBar.Minimum)
                row = _vScrollBar.Minimum;
            if (row > _vScrollBar.Maximum)
                row = _vScrollBar.Maximum;

            _vScrollBar.Value = row;
        }

		public void ClearSelection()
		{
			BeginUpdate();

			try
			{
				ClearSelectionInternal();
			}
			finally
			{
				EndUpdate();
			}
		}

		internal void ClearSelectionInternal()
		{
			while (this.Selection.Count > 0)
				this.Selection[0].IsSelected = false;
		}

		#endregion

		protected override void OnSizeChanged(EventArgs e)
		{
			ArrangeControls();
			SafeUpdateScrollBars();

			base.OnSizeChanged(e);

            this.Invalidate();
		}

		private void ArrangeControls()
		{
			int hBarSize = _hScrollBar.Height;
			int vBarSize = _vScrollBar.Width;
			Rectangle clientRect = ClientRectangle;
			
			_hScrollBar.SetBounds(clientRect.X, clientRect.Bottom - hBarSize,
				clientRect.Width - vBarSize, hBarSize);

			_vScrollBar.SetBounds(clientRect.Right - vBarSize, clientRect.Y,
				vBarSize, clientRect.Height - hBarSize);
		}

		private void SafeUpdateScrollBars()
		{
            this.UpdateScrollBars();
		}

		private void UpdateScrollBars()
		{
			UpdateVScrollBar();
			UpdateHScrollBar();
			UpdateVScrollBar();
			UpdateHScrollBar();
			_hScrollBar.Width = DisplayRectangle.Width;
			_vScrollBar.Height = DisplayRectangle.Height;
		}

		private void UpdateHScrollBar()
		{
			_hScrollBar.Maximum = this.ContentWidth;
			_hScrollBar.LargeChange = Math.Max(DisplayRectangle.Width, 0);
			_hScrollBar.SmallChange = 5;
			_hScrollBar.Visible = _hScrollBar.LargeChange < _hScrollBar.Maximum;
			_hScrollBar.Value = Math.Min(_hScrollBar.Value, _hScrollBar.Maximum - _hScrollBar.LargeChange + 1);
		}

		private void UpdateVScrollBar()
		{
			_vScrollBar.Maximum = Math.Max(RowCount - 1, 0);
			_vScrollBar.LargeChange = _rowLayout.PageRowCount;
			_vScrollBar.Visible = (RowCount > 0) && (_vScrollBar.LargeChange <= _vScrollBar.Maximum);
			_vScrollBar.Value = Math.Min(_vScrollBar.Value, _vScrollBar.Maximum - _vScrollBar.LargeChange + 1);
		}

		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams res = base.CreateParams;

				switch (this.BorderStyle)
				{
                    case BorderStyle.FixedSingle:
				        {
				            res.Style |= 0x800000;
				            break;
				        }
                    case BorderStyle.Fixed3D:
				        {
				            res.ExStyle |= 0x20000;
				            break;
				        }
				}
				return res;
			}
		}

		protected override void OnGotFocus(EventArgs e)
		{
			HideEditor();
			UpdateView();
			ChangeInput();

			base.OnGotFocus(e);
		}

		protected override void OnLeave(EventArgs e)
		{
			if (_currentEditorOwner != null)
				_currentEditorOwner.ApplyChanges();
			
            HideEditor();
			UpdateView();

			base.OnLeave(e);
		}

		protected override void OnFontChanged(EventArgs e)
		{
			base.OnFontChanged(e);

			_measureContext.Font = Font;
			FullUpdate();
        }

        private Dictionary<TreeNodeAdv, List<NodeControlInfo>> _cachedNodeControls = new Dictionary<TreeNodeAdv, List<NodeControlInfo>>();

        internal IEnumerable<NodeControlInfo> GetNodeControls(TreeNodeAdv node)
        {
            if (node == null)
                 return new List<NodeControlInfo>();

            if (!_cachedNodeControls.ContainsKey(node))
            {
                List<NodeControlInfo> ncList = new List<NodeControlInfo>();

                foreach (NodeControlInfo item in this.GetNodeControlsInternal(node))
                {
                    ncList.Add(item);
                }

                _cachedNodeControls.Add(node, ncList);
            }

            return _cachedNodeControls[node];
        }

        public void InvalidateNodeControlCache()
        {
            _cachedNodeControls.Clear();
        }

		private IEnumerable<NodeControlInfo> GetNodeControlsInternal(TreeNodeAdv node)
		{
			if (node == null)
				yield break;

			Rectangle rowRect = _rowLayout.GetRowBounds(node.Row);

            foreach (NodeControlInfo n in GetNodeControls(node, rowRect))
            {
                yield return n;
            }
		}

		internal IEnumerable<NodeControlInfo> GetNodeControls(TreeNodeAdv node, Rectangle rowRect)
		{
		    if (node == null)
		        yield break;

		    int y = rowRect.Y;
		    int x = (node.Level - 1)*_indent + LeftMargin;
		    int width;
		    Rectangle rect;

		    if (ShowPlusMinus)
		    {
		        width = _plusMinus.GetActualSize(node, _measureContext).Width;
		        rect = new Rectangle(x, y, width, rowRect.Height);

		        if (this.Columns.Count > 0 && this.Columns[0].Width < rect.Right)
		            rect.Width = this.Columns[0].Width - x;

		        yield return new NodeControlInfo(_plusMinus, rect, node);
		        
                x += width;
		    }


		    int right = 0;

		    foreach (TreeColumn col in this.Columns)
		    {
		        if (!col.IsVisible || col.Width <= 0)
		            continue;

		        right += col.Width;

		        for (int i = 0; i < this.NodeControls.Count; i++)
		        {
		            NodeControl nc = this.NodeControls[i];
		           
                    if (nc.ParentColumn != col)
		                continue;

		            Size s = nc.GetActualSize(node, _measureContext);

		            if (s.IsEmpty)
		                continue;

		            bool isLastControl = true;
		          
                    for (int k = i + 1; k < this.NodeControls.Count; k++)
		            {
		                if (this.NodeControls[k].ParentColumn == col)
		                {
		                    isLastControl = false;
		                    break;
		                }
		            }

		            width = right - x;
		          
                    if (!isLastControl)
		                width = s.Width;
		          
                    int maxWidth = Math.Max(0, right - x);
		            rect = new Rectangle(x, y, Math.Min(maxWidth, width), rowRect.Height);
		            x += width;

		            yield return new NodeControlInfo(nc, rect, node);
		        }
		        x = right;
		    }
		}

	    internal static double Dist(Point p1, Point p2)
		{
			return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
		}

		public void FullUpdate()
		{
            UnsafeFullUpdate();
		}

        public void RefreshVisualStyles()
        {
            _plusMinus.RefreshVisualStyles();
        }

		private void UnsafeFullUpdate()
		{
			_rowLayout.ClearCache();
			CreateRowMap();
			SafeUpdateScrollBars();
			UpdateView();
			_needFullUpdate = false;
		}

		internal void UpdateView()
		{
			if (!_suspendUpdate)
				this.Invalidate(false);
		}

		internal void UpdateHeaders()
		{
            this.Invalidate(new Rectangle(0, 0, this.Width, this.ColumnHeaderHeight));
		}

		internal void UpdateColumns()
		{
            this.FullUpdate();
		}

		private void CreateNodes()
		{
			this.Selection.Clear();
			this.SelectionStart = null;
			this.Root = new TreeNodeAdv(this, null);
			this.Root.IsExpanded = true;
			
            if (this.Root.Nodes.Count > 0)
				this.CurrentNode = this.Root.Nodes[0];
			else
				this.CurrentNode = null;
		}

		internal void ReadChilds(TreeNodeAdv parentNode)
		{
            this.ReadChilds(parentNode, false);
		}

		internal void ReadChilds(TreeNodeAdv parentNode, bool performFullUpdate)
		{
			if (!parentNode.IsLeaf)
			{
				parentNode.IsExpandedOnce = true;
				List<TreeNodeAdv> oldNodes = new List<TreeNodeAdv>(parentNode.Nodes);
				parentNode.Nodes.Clear();

				if (Model != null)
				{
					IEnumerable items = Model.GetChildren(GetPath(parentNode));
					if (items != null)
						foreach (object obj in items)
						{
							bool found = false;
							if (obj != null)
							{
								for (int i = 0; i < oldNodes.Count; i++)
									if (obj == oldNodes[i].Tag)
									{
										oldNodes[i].RightBounds = oldNodes[i].Height = null;
										AddNode(parentNode, -1, oldNodes[i]);
										oldNodes.RemoveAt(i);
										found = true;
										break;
									}
							}
							if (!found)
								AddNewNode(parentNode, obj, -1);

							if (performFullUpdate)
								FullUpdate();
						}
				}

			}
		}

		private void AddNewNode(TreeNodeAdv parent, object tag, int index)
		{
			TreeNodeAdv node = new TreeNodeAdv(this, tag);
			AddNode(parent, index, node);
		}

		private void AddNode(TreeNodeAdv parent, int index, TreeNodeAdv node)
		{
			if (index >= 0 && index < parent.Nodes.Count)
				parent.Nodes.Insert(index, node);
			else
				parent.Nodes.Add(node);

			node.IsLeaf = Model.IsLeaf(GetPath(node));
			if (node.IsLeaf)
				node.Nodes.Clear();
			if (!this.LoadOnDemand || node.IsExpandedOnce)
				ReadChilds(node);

            this.InvalidateNodeControlCache();
		}

		private struct ExpandArgs
		{
			public TreeNodeAdv Node;
			public bool Value;
			public bool IgnoreChildren;
		}

        internal void SetIsExpanded(TreeNodeAdv node, bool value, bool ignoreChildren)
        {
            ExpandArgs eargs = new ExpandArgs {Node = node, Value = value, IgnoreChildren = ignoreChildren};

            SetIsExpanded(eargs);
        }

		private void SetIsExpanded(ExpandArgs eargs)
		{
		    bool update = !eargs.IgnoreChildren;

		    if (update)
		        BeginUpdate();

		    if (IsMyNode(eargs.Node) && eargs.Node.IsExpanded != eargs.Value)
		        SetIsExpanded(eargs.Node, eargs.Value);

		    if (!eargs.IgnoreChildren)
		        SetIsExpandedRecursive(eargs.Node, eargs.Value);

		    if (update)
		        EndUpdate();
		}

	    internal void SetIsExpanded(TreeNodeAdv node, bool value)
		{
			if (this.Root == node && !value)
				return; //Can't collapse root node

			if (value)
				OnExpanding(node);
			else
				OnCollapsing(node);

			if (value && !node.IsExpandedOnce)
			{
				if (this.LoadOnDemand)
				{
					node.AssignIsExpanded(true);
					Invalidate();
				}
				ReadChilds(node);
			}

			node.AssignIsExpanded(value);
			SmartFullUpdate();

			if (value)
				OnExpanded(node);
			else
				OnCollapsed(node);
		}

		internal void SetIsExpandedRecursive(TreeNodeAdv root, bool value)
		{
		    foreach (TreeNodeAdv node in root.Nodes)
		    {
		        node.IsExpanded = value;
		        SetIsExpandedRecursive(node, value);
		    }
		}

	    private void CreateRowMap()
	    {
	        this.RowMap.Clear();
	        int row = 0;
	        this.ContentWidth = 0;
	      
            foreach (TreeNodeAdv node in VisibleNodes)
	        {
	            node.Row = row;

	            this.RowMap.Add(node);

	            row++;
	        }

	        this.ContentWidth = 0;
         
            foreach (TreeColumn col in this.Columns)
            {
                if (col.IsVisible)
                    this.ContentWidth += col.Width;
            }
	    }

	    internal Rectangle GetNodeBounds(TreeNodeAdv node)
		{
			return GetNodeBounds(GetNodeControls(node));
		}

		private static Rectangle GetNodeBounds(IEnumerable<NodeControlInfo> nodeControls)
		{
			Rectangle res = Rectangle.Empty;

			foreach (NodeControlInfo info in nodeControls)
			{
				res = res == Rectangle.Empty ? info.Bounds : Rectangle.Union(res, info.Bounds);
			}
			return res;
		}

		private void _vScrollBar_ValueChanged(object sender, EventArgs e)
		{
			FirstVisibleRow = _vScrollBar.Value;
            this.Invalidate();
		}

		private void _hScrollBar_ValueChanged(object sender, EventArgs e)
		{
            OffsetX = _hScrollBar.Value;
            this.Invalidate();
		}

		internal void SmartFullUpdate()
		{
			if (_suspendUpdate)
				_needFullUpdate = true;
			else
				FullUpdate();
		}

		internal bool IsMyNode(TreeNodeAdv node)
		{
			if (node == null)
				return false;

			if (node.Tree != this)
				return false;

			while (node.Parent != null)
				node = node.Parent;

			return node == this.Root;
		}

		private void UpdateSelection()
		{
			bool flag = false;

			if (!IsMyNode(this.CurrentNode))
				this.CurrentNode = null;
			if (!IsMyNode(this.SelectionStart))
				this.SelectionStart = null;

			for (int i = this.Selection.Count - 1; i >= 0; i--)
				if (!IsMyNode(this.Selection[i]))
				{
					flag = true;
					this.Selection.RemoveAt(i);
				}

			if (flag)
                OnSelectionChanged();
		}

		internal void ChangeColumnWidth(TreeColumn column)
		{
			if (!(this.Input is ResizeColumnState))
			{
				FullUpdate();
				OnColumnWidthChanged(column);
			}
		}

		public TreeNodeAdv FindNode(TreePath path)
		{
			return FindNode(path, false);
		}

		public TreeNodeAdv FindNode(TreePath path, bool readChilds)
		{
			if (path.IsEmpty())
				return this.Root;
			
            return FindNode(this.Root, path, 0, readChilds);
		}

		private TreeNodeAdv FindNode(TreeNodeAdv root, TreePath path, int level, bool readChilds)
		{
			if (!root.IsExpandedOnce && readChilds)
				ReadChilds(root);

			foreach (TreeNodeAdv node in root.Nodes)
			{
			    if (node.Tag != path.FullPath[level]) 
                    continue;
			    
                if (level == path.FullPath.Length - 1)
			        return node;
				    
			    return FindNode(node, path, level + 1, readChilds);
			}
			return null;
		}

		public TreeNodeAdv FindNodeByTag(object tag)
		{
			return FindNodeByTag(this.Root, tag);
		}

		private static TreeNodeAdv FindNodeByTag(TreeNodeAdv root, object tag)
		{
			foreach (TreeNodeAdv node in root.Nodes)
			{
				if (node.Tag == tag)
					return node;
				
                TreeNodeAdv res = FindNodeByTag(node, tag);
				
                if (res != null)
					return res;
			}

			return null;
		}

		#region Editor

		public void DisplayEditor(Control control, EditableControl owner)
		{
			if (control == null || owner == null)
				throw new ArgumentNullException();

			if (this.CurrentNode != null)
			{
				HideEditor();
				_currentEditor = control;
				_currentEditorOwner = owner;
				UpdateEditorBounds();

				UpdateView();
				control.Parent = this;
				control.Focus();
				owner.UpdateEditor(control);
			}
		}

		public void UpdateEditorBounds()
		{
			if (_currentEditor != null)
			{
				EditorContext context = new EditorContext
				{
				    Owner = this._currentEditorOwner, 
                    CurrentNode = this.CurrentNode, 
                    Editor = this._currentEditor, 
                    DrawContext = this._measureContext
				};

			    SetEditorBounds(context);
			}
		}

		public void HideEditor()
		{
			if (_currentEditorOwner != null)
			{
				_currentEditorOwner.HideEditor(_currentEditor);
				_currentEditor = null;
				_currentEditorOwner = null;
			}
		}

		private void SetEditorBounds(EditorContext context)
		{
			foreach (NodeControlInfo info in GetNodeControls(context.CurrentNode))
			{
				if (context.Owner == info.Control && info.Control is EditableControl)
				{
					Point p = info.Bounds.Location;
					p.X += info.Control.LeftMargin;
					p.X -= OffsetX;
					p.Y -= (_rowLayout.GetRowBounds(FirstVisibleRow).Y - this.ColumnHeaderHeight);
					int width = DisplayRectangle.Width - p.X;
					
                    if ( info.Control.ParentColumn != null && this.Columns.Contains(info.Control.ParentColumn))
					{
						Rectangle rect = GetColumnBounds(info.Control.ParentColumn.Index);
						width = rect.Right - OffsetX - p.X;
					}

					context.Bounds = new Rectangle(p.X, p.Y, width, info.Bounds.Height);

					((EditableControl)info.Control).SetEditorBounds(context);
					return;
				}
			}
		}

		private Rectangle GetColumnBounds(int column)
		{
			int x = 0;
			for (int i = 0; i < this.Columns.Count; i++)
			{
				if (this.Columns[i].IsVisible)
				{
					if (i < column)
						x += this.Columns[i].Width;
					else
						return new Rectangle(x, 0, this.Columns[i].Width, 0);
				}
			}
			return Rectangle.Empty;
		}

		#endregion

		#region ModelEvents
		private void BindModelEvents()
		{
			_model.NodesChanged += _model_NodesChanged;
			_model.NodesInserted += _model_NodesInserted;
			_model.NodesRemoved += _model_NodesRemoved;
			_model.StructureChanged += _model_StructureChanged;
		}

		private void UnbindModelEvents()
		{
			_model.NodesChanged -= _model_NodesChanged;
			_model.NodesInserted -= _model_NodesInserted;
			_model.NodesRemoved -= _model_NodesRemoved;
			_model.StructureChanged -= _model_StructureChanged;
		}

		private void _model_StructureChanged(object sender, TreePathEventArgs e)
		{
			if (e.Path == null)
				throw new ArgumentNullException();

			TreeNodeAdv node = FindNode(e.Path);
		    
            if (node == null) 
                return;
		    
            ReadChilds(node);
		    UpdateSelection();

		    if (_completeSuspendUpdate) 
                return;
		    
            this.FullUpdate();
		    this.Invalidate();
		    //else 
			//	throw new ArgumentException("Path not found");
		}

		private void _model_NodesRemoved(object sender, TreeModelEventArgs e)
		{
			TreeNodeAdv parent = FindNode(e.Path);
			if (parent != null)
			{
				if (e.Indices != null)
				{
					List<int> list = new List<int>(e.Indices);
					
                    list.Sort();
					
                    for (int n = list.Count - 1; n >= 0; n--)
					{
						int index = list[n];
						
                        if (index >= 0 && index <= parent.Nodes.Count)
							parent.Nodes.RemoveAt(index);
						else
							throw new ArgumentOutOfRangeException("Index out of range");
					}
				}
				else
				{
					for (int i = parent.Nodes.Count - 1; i >= 0; i--)
					{
                        for (int n = 0; n < e.Children.Length; n++)
                        {
                            if (parent.Nodes[i].Tag == e.Children[n])
                            {
                                parent.Nodes.RemoveAt(i);
                                break;
                            }
                        }
					}
				}
			}

			UpdateSelection();
			SmartFullUpdate();
		}

		private void _model_NodesInserted(object sender, TreeModelEventArgs e)
		{
			if (e.Indices == null)
				throw new ArgumentNullException("Indices");

			TreeNodeAdv parent = FindNode(e.Path);
			if (parent != null)
			{
				for (int i = 0; i < e.Children.Length; i++)
					AddNewNode(parent, e.Children[i], e.Indices[i]);
			}
			SmartFullUpdate();
		}

		private void _model_NodesChanged(object sender, TreeModelEventArgs e)
		{
			TreeNodeAdv parent = FindNode(e.Path);
			if (parent != null && parent.IsVisible  && parent.IsExpanded)
			{
				if (InvokeRequired)
					this.BeginInvoke(new UpdateContentWidthDelegate(ClearNodesSize), e, parent);
				else
					ClearNodesSize(e, parent);

				SmartFullUpdate();
			}
		}

		private delegate void UpdateContentWidthDelegate(TreeModelEventArgs e, TreeNodeAdv parent);
		private static void ClearNodesSize(TreeModelEventArgs e, TreeNodeAdv parent)
		{
			if (e.Indices != null)
			{
				foreach (int index in e.Indices)
				{
					if (index >= 0 && index < parent.Nodes.Count)
					{
						TreeNodeAdv node = parent.Nodes[index];
						node.Height = node.RightBounds = null;
					}
					else
						throw new ArgumentOutOfRangeException("Index out of range");
				}
			}
			else
			{
				foreach (TreeNodeAdv node in parent.Nodes)
				{
                    foreach (object obj in e.Children)
                    {
                        if (node.Tag == obj)
                        {
                            node.Height = node.RightBounds = null;
                        }
                    }
				}
			}
		}
		#endregion
	}
}