using System.ComponentModel;
using System.Windows.Forms;

namespace Aga.Controls.Tree.NodeControls
{

	public class NodeIntegerTextBox : NodeTextBox
	{
		private bool _allowNegativeSign = true;
		[DefaultValue(true)]
		public bool AllowNegativeSign
		{
			get { return _allowNegativeSign; }
			set { _allowNegativeSign = value; }
		}

	    protected override TextBox CreateTextBox()
		{
            return new NumericTextBox 
            {
                AllowDecimalSeperator = false, 
                AllowNegativeSign = this.AllowNegativeSign
            };
		}

		protected override void DoApplyChanges(TreeNodeAdv node, Control editor)
		{
			SetValue(node, (editor as NumericTextBox).IntValue);
		}
	}
}
