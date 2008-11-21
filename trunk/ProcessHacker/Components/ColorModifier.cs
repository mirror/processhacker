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
        public ColorModifier()
        {
            InitializeComponent();
        }

        private void panelColor_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();

            cd.Color = panelColor.BackColor;

            if (cd.ShowDialog() == DialogResult.OK)
                panelColor.BackColor = cd.Color;
        }

        public Color Color
        {
            get { return panelColor.BackColor; }
            set { panelColor.BackColor = value; }
        }
    }
}
