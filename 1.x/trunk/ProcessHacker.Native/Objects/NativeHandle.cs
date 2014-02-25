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
using ProcessHacker.Common.Objects;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Security;
using ProcessHacker.Native.Security.AccessControl;

namespace ProcessHacker.Native.Objects
{
    /// <summary>
    /// Represents a generic Windows handle which acts as a kernel handle by default.
    /// </summary>
    public class NativeHandle : BaseObject, IEquatable<NativeHandle>, ISecurable, ISynchronizable
    {
        public static IntPtr Invalid
        {
            get { return IntPtr.Zero; }
        }

        public static IntPtr MinusOne
        {
            get { return (-1).ToIntPtr(); }
        }

        public static bool IsInvalid(IntPtr handle)
        {
            return handle == Invalid;
        }

        public static NtStatus WaitAll(ISynchronizable[] objects)
        {
            return WaitAll(objects, false, long.MinValue, false);
        }

        public static NtStatus WaitAll(ISynchronizable[] objects, long timeout)
        {
            return WaitAll(objects, false, timeout);
        }

        public static NtStatus WaitAll(ISynchronizable[] objects, bool alertable, long timeout)
        {
            return WaitAll(objects, alertable, timeout, true);
        }

        public static NtStatus WaitAll(ISynchronizable[] objects, bool alertable, long timeout, bool relative)
        {
            return WaitForMultipleObjects(objects, WaitType.WaitAll, alertable, timeout, relative);
        }

        public static NtStatus WaitAny(ISynchronizable[] objects)
        {
            return WaitAny(objects, false, long.MinValue, false);
        }

        public static NtStatus WaitAny(ISynchronizable[] objects, long timeout)
        {
            return WaitAny(objects, false, timeout);
        }

        public static NtStatus WaitAny(ISynchronizable[] objects, bool alertable, long timeout)
        {
            return WaitAny(objects, alertable, timeout, true);
        }

        public static NtStatus WaitAny(ISynchronizable[] objects, bool alertable, long timeout, bool relative)
        {
            return WaitForMultipleObjects(objects, WaitType.WaitAny, alertable, timeout, relative);
        }

        private static NtStatus WaitForMultipleObjects(ISynchronizable[] objects, WaitType waitType, bool alertable, long timeout, bool relative)
        {
            NtStatus status;
            IntPtr[] handles = new IntPtr[objects.Length];
            long realTimeout = relative ? -timeout : timeout;

            for (int i = 0; i < objects.Length; i++)
                handles[i] = objects[i].Handle;

            if ((status = Win32.NtWaitForMultipleObjects(
                handles.Length,
                handles,
                waitType,
                alertable,
                ref realTimeout
                )) >= NtStatus.Error)
                Win32.Throw(status);

            return status;
        }

        public static implicit operator int(NativeHandle handle)
        {
            return handle.Handle.ToInt32();
        }

        public static implicit operator IntPtr(NativeHandle handle)
        {
            return handle.Handle;
        }

        private IntPtr _handle;

        /// <summary>
        /// Creates a new, invalid handle. You must set the handle using the Handle property.
        /// </summary>
        protected NativeHandle()
        { }

        /// <summary>
        /// Creates a new handle using the specified value. The handle will be closed when 
        /// this object is disposed or garbage-collected.
        /// </summary>
        /// <param name="handle">The handle value.</param>
        public NativeHandle(IntPtr handle)
        {
            _handle = handle;
        }

        /// <summary>
        /// Creates a new handle using the specified value. If owned is set to false, the 
        /// handle will not be closed automatically.
        /// </summary>
        /// <param name="handle">The handle value.</param>
        /// <param name="owned">Specifies whether the handle will be closed automatically.</param>
        public NativeHandle(IntPtr handle, bool owned)
            : base(owned)
        {
            _handle = handle;
        }

        protected sealed override void DisposeObject(bool disposing)
        {
            this.Close();
            _handle = IntPtr.Zero;
        }

        /// <summary>
        /// Closes the handle. This method must not be called directly; instead, 
        /// override this method in a derived class if your handle must be closed 
        /// with a method other than CloseHandle.
        /// </summary>
        protected virtual void Close()
        {
            if (_handle != IntPtr.Zero && _handle.ToInt32() != -1 && _handle.ToInt32() != -2)
                Win32.NtClose(_handle);
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
        /// Determines if the specified object is equal to the current handle.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>Whether the two objects are equal.</returns>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as NativeHandle);
        }

        /// <summary>
        /// Determines if the specified handle is equal to the current handle.
        /// </summary>
        /// <param name="obj">The handle to compare.</param>
        /// <returns>Whether the two handles are equal.</returns>
        public bool Equals(NativeHandle obj)
        {
            if (obj == null)
                return false;
            return obj.Handle == this.Handle;
        }

        /// <summary>
        /// Gets certain information about the handle.
        /// </summary>
        /// <returns>A HANDLE_FLAGS value.</returns>
        public virtual Win32HandleFlags GetHandleFlags()
        {
            Win32HandleFlags flags;

            if (!Win32.GetHandleInformation(this, out flags))
                Win32.Throw();

            return flags;
        }

        /// <summary>
        /// Gets a unique hash code for the handle.
        /// </summary>
        /// <returns>A hash code.</returns>
        public override int GetHashCode()
        {
            return _handle.ToInt32();
        }

        /// <summary>
        /// Gets the handle's name.
        /// </summary>
        /// <returns>A string.</returns>
        public virtual string GetObjectName()
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
                        Win32.Throw(status);

                    var oni = oniMem.ReadStruct<ObjectNameInformation>();

                    return oni.Name.Read();
                }
            }
            else
            {
                Win32.Throw(status);
            }

            return null;
        }

        /// <summary>
        /// Gets the handle's type name.
        /// </summary>
        /// <returns>A string.</returns>
        public virtual string GetObjectTypeName()
        {
            NtStatus status;
            int retLength;

            status = Win32.NtQueryObject(this, ObjectInformationClass.ObjectTypeInformation,
                  IntPtr.Zero, 0, out retLength);

            if (retLength > 0)
            {
                using (MemoryAlloc otiMem = new MemoryAlloc(retLength))
                {
                    if ((status = Win32.NtQueryObject(this, ObjectInformationClass.ObjectTypeInformation,
                        otiMem, otiMem.Size, out retLength)) >= NtStatus.Error)
                        Win32.Throw(status);

                    var oni = otiMem.ReadStruct<ObjectTypeInformation>();

                    return oni.Name.Read();
                }
            }
            else
            {
                Win32.Throw(status);
            }

            return null;
        }

        /// <summary>
        /// Gets the security descriptor of the object.
        /// </summary>
        /// <param name="securityInformation">The information to retrieve.</param>
        /// <returns>A security descriptor.</returns>
        public virtual SecurityDescriptor GetSecurity(SecurityInformation securityInformation)
        {
            return SecurityDescriptor.GetSecurity(this, securityInformation);
        }

        /// <summary>
        /// Gets the security descriptor of the object.
        /// </summary>
        /// <param name="objectType">The type of the object.</param>
        /// <param name="securityInformation">The information to retrieve.</param>
        /// <returns>A security descriptor.</returns>
        protected SecurityDescriptor GetSecurity(SeObjectType objectType, SecurityInformation securityInformation)
        {
            return SecurityDescriptor.GetSecurity(this, objectType, securityInformation);
        }

        /// <summary>
        /// Makes the object referenced by the handle permanent.
        /// </summary>
        public virtual void MakeObjectPermanent()
        {
            NtStatus status;

            if ((status = Win32.NtMakePermanentObject(this)) >= NtStatus.Error)
                Win32.Throw(status);
        }

        /// <summary>
        /// Makes the object referenced by the handle temporary. The object 
        /// will be deleted once the last handle to it is closed. This function 
        /// requires Delete access.
        /// </summary>
        public virtual void MakeObjectTemporary()
        {
            NtStatus status;

            if ((status = Win32.NtMakeTemporaryObject(this)) >= NtStatus.Error)
                Win32.Throw(status);
        }

        /// <summary>
        /// Marks the handle as invalid. This method must only be called from 
        /// within a derived class constructor.
        /// </summary>
        protected void MarkAsInvalid()
        {
            this.DisableOwnership(false);
        }

        /// <summary>
        /// Sets certain information about the handle.
        /// </summary>
        /// <param name="mask">Specifies which flags to set.</param>
        /// <param name="flags">The values of the flags to set.</param>
        public virtual void SetHandleFlags(Win32HandleFlags mask, Win32HandleFlags flags)
        {
            if (!Win32.SetHandleInformation(this, mask, flags))
                Win32.Throw();
        }

        /// <summary>
        /// Sets the security descriptor of the object.
        /// </summary>
        /// <param name="securityInformation">The information to modify.</param>
        /// <param name="securityDescriptor">The security descriptor.</param>
        public virtual void SetSecurity(SecurityInformation securityInformation, SecurityDescriptor securityDescriptor)
        {
            SecurityDescriptor.SetSecurity(this, securityInformation, securityDescriptor);
        }

        /// <summary>
        /// Sets the security descriptor of the object.
        /// </summary>
        /// <param name="objectType">The type of the object.</param>
        /// <param name="securityInformation">The information to modify.</param>
        /// <param name="securityDescriptor">The security descriptor.</param>
        protected void SetSecurity(SeObjectType objectType, SecurityInformation securityInformation, SecurityDescriptor securityDescriptor)
        {
            SecurityDescriptor.SetSecurity(this, objectType, securityInformation, securityDescriptor);
        }

        /// <summary>
        /// Signals the object and waits for another.
        /// </summary>
        public virtual NtStatus SignalAndWait(ISynchronizable waitObject)
        {
            return this.SignalAndWait(waitObject, false);
        }

        /// <summary>
        /// Signals the object and waits for another.
        /// </summary>
        public virtual NtStatus SignalAndWait(ISynchronizable waitObject, bool alertable)
        {
            return this.SignalAndWait(waitObject, alertable, long.MinValue, false);
        }

        /// <summary>
        /// Signals the object and waits for another.
        /// </summary>
        public virtual NtStatus SignalAndWait(ISynchronizable waitObject, bool alertable, long timeout)
        {
            return this.SignalAndWait(waitObject, alertable, timeout, true);
        }

        /// <summary>
        /// Signals the object and waits for another.
        /// </summary>
        public virtual NtStatus SignalAndWait(ISynchronizable waitObject, bool alertable, long timeout, bool relative)
        {
            NtStatus status;
            long realTimeout = relative ? -timeout : timeout;

            if ((status = Win32.NtSignalAndWaitForSingleObject(
                this,
                waitObject.Handle,
                alertable,
                ref timeout
                )) >= NtStatus.Error)
                Win32.Throw(status);

            return status;
        }

        /// <summary>
        /// Closes the current handle and assigns a new handle to the NativeHandle instance.
        /// </summary>
        /// <param name="newHandle">The new handle value.</param>
        protected void SwapHandle(IntPtr newHandle)
        {
            if (!this.Owned || this.Disposed)
                throw new InvalidOperationException();

            this.Close();
            _handle = newHandle;
        }

        /// <summary>
        /// Gets a string that represents the handle.
        /// </summary>
        /// <returns>A string.</returns>
        public override string ToString()
        {
            return this.GetType().Name + ": " + _handle.ToString("x");
        }

        /// <summary>
        /// Waits for the object to be signaled.
        /// </summary>
        public virtual NtStatus Wait()
        {
            return this.Wait(false);
        }

        /// <summary>
        /// Waits for the object to be signaled.
        /// </summary>
        /// <param name="alertable">
        /// Whether user-mode APCs can be delivered during the wait.
        /// </param>
        public virtual NtStatus Wait(bool alertable)
        {
            /* Note that in order to wait for an infinite amount of time 
             * NULL should be passed as the timeout parameter to 
             * KeWaitForSingleObject/MultipleObjects. However, 
             * long.MinValue = -9223372036854775808 
             * = 9223372036854775808 100ns (relative)
             * = 922337203685477580.8 microseconds
             * = 922337203685477.5808 ms
             * = 922337203685.4775808 s
             * = 15372286728.091293013333333333333 minutes
             * = 256204778.80152155022222222222222 hours
             * = 10675199.116730064592592592592593 days
             * = 7306.7755761328299743960250462646 4 years (including one leap year)
             * = 29227.102304531319897584100185058 years (average)
             * = 29.227102304531319897584100185058 millennia
             * That's long enough, I think...
             */
            return this.Wait(alertable, long.MinValue, false);
        }

        /// <summary>
        /// Waits for the object to be signaled.
        /// </summary>
        /// <param name="timeout">The timeout, in 100ns units.</param>
        public NtStatus Wait(long timeout)
        {
            return this.Wait(false, timeout);
        }

        /// <summary>
        /// Waits for the object to be signaled.
        /// </summary>
        /// <param name="alertable">
        /// Whether user-mode APCs can be delivered during the wait.
        /// </param>
        /// <param name="timeout">The timeout, in 100ns units.</param>
        public virtual NtStatus Wait(bool alertable, long timeout)
        {
            return this.Wait(alertable, timeout, true);
        }

        /// <summary>
        /// Waits for the object to be signaled.
        /// </summary>
        /// <param name="timeout">The timeout, in 100ns units.</param>
        /// <param name="relative">Whether the timeout value is relative.</param>
        public NtStatus Wait(long timeout, bool relative)
        {
            return this.Wait(false, timeout, relative);
        }

        /// <summary>
        /// Waits for the object to be signaled.
        /// </summary>
        /// <param name="alertable">
        /// Whether user-mode APCs can be delivered during the wait.
        /// </param>
        /// <param name="timeout">The timeout, in 100ns units.</param>
        /// <param name="relative">Whether the timeout value is relative.</param>
        public virtual NtStatus Wait(bool alertable, long timeout, bool relative)
        {
            NtStatus status;
            long realTimeout = relative ? -timeout : timeout;

            if ((status = Win32.NtWaitForSingleObject(
                this,
                alertable,
                ref realTimeout
                )) >= NtStatus.Error)
                Win32.Throw(status);

            return status;
        }
}

    /// <summary>
    /// Represents a generic Windows handle which acts as a kernel handle by default.
    /// </summary>
    public class NativeHandle<TAccess> : NativeHandle
        where TAccess : struct
    {
        /// <summary>
        /// Creates a new, invalid handle. You must set the handle using the Handle property.
        /// </summary>
        protected NativeHandle()
        { }

        /// <summary>
        /// Creates a new handle using the specified value. The handle will be closed when 
        /// this object is disposed or garbage-collected.
        /// </summary>
        /// <param name="handle">The handle value.</param>
        public NativeHandle(IntPtr handle)
            : base(handle)
        { }

        /// <summary>
        /// Creates a new handle using the specified value. If owned is set to false, the 
        /// handle will not be closed automatically.
        /// </summary>
        /// <param name="handle">The handle value.</param>
        /// <param name="owned">Specifies whether the handle will be closed automatically.</param>
        public NativeHandle(IntPtr handle, bool owned)
            : base(handle, owned)
        { }

        /// <summary>
        /// Creates a new handle by duplicating an existing handle.
        /// </summary>
        /// <param name="handle">The existing handle.</param>
        /// <param name="access">The desired access to the object.</param>
        public NativeHandle(IntPtr handle, TAccess access)
        {
            IntPtr newHandle;

            Win32.DuplicateObject(ProcessHandle.Current, handle, ProcessHandle.Current, out newHandle,
                (int)Convert.ChangeType(access, typeof(int)), 0, 0);
            this.Handle = newHandle;
        }

        /// <summary>
        /// Creates a new handle by duplicating an existing handle from another process.
        /// </summary>
        /// <param name="processHandle">A handle to a process. It must have the PROCESS_DUP_HANDLE permission.</param>
        /// <param name="handle">The existing handle.</param>
        /// <param name="access">The desired access to the object.</param>
        public NativeHandle(ProcessHandle processHandle, IntPtr handle, TAccess access)
        {
            IntPtr newHandle;

            Win32.DuplicateObject(processHandle, handle, ProcessHandle.Current, out newHandle,
                (int)Convert.ChangeType(access, typeof(int)), 0, 0);
            this.Handle = newHandle;
        }

        /// <summary>
        /// Attempts to duplicate the handle with different access rights.
        /// </summary>
        /// <param name="access">The new access rights.</param>
        public void ChangeAccess(TAccess access)
        {
            IntPtr newHandle;

            Win32.DuplicateObject(ProcessHandle.Current, this, ProcessHandle.Current, out newHandle,
                (int)Convert.ChangeType(access, typeof(int)), 0, 0);
            this.SwapHandle(newHandle);
        }

        /// <summary>
        /// Duplicates the handle.
        /// </summary>
        /// <param name="access">The desired access to the object.</param>
        /// <returns>A handle.</returns>
        public NativeHandle<TAccess> Duplicate(TAccess access)
        {
            return new NativeHandle<TAccess>(ProcessHandle.Current, this, access);
        }
    }

    /// <summary>
    /// Represents a generic Windows handle which acts as a kernel handle by default.
    /// </summary>
    public class GenericHandle : NativeHandle<int>
    {
        /// <summary>
        /// Creates a new, invalid handle. You must set the handle using the Handle property.
        /// </summary>
        protected GenericHandle()
            : base()
        { }

        /// <summary>
        /// Creates a new handle using the specified value. The handle will be closed when 
        /// this object is disposed or garbage-collected.
        /// </summary>
        /// <param name="handle">The handle value.</param>
        public GenericHandle(IntPtr handle)
            : base(handle)
        { }

        /// <summary>
        /// Creates a new handle using the specified value. If owned is set to false, the 
        /// handle will not be closed automatically.
        /// </summary>
        /// <param name="handle">The handle value.</param>
        /// <param name="owned">Specifies whether the handle will be closed automatically.</param>
        public GenericHandle(IntPtr handle, bool owned)
            : base(handle, owned)
        { }

        /// <summary>
        /// Creates a new handle by duplicating an existing handle.
        /// </summary>
        /// <param name="handle">The existing handle.</param>
        /// <param name="desiredAccess">The desired access to the object.</param>
        public GenericHandle(IntPtr handle, int desiredAccess)
            : base(handle, desiredAccess)
        { }

        /// <summary>
        /// Creates a new handle by duplicating an existing handle from another process.
        /// </summary>
        /// <param name="processHandle">A handle to a process. It must have the PROCESS_DUP_HANDLE permission.</param>
        /// <param name="handle">The existing handle.</param>
        /// <param name="desiredAccess">The desired access to the object.</param>
        public GenericHandle(ProcessHandle processHandle, IntPtr handle, int desiredAccess)
            : base(processHandle, handle, desiredAccess)
        { }
    }
}
