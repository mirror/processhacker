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
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace ProcessHacker
{
    public partial class Win32
    {
        /// <summary>
        /// Represents a handle to a Windows job object.
        /// </summary>
        public class JobHandle : Win32Handle
        {
            /// <summary>
            /// Creates a service handle using an existing handle. 
            /// The handle will not be closed automatically.
            /// </summary>
            /// <param name="Handle">The handle value.</param>
            /// <returns>The job handle.</returns>
            public static JobHandle FromHandle(int Handle)
            {
                return new JobHandle(Handle, false);
            }

            private JobHandle(int Handle, bool Owned)
                : base(Handle, Owned)
            { }

            /// <summary>
            /// Opens a job by its name.
            /// </summary>
            /// <param name="name">The job name.</param>
            /// <param name="access">The desired access to the job object.</param>
            public JobHandle(string name, JOB_OBJECT_RIGHTS access)
            {
                this.Handle = OpenJobObject(access, false, name);

                if (this.Handle == 0)
                    ThrowLastWin32Error();
            }

            /// <summary>
            /// Opens the job associated with the specified process.
            /// </summary>
            /// <param name="processHandle">The process.</param>
            /// <param name="access">The desired access to the job object.</param>
            public JobHandle(ProcessHandle processHandle, JOB_OBJECT_RIGHTS access)
            {
                this.Handle = Program.KPH.KphOpenProcessJob(processHandle, access);

                if (this.Handle == 0)
                    ThrowLastWin32Error();
            }

            private T QueryStruct<T>(JOB_OBJECT_INFORMATION_CLASS informationClass)
            {
                int retLength;

                using (MemoryAlloc data = new MemoryAlloc(Marshal.SizeOf(typeof(T))))
                {
                    if (!QueryInformationJobObject(this, informationClass, data, data.Size, out retLength))
                    {
                        data.Resize(retLength);

                        if (!QueryInformationJobObject(this, informationClass, data, data.Size, out retLength))
                            ThrowLastWin32Error();
                    }

                    return data.ReadStruct<T>();
                }
            }

            public JOBOBJECT_BASIC_ACCOUNTING_INFORMATION GetBasicAccountingInformation()
            {
                return this.QueryStruct<JOBOBJECT_BASIC_ACCOUNTING_INFORMATION>(
                    JOB_OBJECT_INFORMATION_CLASS.JobObjectBasicAccountingInformation);
            }

            public JOBOBJECT_BASIC_AND_IO_ACCOUNTING_INFORMATION GetBasicAndIoAccountingInformation()
            {
                return this.QueryStruct<JOBOBJECT_BASIC_AND_IO_ACCOUNTING_INFORMATION>(
                    JOB_OBJECT_INFORMATION_CLASS.JobObjectBasicAndIoAccountingInformation);
            }

            public JOBOBJECT_BASIC_LIMIT_INFORMATION GetBasicLimitInformation()
            {
                return this.QueryStruct<JOBOBJECT_BASIC_LIMIT_INFORMATION>(JOB_OBJECT_INFORMATION_CLASS.JobObjectBasicLimitInformation);
            }

            public int[] GetProcessIdList()
            {
                List<int> processIds = new List<int>();
                int retLength;

                // FIXME: Fixed buffer
                using (MemoryAlloc data = new MemoryAlloc(0x1000))
                {
                    if (!QueryInformationJobObject(this, JOB_OBJECT_INFORMATION_CLASS.JobObjectBasicProcessIdList, 
                        data, data.Size, out retLength))
                        ThrowLastWin32Error();

                    JOBOBJECT_BASIC_PROCESS_ID_LIST listInfo = data.ReadStruct<JOBOBJECT_BASIC_PROCESS_ID_LIST>();

                    for (int i = 0; i < listInfo.NumberOfProcessIdsInList; i++)
                    {
                        processIds.Add(data.ReadInt32(8, i));
                    }
                }

                return processIds.ToArray();
            }

            public JOB_OBJECT_BASIC_UI_RESTRICTIONS GetBasicUiRestrictions()
            {
                JOB_OBJECT_BASIC_UI_RESTRICTIONS uiRestrictions;
                int retLength;

                if (!QueryInformationJobObject(this, JOB_OBJECT_INFORMATION_CLASS.JobObjectBasicUIRestrictions,
                    out uiRestrictions, 4, out retLength))
                    ThrowLastWin32Error();

                return uiRestrictions;
            }

            public JOBOBJECT_EXTENDED_LIMIT_INFORMATION GetExtendedLimitInformation()
            {
                return this.QueryStruct<JOBOBJECT_EXTENDED_LIMIT_INFORMATION>(JOB_OBJECT_INFORMATION_CLASS.JobObjectExtendedLimitInformation);
            }
        }
    }
}
