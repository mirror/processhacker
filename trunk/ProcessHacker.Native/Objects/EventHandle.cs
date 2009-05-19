using System;
using System.Runtime.InteropServices;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Security;

namespace ProcessHacker.Native.Objects
{
    public class EventHandle : Win32Handle<EventAccess>
    {
        public static EventHandle Create(EventAccess access, EventType type, bool initialState)
        {
            return Create(access, null, type, initialState);
        }

        public static EventHandle Create(EventAccess access, string name, EventType type, bool initialState)
        {
            return Create(access, name, null, type, initialState);
        }

        public static EventHandle Create(EventAccess access, string name, DirectoryHandle rootDirectory, EventType type, bool initialState)
        {
            int status;
            ObjectAttributes oa = ObjectAttributes.Create(name, 0, rootDirectory);
            IntPtr handle;

            try
            {
                if ((status = Win32.NtCreateEvent(out handle, access, ref oa, type, initialState)) < 0)
                    Win32.ThrowLastError(status);
            }
            finally
            {
                oa.Dispose();
            }

            return new EventHandle(handle, true);
        }

        private EventHandle(IntPtr handle, bool owned)
            : base(handle, owned)
        { }

        public EventHandle(string name, DirectoryHandle rootDirectory, EventAccess access)
        {
            int status;
            ObjectAttributes oa = ObjectAttributes.Create(name, 0, rootDirectory);
            IntPtr handle;

            try
            {
                if ((status = Win32.NtOpenEvent(out handle, access, ref oa)) < 0)
                    Win32.ThrowLastError(status);
            }
            finally
            {
                oa.Dispose();
            }

            this.Handle = handle;
        }

        public void Clear()
        {
            int status;

            if ((status = Win32.NtClearEvent(this)) < 0)
                Win32.ThrowLastError(status);
        }

        public int Pulse()
        {
            int status;
            int previousState;

            if ((status = Win32.NtPulseEvent(this, out previousState)) < 0)
                Win32.ThrowLastError(status);

            return previousState;
        }

        public EventBasicInformation Query()
        {
            int status;
            EventBasicInformation ebi;
            int retLength;

            if ((status = Win32.NtQueryEvent(this, EventInformationClass.EventBasicInformation,
                out ebi, Marshal.SizeOf(typeof(EventBasicInformation)), out retLength)) < 0)
                Win32.ThrowLastError(status);

            return ebi;
        }

        public int Reset()
        {
            int status;
            int previousState;

            if ((status = Win32.NtResetEvent(this, out previousState)) < 0)
                Win32.ThrowLastError(status);

            return previousState;
        }

        public int Set()
        {
            int status;
            int previousState;

            if ((status = Win32.NtSetEvent(this, out previousState)) < 0)
                Win32.ThrowLastError(status);

            return previousState;
        }

        /// <summary>
        /// Sets the event and causes the waiting thread to be context switched 
        /// to regardless of its priority.
        /// </summary>
        public void SetBoostPriority()
        {
            int status;

            if ((status = Win32.NtSetEventBoostPriority(this)) < 0)
                Win32.ThrowLastError(status);
        }
    }
}
