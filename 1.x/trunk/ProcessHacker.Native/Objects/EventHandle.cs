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
using System.Runtime.InteropServices;
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
            NtStatus status;
            ObjectAttributes oa = new ObjectAttributes(name, objectFlags, rootDirectory);
            IntPtr handle;

            try
            {
                if ((status = Win32.NtCreateEvent(out handle, access, ref oa, type, initialState)) >= NtStatus.Error)
                    Win32.Throw(status);
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
            NtStatus status;
            ObjectAttributes oa = new ObjectAttributes(name, objectFlags, rootDirectory);
            IntPtr handle;

            try
            {
                if ((status = Win32.NtOpenEvent(out handle, access, ref oa)) >= NtStatus.Error)
                    Win32.Throw(status);
            }
            finally
            {
                oa.Dispose();
            }

            this.Handle = handle;
        }

        public void Clear()
        {
            NtStatus status;

            if ((status = Win32.NtClearEvent(this)) >= NtStatus.Error)
                Win32.Throw(status);
        }

        public EventBasicInformation GetBasicInformation()
        {
            NtStatus status;
            EventBasicInformation ebi;
            int retLength;

            if ((status = Win32.NtQueryEvent(this, EventInformationClass.EventBasicInformation,
                out ebi, Marshal.SizeOf(typeof(EventBasicInformation)), out retLength)) >= NtStatus.Error)
                Win32.Throw(status);

            return ebi;
        }

        public int Pulse()
        {
            NtStatus status;
            int previousState;

            if ((status = Win32.NtPulseEvent(this, out previousState)) >= NtStatus.Error)
                Win32.Throw(status);

            return previousState;
        }

        public int Reset()
        {
            NtStatus status;
            int previousState;

            if ((status = Win32.NtResetEvent(this, out previousState)) >= NtStatus.Error)
                Win32.Throw(status);

            return previousState;
        }

        public int Set()
        {
            NtStatus status;
            int previousState;

            if ((status = Win32.NtSetEvent(this, out previousState)) >= NtStatus.Error)
                Win32.Throw(status);

            return previousState;
        }

        /// <summary>
        /// Sets the event and causes the waiting thread to be context switched 
        /// to regardless of its priority.
        /// </summary>
        public void SetBoostPriority()
        {
            NtStatus status;

            if ((status = Win32.NtSetEventBoostPriority(this)) >= NtStatus.Error)
                Win32.Throw(status);
        }
    }
}
