/*
 * modified by wj32.
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using ProcessHacker.Common;

namespace Aga.Controls.Tree.NodeControls
{
	public abstract class BaseTextControl : EditableControl
	{
		private readonly TextFormatFlags _baseFormatFlags;
        private TextFormatFlags _formatFlags;
        private readonly Pen _focusPen;
		private readonly StringFormat _format;

		#region Properties

		private Font _font;
		public Font Font
		{
			get { return this._font ?? Control.DefaultFont; }
		    set { this._font = value == Control.DefaultFont ? null : value; }
		}

		protected bool ShouldSerializeFont()
		{
			return (_font != null);
		}

		private HorizontalAlignment _textAlign = HorizontalAlignment.Left;
		[DefaultValue(HorizontalAlignment.Left)]
		public HorizontalAlignment TextAlign
		{
			get { return _textAlign; }
			set 
			{ 
				_textAlign = value;
				SetFormatFlags();
			}
		}

		private StringTrimming _trimming = StringTrimming.None;
		[DefaultValue(StringTrimming.None)]
		public StringTrimming Trimming
		{
			get { return _trimming; }
			set 
			{ 
				_trimming = value;
				SetFormatFlags();
			}
		}

		private bool _displayHiddenContentInToolTip = true;
		[DefaultValue(true)]
		public bool DisplayHiddenContentInToolTip
		{
			get { return _displayHiddenContentInToolTip; }
			set { _displayHiddenContentInToolTip = value; }
		}

	    [DefaultValue(false)]
	    public bool UseCompatibleTextRendering { get; set; }

	    #endregion

		protected BaseTextControl()
		{
			this.IncrementalSearchEnabled = true;

			_focusPen = new Pen(Color.Black) 
            {
                DashStyle = System.Drawing.Drawing2D.DashStyle.Dot
            };

		    _format = new StringFormat(StringFormatFlags.NoClip | StringFormatFlags.FitBlackBox | StringFormatFlags.MeasureTrailingSpaces);
			_baseFormatFlags = TextFormatFlags.PreserveGraphicsClipping | TextFormatFlags.NoPrefix | TextFormatFlags.PreserveGraphicsTranslateTransform;
			SetFormatFlags();
			LeftMargin = 3;
		}

		private void SetFormatFlags()
		{
			_format.Alignment = TextHelper.TranslateAligment(TextAlign);
			_format.Trimming = Trimming;

			_formatFlags = _baseFormatFlags | TextHelper.TranslateAligmentToFlag(TextAlign)| TextHelper.TranslateTrimmingToFlag(Trimming);
		}

		public override Size MeasureSize(TreeNodeAdv node, DrawContext context)
		{
			return GetLabelSize(node, context);
		}

		protected Size GetLabelSize(TreeNodeAdv node, DrawContext context)
		{
			return GetLabelSize(node, context, GetLabel(node));
		}

		protected Size GetLabelSize(TreeNodeAdv node, DrawContext context, string label)
		{
			Font font = GetDrawingFont(node, context, label);

		    Size s = context.Graphics.GetCachedSize(label, font);
  
			if (!s.IsEmpty)
				return s;
		    
            return new Size(10, this.Font.Height);
		}

		protected Font GetDrawingFont(TreeNodeAdv node, DrawContext context, string label)
		{
			Font font = context.Font;
			
            if (this.DrawText != null)
			{
				DrawEventArgs args = new DrawEventArgs(node, context, label) 
                {
                    Font = context.Font
                };

			    OnDrawText(args);
				font = args.Font;
			}

			return font;
		}

		protected void SetEditControlProperties(Control control, TreeNodeAdv node)
		{
			string label = GetLabel(node);
			
            DrawContext context = new DrawContext 
            {
                Font = control.Font
            };

		    control.Font = GetDrawingFont(node, context, label);
		}

		public override void Draw(TreeNodeAdv node, DrawContext context)
		{
			if (context.CurrentEditorOwner == this && node == Parent.CurrentNode)
				return;

			string label = GetLabel(node);

			Rectangle bounds = GetBounds(node, context);
			Rectangle focusRect = new Rectangle(bounds.X, context.Bounds.Y,	bounds.Width, context.Bounds.Height);

			Brush backgroundBrush;
			Color textColor;
			Font font;

			CreateBrushes(node, context, label, out backgroundBrush, out textColor, out font, ref label);

            if (backgroundBrush != null)
            {
                context.Graphics.FillRectangle(backgroundBrush, focusRect);
            }

		    if (context.DrawFocus)
			{
				focusRect.Width--;
				focusRect.Height--;

                //switch (context.DrawSelection)
                //{
                //    case DrawSelectionMode.None:
                //        this._focusPen.Color = SystemColors.ControlText;
                //        break;
                //    default:
                //        this._focusPen.Color = SystemColors.InactiveCaption;
                //        break;
                //}

				context.Graphics.DrawRectangle(_focusPen, focusRect);
			}
            if (!this.UseCompatibleTextRendering)
            {
                context.Graphics.DrawString(label, font, GetFrush(textColor), bounds, _format);
            }
            else
            {
                TextRenderer.DrawText(context.Graphics, label, font, bounds, textColor, _formatFlags);
            }
		}

		private static Dictionary<Color, Brush> _brushes = new Dictionary<Color,Brush>();
		private static Brush GetFrush(Color color)
		{
		    if (_brushes.ContainsKey(color))
				return _brushes[color];
		    
            Brush br = new SolidBrush(color);

		    _brushes.Add(color, br);

		    return br;
		}

		private void CreateBrushes(TreeNodeAdv node, DrawContext context, string text, out Brush backgroundBrush, out Color textColor, out Font font, ref string label)
		{
            //textColor = SystemColors.ControlText;
            // wj32: respect node ForeColor
            textColor = node.ForeColor;

			backgroundBrush = null;
			font = context.Font;

			switch (context.DrawSelection)
			{
                case DrawSelectionMode.Active:
			        {
			            //textColor = SystemColors.HighlightText;
			            //backgroundBrush = SystemBrushes.Highlight;
			            break;
			        }
                case DrawSelectionMode.Inactive:
			        {
			            //textColor = SystemColors.ControlText;
			            //backgroundBrush = SystemBrushes.InactiveBorder;
			            break;
			        }
                case DrawSelectionMode.FullRowSelect:
			        {
			            //textColor = SystemColors.HighlightText;
			            break;
			        }
			}

			//if (!context.Enabled)
				//textColor = SystemColors.GrayText;

			if (this.DrawText != null)
			{
                //DrawEventArgs args = new DrawEventArgs(node, context, text) 
                //{
                //    TextColor = textColor, 
                //    BackgroundBrush = backgroundBrush, 
                //    Font = font
                //};

			    //OnDrawText(args);

				//textColor = args.TextColor;
				//backgroundBrush = args.BackgroundBrush;
				//font = args.Font;
				//label = args.Text;
			}
		}

		public string GetLabel(TreeNodeAdv node)
		{
		    if (node != null && node.Tag != null)
		    {
		        object obj = GetValue(node);

		        if (obj != null)
		            return FormatLabel(obj);
		    }

		    return string.Empty;
		}

	    protected virtual string FormatLabel(object obj)
		{
			return obj.ToString();
		}

		public void SetLabel(TreeNodeAdv node, string value)
		{
			SetValue(node, value);
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);

			if (disposing)
			{
				_focusPen.Dispose();
				_format.Dispose();
			}
		}

		/// <summary>
		/// Fires when control is going to draw a text. Can be used to change text or back color
		/// </summary>
		public event EventHandler<DrawEventArgs> DrawText;
		protected virtual void OnDrawText(DrawEventArgs args)
		{
			if (DrawText != null)
				DrawText(this, args);
		}
	}
}
