using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ProcessHacker.Common.Threading
{
    public struct FastLock : IDisposable
    {
        private const int SpinCount = 40000;

        private int _value;
        private AutoResetEvent _event;
        private bool _spin;

        public FastLock(bool value)
        {
            _value = value ? 1 : 0;
            _event = null;

            // We don't want to spin on uniprocessor systems.
            _spin = Environment.ProcessorCount != 1;
        }

        public void Dispose()
        {
            if (_event != null)
                _event.Close();
        }

        public void Acquire()
        {
            // Fast path.

            if (Interlocked.CompareExchange(ref _value, 1, 0) == 0)
                return;

            // Slow path 1 - spin for a while.

            if (_spin)
            {
                for (int i = 0; i < SpinCount; i++)
                {
                    if (Interlocked.CompareExchange(ref _value, 1, 0) == 0)
                        return;
                }
            }

            // Slow path 2 - wait on the event.

            AutoResetEvent newEvent;

            // Note: see FastEvent.cs for a more detailed explanation of this 
            // technique.

            newEvent = Interlocked.CompareExchange<AutoResetEvent>(ref _event, null, null);

            if (newEvent == null)
            {
                newEvent = new AutoResetEvent(false);

                if (Interlocked.CompareExchange<AutoResetEvent>(
                    ref _event,
                    newEvent,
                    null
                    ) != null)
                {
                    newEvent.Close();
                }
            }

            // Loop trying to acquire the lock. Note that after we 
            // get woken up another thread might have acquired the lock, 
            // and that's why we need a loop.
            while (true)
            {
                if (Interlocked.CompareExchange(ref _value, 1, 0) == 0)
                    break;

                _event.WaitOne();
            }
        }

        public bool Acquire(int millisecondsTimeout)
        {
            // Fast path.

            if (Interlocked.CompareExchange(ref _value, 1, 0) == 0)
                return true;

            // Slow path 1 - spin for a while.

            if (_spin)
            {
                for (int i = 0; i < SpinCount; i++)
                {
                    if (Interlocked.CompareExchange(ref _value, 1, 0) == 0)
                        return true;
                }
            }

            // Slow path 2 - wait on the event.

            AutoResetEvent newEvent;

            // Note: see FastEvent.cs for a more detailed explanation of this 
            // technique.

            newEvent = Interlocked.CompareExchange<AutoResetEvent>(ref _event, null, null);

            if (newEvent == null)
            {
                newEvent = new AutoResetEvent(false);

                if (Interlocked.CompareExchange<AutoResetEvent>(
                    ref _event,
                    newEvent,
                    null
                    ) != null)
                {
                    newEvent.Close();
                }
            }

            int timeout = millisecondsTimeout;

            // Loop trying to acquire the lock. Note that after we 
            // get woken up another thread might have acquired the lock, 
            // and that's why we need a loop.
            while (true)
            {
                if (Interlocked.CompareExchange(ref _value, 1, 0) == 0)
                    return true;

                if (timeout != Timeout.Infinite && timeout <= 0)
                    return false;

                int tickCount = Environment.TickCount;

                if (!_event.WaitOne(timeout))
                    return false;

                tickCount = Environment.TickCount - tickCount;

                // Recompute the timeout.
                if (tickCount > 0 && timeout != Timeout.Infinite)
                    timeout -= tickCount;
            }
        }

        public void Release()
        {
            Interlocked.Exchange(ref _value, 0);

            // Wake up a thread. Note that that thread might 
            // not actually get to acquire the lock because 
            // another thread may have acquired it already.
            if (_event != null)
            {
                _event.Set();
            }
        }

        public bool TryAcquire()
        {
            return Interlocked.CompareExchange(ref _value, 1, 0) == 0;
        }
    }
}
