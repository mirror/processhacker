/*
 * Process Hacker - 
 *   secured handle table
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

namespace ProcessHacker.Common.Objects
{
    public class SecuredHandleTableEntry : HandleTableEntry
    {
        private long _grantedAccess;

        public long GrantedAccess
        {
            get { return _grantedAccess; }
            set { _grantedAccess = value; }
        }

        public bool AreAllAccessesGranted<TAccess>(TAccess access)
            where TAccess : struct
        {
            long accessLong = Convert.ToInt64(access);

            if ((_grantedAccess & accessLong) == accessLong)
                return true;
            else
                return false;
        }

        public bool AreAnyAccessesGranted<TAccess>(TAccess access)
            where TAccess : struct
        {
            long accessLong = Convert.ToInt64(access);

            if ((_grantedAccess & accessLong) != 0)
                return true;
            else
                return false;
        }
    } 

    /// <summary>
    /// Provides methods for managing handles to objects securely.
    /// </summary>
    public class SecuredHandleTable : SecuredHandleTable<SecuredHandleTableEntry>
    { }

    /// <summary>
    /// Provides methods for managing handles to objects securely.
    /// </summary>
    /// <typeparam name="TEntry">The type of each handle table entry.</typeparam>
    public class SecuredHandleTable<TEntry> : HandleTable<TEntry>
        where TEntry : SecuredHandleTableEntry, new()
    {
        /// <summary>
        /// Creates a handle to an object with the specified granted access.
        /// </summary>
        /// <typeparam name="TAccess">The type of access mask.</typeparam>
        /// <param name="obj">The object to reference.</param>
        /// <param name="grantedAccess">The granted access to the object.</param>
        /// <returns>The new handle.</returns>
        public int Allocate<TAccess>(IRefCounted obj, TAccess grantedAccess)
            where TAccess : struct
        {
            TEntry entry = new TEntry();

            entry.GrantedAccess = Convert.ToInt64(grantedAccess);

            return base.Allocate(obj, entry);
        }

        /// <summary>
        /// References an object using a handle.
        /// </summary>
        /// <typeparam name="TAccess">The type of access mask.</typeparam>
        /// <param name="handle">The handle to lookup.</param>
        /// <param name="access">The desired access to the object.</param>
        /// <returns>
        /// An object. This object has been referenced and must be 
        /// dereferenced once it is no longer needed.
        /// </returns>
        public IRefCounted ReferenceByHandle<TAccess>(int handle, TAccess access)
            where TAccess : struct
        {
            return this.ReferenceByHandle<TAccess>(handle, access, false);
        }

        /// <summary>
        /// References an object using a handle.
        /// </summary>
        /// <typeparam name="TAccess">The type of access mask.</typeparam>
        /// <param name="handle">The handle to lookup.</param>
        /// <param name="access">The desired access to the object.</param>
        /// <param name="throwOnAccessDenied">
        /// Whether an exception will be thrown if access to the object is denied.
        /// </param>
        /// <returns>
        /// An object. This object has been referenced and must be 
        /// dereferenced once it is no longer needed.
        /// </returns>
        public IRefCounted ReferenceByHandle<TAccess>(int handle, TAccess access, bool throwOnAccessDenied)
            where TAccess : struct
        {
            TEntry entry;
            IRefCounted obj;

            // Reference the object.
            obj = this.ReferenceByHandle(handle, out entry);

            if (obj == null)
                return null;

            // Check the access.
            if (entry.AreAllAccessesGranted<TAccess>(access))
            {
                // OK, return the object.
                return obj;
            }
            else
            {
                // Access denied. Dereference the object and return.
                obj.Dereference();

                if (throwOnAccessDenied)
                    throw new UnauthorizedAccessException("Access denied.");
                else
                    return null;
            }
        }

        /// <summary>
        /// References an object using a handle.
        /// </summary>
        /// <typeparam name="T">The type of the object to reference.</typeparam>
        /// <typeparam name="TAccess">The type of access mask.</typeparam>
        /// <param name="handle">The handle to lookup.</param>
        /// <param name="access">The desired access to the object.</param>
        /// <returns>
        /// An object. This object has been referenced and must be 
        /// dereferenced once it is no longer needed.
        /// </returns>
        public T ReferenceByHandle<T, TAccess>(int handle, TAccess access)
            where T : class, IRefCounted
            where TAccess : struct
        {
            return this.ReferenceByHandle<T, TAccess>(handle, access, false);
        }

        /// <summary>
        /// References an object using a handle.
        /// </summary>
        /// <typeparam name="T">The type of the object to reference.</typeparam>
        /// <typeparam name="TAccess">The type of access mask.</typeparam>
        /// <param name="handle">The handle to lookup.</param>
        /// <param name="access">The desired access to the object.</param>
        /// <param name="throwOnAccessDenied">
        /// Whether an exception will be thrown if access to the object is denied.
        /// </param>
        /// <returns>
        /// An object. This object has been referenced and must be 
        /// dereferenced once it is no longer needed.
        /// </returns>
        public T ReferenceByHandle<T, TAccess>(int handle, TAccess access, bool throwOnAccessDenied)
            where T : class, IRefCounted
            where TAccess : struct
        {
            IRefCounted obj = this.ReferenceByHandle<TAccess>(handle, access, throwOnAccessDenied);

            if (obj == null)
                return null;

            // Check the type.
            if (obj is T)
            {
                return (T)obj;
            }
            else
            {
                obj.Dereference();
                return null;
            }
        }
    }
}
