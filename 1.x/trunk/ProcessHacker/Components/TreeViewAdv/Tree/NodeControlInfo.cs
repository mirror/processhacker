using Aga.Controls.Tree.NodeControls;
using System.Drawing;

namespace Aga.Controls.Tree
{
	public struct NodeControlInfo
	{
		public static readonly NodeControlInfo Empty = new NodeControlInfo(null, Rectangle.Empty, null);

        public NodeControl Control;
        public Rectangle Bounds;
        public TreeNodeAdv Node;

	    public NodeControlInfo(NodeControl control, Rectangle bounds, TreeNodeAdv node) : this()
		{
			this.Control = control;
			this.Bounds = bounds;
			this.Node = node;
		}
	}
}
