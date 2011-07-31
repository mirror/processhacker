/*
 * Process Hacker - 
 *   job handle
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
using System.Runtime.InteropServices;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Security;

namespace ProcessHacker.Native.Objects
{
    /// <summary>
    /// Represents a handle to a Windows job object.
    /// </summary>
    public sealed class JobObjectHandle : NativeHandle<JobObjectAccess>
    {
        public static JobObjectHandle Create(JobObjectAccess access)
        {
            return Create(access, null);
        }

        public static JobObjectHandle Create(JobObjectAccess access, string name)
        {
            return Create(access, name, 0, null);
        }

        public static JobObjectHandle Create(JobObjectAccess access, string name, ObjectFlags objectFlags, DirectoryHandle rootDirectory)
        {
            NtStatus status;
            ObjectAttributes oa = new ObjectAttributes(name, objectFlags, rootDirectory);
            IntPtr handle;

            try
            {
                if ((status = Win32.NtCreateJobObject(
                    out handle,
                    access,
                    ref oa
                    )) >= NtStatus.Error)
                    Win32.Throw(status);
            }
            finally
            {
                oa.Dispose();
            }

            return new JobObjectHandle(handle, true);
        }

        internal static IntPtr Open(ProcessHandle processHandle, JobObjectAccess access)
        {
            try
            {
                return new IntPtr(KProcessHacker.Instance.KphOpenProcessJob(processHandle, access));
            }
            catch (WindowsException)
            {
                IntPtr handle;

                // Use KPH to set the handle's granted access.
                handle = new IntPtr(KProcessHacker.Instance.KphOpenProcessJob(processHandle,
                    (JobObjectAccess)StandardRights.Synchronize));
                if (handle != IntPtr.Zero)
                    KProcessHacker.Instance.KphSetHandleGrantedAccess(handle, (int)access);

                return handle;
            }
        }

        /// <summary>
        /// Creates a service handle using an existing handle. 
        /// The handle will not be closed automatically.
        /// </summary>
        /// <param name="handle">The handle value.</param>
        /// <returns>The job handle.</returns>
        public static JobObjectHandle FromHandle(IntPtr handle)
        {
            return new JobObjectHandle(handle, false);
        }

        internal JobObjectHandle(IntPtr handle, bool owned)
            : base(handle, owned)
        { }

        public JobObjectHandle(string name, JobObjectAccess access)
            : this(name, 0, null, access)
        { }

        public JobObjectHandle(string name, ObjectFlags objectFlags, DirectoryHandle rootDirectory, JobObjectAccess access)
        {
            NtStatus status;
            ObjectAttributes oa = new ObjectAttributes(name, objectFlags, rootDirectory);
            IntPtr handle;

            try
            {
                if ((status = Win32.NtOpenJobObject(
                    out handle,
                    access,
                    ref oa
                    )) >= NtStatus.Error)
                    Win32.Throw(status);
            }
            finally
            {
                oa.Dispose();
            }

            this.Handle = handle;
        }

        /// <summary>
        /// Opens the job object associated with the specified process.
        /// </summary>
        /// <param name="processHandle">The process.</param>
        /// <param name="access">The desired access to the job object.</param>
        public JobObjectHandle(ProcessHandle processHandle, JobObjectAccess access)
        {
            this.Handle = Open(processHandle, access);

            // If we don't have a handle assume the process isn't in a job.
            if (this.Handle == IntPtr.Zero)
            {
                this.MarkAsInvalid();
                Win32.Throw(NtStatus.ProcessNotInJob);
            }
        }

        private T QueryStruct<T>(JobObjectInformationClass informationClass)
            where T : struct
        {
            int retLength;

            using (MemoryAlloc data = new MemoryAlloc(Marshal.SizeOf(typeof(T))))
            {
                if (!Win32.QueryInformationJobObject(this, informationClass, data, data.Size, out retLength))
                {
                    data.ResizeNew(retLength);

                    if (!Win32.QueryInformationJobObject(this, informationClass, data, data.Size, out retLength))
                        Win32.Throw();
                }

                return data.ReadStruct<T>();
            }
        }

        public JobObjectBasicAccountingInformation GetBasicAccountingInformation()
        {
            return this.QueryStruct<JobObjectBasicAccountingInformation>(
                JobObjectInformationClass.JobObjectBasicAccountingInformation);
        }

        public JobObjectBasicAndIoAccountingInformation GetBasicAndIoAccountingInformation()
        {
            return this.QueryStruct<JobObjectBasicAndIoAccountingInformation>(
                JobObjectInformationClass.JobObjectBasicAndIoAccountingInformation);
        }

        public JobObjectBasicLimitInformation GetBasicLimitInformation()
        {
            return this.QueryStruct<JobObjectBasicLimitInformation>(JobObjectInformationClass.JobObjectBasicLimitInformation);
        }

        public int[] GetProcessIdList()
        {
            List<int> processIds = new List<int>();
            int retLength;

            // FIXME: Fixed buffer
            using (MemoryAlloc data = new MemoryAlloc(0x1000))
            {
                if (!Win32.QueryInformationJobObject(this, JobObjectInformationClass.JobObjectBasicProcessIdList,
                    data, data.Size, out retLength))
                    Win32.Throw();

                JobObjectBasicProcessIdList listInfo = data.ReadStruct<JobObjectBasicProcessIdList>();

                for (int i = 0; i < listInfo.NumberOfProcessIdsInList; i++)
                {
                    processIds.Add(data.ReadInt32(8, i));
                }
            }

            return processIds.ToArray();
        }

        public JobObjectBasicUiRestrictions GetBasicUiRestrictions()
        {
            JobObjectBasicUiRestrictions uiRestrictions;
            int retLength;

            if (!Win32.QueryInformationJobObject(this, JobObjectInformationClass.JobObjectBasicUIRestrictions,
                out uiRestrictions, 4, out retLength))
                Win32.Throw();

            return uiRestrictions;
        }

        public JobObjectExtendedLimitInformation GetExtendedLimitInformation()
        {
            return this.QueryStruct<JobObjectExtendedLimitInformation>(JobObjectInformationClass.JobObjectExtendedLimitInformation);
        }

        public void Terminate()
        {
            this.Terminate(0);
        }

        public void Terminate(int exitCode)
        {
            if (!Win32.TerminateJobObject(this, exitCode))
                Win32.Throw();
        }
    }
}
