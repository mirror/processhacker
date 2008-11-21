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

        static HighlightedListViewItem()
        {
            _colors.Add(ListViewItemState.New, Color.FromArgb(0xe0f0e0));
            _colors.Add(ListViewItemState.Removed, Color.FromArgb(0xf0e0e0));
        }

        public static Dictionary<ListViewItemState, Color> Colors
        {
            get { return _colors; }
        }

        private Color _normalColor = SystemColors.Window;
        private int _highlightingDuration = 1000;

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
