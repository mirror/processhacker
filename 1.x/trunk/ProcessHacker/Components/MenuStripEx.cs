using System.Windows.Forms;
using ProcessHacker.Common;

namespace System
{
    /// <summary>
    /// This class adds some missing functionality that should have been provided in System.Windows.Forms.MenuStrip.
    /// </summary>
    public class MenuStripEx : MenuStrip
    {
        private const uint WM_MOUSEACTIVATE = 0x21;
        private static readonly IntPtr MA_ACTIVATE = new IntPtr(1);
        private static readonly IntPtr MA_ACTIVATEANDEAT = new IntPtr(2);
        private const uint MA_NOACTIVATE = 3;
        private static readonly IntPtr MA_NOACTIVATEANDEAT = new IntPtr(4);

        private bool clickThrough = true;

        /// <summary>Gets or sets whether the ToolStripEx honors item clicks when its containing form does not have input focus.</summary>
        /// <remarks>Default value is true, which is the same behavior provided by the base ToolStrip class.</remarks>
        public bool ClickThrough
        {
            get { return this.clickThrough; }
            set { this.clickThrough = value; }
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (this.clickThrough && m.Msg == WM_MOUSEACTIVATE && m.Result == MA_ACTIVATEANDEAT)
            {
                m.Result = MA_ACTIVATE;
            }
        }
    }
}
