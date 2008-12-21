/*
 * Process Hacker
 * 
 * Copyright (C) 2008 Dean
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ProcessHacker.Components
{
    public partial class Plotter : UserControl
    {
        public Plotter()
        {
            InitializeComponent();
            //SetStyle(ControlStyles.UserPaint, true);
            //SetStyle(ControlStyles.AllPaintingInWmPaint, true); 
            SetStyle(ControlStyles.DoubleBuffer, true); 
            PaintLine();
        }

        private int _gridStartPos = 0;
        private List<float> ListData = new List<float>();



        private void Plotter_Paint(object sender, PaintEventArgs e)
        {
            //e.Graphics
            PaintLine();
        }

        private void PaintLine()
        {
            int tWidth=this.Width;
            int tHeight=this.Height;

            Graphics g = this.CreateGraphics();
            //g.Clear(this.BackColor);
            g.FillRectangle(new SolidBrush(this.BackColor), 0, 0, tWidth, tHeight);

            //draw grid
            int x = tWidth / _gridSize.Width;
            int y = tHeight / _gridSize.Height;

            Pen pGrid = new Pen(_gridColor);
            int pos;

            for (int i = 0; i <= x; i++)
            {
                pos = tWidth - (i * _gridSize.Width + _gridStartPos - 1);
                g.DrawLine(pGrid, pos, 0, pos, tHeight);
            }

            for (int i = 0; i <= y; i++)
            {
                pos = i * _gridSize.Height - 1;
                g.DrawLine(pGrid, 0, pos, tWidth, pos);
            }

            //draw line
            int px = tWidth - _moveStep;
            int start = ListData.Count - 2;
            Pen lGrid = new Pen(_lineColor);
            while (start >= 0)
            {
                float f = (float)ListData[start];
                float fPre = (float)ListData[start + 1];
                int h = (int)(tHeight - (tHeight * f));
                int hPre = (int)(tHeight - (tHeight * fPre));
                g.DrawLine(lGrid, px, h, px + _moveStep, hPre);
                if (px < 0)
                {
                    break;
                }
                px -= _moveStep;
                start--;
            }
            g.Dispose();
        }

        /// <summary>
        /// reset
        /// </summary>
        public void ReSet()
        {
            _gridStartPos = 0;
            ListData.Clear();
            PaintLine();
        }

        /// <summary>
        /// add a CpuUsage data (less than 1)
        /// </summary>
        public void Add(float f)
        {
            ListData.Add(f);
            if (_isMoved)
            {
                _gridStartPos += _moveStep;
                if (_gridStartPos >= _gridSize.Width)
                {
                    _gridStartPos -= _gridSize.Width;
                }
            }
            PaintLine();
        }

        private Color _lineColor = Color.FromArgb(0, 255, 0);
        public Color LineColor
        {
            get { return _lineColor; }
            set { _lineColor = value; }
        }

        private Color _gridColor = Color.Black;
        public Color GridColor
        {
            get { return _gridColor; }
            set { _gridColor = value; }
        }

        private Size _gridSize = new Size(12, 12);
        public Size GridSize
        {
            get { return _gridSize; }
            set { _gridSize = value; }
        }

        private int _moveStep = 3;
        public int MoveStep
        {
            get { return _moveStep; }
            set { _moveStep = value; }
        }

        private bool _isMoved = true;
        public bool IsMoved
        {
            get { return _isMoved; }
            set { _isMoved = value; }
        }

        private void Plotter_Resize(object sender, EventArgs e)
        {
            PaintLine();
        }
    }
}
