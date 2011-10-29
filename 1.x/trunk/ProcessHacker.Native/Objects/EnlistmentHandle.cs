/*
 * Process Hacker - 
 *   enlistment handle
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
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Security;

namespace ProcessHacker.Native.Objects
{
    public class EnlistmentHandle : NativeHandle<EnlistmentAccess>
    {
        public static EnlistmentHandle Create(
            EnlistmentAccess access,
            string name,
            ObjectFlags objectFlags,
            DirectoryHandle rootDirectory,
            ResourceManagerHandle resourceManagerHandle,
            TransactionHandle transactionHandle,
            EnlistmentOptions createOptions,
            NotificationMask notificationMask,
            IntPtr enlistmentKey
            )
        {
            ObjectAttributes oa = new ObjectAttributes(name, objectFlags, rootDirectory);
            IntPtr handle;

            try
            {
                Win32.NtCreateEnlistment(
                    out handle,
                    access,
                    resourceManagerHandle,
                    transactionHandle,
                    ref oa,
                    createOptions,
                    notificationMask,
                    enlistmentKey
                    ).ThrowIf();
            }
            finally
            {
                oa.Dispose();
            }

            return new EnlistmentHandle(handle, true);
        }

        public static EnlistmentHandle FromHandle(IntPtr handle)
        {
            return new EnlistmentHandle(handle, false);
        }

        private EnlistmentHandle(IntPtr handle, bool owned)
            : base(handle, owned)
        { }

        public EnlistmentHandle(
            string name,
            ObjectFlags objectFlags,
            DirectoryHandle rootDirectory,
            ResourceManagerHandle resourceManagerHandle,
            Guid guid,
            EnlistmentAccess access
            )
        {
            ObjectAttributes oa = new ObjectAttributes(name, objectFlags, rootDirectory);
            IntPtr handle;

            try
            {
                Win32.NtOpenEnlistment(
                    out handle,
                    access,
                    resourceManagerHandle,
                    ref guid,
                    ref oa
                    ).ThrowIf();
            }
            finally
            {
                oa.Dispose();
            }

            this.Handle = handle;
        } 

        public void Commit(long virtualClock)
        {
            Win32.NtCommitEnlistment(this, ref virtualClock).ThrowIf();
        }

        public void CommitComplete(long virtualClock)
        {
            Win32.NtCommitComplete(this, ref virtualClock).ThrowIf();
        }

        public EnlistmentBasicInformation BasicInformation
        {
            get
            {
                EnlistmentBasicInformation basicInfo;
                int retLength;

                Win32.NtQueryInformationEnlistment(
                    this,
                    EnlistmentInformationClass.EnlistmentBasicInformation,
                    out basicInfo,
                    EnlistmentBasicInformation.SizeOf,
                    out retLength
                    ).ThrowIf();

                return basicInfo;
            }
        }

        public void Prepare(long virtualClock)
        {
            Win32.NtPrepareEnlistment(this, ref virtualClock).ThrowIf();
        }

        public void PrepareComplete(long virtualClock)
        {
            Win32.NtPrepareComplete(this, ref virtualClock).ThrowIf();
        }

        public void PrePrepare(long virtualClock)
        {
            Win32.NtPrePrepareEnlistment(this, ref virtualClock).ThrowIf();
        }

        public void PrePrepareComplete(long virtualClock)
        {
            Win32.NtPrePrepareComplete(this, ref virtualClock).ThrowIf();
        }

        public void ReadOnly(long virtualClock)
        {
            Win32.NtReadOnlyEnlistment(this, ref virtualClock).ThrowIf();
        }

        public void Recover(IntPtr enlistmentKey)
        {
            Win32.NtRecoverEnlistment(this, enlistmentKey).ThrowIf();
        }

        public void RejectSinglePhase(long virtualClock)
        {
            Win32.NtSinglePhaseReject(this, ref virtualClock).ThrowIf();
        }

        public void Rollback(long virtualClock)
        {
            Win32.NtRollbackEnlistment(this, ref virtualClock).ThrowIf();
        }

        public void RollbackComplete(long virtualClock)
        {
            Win32.NtRollbackComplete(this, ref virtualClock).ThrowIf();
        }
    }
}
