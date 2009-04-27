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
using System.Text;
using System.Threading;

namespace ProcessHacker
{
    public delegate void Action();
    public delegate void Action<T>(T a1);
    public delegate void Action<T, U>(T a1, U a2);
    public delegate void Action<T, U, V>(T a1, U a2, V a3);
    public delegate void Action<T, U, V, W>(T a1, U a2, V a3, W a4);
    public delegate void Action<T, U, V, W, X>(T a1, U a2, V a3, W a4, X a5);
    public delegate void Action<T, U, V, W, X, Y>(T a1, U a2, V a3, W a4, X a5, Y a6);
    public delegate void Action<T, U, V, W, X, Y, Z>(T a1, U a2, V a3, W a4, X a5, Y a6, Z a7);

    /// <summary>
    /// Manages a work queue which is executed by worker threads.
    /// </summary>
    public class WorkQueue
    {
        private class WorkItem
        {              
            public Delegate _work;
            public object[] _args;

            public WorkItem(Delegate work, object[] args)
            {
                _work = work;
                _args = args;
            }

            public Delegate Work
            {
                get { return _work; }
            }

            public object[] Parameter
            {
                get { return _args; }
            }

            public void PerformWork()
            {
                if (_args == null)
                    _work.Method.Invoke(_work.Target, null);
                else
                    _work.Method.Invoke(_work.Target, _args.Length != 0 ? _args : null);
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
        public static void GlobalQueueWorkItem(Delegate work)
        {
            _globalWorkQueue.QueueWorkItem(work);
        }

        /// <summary>
        /// Queues work for the global work queue.
        /// </summary>
        /// <param name="work">The work to be executed.</param>
        /// <param name="args">The arguments to pass to the delegate.</param>
        public static void GlobalQueueWorkItem(Delegate work, params object[] args)
        {
            _globalWorkQueue.QueueWorkItem(work, true, args);
        }

        private Queue<WorkItem> _workQueue = new Queue<WorkItem>();
        private int _maxWorkerThreads = 1;
        private Dictionary<int, Thread> _workerThreads = new Dictionary<int, Thread>();
        private int _busyCount = 0;
        private AutoResetEvent _workArrivedEvent = new AutoResetEvent(false);

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
        /// Gets or sets the worker thread limit.
        /// </summary>
        public int MaxWorkerThreads
        {
            get { return _maxWorkerThreads; }
            set { _maxWorkerThreads = value; }
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
        /// Creates a worker thread.
        /// </summary>
        private void CreateWorkerThread()
        {
            Thread workThread = new Thread(this.WorkerThreadStart);
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
        /// Queues work for the worker thread(s).
        /// </summary>
        /// <param name="work">The work to be performed.</param>
        public void QueueWorkItem(Delegate work)
        {
            this.QueueWorkItem(work, true, null);
        }

        /// <summary>
        /// Queues work for the worker thread(s).
        /// </summary>
        /// <param name="work">The work to be performed.</param>
        /// <param name="args">The arguments to pass to the delegate.</param>
        public void QueueWorkItem(Delegate work, params object[] args)
        {
            this.QueueWorkItem(work, true, args);
        }

        /// <summary>
        /// Queues work for the worker thread(s).
        /// </summary>
        /// <param name="work">The work to be performed.</param>
        /// <param name="isArray">Ignored.</param>
        /// <param name="args">The arguments to pass to the delegate.</param>
        public void QueueWorkItem(Delegate work, bool isArray, object[] args)
        {
            lock (_workQueue)
                _workQueue.Enqueue(new WorkItem(work, args));

            _workArrivedEvent.Set();

            // Check if all worker threads are currently busy.
            if (_busyCount == _workerThreads.Count)
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
                        if (_workerThreads.Count > _maxWorkerThreads)
                        {
                            // We have an excess amount of worker threads.
                            this.DestroyWorkerThread();
                            return;
                        }
                    }
                }

                // Check for work
                if (_workQueue.Count > 0)
                {
                    WorkItem item = null;

                    // There is work, but we must lock and re-check.
                    lock (_workQueue)
                    {
                        if (_workQueue.Count > 0)
                            item = _workQueue.Dequeue();
                        else
                            continue;
                    }

                    Interlocked.Increment(ref _busyCount);

                    try
                    {
                        item.PerformWork();
                    }
                    catch (Exception ex)
                    {
                        Logging.Log(ex);
                    }

                    Interlocked.Decrement(ref _busyCount);
                }
                else
                {
                    // No work available. Wait for work.
                    if (_workArrivedEvent.WaitOne(1000))
                    {
                        // Work arrived. Go back so we can perform it.
                        continue;
                    }
                    else
                    {
                        // No work arrived in 1 second. Delete the thread.
                        lock (_workerThreads)
                            this.DestroyWorkerThread();

                        return;
                    }
                }
            }
        }
    }
}
