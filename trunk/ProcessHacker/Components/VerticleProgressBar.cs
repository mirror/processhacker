using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace ProcessHacker.Components
{
    public partial class VerticleProgressBar : UserControl
    {
        public VerticleProgressBar()
        {
            InitializeComponent();
        }

        private float _value;
        public float Value
        {
            get { return _value; }
            set { _value = value; this.Invalidate(); }
        }

        private void VerticleProgressBar_Paint(object sender, PaintEventArgs e)
        {
            VisualStyleRenderer rBar = new VisualStyleRenderer(VisualStyleElement.ProgressBar.BarVertical.Normal);

            rBar.DrawBackground(e.Graphics, e.ClipRectangle);

            VisualStyleRenderer rChunk = new VisualStyleRenderer(VisualStyleElement.ProgressBar.ChunkVertical.Normal);

            rChunk.DrawBackground(e.Graphics, new Rectangle(
                new Point(e.ClipRectangle.Left + 1, e.ClipRectangle.Top + 1 + (int)(this.Size.Height * (1 - _value))),
                new Size(this.Size.Width - 2, (int)(this.Size.Height * _value) - 2)));
        }
    }
}
