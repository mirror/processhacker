/*
 * Process Hacker - 
 *   wait manager
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

using System.Collections.Generic;
using System.Threading;
using ProcessHacker.Common.Objects;  
using ProcessHacker.Common.Threading;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;

namespace ProcessHacker.Native.Threading
{
    public delegate void ObjectSignaledDelegate(ISynchronizable obj);

    /// <summary>
    /// Provides methods for waiting on dispatcher objects.
    /// </summary>
    public sealed class Waiter : BaseObject
    {
        private class WaiterThread : BaseObject
        {
            public event ObjectSignaledDelegate ObjectSignaled;

            private Waiter _owner;
            private bool _terminating = false;
            private Thread _thread;
            private FastEvent _threadInitializedEvent = new FastEvent(false);
            private ThreadHandle _threadHandle;
            private List<ISynchronizable> _waitObjects = new List<ISynchronizable>();

            public WaiterThread(Waiter owner)
            {
                _owner = owner;

                // Create the waiter thread.
                _thread = new Thread(this.WaiterThreadStart, ProcessHacker.Common.Utils.SixteenthStackSize);
                _thread.IsBackground = true;
                _thread.SetApartmentState(ApartmentState.STA);
                _thread.Start();

                // Wait for the thread to initialize.
                _threadInitializedEvent.Wait();
            }

            protected override void DisposeObject(bool disposing)
            {
                lock (_thread)
                {
                    if (_threadInitializedEvent.Value)
                    {
                        // Terminate the waiter thread.
                        this.Terminate();
                    }
                }

                if (_threadHandle != null)
                {
                    // Close the thread handle.
                    _threadHandle.Dispose();
                }

                // Avoid hanging on to objects.
                lock (_waitObjects)
                    _waitObjects.Clear();
            }

            public int Count
            {
                get
                {
                    lock (_waitObjects)
                        return _waitObjects.Count;
                }
            }

            public ISynchronizable[] Objects
            {
                get
                {
                    lock (_waitObjects)
                        return _waitObjects.ToArray();
                }
            }

            public bool Add(ISynchronizable obj)
            {
                lock (_waitObjects)
                {
                    // Check if we already have the maximum number of wait objects.
                    if (_waitObjects.Count >= Win32.MaximumWaitObjects)
                        return false;

                    _waitObjects.Add(obj);
                    this.NotifyChange();
                    return true;
                }
            }

            public void NotifyChange()
            {
                _threadHandle.Alert();
            }

            private void OnObjectSignaled(ISynchronizable obj)
            {
                if (this.ObjectSignaled != null)
                    this.ObjectSignaled(obj);
            }

            public bool Remove(ISynchronizable obj)
            {
                lock (_waitObjects)
                {
                    if (!_waitObjects.Contains(obj))
                        return false;

                    _waitObjects.Remove(obj);
                    this.NotifyChange();
                    return true;
                }
            }

            public void Terminate()
            {
                _terminating = true;
                this.NotifyChange();
            }

            private void WaiterThreadStart()
            {
                ISynchronizable[] waitObjects = null;

                // Open a handle to the current thread.
                _threadHandle = ThreadHandle.OpenCurrent(ThreadAccess.Alert);

                // Signal that the thread has been initialized.
                _threadInitializedEvent.Set();

                while (!_terminating)
                {
                    bool doWait;

                    lock (_waitObjects)
                    {
                        // Check if we have any objects to wait for. If we do, use WaitAny. 
                        // Otherwise, wait forever (alertably).
                        if (_waitObjects.Count > 0)
                        {
                            waitObjects = _waitObjects.ToArray();
                            doWait = true;
                        }
                        else
                        {
                            doWait = false;
                        }
                    }

                    NtStatus waitStatus;

                    if (doWait)
                    {
                        try
                        {
                            // Wait for the objects, (almost) forever.
                            waitStatus = NativeHandle.WaitAny(waitObjects, true, long.MinValue, false);
                        }
                        catch (WindowsException)
                        {
                            // We probably got Access denied on one of the objects. 
                            // We can't do anything about this, so just wait forever.
                            waitStatus = ThreadHandle.Sleep(true, long.MinValue, false);
                        }
                    }
                    else
                    {
                        // Wait forever.
                        waitStatus = ThreadHandle.Sleep(true, long.MinValue, false);
                    }

                    if (waitStatus == NtStatus.Alerted)
                    {
                        // The wait was changed. Go back to refresh the wait objects array.
                        // The thread is also alerted to notify that the thread should terminate.
                        continue;
                    }
                    else if (waitStatus >= NtStatus.Wait0 && waitStatus <= NtStatus.Wait63)
                    {
                        // One of the objects was signaled.
                        ISynchronizable signaledObject = waitObjects[(int)(waitStatus - NtStatus.Wait0)];

                        // Remove the object now that it is signaled.
                        lock (_waitObjects)
                        {
                            // Just in case someone already removed the object.
                            if (_waitObjects.Contains(signaledObject))
                                _waitObjects.Remove(signaledObject);
                        }

                        // Call the object-signaled event.
                        OnObjectSignaled(signaledObject);

                        // Balance the threads (which may involve terminating the current one).
                        _owner.BalanceWaiterThreads();
                    }
                }
            }
        }

        /// <summary>
        /// Raised when an object is signaled.
        /// </summary>
        public event ObjectSignaledDelegate ObjectSignaled;

        private List<WaiterThread> _waiterThreads = new List<WaiterThread>();
        private List<ISynchronizable> _waitObjects = new List<ISynchronizable>();

        /// <summary>
        /// Creates a waiter.
        /// </summary>
        public Waiter()
        {

        }

        protected override void DisposeObject(bool disposing)
        {
            // Tell the waiter threads to terminate.
            foreach (var waiterThread in _waiterThreads)
                waiterThread.Terminate();
            _waiterThreads.Clear();
        }

        public int Count
        {
            get
            {
                lock (_waitObjects)
                    return _waitObjects.Count;
            }
        }

        public ISynchronizable[] Objects
        {
            get
            {
                lock (_waitObjects)
                    return _waitObjects.ToArray();
            }
        }

        /// <summary>
        /// Adds an object for the waiter to wait on.
        /// </summary>
        /// <param name="obj">The object to wait for.</param>
        public void Add(ISynchronizable obj)
        {
            lock (_waitObjects)
                _waitObjects.Add(obj);

            foreach (var waiterThread in this.GetWaiterThreads())
            {
                if (waiterThread.Add(obj))
                    return;
            }

            // We couldn't add the object to any existing waiter thread. 
            // Create a new waiter thread and add the object to that.
            this.CreateWaiterThread(obj);
        }  

        internal void BalanceWaiterThreads()
        {
            lock (_waitObjects)
            {
                // Eliminate waiter threads with no objects.
                foreach (var waiterThread in this.GetWaiterThreads())
                {
                    if (waiterThread.Count == 0)
                        this.DeleteWaiterThread(waiterThread);
                }
            }
        }

        private WaiterThread CreateWaiterThread()
        {
            return this.CreateWaiterThread(null);
        }

        private WaiterThread CreateWaiterThread(ISynchronizable obj)
        {
            WaiterThread waiterThread = new WaiterThread(this);

            waiterThread.ObjectSignaled += this.OnObjectSignaled;

            if (obj != null)
                waiterThread.Add(obj);

            lock (_waiterThreads)
                _waiterThreads.Add(waiterThread);

            return waiterThread;
        }

        private void DeleteWaiterThread(WaiterThread waiterThread)
        {
            lock (_waiterThreads)
            {
                _waiterThreads.Remove(waiterThread);
                waiterThread.ObjectSignaled -= this.OnObjectSignaled;
                waiterThread.Dispose();
            }
        }

        private WaiterThread[] GetWaiterThreads()
        {
            lock (_waiterThreads)
                return _waiterThreads.ToArray();
        }

        private void OnObjectSignaled(ISynchronizable obj)
        {
            if (ObjectSignaled != null)
                ObjectSignaled(obj);
        }

        /// <summary>
        /// Removes an object the waiter is waiting on.
        /// </summary>
        /// <param name="obj">An object which is currently being waited on.</param>
        /// <returns>Whether the object was successfully removed.</returns>
        public bool Remove(ISynchronizable obj)
        {
            foreach (var waiterThread in this.GetWaiterThreads())
            {
                if (waiterThread.Remove(obj))
                {
                    lock (_waitObjects)
                        _waitObjects.Remove(obj);

                    this.BalanceWaiterThreads();
                    return true;
                }
            }

            // We couldn't remove the object.
            return false;
        }
    }
}
