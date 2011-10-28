using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using System.Drawing.Design;

using Aga.Controls.Tree.NodeControls;

namespace Aga.Controls.Tree
{
	public sealed partial class TreeViewAdv
	{
		private Cursor _innerCursor;

		public override Cursor Cursor
		{
			get
			{
                if (_innerCursor != null)
                    return _innerCursor;
               
                return base.Cursor;
			}
			set { base.Cursor = value; }
		}

		#region Internal Properties

		private IRowLayout _rowLayout;

	    public int ColumnHeaderHeight { get; private set; }

	    /// <summary>
		/// returns all nodes, which parent is expanded
		/// </summary>
		private IEnumerable<TreeNodeAdv> VisibleNodes
		{
			get
			{
				TreeNodeAdv node = this.Root;
				
                while (node != null)
				{
					node = node.NextVisibleNode;
					
                    if (node != null)
						yield return node;
				}
			}
		}

		private bool _suspendSelectionEvent;
		internal bool SuspendSelectionEvent
		{
			get { return _suspendSelectionEvent; }
			set
			{
				if (value != _suspendSelectionEvent)
				{
					_suspendSelectionEvent = value;
					if (!_suspendSelectionEvent && _fireSelectionEvent)
						OnSelectionChanged();
				}
			}
		}

	    internal List<TreeNodeAdv> RowMap { get; private set; }
	    internal TreeNodeAdv SelectionStart { get; set; }
	    internal InputState Input { get; set; }

	    /// <summary>
		/// Number of rows fits to the current page
		/// </summary>
		internal int CurrentPageSize
		{
			get { return _rowLayout.CurrentPageSize; }
		}

		/// <summary>
		/// Number of all visible nodes (which parent is expanded)
		/// </summary>
		internal int RowCount
		{
			get { return this.RowMap.Count; }
		}

	    private int ContentWidth { get; set; }

	    private int _firstVisibleRow;
		internal int FirstVisibleRow
		{
			get { return _firstVisibleRow; }
			set
			{
				HideEditor();

				_firstVisibleRow = value;

				UpdateView();
			}
		}

		private int _offsetX;
		internal int OffsetX
		{
			get { return _offsetX; }
			private set
			{
				HideEditor();

				_offsetX = value;

				UpdateView();
			}
		}

		public override Rectangle DisplayRectangle
		{
			get
			{
				Rectangle r = ClientRectangle;
				//r.Y += ColumnHeaderHeight;
				//r.Height -= ColumnHeaderHeight;
				int w = _vScrollBar.Visible ? _vScrollBar.Width : 0;
				int h = _hScrollBar.Visible ? _hScrollBar.Height : 0;
				return new Rectangle(r.X, r.Y, r.Width - w, r.Height - h);
			}
		}

	    internal List<TreeNodeAdv> Selection { get; private set; }

	    #endregion

		#region Public Properties

		#region DesignTime

	    private bool _showLines = true;
		[DefaultValue(true), Category("Behavior")]
		public bool ShowLines
		{
			get { return _showLines; }
			set
			{
				_showLines = value;
				UpdateView();
			}
		}

		private bool _showPlusMinus = true;
		[DefaultValue(true), Category("Behavior")]
		public bool ShowPlusMinus
		{
			get { return _showPlusMinus; }
			set
			{
				_showPlusMinus = value;
				FullUpdate();
			}
		}

	    [DefaultValue(false), Category("Behavior")]
	    public bool ShowNodeToolTips { get; set; }

	    private ITreeModel _model;
		[Category("Data")]
		public ITreeModel Model
		{
			get { return _model; }
			set
			{
				if (_model != value)
				{
					if (_model != null)
						UnbindModelEvents();

					_model = value;

					CreateNodes();
					FullUpdate();

					if (_model != null)
						BindModelEvents();
				}
			}
		}

        // Font proprety for Tahoma as default font
        // wj32: Apparently some people don't have Tahoma...
        //private static Font _font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)), false);
        private static Font _font = DefaultFont;
        [Category("Appearance")]
        public override Font Font
        {
            get
            {
                return (base.Font);
            }
            set
            {
                if (value == null)
                    base.Font = _font;
                else
                {
                    if (value == Control.DefaultFont)
                        base.Font = _font;
                    else
                        base.Font = value;
                }
            }
        }
        public override void ResetFont()
        {
            Font = null;
        }
        private bool ShouldSerializeFont()
        {
            return (!Font.Equals(_font));
        }
        // End font property

		private BorderStyle _borderStyle = BorderStyle.Fixed3D;
		[DefaultValue(BorderStyle.Fixed3D), Category("Appearance")]
		public BorderStyle BorderStyle
		{
			get { return this._borderStyle; }
		    set
			{
				if (_borderStyle != value)
				{
					_borderStyle = value;

                    this.RecreateHandle();
                    this.Invalidate();
				}
			}
		}

		private bool _autoRowHeight;
		[DefaultValue(false), Category("Appearance")]
		public bool AutoRowHeight
		{
			get { return _autoRowHeight; }
		    set
			{
				_autoRowHeight = value;
				if (value)
					_rowLayout = new AutoRowHeightLayout(this, RowHeight);
				else
					_rowLayout = new FixedRowHeightLayout(this, RowHeight);
				FullUpdate();
			}
		}

		private int _rowHeight = 16;
		[DefaultValue(16), Category("Appearance")]
		public int RowHeight
		{
			get
			{
				return _rowHeight;
			}
			set
			{
				if (value <= 0)
					throw new ArgumentOutOfRangeException("value");

				_rowHeight = value;
				_rowLayout.PreferredRowHeight = value;
				FullUpdate();
			}
		}

		private TreeSelectionMode _selectionMode = TreeSelectionMode.Single;
		[DefaultValue(TreeSelectionMode.Single), Category("Behavior")]
		public TreeSelectionMode SelectionMode
		{
			get { return _selectionMode; }
			set { _selectionMode = value; }
		}

		private bool _hideSelection;
		[DefaultValue(false), Category("Behavior")]
		public bool HideSelection
		{
			get { return _hideSelection; }
			set
			{
				_hideSelection = value;
				UpdateView();
			}
		}

		private float _topEdgeSensivity = 0.3f;
		[DefaultValue(0.3f), Category("Behavior")]
		public float TopEdgeSensivity
		{
			get { return _topEdgeSensivity; }
			set
			{
				if (value < 0 || value > 1)
					throw new ArgumentOutOfRangeException();
				_topEdgeSensivity = value;
			}
		}

		private float _bottomEdgeSensivity = 0.3f;
		[DefaultValue(0.3f), Category("Behavior")]
		public float BottomEdgeSensivity
		{
			get { return _bottomEdgeSensivity; }
			set
			{
				if (value < 0 || value > 1)
					throw new ArgumentOutOfRangeException("BottomEdgeSensivity", "value should be from 0 to 1");
				_bottomEdgeSensivity = value;
			}
		}

	    [DefaultValue(false), Category("Behavior")]
	    public bool LoadOnDemand { get; set; }

	    private int _indent = 19;
		[DefaultValue(19), Category("Behavior")]
		public int Indent
		{
			get { return _indent; }
			set
			{
				_indent = value;
				UpdateView();
			}
		}

		private Color _lineColor = SystemColors.ControlDark;
		[Category("Behavior")]
		public Color LineColor
		{
			get { return _lineColor; }
			set
			{
				_lineColor = value;
				CreateLinePen();
				UpdateView();
			}
		}

	    [Category("Behavior"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	    public TreeColumnCollection Columns { get; private set; }

	    [Category("Behavior"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Editor(typeof(NodeControlCollectionEditor), typeof(UITypeEditor))]
	    public NodeControlsCollection NodeControls { get; private set; }

	    #endregion

		#region RunTime

	    [Browsable(false)]
	    public IToolTipProvider DefaultToolTipProvider { get; set; }

	    [Browsable(false)]
		public IEnumerable<TreeNodeAdv> AllNodes
		{
			get
			{
				if (this.Root.Nodes.Count > 0)
				{
					TreeNodeAdv node = this.Root.Nodes[0];
					
                    while (node != null)
					{
						yield return node;
						
                        if (node.Nodes.Count > 0)
							node = node.Nodes[0];
						else if (node.NextNode != null)
							node = node.NextNode;
						else
							node = node.BottomNode;
					}
				}
			}
		}

	    [Browsable(false)]
	    public TreeNodeAdv Root { get; private set; }

	    [Browsable(false)]
	    public ReadOnlyCollection<TreeNodeAdv> SelectedNodes { get; private set; }

	    [Browsable(false)]
		public TreeNodeAdv SelectedNode
		{
			get
			{
				if (this.Selection.Count > 0)
				{
					if (this.CurrentNode != null && this.CurrentNode.IsSelected)
						return this.CurrentNode;
					
                    return this.Selection[0];
				}
				
                return null;
			}
			set
			{
				if (SelectedNode == value)
					return;

				BeginUpdate();

				try
				{
					if (value == null)
					{
						ClearSelectionInternal();
					}
					else
					{
						if (!IsMyNode(value))
							throw new ArgumentException();

						ClearSelectionInternal();
						value.IsSelected = true;
						this.CurrentNode = value;
						EnsureVisible(value);
					}
				}
				finally
				{
					EndUpdate();
				}
			}
		}

	    [Browsable(false)]
	    public TreeNodeAdv CurrentNode { get; internal set; }

	    [Browsable(false)]
        public int ItemCount
        {
            get { return this.RowMap.Count; }
        } 

		#endregion

		#endregion

	}
}
