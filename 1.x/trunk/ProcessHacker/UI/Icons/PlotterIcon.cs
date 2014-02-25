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
using System.Windows.Forms; // DO NOT REMOVE, needed in Debug mode
using ProcessHacker.Common;
using ProcessHacker.Components;

namespace ProcessHacker
{
    public abstract class PlotterIcon : UsageIcon
    {
        private CircularBuffer<float> _dataHistory1;
        private CircularBuffer<float> _dataHistory2;
        private CircularBuffer<long> _longDataHistory1;
        private CircularBuffer<long> _longDataHistory2;
        private Plotter _plotter;

        public PlotterIcon()
        {
            _dataHistory1 = new CircularBuffer<float>(20);
            _dataHistory2 = new CircularBuffer<float>(20);
            _longDataHistory1 = new CircularBuffer<long>(20);
            _longDataHistory2 = new CircularBuffer<long>(20);

            _plotter = new Plotter()
            {
                Size = this.Size,
                ShowGrid = false,
                BackColor = Color.Black,
                MoveStep = 2,
                Data1 = _dataHistory1,
                Data2 = _dataHistory2,
                LongData1 = _longDataHistory1,
                LongData2 = _longDataHistory2
            };
        }

        public override void Dispose()
        {
            _plotter.Dispose();
            base.Dispose();
        }

        protected void Update(float v1, float v2)
        {
            _dataHistory1.Add(v1);
            _dataHistory2.Add(v2);
        }

        protected void Update(long v1, long v2)
        {
            _longDataHistory1.Add(v1);
            _longDataHistory2.Add(v2);
        }

        public void Redraw()
        {
            Icon newIcon;
            Icon oldIcon = this.Icon;

            using (Bitmap bm = new Bitmap(this.Size.Width, this.Size.Height))
            {
                // Update the plotter size if our size has changed.
                if (_plotter.Size != this.Size)
                    _plotter.Size = this.Size;

                using (Graphics g = Graphics.FromImage(bm))
                    _plotter.Draw(g);

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
