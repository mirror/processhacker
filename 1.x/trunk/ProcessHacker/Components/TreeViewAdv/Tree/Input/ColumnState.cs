namespace Aga.Controls.Tree
{
	internal abstract class ColumnState : InputState
	{
	    public TreeColumn Column { get; private set; }

	    protected ColumnState(TreeViewAdv tree, TreeColumn column)
			: base(tree)
		{
			this.Column = column;
		}
	}
}
