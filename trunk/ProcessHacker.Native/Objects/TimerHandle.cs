using System;
using System.Collections.Generic;
using System.Text;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Security;
using System.Runtime.InteropServices;

namespace ProcessHacker.Native.Objects
{
    public class TimerHandle : Win32Handle<TimerAccess>
    {
        public delegate void TimerApcRoutine(IntPtr context, int lowValue, int highValue);

        public static TimerHandle Create(TimerAccess access, TimerType type)
        {
            return Create(access, null, type);
        }

        public static TimerHandle Create(TimerAccess access, string name, TimerType type)
        {
            return Create(access, name, null, type);
        }

        public static TimerHandle Create(TimerAccess access, string name, DirectoryHandle rootDirectory, TimerType type)
        {
            int status;
            ObjectAttributes oa = ObjectAttributes.Create(name, 0, rootDirectory);
            IntPtr handle;

            try
            {
                if ((status = Win32.NtCreateTimer(out handle, access, ref oa, type)) < 0)
                    Win32.ThrowLastError(status);
            }
            finally
            {
                oa.Dispose();
            }

            return new TimerHandle(handle, true);
        }

        private TimerApcRoutine _routine;

        private TimerHandle(IntPtr handle, bool owned)
            : base(handle, owned)
        { }

        public TimerHandle(string name, DirectoryHandle rootDirectory, TimerAccess access)
        {
            int status;
            ObjectAttributes oa = ObjectAttributes.Create(name, 0, rootDirectory);
            IntPtr handle;

            try
            {
                if ((status = Win32.NtOpenTimer(out handle, access, ref oa)) < 0)
                    Win32.ThrowLastError(status);
            }
            finally
            {
                oa.Dispose();
            }

            this.Handle = handle;
        }

        public TimerHandle(string name, TimerAccess access)
            : this(name, null, access)
        { }

        public bool Cancel()
        {
            int status;
            bool currentState;

            if ((status = Win32.NtCancelTimer(this, out currentState)) < 0)
                Win32.ThrowLastError(status);

            return currentState;
        }

        public TimerBasicInformation Query()
        {
            int status;
            TimerBasicInformation tbi;
            int retLength;

            if ((status = Win32.NtQueryTimer(this, TimerInformationClass.TimerBasicInformation,
                out tbi, Marshal.SizeOf(typeof(TimerBasicInformation)), out retLength)) < 0)
                Win32.ThrowLastError(status);

            return tbi;
        }

        public bool Set(long dueTime, TimerApcRoutine routine, IntPtr context, bool resume, int period)
        {
            int status;
            bool previousState;

            _routine = routine;

            if ((status = Win32.NtSetTimer(this, ref dueTime, routine, context,
                resume, period, out previousState)) < 0)
                Win32.ThrowLastError(status);

            return previousState;
        }

        public bool Set(long dueTime, TimerApcRoutine routine, IntPtr context, int period)
        {
            return this.Set(dueTime, routine, context, false, period);
        }

        public bool Set(long dueTime, TimerApcRoutine routine, int period)
        {
            return this.Set(dueTime, routine, IntPtr.Zero, period);
        }

        public bool Set(long dueTime, int period)
        {
            return this.Set(dueTime, null, period);
        }
    }
}
