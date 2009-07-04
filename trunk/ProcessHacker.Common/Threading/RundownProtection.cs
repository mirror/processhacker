using System;
using System.Threading;

namespace ProcessHacker.Common.Threading
{
    /// <summary>
    /// Provides methods for managing object/resource destruction.
    /// </summary>
    public sealed class RundownProtection
    {
        private object _rundownLock = new object();
        private volatile bool _rundownActive = false;
        private int _refCount = 0;

        /// <summary>
        /// Attempts to acquire rundown protection.
        /// </summary>
        /// <returns>Whether rundown protection was acquired.</returns>
        public bool Acquire()
        {
            Thread.BeginCriticalRegion();

            try
            {
                lock (_rundownLock)
                {
                    if (_rundownActive)
                        return false;

                    Interlocked.Increment(ref _refCount);

                    return true;
                }
            }
            finally
            {
                Thread.EndCriticalRegion();
            }
        }

        /// <summary>
        /// Releases rundown protection.
        /// </summary>
        public void Release()
        {
            Thread.BeginCriticalRegion();

            try
            {
                lock (_rundownLock)
                {
                    int newRefCount = Interlocked.Decrement(ref _refCount);

                    if (newRefCount < 0)
                        throw new InvalidOperationException("Reference count cannot be negative.");

                    if (_rundownActive)
                    {
                        // If we are the last out, release all waiters.
                        if (newRefCount == 0)
                            Monitor.PulseAll(_rundownLock);
                    }
                }
            }
            finally
            {
                Thread.EndCriticalRegion();
            }
        }

        /// <summary>
        /// Waits for all references to be released while disallowing 
        /// attempts to acquire rundown protection.
        /// </summary>
        public void Wait()
        {
            this.Wait(-1);
        }

        /// <summary>
        /// Waits for all references to be released while disallowing 
        /// attempts to acquire rundown protection.
        /// </summary>
        /// <param name="timeout">The timeout, in milliseconds.</param>
        /// <returns>Whether all references were released.</returns>
        public bool Wait(int timeout)
        {
            lock (_rundownLock)
            {
                _rundownActive = true;

                // If there are no references, we can exit.
                if (Thread.VolatileRead(ref _refCount) == 0)
                    return true;

                // Otherwise, wait for the release signal.
                return Monitor.Wait(_rundownLock, timeout);
            }
        }
    }
}
