using System.Drawing;

using Aga.Controls.Tree.NodeControls;

namespace Aga.Controls.Tree
{
	public struct DrawContext
	{
        public Graphics Graphics;
        public Rectangle Bounds;
        public Font Font;
        public DrawSelectionMode DrawSelection;
        public bool DrawFocus;
        public NodeControl CurrentEditorOwner;
        public bool Enabled;
	}
}
