/*
 * Process Hacker - 
 *   user-friendly color modifier control
 * 
 * Copyright (C) 2008 wj32
 * 
 * This file is part of Process Hacker.
 * 
 * Process Hacker is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * Process Hacker is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Process Hacker.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ProcessHacker.Components
{
    public partial class ColorModifier : UserControl
    {
        public event EventHandler ColorChanged;

        private Color _color;

        public ColorModifier()
        {
            InitializeComponent();
        }

        private void panelColor_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();

            cd.Color = panelColor.BackColor;
            cd.FullOpen = true;

            if (cd.ShowDialog() == DialogResult.OK)
            {
                _color = cd.Color;
                panelColor.BackColor = cd.Color;

                if (this.ColorChanged != null)
                    this.ColorChanged(this, new EventArgs());
            }
        }

        public Color Color
        {
            get { return _color; }
            set
            {
                _color = value;
                panelColor.BackColor = value;

                if (this.ColorChanged != null)
                    this.ColorChanged(this, new EventArgs());
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
