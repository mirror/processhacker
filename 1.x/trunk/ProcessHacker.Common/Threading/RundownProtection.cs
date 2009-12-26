/*
 * Process Hacker - 
 *   rundown protection
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
    /// Provides methods for managing object/resource destruction.
    /// </summary>
    public struct RundownProtection
    {
        private const int RundownActive = 0x1;
        private const int RundownCountShift = 1;
        private const int RundownCountIncrement = 0x2;

        private int _value;
        private FastEvent _wakeEvent;

        /// <summary>
        /// Initializes a rundown protection structure.
        /// </summary>
        /// <param name="value">The initial usage count.</param>
        public RundownProtection(int value)
        {
            _value = RundownCountIncrement * value;
            _wakeEvent = new FastEvent(false);
        }

        /// <summary>
        /// Gets the current usage count.
        /// </summary>
        public int Count
        {
            get { return _value >> RundownCountShift; }
        }

        /// <summary>
        /// Gets whether the rundown has been initiated.
        /// </summary>
        public bool Rundown
        {
            get { return (_value & RundownActive) != 0; }
        }

        /// <summary>
        /// Attempts to acquire rundown protection.
        /// </summary>
        /// <returns>Whether rundown protection was acquired.</returns>
        public bool Acquire()
        {
            int value;

            while (true)
            {
                value = _value;

                // Has the rundown already started?
                if ((value & RundownActive) != 0)
                    return false;

                // Attempt to increment the usage count.
                if (Interlocked.CompareExchange(
                    ref _value,
                    value + RundownCountIncrement,
                    value
                    ) == value)
                    return true;
            }
        }

        /// <summary>
        /// Attempts to acquire rundown protection.
        /// </summary>
        /// <param name="count">The usage count to add.</param>
        /// <returns>Whether rundown protection was acquired.</returns>
        public bool Acquire(int count)
        {
            int value;

            while (true)
            {
                value = _value;

                // Has the rundown already started?
                if ((value & RundownActive) != 0)
                    return false;

                // Attempt to increase the usage count.
                if (Interlocked.CompareExchange(
                    ref _value,
                    value + RundownCountIncrement * count,
                    value
                    ) == value)
                    return true;
            }
        }

        /// <summary>
        /// Releases rundown protection.
        /// </summary>
        public void Release()
        {
            int value;

            while (true)
            {
                value = _value;

                // Has the rundown already started?
                if ((value & RundownActive) != 0)
                {
                    int newValue;

                    newValue = Interlocked.Add(ref _value, -RundownCountIncrement);

                    // Are we the last out? Set the event if that's the case.
                    if (newValue == RundownActive)
                    {
                        _wakeEvent.Set();
                    }

                    return;
                }
                else
                {
                    if (Interlocked.CompareExchange(
                        ref _value,
                        value - RundownCountIncrement,
                        value
                        ) == value)
                        return;
                }
            }
        }

        /// <summary>
        /// Releases rundown protection.
        /// </summary>
        /// <param name="count">The usage count to subtract.</param>
        public void Release(int count)
        {
            int value;

            while (true)
            {
                value = _value;

                // Has the rundown already started?
                if ((value & RundownActive) != 0)
                {
                    int newValue;

                    newValue = Interlocked.Add(ref _value, -RundownCountIncrement * count);

                    // Are we the last out? Set the event if that's the case.
                    if (newValue == RundownActive)
                    {
                        _wakeEvent.Set();
                    }

                    return;
                }
                else
                {
                    if (Interlocked.CompareExchange(
                        ref _value,
                        value - RundownCountIncrement * count,
                        value
                        ) == value)
                        return;
                }
            }
        }

        /// <summary>
        /// Waits for all references to be released while disallowing 
        /// attempts to acquire rundown protection.
        /// </summary>
        public void Wait()
        {
            this.Wait(Timeout.Infinite);
        }

        /// <summary>
        /// Waits for all references to be released while disallowing 
        /// attempts to acquire rundown protection.
        /// </summary>
        /// <param name="millisecondsTimeout">The timeout, in milliseconds.</param>
        /// <returns>Whether all references were released.</returns>
        public bool Wait(int millisecondsTimeout)
        {
            int value;

            // Fast path. Just in case there are no users, we can go ahead 
            // and set the active flag and exit. Or if someone has already 
            // initiated the rundown, exit as well.
            value = Interlocked.CompareExchange(ref _value, RundownActive, 0);

            if (value == 0 || value == RundownActive)
                return true;

            // Set the rundown active flag.
            do
            {
                value = _value;
            } while (Interlocked.CompareExchange(
                ref _value,
                value | RundownActive,
                value
                ) != value);

            // Wait for the event, but only if we had users.
            if ((value & ~RundownActive) != 0)
                return _wakeEvent.Wait(millisecondsTimeout);
            else
                return true;
        }
    }
}
