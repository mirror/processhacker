/*
 * Process Hacker - 
 *   semaphore
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
    /// Represents a semaphore which can be used to control access to a shared resource.
    /// </summary>
    public sealed class Semaphore : NativeObject<SemaphoreHandle>
    {
        /// <summary>
        /// Creates a binary semaphore.
        /// </summary>
        public Semaphore()
            : this(null)
        { }

        /// <summary>
        /// Creates a binary semaphore.
        /// </summary>
        /// <param name="initialCount">The initial count of the semaphore.</param>
        /// <param name="maximumCount">The maximum count of the semaphore.</param>
        public Semaphore(int initialCount, int maximumCount)
            : this(null, initialCount, maximumCount)
        { }

        /// <summary>
        /// Creates or opens a semaphore.
        /// </summary>
        /// <param name="name">
        /// The name of the new semaphore, or the name of an existing semaphore.
        /// </param>
        public Semaphore(string name)
            : this(name, 1, 1)
        { }

        /// <summary>
        /// Creates a semaphore.
        /// </summary>
        /// <param name="name">The name of the new semaphore.</param>
        /// <param name="initialCount">The initial count of the semaphore.</param>
        /// <param name="maximumCount">The maximum count of the semaphore.</param>
        public Semaphore(string name, int initialCount, int maximumCount)
        {
            this.Handle = SemaphoreHandle.Create(
                SemaphoreAccess.All,
                name,
                ObjectFlags.OpenIf,
                null,
                initialCount,
                maximumCount
                );
        }

        /// <summary>
        /// Gets the current count of the semaphore.
        /// </summary>
        public int Count
        {
            get { return this.Handle.GetBasicInformation().CurrentCount; }
        }

        /// <summary>
        /// Gets the maximum count of the semaphore.
        /// </summary>
        public int MaximumCount
        {
            get { return this.Handle.GetBasicInformation().MaximumCount; }
        }

        /// <summary>
        /// Releases the semaphore, incrementing the count.
        /// </summary>
        public void Release()
        {
            this.Handle.Release();
        }

        /// <summary>
        /// Releases the semaphore, incrementing the count by the 
        /// specified amount.
        /// </summary>
        /// <param name="count">The amount to increment the count by.</param>
        public void Release(int count)
        {
            this.Handle.Release(count);
        }
    }
}
