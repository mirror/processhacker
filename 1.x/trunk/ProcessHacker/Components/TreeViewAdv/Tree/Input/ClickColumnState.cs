using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Aga.Controls.Tree
{
	internal class ClickColumnState : ColumnState
	{
		private Point _location;

		public ClickColumnState(TreeViewAdv tree, TreeColumn column, Point location)
			: base(tree, column)
		{
			_location = location;
		}

		public override void KeyDown(KeyEventArgs args)
		{
		}

		public override void MouseDown(TreeNodeAdvMouseEventArgs args)
		{
		}

		public override bool MouseMove(MouseEventArgs args)
		{
			if (TreeViewAdv.Dist(_location, args.Location) > TreeViewAdv.ItemDragSensivity)
			{
				this.Tree.Input = new ReorderColumnState(this.Tree, this.Column, args.Location);
				this.Tree.UpdateView();
			}

			return true;
		}

		public override void MouseUp(TreeNodeAdvMouseEventArgs args)
		{
			this.Tree.ChangeInput();
			this.Tree.UpdateView();
			this.Tree.OnColumnClicked(this.Column);
		}
	}
}
