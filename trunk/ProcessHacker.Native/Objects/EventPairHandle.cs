using System;
using System.Collections.Generic;
using System.Text;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Security;

namespace ProcessHacker.Native.Objects
{
    /// <summary>
    /// Represents an event pair, an object consisting of two events, high and low.
    /// </summary>
    public class EventPairHandle : Win32Handle<EventPairAccess>
    {
        /// <summary>
        /// Creates an unnamed event pair.
        /// </summary>
        /// <param name="access">The desired access to the event pair.</param>
        /// <returns>A handle to an event pair.</returns>
        public static EventPairHandle Create(EventPairAccess access)
        {
            return Create(access, null, null);
        }

        /// <summary>
        /// Creates an event pair.
        /// </summary>
        /// <param name="access">The desired access to the event pair.</param>
        /// <param name="name">
        /// The name of the event pair. If rootDirectory is null, you must specify a fully 
        /// qualified name. Example: \BaseNamedObjects\MyEventPair.
        /// </param>              
        /// <param name="rootDirectory">
        /// The directory in which to place the event pair. This can be null.
        /// </param>
        /// <returns>A handle to an event pair.</returns>
        public static EventPairHandle Create(EventPairAccess access, string name, DirectoryHandle rootDirectory)
        {
            int status;
            ObjectAttributes oa = ObjectAttributes.Create(name, 0, rootDirectory);
            IntPtr handle;

            try
            {
                if ((status = Win32.NtCreateEventPair(out handle, access, ref oa)) < 0)
                    Win32.ThrowLastError(status);
            }
            finally
            {
                oa.Dispose();
            }

            return new EventPairHandle(handle, true);
        }

        private EventPairHandle(IntPtr handle, bool owned)
            : base(handle, owned)
        { }

        /// <summary>
        /// Opens a named event pair.
        /// </summary>
        /// <param name="name">
        /// The name of the event pair. If rootDirectory is null, 
        /// you must specify a fully qualified name.</param>
        /// <param name="rootDirectory">The directory object in which the event pair can be found.</param>
        /// <param name="access">The desired access to the event pair.</param>
        public EventPairHandle(string name, DirectoryHandle rootDirectory, EventPairAccess access)
        {
            int status;
            ObjectAttributes oa = ObjectAttributes.Create(name, 0, rootDirectory);
            IntPtr handle;

            try
            {
                if ((status = Win32.NtOpenEventPair(out handle, access, ref oa)) < 0)
                    Win32.ThrowLastError(status);
            }
            finally
            {
                oa.Dispose();
            }

            this.Handle = handle;
        }

        public EventPairHandle(string name, EventPairAccess access)
            : this(name, null, access)
        { }

        /// <summary>
        /// Sets the high event.
        /// </summary>
        public void SetHigh()
        {
            int status;

            if ((status = Win32.NtSetHighEventPair(this)) < 0)
                Win32.ThrowLastError(status);
        }

        /// <summary>
        /// Sets the high event and waits for the low event.
        /// </summary>
        public void SetHighWaitLow()
        {
            int status;

            if ((status = Win32.NtSetHighWaitLowEventPair(this)) < 0)
                Win32.ThrowLastError(status);
        }

        /// <summary>
        /// Sets the low event.
        /// </summary>
        public void SetLow()
        {
            int status;

            if ((status = Win32.NtSetLowEventPair(this)) < 0)
                Win32.ThrowLastError(status);
        }

        /// <summary>
        /// Sets the low event and waits for the high event.
        /// </summary>
        public void SetLowWaitHigh()
        {
            int status;

            if ((status = Win32.NtSetLowWaitHighEventPair(this)) < 0)
                Win32.ThrowLastError(status);
        }

        /// <summary>
        /// Waits for the high event.
        /// </summary>
        public void WaitHigh()
        {
            int status;

            if ((status = Win32.NtWaitHighEventPair(this)) < 0)
                Win32.ThrowLastError(status);
        }

        /// <summary>
        /// Waits for the low event.
        /// </summary>
        public void WaitLow()
        {
            int status;

            if ((status = Win32.NtWaitLowEventPair(this)) < 0)
                Win32.ThrowLastError(status);
        }
    }
}
