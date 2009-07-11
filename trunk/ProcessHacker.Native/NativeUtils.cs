using System;
using ProcessHacker.Native.Objects;

namespace ProcessHacker.Native
{
    /// <summary>
    /// Provides various utility methods.
    /// </summary>
    public static class NativeUtils
    {
        /// <summary>
        /// Calls a function.
        /// </summary>
        /// <param name="address">The address of the function.</param>
        /// <param name="param1">The first parameter to pass.</param>
        /// <param name="param2">The second parameter to pass.</param>
        /// <param name="param3">The third parameter to pass.</param>
        public static void Call(IntPtr address, IntPtr param1, IntPtr param2, IntPtr param3)
        {
            // Queue a user-mode APC to the current thread.
            ThreadHandle.Current.QueueApc(address, param1, param2, param3);
            // Flush the APC queue.
            ThreadHandle.TestAlert();
        }
    }
}
