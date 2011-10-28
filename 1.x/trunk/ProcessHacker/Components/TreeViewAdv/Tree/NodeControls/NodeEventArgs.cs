using System;

namespace Aga.Controls.Tree.NodeControls
{
	public class NodeEventArgs : EventArgs
	{
	    public TreeNodeAdv Node { get; private set; }

	    public NodeEventArgs(TreeNodeAdv node)
		{
			this.Node = node;
		}
	}
}
