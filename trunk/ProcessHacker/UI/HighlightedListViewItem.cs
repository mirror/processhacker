/*
 * Process Hacker - 
 *   reusable ListViewItem highlighting
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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ProcessHacker.UI
{
    public enum ListViewItemState
    {
        Normal, New, Removed
    }

    public class HighlightingContext : IDisposable
    {
        public static event MethodInvoker HighlightingDurationChanged;

        private static Dictionary<ListViewItemState, Color> _colors = new Dictionary<ListViewItemState, Color>();
        private static int _highlightingDuration = 1000;
        private static bool _stateHighlighting = true;

        static HighlightingContext()
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
            set
            {
                _highlightingDuration = value;

                if (HighlightingDurationChanged != null)
                    HighlightingDurationChanged();
            }
        }

        /// <summary>
        /// Gets or sets whether state highlighting is on.
        /// </summary>
        public static bool StateHighlighting
        {
            get { return _stateHighlighting; }
            set { _stateHighlighting = value; }
        }

        private ListView _list;
        private Queue<MethodInvoker> _preQueue = new Queue<MethodInvoker>();
        private Queue<MethodInvoker> _queue = new Queue<MethodInvoker>();

        public HighlightingContext(ListView list)
        {
            _list = list;
        }

        public void Tick()
        {
            if (!_list.IsHandleCreated)
                return;

            _list.BeginInvoke(new MethodInvoker(delegate
            {
                // Execute the pre-queue items.
                _list.BeginUpdate();

                while (_preQueue.Count > 0)
                    _preQueue.Dequeue().Invoke();

                _list.EndUpdate();

                // Execute the normal queue items.
                System.Threading.Timer t = null;

                t = new System.Threading.Timer(o =>
                {
                    if (_list.IsHandleCreated)
                    {
                        _list.BeginInvoke(new MethodInvoker(delegate
                        {
                            _list.BeginUpdate();

                            while (_queue.Count > 0)
                                _queue.Dequeue().Invoke();

                            _list.EndUpdate();
                        }));
                    }

                    t.Dispose();
                }, null, HighlightingContext.HighlightingDuration, System.Threading.Timeout.Infinite);
            }));
        }

        public void Enqueue(MethodInvoker method)
        {
            _queue.Enqueue(method);
        }

        public void EnqueuePre(MethodInvoker method)
        {
            _preQueue.Enqueue(method);
        }

        public void Dispose()
        {
            // Nothing
        }
    }

    /// <summary>
    /// A list view item that supports temporary highlighting.
    /// </summary>
    public class HighlightedListViewItem : ListViewItem
    {
        private HighlightingContext _context;
        private Color _normalColor = SystemColors.Window;
        private ListViewItemState _state = ListViewItemState.Normal;

        public HighlightedListViewItem(HighlightingContext context)
            : this(context, true)
        { }

        public HighlightedListViewItem(HighlightingContext context, bool highlight)
            : this(context, "", highlight)
        { }

        public HighlightedListViewItem(HighlightingContext context, string text)
            : this(context, text, true)
        { }

        public HighlightedListViewItem(HighlightingContext context, string text, bool highlight)
            : base(text)
        {
            _context = context;

            if (HighlightingContext.StateHighlighting && highlight)
            {
                this.BackColor = HighlightingContext.Colors[ListViewItemState.New];
                _state = ListViewItemState.New;

                _context.Enqueue(delegate
                    {
                        this.BackColor = _normalColor;
                        _state = ListViewItemState.Normal;
                    });
            }
            else
            {
                this.BackColor = _normalColor;
            }
        }

        public override void Remove()
        {
            if (HighlightingContext.StateHighlighting)
            {
                _context.EnqueuePre(delegate
                    {
                        this.BackColor = HighlightingContext.Colors[ListViewItemState.Removed];

                        _context.Enqueue(delegate
                        {
                            this.BaseRemove();
                        });
                    });
            }
            else
            {
                base.Remove();
            }
        }

        private void BaseRemove()
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
            _context.EnqueuePre(delegate
            {
                this.BackColor = HighlightingContext.Colors[state];
                _state = state;

                _context.Enqueue(delegate
                {
                    this.BackColor = _normalColor;
                    _state = ListViewItemState.Normal;
                });
            });
        }
    }
}
