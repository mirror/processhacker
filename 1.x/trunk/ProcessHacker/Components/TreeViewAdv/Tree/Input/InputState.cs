using System.Windows.Forms;

namespace Aga.Controls.Tree
{
	internal abstract class InputState
	{
	    public TreeViewAdv Tree { get; private set; }

	    protected InputState(TreeViewAdv tree)
		{
			this.Tree = tree;
		}

		public abstract void KeyDown(KeyEventArgs args);
		public abstract void MouseDown(TreeNodeAdvMouseEventArgs args);
		public abstract void MouseUp(TreeNodeAdvMouseEventArgs args);

		/// <summary>
		/// handle OnMouseMove event
		/// </summary>
		/// <param name="args"></param>
		/// <returns>true if event was handled and should be dispatched</returns>
		public virtual bool MouseMove(MouseEventArgs args)
		{
			return false;
		}
	}
}
