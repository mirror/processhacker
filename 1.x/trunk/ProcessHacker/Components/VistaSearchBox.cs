using System;
using System.ComponentModel;
using System.Drawing;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace ProcessHacker
{
	[DefaultEvent("TextChanged")]
	[DefaultProperty("Text")]
	public partial class VistaSearchBox : Control
	{
        private const string DefaultInactiveText = "Search";
        private string _inactiveText;
        
		private bool _active;

		private Color _hoverButtonColor;
		private Color _activeBackColor;
		private Color _activeForeColor;
		private Color _inactiveBackColor;
		private Color _inactiveForeColor;

		private Font _inactiveFont;

        protected override CreateParams CreateParams
        {
            [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            get
            {
                int WS_BORDER = 0x00800000;
                int WS_EX_CLIENTEDGE = 0x00000200;
                int WS_EX_CONTROLPARENT = 0x00010000;

                CreateParams createParams = base.CreateParams;
                createParams.ExStyle |= WS_EX_CONTROLPARENT;
                createParams.ExStyle &= ~WS_EX_CLIENTEDGE;

                // make sure WS_BORDER is present in the style
                createParams.Style |= WS_BORDER;

                return createParams;
            }
        }

        public VistaSearchBox()
        {
            _hoverButtonColor = SystemColors.GradientInactiveCaption;
            _activeBackColor = SystemColors.Window;
            _activeForeColor = SystemColors.WindowText;
            _inactiveBackColor = SystemColors.InactiveBorder;
            _inactiveForeColor = SystemColors.GrayText;

            _inactiveFont = new Font(this.Font, FontStyle.Italic);

            _inactiveText = DefaultInactiveText;

            InitializeComponent();

            BackColor = InactiveBackColor;
            ForeColor = InactiveForeColor;

            searchOverlayLabel.Font = InactiveFont;
            searchOverlayLabel.ForeColor = InactiveForeColor;
            searchOverlayLabel.BackColor = InactiveBackColor;
            searchOverlayLabel.Text = InactiveText;

            searchText.Font = Font;
            searchText.ForeColor = ActiveForeColor;
            searchText.BackColor = InactiveBackColor;

            _active = false;

            SetTextActive(false);
            SetActive(false);
        }
		
		#region Events

		public new event EventHandler TextChanged
		{
			add { searchText.TextChanged += value; }
			remove { searchText.TextChanged -= value; }
		}

		#endregion

		#region Properties

		[Category("Appearance")]
		[DefaultValue(typeof(Color), "GradientInactiveCaption")]
		public Color HoverButtonColor
		{
			get { return _hoverButtonColor; }
			set { _hoverButtonColor = value; }
		}

		[Category("Appearance")]
		[DefaultValue(typeof(Color), "WindowText")]
		public Color ActiveForeColor
		{
			get { return _activeForeColor; }
			set { _activeForeColor = value; }
		}

		[Category("Appearance")]
		[DefaultValue(typeof(Color), "Window")]
		public Color ActiveBackColor
		{
			get { return _activeBackColor; }
			set { _activeBackColor = value; }
		}

		[Category("Appearance")]
		[DefaultValue(typeof(Color), "GrayText")]
		public Color InactiveForeColor
		{
			get { return _inactiveForeColor; }
			set { _inactiveForeColor = value; }
		}

		[Category("Appearance")]
		[DefaultValue(typeof(Color), "InactiveBorder")]
		public Color InactiveBackColor
		{
			get { return _inactiveBackColor; }
			set { _inactiveBackColor = value; }
		}

		[Category("Appearance")]
		[DefaultValue(typeof(Cursor), "IBeam")]
		public override Cursor Cursor
		{
			get { return base.Cursor; }
			set { base.Cursor = value; }
		}

		[Browsable(false)]
		public override Color ForeColor
		{
			get { return base.ForeColor; }
			set { base.ForeColor = value; }
		}

		[Browsable(false)]
		public override Color BackColor
		{
			get 
            { 
                return base.BackColor;
            }
			set 
            { 
                base.BackColor = value;
            }
		}

		[Category("Appearance")]
        [DefaultValue(typeof(string), DefaultInactiveText)]
		public string InactiveText
		{
			get 
            {
                return _inactiveText; 
            }
			set
            { 
                _inactiveText = value;

                searchOverlayLabel.Text = value;
            }
		}

		[Category("Appearance")]
		[DefaultValue(typeof(Font), "Microsoft Sans Serif, 8.25pt")]
		public Font ActiveFont
		{
			get { return base.Font; }
			set { base.Font = value; }
		}

		[Category("Appearance")]
		[DefaultValue(typeof(Font), "Microsoft Sans Serif, 8.25pt, Italic")]
		public Font InactiveFont
		{
			get { return _inactiveFont; }
			set { _inactiveFont = value; }
		}

		[Browsable(false)]
		public override Font Font
		{
			get { return base.Font; }
			set { base.Font = value; }
		}

		[Category("Appearance")]
		public override string Text
		{
			get { return searchText.Text; }
			set { searchText.Text = value; }
		}

		protected bool TextEntered
		{
			get { return !String.IsNullOrEmpty(searchText.Text); }
		}

		#endregion

		#region Methods

		private void SetActive(bool value)
		{
			if (TextEntered)
				value = true;

			if (_active == value)
				return;

			this.BackColor = value ? ActiveBackColor : InactiveBackColor;
			this.ForeColor = value ? ActiveForeColor : InactiveForeColor;

			_active = value;
		}

		private void SetTextActive(bool value)
		{
			bool active = value || TextEntered;

			this.searchOverlayLabel.Visible = !active;
			this.searchText.Visible = active;

			if (value && !searchText.Focused)
				this.searchText.Select();
		}

		#endregion

		#region Event Methods

		protected override void OnGotFocus(EventArgs e)
		{
			SetTextActive(true);
			SetActive(true);

			base.OnGotFocus(e);
		}

		protected override void OnLostFocus(EventArgs e)
		{
			if (this.searchText.Focused)
				return;

			SetTextActive(false);
			SetActive(false);

			base.OnLostFocus(e);
		}

		protected override void OnClick(EventArgs e)
		{
			this.Select();

			base.OnClick(e);
		}

		protected override void OnForeColorChanged(EventArgs e)
		{
			this.searchText.ForeColor = this.ForeColor;

			base.OnForeColorChanged(e);
		}

		protected override void OnBackColorChanged(EventArgs e)
		{
			this.searchOverlayLabel.BackColor = this.BackColor;
			this.searchText.BackColor = this.BackColor;

			base.OnBackColorChanged(e);
		}

		protected override void OnTextChanged(EventArgs e)
		{
			searchImage.Image = TextEntered ? ProcessHacker.Properties.Resources.active_search : ProcessHacker.Properties.Resources.inactive_search;

			base.OnTextChanged(e);
		}

        [DllImport("user32.dll", EntryPoint = "ReleaseCapture")]
        public static extern bool StopMouseCapture();
        [DllImport("user32.dll", EntryPoint = "SetCapture")]
        public static extern IntPtr StartMouseCapture(IntPtr hWnd);

		private void searchImage_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (e.X < 0 || e.X > searchImage.Width || e.Y < 0 || e.Y > searchImage.Height)
			{
				StopMouseCapture();
				searchImage.BackColor = Color.Empty;
			}
			else
			{
				StartMouseCapture(searchImage.Handle);
				if (TextEntered)
					searchImage.BackColor = HoverButtonColor;
			}
		}

		private void searchImage_Click(object sender, System.EventArgs e)
		{
			if (TextEntered)
			{
				this.searchText.ResetText();
				OnLostFocus(EventArgs.Empty);
			}
		}

		private void searchText_TextChanged(object sender, EventArgs e)
		{
			OnTextChanged(e);
		}

		private void searchText_LostFocus(object sender, System.EventArgs e)
		{
			OnLostFocus(e);
		}

		private void searchText_GotFocus(object sender, System.EventArgs e)
		{
			OnGotFocus(e);
		}

        private void searchOverlayLabel_Click(object sender, EventArgs e)
        {
            OnClick(EventArgs.Empty);
        }

		#endregion

	}
}
