/* 
 * Process Hacker - 
 *   indicator component
 * 
 * Copyright (C) 2009 Dean
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
using System.Drawing.Drawing2D;

namespace ProcessHacker.Components
{
    public partial class Indicator : UserControl
    {
        private Color _lineColor1 = Color.FromArgb(255, 0, 0);
        public Color Color1
        {
            get { return _lineColor1; }
            set { _lineColor1 = value; }
        }
        private Color _lineColor2 = Color.FromArgb(0, 255, 0);
        public Color Color2
        {
            get { return _lineColor2; }
            set { _lineColor2 = value; }
        }
        private long _data1;
        public long Data1
        {
            get { return _data1; }
            set { _data1 = value; }
        }
        private long _data2;
        public long Data2
        {
            get { return _data2; }
            set { _data2 = value; }
        }

        

        public Indicator()
        {
            InitializeComponent();
            base.SetStyle(ControlStyles.ResizeRedraw | ControlStyles.UserPaint | 
                ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);                    
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            //SolidBrush brush = new SolidBrush(this.ForeColor);
            SolidBrush brush1 = new SolidBrush(this.Color1);
            SolidBrush brush2 = new SolidBrush(this.Color2);
            int width = base.ClientSize.Width;
            int height = base.ClientSize.Height;
            int num1 = height;
            num1 -= this.Font.Height + 4;
            RectangleF layoutRectangle = new RectangleF(0f, (float)num1, (float)width, (float)height);
            StringFormat format = new StringFormat(StringFormatFlags.NoWrap);
            format.Alignment = StringAlignment.Center;
            e.Graphics.DrawString(this.Text, this.Font, brush1, layoutRectangle, format);
            int num2 = (((height - 6) - 4) - this.Font.Height) - 4;
            num2++;
            int num3 = num2 / 3;
            byte red = (byte)(this.ForeColor.R / 2);
            byte green = (byte)(this.ForeColor.G / 2);
            byte blue = (byte)(this.ForeColor.B / 2);

            Pen pen = new Pen(Color.FromArgb(red, green, blue));
            pen.DashStyle = DashStyle.Dot;

            int num4 = (width - this.GraphWidth) / 2;
            int num5 = ((height - 4) - this.Font.Height) - 7;
            int num6 = this.GraphWidth / 2;
            int x = num4;
            int y = 0;
            int num7 = 0;
            int num8 = 0;
            int num9 = (int)Math.Ceiling((double)(((this.Data1 - this.Minimum) * 1.0) / ((this.Maximum - (this.Minimum * 1.0)) / ((double)num3))));
            int num10 = (int)Math.Ceiling((double)(((this.Data1 + this.Data2 - this.Minimum) * 1.0) / ((this.Maximum - (this.Minimum * 1.0)) / ((double)num3))));

            for (int i = 0; i < num3; i++)
            {
                x = num4;
                y = (num5 - (i * 3)) - 1;
                num7 = x + num6;
                num8 = y;
                if (i < num9)
                {
                    e.Graphics.FillRectangle(brush1, x, y, num6, 2);
                    e.Graphics.FillRectangle(brush1, num7 + 1, y, num6, 2);
                }
                else if (i < num10)
                {
                    e.Graphics.FillRectangle(brush2, x, y, num6, 2);
                    e.Graphics.FillRectangle(brush2, num7 + 1, y, num6, 2);
                }
                else
                {
                    e.Graphics.DrawLine(pen, x, y, num7, num8);
                    e.Graphics.DrawLine(pen, x + 1, y + 1, num7, num8 + 1);
                    x = num7 + 1;
                    num7 = x + num6;
                    e.Graphics.DrawLine(pen, x, y, num7, num8);
                    e.Graphics.DrawLine(pen, x + 1, y + 1, num7, num8 + 1);
                }
            }
            pen.Dispose();
            //brush.Dispose();
            brush1.Dispose();
            brush2.Dispose();
        }
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams createParams = base.CreateParams;
                createParams.ExStyle |= 0x200;
                return createParams;
            }
        }
        private int _GraphWidth=0x21;
        private long _Maximum = long.MaxValue;
        private long _Minimum = 0;
        public int GraphWidth
        {
            get
            {
                return this._GraphWidth;
            }
            set
            {
                this._GraphWidth = value;
                base.Invalidate();
            }
        }
        public long Maximum
        {
            get
            {
                return this._Maximum;
            }
            set
            {
                if (value < this.Minimum)
                {
                    //throw new ArgumentException();
                    return;
                }

                this._Maximum = value;
                base.Invalidate();
            }
        }
        public long Minimum
        {
            get
            {
                return this._Minimum;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException();
                }
                this._Minimum = value;
                base.Invalidate();
            }
        }
        public string TextValue
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
                base.Invalidate();
            }
        }
    }
}

