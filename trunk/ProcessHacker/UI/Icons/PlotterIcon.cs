/*
 * Process Hacker - 
 *   plotter icon
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

using System.Drawing;
using ProcessHacker.Common;
using ProcessHacker.Components;
using System.Windows.Forms;

namespace ProcessHacker
{
    public abstract class PlotterIcon : UsageIcon
    {
        private HistoryManager<bool, float> _floatHistory = new HistoryManager<bool, float>();
        private HistoryManager<bool, long> _longHistory = new HistoryManager<bool, long>();
        private Plotter _plotter;

        public PlotterIcon()
        {
            _floatHistory.Add(true);
            _floatHistory.Add(false);
            _longHistory.Add(true);
            _longHistory.Add(false);

            _plotter = new Plotter()
            {
                Size = this.Size,
                ShowGrid = false,
                BackColor = Color.Black,
                MoveStep = 2,
                Data1 = _floatHistory[true],
                Data2 = _floatHistory[false],
                LongData1 = _longHistory[true],
                LongData2 = _longHistory[false]
            };
        }

        public override void Dispose()
        {
            _plotter.Dispose();
            base.Dispose();
        }

        protected void Update(float v1, float v2)
        {
            _floatHistory.Update(true, v1);
            _floatHistory.Update(false, v2);
        }

        protected void Update(long v1, long v2)
        {
            _longHistory.Update(true, v1);
            _longHistory.Update(false, v2);
        }

        public void Redraw()
        {
            // If this is running in debug mode, we should do the usual 
            // safe invoking. Otherwise, leave it out...
#if DEBUG
            if (this.Parent.InvokeRequired)
            {
                this.Parent.BeginInvoke(new MethodInvoker(this.Redraw));
                return;
            }
#endif

            Icon newIcon;
            Icon oldIcon = this.Icon;

            using (Bitmap bm = new Bitmap(this.Size.Width, this.Size.Height))
            {
                // Update the plotter size if our size has changed.
                if (_plotter.Size != this.Size)
                    _plotter.Size = this.Size;

                _plotter.Draw();
                _plotter.DrawToBitmap(bm, new Rectangle(new Point(0, 0), _plotter.Size));

                newIcon = Icon.FromHandle(bm.GetHicon());
            }

            this.Icon = newIcon;
            ProcessHacker.Native.Api.Win32.DestroyIcon(oldIcon.Handle);
        }

        protected bool UseLongData
        {
            get { return _plotter.UseLongData; }
            set { _plotter.UseLongData = value; }
        }

        protected bool UseSecondLine
        {
            get { return _plotter.UseSecondLine; }
            set { _plotter.UseSecondLine = value; }
        }

        protected bool OverlaySecondLine
        {
            get { return _plotter.OverlaySecondLine; }
            set { _plotter.OverlaySecondLine = value; }
        }

        protected long MinMaxValue
        {
            get { return _plotter.MinMaxValue; }
            set { _plotter.MinMaxValue = value; }
        }

        protected Color BackColor
        {
            get { return _plotter.BackColor; }
            set { _plotter.BackColor = value; }
        }

        protected Color LineColor1
        {
            get { return _plotter.LineColor1; }
            set { _plotter.LineColor1 = value; }
        }

        protected Color LineColor2
        {
            get { return _plotter.LineColor2; }
            set { _plotter.LineColor2 = value; }
        }
    }
}
