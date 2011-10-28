using System.Drawing;

namespace Aga.Controls.Tree.NodeControls
{
	public class DrawEventArgs : NodeEventArgs
	{
	    public DrawContext Context { get; private set; }
	    public Brush BackgroundBrush { get; set; }
	    public Font Font { get; set; }
	    public Color TextColor { get; set; }
	    public string Text { get; private set; }

	    public DrawEventArgs(TreeNodeAdv node, DrawContext context, string text)
			: base(node)
		{
			this.Context = context;
			this.Text = text;
		}
	}
}
