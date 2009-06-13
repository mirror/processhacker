/*
 * Process Hacker - 
 *   disposable object base functionality
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
using System.Threading;

namespace ProcessHacker.Common.Objects
{
    /// <summary>
    /// Provides methods for managing a disposable object or resource.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Each disposable object starts with a reference count of one 
    /// when it is created. The object is not owned by the creator; 
    /// rather, it is owned by the GC (garbage collector). If the user 
    /// does not dispose the object, the finalizer will be called by 
    /// the GC, the reference count will be decremented and the object 
    /// will be freed. If the user chooses to call Dispose, the reference 
    /// count will be decremented and the object will be freed. The 
    /// object is no longer owned by the GC and the finalizer will be 
    /// suppressed. Any further calls to Dispose will have no effect.
    /// </para>
    /// <para>
    /// If the user chooses to use reference counting, the object 
    /// functions normally with the GC. If the object's reference count 
    /// is incremented after it is created and becomes 2, it will be 
    /// decremented when it is finalized or disposed. Only after the 
    /// object is dereferenced will the reference count become 0 and 
    /// the object will be freed.
    /// </para>
    /// </remarks>
    public abstract class BaseObject : IDisposable, IReferenceCountedObject
    {
        private static int _createdCount = 0;
        private static int _freedCount = 0;
        private static int _disposedCount = 0;
        private static int _finalizedCount = 0;
        private static int _referencedCount = 0;
        private static int _dereferencedCount = 0;

        /// <summary>
        /// Gets the number of disposable objects that have been created.
        /// </summary>
        public static int CreatedCount { get { return _createdCount; } }
        /// <summary>
        /// Gets the number of disposable objects that have been freed.
        /// </summary>
        public static int FreedCount { get { return _freedCount; } }
        /// <summary>
        /// Gets the number of disposable objects that have been Disposed with managed = true.
        /// </summary>
        public static int DisposedCount { get { return _disposedCount; } }
        /// <summary>
        /// Gets the number of disposable objects that have been Disposed with managed = false.
        /// </summary>
        public static int FinalizedCount { get { return _finalizedCount; } }
        /// <summary>
        /// Gets the number of times disposable objects have been referenced.
        /// </summary>
        public static int ReferencedCount { get { return _referencedCount; } }
        /// <summary>
        /// Gets the number of times disposable objects have been dereferenced.
        /// </summary>
        public static int DereferencedCount { get { return _dereferencedCount - _disposedCount - _finalizedCount; } }

#if DEBUG
        /// <summary>
        /// A stack trace collected when the object is created.
        /// </summary>
        private string _creationStackTrace;
#endif
        /// <summary>
        /// Whether the object is owned (rather, whether this class should 
        /// take care of anything).
        /// </summary>
        private bool _owned = true;
        /// <summary>
        /// Whether the object is owned by the garbage collector (to ensure 
        /// calling Dispose more than once has no effect).
        /// </summary>
        private volatile bool _ownedByGc = true;
        /// <summary>
        /// Synchronization for all reference-related methods.
        /// </summary>
        private object _refLock = new object();
        /// <summary>
        /// The reference count of the object.
        /// </summary>
        private int _refCount = 1;
        /// <summary>
        /// The weak reference count of the object. This is subtracted 
        /// from the actual reference count when the object is no longer 
        /// reachable.
        /// </summary>
        private int _weakRefCount = 0;
        /// <summary>
        /// Whether the finalizer will run.
        /// </summary>
        private volatile bool _finalizerRegistered = true;
        /// <summary>
        /// Synchronization for finalizer-related methods.
        /// </summary>
        private object _finalizerRegisterLock = new object();
        /// <summary>
        /// Whether the object has been freed.
        /// </summary>
        private volatile bool _disposed = false;

        /// <summary>
        /// Initializes a disposable object.
        /// </summary>
        public BaseObject()
            : this(true)
        { }

        /// <summary>
        /// Initializes a disposable object.
        /// </summary>
        /// <param name="owned">Whether the resource is owned.</param>
        public BaseObject(bool owned)
        {
            _owned = owned;

            // Don't need to finalize the object if it doesn't need to be disposed.
            if (!_owned)
                GC.SuppressFinalize(this);

            Interlocked.Increment(ref _createdCount);
#if DEBUG
            _creationStackTrace = Environment.StackTrace;
#endif
        }

        /// <summary>
        /// Ensures that the GC does not own the object and destroys
        /// all weak references.
        /// </summary>
        ~BaseObject()
        {
            // Get rid of GC ownership if still present.
            this.Dispose(false);
            // Zero the weak reference count.
            this.Dereference(_weakRefCount, false);
            _weakRefCount = 0;
        }

        /// <summary>
        /// Ensures that the GC does not own the object.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Ensures that the GC does not own the object.
        /// </summary>
        /// <param name="managed">Whether to dispose managed resources.</param>
        public void Dispose(bool managed)
        {
            Thread.BeginCriticalRegion();

            if (managed)
                Monitor.Enter(_refLock);

            try
            {
                // Only proceed if the object is owned by the GC.
                if (_ownedByGc)
                {
                    // Decrement the reference count.
                    this.Dereference(managed);
                    _ownedByGc = false;

                    // Disable the finalizer if we don't need it.
                    if (Thread.VolatileRead(ref _weakRefCount) == 0)
                    {
                        if (managed)
                            this.DisableFinalizer();
                    }

                    // Stats...
                    if (managed)
                        Interlocked.Increment(ref _disposedCount);
                    else
                        Interlocked.Increment(ref _finalizedCount);
                }
            }
            finally
            {
                if (managed)
                    Monitor.Exit(_refLock);

                Thread.EndCriticalRegion();
            }
        }

        /// <summary>
        /// Queues the object for disposal in the current delayed release pool.
        /// </summary>
        public void DisposeDelayed()
        {
            DelayedReleasePool.CurrentPool.AddDispose(this);
        }

        /// <summary>
        /// Disposes the resources of the object. This method must not be 
        /// called directly; instead, override this method in a derived class.
        /// </summary>
        /// <param name="disposing">Whether or not to dispose managed objects.</param>
        protected virtual void DisposeObject(bool disposing) { }

        /// <summary>
        /// Gets whether the object has been freed.
        /// </summary>
        public bool Disposed
        {
            get { return _disposed; }
        }

        /// <summary>
        /// Gets whether the object will be freed.
        /// </summary>
        public bool Owned
        {
            get { return _owned; }
        }

        /// <summary>
        /// Gets whether the object is owned by the garbage collector.
        /// </summary>
        public bool OwnedByGc
        {
            get { return _ownedByGc; }
        }

        /// <summary>
        /// Gets the current reference count of the object.
        /// </summary>
        /// <remarks>
        /// This information is for debugging purposes ONLY. DO NOT 
        /// base memory management logic upon this value.
        /// </remarks>
        public int ReferenceCount
        {
            get { return Thread.VolatileRead(ref _refCount); }
        }

        /// <summary>
        /// Gets the current weak reference count of the object.
        /// </summary>
        /// <remarks>
        /// This information is for debugging purposes ONLY. DO NOT 
        /// base memory management logic upon this value.
        /// </remarks>
        public int WeakReferenceCount
        {
            get { return Thread.VolatileRead(ref _weakRefCount); }
        }

        private void DisableFinalizer()
        {
            lock (_finalizerRegisterLock)
            {
                if (_finalizerRegistered)
                {
                    GC.SuppressFinalize(this);
                    _finalizerRegistered = false;
                }
            }
        }

        /// <summary>
        /// Decrements the reference count of the object.
        /// </summary>
        /// <returns>The old reference count.</returns>
        /// <remarks>
        /// <para>
        /// DO NOT call Dereference if you have not called Reference. 
        /// Call Dispose instead.
        /// </para>
        /// <para>
        /// If you are calling Dereference from a finalizer, call 
        /// Dereference(false).
        /// </para>
        /// </remarks>
        public int Dereference()
        {
            return this.Dereference(true);
        }

        /// <summary>
        /// Decrements the reference count of the object.
        /// </summary>
        /// <param name="managed">Whether to dispose managed resources.</param>
        /// <returns>The new reference count.</returns>
        /// <remarks>
        /// <para>If you are calling this method from a finalizer, set 
        /// <paramref name="managed" /> to false.</para>
        /// </remarks>
        public int Dereference(bool managed)
        {
            return this.Dereference(1, managed);
        }

        private int Dereference(int count, bool managed)
        {
            Thread.BeginCriticalRegion();

            if (managed)
                Monitor.Enter(_refLock);

            try
            {
                if (!_owned)
                    return 0;

                Interlocked.Add(ref _dereferencedCount, count);

                // Decrease the reference count.
                int newRefCount = Interlocked.Add(ref _refCount, -count);

                if (newRefCount < 0)
                    throw new InvalidOperationException("Reference count cannot be negative.");

                // Free the object if the reference count is 0.
                if (newRefCount == 0 && !_disposed)
                {
                    this.DisposeObject(managed);
                    _disposed = true;
                    Interlocked.Increment(ref _freedCount);
                }

                return newRefCount;
            }
            finally
            {
                if (managed)
                    Monitor.Exit(_refLock);

                Thread.EndCriticalRegion();
            }
        }

        /// <summary>
        /// Queues the object for dereferencing in the current delayed release pool.
        /// </summary>
        public void DereferenceDelayed()
        {
            DelayedReleasePool.CurrentPool.AddDereference(this);
        }

        private void EnableFinalizer()
        {
            lock (_finalizerRegisterLock)
            {
                if (!_finalizerRegistered)
                {
                    GC.ReRegisterForFinalize(this);
                    _finalizerRegistered = true;
                }
            }
        }

        /// <summary>
        /// Increments the reference count of the object.
        /// </summary>
        /// <returns>The new reference count.</returns>
        /// <remarks>
        /// <para>
        /// You must call Dereference once (when you are finished with the 
        /// object) to match each call to Reference. Do not call Dispose.
        /// </para>
        /// </remarks>
        public int Reference()
        {
            if (!_owned)
                return 0;

            Interlocked.Increment(ref _referencedCount);

            return Interlocked.Increment(ref _refCount);
        }

        /// <summary>
        /// Increments the reference count of the object and ensures the 
        /// reference count will decremented when the object is no 
        /// longer reachable.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// You must not call Dereference to decrement the reference count 
        /// as it will be decremented automatically.
        /// </remarks>
        public int ReferenceWeak()
        {
            if (!_owned)
                return 0;

            Thread.BeginCriticalRegion();

            try
            {
                Interlocked.Increment(ref _weakRefCount);
                this.EnableFinalizer();
                return Interlocked.Increment(ref _refCount);
            }
            finally
            {
                Thread.EndCriticalRegion();
            }
        }
    }
}
