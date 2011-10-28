namespace Aga.Controls.Tree
{
	internal class InputWithControl : NormalInputState
	{
		public InputWithControl(TreeViewAdv tree): base(tree)
		{
		}

		protected override void DoMouseOperation(TreeNodeAdvMouseEventArgs args)
		{
			if (this.Tree.SelectionMode == TreeSelectionMode.Single)
			{
				base.DoMouseOperation(args);
			}
			else if (CanSelect(args.Node))
			{
				args.Node.IsSelected = !args.Node.IsSelected;
				this.Tree.SelectionStart = args.Node;
			}
		}

		protected override void MouseDownAtEmptySpace(TreeNodeAdvMouseEventArgs args)
		{
		}
	}
}
