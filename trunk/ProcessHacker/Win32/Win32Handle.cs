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
using System.Runtime.InteropServices;

namespace ProcessHacker
{
    public partial class Win32
    {
        /// <summary>
        /// Represents a generic Windows handle.
        /// </summary>
        public class Win32Handle : IDisposable
        {
            private bool _owned = true;
            private bool _closed = false;
            private int _handle;

            public static implicit operator int(Win32Handle handle)
            {
                return handle.Handle;
            }

            /// <summary>
            /// Creates a new, invalid handle. You must set the handle using the Handle property.
            /// </summary>
            protected Win32Handle()
            { }

            /// <summary>
            /// Creates a new handle using the specified value. The handle will be closed when 
            /// this object is disposed or garbage-collected.
            /// </summary>
            /// <param name="handle">The handle value.</param>
            public Win32Handle(int handle)
            {
                _handle = handle;
            }

            /// <summary>
            /// Creates a new handle using the specified value. If owned is set to false, the 
            /// handle will not be closed automatically.
            /// </summary>
            /// <param name="handle">The handle value.</param>
            /// <param name="owned">Specifies whether the handle will be closed automatically.</param>
            public Win32Handle(int handle, bool owned)
            {
                _handle = handle;
                _owned = owned;
            }

            /// <summary>
            /// Gets whether this handle is closed.
            /// </summary>
            public bool Closed
            {
                get { return _closed; }
            }

            /// <summary>
            /// Gets whether the handle will be automatically closed.
            /// </summary>
            public bool Owned
            {
                get { return _owned; }
            }

            /// <summary>
            /// Gets the handle value.
            /// </summary>
            public int Handle
            {
                get { return _handle; }
                protected set { _handle = value; }
            }

            /// <summary>
            /// Closes the handle. This method must not be called directly; instead, 
            /// override this method in a derived class if your handle must be closed 
            /// with a method other than CloseHandle.
            /// </summary>
            protected virtual void Close()
            {
                CloseHandle(_handle);
            }

            ~Win32Handle()
            {
                this.Dispose();
            }

            /// <summary>
            /// Closes the handle.
            /// </summary>
            public void Dispose()
            {
                if (!_closed && _owned)
                {
                    _closed = true;
                    Close();
                }
            }
        }
    }
}
