/*
 * Process Hacker - 
 *   private namespace handle
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
using ProcessHacker.Common.Objects;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Security;

namespace ProcessHacker.Native.Objects
{
    /// <summary>
    /// Represents a private namespace, a private directory object.
    /// </summary>
    public class PrivateNamespaceHandle : DirectoryHandle
    {
        public static PrivateNamespaceHandle Create(BoundaryDescriptor boundaryDescriptor, string aliasPrefix)
        {
            IntPtr handle = IntPtr.Zero;

            handle = Win32.CreatePrivateNamespace(IntPtr.Zero, boundaryDescriptor.Descriptor, aliasPrefix);

            if (handle == IntPtr.Zero)
                Win32.ThrowLastError();

            return new PrivateNamespaceHandle(handle, true);
        }

        private bool _destroy = false;

        private PrivateNamespaceHandle(IntPtr handle, bool owned)
            : base(handle, owned)
        { }

        public PrivateNamespaceHandle(BoundaryDescriptor boundaryDescriptor, string aliasPrefix)
        {
            this.Handle = Win32.OpenPrivateNamespace(boundaryDescriptor.Descriptor, aliasPrefix);

            if (this.Handle == IntPtr.Zero)
            {
                this.MarkAsInvalid();
                Win32.ThrowLastError();
            }
        }

        protected override void Close()
        {
            Win32.ClosePrivateNamespace(this, _destroy ? PrivateNamespaceFlags.Destroy : 0);
        }

        public void MarkForDestruction()
        {
            _destroy = true;
        }
    }

    public class BoundaryDescriptor : BaseObject
    {
        private IntPtr _descriptor;

        public BoundaryDescriptor(string name)
            : this(name, null)
        { }

        public BoundaryDescriptor(string name, IEnumerable<Sid> sids)
        {
            _descriptor = Win32.CreateBoundaryDescriptor(name, 0);

            if (_descriptor == IntPtr.Zero)
            {
                this.DisableOwnership(false);
                Win32.ThrowLastError();
            }

            if (sids != null)
            {
                foreach (Sid sid in sids)
                    this.Add(sid);
            }
        }

        protected override void DisposeObject(bool disposing)
        {
            Win32.DeleteBoundaryDescriptor(_descriptor);
        }

        public IntPtr Descriptor
        {
            get { return _descriptor; }
        }

        public void Add(Sid sid)
        {
            if (!Win32.AddSIDToBoundaryDescriptor(ref _descriptor, sid))
                Win32.ThrowLastError();
        }
    }
}
