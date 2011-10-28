using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Aga.Controls.Tree
{
	internal class ReorderColumnState : ColumnState
	{
		#region Properties

		private Point _location;
		public Point Location
		{
			get { return _location; }
		}

		private Bitmap _ghostImage;
		public Bitmap GhostImage
		{
			get { return _ghostImage; }
		}

		private TreeColumn _dropColumn;
		public TreeColumn DropColumn
		{
			get { return _dropColumn; }
		}

		private int _dragOffset;
		public int DragOffset
		{
			get { return _dragOffset; }
		}

		#endregion

		public ReorderColumnState(TreeViewAdv tree, TreeColumn column, Point initialMouseLocation)
			: base(tree, column)
		{
			_location = new Point(initialMouseLocation.X + this.Tree.OffsetX, 0);
			_dragOffset = tree.GetColumnX(column) - initialMouseLocation.X;
			_ghostImage = column.CreateGhostImage(new Rectangle(0, 0, column.Width, tree.ColumnHeaderHeight), tree.Font);
		}

		public override void KeyDown(KeyEventArgs args)
		{
			args.Handled = true;
			if (args.KeyCode == Keys.Escape)
				FinishResize();
		}

		public override void MouseDown(TreeNodeAdvMouseEventArgs args)
		{
		}

		public override void MouseUp(TreeNodeAdvMouseEventArgs args)
		{
			FinishResize();
		}

		public override bool MouseMove(MouseEventArgs args)
		{
			_dropColumn = null;
			_location = new Point(args.X + this.Tree.OffsetX, 0);
			int x = 0;
			foreach (TreeColumn c in this.Tree.Columns)
			{
				if (c.IsVisible)
				{
					if (_location.X < x + c.Width / 2)
					{
						_dropColumn = c;
						break;
					}
					x += c.Width;
				}
			}
			this.Tree.UpdateHeaders();
			return true;
		}

		private void FinishResize()
		{
			this.Tree.ChangeInput();
			if (this.Column == DropColumn)
				this.Tree.UpdateView();
			else
			{
				this.Tree.Columns.Remove(this.Column);
				if (DropColumn == null)
					this.Tree.Columns.Add(this.Column);
				else
					this.Tree.Columns.Insert(this.Tree.Columns.IndexOf(DropColumn), this.Column);

				this.Tree.OnColumnReordered(this.Column);
			}
		}
	}
}