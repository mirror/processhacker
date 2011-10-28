using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Aga.Controls.Tree.NodeControls;
using System.Drawing.Imaging;
using System.Threading;

namespace Aga.Controls.Tree
{
	public sealed partial class TreeViewAdv
	{
        // Note by wj32: I've added a whole bunch of this.Invalidate()s that are necessary for PH.
		#region Keys

		protected override bool IsInputChar(char charCode)
		{
			return true;
		}

		protected override bool IsInputKey(Keys keyData)
		{
			if (((keyData & Keys.Up) == Keys.Up)
				|| ((keyData & Keys.Down) == Keys.Down)
				|| ((keyData & Keys.Left) == Keys.Left)
				|| ((keyData & Keys.Right) == Keys.Right))
				return true;
			else
				return base.IsInputKey(keyData);
		}

		internal void ChangeInput()
		{
			if ((ModifierKeys & Keys.Shift) == Keys.Shift)
			{
				if (!(this.Input is InputWithShift))
					this.Input = new InputWithShift(this);
			}
			else if ((ModifierKeys & Keys.Control) == Keys.Control)
			{
				if (!(this.Input is InputWithControl))
					this.Input = new InputWithControl(this);
			}
			else
			{
				if (!(this.Input.GetType() == typeof(NormalInputState)))
					this.Input = new NormalInputState(this);
			}
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
			if (!e.Handled)
			{
				if (e.KeyCode == Keys.ShiftKey || e.KeyCode == Keys.ControlKey)
					ChangeInput();
				this.Input.KeyDown(e);
				if (!e.Handled)
				{
					foreach (NodeControlInfo item in GetNodeControls(this.CurrentNode))
					{
						item.Control.KeyDown(e);
						if (e.Handled)
							break;
					}
				}
            }
            this.Invalidate();
		}

		protected override void OnKeyUp(KeyEventArgs e)
		{
			base.OnKeyUp(e);
			if (!e.Handled)
			{
				if (e.KeyCode == Keys.ShiftKey || e.KeyCode == Keys.ControlKey)
					ChangeInput();
				if (!e.Handled)
				{
					foreach (NodeControlInfo item in GetNodeControls(this.CurrentNode))
					{
						item.Control.KeyUp(e);
						if (e.Handled)
							return;
					}
				}
			}
		}

		protected override void OnKeyPress(KeyPressEventArgs e)
		{
			base.OnKeyPress(e);
			if (!e.Handled)
                _search.Search(e.KeyChar);
            this.Invalidate();
		}

		#endregion

		#region Mouse

		private TreeNodeAdvMouseEventArgs CreateMouseArgs(MouseEventArgs e)
		{
			TreeNodeAdvMouseEventArgs args = new TreeNodeAdvMouseEventArgs(e);
			args.ViewLocation = new Point(e.X + OffsetX,
				e.Y + _rowLayout.GetRowBounds(FirstVisibleRow).Y - this.ColumnHeaderHeight);
			args.ModifierKeys = ModifierKeys;
			args.Node = GetNodeAt(e.Location);
			NodeControlInfo info = GetNodeControlInfoAt(args.Node, e.Location);
			args.ControlBounds = info.Bounds;
			args.Control = info.Control;
			return args;
		}

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			_search.EndSearch();
			if (SystemInformation.MouseWheelScrollLines > 0)
			{
				int lines = e.Delta * SystemInformation.MouseWheelScrollLines /
                    SystemInformation.MouseWheelScrollDelta;
				int newValue = _vScrollBar.Value - lines;
				newValue = Math.Min(_vScrollBar.Maximum - _vScrollBar.LargeChange + 1, newValue);
				newValue = Math.Min(_vScrollBar.Maximum, newValue);
				_vScrollBar.Value = Math.Max(_vScrollBar.Minimum, newValue);
			}
			base.OnMouseWheel(e);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (!Focused)
				Focus();

			_search.EndSearch();
			if (e.Button == MouseButtons.Left)
			{
				TreeColumn c;
				c = GetColumnDividerAt(e.Location);
				if (c != null)
				{
                    this.Input = new ResizeColumnState(this, c, e.Location);
                    this.Invalidate();
					return;
				}
				c = GetColumnAt(e.Location);
				if (c != null)
				{
					this.Input = new ClickColumnState(this, c, e.Location);
                    UpdateView();
                    this.Invalidate();
					return;
				}
			}

			ChangeInput();
			TreeNodeAdvMouseEventArgs args = CreateMouseArgs(e);

			if (args.Node != null && args.Control != null)
				args.Control.MouseDown(args);

			if (!args.Handled)
				this.Input.MouseDown(args);

            base.OnMouseDown(e);
            this.Invalidate();
		}

		protected override void OnMouseClick(MouseEventArgs e)
		{
			TreeNodeAdvMouseEventArgs args = CreateMouseArgs(e);
			
            if (args.Node != null)
				OnNodeMouseClick(args);

			base.OnMouseClick(e);

            this.FullUpdate();
            this.Invalidate();
		}

		protected override void OnMouseDoubleClick(MouseEventArgs e)
		{
			TreeNodeAdvMouseEventArgs args = CreateMouseArgs(e);

			if (args.Node != null && args.Control != null)
				args.Control.MouseDoubleClick(args);

			if (!args.Handled)
			{
				if (args.Node != null)
					OnNodeMouseDoubleClick(args);
			}

			base.OnMouseDoubleClick(e);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			TreeNodeAdvMouseEventArgs args = CreateMouseArgs(e);
			
            if (this.Input is ResizeColumnState)
				this.Input.MouseUp(args);
			else
			{
				if (args.Node != null && args.Control != null)
					args.Control.MouseUp(args);
				
                if (!args.Handled)
					this.Input.MouseUp(args);

				base.OnMouseUp(e);
			}
		}


		protected override void OnMouseMove(MouseEventArgs e)
		{
		    if (this.Input.MouseMove(e))
				return;

			base.OnMouseMove(e);

			SetCursor(e);

			UpdateToolTip(e);
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			_hotColumn = null;
			UpdateHeaders();
			base.OnMouseLeave(e);
		}

		private void SetCursor(MouseEventArgs e)
		{
		    using (TreeColumn col = this.GetColumnDividerAt(e.Location))
		    {
		        if (col == null)
		            this._innerCursor = null;
		        else
		        {
		            switch (col.Width)
		            {
		                case 0:
		                    this._innerCursor = Cursors.VSplit;
		                    break;
		                default:
		                    this._innerCursor = Cursors.VSplit;
		                    break;
		            }
		        }

		        //col = this.GetColumnAt(e.Location);

		        if (col != this._hotColumn)
		        {
		            this._hotColumn = col;

		            this.UpdateHeaders();
		        }
		    }
		}

		internal TreeColumn GetColumnAt(Point p)
		{
			if (p.Y > this.ColumnHeaderHeight)
				return null;

			int x = -OffsetX;
			
            foreach (TreeColumn col in this.Columns)
			{
				if (col.IsVisible)
				{
					Rectangle rect = new Rectangle(x, 0, col.Width, this.ColumnHeaderHeight);
					
                    x += col.Width;
				
                    if (rect.Contains(p))
						return col;
				}
			}
			return null;
		}

		internal int GetColumnX(TreeColumn column)
		{
			int x = -OffsetX;
			foreach (TreeColumn col in this.Columns)
			{
				if (col.IsVisible)
				{
				    if (column == col)
						return x;
				    
                    x += col.Width;
				}
			}
			return x;
		}

		internal TreeColumn GetColumnDividerAt(Point p)
		{
			if (p.Y > this.ColumnHeaderHeight)
				return null;

			int x = -OffsetX;
			TreeColumn prevCol = null;
			Rectangle left, right;
		
            foreach (TreeColumn col in this.Columns)
			{
				if (col.IsVisible)
				{
					if (col.Width > 0)
					{
						left = new Rectangle(x, 0, DividerWidth / 2, this.ColumnHeaderHeight);
						right = new Rectangle(x + col.Width - (DividerWidth / 2), 0, DividerWidth / 2, this.ColumnHeaderHeight);
						
                        if (left.Contains(p) && prevCol != null)
							return prevCol;
						else if (right.Contains(p))
							return col;
					}
					prevCol = col;
					x += col.Width;
				}
			}

			left = new Rectangle(x, 0, DividerWidth / 2, this.ColumnHeaderHeight);
			
            if (left.Contains(p) && prevCol != null)
				return prevCol;

			return null;
		}

		TreeColumn _tooltipColumn;
		private void UpdateToolTip(MouseEventArgs e)
		{
			TreeColumn col = GetColumnAt(e.Location);
			if (col != null)
			{
				if (col != _tooltipColumn)
					SetTooltip(col.TooltipText);
			}
			else
				DisplayNodesTooltip(e);
			_tooltipColumn = col;
		}

		TreeNodeAdv _hotNode;
		NodeControl _hotControl;

		private void DisplayNodesTooltip(MouseEventArgs e)
		{
			if (this.ShowNodeToolTips)
			{
				TreeNodeAdvMouseEventArgs args = CreateMouseArgs(e);
				if (args.Node != null && args.Control != null)
				{
					if (args.Node != _hotNode || args.Control != _hotControl)
						SetTooltip(GetNodeToolTip(args));
				}
				else
					_toolTip.SetToolTip(this, null);

				_hotControl = args.Control;
				_hotNode = args.Node;
			}
			else
				_toolTip.SetToolTip(this, null);
		}

		private void SetTooltip(string text)
		{
			if (!String.IsNullOrEmpty(text))
			{
				_toolTip.Active = false;
				_toolTip.SetToolTip(this, text);
				_toolTip.Active = true;
			}
			else
				_toolTip.SetToolTip(this, null);
		}

		private string GetNodeToolTip(TreeNodeAdvMouseEventArgs args)
		{
			string msg = args.Control.GetToolTip(args.Node);

			BaseTextControl btc = args.Control as BaseTextControl;
			if (btc != null && btc.DisplayHiddenContentInToolTip && String.IsNullOrEmpty(msg))
			{
				Size ms = btc.GetActualSize(args.Node, _measureContext);
				if (ms.Width > args.ControlBounds.Size.Width || ms.Height > args.ControlBounds.Size.Height
					|| args.ControlBounds.Right - OffsetX > DisplayRectangle.Width)
					msg = btc.GetLabel(args.Node);
			}

			if (String.IsNullOrEmpty(msg) && this.DefaultToolTipProvider != null)
				msg = this.DefaultToolTipProvider.GetToolTip(args.Node, args.Control);

			return msg;
		}

		#endregion
	}
}
