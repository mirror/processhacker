/*
 * Process Hacker - 
 *   fast event
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
    /// Provides a fast synchronization event.
    /// </summary>
    /// <remarks>
    /// This event structure will not create any kernel-mode 
    /// event object until necessary.
    /// </remarks>
    public struct FastEvent
    {
        private int _value;
        private ManualResetEvent _event;
        private int _eventRefCount;

        /// <summary>
        /// Creates a synchronization event.
        /// </summary>
        /// <param name="value">
        /// The initial value of the event. Always set to false.
        /// </param>
        public FastEvent(bool value)
        {
            _value = value ? 1 : 0;
            _event = null;
            // Initial reference for the Set method.
            _eventRefCount = 1;
        }

        /// <summary>
        /// Gets the current value of the event.
        /// </summary>
        public bool Value
        {
            get { return _value == 1; }
        }

        /// <summary>
        /// Dereferences the event, closing it if necessary.
        /// </summary>
        private void DerefEvent()
        {
            if (Interlocked.Decrement(ref _eventRefCount) == 0)
            {
                if (_event != null)
                {
                    _event.Close();
                    _event = null;
                }
            }
        }

        /// <summary>
        /// References the event.
        /// </summary>
        private void RefEvent()
        {
            Interlocked.Increment(ref _eventRefCount);
        }

        /// <summary>
        /// Sets the event.
        /// </summary>
        public void Set()
        {
            // 1. Value = 1.
            // 2. Event = Global Event.
            // 3. Set Event.
            // 4. [Optional] Dereference the Global Event.

            int oldValue;

            // Set the value.
            oldValue = Interlocked.Exchange(ref _value, 1);

            // Don't try to do anything if the event has already been set.
            if (oldValue == 1)
                return;

            // Do an update-to-date read.
            ManualResetEvent localEvent = Interlocked.CompareExchange<ManualResetEvent>(
                ref _event,
                null,
                null
                );

            // Set the event if we had one.
            if (localEvent != null)
            {
                localEvent.Set();
            }

            // Note that at this point we don't need to worry about anyone 
            // creating the event and waiting for it, because if they did 
            // they would check the value first. It would be 1, so they 
            // wouldn't wait at all.

            this.DerefEvent();
        }

        /// <summary>
        /// Waits for the event to be set.
        /// </summary>
        public void Wait()
        {
            this.Wait(Timeout.Infinite);
        }

        /// <summary>
        /// Waits for the event to be set.
        /// </summary>
        /// <param name="millisecondsTimeout">The number of milliseconds to wait.</param>
        /// <returns>Whether the event was set before the timeout period elapsed.</returns>
        public bool Wait(int millisecondsTimeout)
        {
            // 1. [Optional] If Value = 1, Return.
            // 2. [Optional] If Timeout = 0 And Value = 0, Return.
            // 3. [Optional] Reference the Global Event.
            // 4. [Optional] If Global Event is present, skip Step 5.
            // 5. Create Event.
            // 6. Global Event = Event only if Global Event is not present.
            // 7. If Value = 1, Return (rather, go to Step 9).
            // 8. Wait for Global Event.
            // 9. [Optional] Dereference the Global Event.

            bool result;
            ManualResetEvent newEvent;

            // Shortcut: return immediately if the event is set.
            if (Thread.VolatileRead(ref _value) == 1)
                return true;

            // Shortcut: if the timeout is 0, return immediately if 
            // the event isn't set.
            if (millisecondsTimeout == 0)
            {
                if (Thread.VolatileRead(ref _value) == 0)
                    return false;
            }

            // Prevent the event from being closed or invalidated.
            this.RefEvent();

            // Shortcut: don't bother creating an event if we already have one.
            newEvent = Interlocked.CompareExchange<ManualResetEvent>(ref _event, null, null);

            // If we don't have an event, create one and try to set it.
            if (newEvent == null)
            {
                // Create an event. We might not need it, though.
                newEvent = new ManualResetEvent(false);

                // Atomically use the event only if we don't already 
                // have one.
                if (Interlocked.CompareExchange<ManualResetEvent>(
                    ref _event,
                    newEvent,
                    null
                    ) != null)
                {
                    // Someone else set the event before we did.
                    newEvent.Close();
                }
            }

            try
            {
                // Check the value to see if we are meant to wait.
                if (Thread.VolatileRead(ref _value) == 1)
                    return true;

                result = _event.WaitOne(millisecondsTimeout, false);

                return result;
            }
            finally
            {
                // We don't need the event anymore.
                this.DerefEvent();
            }
        }
    }
}
