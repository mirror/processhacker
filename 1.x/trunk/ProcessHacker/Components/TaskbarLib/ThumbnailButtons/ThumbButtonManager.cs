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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TaskbarLib.Interop;

namespace TaskbarLib
{
    /// <summary>
    /// Manages a set of taskbar thumbnail buttons in an application.
    /// </summary>
    public sealed class ThumbButtonManager : IDisposable
    {
        private sealed class MessageFilter : IMessageFilter
        {
            private ThumbButtonManager _manager;

            public MessageFilter(ThumbButtonManager manager)
            {
                _manager = manager;
            }

            public bool PreFilterMessage(ref Message m)
            {
                if (m.Msg == (int)Windows7Taskbar.TaskbarButtonCreatedMessage)
                {
                    _manager.OnTaskbarButtonCreated();
                    return true;
                }
                else if (m.Msg == (int)ProcessHacker.Native.Api.WindowMessage.Command)
                {
                    _manager.OnCommand(m.WParam);
                }

                return false;
            }
        }

        public event EventHandler TaskbarButtonCreated;

        private Form _form;
        private MessageFilter _filter;
        private bool _disposed;

        /// <summary>
        /// Initializes a new manager on the specified form.
        /// </summary>
        /// <param name="form">The form.</param>
        public ThumbButtonManager(Form form)
        {
            _form = form;

            Application.AddMessageFilter(_filter = new MessageFilter(this));
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                Application.RemoveMessageFilter(_filter);
                _disposed = true;
            }
        }

        /// <summary>
        /// Creates a new taskbar thumbnail button.
        /// </summary>
        /// <param name="id">The button's id.</param>
        /// <param name="icon">The button's icon.</param>
        /// <param name="tooltip">The button's tooltip.</param>
        /// <returns>An object of type <see cref="ThumbButton"/>
        /// representing the newly created thumbnail button.</returns>
        public ThumbButton CreateThumbButton(int id, Icon icon, string tooltip)
        {
            return new ThumbButton(this, id, icon, tooltip);
        }

        /// <summary>
        /// Adds the specified taskbar thumbnail buttons to the application's
        /// thumbnail toolbar.
        /// </summary>
        /// <remarks>
        /// Thumbnail buttons can only be added once - after being added,
        /// they cannot be removed or deleted.  However, they can be shown,
        /// hidden, enabled and disabled.
        /// </remarks>
        /// <param name="buttons">The buttons to add.</param>
        public void AddThumbButtons(params ThumbButton[] buttons)
        {
            Array.ForEach(buttons, b => _thumbButtons.Add(b.Id, b));

            RefreshThumbButtons();
        }

        /// <summary>
        /// Gets a specific thumbnail button by its id.
        /// </summary>
        /// <param name="id">The thumbnail button's id.</param>
        /// <returns>An object of type <see cref="ThumbButton"/>
        /// with the specified id.</returns>
        public ThumbButton this[int id]
        {
            get
            {
                return _thumbButtons[id];
            }
        }

        internal void OnCommand(IntPtr wParam)
        {
            if (((wParam.ToInt32() >> 16) & 0xffff) == SafeNativeMethods.THBN_CLICKED)
            {
                _thumbButtons[wParam.ToInt32() & 0xffff].OnClick();
            }
        }

        internal void OnTaskbarButtonCreated()
        {
            if (this.TaskbarButtonCreated != null)
                this.TaskbarButtonCreated(this, new EventArgs());

            _buttonsLoaded = false;
            this.RefreshThumbButtons();
        }

        #region Implementation

        private bool _buttonsLoaded;
        internal void RefreshThumbButtons()
        {
            THUMBBUTTON[] win32Buttons = (from thumbButton in _thumbButtons.Values select thumbButton.Win32ThumbButton).ToArray();

            if (_buttonsLoaded)
            {
                Windows7Taskbar.TaskbarList.ThumbBarUpdateButtons(_form.Handle, win32Buttons.Length, win32Buttons);
            }
            else //First time
            {
                Windows7Taskbar.TaskbarList.ThumbBarAddButtons(_form.Handle, win32Buttons.Length, win32Buttons);
                _buttonsLoaded = true;
            }
        }

        private Dictionary<int, ThumbButton> _thumbButtons = new Dictionary<int, ThumbButton>();

        #endregion
    }

}