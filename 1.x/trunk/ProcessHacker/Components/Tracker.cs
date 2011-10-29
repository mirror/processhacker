using System;
using ProcessHacker.Common;

namespace ProcessHacker
{
    using System.Windows.Forms;
    using System.ComponentModel;
    using System.Drawing;

    public class Tracker : Control
    {
        public Color DrawColor = Color.Empty;

        //SedondLine Maximum
        public int value2Max = 100;

        private int mover;

        public System.Collections.Generic.IList<int> values;
        public System.Collections.Generic.IList<long> values2;

        private int mValue;
        private int mMinimum;
        private int mMaximum = 100;
        private int mLower = 25;
        private int mUpper = 75;

        private int intDivision = 2;
        private int mGrid = 12;

        public bool UseSecondLine { get; set; }

        public Tracker()
        {
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.DoubleBuffer, true);

            this.Draw();
        }

        [Category("Behavior"), DefaultValue(12), Description("The grid of Tracker.")]
        public int Grid
        {
            get { return mGrid; }
            set
            {
                if (value > 0) 
                    mGrid = value;
            }
        }

        [Category("Behavior"), DefaultValue(0), Description("The current value of Tracker, in the range specified by the Minimum and Maximum properties.")]
        public int Value
        {
            get { return this.mValue; }
            set
            {
                if (value > this.mMaximum)
                {
                    this.mValue = this.mMaximum;
                }
                else if (value < this.mMinimum)
                {
                    this.mValue = this.mMinimum;
                }
                else
                {
                    this.mValue = value;
                }
            }
        }

        [Category("Behavior"), DefaultValue(0), Description("The lower bound of the range this Tracker is working with.")]
        public int Minimum
        {
            get { return mMinimum; }
            set
            {
                if (value > mLower)
                {
                    mMinimum = mLower;
                }
                else
                {
                    mMinimum = value;
                }
                this.Invalidate();
            }
        }

        [Category("Behavior"), DefaultValue(100), Description("The upper bound of the range this Tracker is working with.")]
        public int Maximum
        {
            get { return mMaximum; }
            set
            {
                if (value < mUpper)
                {
                    mMaximum = mUpper;
                }
                else
                {
                    mMaximum = value;
                }

                this.Invalidate();
            }
       }

        [Category("Behavior"), DefaultValue(75), Description("The upper value of the normal range.")]
        public int UpperRange
        {
            get { return mUpper; }
            set
            {
                if (value > mMaximum)
                {
                    mUpper = mMaximum;
                }
                else if (value < mLower)
                {
                    mUpper = mLower;
                }
                else
                {
                    mUpper = value;
                }
                this.Invalidate();
            }
        }

        [Category("Behavior"), DefaultValue(25), Description("The lower value of the normal range.")]
        public int LowerRange
        {
            get { return mLower; }
            set
            {
                if (value > mUpper)
                {
                    mLower = mUpper;
                }
                else if (value < mMinimum)
                {
                    mLower = mMinimum;
                }
                else
                {
                    mLower = value;
                }
                this.Invalidate();
            }
        }

        public void Draw()
        {
            if (this.mover >= this.mGrid - intDivision)
            {
                mover = 0;
            }
            else
            {
                mover += intDivision;
            }

            this.Refresh();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(Color.Black);
            //e.Graphics.SmoothingMode = Settings.Instance.PlotterSmoothingMode;

            int width = this.Width;
            int height = this.Height;

            // draw grid
            using (Pen p = new Pen(Color.DarkGreen))
            {
                int pos;

                // X Axis
                for (int i = 0; i <= height / this.mGrid; i++)
                {
                    pos = i * this.mGrid;

                    e.Graphics.DrawLine(p, 0, pos, width, pos);
                }
                // Y Axis
                for (int i = 0; i <= width + 1 / this.mGrid; i++)
                {
                    pos = i * this.mGrid - this.mover;

                    e.Graphics.DrawLine(p, pos, 0, pos, height);
                }
            }

            if (this.values != null)
            {
                for (int i = 1; i <= this.values.Count - 1; i++)
                {
                    int x = width - intDivision*(this.values.Count - i);
                    int y = height*(this.Maximum - this.values[i])/(this.Maximum - this.Minimum);

                    int xx = width - intDivision*(this.values.Count - i + 1);
                    int yy = (height*(this.Maximum - this.values[i - 1])/(this.Maximum - this.Minimum));

                    using (Pen p = new Pen(this.DrawColor))
                    {
                        p.Width = 2;

                        using (SolidBrush b = new SolidBrush(Color.FromArgb(100, p.Color)))
                        {
                            // Fill in the area below the line.
                            e.Graphics.FillPolygon(b, new[]
                            {
                                new Point(x, y),
                                new Point(x + intDivision, yy),
                                new Point(x + intDivision, height),
                                new Point(x, height)
                            });

                            //draw first line
                            e.Graphics.DrawLine(p, x, y, xx, yy);
                        }
                    }
                }
            }


            //todo: complete.
            if (this.UseSecondLine)
            {
                for (int i = 0; i <= this.values2.Count - 1; i++)
                {
                    int x = width - intDivision*(values2.Count - i);
                    int y = height*(value2Max - (int)values2[i])/(value2Max - this.Minimum);

                    int xx = width - intDivision*(values2.Count - i + 1);
                    int yy = (height*(value2Max - (int)values2[i - 1])/(value2Max - this.Minimum));

                    using (Pen p = new Pen(Color.Pink))
                    {
                        // Fill in the area below the line.
                        e.Graphics.FillPolygon(new SolidBrush(Color.FromArgb(100, p.Color)), new[]
                        {
                            new Point(x, y),
                            new Point(x + intDivision, yy),
                            new Point(x + intDivision, height),
                            new Point(x, height)
                        });

                        //draw first line
                        e.Graphics.DrawLine(p, x, y, xx, yy);
                    }
                }
            }

            // Draw the text, if any.
            if (!string.IsNullOrEmpty(this.Text))
            {
                using (SolidBrush b = new SolidBrush(this._textBoxColor))
                {
                    // Draw the background for the text.
                    e.Graphics.FillRectangle(b, new Rectangle(this._boxPosition, this._boxSize));
                }

                // Draw the text.
                TextRenderer.DrawText(e.Graphics, this.Text, this.Font, this._textPosition, _textColor);
            }

            //todo: Draw the Border?
            //ControlPaint.DrawBorder3D(e.Graphics, 0, 0, this.Width, this.Height, Border3DStyle.Sunken);
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
            get { return _text; }
            set
            {
                _text = value;

                _textSize = this.CreateGraphics().GetCachedSize(this.Text, this.Font);
                _boxSize = new Size(_textSize.Width + _textPadding.Left + _textPadding.Right, _textSize.Height + _textPadding.Top + _textPadding.Bottom);

                // work out Y
                switch (_textAlign)
                {
                    case ContentAlignment.BottomCenter:
                    case ContentAlignment.BottomLeft:
                    case ContentAlignment.BottomRight:
                        {
                            _boxPosition.Y = this.Size.Height - _boxSize.Height - _textMargin.Bottom;
                            break;
                        }
                    case ContentAlignment.MiddleCenter:
                    case ContentAlignment.MiddleLeft:
                    case ContentAlignment.MiddleRight:
                        {
                            _boxPosition.Y = (this.Size.Height - _boxSize.Height) / 2;
                            break;
                        }

                    case ContentAlignment.TopCenter:
                    case ContentAlignment.TopLeft:
                    case ContentAlignment.TopRight:
                        {
                            _boxPosition.Y = _textMargin.Top;
                            break;
                        }
                }

                // work out X
                switch (_textAlign)
                {
                    case ContentAlignment.BottomLeft:
                    case ContentAlignment.MiddleLeft:
                    case ContentAlignment.TopLeft:
                        {
                            _boxPosition.X = _textMargin.Left;
                            break;
                        }
                    case ContentAlignment.BottomCenter:
                    case ContentAlignment.MiddleCenter:
                    case ContentAlignment.TopCenter:
                        {
                            _boxPosition.X = (this.Size.Width - _boxSize.Width) / 2;
                            break;
                        }
                    case ContentAlignment.BottomRight:
                    case ContentAlignment.MiddleRight:
                    case ContentAlignment.TopRight:
                        {
                            _boxPosition.X = this.Size.Width - _boxSize.Width - _textMargin.Right;
                            break;
                        }
                }

                _textPosition = new Point(_boxPosition.X + _textPadding.Left, _boxPosition.Y + _textPadding.Top);
            }
        }


    }
}