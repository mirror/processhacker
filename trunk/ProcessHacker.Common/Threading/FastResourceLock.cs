using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessHacker.Common.Threading
{
    public unsafe class FastResourceLock
    {
        // Indicates that the lock is held.
        private const int _locked = 0x1;
        // Indicates that there are chained waiters.
        private const int _waiters = 0x2;
        // Flags mask.
        private const int _flags = 0x3;

        private int _spinCount;
        private void* _value;

        public struct FastResourceLockContext : IDisposable
        {
            private bool _disposed;
            private FastResourceLock _lock;
            private bool _shared;

            internal FastResourceLockContext(FastResourceLock loc, bool shared)
            {
                _disposed = false;
                _lock = loc;
                _shared = shared;
            }

            public void Dispose()
            {
                if (!_disposed)
                {
                    if (_shared)
                        _lock.ReleaseShared();
                    else
                        _lock.ReleaseExclusive();

                    _disposed = true;
                }
            }
        }

        public struct FastResourceWaitBlock
        {
            public FastResourceWaitBlock* Previous;
            public FastResourceWaitBlock* Next;
            public FastResourceWaitBlock* Last;

            public int SharedCount;

            public IntPtr WakeEventHandle;
        }

        public FastResourceLock()
        {
            _value = null;

            if (Environment.ProcessorCount == 1)
                _spinCount = 0;
            else
                _spinCount = 1024;
        }

        public void AcquireExclusive()
        {

        }

        public FastResourceLockContext AcquireExclusiveContext()
        {
            this.AcquireExclusive();
            return new FastResourceLockContext(this, false);
        }

        public void AcquireShared()
        {

        }

        public FastResourceLockContext AcquireSharedContext()
        {
            this.AcquireShared();
            return new FastResourceLockContext(this, true);
        }

        public void ReleaseExclusive()
        {

        }

        public void ReleaseShared()
        {

        }
    }
}
