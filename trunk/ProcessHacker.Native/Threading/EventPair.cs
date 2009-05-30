using System;
using System.Collections.Generic;
using System.Text;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;

namespace ProcessHacker.Native.Threading
{
    /// <summary>
    /// Represents an event pair which contains two events, high and low.
    /// </summary>
    public class EventPair : NativeObject<EventPairHandle>
    {
        /// <summary>
        /// Creates an event pair.
        /// </summary>
        public EventPair()
            : this(null)
        { }

        /// <summary>
        /// Creates or opens an event pair.
        /// </summary>
        /// <param name="name">
        /// The name of the new event pair, or the name of an 
        /// existing event pair.
        /// </param>
        public EventPair(string name)
        {
            this.Handle = EventPairHandle.Create(
                EventPairAccess.All,
                name,
                ObjectFlags.OpenIf,
                null
                );
        }

        /// <summary>
        /// Sets the high event.
        /// </summary>
        public void SetHigh()
        {
            this.Handle.SetHigh();
        }

        /// <summary>
        /// Sets the high event and waits for the low event.
        /// </summary>
        public WaitStatus SetHighWaitLow()
        {
            return (WaitStatus)this.Handle.SetHighWaitLow();
        }

        /// <summary>
        /// Sets the low event.
        /// </summary>
        public void SetLow()
        {
            this.Handle.SetLow();
        }

        /// <summary>
        /// Sets the low event and waits for the high event.
        /// </summary>
        public WaitStatus SetLowWaitHigh()
        {
            return (WaitStatus)this.Handle.SetLowWaitHigh();
        }

        /// <summary>
        /// Waits for the high event.
        /// </summary>
        public WaitStatus WaitHigh()
        {
            return (WaitStatus)this.Handle.WaitHigh();
        }

        /// <summary>
        /// Waits for the low event.
        /// </summary>
        public WaitStatus WaitLow()
        {
            return (WaitStatus)this.Handle.WaitLow();
        }
    }
}
