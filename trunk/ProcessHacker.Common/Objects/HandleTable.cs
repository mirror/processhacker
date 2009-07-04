/*
 * Process Hacker - 
 *   handle table
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

namespace ProcessHacker.Common.Objects
{
    public class HandleTableEntry
    {
        private int _handle;
        private IRefCounted _object;

        public int Handle
        {
            get { return _handle; }
            internal set { _handle = value; }
        }

        public IRefCounted Object
        {
            get { return _object; }
            internal set { _object = value; }
        }
    }

    /// <summary>
    /// Provides methods for managing handles to objects.
    /// </summary>
    public class HandleTable : HandleTable<HandleTableEntry>
    { }

    /// <summary>
    /// Provides methods for managing handles to objects.
    /// </summary>         
    /// <typeparam name="TEntry">The type of each handle table entry.</typeparam>
    public class HandleTable<TEntry> : BaseObject
        where TEntry : HandleTableEntry, new()
    {
        /// <summary>
        /// Represents a callback function for handle table enumeration.
        /// </summary>
        /// <param name="handle">The current handle.</param>
        /// <param name="obj">The current object.</param>
        /// <returns>Return true to stop enumerating; otherwise return false.</returns>
        public delegate bool EnumerateHandleTableDelegate(int handle, TEntry entry);

        private IdGenerator _handleGenerator = new IdGenerator(4, 4);
        private Dictionary<int, TEntry> _handles =
            new Dictionary<int, TEntry>();

        protected override void DisposeObject(bool disposing)
        {
            lock (_handles)
            {
                foreach (var entry in _handles.Values)
                    entry.Object.Dereference(disposing);
            }
        }

        /// <summary>
        /// Creates a handle to the specified object.
        /// </summary>
        /// <param name="obj">The object to reference.</param>
        /// <returns>The new handle.</returns>
        public int Allocate(IRefCounted obj)
        {
            TEntry entry = new TEntry();

            return this.Allocate(obj, entry);
        }

        /// <summary>
        /// Creates a handle to the specified object.
        /// </summary>
        /// <param name="obj">The object to reference.</param>
        /// <param name="entry">The handle table entry to use.</param>
        /// <returns>The new handle.</returns>
        public int Allocate(IRefCounted obj, TEntry entry)
        {
            int handle = _handleGenerator.Pop();

            // Reference the object so it does not get freed while 
            // it is stored in the handle table.
            obj.Reference();
            // GC should not own the object.
            obj.Dispose();
            // Initialize the entry.
            entry.Handle = handle;
            entry.Object = obj;

            // Add the handle entry.
            lock (_handles)
            {
                _handles.Add(handle, entry);
            }

            return handle;
        }

        /// <summary>
        /// Enumerates the handles in the handle table.
        /// </summary>
        /// <param name="callback">The callback for the enumeration.</param>
        /// <returns>Whether the enumeration was stopped by the callback.</returns>
        public bool Enumerate(EnumerateHandleTableDelegate callback)
        {
            lock (_handles)
            {
                foreach (var entry in _handles.Values)
                {
                    if (callback(entry.Handle, entry))
                        return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Closes a handle.
        /// </summary>
        /// <param name="handle">The handle to close.</param>
        /// <returns>Whether the handle was closed.</returns>
        public bool Free(int handle)
        {
            IRefCounted obj;

            lock (_handles)
            {
                if (!_handles.ContainsKey(handle))
                    return false;

                // Store the object reference for dereferencing later.
                obj = _handles[handle].Object;
                // Remove the handle so it can no longer be used.
                _handles.Remove(handle);
            }

            // Make the handle value available.
            _handleGenerator.Push(handle);
            // Dereference the object (this doesn't need to be in the locking block).
            obj.Dereference();

            return true;
        }

        /// <summary>
        /// Gets the handle table entry for a handle.
        /// </summary>
        /// <param name="handle">The handle to lookup.</param>
        /// <returns>A handle table entry.</returns>
        public TEntry LookupEntry(int handle)
        {
            lock (_handles)
            {
                if (_handles.ContainsKey(handle))
                    return _handles[handle];
                else
                    return null;
            }
        }

        /// <summary>
        /// Gets the object referenced by a handle.
        /// </summary>
        /// <param name="handle">The handle to lookup.</param>
        /// <returns>
        /// An object. This object has not been referenced and is 
        /// not guaranteed to be valid.
        /// </returns>
        public IRefCounted LookupObject(int handle)
        {
            return this.LookupEntry(handle).Object;
        }

        /// <summary>
        /// Gets the object referenced by a handle.
        /// </summary>
        /// <typeparam name="T">The type of the object to retrieve.</typeparam>
        /// <param name="handle">The handle to lookup.</param>
        /// <returns>
        /// An object. This object has not been referenced and is 
        /// not guaranteed to be valid.
        /// </returns>
        public T LookupObject<T>(int handle)
            where T : class, IRefCounted
        {
            return this.LookupObject(handle) as T;
        }

        /// <summary>
        /// References an object using a handle.
        /// </summary>
        /// <param name="handle">The handle to lookup.</param>
        /// <returns>
        /// An object. This object has been referenced and must be 
        /// dereferenced once it is no longer needed.
        /// </returns>
        public IRefCounted ReferenceByHandle(int handle)
        {
            TEntry entry;
            return this.ReferenceByHandle(handle, out entry);
        }

        /// <summary>
        /// References an object using a handle.
        /// </summary>
        /// <param name="handle">The handle to lookup.</param>
        /// <param name="entry">The handle table entry.</param>
        /// <returns>
        /// An object. This object has been referenced and must be 
        /// dereferenced once it is no longer needed.
        /// </returns>
        public IRefCounted ReferenceByHandle(int handle, out TEntry entry)
        {
            lock (_handles)
            {
                if (_handles.ContainsKey(handle))
                {
                    IRefCounted obj = _handles[handle].Object;

                    obj.Reference();
                    entry = _handles[handle];

                    return obj;
                }
                else
                {
                    entry = null;
                    return null;
                }
            }
        }

        /// <summary>
        /// References an object using a handle.
        /// </summary>
        /// <typeparam name="T">The type of the object to reference.</typeparam>
        /// <param name="handle">The handle to lookup.</param>
        /// <returns>
        /// An object. This object has been referenced and must be 
        /// dereferenced once it is no longer needed.
        /// </returns>
        public T ReferenceByHandle<T>(int handle)
            where T : class, IRefCounted
        {
            TEntry entry;
            return this.ReferenceByHandle<T>(handle, out entry);
        }

        /// <summary>
        /// References an object using a handle.
        /// </summary>
        /// <typeparam name="T">The type of the object to reference.</typeparam>
        /// <param name="handle">The handle to lookup.</param>
        /// <param name="entry">The handle table entry.</param>
        /// <returns>
        /// An object. This object has been referenced and must be 
        /// dereferenced once it is no longer needed.
        /// </returns>
        public T ReferenceByHandle<T>(int handle, out TEntry entry)
            where T : class, IRefCounted
        {
            IRefCounted obj = this.ReferenceByHandle(handle, out entry);

            if (obj == null)
                return null;

            // Check the type.
            if (obj is T)
            {
                return (T)obj;
            }
            else
            {
                // Wrong type. Dereference and return.
                obj.Dereference();
                return null;
            }
        }
    }
}
