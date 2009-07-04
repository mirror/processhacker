/*
 * Process Hacker - 
 *   event
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
    /// Represents a thread synchronization event.
    /// </summary>
    public sealed class Event : NativeObject<EventHandle>
    {
        /// <summary>
        /// Creates an event.
        /// </summary>
        public Event()
            : this(null)
        { }

        /// <summary>
        /// Creates an event.
        /// </summary>
        /// <param name="autoReset">
        /// Whether the event should automatically reset to a non-signaled state
        /// after all waiters are released.
        /// </param>
        /// <param name="initialState">
        /// Whether the event should be set to a signaled state initially.
        /// </param>
        public Event(bool autoReset, bool initialState)
            : this(null, autoReset, initialState)
        { }

        /// <summary>
        /// Creates or opens an event.
        /// </summary>
        /// <param name="name">
        /// The name of the new event, or the name of an existing event to open.
        /// </param>
        public Event(string name)
            : this(name, false, false)
        { }

        /// <summary>
        /// Creates an event.
        /// </summary>
        /// <param name="name">
        /// The name of the new event.
        /// </param>
        /// <param name="autoReset">
        /// Whether the event should automatically reset to a non-signaled state
        /// after all waiters are released.
        /// </param>
        /// <param name="initialState">
        /// Whether the event should be set to a signaled state initially.
        /// </param>
        public Event(string name, bool autoReset, bool initialState)
        {
            this.Handle = EventHandle.Create(
                EventAccess.All,
                name,
                ObjectFlags.OpenIf,
                null,
                autoReset ? EventType.SynchronizationEvent : EventType.NotificationEvent,
                initialState
                );
        }

        /// <summary>
        /// Gets whether the event will automatically reset 
        /// after waiters are released.
        /// </summary>
        public bool AutoReset
        {
            get
            {
                return this.Handle.GetBasicInformation().EventType == 
                    EventType.SynchronizationEvent;
            }
        }

        /// <summary>
        /// Gets whether the event is in the signaled state.
        /// </summary>
        public bool Signaled
        {
            get { return this.Handle.GetBasicInformation().EventState != 0; }
        }

        /// <summary>
        /// Attempts to satisfy as many waits as possible and sets 
        /// the event's state to non-signaled.
        /// </summary>
        public void Pulse()
        {
            this.Handle.Pulse();
        }

        /// <summary>
        /// Sets the event's state to non-signaled.
        /// </summary>
        public void Reset()
        {
            this.Handle.Reset();
        }

        /// <summary>
        /// Sets the event's state to signaled.
        /// </summary>
        public void Set()
        {
            this.Handle.Set();
        }
    }
}
