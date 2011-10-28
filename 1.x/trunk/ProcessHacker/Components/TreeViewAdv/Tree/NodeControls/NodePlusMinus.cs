/*
 * Modified by wj32.
 */

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using ProcessHacker.Properties;

namespace Aga.Controls.Tree.NodeControls
{
	internal class NodePlusMinus : NodeControl
	{
        private readonly TreeViewAdv _tree;
        public static int ImageSize = 9;
		public static int Width = 16;
        private VisualStyleRenderer _openedRenderer;
        private VisualStyleRenderer _closedRenderer;
		private Bitmap _plus;
		private Bitmap _minus;

		public NodePlusMinus(TreeViewAdv tree)
		{
            _tree = tree;
            this.RefreshVisualStyles();
		}

        private Bitmap Plus
        {
            get
            {
                if (_plus == null)
                    _plus = Resources.plus;

                return _plus;
            }
        }

        private Bitmap Minus
        {
            get
            {
                if (_minus == null)
                    _minus = Resources.minus;

                return _minus;
            }
        }

        public void RefreshVisualStyles()
        {
            if (ExplorerVisualStyle.VisualStylesEnabled)
            {
                try
                {
                    _openedRenderer = ExplorerVisualStyle.TvOpenedRenderer;
                    _closedRenderer = ExplorerVisualStyle.TvClosedRenderer;
                }
                catch
                {
                    _openedRenderer = new VisualStyleRenderer(VisualStyleElement.TreeView.Glyph.Opened);
                    _closedRenderer = new VisualStyleRenderer(VisualStyleElement.TreeView.Glyph.Closed);
                }
            }
            else
            {
                try
                {
                    _openedRenderer = new VisualStyleRenderer(VisualStyleElement.TreeView.Glyph.Opened);
                    _closedRenderer = new VisualStyleRenderer(VisualStyleElement.TreeView.Glyph.Closed);
                }
                catch
                {
                }
            }
        }

		public override Size MeasureSize(TreeNodeAdv node, DrawContext context)
		{
			return new Size(Width, Width);
		}

		public override void Draw(TreeNodeAdv node, DrawContext context)
		{
		    if (!node.CanExpand)
		        return;

		    Rectangle r = context.Bounds;

            if (ExplorerVisualStyle.VisualStylesEnabled)
		    {
		        VisualStyleRenderer renderer;

		        if (node.IsExpanded)
		            renderer = this._openedRenderer;
		        else
		            renderer = this._closedRenderer;

		        try
		        {
                    renderer.DrawBackground(context.Graphics, new Rectangle(r.X, r.Y, Width, Width));
		        }
		        catch (InvalidOperationException)
		        {
		            
		        }
		    }
            else
		    {
		        Image img;

		        if (node.IsExpanded)
		            img = this.Minus;
		        else
		            img = this.Plus;

                int dy = (int)Math.Round((float)(r.Height - ImageSize) / 2);

		        context.Graphics.DrawImageUnscaled(img, new Point(r.X, r.Y + dy));
		    }
		}

		public override void MouseDown(TreeNodeAdvMouseEventArgs args)
		{
            if (args.Button == MouseButtons.Left)
            {
                args.Handled = true;

                if (args.Node.CanExpand)
                {
                    args.Node.IsExpanded = !args.Node.IsExpanded;
                    // fixed by wj32
                    _tree.FullUpdate();
                }
            }
		}

		public override void MouseDoubleClick(TreeNodeAdvMouseEventArgs args)
		{
			args.Handled = true; // Supress expand/collapse when double click on plus/minus
		}
	}
}
