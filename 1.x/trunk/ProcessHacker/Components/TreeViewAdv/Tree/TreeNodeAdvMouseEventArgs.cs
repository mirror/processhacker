using System.Windows.Forms;
using System.Drawing;
using Aga.Controls.Tree.NodeControls;

namespace Aga.Controls.Tree
{
	public class TreeNodeAdvMouseEventArgs : MouseEventArgs
	{
	    public TreeNodeAdv Node { get; internal set; }
	    public NodeControl Control { get; internal set; }
	    public Point ViewLocation { get; internal set; }
	    public Keys ModifierKeys { get; internal set; }
	    public bool Handled { get; set; }
	    public Rectangle ControlBounds { get; internal set; }

	    public TreeNodeAdvMouseEventArgs(MouseEventArgs args)
			: base(args.Button, args.Clicks, args.X, args.Y, args.Delta)
		{
		}
	}
}
