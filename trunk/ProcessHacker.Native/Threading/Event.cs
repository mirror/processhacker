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
    public class Event : NativeObject<EventHandle>
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
