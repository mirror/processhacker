/*
 * Process Hacker - 
 *   NotifyIcon wrapper
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
using System.Drawing;
using System.Windows.Forms;
using ProcessHacker.Native;
using ProcessHacker.Native.Api;

namespace ProcessHacker
{
    public class UsageIcon : IDisposable
    {
        private static UsageIcon _activeUsageIcon;

        public static UsageIcon ActiveUsageIcon
        {
            get { return _activeUsageIcon; }
            set
            {
                _activeUsageIcon = value;

                if (value == null)
                {
                    if (OSVersion.HasExtendedTaskbar)
                    {
                        TaskbarLib.Windows7Taskbar.SetTaskbarOverlayIcon(
                            null,
                            ""
                            );
                    }
                }
            }
        }

        public static Size GetSmallIconSize()
        {
            return new Size(
                Win32.GetSystemMetrics(49), // SM_CXSMICON
                Win32.GetSystemMetrics(50) // SM_CYSMICON
                );
        }

        public event MouseEventHandler MouseClick;
        public event MouseEventHandler MouseDoubleClick;

        private Control _parent;
        private Size _size;
        private NotifyIcon _notifyIcon;

        public UsageIcon()
        {
            _notifyIcon = new NotifyIcon();

            _notifyIcon.MouseClick += new MouseEventHandler(notifyIcon_MouseClick);
            _notifyIcon.MouseDoubleClick += new MouseEventHandler(notifyIcon_MouseDoubleClick);

            _size = GetSmallIconSize();
        }

        public virtual void Dispose()
        {
            _notifyIcon.Dispose();
        }

        private void notifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (this.MouseClick != null)
                this.MouseClick(sender, e);
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.MouseDoubleClick != null)
                this.MouseDoubleClick(sender, e);
        }

        public void ShowBalloonTip(int timeout, string tipTitle, string tipText, ToolTipIcon tipIcon)
        {
            _notifyIcon.ShowBalloonTip(timeout, tipTitle, tipText, tipIcon);
        }

        public Control Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        public ContextMenu ContextMenu
        {
            get { return _notifyIcon.ContextMenu; }
            set { _notifyIcon.ContextMenu = value; }
        }

        public Icon Icon
        {
            get { return _notifyIcon.Icon; }
            set
            {
                _notifyIcon.Icon = value;

                if (this == _activeUsageIcon)
                {
                    if (OSVersion.HasExtendedTaskbar)
                    {
                        TaskbarLib.Windows7Taskbar.SetTaskbarOverlayIcon(
                            value,
                            ""
                            );
                    }
                }
            }
        }

        public bool Visible
        {
            get { return _notifyIcon.Visible; }
            set { _notifyIcon.Visible = value; }
        }

        public Size Size
        {
            get { return _size; }
            set { _size = value; }
        }

        protected string Text
        {
            get { return _notifyIcon.Text; }
            set { _notifyIcon.Text = value; }
        }
    }
}
