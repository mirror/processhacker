/*
 * Modified by wj32.
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Aga.Controls.Properties;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Aga.Controls.Tree.NodeControls
{
	internal class NodePlusMinus : NodeControl
	{
        private TreeViewAdv _tree;
		public const int ImageSize = 9;
		public const int Width = 16;
        private bool _useVisualStyles;
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
            bool useVisualStyles = Application.RenderWithVisualStyles;

            if (useVisualStyles)
            {
                try
                {
                    _openedRenderer = new VisualStyleRenderer(VisualStyleElement.TreeView.Glyph.Opened);
                    _closedRenderer = new VisualStyleRenderer(VisualStyleElement.TreeView.Glyph.Closed);
                }
                catch
                {
                    useVisualStyles = false;
                }
            }

            _useVisualStyles = useVisualStyles;
        }

		public override Size MeasureSize(TreeNodeAdv node, DrawContext context)
		{
			return new Size(Width, Width);
		}

		public override void Draw(TreeNodeAdv node, DrawContext context)
		{
			if (node.CanExpand)
			{
				Rectangle r = context.Bounds;
				int dy = (int)Math.Round((float)(r.Height - ImageSize) / 2);

                if (_useVisualStyles)
                {
                    VisualStyleRenderer renderer;

                    if (node.IsExpanded)
                        renderer = _openedRenderer;
                    else
                        renderer = _closedRenderer;

                    try
                    {
                        renderer.DrawBackground(context.Graphics, new Rectangle(r.X, r.Y + dy, ImageSize, ImageSize));
                    }
                    catch (InvalidOperationException)
                    {
                        // Fucking retarded VisualStyleRenderer throws exceptions.
                        _useVisualStyles = false;
                    }
                }

                if (!_useVisualStyles)
                {
                    Image img;

                    if (node.IsExpanded)
                        img = this.Minus;
                    else
                        img = this.Plus;

                    context.Graphics.DrawImageUnscaled(img, new Point(r.X, r.Y + dy));
                }
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
