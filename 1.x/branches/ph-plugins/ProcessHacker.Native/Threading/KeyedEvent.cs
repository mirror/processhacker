/*
 * Process Hacker - 
 *   keyed event
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
using System.Collections.Generic;
using System.Text;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;

namespace ProcessHacker.Native.Threading
{
    /// <summary>
    /// Represents a keyed event.
    /// </summary>
    public sealed class KeyedEvent : NativeObject<KeyedEventHandle>
    {
        /// <summary>
        /// Creates a new keyed event.
        /// </summary>
        public KeyedEvent()
            : this(null)
        { }

        /// <summary>
        /// Creates or opens a keyed event.
        /// </summary>
        /// <param name="name">
        /// The name of the new keyed event, or the name of an 
        /// existing keyed event.
        /// </param>
        public KeyedEvent(string name)
        {
            this.Handle = KeyedEventHandle.Create(
                KeyedEventAccess.All,
                name,
                ObjectFlags.OpenIf,
                null
                );
        }

        /// <summary>
        /// Releases the specified key. If no other thread is waiting 
        /// on the key, the function blocks until a thread does.
        /// </summary>
        /// <param name="key">The key, which must be divisible by 2.</param>
        public void ReleaseKey(int key)
        {
            this.Handle.ReleaseKey(new IntPtr(key), false, long.MinValue, false);
        }

        /// <summary>
        /// Releases the specified key. If no other thread is waiting 
        /// on the key, the function blocks until a thread does.
        /// </summary>
        /// <param name="key">The key, which must be divisible by 2.</param>
        /// <param name="timeout">A timeout value, in milliseconds.</param>
        public void ReleaseKey(int key, int timeout)
        {
            this.Handle.ReleaseKey(new IntPtr(key), false, timeout * Win32.TimeMsTo100Ns, true);
        }

        /// <summary>
        /// Releases the specified key. If no other thread is waiting 
        /// on the key, the function blocks until a thread does.
        /// </summary>
        /// <param name="key">The key, which must be divisible by 2.</param>
        /// <param name="timeout">A time to wait until.</param>
        public void ReleaseKey(int key, DateTime timeout)
        {
            this.Handle.ReleaseKey(new IntPtr(key), false, timeout.ToFileTime(), false);
        }

        /// <summary>
        /// Waits for the specified key to be released.
        /// </summary>
        /// <param name="key">The key, which must be divisible by 2.</param>
        public void WaitKey(int key)
        {
            this.Handle.WaitKey(new IntPtr(key), false, long.MinValue, false);
        }

        /// <summary>
        /// Waits for the specified key to be released.
        /// </summary>
        /// <param name="key">The key, which must be divisible by 2.</param>
        /// <param name="timeout">A time to wait until.</param>
        public void WaitKey(int key, int timeout)
        {
            this.Handle.WaitKey(new IntPtr(key), false, timeout * Win32.TimeMsTo100Ns, true);
        }

        /// <summary>
        /// Waits for the specified key to be released.
        /// </summary>
        /// <param name="key">The key, which must be divisible by 2.</param>
        /// <param name="timeout">A time to wait until.</param>
        public void WaitKey(int key, DateTime timeout)
        {
            this.Handle.WaitKey(new IntPtr(key), false, timeout.ToFileTime(), false);
        }
    }
}
