/*
 * Process Hacker - 
 *   timer
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
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;

namespace ProcessHacker.Native.Threading
{
    /// <summary>
    /// Represents a callback to be called when a timer is signaled.
    /// </summary>
    /// <param name="context">The context passed when the timer was set.</param>
    public delegate void TimerCallback(IntPtr context);

    /// <summary>
    /// Represents a timer.
    /// </summary>
    public sealed class Timer : NativeObject<TimerHandle>
    {
        private TimerCallback _callback;

        /// <summary>
        /// Creates a timer.
        /// </summary>
        public Timer()
            : this(null)
        { }

        /// <summary>
        /// Creates a timer.
        /// </summary>
        /// <param name="autoReset">
        /// Whether the timer should automatically reset to a 
        /// non-signaled state after waiters have been released.
        /// </param>
        public Timer(bool autoReset)
            : this(null, autoReset)
        { }

        /// <summary>
        /// Creates or opens a timer.
        /// </summary>
        /// <param name="name">
        /// The name of the new timer, or the name of an existing timer.
        /// </param>
        public Timer(string name)
            : this(name, false)
        { }

        /// <summary>
        /// Creates a timer.
        /// </summary>
        /// <param name="name">The name of the new timer.</param>
        /// <param name="autoReset">
        /// Whether the timer should automatically reset to a 
        /// non-signaled state after waiters have been released.
        /// </param>
        public Timer(string name, bool autoReset)
        {
            this.Handle = TimerHandle.Create(
                TimerAccess.All,
                name,
                ObjectFlags.OpenIf,
                null,
                autoReset ? TimerType.SynchronizationTimer : TimerType.NotificationTimer
                );
        }

        /// <summary>
        /// Gets the remaining time before the timer is signaled.
        /// </summary>
        public TimeSpan RemainingTime
        {
            get { return new TimeSpan(this.Handle.GetBasicInformation().RemainingTime); }
        }

        /// <summary>
        /// Gets whether the timer is signaled.
        /// </summary>
        public bool Signaled
        {
            get { return this.Handle.GetBasicInformation().TimerState; }
        }

        /// <summary>
        /// Cancels the timer, preventing it from being signaled.
        /// </summary>
        public void Cancel()
        {
            this.Handle.Cancel();
        }

        /// <summary>
        /// Starts the timer.
        /// </summary>
        /// <param name="dueTime">The due time, in milliseconds.</param>
        public void Set(int dueTime)
        {
            this.Set(dueTime, 0);
        }

        /// <summary>
        /// Starts the timer.
        /// </summary>
        /// <param name="dueTime">The due time, in milliseconds.</param>
        /// <param name="period">The interval to use for periodic signaling, in milliseconds.</param>
        public void Set(int dueTime, int period)
        {
            this.Set(null, dueTime, period);
        }

        /// <summary>
        /// Starts the timer.
        /// </summary>
        /// <param name="callback">A function to be called when the timer is signaled.</param>
        /// <param name="dueTime">The due time, in milliseconds.</param>
        /// <param name="period">The interval to use for periodic signaling, in milliseconds.</param>
        public void Set(TimerCallback callback, int dueTime, int period)
        {
            this.Set(callback, dueTime, period, IntPtr.Zero);
        }

        /// <summary>
        /// Starts the timer.
        /// </summary>
        /// <param name="callback">A function to be called when the timer is signaled.</param>
        /// <param name="dueTime">The due time, in milliseconds.</param>
        /// <param name="period">The interval to use for periodic signaling, in milliseconds.</param>
        /// <param name="context">A value to pass to the callback function.</param>
        public void Set(TimerCallback callback, int dueTime, int period, IntPtr context)
        {
            TimerApcRoutine apcRoutine = (context_, lowPart, highPart) => callback(context_);

            _callback = callback;
            this.Handle.Set(
                dueTime * Win32.TimeMsTo100Ns,
                true,
                callback != null ? apcRoutine : null,
                context,
                period
                );
        }

        /// <summary>
        /// Starts the timer.
        /// </summary>
        /// <param name="dueTime">The time at which the timer will be signaled.</param>
        public void Set(DateTime dueTime)
        {
            this.Set(dueTime, 0);
        }

        /// <summary>
        /// Starts the timer.
        /// </summary>
        /// <param name="dueTime">The time at which the timer will be signaled.</param>
        /// <param name="period">The interval to use for periodic signaling, in milliseconds.</param>
        public void Set(DateTime dueTime, int period)
        {
            this.Set(null, dueTime, period);
        }

        /// <summary>
        /// Starts the timer.
        /// </summary>
        /// <param name="callback">A function to be called when the timer is signaled.</param>
        /// <param name="dueTime">The time at which the timer will be signaled.</param>
        /// <param name="period">The interval to use for periodic signaling, in milliseconds.</param>
        public void Set(TimerCallback callback, DateTime dueTime, int period)
        {
            this.Set(callback, dueTime, period, IntPtr.Zero);
        }

        /// <summary>
        /// Starts the timer.
        /// </summary>
        /// <param name="callback">A function to be called when the timer is signaled.</param>
        /// <param name="dueTime">The time at which the timer will be signaled.</param>
        /// <param name="period">The interval to use for periodic signaling, in milliseconds.</param>
        /// <param name="context">A value to pass to the callback function.</param>
        public void Set(TimerCallback callback, DateTime dueTime, int period, IntPtr context)
        {
            TimerApcRoutine apcRoutine = (context_, lowPart, highPart) => callback(context_);

            _callback = callback;
            this.Handle.Set(
                dueTime.ToFileTime(),
                false,
                callback != null ? apcRoutine : null,
                context,
                period
                );
        }
    }
}
