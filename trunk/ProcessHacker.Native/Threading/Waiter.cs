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

using System;
using System.Collections.Generic;
using System.Threading;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;

namespace ProcessHacker.Native.Threading
{
    public delegate void ObjectSignaledDelegate(ISynchronizable obj);

    public class Waiter : IDisposable
    {
        private class WaiterThread : IDisposable
        {
            public event ObjectSignaledDelegate ObjectSignaled;

            private bool _disposed = false;
            private object _disposeLock = new object();
            private bool _terminating = false;
            private Thread _thread;
            private List<ISynchronizable> _waitObjects = new List<ISynchronizable>();
            private Event _waitChangedEvent = new Event(true, false);

            public WaiterThread()
            {
                _thread = new Thread(this.WaiterThreadStart);
                _thread.IsBackground = true;
                _thread.SetApartmentState(ApartmentState.STA);
                _thread.Start();
            }

            ~WaiterThread()
            {
                this.Dispose(false);
            }

            public void Dispose()
            {
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }

            private void Dispose(bool disposing)
            {
                if (disposing)
                    Monitor.Enter(_disposeLock);

                try
                {
                    if (!_disposed)
                    {
                        this.Terminate();
                        _waitChangedEvent.Dispose();
                        _disposed = true;
                    }
                }
                finally
                {
                    Monitor.Exit(_disposeLock);
                }
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
                    if (_waitObjects.Count >= Win32.MaximumWaitObjects - 1)
                        return false;

                    _waitObjects.Add(obj);
                    this.NotifyChange();
                    return true;
                }
            }

            public void NotifyChange()
            {
                _waitChangedEvent.Set();
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
                ISynchronizable[] waitObjects;

                while (!_terminating)
                {
                    lock (_waitObjects)
                    {
                        waitObjects = new ISynchronizable[_waitObjects.Count + 1];
                        waitObjects[0] = _waitChangedEvent.Handle;
                        Array.Copy(_waitObjects.ToArray(), 0, waitObjects, 1, _waitObjects.Count);
                    }

                    NtStatus waitStatus = NativeHandle.WaitAny(waitObjects);

                    if (waitStatus == NtStatus.Wait0)
                    {
                        // The wait was changed. Go back to refresh the wait objects array.
                        // The event is also signaled to notify that the thread should terminate.
                        continue;
                    }
                    else if (waitStatus > NtStatus.Wait0 && waitStatus <= NtStatus.Wait63)
                    {
                        // One of the objects was signaled.
                        ISynchronizable signaledObject = waitObjects[(int)(waitStatus - NtStatus.Wait0)];
                        // Remove the object now that it is signaled.
                        _waitObjects.Remove(signaledObject);
                        // Call the object-signaled event.
                        OnObjectSignaled(signaledObject);
                    }
                }
            }
        }

        /// <summary>
        /// Raised when an object is signaled.
        /// </summary>
        public event ObjectSignaledDelegate ObjectSignaled;

        private bool _disposed = false;
        private object _disposeLock = new object();
        private List<WaiterThread> _waiterThreads = new List<WaiterThread>();
        private List<ISynchronizable> _waitObjects = new List<ISynchronizable>();

        /// <summary>
        /// Creates a waiter.
        /// </summary>
        public Waiter()
        {

        }

        ~Waiter()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Releases resources used by the waiter.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
                Monitor.Enter(_disposeLock);

            try
            {
                if (!_disposed)
                {
                    // Tell the waiter threads to terminate.
                    foreach (var waiterThread in _waiterThreads)
                        waiterThread.Terminate();
                    _waiterThreads.Clear();

                    _disposed = true;
                }
            }
            finally
            {
                Monitor.Exit(_disposeLock);
            }
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

        private void BalanceWaiterThreads()
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
            WaiterThread waiterThread = new WaiterThread();

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
                waiterThread.Terminate();
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
        public void Remove(ISynchronizable obj)
        {
            foreach (var waiterThread in this.GetWaiterThreads())
            {
                if (waiterThread.Remove(obj))
                {
                    lock (_waitObjects)
                        _waitObjects.Remove(obj);

                    this.BalanceWaiterThreads();
                    return;
                }
            }

            // We couldn't remove the object.
            throw new ArgumentException("The object is not being waited on.");
        }
    }
}
