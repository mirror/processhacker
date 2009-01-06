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
using System.Drawing.Drawing2D;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ProcessHacker.Components
{
    public partial class Plotter : UserControl
    {
        private const BufferedGraphics NO_MANAGED_BACK_BUFFER = null;
        private BufferedGraphicsContext _graphicManager;
        private BufferedGraphics _managedBackBuffer;

        public Plotter()
        {
            InitializeComponent();

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            _graphicManager = BufferedGraphicsManager.Current;
            _graphicManager.MaximumBuffer =
                new Size(this.Width + 1, this.Height + 1);
            _managedBackBuffer =
                _graphicManager.Allocate(this.CreateGraphics(), this.ClientRectangle);

            Draw();
        }

        private int _gridStartPos = 0;
        private List<float> _listData1 = new List<float>();
        private List<float> _listData2 = new List<float>();

        private void Plotter_Paint(object sender, PaintEventArgs e)
        {
            _managedBackBuffer.Render(e.Graphics);
        }

        private void Draw()
        {
            int tWidth = this.Width;
            int tHeight = this.Height;

            Graphics g = _managedBackBuffer.Graphics;
            g.SmoothingMode = Properties.Settings.Default.PlotterAntialias ?
                SmoothingMode.AntiAlias : SmoothingMode.Default;

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

            //draw lines
            int px = tWidth - _moveStep;
            int start = _listData1.Count - 2;
            Pen lGrid1 = new Pen(_lineColor1);
            Pen lGrid2 = new Pen(_lineColor2);

            while (start >= 0)
            {
                float f = _listData1[start];
                float fPre = _listData1[start + 1];
                int h = (int)(tHeight - (tHeight * f));
                int hPre = (int)(tHeight - (tHeight * fPre));

                // fill in the area below the line
                g.FillPolygon(new SolidBrush(Color.FromArgb(100, _lineColor1)),
                    new Point[] { new Point(px, h), new Point(px + _moveStep, hPre), 
                        new Point(px + _moveStep, tHeight), new Point(px, tHeight) });
                g.DrawLine(lGrid1, px, h, px + _moveStep, hPre);

                if (this.UseSecondLine)
                {
                    f = _listData2[start];
                    fPre = _listData2[start + 1];
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

            // draw text
            if (this.Text != "" && this.Text != null)
            {
                // draw the background for the text
                g.FillRectangle(new SolidBrush(_textBoxColor),
                    new Rectangle(_boxPosition, _boxSize));

                // draw the text
                TextRenderer.DrawText(g, this.Text, this.Font, _textPosition, _textColor);
            }

            this.Refresh();
        }

        /// <summary>
        /// reset
        /// </summary>
        public void Reset()
        {
            _gridStartPos = 0;
            _listData1.Clear();
            _listData2.Clear();
            Draw();
        }

        /// <summary>
        /// Adds two numbers to the lists of data. The numbers represent a fraction 
        /// of the plotter's height.
        /// </summary>
        /// <param name="f1">A floating-point number less than or equal to 1.</param> 
        /// <param name="f2">A floating-point number less than or equal to 1.</param>
        public void Add(float f1, float f2)
        {
            if (f1 > 1.0f)
                f1 = 1.0f;
            if (f2 > 1.0f)
                f2 = 1.0f;
            if (f1 < 0.0f || f1 == float.NaN)
                f1 = 0.0f;
            if (f2 < 0.0f || f2 == float.NaN)
                f2 = 0.0f;

            _listData1.Add(f1);
            _listData2.Add(f2);

            if (_listData1.Count > this.ClientRectangle.Width / this.MoveStep + 1)
                _listData1.RemoveRange(0, _listData1.Count - 1 - this.ClientRectangle.Width / this.MoveStep);
            if (_listData2.Count > this.ClientRectangle.Width / this.MoveStep + 1)
                _listData2.RemoveRange(0, _listData2.Count - 1 - this.ClientRectangle.Width / this.MoveStep);

            if (_isMoved)
            {
                _gridStartPos += _moveStep;
                if (_gridStartPos >= _gridSize.Width)
                {
                    _gridStartPos -= _gridSize.Width;
                }
            }

            this.Draw();
        }

        private List<long> _listRawData1 = new List<long>();
        private List<long> _listRawData2 = new List<long>();

        /// <summary>
        /// Adds two numbers to the lists of data. The numbers are raw values, and 
        /// the graph will be automatically scaled to fit the largest value.
        /// </summary>
        /// <param name="v1">A long.</param>
        /// <param name="v2">A long.</param>
        public void Add(long v1, long v2)
        {
            _listRawData1.Add(v1);
            _listRawData2.Add(v2);

            if (_listRawData1.Count > this.ClientRectangle.Width / this.MoveStep + 1)
                _listRawData1.RemoveRange(0, _listRawData1.Count - 1 - this.ClientRectangle.Width / this.MoveStep);
            if (_listRawData2.Count > this.ClientRectangle.Width / this.MoveStep + 1)
                _listRawData2.RemoveRange(0, _listRawData2.Count - 1 - this.ClientRectangle.Width / this.MoveStep);

            // find the largest value
            long max = 0;

            foreach (long l in _listRawData1)
                if (l > max)
                    max = l;
            foreach (long l in _listRawData2)
                if (l > max)
                    max = l;

            // redo the other list
            _listData1.Clear();
            _listData2.Clear();

            foreach (long l in _listRawData1)
            {
                if (max != 0)
                    _listData1.Add((float)l / max);
                else
                    _listData1.Add(0);
            }

            foreach (long l in _listRawData2)
            {
                if (max != 0)
                    _listData2.Add((float)l / max);
                else
                    _listData2.Add(0);
            }

            if (_isMoved)
            {
                _gridStartPos += _moveStep;
                if (_gridStartPos >= _gridSize.Width)
                {
                    _gridStartPos -= _gridSize.Width;
                }
            }
            
            this.Draw();
        }

        #region Text

        private Point _textPosition, _boxPosition;
        private Size _textSize, _boxSize;

        private Padding _textMargin = new Padding(3, 3, 3, 3);
        public Padding TextMargin
        {
            get { return _textMargin; }
            set { _textMargin = value; }
        }

        private Padding _textPadding = new Padding(3, 3, 3, 3);
        public Padding TextPadding
        {
            get { return _textPadding; }
            set { _textPadding = value; }
        }

        private ContentAlignment _textAlign = ContentAlignment.TopLeft;
        public ContentAlignment TextPosition
        {
            get { return _textAlign; }
            set { _textAlign = value; }
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

                _textSize = TextRenderer.MeasureText(this.Text, this.Font);
                _boxSize = new Size(
                    _textSize.Width + _textPadding.Left + _textPadding.Right,
                    _textSize.Height + _textPadding.Top + _textPadding.Bottom);

                // work out Y
                switch (_textAlign)
                {
                    case ContentAlignment.BottomCenter:
                    case ContentAlignment.BottomLeft:
                    case ContentAlignment.BottomRight:
                        _boxPosition.Y = this.Size.Height - _boxSize.Height - _textMargin.Bottom;
                        break;

                    case ContentAlignment.MiddleCenter:
                    case ContentAlignment.MiddleLeft:
                    case ContentAlignment.MiddleRight:
                        _boxPosition.Y = (this.Size.Height - _boxSize.Height) / 2;
                        break;

                    case ContentAlignment.TopCenter:
                    case ContentAlignment.TopLeft:
                    case ContentAlignment.TopRight:
                        _boxPosition.Y = _textMargin.Top;
                        break;
                }

                // work out X
                switch (_textAlign)
                {
                    case ContentAlignment.BottomLeft:
                    case ContentAlignment.MiddleLeft:
                    case ContentAlignment.TopLeft:
                        _boxPosition.X = _textMargin.Left;
                        break;

                    case ContentAlignment.BottomCenter:
                    case ContentAlignment.MiddleCenter:
                    case ContentAlignment.TopCenter:
                        _boxPosition.X = (this.Size.Width - _boxSize.Width) / 2;
                        break;

                    case ContentAlignment.BottomRight:
                    case ContentAlignment.MiddleRight:
                    case ContentAlignment.TopRight:
                        _boxPosition.X = this.Size.Width - _boxSize.Width - _textMargin.Right;
                        break;
                }

                _textPosition = new Point(
                    _boxPosition.X + _textPadding.Left,
                    _boxPosition.Y + _textPadding.Top);

                this.Draw();
            }
        }

        private Color _textBoxColor = Color.FromArgb(127, Color.Black);
        public Color TextBoxColor
        {
            get { return _textBoxColor; }
            set { _textBoxColor = value; }
        }

        private Color _textColor = Color.FromArgb(0, 255, 0);
        public Color TextColor
        {
            get { return _textColor; }
            set { _textColor = value; }
        }

        #endregion

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

        private Color _gridColor = Color.Green;
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
            if (_managedBackBuffer != NO_MANAGED_BACK_BUFFER)
                _managedBackBuffer.Dispose();

            if (_graphicManager == null)
                _graphicManager = BufferedGraphicsManager.Current;

            _graphicManager.MaximumBuffer =
                new Size(this.Width + 1, this.Height + 1);
            _managedBackBuffer =
                _graphicManager.Allocate(this.CreateGraphics(), this.ClientRectangle);
            this.Draw();
        }
    }
}
