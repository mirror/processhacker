using System;

namespace Aga.Controls.Tree
{
	public class TreeViewAdvEventArgs : EventArgs
	{
	    public TreeNodeAdv Node { get; private set; }

	    public TreeViewAdvEventArgs(TreeNodeAdv node)
		{
			this.Node = node;
		}
	}
}
