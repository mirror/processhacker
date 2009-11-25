/*
 * Process Hacker - 
 *   event pair handle
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
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Security;

namespace ProcessHacker.Native.Objects
{
    /// <summary>
    /// Represents an event pair, an object consisting of two events, high and low.
    /// </summary>
    public sealed class EventPairHandle : NativeHandle<EventPairAccess>
    {
        /// <summary>
        /// Creates an unnamed event pair.
        /// </summary>
        /// <param name="access">The desired access to the event pair.</param>
        /// <returns>A handle to an event pair.</returns>
        public static EventPairHandle Create(EventPairAccess access)
        {
            return Create(access, null, 0, null);
        }

        /// <summary>
        /// Creates an event pair.
        /// </summary>
        /// <param name="access">The desired access to the event pair.</param>
        /// <param name="name">
        /// The name of the event pair. If rootDirectory is null, you must specify a fully 
        /// qualified name. Example: \BaseNamedObjects\MyEventPair.
        /// </param>     
        /// <param name="objectFlags">The flags to use when creating the object.</param>
        /// <param name="rootDirectory">
        /// The directory in which to place the event pair. This can be null.
        /// </param>
        /// <returns>A handle to an event pair.</returns>
        public static EventPairHandle Create(EventPairAccess access, string name, ObjectFlags objectFlags, DirectoryHandle rootDirectory)
        {
            NtStatus status;
            ObjectAttributes oa = new ObjectAttributes(name, objectFlags, rootDirectory);
            IntPtr handle;

            try
            {
                if ((status = Win32.NtCreateEventPair(out handle, access, ref oa)) >= NtStatus.Error)
                    Win32.Throw(status);
            }
            finally
            {
                oa.Dispose();
            }

            return new EventPairHandle(handle, true);
        }

        public static EventPairHandle FromHandle(IntPtr handle)
        {
            return new EventPairHandle(handle, false);
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
        /// <param name="objectFlags">The flags to use when opening the object.</param>
        /// <param name="rootDirectory">The directory object in which the event pair can be found.</param>
        /// <param name="access">The desired access to the event pair.</param>
        public EventPairHandle(string name, ObjectFlags objectFlags, DirectoryHandle rootDirectory, EventPairAccess access)
        {
            NtStatus status;
            ObjectAttributes oa = new ObjectAttributes(name, objectFlags, rootDirectory);
            IntPtr handle;

            try
            {
                if ((status = Win32.NtOpenEventPair(out handle, access, ref oa)) >= NtStatus.Error)
                    Win32.Throw(status);
            }
            finally
            {
                oa.Dispose();
            }

            this.Handle = handle;
        }

        public EventPairHandle(string name, EventPairAccess access)
            : this(name, 0, null, access)
        { }

        /// <summary>
        /// Sets the high event.
        /// </summary>
        public void SetHigh()
        {
            NtStatus status;

            if ((status = Win32.NtSetHighEventPair(this)) >= NtStatus.Error)
                Win32.Throw(status);
        }

        /// <summary>
        /// Sets the high event and waits for the low event.
        /// </summary>
        public NtStatus SetHighWaitLow()
        {
            NtStatus status;

            if ((status = Win32.NtSetHighWaitLowEventPair(this)) >= NtStatus.Error)
                Win32.Throw(status);

            return status;
        }

        /// <summary>
        /// Sets the low event.
        /// </summary>
        public void SetLow()
        {
            NtStatus status;

            if ((status = Win32.NtSetLowEventPair(this)) >= NtStatus.Error)
                Win32.Throw(status);
        }

        /// <summary>
        /// Sets the low event and waits for the high event.
        /// </summary>
        public NtStatus SetLowWaitHigh()
        {
            NtStatus status;

            if ((status = Win32.NtSetLowWaitHighEventPair(this)) >= NtStatus.Error)
                Win32.Throw(status);

            return status;
        }

        /// <summary>
        /// Waits for the high event.
        /// </summary>
        public NtStatus WaitHigh()
        {
            NtStatus status;

            if ((status = Win32.NtWaitHighEventPair(this)) >= NtStatus.Error)
                Win32.Throw(status);

            return status;
        }

        /// <summary>
        /// Waits for the low event.
        /// </summary>
        public NtStatus WaitLow()
        {
            NtStatus status;

            if ((status = Win32.NtWaitLowEventPair(this)) >= NtStatus.Error)
                Win32.Throw(status);

            return status;
        }
    }
}
