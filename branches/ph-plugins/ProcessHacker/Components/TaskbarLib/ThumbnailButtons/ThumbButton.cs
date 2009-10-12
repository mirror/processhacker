/*
 * Process Hacker - 
 *   ProcessHacker Taskbar Extensions
 * 
 * Copyright (C) 2009 dmex
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
 * 
 */

using System;
using System.Drawing;
using TaskbarLib.Interop;

namespace TaskbarLib
{
    /// <summary>
    /// Represents a taskbar thumbnail button in the thumbnail toolbar.
    /// </summary>
    public sealed class ThumbButton
    {
        private ThumbButtonManager _manager;

        internal ThumbButton(ThumbButtonManager manager, int id, Icon icon, string tooltip)
        {
            _manager = manager;

            Id = id;
            Icon = icon;
            Tooltip = tooltip;
        }

        /// <summary>
        /// The event that occurs when the taskbar thumbnail button
        /// is clicked.
        /// </summary>
        public event EventHandler Click;

        /// <summary>
        /// Gets or sets thumbnail button's id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the thumbnail button's icon.
        /// </summary>
        public Icon Icon { get; set; }

        /// <summary>
        /// Gets or sets the thumbnail button's tooltip.
        /// </summary>
        public string Tooltip { get; set; }

        internal ThumbnailButtonFlags Flags { get; set; }

        internal THUMBBUTTON Win32ThumbButton
        {
            get
            {
                THUMBBUTTON win32ThumbButton = new THUMBBUTTON();
                win32ThumbButton.iId = Id;
                win32ThumbButton.szTip = Tooltip;
                win32ThumbButton.hIcon = Icon.Handle;
                win32ThumbButton.dwFlags = Flags | ThumbnailButtonFlags.DISMISSONCLICK;

                win32ThumbButton.dwMask = ThumbnailButtonMask.Flags;
                if (Tooltip != null)
                    win32ThumbButton.dwMask |= ThumbnailButtonMask.Tooltip;
                if (Icon != null)
                    win32ThumbButton.dwMask |= ThumbnailButtonMask.Icon;

                return win32ThumbButton;
            }
        }

        /// <summary>
        /// Gets or sets the thumbnail button's visibility.
        /// </summary>
        public bool Visible
        {
            get
            {
                return (this.Flags & ThumbnailButtonFlags.HIDDEN) == 0;
            }
            set
            {
                if (value)
                {
                    this.Flags &= ~(ThumbnailButtonFlags.HIDDEN);
                }
                else
                {
                    this.Flags |= ThumbnailButtonFlags.HIDDEN;
                }
                _manager.RefreshThumbButtons();
            }
        }

        public bool NoIconBackground
        {
            get
            {
                return (this.Flags & ThumbnailButtonFlags.NOBACKGROUND) == 0;
            }
            set
            {
                if (value)
                {
                    this.Flags &= ~(ThumbnailButtonFlags.NOBACKGROUND);
                }
                else
                {
                    this.Flags |= ThumbnailButtonFlags.NOBACKGROUND;
                }
                _manager.RefreshThumbButtons();
            }
        }

        /// <summary>
        /// Gets or sets the thumbnail button's enabled state.
        /// </summary>
        public bool Enabled
        {
            get
            {
                return (this.Flags & ThumbnailButtonFlags.DISABLED) == 0;
            }
            set
            {
                if (value)
                {
                    this.Flags &= ~(ThumbnailButtonFlags.DISABLED);
                }
                else
                {
                    this.Flags |= ThumbnailButtonFlags.DISABLED;
                }
                _manager.RefreshThumbButtons();
            }
        }

        internal void OnClick()
        {
            if (Click != null)
                Click(this, EventArgs.Empty);
        }
    }

}