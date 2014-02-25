/*
 * Process Hacker - 
 *   delayed release pool
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

namespace ProcessHacker.Common.Objects
{
    /// <summary>
    /// Indicates that an operation was performed out-of-order.
    /// </summary>
    public class OutOfOrderException : Exception
    {
        public OutOfOrderException(string message)
            : base(message)
        { }
    }

    /// <summary>
    /// Represents a pool of objects to be disposed or dereferenced at some point.
    /// </summary>
    public sealed class DelayedReleasePool : BaseObject
    {
        /// <summary>
        /// Describes how an object should be disposed.
        /// </summary>
        [Flags]
        private enum DelayedReleaseFlags
        {
            Dispose = 0x1,
            Dereference = 0x2
        }

        /// <summary>
        /// Describes an object that is to be disposed.
        /// </summary>
        private struct DelayedReleaseObject
        {
            private DelayedReleaseFlags _flags;
            private BaseObject _object;

            public DelayedReleaseObject(DelayedReleaseFlags flags, BaseObject obj)
            {
                _flags = flags;
                _object = obj;
            }

            public DelayedReleaseFlags Flags
            {
                get { return _flags; }
            }

            public BaseObject Object
            {
                get { return _object; }
            }
        }

        [ThreadStatic]
        private static Stack<DelayedReleasePool> _poolStack;
        [ThreadStatic]
        private static DelayedReleasePool _currentPool;

        /// <summary>
        /// Gets the currently active delayed release pool.
        /// </summary>
        public static DelayedReleasePool CurrentPool
        {
            get
            {
                if (_currentPool == null)
                    _currentPool = new DelayedReleasePool();

                return _currentPool;
            }
            private set { _currentPool = value; }
        }

        /// <summary>
        /// Gets the stack of delayed release pools.
        /// </summary>
        private static Stack<DelayedReleasePool> PoolStack
        {
            get
            {
                // No locking needed because the stack is thread-local.
                if (_poolStack == null)
                    _poolStack = new Stack<DelayedReleasePool>();

                return _poolStack;
            }
        }

        /// <summary>
        /// Restores an older delayed release pool from the pool stack.
        /// </summary>
        /// <param name="pool">The current delayed release pool.</param>
        private static void PopPool(DelayedReleasePool pool)
        {
            if (_currentPool != pool)
                throw new OutOfOrderException(
                    "Attempted to pop a pool when it wasn't on top of the stack. " +
                    "This usually indicates that a pool was popped out-of-order."
                    );

            _currentPool = PoolStack.Pop();
        }

        /// <summary>
        /// Sets the specified delayed release pool as the currently active pool.
        /// </summary>
        /// <param name="pool">The pool to set.</param>
        private static void PushPool(DelayedReleasePool pool)
        {
            PoolStack.Push(_currentPool);
            _currentPool = pool;
        }

        private int _creatorThreadId;
        private List<DelayedReleaseObject> _objects = new List<DelayedReleaseObject>();

        /// <summary>
        /// Creates a delayed release pool and sets it as the currently active pool.
        /// </summary>
        public DelayedReleasePool()
        {
            _creatorThreadId = Thread.CurrentThread.ManagedThreadId;
            PushPool(this);
        }

        protected override void DisposeObject(bool disposing)
        {
            // Only pop the pool if we're on the same thread as the 
            // creator thread. This either means that the thread has 
            // died, or the user forgot to pop the pool by calling 
            // Dispose. If they forgot, it's not our problem...
            if (
                disposing && 
                _creatorThreadId == Thread.CurrentThread.ManagedThreadId
                )
                PopPool(this);

            this.Drain(disposing);
        }

        /// <summary>
        /// Adds the specified object for dereferencing.
        /// </summary>
        /// <param name="obj">The object to dereference.</param>
        public void AddDereference(BaseObject obj)
        {
            _objects.Add(new DelayedReleaseObject(DelayedReleaseFlags.Dereference, obj));
        }

        /// <summary>
        /// Adds the specified object for disposal.
        /// </summary>
        /// <param name="obj">The object to dispose.</param>
        public void AddDispose(BaseObject obj)
        {
            _objects.Add(new DelayedReleaseObject(DelayedReleaseFlags.Dispose, obj));
        }

        /// <summary>
        /// Releases all objects in the pool.
        /// </summary>
        public void Drain()
        {
            this.Drain(true);
        }

        /// <summary>
        /// Releases all objects in the pool.
        /// </summary>
        /// <param name="managed">Whether to release managed resources.</param>
        public void Drain(bool managed)
        {
            foreach (var obj in _objects)
            {
                if ((obj.Flags & DelayedReleaseFlags.Dispose) == DelayedReleaseFlags.Dispose)
                    obj.Object.Dispose();
                if ((obj.Flags & DelayedReleaseFlags.Dereference) == DelayedReleaseFlags.Dereference)
                    obj.Object.Dereference(managed);
            }

            _objects.Clear();
        }
    }
}
