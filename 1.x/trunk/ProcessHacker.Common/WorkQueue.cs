/*
 * Process Hacker - 
 *   thread pool/work queue
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
using ProcessHacker.Common.Threading;

namespace ProcessHacker.Common
{
    /// <summary>
    /// Manages a work queue which is executed by worker threads.
    /// </summary>
    public sealed class WorkQueue
    {
        /// <summary>
        /// Represents a work item to be executed on a worker thread.
        /// </summary>
        public sealed class WorkItem
        {
            private WorkQueue _owner;
            private string _tag;
            private Delegate _work;
            private object[] _args;
            private bool _enabled = true;
            private FastEvent _completedEvent = new FastEvent(false);
            private object _result;
            private Exception _exception;

            internal WorkItem(WorkQueue owner, Delegate work, object[] args)
                : this(owner, work, args, null)
            { }

            internal WorkItem(WorkQueue owner, Delegate work, object[] args, string tag)
            {
                _owner = owner;
                _work = work;
                _args = args;
                _tag = tag;
            }

            public Delegate Work
            {
                get { return _work; }
            }

            public object[] Arguments
            {
                get { return _args; }
            }

            /// <summary>
            /// The tag associated with the work item.
            /// </summary>
            public string Tag
            {
                get { return _tag; }
            }

            /// <summary>
            /// Whether the work item is to be executed.
            /// </summary>
            internal bool Enabled
            {
                get { return _enabled; }
                set { _enabled = value; }
            }

            /// <summary>
            /// Whether the work item has been completed.
            /// </summary>
            public bool Completed
            {
                get { return _completedEvent.Value; }
            }

            /// <summary>
            /// The value returned by the target method.
            /// </summary>
            public object Result
            {
                get { return _result; }
            }

            /// <summary>
            /// The exception thrown by the work item target, if any.
            /// </summary>
            public Exception Exception
            {
                get { return _exception; }
            }

            /// <summary>
            /// If the work item has not been executed yet, prevents the 
            /// work item from executing. Otherwise, takes no action.
            /// </summary>
            /// <returns>True if the work item has not been executed yet; otherwise false.</returns>
            public bool Abort()
            {
                return _owner.RemoveQueuedWorkItem(this);
            }

            /// <summary>
            /// Waits for the work item to complete and returns the result.
            /// </summary>
            /// <returns>The value returned by the target method.</returns>
            public object GetResult()
            {
                this.WaitOne();
                return _result;
            }

            /// <summary>
            /// Performs the work.
            /// </summary>
            internal void PerformWork()
            {
                if (!_enabled)
                    return;

                try
                {
                    if (_args == null)
                        _result = _work.Method.Invoke(_work.Target, null);
                    else
                        _result = _work.Method.Invoke(_work.Target, _args.Length != 0 ? _args : null);
                }
                catch (Exception ex)
                {
                    _exception = ex;
                }

                _completedEvent.Set();
            }

            /// <summary>
            /// Waits for the work item to be completed.
            /// </summary>
            /// <returns>Always returns true.</returns>
            public bool WaitOne()
            {
                return this.WaitOne(-1);
            }

            /// <summary>
            /// Waits for the work item to be completed.
            /// </summary>
            /// <param name="timeout">The timeout for the wait operation.</param>
            /// <returns>
            /// True if the work item was completed within the timeout 
            /// (or was already completed); otherwise false.
            /// </returns>
            public bool WaitOne(int timeout)
            {
                return _completedEvent.Wait(timeout);
            }
        }

        private static WorkQueue _globalWorkQueue = new WorkQueue();

        /// <summary>
        /// Gets the global work queue instance.
        /// </summary>
        public static WorkQueue GlobalWorkQueue
        {
            get { return _globalWorkQueue; }
        }

        /// <summary>
        /// Queues work for the global work queue.
        /// </summary>
        /// <param name="work">The work to be executed.</param>
        public static WorkItem GlobalQueueWorkItem(Delegate work)
        {
            return _globalWorkQueue.QueueWorkItem(work);
        }

        /// <summary>
        /// Queues work for the global work queue.
        /// </summary>
        /// <param name="work">The work to be executed.</param>
        /// <param name="args">The arguments to pass to the delegate.</param>
        public static WorkItem GlobalQueueWorkItem(Delegate work, params object[] args)
        {
            return _globalWorkQueue.QueueWorkItemTag(work, null, true, args);
        }

        /// <summary>
        /// Queues work for the global work queue.
        /// </summary>
        /// <param name="work">The work to be executed.</param>
        /// <param name="tag">A tag for the work item.</param>
        public static WorkItem GlobalQueueWorkItemTag(Delegate work, string tag)
        {
            return _globalWorkQueue.QueueWorkItemTag(work, tag, true, null);
        }

        /// <summary>
        /// Queues work for the global work queue.
        /// </summary>
        /// <param name="work">The work to be executed.</param>
        /// <param name="tag">A tag for the work item.</param>
        /// <param name="args">The arguments to pass to the delegate.</param>
        public static WorkItem GlobalQueueWorkItemTag(Delegate work, string tag, params object[] args)
        {
            return _globalWorkQueue.QueueWorkItemTag(work, tag, true, args);
        }

        /// <summary>
        /// The work queue. This object is used as a lock.
        /// </summary>
        private Queue<WorkItem> _workQueue = new Queue<WorkItem>();
        /// <summary>
        /// The maximum number of worker threads. If there are less worker threads 
        /// than this limit, they will be created as necessary. If there are more 
        /// worker threads than this limit, they will terminate once they have 
        /// finished processing their current work items.
        /// </summary>
        private int _maxWorkerThreads = 1;
        /// <summary>
        /// The minimum number of worker threads. Worker threads will be created 
        /// as necessary and the number of worker threads will never drop below 
        /// this number.
        /// </summary>
        private int _minWorkerThreads = 0;
        /// <summary>
        /// The pool of worker threads. This object is used as a lock.
        /// </summary>
        private Dictionary<int, Thread> _workerThreads = new Dictionary<int, Thread>();
        /// <summary>
        /// The number of worker threads which are currently running work.
        /// </summary>
        private int _busyCount = 0;
        /// <summary>
        /// A worker will block on the work-arrived event for this amount of time 
        /// before terminating.
        /// </summary>
        private int _noWorkTimeout = 1000;
        /// <summary>
        /// If true, prevents new work items from being queued.
        /// </summary>
        private volatile bool _isJoining = false;

        /// <summary>
        /// Creates a new work queue.
        /// </summary>
        public WorkQueue()
        { }

        /// <summary>
        /// Gets the number of worker threads that are currently busy.
        /// </summary>
        public int BusyCount
        {
            get { return _busyCount; }
        }

        /// <summary>
        /// Gets or sets the maximum number of worker threads.
        /// </summary>
        public int MaxWorkerThreads
        {
            get { return _maxWorkerThreads; }
            set { _maxWorkerThreads = value; }
        }

        /// <summary>
        /// Gets or sets the minimum number of worker threads.
        /// </summary>
        public int MinWorkerThreads
        {
            get { return _minWorkerThreads; }
            set { _minWorkerThreads = value; }
        }

        /// <summary>
        /// Gets or sets the time, in milliseconds, after which a 
        /// worker thread with no work will terminate. Specify 0 so that
        /// worker threads will terminate immediately, or specify -1 so that
        /// worker threads will wait indefinitely for work.
        /// </summary>
        public int NoWorkTimeout
        {
            get { return _noWorkTimeout; }
            set { _noWorkTimeout = value; }
        }

        /// <summary>
        /// Gets the number of queued work items.
        /// </summary>
        public int QueuedCount
        {
            get { return _workQueue.Count; }
        }

        /// <summary>
        /// Gets the number of worker threads that are alive.
        /// </summary>
        public int WorkerCount
        {
            get { return _workerThreads.Count; }
        }

        /// <summary>
        /// Creates worker threads if necessary to satisfy the 
        /// worker thread minimum.
        /// </summary>
        public void CreateMinimumWorkerThreads()
        {
            if (_workerThreads.Count < _minWorkerThreads)
            {
                lock (_workerThreads)
                {
                    // Create worker threads until we have enough.
                    while (_workerThreads.Count < _minWorkerThreads)
                        this.CreateWorkerThread();
                }
            }
        }

        /// <summary>
        /// Creates a worker thread.
        /// </summary>
        private void CreateWorkerThread()
        {
            Thread workThread = new Thread(this.WorkerThreadStart, Utils.SixteenthStackSize);
            workThread.IsBackground = true;
            workThread.Priority = ThreadPriority.Lowest;
            workThread.SetApartmentState(ApartmentState.STA);
            _workerThreads.Add(workThread.ManagedThreadId, workThread);
            workThread.Start();
        }

        /// <summary>
        /// Destroys the current worker thread.
        /// </summary>
        private void DestroyWorkerThread()
        {
            _workerThreads.Remove(Thread.CurrentThread.ManagedThreadId);
        }

        /// <summary>
        /// Gets the work items in the queue.
        /// </summary>
        /// <returns>An array of WorkItem objects.</returns>
        public WorkItem[] GetQueuedWorkItems()
        {
            lock (_workQueue)
                return _workQueue.ToArray();
        }

        /// <summary>
        /// Waits for all work items to complete and prevents new work items from being queued.
        /// </summary>
        public void JoinAll()
        {
            _isJoining = true;

            // Check for work items.
            while (_workQueue.Count > 0)
            {
                WorkItem workItem = null;

                // Lock and re-check.
                lock (_workQueue)
                {
                    if (_workQueue.Count > 0)
                        workItem = _workQueue.Peek();
                    else
                        continue;
                }

                // Wait for this work item to finish.
                workItem.WaitOne();
            }
        }

        /// <summary>
        /// Removes the work item from the work queue.
        /// </summary>
        /// <param name="workItem">The work item to remove</param>
        /// <returns>If the work item was in the work queue, true. Otherwise, false.</returns>
        public bool RemoveQueuedWorkItem(WorkItem workItem)
        {
            // Lock the work queue to prevent data corruption.
            lock (_workQueue)
            {
                // Check if the work queue (still) contains the work item.
                if (_workQueue.Contains(workItem))
                {
                    // The work item is in the queue. Prevent it from executing.
                    workItem.Enabled = false;
                    return true;
                }
                else
                {
                    // The work item is no longer in the queue.
                    return false;
                }
            }
        }

        /// <summary>
        /// Allows new work items to be queued.
        /// </summary>
        public void ResetJoin()
        {
            _isJoining = false;
        }

        /// <summary>
        /// Queues work for the worker thread(s).
        /// </summary>
        /// <param name="work">The work to be performed.</param>
        public WorkItem QueueWorkItem(Delegate work)
        {
            return this.QueueWorkItemTag(work, null, true, null);
        }

        /// <summary>
        /// Queues work for the worker thread(s).
        /// </summary>
        /// <param name="work">The work to be performed.</param>
        /// <param name="args">The arguments to pass to the delegate.</param>
        public WorkItem QueueWorkItem(Delegate work, params object[] args)
        {
            return this.QueueWorkItemTag(work, null, true, args);
        }

        /// <summary>
        /// Queues work for the worker thread(s).
        /// </summary>
        /// <param name="work">The work to be performed.</param>
        /// <param name="tag">A tag for the work item.</param>
        public WorkItem QueueWorkItemTag(Delegate work, string tag)
        {
            return this.QueueWorkItemTag(work, tag, true, null);
        }

        /// <summary>
        /// Queues work for the worker thread(s).
        /// </summary>
        /// <param name="work">The work to be performed.</param>
        /// <param name="tag">A tag for the work item.</param>
        /// <param name="args">The arguments to pass to the delegate.</param>
        public WorkItem QueueWorkItemTag(Delegate work, string tag, params object[] args)
        {
            return this.QueueWorkItemTag(work, tag, true, args);
        }

        /// <summary>
        /// Queues work for the worker thread(s).
        /// </summary>
        /// <param name="work">The work to be performed.</param>
        /// <param name="tag">A tag for the work item.</param>
        /// <param name="isArray">Ignored.</param>
        /// <param name="args">The arguments to pass to the delegate.</param>
        public WorkItem QueueWorkItemTag(Delegate work, string tag, bool isArray, object[] args)
        {
            WorkItem workItem;

            // Can't queue any work items if joining.
            if (_isJoining)
                return null;

            lock (_workQueue)
            {
                _workQueue.Enqueue(workItem = new WorkItem(this, work, args, tag));
                Monitor.Pulse(_workQueue);
            }

            // Check if all worker threads are currently busy.
            if (Thread.VolatileRead(ref _busyCount) == _workerThreads.Count)
            {
                // Check if we still have available worker threads
                if (_workerThreads.Count < _maxWorkerThreads)
                {
                    // We do, so we must lock and re-check.
                    lock (_workerThreads)
                    {
                        if (_workerThreads.Count < _maxWorkerThreads)
                        {
                            this.CreateWorkerThread();
                        }
                    }
                }
            }

            return workItem;
        }

        /// <summary>
        /// The entry point for all worker threads.
        /// </summary>
        private void WorkerThreadStart()
        {
            while (true)
            {
                // Check if we have more worker threads than the limit.
                if (_workerThreads.Count > _maxWorkerThreads)
                {
                    // Lock and re-check.
                    lock (_workerThreads)
                    {
                        // Check the minimum as well.
                        if (_workerThreads.Count > _maxWorkerThreads && 
                            _workerThreads.Count > _minWorkerThreads)
                        {
                            // We have an excess amount of worker threads.
                            this.DestroyWorkerThread();
                            return;
                        }
                    }
                }

                // Check for work.
                if (_workQueue.Count > 0)
                {
                    WorkItem workItem = null;

                    // There is work, but we must lock and re-check.
                    lock (_workQueue)
                    {
                        if (_workQueue.Count > 0)
                            workItem = _workQueue.Dequeue();
                        else
                            continue;
                    }

                    Interlocked.Increment(ref _busyCount);
                    workItem.PerformWork();
                    Interlocked.Decrement(ref _busyCount);
                }
                else
                {
                    // No work available. Wait for work.
                    bool workArrived = false;

                    lock (_workQueue)
                        workArrived = Monitor.Wait(_workQueue, _noWorkTimeout);

                    if (workArrived)
                    {
                        // Work arrived. Go back so we can perform it.
                        continue;
                    }
                    else
                    {
                        // No work arrived during the timeout period. Delete the thread.
                        lock (_workerThreads)
                        {
                            // Check the minimum.
                            if (_workerThreads.Count > _minWorkerThreads)
                            {
                                this.DestroyWorkerThread();
                                return;
                            }
                        }
                    }
                }
            }
        }
    }
}
