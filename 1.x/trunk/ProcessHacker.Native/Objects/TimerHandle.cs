using System;
using System.Runtime.InteropServices;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Security;

namespace ProcessHacker.Native.Objects
{
    public sealed class TimerHandle : NativeHandle<TimerAccess>
    {
        /// <summary>
        /// Creates a timer.
        /// </summary>
        /// <param name="access">The desired access to the timer.</param>
        /// <param name="type">
        /// The type of timer; synchronization timers will be reset once waiting threads are released.
        /// </param>
        /// <returns>A handle to the timer.</returns>
        public static TimerHandle Create(TimerAccess access, TimerType type)
        {
            return Create(access, null, type);
        }

        /// <summary>
        /// Creates a timer.
        /// </summary>
        /// <param name="access">The desired access to the timer.</param>
        /// <param name="name">A name for the timer in the object manager namespace.</param>
        /// <param name="type">
        /// The type of timer; synchronization timers will be reset once waiting threads are released.
        /// </param>
        /// <returns>A handle to the timer.</returns>
        public static TimerHandle Create(TimerAccess access, string name, TimerType type)
        {
            return Create(access, name, 0, null, type);
        }

        /// <summary>
        /// Creates a timer.
        /// </summary>
        /// <param name="access">The desired access to the timer.</param>
        /// <param name="name">A name for the timer in the object manager namespace.</param>
        /// <param name="objectFlags">The flags to use when creating the object.</param>
        /// <param name="rootDirectory">The directory in which to place the timer. This can be null.</param>
        /// <param name="type">
        /// The type of timer; synchronization timers will be reset once waiting threads are released.
        /// </param>
        /// <returns>A handle to the timer.</returns>
        public static TimerHandle Create(TimerAccess access, string name, ObjectFlags objectFlags, DirectoryHandle rootDirectory, TimerType type)
        {
            NtStatus status;
            ObjectAttributes oa = new ObjectAttributes(name, objectFlags, rootDirectory);
            IntPtr handle;

            try
            {
                if ((status = Win32.NtCreateTimer(out handle, access, ref oa, type)) >= NtStatus.Error)
                    Win32.Throw(status);
            }
            finally
            {
                oa.Dispose();
            }

            return new TimerHandle(handle, true);
        }

        public static TimerHandle FromHandle(IntPtr handle)
        {
            return new TimerHandle(handle, false);
        }

        private TimerApcRoutine _routine;

        private TimerHandle(IntPtr handle, bool owned)
            : base(handle, owned)
        { }

        public TimerHandle(string name, ObjectFlags objectFlags, DirectoryHandle rootDirectory, TimerAccess access)
        {
            NtStatus status;
            ObjectAttributes oa = new ObjectAttributes(name, objectFlags, rootDirectory);
            IntPtr handle;

            try
            {
                if ((status = Win32.NtOpenTimer(out handle, access, ref oa)) >= NtStatus.Error)
                    Win32.Throw(status);
            }
            finally
            {
                oa.Dispose();
            }

            this.Handle = handle;
        }

        public TimerHandle(string name, TimerAccess access)
            : this(name, 0, null, access)
        { }

        /// <summary>
        /// Cancels the timer, preventing it from being signaled.
        /// </summary>
        /// <returns>The state of the timer (whether it is signaled).</returns>
        public bool Cancel()
        {
            NtStatus status;
            bool currentState;

            if ((status = Win32.NtCancelTimer(this, out currentState)) >= NtStatus.Error)
                Win32.Throw(status);

            return currentState;
        }

        /// <summary>
        /// Gets information about the timer.
        /// </summary>
        public TimerBasicInformation GetBasicInformation()
        {
            NtStatus status;
            TimerBasicInformation tbi;
            int retLength;

            if ((status = Win32.NtQueryTimer(this, TimerInformationClass.TimerBasicInformation,
                out tbi, Marshal.SizeOf(typeof(TimerBasicInformation)), out retLength)) >= NtStatus.Error)
                Win32.Throw(status);

            return tbi;
        }

        /// <summary>
        /// Sets the timer.
        /// </summary>
        /// <param name="dueTime">The time at which the timer is to be signaled.</param>
        /// <param name="period">
        /// The time interval for periodic signaling of the timer, in milliseconds. 
        /// Specify 0 for no periodic signaling.
        /// </param>
        /// <returns>The state of the timer (whether it is signaled).</returns>
        public bool Set(DateTime dueTime, int period)
        {
            return this.Set(dueTime.ToFileTime(), false, null, IntPtr.Zero, period);
        }

        /// <summary>
        /// Sets the timer.
        /// </summary>
        /// <param name="dueTime">A relative due time, in 100ns units.</param>
        /// <param name="period">
        /// The time interval for periodic signaling of the timer, in milliseconds. 
        /// Specify 0 for no periodic signaling.
        /// </param>
        /// <returns>The state of the timer (whether it is signaled).</returns>
        public bool Set(long dueTime, int period)
        {
            return this.Set(dueTime, null, period);
        }

        /// <summary>
        /// Sets the timer.
        /// </summary>
        /// <param name="dueTime">A relative due time, in 100ns units.</param>
        /// <param name="routine">A routine to call when the timer is signaled.</param>
        /// <param name="period">
        /// The time interval for periodic signaling of the timer, in milliseconds. 
        /// Specify 0 for no periodic signaling.
        /// </param>
        /// <returns>The state of the timer (whether it is signaled).</returns>
        public bool Set(long dueTime, TimerApcRoutine routine, int period)
        {
            return this.Set(dueTime, true, routine, IntPtr.Zero, period);
        }

        /// <summary>
        /// Sets the timer.
        /// </summary>
        /// <param name="dueTime">A due time, in 100ns units.</param>
        /// <param name="relative">Whether the due time is relative.</param>
        /// <param name="routine">A routine to call when the timer is signaled.</param>
        /// <param name="context">A value to pass to the timer callback routine.</param>
        /// <param name="period">
        /// The time interval for periodic signaling of the timer, in milliseconds. 
        /// Specify 0 for no periodic signaling.
        /// </param>
        /// <returns>The state of the timer (whether it is signaled).</returns>
        public bool Set(long dueTime, bool relative, TimerApcRoutine routine, IntPtr context, int period)
        {
            return this.Set(dueTime, relative, routine, context, false, period);
        }

        /// <summary>
        /// Sets the timer.
        /// </summary>
        /// <param name="dueTime">A due time, in 100ns units.</param>
        /// <param name="relative">Whether the due time is relative.</param>
        /// <param name="routine">A routine to call when the timer is signaled.</param>
        /// <param name="context">A value to pass to the timer callback routine.</param>
        /// <param name="resume">
        /// Whether the power manager should restore the system when the timer is signaled.
        /// </param>
        /// <param name="period">
        /// The time interval for periodic signaling of the timer, in milliseconds. 
        /// Specify 0 for no periodic signaling.
        /// </param>
        /// <returns>The state of the timer (whether it is signaled).</returns>
        public bool Set(long dueTime, bool relative, TimerApcRoutine routine, IntPtr context, bool resume, int period)
        {
            NtStatus status;
            long realDueTime = relative ? -dueTime : dueTime;
            bool previousState;

            // Keep the APC routine alive.
            _routine = routine;

            if ((status = Win32.NtSetTimer(
                this,
                ref realDueTime,
                routine,
                context,
                resume,
                period,
                out previousState
                )) >= NtStatus.Error)
                Win32.Throw(status);

            return previousState;
        }
    }
}
