using System;

namespace Aga.Controls.Tree
{
	internal class InputWithShift: NormalInputState
	{
		public InputWithShift(TreeViewAdv tree): base(tree)
		{
		}

		protected override void FocusRow(TreeNodeAdv node)
		{
			this.Tree.SuspendSelectionEvent = true;
			try
			{
				if (this.Tree.SelectionMode == TreeSelectionMode.Single || this.Tree.SelectionStart == null)
					base.FocusRow(node);
				else if (CanSelect(node))
				{
					SelectAllFromStart(node);
					this.Tree.CurrentNode = node;
					this.Tree.ScrollTo(node);
				}
			}
			finally
			{
				this.Tree.SuspendSelectionEvent = false;
			}
		}

		protected override void DoMouseOperation(TreeNodeAdvMouseEventArgs args)
		{
			if (this.Tree.SelectionMode == TreeSelectionMode.Single || this.Tree.SelectionStart == null)
			{
				base.DoMouseOperation(args);
			}
			else if (CanSelect(args.Node))
			{
				this.Tree.SuspendSelectionEvent = true;
				try
				{
					SelectAllFromStart(args.Node);
				}
				finally
				{
					this.Tree.SuspendSelectionEvent = false;
				}
			}
		}

		protected override void MouseDownAtEmptySpace(TreeNodeAdvMouseEventArgs args)
		{
		}

		private void SelectAllFromStart(TreeNodeAdv node)
		{
			this.Tree.ClearSelectionInternal();
			int a = node.Row;
			int b = this.Tree.SelectionStart.Row;
			for (int i = Math.Min(a, b); i <= Math.Max(a, b); i++)
			{
				if (this.Tree.SelectionMode == TreeSelectionMode.Multi || this.Tree.RowMap[i].Parent == node.Parent)
					this.Tree.RowMap[i].IsSelected = true;
			}
		}
	}
}
