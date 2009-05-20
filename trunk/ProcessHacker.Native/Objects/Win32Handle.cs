/*
 * Process Hacker - 
 *   windows handle
 * 
 * Copyright (C) 2008-2009 wj32
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
using ProcessHacker.Native.Api;

namespace ProcessHacker.Native.Objects
{
    /// <summary>
    /// Represents a generic Windows handle.
    /// </summary>
    public class Win32Handle : Win32Handle<int>
    {
        public static NtStatus WaitAll(ISynchronizable[] objects, bool alertable, long timeout)
        {
            return WaitForMultipleObjects(objects, WaitType.WaitAll, alertable, timeout);
        }

        public static NtStatus WaitAll(ISynchronizable[] objects, long timeout)
        {
            return WaitAll(objects, false, timeout);
        }

        public static NtStatus WaitAll(ISynchronizable[] objects)
        {
            return WaitAll(objects, -1);
        }

        public static NtStatus WaitAny(ISynchronizable[] objects, bool alertable, long timeout)
        {
            return WaitForMultipleObjects(objects, WaitType.WaitAny, alertable, timeout);
        }

        public static NtStatus WaitAny(ISynchronizable[] objects, long timeout)
        {
            return WaitAny(objects, false, timeout);
        }

        public static NtStatus WaitAny(ISynchronizable[] objects)
        {
            return WaitAny(objects, -1);
        }

        private static NtStatus WaitForMultipleObjects(ISynchronizable[] objects, WaitType waitType, bool alertable, long timeout)
        {
            NtStatus status;
            IntPtr[] handles = new IntPtr[objects.Length];

            for (int i = 0; i < objects.Length; i++)
                handles[i] = objects[i].Handle;

            if ((status = Win32.NtWaitForMultipleObjects(
                handles.Length,
                handles,
                waitType,
                alertable,
                ref timeout
                )) >= NtStatus.Error)
                Win32.ThrowLastError(status);

            return status;
        }

        /// <summary>
        /// Creates a new, invalid handle. You must set the handle using the Handle property.
        /// </summary>
        protected Win32Handle()
            : base()
        { }

        /// <summary>
        /// Creates a new handle using the specified value. The handle will be closed when 
        /// this object is disposed or garbage-collected.
        /// </summary>
        /// <param name="handle">The handle value.</param>
        public Win32Handle(IntPtr handle)
            : base(handle)
        { }

        /// <summary>
        /// Creates a new handle using the specified value. If owned is set to false, the 
        /// handle will not be closed automatically.
        /// </summary>
        /// <param name="handle">The handle value.</param>
        /// <param name="owned">Specifies whether the handle will be closed automatically.</param>
        public Win32Handle(IntPtr handle, bool owned)
            : base(handle, owned)
        { }

        /// <summary>
        /// Creates a new handle by duplicating an existing handle.
        /// </summary>
        /// <param name="handle">The existing handle.</param>
        /// <param name="desiredAccess">The desired access to the object.</param>
        public Win32Handle(IntPtr handle, int desiredAccess)
            : base(handle, desiredAccess)
        { }

        /// <summary>
        /// Creates a new handle by duplicating an existing handle from another process.
        /// </summary>
        /// <param name="processHandle">A handle to a process. It must have the PROCESS_DUP_HANDLE permission.</param>
        /// <param name="handle">The existing handle.</param>
        /// <param name="desiredAccess">The desired access to the object.</param>
        public Win32Handle(ProcessHandle processHandle, IntPtr handle, int desiredAccess)
            : base(processHandle, handle, desiredAccess)
        { }
    }

    /// <summary>
    /// Represents a generic Windows handle.
    /// </summary>
    public class Win32Handle<TAccess> : ISynchronizable, IDisposable
        where TAccess : struct
    {
        private object _disposeLock = new object();
        private bool _owned = true;
        private bool _disposed = false;
        private IntPtr _handle;

        public static implicit operator int(Win32Handle<TAccess> handle)
        {
            return handle.Handle.ToInt32();
        }

        public static implicit operator IntPtr(Win32Handle<TAccess> handle)
        {
            return handle.Handle;
        }

        /// <summary>
        /// Creates a new, invalid handle. You must set the handle using the Handle property.
        /// </summary>
        protected Win32Handle()
        { }

        /// <summary>
        /// Creates a new handle using the specified value. The handle will be closed when 
        /// this object is disposed or garbage-collected.
        /// </summary>
        /// <param name="handle">The handle value.</param>
        public Win32Handle(IntPtr handle)
        {
            _handle = handle;
        }

        /// <summary>
        /// Creates a new handle using the specified value. If owned is set to false, the 
        /// handle will not be closed automatically.
        /// </summary>
        /// <param name="handle">The handle value.</param>
        /// <param name="owned">Specifies whether the handle will be closed automatically.</param>
        public Win32Handle(IntPtr handle, bool owned)
        {
            _handle = handle;
            _owned = owned;
        }

        /// <summary>
        /// Creates a new handle by duplicating an existing handle.
        /// </summary>
        /// <param name="handle">The existing handle.</param>
        /// <param name="desiredAccess">The desired access to the object.</param>
        public Win32Handle(IntPtr handle, TAccess access)
        {
            Win32.DuplicateObject(ProcessHandle.GetCurrent(), handle, ProcessHandle.GetCurrent(), out _handle,
                (int)Convert.ChangeType(access, typeof(int)), 0, 0);
            _owned = true;
        }

        /// <summary>
        /// Creates a new handle by duplicating an existing handle from another process.
        /// </summary>
        /// <param name="processHandle">A handle to a process. It must have the PROCESS_DUP_HANDLE permission.</param>
        /// <param name="handle">The existing handle.</param>
        /// <param name="desiredAccess">The desired access to the object.</param>
        public Win32Handle(ProcessHandle processHandle, IntPtr handle, TAccess access)
        {
            Win32.DuplicateObject(processHandle, handle, ProcessHandle.GetCurrent(), out _handle,
                (int)Convert.ChangeType(access, typeof(int)), 0, 0);
            _owned = true;
        }

        /// <summary>
        /// Gets whether this handle is closed.
        /// </summary>
        public bool Disposed
        {
            get { return _disposed; }
        }

        /// <summary>
        /// Gets whether the handle will be automatically closed.
        /// </summary>
        public bool Owned
        {
            get { return _owned; }
        }

        /// <summary>
        /// Gets the handle value.
        /// </summary>
        public IntPtr Handle
        {
            get { return _handle; }
            protected set { _handle = value; }
        }

        /// <summary>
        /// Duplicates the handle.
        /// </summary>
        /// <param name="desiredAccess">The desired access to the object.</param>
        /// <returns>A handle.</returns>
        public Win32Handle<TAccess> Duplicate(TAccess access)
        {
            return new Win32Handle<TAccess>(ProcessHandle.GetCurrent(), this, access);
        }

        /// <summary>
        /// Gets certain information about the handle.
        /// </summary>
        /// <returns>A HANDLE_FLAGS value.</returns>
        public Win32HandleFlags GetHandleInformation()
        {
            Win32HandleFlags flags;

            if (!Win32.GetHandleInformation(this, out flags))
                Win32.ThrowLastError();

            return flags;
        }

        /// <summary>
        /// Gets the handle's name.
        /// </summary>
        /// <returns>A string.</returns>
        public string GetHandleName()
        {
            NtStatus status;
            int retLength;

            status = Win32.NtQueryObject(this, ObjectInformationClass.ObjectNameInformation,
                  IntPtr.Zero, 0, out retLength);

            if (retLength > 0)
            {
                using (MemoryAlloc oniMem = new MemoryAlloc(retLength))
                {
                    if ((status = Win32.NtQueryObject(this, ObjectInformationClass.ObjectNameInformation,
                        oniMem, oniMem.Size, out retLength)) >= NtStatus.Error)
                        Win32.ThrowLastError(status);

                    ObjectNameInformation oni = oniMem.ReadStruct<ObjectNameInformation>();

                    return oni.Name.Read();
                }
            }
            else
            {
                Win32.ThrowLastError(status);
            }

            return null;
        }

        /// <summary>
        /// Makes the object referenced by the handle permanent.
        /// </summary>
        public void MakePermanent()
        {
            NtStatus status;

            if ((status = Win32.NtMakePermanentObject(this)) >= NtStatus.Error)
                Win32.ThrowLastError(status);
        }

        /// <summary>
        /// Makes the object referenced by the handle temporary. The object 
        /// will be deleted once the last handle to it is closed.
        /// </summary>
        public void MakeTemporary()
        {
            NtStatus status;

            if ((status = Win32.NtMakeTemporaryObject(this)) >= NtStatus.Error)
                Win32.ThrowLastError(status);
        }

        /// <summary>
        /// Sets certain information about the handle.
        /// </summary>
        /// <param name="mask">Specifies which flags to set.</param>
        /// <param name="flags">The values of the flags to set.</param>
        public void SetHandleInformation(Win32HandleFlags mask, Win32HandleFlags flags)
        {
            if (!Win32.SetHandleInformation(this, mask, flags))
                Win32.ThrowLastError();
        }

        /// <summary>
        /// Signals the object and waits for another.
        /// </summary>
        public NtStatus SignalAndWait(ISynchronizable waitObject, bool alertable, long timeout)
        {
            return Win32.NtSignalAndWaitForSingleObject(this, waitObject.Handle, alertable, ref timeout);
        }

        /// <summary>
        /// Waits for the object.
        /// </summary>
        public NtStatus Wait()
        {
            return this.Wait(-1);
        }

        public NtStatus Wait(long timeout)
        {
            return this.Wait(false, timeout);
        }

        public NtStatus Wait(bool alertable, long timeout)
        {
            NtStatus status;

            if ((status = Win32.NtWaitForSingleObject(
                this,
                alertable,
                ref timeout
                )) >= NtStatus.Error)
                Win32.ThrowLastError(status);

            return status;
        }

        /// <summary>
        /// Closes the handle. This method must not be called directly; instead, 
        /// override this method in a derived class if your handle must be closed 
        /// with a method other than CloseHandle.
        /// </summary>
        protected virtual void Close()
        {
            Win32.CloseHandle(_handle);
        }

        ~Win32Handle()
        {
            this.Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                    Monitor.Enter(_disposeLock);

                if (!_disposed && _owned)
                {
                    this.Close();
                    _disposed = true;
                }
            }
            finally
            {
                if (disposing)
                    Monitor.Exit(_disposeLock);
            }
        }

        /// <summary>
        /// Closes the handle.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
