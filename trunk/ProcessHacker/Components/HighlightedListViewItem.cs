/*
 * Process Hacker
 * 
 * Copyright (C) 2008 wj32
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
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ProcessHacker
{
    public enum ListViewItemState
    {
        New, Removed
    }

    /// <summary>
    /// A list view item that supports temporary highlighting.
    /// </summary>
    public class HighlightedListViewItem : ListViewItem
    {
        private static Dictionary<ListViewItemState, Color> _colors = new Dictionary<ListViewItemState, Color>();
        private static int _highlightingDuration = 1000;

        static HighlightedListViewItem()
        {
            _colors.Add(ListViewItemState.New, Color.FromArgb(0xe0f0e0));
            _colors.Add(ListViewItemState.Removed, Color.FromArgb(0xf0e0e0));
        }

        public static Dictionary<ListViewItemState, Color> Colors
        {
            get { return _colors; }
        }

        public static int HighlightingDuration
        {
            get { return _highlightingDuration; }
            set { _highlightingDuration = value; }
        }

        private Color _normalColor = SystemColors.Window;

        public HighlightedListViewItem() : this("") { }

        public HighlightedListViewItem(string text) : base(text)
        {
            this.BackColor = _colors[ListViewItemState.New];
            this.PerformDelayed(new MethodInvoker(delegate { this.BackColor = _normalColor; }));
        }

        public override void Remove()
        {
            this.BackColor = _colors[ListViewItemState.Removed];
            this.PerformDelayed(new MethodInvoker(delegate { base.Remove(); }));  
        }

        public Color NormalColor
        {
            get { return _normalColor; }
            set { _normalColor = value; }
        }

        public void SetTemporaryState(ListViewItemState state)
        {
            this.BackColor = _colors[state];
            this.PerformDelayed(new MethodInvoker(delegate { this.BackColor = _normalColor; }));
        }

        private void PerformDelayed(MethodInvoker method)
        {
            Timer t = new Timer();

            t.Tick += new EventHandler(delegate(object o, EventArgs e)
            {
                method();
            });

            t.Interval = _highlightingDuration;
            t.Start();
        }
    }
}
