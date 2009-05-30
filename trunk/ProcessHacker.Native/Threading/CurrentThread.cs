using System;
using System.Collections.Generic;
using System.Text;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;

namespace ProcessHacker.Native.Threading
{
    /// <summary>
    /// Provides methods for manipulating the current thread.
    /// </summary>
    public static class CurrentThread
    {
        /// <summary>
        /// Switches to another thread.
        /// </summary>
        public static void Sleep()
        {
            Yield();
        }

        /// <summary>
        /// Suspends execution of the current thread.
        /// </summary>
        /// <param name="interval">The interval to sleep, in milliseconds.</param>
        public static void Sleep(int interval)
        {
            ThreadHandle.Sleep(interval * Win32.TimeMsTo100Ns, true);
        }

        /// <summary>
        /// Suspends execution of the current thread.
        /// </summary>
        /// <param name="time">The time at which wake up.</param>
        public static void Sleep(DateTime time)
        {
            ThreadHandle.Sleep(time.ToFileTime(), false);
        }

        /// <summary>
        /// Switches to another thread.
        /// </summary>
        public static void Yield()
        {
            ThreadHandle.Yield();
        }
    }
}
