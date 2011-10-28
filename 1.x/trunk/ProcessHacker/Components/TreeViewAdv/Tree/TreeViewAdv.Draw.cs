/*
 * Modified by wj32.
 */

using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Aga.Controls.Tree
{
	public sealed partial class TreeViewAdv
	{
		private void CreatePens()
		{
			CreateLinePen();
		}

		private void CreateLinePen()
		{
			_linePen = new Pen(_lineColor) 
            {
                DashStyle = DashStyle.Dot
            };
		}

        protected override void OnPaint(PaintEventArgs e)
        {
#if DEBUG
            this.BeginPerformanceCount();
#endif
            DrawContext context = new DrawContext
            {
                Graphics = e.Graphics, 
                Font = this.Font, 
                Enabled = Enabled
            };

            this.DrawColumnHeaders(e.Graphics);

            int y = this.ColumnHeaderHeight;

            if (this.Columns.Count == 0 || e.ClipRectangle.Height <= y)
                return;

            int firstRowY = _rowLayout.GetRowBounds(this.FirstVisibleRow).Y;

            y -= firstRowY;

            e.Graphics.ResetTransform();
            e.Graphics.TranslateTransform(-OffsetX, y);

            Rectangle displayRect = e.ClipRectangle;

            for (int row = this.FirstVisibleRow; row < this.RowCount; row++)
            {
                Rectangle rowRect = _rowLayout.GetRowBounds(row);

                if (rowRect.Y + y > displayRect.Bottom)
                    break;

                this.DrawRow(e, ref context, row, rowRect);
            }

            e.Graphics.ResetTransform();

            this.DrawScrollBarsBox(e.Graphics);

#if DEBUG
            this.EndPerformanceCount(e);
#endif
        }

	    private void DrawRow(PaintEventArgs e, ref DrawContext context, int row, Rectangle rowRect)
		{
		    TreeNodeAdv node = this.RowMap[row];

		    context.DrawSelection = DrawSelectionMode.None;
		    context.CurrentEditorOwner = _currentEditorOwner;

		    bool focused = this.Focused;

		    if (node.IsSelected && focused)
		    {
		        context.DrawSelection = DrawSelectionMode.Active;
		    }
		    else if (node.IsSelected && !focused && !this.HideSelection)
		    {
		        context.DrawSelection = DrawSelectionMode.Inactive;
		    }

            Rectangle focusRect = new Rectangle(OffsetX, rowRect.Y, this.Width - (this._vScrollBar.Visible ? this._vScrollBar.Width : 0), rowRect.Height);

		    context.DrawFocus = false;

		    if (context.DrawSelection != DrawSelectionMode.Active)
		    {
                using (SolidBrush b = new SolidBrush(node.BackColor))
                {
                    e.Graphics.FillRectangle(b, focusRect);
                }
		    }

		    if (context.DrawSelection == DrawSelectionMode.Active || context.DrawSelection == DrawSelectionMode.Inactive)
		    {
		        if (context.DrawSelection == DrawSelectionMode.Active)
		        {
		            context.DrawSelection = DrawSelectionMode.FullRowSelect;

                    if (ExplorerVisualStyle.VisualStylesEnabled)
                    {
                        ExplorerVisualStyle.TvItemSelectedRenderer.DrawBackground(context.Graphics, focusRect);
                    }
                    else
                    {
		                e.Graphics.FillRectangle(SystemBrushes.Highlight, focusRect);
		            }
		        }
		        else
		        {
		            context.DrawSelection = DrawSelectionMode.None;
		        }
		    }

		    this.DrawNode(node, context);
		}

	    private void DrawColumnHeaders(Graphics gr)
		{
			ReorderColumnState reorder = this.Input as ReorderColumnState;
			
            int x = 0;

			TreeColumn.DrawBackground(gr, new Rectangle(0, 0, ClientRectangle.Width + 2, this.ColumnHeaderHeight - 1), false, false);
			
            gr.TranslateTransform(-OffsetX, 0);

			foreach (TreeColumn c in this.Columns)
			{
			    if (!c.IsVisible)
			        continue;

			    if (x + c.Width >= this.OffsetX && x - this.OffsetX < this.Bounds.Width)// skip invisible columns (fixed by wj32)
			    {
			        Rectangle rect = new Rectangle(x, 0, c.Width, this.ColumnHeaderHeight - 1);
			        
                    gr.SetClip(rect);

			        bool pressed = ((this.Input is ClickColumnState || reorder != null) && ((this.Input as ColumnState).Column == c));
			        
                    c.Draw(gr, rect, this.Font, pressed, this._hotColumn == c);
			        
                    gr.ResetClip();

                    if (reorder != null && reorder.DropColumn == c)
                    {
                        TreeColumn.DrawDropMark(gr, rect);
                    }
			    }
			    x += c.Width;
			}

			if (reorder != null)
			{
                if (reorder.DropColumn == null)
                {
                    TreeColumn.DrawDropMark(gr, new Rectangle(x, 0, 0, this.ColumnHeaderHeight));
                }

			    gr.DrawImage(reorder.GhostImage, new Point(reorder.Location.X +  + reorder.DragOffset, reorder.Location.Y));
			}
		}

		public void DrawNode(TreeNodeAdv node, DrawContext context)
		{
            //todo, node collection needs locking.
            foreach (NodeControlInfo item in GetNodeControls(node))
			{
				if (item.Bounds.X + item.Bounds.Width >= OffsetX && item.Bounds.X - OffsetX < this.Bounds.Width)// skip invisible nodes (fixed by wj32)
				{
				    context.Bounds = item.Bounds;

				    context.Graphics.SetClip(context.Bounds);

				    item.Control.Draw(node, context);

				    context.Graphics.ResetClip();
				}
			}
		}

		private void DrawScrollBarsBox(Graphics gr)
		{
			Rectangle r1 = this.DisplayRectangle;
            Rectangle r2 = this.ClientRectangle;

			gr.FillRectangle(SystemBrushes.Control, new Rectangle(r1.Right, r1.Bottom, r2.Width - r1.Width, r2.Height - r1.Height));
		}

	    private double _totalTime;
        private int _paintCount;

        private void BeginPerformanceCount()
        {
            _paintCount++;
            TimeCounter.Start();
        }

        private void EndPerformanceCount(PaintEventArgs e)
        {
            double time = TimeCounter.Finish();
            _totalTime += time;
            
            string debugText = string.Format("FPS {0:0.0}; Avg. FPS {1:0.0}", 1 / time, 1 / (_totalTime / _paintCount));

            e.Graphics.FillRectangle(Brushes.White, new Rectangle(this.DisplayRectangle.Width - 150, this.DisplayRectangle.Height - 20, 150, 20));

            TextRenderer.DrawText(e.Graphics, debugText, this.Font, new Point(this.DisplayRectangle.Width - 150, this.DisplayRectangle.Height - 20), Color.Black);

            //e.Graphics.DrawString(debugText, this.Font, Brushes.Gray, new PointF(DisplayRectangle.Width - 150, DisplayRectangle.Height - 20));
        }
	}
}
