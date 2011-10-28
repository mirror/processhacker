using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.ComponentModel;

namespace Aga.Controls.Tree.NodeControls
{
	public class NodeTextBox: BaseTextControl
	{
	    private TextBox EditorTextBox
		{
			get { return this.CurrentEditor as TextBox; }
		}

	    protected override Size CalculateEditorSize(EditorContext context)
		{
		    return context.Bounds.Size;
		}

	    public override void KeyDown(KeyEventArgs args)
		{
			if (args.KeyCode == Keys.F2 && Parent.CurrentNode != null)
			{
				args.Handled = true;
				BeginEditByUser();
			}
		}

		protected override Control CreateEditor(TreeNodeAdv node)
		{
			TextBox textBox = CreateTextBox();
			textBox.TextAlign = TextAlign;
			textBox.Text = GetLabel(node);
			textBox.BorderStyle = BorderStyle.FixedSingle;
			textBox.TextChanged += this.textBox_TextChanged;
			_label = textBox.Text;
			SetEditControlProperties(textBox, node);
			return textBox;
		}

		protected virtual TextBox CreateTextBox()
		{
			return new TextBox();
		}

		private string _label;
		private void textBox_TextChanged(object sender, EventArgs e)
		{
			_label = EditorTextBox.Text;
			Parent.UpdateEditorBounds();
		}

		protected override void DoApplyChanges(TreeNodeAdv node, Control editor)
		{
			string oldLabel = GetLabel(node);
			if (oldLabel != _label)
			{
				SetLabel(node, _label);
				OnLabelChanged();
			}
		}

		public void Cut()
		{
			if (EditorTextBox != null)
				EditorTextBox.Cut();
		}

		public void Copy()
		{
			if (EditorTextBox != null)
				EditorTextBox.Copy();
		}

		public void Paste()
		{
			if (EditorTextBox != null)
				EditorTextBox.Paste();
		}

		public void Delete()
		{
			if (EditorTextBox != null)
			{
				int len = Math.Max(EditorTextBox.SelectionLength, 1);
				if (EditorTextBox.SelectionStart < EditorTextBox.Text.Length)
				{
					int start = EditorTextBox.SelectionStart;
					EditorTextBox.Text = EditorTextBox.Text.Remove(EditorTextBox.SelectionStart, len);
					EditorTextBox.SelectionStart = start;
				}
			}
		}

		public event EventHandler LabelChanged;
		protected void OnLabelChanged()
		{
			if (LabelChanged != null)
				LabelChanged(this, EventArgs.Empty);
		}
	}
}
