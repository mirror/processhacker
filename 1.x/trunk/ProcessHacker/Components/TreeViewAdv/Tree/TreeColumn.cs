using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using System.Windows.Forms.VisualStyles;
using System.Drawing.Imaging;
using ProcessHacker.Common;

namespace Aga.Controls.Tree
{
	[TypeConverter(typeof(TreeColumnConverter)), DesignTimeVisible(false), ToolboxItem(false)]
	public class TreeColumn : Component
	{
		private class TreeColumnConverter : ComponentConverter
		{
			public TreeColumnConverter()
				: base(typeof(TreeColumn))
			{
			}

			public override bool GetPropertiesSupported(ITypeDescriptorContext context)
			{
				return false;
			}
		}

		private static int HeaderLeftMargin = 5;
        private static int HeaderRightMargin = 5;   
		private static int SortOrderMarkMargin = 8;

        private TextFormatFlags _headerFlags;
        private static TextFormatFlags _baseHeaderFlags = TextFormatFlags.NoPadding | 
                                                   TextFormatFlags.EndEllipsis |
                                                   TextFormatFlags.VerticalCenter |
												TextFormatFlags.PreserveGraphicsTranslateTransform;

		#region Properties

	    internal TreeColumnCollection Owner { get; set; }

	    [Browsable(false)]
		public int Index
		{
			get
			{
			    if (this.Owner != null)
					return this.Owner.IndexOf(this);
			    
                return -1;
			}
		}

		private string _header;
		[Localizable(true)]
		public string Header
		{
			get { return _header; }
			set 
			{ 
				_header = value;
				OnHeaderChanged();
			}
		}

	    [Localizable(true)]
	    public string TooltipText { get; set; }

	    private int _width;
		[DefaultValue(50), Localizable(true)]
		public int Width
		{
			get
            {
                return _width;
            }
			set 
			{
				if (_width != value)
				{
                    _width = Math.Max(MinColumnWidth, value);
                    if (_maxColumnWidth > 0)
                    {
                        _width = Math.Min(_width, MaxColumnWidth);
                    }
					OnWidthChanged();
				}
			}
		}

        private int _minColumnWidth;
        [DefaultValue(0)]
        public int MinColumnWidth
        {
            get { return _minColumnWidth; }
            set
            {
				if (value < 0)
					throw new ArgumentOutOfRangeException("value");

				_minColumnWidth = value;
                Width = Math.Max(value, Width);
            }
        }

        private int _maxColumnWidth;
        [DefaultValue(0)]
        public int MaxColumnWidth
        {
            get { return _maxColumnWidth; }
            set
            {
				if (value < 0)
					throw new ArgumentOutOfRangeException("value");

				_maxColumnWidth = value;
				if (value > 0)
					Width = Math.Min(value, _width);
            }
        }

		private bool _visible = true;
		[DefaultValue(true)]
		public bool IsVisible
		{
			get { return _visible; }
			set 
			{ 
				_visible = value;
				OnIsVisibleChanged();
			}
		}

		private HorizontalAlignment _textAlign = HorizontalAlignment.Left;
		[DefaultValue(HorizontalAlignment.Left)]
		public HorizontalAlignment TextAlign
		{
			get { return _textAlign; }
			set 
			{
				if (value != _textAlign)
				{
					_textAlign = value;
                    _headerFlags = _baseHeaderFlags | TextHelper.TranslateAligmentToFlag(value);
					OnHeaderChanged();
				}
			}
		}

	    [DefaultValue(false)]
	    public bool Sortable { get; set; }

	    private SortOrder _sort_order = SortOrder.None;
		public SortOrder SortOrder
		{
			get { return _sort_order; }
			set
			{
				if (value == _sort_order)
					return;
				_sort_order = value;
				OnSortOrderChanged();
			}
		}

		public Size SortMarkSize
		{
			get
			{
                if (ExplorerVisualStyle.VisualStylesEnabled)
					return new Size(9, 5);
			    
                return new Size(7, 4);
			}
		}
		#endregion

		public TreeColumn(): 
			this(string.Empty, 50)
		{
		}

        public TreeColumn(string header, int width)
		{
			_header = header;
			_width = width;
            _headerFlags = _baseHeaderFlags | TextFormatFlags.Left;
		}

		public override string ToString()
		{
		    if (string.IsNullOrEmpty(Header))
				return GetType().Name;
		    
            return this.Header;
		}

	    #region Draw

        private static VisualStyleRenderer _normalRenderer;
        private static VisualStyleRenderer _hotRenderer;
		private static VisualStyleRenderer _pressedRenderer;

		internal Bitmap CreateGhostImage(Rectangle bounds, Font font)
		{
			Bitmap b = new Bitmap(bounds.Width, bounds.Height, PixelFormat.Format32bppArgb);

            using (Graphics gr = Graphics.FromImage(b))
            {
                gr.FillRectangle(SystemBrushes.ControlDark, bounds);

                DrawContent(gr, bounds, font);

                BitmapHelper.SetAlphaChanelValue(b, 150);
            }

		    return b;
		}

		internal void Draw(Graphics gr, Rectangle bounds, Font font, bool pressed, bool hot)
        {
			DrawBackground(gr, bounds, pressed, hot);
			DrawContent(gr, bounds, font);
		}

        internal void DrawContent(Graphics gr, Rectangle bounds, Font font)
        {
            Rectangle innerBounds = new Rectangle(bounds.X + HeaderLeftMargin, bounds.Y, bounds.Width - HeaderLeftMargin - HeaderRightMargin, bounds.Height);

            if (this.SortOrder != SortOrder.None)
                innerBounds.Width -= (this.SortMarkSize.Width + SortOrderMarkMargin);

            Size maxTextSize = gr.GetCachedSize(this.Header, font);// innerBounds.Size, TextFormatFlags.NoPadding);
            Size textSize = gr.GetCachedSize(this.Header, font);// innerBounds.Size, _baseHeaderFlags);

            if (SortOrder != SortOrder.None)
            {
				int tw = Math.Min(textSize.Width, innerBounds.Size.Width);

                int x;

                switch (this.TextAlign)
                {
                    case HorizontalAlignment.Left:
                        {
                            x = innerBounds.X + tw + SortOrderMarkMargin;
                            break;
                        }
                    case HorizontalAlignment.Right:
                        {
                            x = innerBounds.Right + SortOrderMarkMargin;
                            break;
                        }
                    default:
                        {
                            x = innerBounds.X + tw + (innerBounds.Width - tw) / 2 + SortOrderMarkMargin;
                            break;
                        }
                }

                DrawSortMark(gr, bounds, x);
			}

            if (textSize.Width < maxTextSize.Width)
                TextRenderer.DrawText(gr, Header, font, innerBounds, SystemColors.ControlText, _baseHeaderFlags | TextFormatFlags.Left);
            else
                TextRenderer.DrawText(gr, Header, font, innerBounds, SystemColors.ControlText, _headerFlags);
        }

		private void DrawSortMark(Graphics gr, Rectangle bounds, int x)
		{
			int y = bounds.Y + bounds.Height / 2 - 2;
			x = Math.Max(x, bounds.X + SortOrderMarkMargin);

            int w2 = SortMarkSize.Width / 2;
            
            switch (this.SortOrder)
            {
                case SortOrder.Ascending:
                    {
                        Point[] points = new[] 
                        { 
                            new Point(x, y), 
                            new Point(x + this.SortMarkSize.Width, y), 
                            new Point(x + w2, y + this.SortMarkSize.Height)
                        };

                        gr.FillPolygon(SystemBrushes.ControlDark, points);
                    }
                    break;
                case SortOrder.Descending:
                    {
                        Point[] points = new[] 
                        { 
                            new Point(x - 1, y + this.SortMarkSize.Height), 
                            new Point(x + this.SortMarkSize.Width, y + this.SortMarkSize.Height), 
                            new Point(x + w2, y - 1) 
                        };

                        gr.FillPolygon(SystemBrushes.ControlDark, points);
                    }
                    break;
            }
		}

		internal static void DrawDropMark(Graphics gr, Rectangle rect)
		{
			gr.FillRectangle(SystemBrushes.HotTrack, rect.X-1, rect.Y, 2, rect.Height);
		}

		internal static void DrawBackground(Graphics gr, Rectangle bounds, bool pressed, bool hot)
        {
            if (ExplorerVisualStyle.VisualStylesEnabled)
			{
                if (_normalRenderer == null)
                    _normalRenderer = new VisualStyleRenderer(VisualStyleElement.Header.Item.Normal);
               
                if (_hotRenderer == null)
                    _hotRenderer = new VisualStyleRenderer(VisualStyleElement.Header.Item.Hot);
               
                if (_pressedRenderer == null)
                    _pressedRenderer = new VisualStyleRenderer(VisualStyleElement.Header.Item.Pressed);

				if (pressed)
					_pressedRenderer.DrawBackground(gr, bounds);
				else if (hot)
					_hotRenderer.DrawBackground(gr, bounds);
				else
					_normalRenderer.DrawBackground(gr, bounds);
			}
            else
            {
                gr.Clear(SystemColors.Control);

                Pen p1 = SystemPens.ControlLightLight;
                Pen p2 = SystemPens.ControlDark;
                Pen p3 = SystemPens.ControlDarkDark;

                if (pressed)
                    gr.DrawRectangle(p2, bounds.X, bounds.Y, bounds.Width, bounds.Height);
                else
                {
                    gr.DrawLine(p1, bounds.X, bounds.Y, bounds.Right, bounds.Y);
                    gr.DrawLine(p3, bounds.X, bounds.Bottom, bounds.Right, bounds.Bottom);
                    gr.DrawLine(p3, bounds.Right - 1, bounds.Y, bounds.Right - 1, bounds.Bottom - 1);
                    gr.DrawLine(p1, bounds.Left, bounds.Y + 1, bounds.Left, bounds.Bottom - 2);
                    gr.DrawLine(p2, bounds.Right - 2, bounds.Y + 1, bounds.Right - 2, bounds.Bottom - 2);
                    gr.DrawLine(p2, bounds.X, bounds.Bottom - 1, bounds.Right - 2, bounds.Bottom - 1);
                }
            }
		}

		#endregion

		#region Events

		public event EventHandler HeaderChanged;
		private void OnHeaderChanged()
		{
			if (HeaderChanged != null)
				HeaderChanged(this, EventArgs.Empty);
		}

		public event EventHandler SortOrderChanged;
		private void OnSortOrderChanged()
		{
			if (SortOrderChanged != null)
				SortOrderChanged(this, EventArgs.Empty);
		}

		public event EventHandler IsVisibleChanged;
		private void OnIsVisibleChanged()
		{
			if (IsVisibleChanged != null)
				IsVisibleChanged(this, EventArgs.Empty);
		}

		public event EventHandler WidthChanged;
		private void OnWidthChanged()
		{
			if (WidthChanged != null)
				WidthChanged(this, EventArgs.Empty);
		}

		#endregion
	}
}
