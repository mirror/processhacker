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

namespace ProcessHacker.Common
{
    /// <summary>
    /// Provides methods for managing a disposable object or resource.
    /// </summary>
    public abstract class DisposableObject : IDisposable, IReferenceCountedObject
    {
        private bool _owned = true;
        private int _refCount = 1;
        private bool _disposed = false;

        /// <summary>
        /// Initializes a disposable object.
        /// </summary>
        public DisposableObject()
            : this(true)
        { }

        /// <summary>
        /// Initializes a disposable object.
        /// </summary>
        /// <param name="owned">Whether the resource is owned.</param>
        public DisposableObject(bool owned)
        {
            _owned = owned;

            // Don't need to finalize the object if it doesn't need to be disposed.
            if (!_owned)
                GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Frees the resources associated with the object.
        /// </summary>
        ~DisposableObject()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Decrements the reference count of the object.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Decrements the reference count of the object.
        /// </summary>
        /// <param name="disposing">Whether to dispose managed resources.</param>
        public void Dispose(bool disposing)
        {
            _refCount--;

            if (_refCount < 0)
                _refCount = 0;

            if (!_disposed && _refCount == 0 && _owned)
            {
                this.DisposeObject(disposing);

                _disposed = true;
            }
        }

        /// <summary>
        /// Disposes the resources of the object. This method must not be 
        /// called directly; instead, override this method in a derived class.
        /// </summary>
        /// <param name="disposing">Whether or not to dispose managed objects.</param>
        protected virtual void DisposeObject(bool disposing) { }

        /// <summary>
        /// Gets whether the object is freed.
        /// </summary>
        public bool Disposed
        {
            get { return _disposed; }
        }

        /// <summary>
        /// Gets whether the object can be freed.
        /// </summary>
        public bool Owned
        {
            get { return _owned; }
        }

        /// <summary>
        /// Gets the current reference count of the object.
        /// </summary>
        public int ReferenceCount
        {
            get { return _refCount; }
        }

        /// <summary>
        /// Decrements the reference count of the object.
        /// </summary>
        /// <returns>The old reference count.</returns>
        public int Dereference()
        {
            return this.Dereference(true);
        }

        /// <summary>
        /// Decrements the reference count of the object.
        /// </summary>
        /// <param name="managed">Whether to dispose managed resources.</param>
        /// <returns>The old reference count.</returns>
        public int Dereference(bool managed)
        {
            if (!_owned)
                return 0;

            if (_refCount == 0)
                throw new InvalidOperationException("Reference count cannot be negative.");

            this.Dispose(managed);
            return _refCount;
        }

        /// <summary>
        /// Increments the reference count of the object.
        /// </summary>
        /// <returns>The old reference count.</returns>
        public int Reference()
        {
            if (!_owned)
                return 0;

            int oldRefCount = _refCount;

            _refCount++;

            return oldRefCount;
        }
    }
}
