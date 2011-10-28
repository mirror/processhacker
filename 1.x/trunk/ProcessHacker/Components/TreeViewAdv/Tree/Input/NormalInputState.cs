using System;
using System.Windows.Forms;

namespace Aga.Controls.Tree
{
	internal class NormalInputState : InputState
	{
		private bool _mouseDownFlag;

		public NormalInputState(TreeViewAdv tree) : base(tree)
		{
		}

		public override void KeyDown(KeyEventArgs args)
		{
			if (this.Tree.CurrentNode == null && this.Tree.Root.Nodes.Count > 0)
				this.Tree.CurrentNode = this.Tree.Root.Nodes[0];

			if (this.Tree.CurrentNode != null)
			{
				switch (args.KeyCode)
				{
					case Keys.Right:
                        if (!this.Tree.CurrentNode.IsExpanded)
                        {
                            this.Tree.CurrentNode.IsExpanded = true;
                            // by fliser
                            this.Tree.FullUpdate();
                        }
                        else if (this.Tree.CurrentNode.Nodes.Count > 0)
                            this.Tree.SelectedNode = this.Tree.CurrentNode.Nodes[0];
						args.Handled = true;
						break;
					case Keys.Left:
                        if (this.Tree.CurrentNode.IsExpanded)
                        {
                            this.Tree.CurrentNode.IsExpanded = false;
                            // by fliser
                            this.Tree.FullUpdate();
                        }
                        else if (this.Tree.CurrentNode.Parent != this.Tree.Root)
                            this.Tree.SelectedNode = this.Tree.CurrentNode.Parent;
						args.Handled = true;
						break;
					case Keys.Down:
						NavigateForward(1);
						args.Handled = true;
						break;
					case Keys.Up:
						NavigateBackward(1);
						args.Handled = true;
						break;
					case Keys.PageDown:
						NavigateForward(Math.Max(1, this.Tree.CurrentPageSize - 1));
						args.Handled = true;
						break;
					case Keys.PageUp:
						NavigateBackward(Math.Max(1, this.Tree.CurrentPageSize - 1));
						args.Handled = true;
						break;
					case Keys.Home:
						if (this.Tree.RowMap.Count > 0)
							FocusRow(this.Tree.RowMap[0]);
						args.Handled = true;
						break;
					case Keys.End:
						if (this.Tree.RowMap.Count > 0)
							FocusRow(this.Tree.RowMap[this.Tree.RowMap.Count-1]);
						args.Handled = true;
						break;
					case Keys.Subtract:
						this.Tree.CurrentNode.Collapse();
                        // by fliser
                        this.Tree.FullUpdate();
                        args.Handled = true;
						args.SuppressKeyPress = true;
						break;
					case Keys.Add:
						this.Tree.CurrentNode.Expand();
                        // by fliser
                        this.Tree.FullUpdate();
                        args.Handled = true;
						args.SuppressKeyPress = true;
						break;
					case Keys.Multiply:
						this.Tree.CurrentNode.ExpandAll();
                        // by fliser
                        this.Tree.FullUpdate();
                        args.Handled = true;
						args.SuppressKeyPress = true;
						break;
				}
			}
		}

		public override void MouseDown(TreeNodeAdvMouseEventArgs args)
		{
			if (args.Node != null)
			{
				if (args.Button == MouseButtons.Left || args.Button == MouseButtons.Right)
				{
					this.Tree.BeginUpdate();
					try
					{
						this.Tree.CurrentNode = args.Node;

						if (args.Node.IsSelected)
							_mouseDownFlag = true;
						else
						{
							_mouseDownFlag = false;
							DoMouseOperation(args);
						}
					}
					finally
					{
						this.Tree.EndUpdate();
					}
				}

			}
			else
			{
				MouseDownAtEmptySpace(args);
			}
		}

		public override void MouseUp(TreeNodeAdvMouseEventArgs args)
		{
			if (_mouseDownFlag)
			{
				switch (args.Button)
				{
				    case MouseButtons.Left:
				        this.DoMouseOperation(args);
				        break;
				    case MouseButtons.Right:
				        this.Tree.CurrentNode = args.Node;
				        break;
				}
			}
			_mouseDownFlag = false;
		}


		private void NavigateBackward(int n)
		{
			int row = Math.Max(this.Tree.CurrentNode.Row - n, 0);
			if (row != this.Tree.CurrentNode.Row)
				FocusRow(this.Tree.RowMap[row]);
		}

		private void NavigateForward(int n)
		{
			int row = Math.Min(this.Tree.CurrentNode.Row + n, this.Tree.RowCount - 1);
			if (row != this.Tree.CurrentNode.Row)
				FocusRow(this.Tree.RowMap[row]);
		}

		protected virtual void MouseDownAtEmptySpace(TreeNodeAdvMouseEventArgs args)
		{
			this.Tree.ClearSelectionInternal();
		}

		protected virtual void FocusRow(TreeNodeAdv node)
		{
			this.Tree.SuspendSelectionEvent = true;
			try
			{
				this.Tree.ClearSelectionInternal();
				this.Tree.CurrentNode = node;
				this.Tree.SelectionStart = node;
				node.IsSelected = true;
				this.Tree.ScrollTo(node);
			}
			finally
			{
				this.Tree.SuspendSelectionEvent = false;
			}
		}

		protected bool CanSelect(TreeNodeAdv node)
		{
			if (this.Tree.SelectionMode == TreeSelectionMode.MultiSameParent)
			{
				return (this.Tree.SelectionStart == null || node.Parent == this.Tree.SelectionStart.Parent);
			}

            return true;
		}

		protected virtual void DoMouseOperation(TreeNodeAdvMouseEventArgs args)
		{
			this.Tree.SuspendSelectionEvent = true;
			try
			{
				this.Tree.ClearSelectionInternal();
				if (args.Node != null)
					args.Node.IsSelected = true;
				this.Tree.SelectionStart = args.Node;
			}
			finally
			{
				this.Tree.SuspendSelectionEvent = false;
			}
		}
	}
}
