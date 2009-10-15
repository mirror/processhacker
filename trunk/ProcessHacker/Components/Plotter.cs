/*
 * Process Hacker - 
 *   plotter control
 * 
 * Copyright (C) 2008-2009 wj32
 * Copyright (C) 2008 Dean
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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using ProcessHacker.Common;

namespace ProcessHacker.Components
{
    public partial class Plotter : UserControl
    {
        private static int _globalMoveStep = 3;

        public static int GlobalMoveStep
        {
            get { return _globalMoveStep; }
            set { _globalMoveStep = value; }
        }

        public delegate string GetToolTipDelegate(int item);

        private const BufferedGraphics NO_MANAGED_BACK_BUFFER = null;
        private BufferedGraphicsContext _graphicManager;
        private BufferedGraphics _managedBackBuffer;
        private bool _showToolTip;
        private Point _mouseLocation;
        private string _lastToolTip;

        public Plotter()
        {
            InitializeComponent();

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            _graphicManager = BufferedGraphicsManager.Current;
            _graphicManager.MaximumBuffer =
                new Size(this.Width + 1, this.Height + 1);
            _managedBackBuffer =
                _graphicManager.Allocate(this.CreateGraphics(), this.ClientRectangle);

            this.Draw();
        }

        public GetToolTipDelegate GetToolTip;

        private int _gridStartPos = 0;

        private void Plotter_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                this.Render(e.Graphics);
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
            }
        }

        public void Render(Graphics g)
        {
            _managedBackBuffer.Render(g);
        }

        public void Draw()
        {
            this.Draw(_managedBackBuffer.Graphics);

            if (_showToolTip)
                this.ShowToolTip();

            this.Refresh();
        }

        public void Draw(Graphics g)
        {
            int tWidth = this.Width;
            int tHeight = this.Height;
            int moveStep = this.EffectiveMoveStep;

            g.SmoothingMode = Settings.Instance.PlotterAntialias ? 
                SmoothingMode.AntiAlias : SmoothingMode.Default;

            g.FillRectangle(new SolidBrush(this.BackColor), 0, 0, tWidth, tHeight);

            // Draw the grid (if enabled).

            if (_showGrid)
            {
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
            }

            // Validate and if necessary, fix the data.

            if (_useLongData && (_longData1 == null || (this.UseSecondLine && _longData2 == null)))
                return;

            if (_useLongData)
                this.FixLongData();

            if (_data1 == null || (this.UseSecondLine && _data2 == null))
                return;

            // Draw the lines.

            int px = tWidth - moveStep;
            int start = 0;
            Pen lGrid1 = new Pen(_lineColor1);
            Pen lGrid2 = new Pen(_lineColor2);

            while (start < _data1.Count - 1)
            {
                float f = _data1[start + 1];
                float fPre = _data1[start];

                int h = (int)(tHeight - (tHeight * f));
                int hPre = (int)(tHeight - (tHeight * fPre));

                // Fill in the area below the line.

                g.FillPolygon(new SolidBrush(Color.FromArgb(100, _lineColor1)),
                    new Point[] { new Point(px, h), new Point(px + moveStep, hPre), 
                        new Point(px + moveStep, tHeight), new Point(px, tHeight) });
                g.DrawLine(lGrid1, px, h, px + moveStep, hPre);

                if (this.UseSecondLine)
                {
                    f = _data2[start + 1];
                    fPre = _data2[start];

                    if (!this.OverlaySecondLine)
                    {
                        f += _data1[start + 1];
                        fPre += _data1[start];

                        if (f > 1.0f)
                            f = 1.0f;
                        if (fPre > 1.0f)
                            fPre = 1.0f;
                    }

                    h = (int)(tHeight - (tHeight * f));
                    hPre = (int)(tHeight - (tHeight * fPre));

                    // Draw the second line.

                    if (this.OverlaySecondLine)
                    {
                        g.FillPolygon(new SolidBrush(Color.FromArgb(100, _lineColor2)),
                            new Point[] { new Point(px, h), new Point(px + moveStep, hPre), 
                            new Point(px + moveStep, tHeight), new Point(px, tHeight) });
                        g.DrawLine(lGrid2, px, h, px + moveStep, hPre);
                    }
                    else
                    {
                        g.FillPolygon(new SolidBrush(Color.FromArgb(100, _lineColor2)),
                            new Point[] { new Point(px, h), new Point(px + moveStep, hPre), 
                            new Point(px + moveStep, tHeight - (int)(tHeight * _data1[start])),
                            new Point(px, tHeight - (int)(tHeight * _data1[start + 1])) });
                        g.DrawLine(lGrid2, px, h, px + moveStep, hPre);
                    }
                }

                if (px < 0)
                {
                    break;
                }

                px -= moveStep;
                start++;
            }

            // Draw the text, if any.
            if (!string.IsNullOrEmpty(_text))
            {
                // Draw the background for the text.
                g.FillRectangle(new SolidBrush(_textBoxColor),
                    new Rectangle(_boxPosition, _boxSize));

                // Draw the text.
                TextRenderer.DrawText(g, _text, this.Font, _textPosition, _textColor);
            }
        }

        private void ShowToolTip()
        {
            this.ShowToolTip(false);
        }

        private void ShowToolTip(bool force)
        {
            if (this.GetToolTip != null)
            {
                int itemIndex = (this.Width - _mouseLocation.X) / this.EffectiveMoveStep;

                if (itemIndex < this.Data1.Count)
                {
                    try
                    {
                        string currentToolTip = this.GetToolTip(itemIndex);

                        if (currentToolTip != _lastToolTip || force)
                            toolTip.Show(currentToolTip, this,
                                new Point(_mouseLocation.X + 10, _mouseLocation.Y + 10),
                                int.MaxValue);

                        _lastToolTip = currentToolTip;
                    }
                    catch (Exception ex)
                    {
                        Logging.Log(ex);
                    }
                }
                else
                {
                    toolTip.Hide(this);
                }
            }
        }

        public void MoveGrid()
        {
            _gridStartPos += this.EffectiveMoveStep;

            if (_gridStartPos >= _gridSize.Width)
            {
                _gridStartPos -= _gridSize.Width;
            }
        }

        public void FixLongData()
        {
            // find the largest value
            long max = 0;
            // restrict scaling to the currently visible data points
            int maxIndex = this.Width / this.EffectiveMoveStep;

            for (int i = 0; i < _longData1.Count && i <= maxIndex; i++)
                if (_longData1[i] > max)
                    max = _longData1[i];
            for (int i = 0; i < _longData2.Count && i <= maxIndex; i++)
                if (_longData2[i] > max)
                    max = _longData2[i];

            if (max < _minMaxValue)
                max = _minMaxValue;

            // redo the float list
            _data1 = new List<float>();
            _data2 = new List<float>();

            for (int i = 0; i < _longData1.Count && i <= maxIndex; i++)
            {
                if (max != 0)
                    _data1.Add((float)_longData1[i] / max);
                else
                    _data1.Add(0);
            }

            for (int i = 0; i < _longData2.Count && i <= maxIndex; i++)
            {
                if (max != 0)
                    _data2.Add((float)_longData2[i] / max);
                else
                    _data2.Add(0);
            }
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

        private string _text;
        public override string Text
        {
            get
            {
                return _text;
            }
            set
            {
                base.Text = _text = value;

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
        public bool OverlaySecondLine { get; set; }

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

        private bool _showGrid = true;
        public bool ShowGrid
        {
            get { return _showGrid; }
            set { _showGrid = value; }
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

        private int _moveStep = -1;
        public int MoveStep
        {
            get { return _moveStep; }
            set { _moveStep = value; }
        }

        public int EffectiveMoveStep
        {
            get { return _moveStep == -1 ? GlobalMoveStep : _moveStep; }
        }

        private IList<float> _data1;
        public IList<float> Data1
        {
            get { return _data1; }
            set { _data1 = value; }
        }

        private IList<float> _data2;
        public IList<float> Data2
        {
            get { return _data2; }
            set { _data2 = value; }
        }

        private bool _useLongData;
        public bool UseLongData
        {
            get { return _useLongData; }
            set { _useLongData = value; }
        }

        private IList<long> _longData1;
        public IList<long> LongData1
        {
            get { return _longData1; }
            set { _longData1 = value; }
        }

        private IList<long> _longData2;
        public IList<long> LongData2
        {
            get { return _longData2; }
            set { _longData2 = value; }
        }

        private long _minMaxValue = 0;
        /// <summary>
        /// The minimum scaling value to be used for long data.
        /// </summary>
        public long MinMaxValue
        {
            get { return _minMaxValue; }
            set { _minMaxValue = value; }
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

        private void Plotter_MouseEnter(object sender, EventArgs e)
        {
            _showToolTip = true;
        }

        private void Plotter_MouseLeave(object sender, EventArgs e)
        {
            _showToolTip = false;
            toolTip.Hide(this);
        }

        private void Plotter_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Location != _mouseLocation)
            {
                _mouseLocation = e.Location;
                this.ShowToolTip(true);
            }
            else
            {
                _mouseLocation = e.Location;
                this.ShowToolTip();
            }
        }
    }
}
