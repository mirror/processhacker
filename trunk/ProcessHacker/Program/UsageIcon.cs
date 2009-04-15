/*
 * Process Hacker - 
 *   CPU usage icon drawing code
 * 
 * Copyright (C) 2008-2009 wj32
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
using ProcessHacker.Components;

namespace ProcessHacker
{
    public class UsageIcon
    {
        private HistoryManager<bool, float> _history = new HistoryManager<bool, float>();
        private Plotter _plotter;
        private int _width;
        private int _height;

        public UsageIcon(int width, int height)
        {
            _history.Add(true);
            _history.Add(false);

            _width = width;
            _height = height;
            _plotter = new Plotter()
            {
                Size = new Size(_width, _height),
                UseSecondLine = true,
                ShowGrid = false,
                BackColor = Color.Black,
                MoveStep = 2,
                Data1 = _history[true],
                Data2 = _history[false]
            };
        }

        public void Update(float k, float u)
        {
            _history.Update(true, k);
            _history.Update(false, u);
        }

        public Color BackColor { get; set; }

        public Color Color { get; set; }

        public Icon GetIcon()
        {
            _plotter.LineColor1 = Properties.Settings.Default.PlotterCPUKernelColor;
            _plotter.LineColor2 = Properties.Settings.Default.PlotterCPUUserColor;

            using (Bitmap bm = new Bitmap(_width, _height))
            {
                _plotter.Draw();
                _plotter.DrawToBitmap(bm, new Rectangle(new Point(0, 0), bm.Size));

                return Icon.FromHandle(bm.GetHicon());
            }
        }
    }
}
