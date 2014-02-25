/*
 * Process Hacker - 
 *   vertical progress bar
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
