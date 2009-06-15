/*
 * Process Hacker - 
 *   spinlock
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
using System.Threading;

namespace ProcessHacker.Common.Threading
{
    /// <summary>
    /// Represents a spinlock, a high-performance mutual exclusion lock.
    /// </summary>
    public class SpinLock
    {
        public struct SpinLockContext : IDisposable
        {
            private bool _disposed;
            private SpinLock _spinLock;

            internal SpinLockContext(SpinLock spinLock)
            {
                _spinLock = spinLock;
                _spinLock.Acquire();
                _disposed = false;
            }

            public void Dispose()
            {
                if (!_disposed)
                {
                    _spinLock.Release();
                    _disposed = true;
                }
            }
        }

        private int _value = 0;
        private bool _spin;
        private int _acquireCount = 0;
        private int _spinCount = 0;

        /// <summary>
        /// Creates a spinlock.
        /// </summary>
        public SpinLock()
        {
            // We don't want to spin on uniprocessor systems.
            if (Environment.ProcessorCount == 1)
                _spin = false;
            else
                _spin = true;
        }

        /// <summary>
        /// Acquires the spinlock.
        /// </summary>
        public void Acquire()
        {
            Thread.BeginCriticalRegion();

            Interlocked.Increment(ref _acquireCount);

            if (_spin)
            {
                while (Interlocked.CompareExchange(ref _value, 1, 0) == 1)
                    Thread.SpinWait((_spinCount++ % Thread.VolatileRead(ref _acquireCount)) + 1);
            }
            else
            {
                while (Interlocked.CompareExchange(ref _value, 1, 0) == 1)
                    Thread.Sleep(0);
            }

            Thread.EndCriticalRegion();
        }

        /// <summary>
        /// Acquires the spinlock using a context object.
        /// </summary>
        /// <returns>A disposable context object.</returns>
        public SpinLockContext AcquireContext()
        {
            return new SpinLockContext(this);
        }

        /// <summary>
        /// Releases the spinlock.
        /// </summary>
        public void Release()
        {
            Thread.BeginCriticalRegion();
            Interlocked.Exchange(ref _value, 0);
            Interlocked.Decrement(ref _acquireCount);
            Thread.EndCriticalRegion();
        }
    }
}
