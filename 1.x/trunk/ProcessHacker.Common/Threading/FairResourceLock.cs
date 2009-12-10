using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ProcessHacker.Common.Threading
{
    public unsafe sealed class FairResourceLock : IDisposable
    {
        // Lock owned
        private const int LockOwned = 0x1;
        // Waiters present
        private const int LockWaiters = 0x2;

        // Shared owners count
        private const int LockSharedOwnersShift = 2;
        private const int LockSharedOwnersMask = 0x3fffffff;
        private const int LockSharedOwnersIncrement = 0x4;

        private const int WaiterExclusive = 0x1;
        private const int WaiterSpinning = 0x2;

        private unsafe struct WaitBlock
        {
            public WaitBlock* Flink;
            public WaitBlock* Blink;
            public int Flags;
        }

        // The number of times to spin before going to sleep.
        private static readonly int SpinCount = NativeMethods.SpinCount;

        private static void InsertHeadList(WaitBlock* listHead, WaitBlock* entry)
        {
            WaitBlock* flink;

            flink = listHead->Flink;
            entry->Flink = flink;
            entry->Blink = listHead;
            flink->Blink = entry;
            listHead->Flink = entry;
        }

        private static void InsertTailList(WaitBlock* listHead, WaitBlock* entry)
        {
            WaitBlock* blink;

            blink = listHead->Blink;
            entry->Flink = listHead;
            entry->Blink = blink;
            blink->Flink = entry;
            listHead->Blink = entry;
        }

        private static WaitBlock* RemoveHeadList(WaitBlock* listHead)
        {
            WaitBlock* flink;
            WaitBlock* entry;

            entry = listHead->Flink;
            flink = entry->Flink;
            listHead->Flink = flink;
            flink->Blink = listHead;

            return entry;
        }

        private int _value;
        private SpinLock _lock;
        private IntPtr _wakeEvent;

        private WaitBlock* _waitersListHead;
        private object __waitersListHead;
        private System.Runtime.InteropServices.GCHandle __waitersListHeadHandle;

        private WaitBlock* _firstSharedWaiter;

        public FairResourceLock()
        {
            _value = 0;
            _lock = new SpinLock();

            __waitersListHead = new WaitBlock();
            __waitersListHeadHandle =
                System.Runtime.InteropServices.GCHandle.Alloc(
                __waitersListHead,
                System.Runtime.InteropServices.GCHandleType.Pinned
                );
            _waitersListHead = (WaitBlock*)__waitersListHeadHandle.AddrOfPinnedObject();
            _waitersListHead->Flink = _waitersListHead;
            _waitersListHead->Blink = _waitersListHead;
            _firstSharedWaiter = _waitersListHead;

            if (NativeMethods.NtCreateKeyedEvent(
                out _wakeEvent,
                0x3,
                IntPtr.Zero,
                0
                ) < 0)
                throw new Exception("Failed to create the wake event.");
        }

        ~FairResourceLock()
        {
            this.Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (_waitersListHead != null)
            {
                __waitersListHeadHandle.Free();
                _waitersListHead = null;
                __waitersListHead = null;
            }

            NativeMethods.CloseHandle(_wakeEvent);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void AcquireExclusive()
        {
            int value;
            int i = 0;

            while (true)
            {
                value = _value;

                // Try to obtain the lock.
                if ((value & LockOwned) == 0)
                {
                    if (Interlocked.CompareExchange(
                        ref _value,
                        value | LockOwned,
                        value
                        ) == value)
                        break;
                }
                else if (i >= SpinCount / 10)
                {
                    // We need to wait.
                    WaitBlock waitBlock;

                    waitBlock.Flags = WaiterExclusive | WaiterSpinning;

                    // Obtain the waiters list lock.
                    _lock.Acquire();

                    try
                    {
                        // Try to set the waiters bit.
                        if (Interlocked.CompareExchange(
                            ref _value,
                            value | LockWaiters,
                            value
                            ) != value)
                        {
                            // Unfortunately we have to go back. This is 
                            // very wasteful since the waiters list lock 
                            // must be released again, but must happen since 
                            // the lock may have been released.
                            continue;
                        }

                        // Put our wait block behind other exclusive waiters but 
                        // in front of all shared waiters.
                        InsertTailList(_firstSharedWaiter, &waitBlock);
                    }
                    finally
                    {
                        _lock.Release();
                    }

                    this.Block(&waitBlock);

                    // Go back and try again.
                    continue;
                }

                i++;
            }
        }

        public void AcquireShared()
        {
            int value;
            int i = 0;

            while (true)
            {
                value = _value;

                // Try to obtain the lock.
                // Note that we don't acquire if there are waiters and 
                // the lock is already owned in shared mode, in order to 
                // give exclusive acquires precedence.
                if (
                    (value & LockOwned) == 0 ||
                    ((value & LockWaiters) == 0 && ((value >> LockSharedOwnersShift) & LockSharedOwnersMask) != 0)
                    )
                {
                    if ((value & LockOwned) == 0)
                    {
                        if (Interlocked.CompareExchange(
                            ref _value,
                            value + LockOwned + LockSharedOwnersIncrement,
                            value
                            ) == value)
                            break;
                    }
                    else
                    {
                        if (Interlocked.CompareExchange(
                            ref _value,
                            value + LockSharedOwnersIncrement,
                            value
                            ) == value)
                            break;
                    }
                }
                else if (i >= SpinCount / 10)
                {
                    // We need to wait.
                    WaitBlock waitBlock;

                    waitBlock.Flags = WaiterSpinning;

                    // Obtain the waiters list lock.
                    _lock.Acquire();

                    try
                    {
                        // Try to set the waiters bit.
                        if (Interlocked.CompareExchange(
                            ref _value,
                            value | LockWaiters,
                            value
                            ) != value)
                        {
                            // Unfortunately we have to go back. This is 
                            // very wasteful since the waiters list lock 
                            // must be released again, but must happen since 
                            // the lock may have been released.
                            continue;
                        }

                        // Put our wait block behind other waiters.
                        InsertTailList(_waitersListHead, &waitBlock);

                        // Set the first shared waiter pointer.
                        if (
                            waitBlock.Blink == _waitersListHead ||
                            (waitBlock.Blink->Flags & WaiterExclusive) != 0
                            )
                        {
                            _firstSharedWaiter = &waitBlock;
                        }
                    }
                    finally
                    {
                        _lock.Release();
                    }

                    this.Block(&waitBlock);

                    // Go back and try again.
                    continue;
                }

                i++;
            }
        }

        private void Block(WaitBlock* waitBlock)
        {
            int flags;

            // Spin for a while.
            for (int j = 0; j < SpinCount; j++)
            {
                if ((Thread.VolatileRead(ref waitBlock->Flags) & WaiterSpinning) == 0)
                    break;
            }

            // Clear the spinning flag.
            do
            {
                flags = waitBlock->Flags;
            } while (Interlocked.CompareExchange(
                ref waitBlock->Flags,
                flags & ~WaiterSpinning,
                flags
                ) != flags);

            // Go to sleep if necessary.
            if ((flags & WaiterSpinning) != 0)
            {
                if (NativeMethods.NtWaitForKeyedEvent(
                    _wakeEvent,
                    new IntPtr(waitBlock),
                    false,
                    IntPtr.Zero
                    ) != 0)
                    throw new Exception(Utils.MsgFailedToWaitIndefinitely);
            }
        }

        public void ReleaseExclusive()
        {
            int value;

            while (true)
            {
                value = _value;

                if (Interlocked.CompareExchange(
                    ref _value,
                    value - LockOwned,
                    value
                    ) == value)
                {
                    if ((value & LockWaiters) != 0)
                        this.Wake();

                    break;
                }
            }
        }

        public void ReleaseShared()
        {
            int value;
            int newValue;

            while (true)
            {
                value = _value;

                if (((value >> LockSharedOwnersShift) & LockSharedOwnersMask) != 1)
                    newValue = value - LockSharedOwnersIncrement;
                else
                    newValue = value - LockOwned - LockSharedOwnersIncrement;

                if (Interlocked.CompareExchange(
                    ref _value,
                    newValue,
                    value
                    ) == value)
                {
                    if ((value & LockWaiters) != 0)
                        this.Wake();

                    break;
                }
            }
        }

        private void Unblock(WaitBlock* waitBlock)
        {
            int flags;

            // Clear the spinning flag.
            do
            {
                flags = waitBlock->Flags;
            } while (Interlocked.CompareExchange(
                ref waitBlock->Flags,
                flags & ~WaiterSpinning,
                flags
                ) != flags);

            if ((flags & WaiterSpinning) == 0)
            {
                NativeMethods.NtReleaseKeyedEvent(
                    _wakeEvent,
                    new IntPtr(waitBlock),
                    false,
                    IntPtr.Zero
                    );
            }
        }

        private void Wake()
        {
            WaitBlock wakeList = new WaitBlock();
            WaitBlock* wb = null;

            wakeList.Flink = &wakeList;
            wakeList.Blink = &wakeList;

            _lock.Acquire();

            try
            {
                bool first = true;

                while (true)
                {
                    wb = _waitersListHead->Flink;

                    if (wb == _waitersListHead)
                    {
                        int value;

                        // No more waiters. Clear the waiters bit.
                        do
                        {
                            value = _value;
                        } while (Interlocked.CompareExchange(
                            ref _value,
                            value & ~LockWaiters,
                            value
                            ) != value);

                        break;
                    }

                    // If this is an exclusive waiter, don't wake 
                    // anyone else.
                    if (first && (wb->Flags & WaiterExclusive) != 0)
                    {
                        wb = RemoveHeadList(_waitersListHead);
                        break;
                    }

                    // If this is not the first waiter we have looked at 
                    // and it is an exclusive waiter, don't wake anyone 
                    // else - it means we have reached the end of a chain 
                    // of shared waiters.
                    if (!first && (wb->Flags & WaiterExclusive) != 0)
                        break;

                    // Remove the waiter and add it to the wake list.
                    wb = RemoveHeadList(_waitersListHead);
                    InsertTailList(&wakeList, wb);

                    first = false;
                }

                if ((wb->Flags & WaiterExclusive) == 0)
                {
                    // If we removed shared waiters, we removed all of them. 
                    // Reset the first shared waiter pointer.
                    _firstSharedWaiter = _waitersListHead;
                }
            }
            finally
            {
                _lock.Release();
            }

            // If we removed one exclusive waiter, unblock it.
            if ((wb->Flags & WaiterExclusive) != 0)
            {
                this.Unblock(wb);
                return;
            }

            // Carefully traverse the wake list and wake each waiter.
            wb = wakeList.Flink;

            while (true)
            {
                WaitBlock* flink;

                if (wb == &wakeList)
                    break;

                flink = wb->Flink;
                this.Unblock(wb);
                wb = flink;
            }
        }
    }
}
