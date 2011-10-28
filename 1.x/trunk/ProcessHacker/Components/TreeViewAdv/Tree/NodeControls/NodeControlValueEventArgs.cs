namespace Aga.Controls.Tree.NodeControls
{
	public class NodeControlValueEventArgs : NodeEventArgs
	{
	    public object Value { get; set; }

	    public NodeControlValueEventArgs(TreeNodeAdv node)
			:base(node)
		{
		}
	}
}
