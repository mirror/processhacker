/*
 * Process Hacker
 *                 
 * Copyright (C) 2008 wj32
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
        private List<float> ListData1 = new List<float>();
        private List<float> ListData2 = new List<float>();

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
            int start = ListData1.Count - 2;
            Pen lGrid1 = new Pen(_lineColor1);
            Pen lGrid2 = new Pen(_lineColor2);

            while (start >= 0)
            {
                float f = ListData1[start];
                float fPre = ListData1[start + 1];
                int h = (int)(tHeight - (tHeight * f));
                int hPre = (int)(tHeight - (tHeight * fPre));

                // fill in the area below the line
                g.FillPolygon(new SolidBrush(Color.FromArgb(100, _lineColor1)),
                    new Point[] { new Point(px, h), new Point(px + _moveStep, hPre), 
                        new Point(px + _moveStep, tHeight), new Point(px, tHeight) });
                g.DrawLine(lGrid1, px, h, px + _moveStep, hPre);

                if (this.UseSecondLine)
                {
                    f = ListData2[start];
                    fPre = ListData2[start + 1];
                    h = (int)(tHeight - (tHeight * f));
                    hPre = (int)(tHeight - (tHeight * fPre));

                    // draw the second line
                    g.FillPolygon(new SolidBrush(Color.FromArgb(100, _lineColor2)),
                        new Point[] { new Point(px, h), new Point(px + _moveStep, hPre), 
                        new Point(px + _moveStep, tHeight), new Point(px, tHeight) });
                    g.DrawLine(lGrid2, px, h, px + _moveStep, hPre);
                }

                if (px < 0)
                {
                    break;
                }
                px -= _moveStep;
                start--;
            }

            // draw the text
            if (this.Text != "" && this.Text != null)
            {
                Font font = new Font(FontFamily.GenericSansSerif, 8);
                Size textSize = TextRenderer.MeasureText(this.Text, font);
                int xMargin = 3;
                int yMargin = 3;
                int xPadding = 5;
                int yPadding = 3;

                // draw the background for the text
                g.FillRectangle(new SolidBrush(Color.FromArgb(127, Color.Black)),
                    new Rectangle(this.Size.Width - textSize.Width - xPadding * 2 - xMargin, yMargin,
                        textSize.Width + xPadding * 2, textSize.Height + yPadding * 2));
                TextRenderer.DrawText(g, this.Text, font,
                    new Point(this.Size.Width - textSize.Width - xPadding - xMargin, yPadding + yMargin), _textColor);
            }

            g.Dispose();
        }

        /// <summary>
        /// reset
        /// </summary>
        public void ReSet()
        {
            _gridStartPos = 0;
            ListData1.Clear();
            ListData2.Clear();
            PaintLine();
        }

        /// <summary>
        /// Adds two numbers to the lists of data.
        /// </summary>
        /// <param name="f1">A floating-point number less than or equal to 1.</param> 
        /// <param name="f2">A floating-point number less than or equal to 1.</param>
        public void Add(float f1, float f2)
        {
            ListData1.Add(f1);
            ListData2.Add(f2);

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

        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
                this.Invalidate();
            }
        }

        private Color _textColor = Color.FromArgb(0, 255, 0);
        public Color TextColor
        {
            get { return _textColor; }
            set { _textColor = value; }
        }

        public bool UseSecondLine { get; set; }

        private Color _lineColor1 = Color.FromArgb(0, 255, 0);
        public Color LineColor1
        {
            get { return _lineColor1; }
            set { _lineColor1 = value; }
        }

        private Color _lineColor2 = Color.FromArgb(255, 0, 0);
        public Color LineColor2
        {
            get { return _lineColor2; }
            set { _lineColor2 = value; }
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
