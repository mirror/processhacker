/*
 * Process Hacker - 
 *   transaction handle
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
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Security;

namespace ProcessHacker.Native.Objects
{
    public class TransactionHandle : NativeHandle<TransactionAccess>
    {
        public static TransactionHandle Create(
            TransactionAccess access,
            string name,
            ObjectFlags objectFlags,
            DirectoryHandle rootDirectory,
            Guid unitOfWorkGuid,
            TmHandle tmHandle,
            TransactionOptions createOptions,
            long timeout,
            string description
            )
        {
            NtStatus status;
            ObjectAttributes oa = new ObjectAttributes(name, objectFlags, rootDirectory);
            IntPtr handle;

            try
            {
                UnicodeString descriptionStr = new UnicodeString(description);

                try
                {
                    if ((status = Win32.NtCreateTransaction(
                        out handle,
                        access,
                        ref oa,
                        ref unitOfWorkGuid,
                        tmHandle ?? IntPtr.Zero,
                        createOptions,
                        0,
                        0,
                        ref timeout,
                        ref descriptionStr
                        )) >= NtStatus.Error)
                        Win32.ThrowLastError(status);
                }
                finally
                {
                    descriptionStr.Dispose();
                }
            }
            finally
            {
                oa.Dispose();
            }

            return new TransactionHandle(handle, true);
        }

        private TransactionHandle(IntPtr handle, bool owned)
            : base(handle, owned)
        { }

        public TransactionHandle(
            string name,
            ObjectFlags objectFlags,
            DirectoryHandle rootDirectory,
            Guid unitOfWorkGuid,
            TmHandle tmHandle,
            TransactionAccess access
            )
        {
            NtStatus status;
            ObjectAttributes oa = new ObjectAttributes(name, objectFlags, rootDirectory);
            IntPtr handle;

            try
            {
                if ((status = Win32.NtOpenTransaction(
                    out handle,
                    access,
                    ref oa,
                    ref unitOfWorkGuid,
                    tmHandle ?? IntPtr.Zero
                    )) >= NtStatus.Error)
                    Win32.ThrowLastError(status);
            }
            finally
            {
                oa.Dispose();
            }

            this.Handle = handle;
        }

        public static TransactionHandle FromHandle(IntPtr handle)
        {
            return new TransactionHandle(handle, false);
        }

        public void Commit(bool wait)
        {
            NtStatus status;

            if ((status = Win32.NtCommitTransaction(this, wait)) >= NtStatus.Error)
                Win32.ThrowLastError(status);
        }

        public TransactionBasicInformation GetBasicInformation()
        {
            NtStatus status;
            TransactionBasicInformation basicInfo;
            int retLength;

            if ((status = Win32.NtQueryInformationTransaction(
                this,
                TransactionInformationClass.TransactionBasicInformation,
                out basicInfo,
                Marshal.SizeOf(typeof(TransactionBasicInformation)),
                out retLength
                )) >= NtStatus.Error)
                Win32.ThrowLastError(status);

            return basicInfo;
        }

        public string GetDescription()
        {
            using (var data = this.GetPropertiesInformation())
            {
                var propertiesInfo = data.ReadStruct<TransactionPropertiesInformation>();

                return data.ReadUnicodeString(
                    TransactionPropertiesInformation.DescriptionOffset,
                    propertiesInfo.DescriptionLength / 2
                    );
            }
        }

        private MemoryAlloc GetPropertiesInformation()
        {
            NtStatus status;
            int retLength;

            var data = new MemoryAlloc(0x1000);

            status = Win32.NtQueryInformationTransaction(
                this,
                TransactionInformationClass.TransactionPropertiesInformation,
                data,
                data.Size,
                out retLength
                );

            if (status == NtStatus.BufferTooSmall)
            {
                // Resize the buffer and try again.
                data.Resize(retLength);

                status = Win32.NtQueryInformationTransaction(
                    this,
                    TransactionInformationClass.TransactionPropertiesInformation,
                    data,
                    data.Size,
                    out retLength
                    );
            }

            if (status >= NtStatus.Error)
            {
                data.Dispose();
                Win32.ThrowLastError(status);
            }

            return data;
        }

        public long GetTimeout()
        {
            using (var data = this.GetPropertiesInformation())
                return data.ReadStruct<TransactionPropertiesInformation>().Timeout;
        }

        public void Rollback(bool wait)
        {
            NtStatus status;

            if ((status = Win32.NtRollbackTransaction(this, wait)) >= NtStatus.Error)
                Win32.ThrowLastError(status);
        }
    }
}
