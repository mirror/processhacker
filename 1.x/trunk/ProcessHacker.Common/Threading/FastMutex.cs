using System;
using System.Threading;

namespace ProcessHacker.Common.Threading
{
    /// <summary>
    /// Provides methods for synchronizing access to a shared resource.
    /// </summary>
    /// <remarks>Just a wrapper around Monitor (minus the event methods 
    /// like Pulse and Wait).</remarks>
    public sealed class FastMutex
    {
        /// <summary>
        /// Represents a context for mutex acquisition.
        /// </summary>
        public struct FastMutexContext : IDisposable
        {
            private bool _disposed;
            private FastMutex _fastMutex;

            internal FastMutexContext(FastMutex fastMutex)
            {
                _fastMutex = fastMutex;
                _disposed = false;
            }

            /// <summary>
            /// Releases the mutex.
            /// </summary>
            public void Dispose()
            {
                if (!_disposed)
                {
                    _fastMutex.Release();
                    _disposed = true;
                }
            }
        }

        private object _lock = new object();

        /// <summary>
        /// Acquires the mutex and prevents others from acquiring it. 
        /// If the mutex is already acquired, the function will block 
        /// until it can acquire the mutex.
        /// </summary>
        public void Acquire()
        {
            Monitor.Enter(_lock);
        }

        /// <summary>
        /// Acquires the mutex and returns a context object which 
        /// must be disposed to release the mutex.
        /// </summary>
        /// <returns>The context object.</returns>
        public FastMutexContext AcquireContext()
        {
            this.Acquire();
            return new FastMutexContext(this);
        }

        /// <summary>
        /// Releases the mutex and allows others to acquire the mutex.
        /// </summary>
        public void Release()
        {
            Monitor.Exit(_lock);
        }

        /// <summary>
        /// Attempts to acquire the mutex and returns immediately 
        /// regardless of whether the mutex was acquired.
        /// </summary>
        /// <returns>Whether or not the mutex was acquired.</returns>
        public bool TryAcquire()
        {
            return Monitor.TryEnter(_lock);
        }

        /// <summary>
        /// Attempts to acquire the mutex and returns after a 
        /// timeout period if the mutex could not be acquired.
        /// </summary>
        /// <param name="millisecondsTimeout">The timeout, in milliseconds.</param>
        /// <returns>Whether or not the mutex was acquired.</returns>
        public bool TryAcquire(int millisecondsTimeout)
        {
            return Monitor.TryEnter(_lock, millisecondsTimeout);
        }
    }
}
