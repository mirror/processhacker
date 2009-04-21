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
    public class JobObjectHandle : Win32Handle<JobObjectAccess>
    {
        /// <summary>
        /// Creates a service handle using an existing handle. 
        /// The handle will not be closed automatically.
        /// </summary>
        /// <param name="Handle">The handle value.</param>
        /// <returns>The job handle.</returns>
        public static JobObjectHandle FromHandle(int handle)
        {
            return new JobObjectHandle(handle, false);
        }

        public static JobObjectHandle Create(string name)
        {
            int jobHandle = Win32.CreateJobObject(0, name);

            if (jobHandle == 0)
                Win32.ThrowLastWin32Error();

            return new JobObjectHandle(jobHandle, true);
        }

        private JobObjectHandle(int handle, bool owned)
            : base(handle, owned)
        { }

        /// <summary>
        /// Opens a job by its name.
        /// </summary>
        /// <param name="name">The job name.</param>
        /// <param name="access">The desired access to the job object.</param>
        public JobObjectHandle(string name, JobObjectAccess access)
        {
            this.Handle = Win32.OpenJobObject(access, false, name);

            if (this.Handle == 0)
                Win32.ThrowLastWin32Error();
        }

        /// <summary>
        /// Opens the job associated with the specified process.
        /// </summary>
        /// <param name="processHandle">The process.</param>
        /// <param name="access">The desired access to the job object.</param>
        public JobObjectHandle(ProcessHandle processHandle, JobObjectAccess access)
        {
            this.Handle = KProcessHacker.Instance.KphOpenProcessJob(processHandle, access);

            if (this.Handle == 0)
                Win32.ThrowLastWin32Error();
        }

        private T QueryStruct<T>(JobObjectInformationClass informationClass)
        {
            int retLength;

            using (MemoryAlloc data = new MemoryAlloc(Marshal.SizeOf(typeof(T))))
            {
                if (!Win32.QueryInformationJobObject(this, informationClass, data, data.Size, out retLength))
                {
                    data.Resize(retLength);

                    if (!Win32.QueryInformationJobObject(this, informationClass, data, data.Size, out retLength))
                        Win32.ThrowLastWin32Error();
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
                    Win32.ThrowLastWin32Error();

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
                Win32.ThrowLastWin32Error();

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
                Win32.ThrowLastWin32Error();
        }
    }

    [Flags]
    public enum JobObjectInformationClass : int
    {
        JobObjectBasicAccountingInformation = 1,
        JobObjectBasicLimitInformation = 2,
        JobObjectBasicProcessIdList = 3,
        JobObjectBasicUIRestrictions = 4,
        JobObjectSecurityLimitInformation = 5,
        JobObjectBasicAndIoAccountingInformation = 8,
        JobObjectExtendedLimitInformation = 9,
        JobObjectGroupInformation = 11
    }

    [Flags]
    public enum JobObjectLimitFlags : uint
    {
        WorkingSet = 0x1,
        ProcessTime = 0x2,
        JobTime = 0x4,
        ActiveProcess = 0x8,
        Affinity = 0x10,
        PriorityClass = 0x20,
        PreserveJobTime = 0x40,
        SchedulingClass = 0x80,
        ProcessMemory = 0x100,
        JobMemory = 0x200,
        DieOnUnhandledException = 0x400,
        BreakawayOk = 0x800,
        SilentBreakawayOk = 0x1000,
        KillOnJobClose = 0x2000,
    }

    [Flags]
    public enum JobObjectBasicUiRestrictions : uint
    {
        Handles = 0x1,
        ReadClipboard = 0x2,
        WriteClipboard = 0x4,
        SystemParameters = 0x8,
        DisplaySettings = 0x10,
        GlobalAtoms = 0x20,
        Desktop = 0x40,
        ExitWindows = 0x80
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct JobObjectBasicAccountingInformation
    {
        public long TotalUserTime;
        public long TotalKernelTime;
        public long ThisPeriodTotalUserTime;
        public long ThisPeriodTotalKernelTime;
        public int TotalPageFaultCount;
        public int TotalProcesses;
        public int ActiveProcesses;
        public int TotalTerminatedProcesses;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct JobObjectBasicAndIoAccountingInformation
    {
        public JobObjectBasicAccountingInformation BasicInfo;
        public IoCounters IoInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct JobObjectBasicLimitInformation
    {
        public long PerProcessUserTimeLimit;
        public long PerJobUserTimeLimit;
        public JobObjectLimitFlags LimitFlags;
        public int MinimumWorkingSetSize;
        public int MaximumWorkingSetSize;
        public int ActiveProcessLimit;
        public int Affinity;
        public int PriorityClass;
        public int SchedulingClass;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct JobObjectBasicProcessIdList
    {
        public int NumberOfAssignedProcesses;
        public int NumberOfProcessIdsInList;
        /* an array follows */
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct JobObjectEndOfJobTimeInformation
    {
        public int EndOfJobTimeAction; // 0: Terminate, 1: Post
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct JobObjectExtendedLimitInformation
    {
        public JobObjectBasicLimitInformation BasicLimitInformation;
        public IoCounters IoInfo;
        public int ProcessMemoryLimit;
        public int JobMemoryLimit;
        public int PeakProcessMemoryUsed;
        public int PeakJobMemoryUsed;
    }
}
