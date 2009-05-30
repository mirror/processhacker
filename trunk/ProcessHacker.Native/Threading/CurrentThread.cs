/*
 * Process Hacker - 
 *   thread functions
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

namespace ProcessHacker.Native.Threading
{
    /// <summary>
    /// Provides methods for manipulating the current thread.
    /// </summary>
    public static class CurrentThread
    {
        /// <summary>
        /// Switches to another thread.
        /// </summary>
        public static void Sleep()
        {
            Yield();
        }

        /// <summary>
        /// Suspends execution of the current thread.
        /// </summary>
        /// <param name="interval">The interval to sleep, in milliseconds.</param>
        public static void Sleep(int interval)
        {
            ThreadHandle.Sleep(interval * Win32.TimeMsTo100Ns, true);
        }

        /// <summary>
        /// Suspends execution of the current thread.
        /// </summary>
        /// <param name="time">The time at which wake up.</param>
        public static void Sleep(DateTime time)
        {
            ThreadHandle.Sleep(time.ToFileTime(), false);
        }

        /// <summary>
        /// Switches to another thread.
        /// </summary>
        public static void Yield()
        {
            ThreadHandle.Yield();
        }
    }
}
