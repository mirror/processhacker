/*
 * Process Hacker - 
 *   easy window finder
 * 
 * Copyright (C) 2009 wj32
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
using System.Windows.Forms;
using ProcessHacker.Native.Api;

namespace ProcessHacker.Components
{
    public delegate void TargetWindowFoundDelegate(int pid, int tid);

    public class TargetWindowButton : ToolStripButton
    {
        public event TargetWindowFoundDelegate TargetWindowFound;

        private Control _parent;
        private Control _dummy;
        private IntPtr _currentHWnd;
        private bool _targeting = false;

        public TargetWindowButton()
        {
            this.Image = Properties.Resources.application;
            this.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.Text = "Find Window";
            this.ToolTipText = "Find Window";

            _dummy = new Control();
            _dummy.MouseMove += dummy_MouseMove;
            _dummy.MouseUp += dummy_MouseUp;
        }

        private Form FindParentForm(Control c)
        {
            if (c == null)
                return null;
            if (c is Form)
                return c as Form;

            return this.FindParentForm(c.Parent);
        }

        private void DrawWindowRectangle(IntPtr hWnd)
        {
            Rect rect;

            Win32.GetWindowRect(hWnd, out rect);

            IntPtr windowDc = Win32.GetWindowDC(hWnd);

            if (windowDc != IntPtr.Zero)
            {
                // Pen width of system border width times 3.
                int penWidth = Win32.GetSystemMetrics(5) * 3;
                // Save the DC.
                int oldDc = Win32.SaveDC(windowDc);
                // Get an inversion effect.
                Win32.SetROP2(windowDc, GdiBlendMode.Not);

                // Create a pen.
                IntPtr pen = Win32.CreatePen(GdiPenStyle.InsideFrame, penWidth, IntPtr.Zero);
                Win32.SelectObject(windowDc, pen);
                // Get the null brush.
                IntPtr brush = Win32.GetStockObject(GdiStockObject.NullBrush);
                Win32.SelectObject(windowDc, brush);
                // Draw the rectangle.
                Win32.Rectangle(windowDc, 0, 0, rect.Right - rect.Left, rect.Bottom - rect.Top);

                // Delete the pen.
                Win32.DeleteObject(pen);
                // Restore and release the old DC.
                Win32.RestoreDC(windowDc, oldDc);
                Win32.ReleaseDC(hWnd, windowDc);
            }
        }

        private void RedrawWindow(IntPtr hWnd)
        {
            this.RedrawWindow(hWnd, true);
        }

        private void RedrawWindow(IntPtr hWnd, bool workaround)
        {
            if (!Win32.RedrawWindow(
               hWnd,
               IntPtr.Zero,
               IntPtr.Zero,
               RedrawWindowFlags.Invalidate | // redraws the window
               RedrawWindowFlags.Erase | // for those toolbar backgrounds and empty forms
               RedrawWindowFlags.UpdateNow |
               RedrawWindowFlags.AllChildren |
               RedrawWindowFlags.Frame // important, even more so without desktop composition
               ) && workaround)
            {
                // Since the rectangle is just an inversion we can redo it.
                DrawWindowRectangle(hWnd);
            }
        }

        protected override void OnParentChanged(ToolStrip oldParent, ToolStrip newParent)
        {
            _parent = newParent;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            // Direct all mouse events to the dummy control.
            Win32.SetCapture(_dummy.Handle);
            _targeting = true;
            this.FindParentForm(_parent).SendToBack();

            dummy_MouseMove(null, null);
        }

        protected override void OnClick(EventArgs e)
        {
            // Handles the case where the user simply clicks on the button.
            dummy_MouseUp(null, null);
        }

        void dummy_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_targeting)
                return;

            IntPtr oldHWnd = _currentHWnd;

            // Get the window at the mouse position.
            _currentHWnd = Win32.WindowFromPoint(Control.MousePosition);

            // Don't paint the window again.
            if (_currentHWnd == oldHWnd)
                return;

            // Get the old window to repaint its border (since we painted all over it).
            if (oldHWnd != IntPtr.Zero)
                this.RedrawWindow(oldHWnd);

            bool isPhWindow = false;
            int pid, tid;

            tid = Win32.GetWindowThreadProcessId(_currentHWnd, out pid);
            isPhWindow = pid == Program.CurrentProcessId;

            // Draw a rectangle over the current window.
            if (
                _currentHWnd != IntPtr.Zero &&
                !isPhWindow // don't paint on ourself
                )
                this.DrawWindowRectangle(_currentHWnd);
        }

        void dummy_MouseUp(object sender, MouseEventArgs e)
        {
            this.FindParentForm(_parent).BringToFront();
            _targeting = false;
            Win32.ReleaseCapture();

            if (_currentHWnd != IntPtr.Zero)
            {
                // Redraw the window we found.
                this.RedrawWindow(_currentHWnd, false);

                int pid, tid;

                tid = Win32.GetWindowThreadProcessId(_currentHWnd, out pid);

                if (this.TargetWindowFound != null)
                    this.TargetWindowFound(pid, tid);
            }
        }
    }
}
