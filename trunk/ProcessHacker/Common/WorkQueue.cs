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
    public class WorkQueue
    {
        public delegate void WorkItemDelegate(object parameter);

        private class WorkItem
        {              
            public Delegate _work;
            public object _parameter;

            public WorkItem(Delegate work, object parameter)
            {
                _work = work;
                _parameter = parameter;
            }

            public Delegate Work
            {
                get { return _work; }
            }

            public object Parameter
            {
                get { return _parameter; }
            }

            public void PerformWork()
            {
                _work.DynamicInvoke(_parameter);
            }
        }

        private static WorkQueue _globalWorkQueue = new WorkQueue();

        public static WorkQueue GlobalWorkQueue
        {
            get { return _globalWorkQueue; }
        }

        public static void GlobalQueueWorkItem(Delegate work)
        {
            _globalWorkQueue.QueueWorkItem(work);
        }

        public static void GlobalQueueWorkItem(Delegate work, object parameter)
        {
            _globalWorkQueue.QueueWorkItem(work, parameter);
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
        /// Gets the worker thread limit.
        /// </summary>
        public int MaxWorkerThreads
        {
            get { return _maxWorkerThreads; }
            set { _maxWorkerThreads = value; }
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
            this.QueueWorkItem(work, null);
        }

        /// <summary>
        /// Queues work for the worker thread(s).
        /// </summary>
        /// <param name="work">The work to be performed.</param>
        /// <param name="parameter">A parameter to pass to the delegate.</param>
        public void QueueWorkItem(Delegate work, object parameter)
        {
            lock (_workQueue)
                _workQueue.Enqueue(new WorkItem(work, parameter));

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
                    item.PerformWork();
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
