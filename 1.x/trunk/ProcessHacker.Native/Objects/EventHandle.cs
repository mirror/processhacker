/*
 * Process Hacker - 
 *   event handle
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
    public sealed class EventHandle : NativeHandle<EventAccess>
    {
        public static EventHandle Create(EventAccess access, EventType type, bool initialState)
        {
            return Create(access, null, type, initialState);
        }

        public static EventHandle Create(EventAccess access, string name, EventType type, bool initialState)
        {
            return Create(access, name, 0, null, type, initialState);
        }

        public static EventHandle Create(EventAccess access, string name, ObjectFlags objectFlags, DirectoryHandle rootDirectory, EventType type, bool initialState)
        {
            ObjectAttributes oa = new ObjectAttributes(name, objectFlags, rootDirectory);
            IntPtr handle;

            try
            {
                Win32.NtCreateEvent(out handle, access, ref oa, type, initialState).ThrowIf();
            }
            finally
            {
                oa.Dispose();
            }

            return new EventHandle(handle, true);
        }

        public static EventHandle FromHandle(IntPtr handle)
        {
            return new EventHandle(handle, false);
        }

        private EventHandle(IntPtr handle, bool owned)
            : base(handle, owned)
        { }

        public EventHandle(string name, EventAccess access)
            : this(name, 0, null, access)
        { }

        public EventHandle(string name, ObjectFlags objectFlags, DirectoryHandle rootDirectory, EventAccess access)
        {
            ObjectAttributes oa = new ObjectAttributes(name, objectFlags, rootDirectory);
            IntPtr handle;

            try
            {
                Win32.NtOpenEvent(out handle, access, ref oa).ThrowIf();
            }
            finally
            {
                oa.Dispose();
            }

            this.Handle = handle;
        }

        public void Clear()
        {
            Win32.NtClearEvent(this).ThrowIf();
        }

        public EventBasicInformation BasicInformation
        {
            get
            {
                EventBasicInformation ebi;
                int retLength;

                Win32.NtQueryEvent(
                    this,
                    EventInformationClass.EventBasicInformation,
                    out ebi,
                    EventBasicInformation.SizeOf,
                    out retLength
                    ).ThrowIf();

                return ebi;
            }
        }

        public int Pulse()
        {
            int previousState;

            Win32.NtPulseEvent(this, out previousState).ThrowIf();

            return previousState;
        }

        public int Reset()
        {
            int previousState;

            Win32.NtResetEvent(this, out previousState).ThrowIf();

            return previousState;
        }

        public int Set()
        {
            int previousState;

            Win32.NtSetEvent(this, out previousState).ThrowIf();

            return previousState;
        }

        /// <summary>
        /// Sets the event and causes the waiting thread to be context switched 
        /// to regardless of its priority.
        /// </summary>
        public void SetBoostPriority()
        {
            Win32.NtSetEventBoostPriority(this).ThrowIf();
        }
    }
}
