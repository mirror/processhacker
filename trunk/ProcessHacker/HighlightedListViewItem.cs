/*
 * Process Hacker - 
 *   reusable ListViewItem highlighting
 * 
 * Copyright (C) 2008 wj32
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
using System.Text;
using System.Windows.Forms;

namespace ProcessHacker
{
    public enum ListViewItemState
    {
        Normal, New, Removed
    }

    /// <summary>
    /// A list view item that supports temporary highlighting.
    /// </summary>
    public class HighlightedListViewItem : ListViewItem
    {
        private static Dictionary<ListViewItemState, Color> _colors = new Dictionary<ListViewItemState, Color>();
        private ListViewItemState _state = ListViewItemState.Normal;
        private static int _highlightingDuration = 1000;
        private static bool _stateHighlighting = true;

        static HighlightedListViewItem()
        {
            _colors.Add(ListViewItemState.New, Color.FromArgb(0xe0f0e0));
            _colors.Add(ListViewItemState.Removed, Color.FromArgb(0xf0e0e0));
        }

        public static Dictionary<ListViewItemState, Color> Colors
        {
            get { return _colors; }
        }

        /// <summary>
        /// Gets or sets the duration, in milliseconds, of state highlighting.
        /// </summary>
        public static int HighlightingDuration
        {
            get { return _highlightingDuration; }
            set { _highlightingDuration = value; }
        }

        /// <summary>
        /// Gets or sets whether state highlighting is on.
        /// </summary>
        public static bool StateHighlighting
        {
            get { return _stateHighlighting; }
            set { _stateHighlighting = value; }
        }

        private Color _normalColor = SystemColors.Window;

        public HighlightedListViewItem() : this(true) { }

        public HighlightedListViewItem(bool highlight) : this("", highlight) { }

        public HighlightedListViewItem(string text) : this(text, true) { }

        public HighlightedListViewItem(string text, bool highlight) : base(text)
        {
            if (_stateHighlighting && highlight)
            {
                this.BackColor = _colors[ListViewItemState.New];
                _state = ListViewItemState.New;
                this.PerformDelayed(new MethodInvoker(delegate
                {
                    this.BackColor = _normalColor;
                    _state = ListViewItemState.Normal;
                }));
            }
            else
            {
                this.BackColor = _normalColor;
            }
        }

        public override void Remove()
        {
            if (_stateHighlighting)
            {
                this.BackColor = _colors[ListViewItemState.Removed];
                this.PerformDelayed(new MethodInvoker(RemoveThis));
            }
            else
            {
                base.Remove();
            }
        }

        private void RemoveThis()
        {
            base.Remove();
        }

        public Color NormalColor
        {
            get { return _normalColor; }
            set
            {
                _normalColor = value;

                if (_state == ListViewItemState.Normal)
                    this.BackColor = value;
            }
        }

        public void SetTemporaryState(ListViewItemState state)
        {
            this.BackColor = _colors[state];
            _state = state;
            this.PerformDelayed(new MethodInvoker(delegate
            {
                this.BackColor = _normalColor;
                _state = ListViewItemState.Normal;
            }));
        }

        private void PerformDelayed(MethodInvoker method)
        {
            Timer t = new Timer();
            EventHandler handler = null;
            
            handler = new EventHandler(delegate(object o, EventArgs e)
            {
                method();
                t.Tick -= handler;
                t.Dispose();
            });

            t.Tick += handler;
            t.Interval = _highlightingDuration;
            t.Start();
        }
    }
}
