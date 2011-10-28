using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

namespace Aga.Controls.Tree.NodeControls
{
	public abstract class EditableControl : InteractiveControl
	{
		private Timer _timer;
		private bool _editFlag;

		#region Properties

	    protected TreeNodeAdv EditNode { get; private set; }
	    protected Control CurrentEditor { get; private set; }

	    [DefaultValue(false)]
	    public bool EditOnClick { get; set; }

	    #endregion

		protected EditableControl()
		{
			_timer = new Timer
			{
			    Interval = 1000
			};

		    _timer.Tick += this.TimerTick;
		}

		private void TimerTick(object sender, EventArgs e)
		{
			_timer.Stop();

			if (_editFlag)
				BeginEditByUser();
			
            _editFlag = false;
		}

		public void SetEditorBounds(EditorContext context)
		{
			Size size = CalculateEditorSize(context);
			context.Editor.Bounds = new Rectangle(context.Bounds.X, context.Bounds.Y, Math.Min(size.Width, context.Bounds.Width), context.Bounds.Height);
		}

		protected abstract Size CalculateEditorSize(EditorContext context);

		protected virtual bool CanEdit(TreeNodeAdv node)
		{
			return (node.Tag != null) && IsEditEnabled(node);
		}

		protected void BeginEditByUser()
		{
			if (EditEnabled)
				BeginEdit();
		}

		public void BeginEdit()
		{
			if (Parent.CurrentNode != null && CanEdit(Parent.CurrentNode))
			{
				CancelEventArgs args = new CancelEventArgs();
				OnEditorShowing(args);

				if (!args.Cancel)
				{
					this.CurrentEditor = CreateEditor(Parent.CurrentNode);
					this.CurrentEditor.Validating += this.EditorValidating;
					this.CurrentEditor.KeyDown += this.EditorKeyDown;
					this.EditNode = Parent.CurrentNode;
					Parent.DisplayEditor(this.CurrentEditor, this);
				}
			}
		}

		private void EditorKeyDown(object sender, KeyEventArgs e)
		{
		    switch (e.KeyCode)
		    {
		        case Keys.Escape:
		            this.EndEdit(false);
		            break;
		        case Keys.Enter:
		            this.EndEdit(true);
		            break;
		    }
		}

	    private void EditorValidating(object sender, CancelEventArgs e)
		{
			ApplyChanges();
		}

		internal void HideEditor(Control editor)
		{
			editor.Validating -= this.EditorValidating;
			editor.Parent = null;
			editor.Dispose();
			this.EditNode = null;
			OnEditorHided();
		}

		public void EndEdit(bool applyChanges)
		{
			if (!applyChanges)
				this.CurrentEditor.Validating -= this.EditorValidating;
			Parent.Focus();
		}

		public virtual void UpdateEditor(Control control)
		{
		}

		public virtual void ApplyChanges()
		{
			try
			{
				DoApplyChanges(this.EditNode, this.CurrentEditor);
			}
			catch (ArgumentException ex)
			{
				MessageBox.Show(ex.Message, "Value is not valid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}

		protected abstract void DoApplyChanges(TreeNodeAdv node, Control editor);

		protected abstract Control CreateEditor(TreeNodeAdv node);

		public override void MouseDown(TreeNodeAdvMouseEventArgs args)
		{
			_editFlag = (!this.EditOnClick && args.Button == MouseButtons.Left 
				&& args.ModifierKeys == Keys.None && args.Node.IsSelected);
		}

		public override void MouseUp(TreeNodeAdvMouseEventArgs args)
		{
			if (this.EditOnClick && args.Button == MouseButtons.Left && args.ModifierKeys == Keys.None)
			{
				BeginEdit();
				args.Handled = true;
			}
			else if (_editFlag && args.Node.IsSelected)
				_timer.Start();
		}

		public override void MouseDoubleClick(TreeNodeAdvMouseEventArgs args)
		{
			_editFlag = false;
			_timer.Stop();
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
				_timer.Dispose();
		}

		#region Events

		public event CancelEventHandler EditorShowing;
		protected void OnEditorShowing(CancelEventArgs args)
		{
			if (EditorShowing != null)
				EditorShowing(this, args);
		}

		public event EventHandler EditorHided;
		protected void OnEditorHided()
		{
			if (EditorHided != null)
				EditorHided(this, EventArgs.Empty);
		}

		#endregion
	}
}
