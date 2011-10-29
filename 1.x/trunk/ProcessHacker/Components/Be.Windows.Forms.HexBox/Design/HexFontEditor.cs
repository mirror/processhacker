using System;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Be.Windows.Forms.Design
{
	/// <summary>
	/// Display only fixed-piched fonts
	/// </summary>
	internal class HexFontEditor : FontEditor
	{
		object value;

	    /// <summary>
		/// Edits the value
		/// </summary>
		public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			this.value = value;
			if (provider != null)
			{
				IWindowsFormsEditorService service1 = (IWindowsFormsEditorService) provider.GetService(typeof(IWindowsFormsEditorService));
				if (service1 != null)
				{
                    FontDialog fontDialog = new FontDialog
                    {
                        ShowApply = false,
                        ShowColor = false,
                        AllowVerticalFonts = false,
                        AllowScriptChange = false,
                        FixedPitchOnly = true,
                        ShowEffects = false,
                        ShowHelp = false
                    };

				    Font font = value as Font;
					if(font != null)
					{
						fontDialog.Font = font;
					}
					if (fontDialog.ShowDialog() == DialogResult.OK)
					{
						this.value = fontDialog.Font;
					}

					fontDialog.Dispose();
				}
			}

			value = this.value;
			this.value = null;
			return value;
		}

		public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.Modal;
		}
	}
}
