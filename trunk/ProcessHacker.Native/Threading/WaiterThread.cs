/*
 * Process Hacker - 
 *   waiter thread
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
using System.Text;
using System.Threading;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;   
using ProcessHacker.Native.Security;

namespace ProcessHacker.Native.Threading
{
    public delegate void ObjectSignaledDelegate(ISynchronizable obj);

    public class WaiterThread : IDisposable
    {
        private enum WaiterThreadMessageType
        {
            AddObject,
            RemoveObject
        }

        private class WaiterThreadMessage
        {
            private WaiterThreadMessageType _type;
            private object _param;

            public WaiterThreadMessage(WaiterThreadMessageType type)
                : this(type, null)
            { }

            public WaiterThreadMessage(WaiterThreadMessageType type, object param)
            {
                _type = type;
                _param = param;
            }

            public WaiterThreadMessageType Type { get { return _type; } }
            public object Param { get { return _param; } }
        }

        /// <summary>
        /// Raised when an object is signaled.
        /// </summary>
        public event ObjectSignaledDelegate ObjectSignaled;

        private bool _disposed = false;
        private bool _terminating = false;
        private object _disposeLock = new object();

        private Thread _waiterThread;
        private List<ISynchronizable> _waitObjects = new List<ISynchronizable>();

        private Queue<WaiterThreadMessage> _waitMessageQueue = new Queue<WaiterThreadMessage>(); 
        private Event _waitMessageEvent = new Event(true, false);

        /// <summary>
        /// Creates a waiter thread.
        /// </summary>
        public WaiterThread()
        {
            _waiterThread = new Thread(this.WaiterThreadStart);
            _waiterThread.IsBackground = true;
            _waiterThread.SetApartmentState(ApartmentState.STA);
            _waiterThread.Start();
        }

        ~WaiterThread()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Releases resources used by the waiter thread.
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
                    // Tell the waiter thread to terminate.
                    _terminating = true;
                    _waitMessageEvent.Set();

                    _disposed = true;
                }
            }
            finally
            {
                Monitor.Exit(_disposeLock);
            }
        }

        /// <summary>
        /// Adds an object for the waiter thread to wait on.
        /// </summary>
        /// <param name="obj">The object to wait for.</param>
        public void Add(ISynchronizable obj)
        {
            lock (_waitMessageQueue)
            {
                if (_waitObjects.Count >= Win32.MaximumWaitObjects - 1)
                    throw new TooManyWaitObjectsException();

                this.SendWaiterThreadMessage(new WaiterThreadMessage(WaiterThreadMessageType.AddObject, obj));
            }
        }

        private void OnObjectSignaled(ISynchronizable obj)
        {
            if (ObjectSignaled != null)
                ObjectSignaled(obj);
        }

        /// <summary>
        /// Removes an object the waiter thread is waiting on.
        /// </summary>
        /// <param name="obj">An object which is currently being waited on.</param>
        public void Remove(ISynchronizable obj)
        {
            this.SendWaiterThreadMessage(new WaiterThreadMessage(WaiterThreadMessageType.RemoveObject, obj));
        }

        private void SendWaiterThreadMessage(WaiterThreadMessage message)
        {
            lock (_waitMessageQueue)
            {
                _waitMessageQueue.Enqueue(message);
                _waitMessageEvent.Set();
            }
        }

        private void WaiterThreadStart()
        {
            ISynchronizable[] waitObjects;

            while (!_terminating)
            {
                waitObjects = new ISynchronizable[_waitObjects.Count + 1];
                waitObjects[0] = _waitMessageEvent.Handle;
                Array.Copy(_waitObjects.ToArray(), 0, waitObjects, 1, _waitObjects.Count);

                NtStatus waitStatus = NativeHandle.WaitAny(waitObjects);

                if (waitStatus == NtStatus.Wait0)
                {
                    // We have a message in the message queue (or we are terminating).
                    WaiterThreadMessage message;

                    // Lock and retrieve the message.
                    lock (_waitMessageQueue)
                    {
                        if (_waitMessageQueue.Count > 0)
                            message = _waitMessageQueue.Dequeue();
                        else
                            continue;
                    }

                    switch (message.Type)
                    {
                        case WaiterThreadMessageType.AddObject:
                            {
                                _waitObjects.Add((ISynchronizable)message.Param);
                            }
                            break;
                        case WaiterThreadMessageType.RemoveObject:
                            {
                                _waitObjects.Remove((ISynchronizable)message.Param);
                            }
                            break;
                    }
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

    public class TooManyWaitObjectsException : Exception
    {
        public override string Message
        {
            get
            {
                return "An attempt was made to add too many objects to the wait list.";
            }
        }
    }
}
