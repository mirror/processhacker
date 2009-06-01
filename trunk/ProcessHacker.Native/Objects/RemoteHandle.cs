/*
 * Process Hacker - 
 *   remote handle
 * 
 * Copyright (C) 2008 wj32
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
namespace ProcessHacker.Native.Objects
{
    /// <summary>
    /// Represents a handle owned by another process.
    /// </summary>
    public class RemoteHandle
    {
        private ProcessHandle _phandle;
        private IntPtr _handle;

        public RemoteHandle(ProcessHandle phandle, IntPtr handle)
        {
            _phandle = phandle;
            _handle = handle;
        }

        public ProcessHandle ProcessHandle
        {
            get { return _phandle; }
        }

        public IntPtr Handle
        {
            get { return _handle; }
        }

        /// <summary>
        /// Duplicates the handle owned by the process.
        /// </summary>
        /// <param name="access">The desired access to the handle's object.</param>
        /// <returns>A local copy of the handle.</returns>
        /// <remarks>
        /// We can't use a template for this because of C#'s rules for template 
        /// restrictions. Specifically, we can only specify that the type must have a
        /// constructor with 0 arguments, but no more.
        /// </remarks>
        public int GetHandle(int access)
        {
            return new GenericHandle(_phandle, _handle, access);
        }
    }
}
