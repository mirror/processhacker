using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ProcessHacker.Common.Threading
{
    public struct FastLock : IDisposable
    {
        private const int SpinCount = 4000;

        private int _value;
        private IntPtr _event;

        public FastLock(bool value)
        {
            _value = value ? 1 : 0;
            _event = IntPtr.Zero;
        }

        public void Dispose()
        {
            if (_event != IntPtr.Zero)
                NativeMethods.CloseHandle(_event);
        }

        public void Acquire()
        {
            // Fast path.

            if (Interlocked.CompareExchange(ref _value, 1, 0) == 0)
                return;

            // Slow path 1 - spin for a while.

            if (NativeMethods.SpinEnabled)
            {
                for (int i = 0; i < SpinCount; i++)
                {
                    if (Interlocked.CompareExchange(ref _value, 1, 0) == 0)
                        return;
                }
            }

            // Slow path 2 - wait on the event.

            IntPtr newEvent;

            // Note: see FastEvent.cs for a more detailed explanation of this 
            // technique.

            newEvent = Interlocked.CompareExchange(ref _event, IntPtr.Zero, IntPtr.Zero);

            if (newEvent == IntPtr.Zero)
            {
                newEvent = NativeMethods.CreateEvent(IntPtr.Zero, false, false, null);

                if (Interlocked.CompareExchange(
                    ref _event,
                    newEvent,
                    IntPtr.Zero
                    ) != IntPtr.Zero)
                {
                    NativeMethods.CloseHandle(newEvent);
                }
            }

            // Loop trying to acquire the lock. Note that after we 
            // get woken up another thread might have acquired the lock, 
            // and that's why we need a loop.
            while (true)
            {
                if (Interlocked.CompareExchange(ref _value, 1, 0) == 0)
                    break;

                if (NativeMethods.WaitForSingleObject(
                    _event,
                    Timeout.Infinite
                    ) != NativeMethods.WaitObject0)
                {
                    Utils.Break(Utils.MsgFailedToWaitIndefinitely);
                }
            }
        }

        public void Release()
        {
            Interlocked.Exchange(ref _value, 0);

            // Wake up a thread. Note that that thread might 
            // not actually get to acquire the lock because 
            // another thread may have acquired it already.
            if (_event != IntPtr.Zero)
            {
                NativeMethods.SetEvent(_event);
            }
        }

        public bool TryAcquire()
        {
            return Interlocked.CompareExchange(ref _value, 1, 0) == 0;
        }
    }
}
