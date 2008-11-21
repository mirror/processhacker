using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ProcessHacker.Components
{
    public partial class ColorModifier : UserControl
    {
        private Color _color;

        public ColorModifier()
        {
            InitializeComponent();
        }

        private void panelColor_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();

            cd.Color = panelColor.BackColor;

            if (cd.ShowDialog() == DialogResult.OK)
            {
                _color = cd.Color;
                panelColor.BackColor = cd.Color;
            }
        }

        public Color Color
        {
            get { return _color; }
            set
            {
                _color = value;
                panelColor.BackColor = value;
            }
        }

        private void panelColor_MouseEnter(object sender, EventArgs e)
        {
            panelColor.BackColor = Color.FromArgb(0xcc, _color);
        }

        private void panelColor_MouseLeave(object sender, EventArgs e)
        {
            panelColor.BackColor = _color;
        }
    }
}
