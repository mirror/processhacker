/*
 * Process Hacker - 
 *   CPU usage icon drawing code
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
using System.Diagnostics;
using System.Drawing;   
using System.Drawing.Drawing2D;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Aga.Controls.Tree;
using System.Collections.Generic;

namespace ProcessHacker
{
    public class UsageIcon
    {
        private List<float> _values;
        private int _width;
        private int _height;

        public UsageIcon(int width, int height)
        {
            _values = new List<float>();
            _width = width;
            _height = height;

            for (int i = 0; i < width; i++)
                _values.Add(0);
        }

        public void Update(float value)
        {
            if (value > 1)
                throw new ArgumentOutOfRangeException();

            // shift values left, push value onto the end
            _values.RemoveAt(0);
            _values.Add(value);
        }

        public Color BackColor { get; set; }

        public Color Color { get; set; }

        public Icon GetIcon()
        {
            using (Bitmap bm = new Bitmap(_width, _height))
            {
                using (Graphics g = Graphics.FromImage(bm))
                {
                    g.FillRectangle(new SolidBrush(this.BackColor), new Rectangle(0, 0, _width, _height));

                    for (int x = 0; x < _width; x++)
                    {
                        int height = (int)(_values[x] * _height);

                        g.DrawLine(new Pen(this.Color), new Point(x, _height), new Point(x, _height - height));
                    }

                    try
                    {
                        // HACK
                        // Seems to throw "generic GDI+ errors". Very helpful...
                        return Icon.FromHandle(bm.GetHicon());
                    }
                    catch
                    {
                        return Program.HackerWindow.Icon;
                    }
                }
            }
        }
    }
}
