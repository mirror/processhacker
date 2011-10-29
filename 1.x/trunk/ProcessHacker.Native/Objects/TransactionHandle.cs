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
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Security;

namespace ProcessHacker.Native.Objects
{
    public class TransactionHandle : NativeHandle<TransactionAccess>
    {
        public struct CurrentTransactionContext : IDisposable
        {
            private readonly TransactionHandle _oldHandle;
            private bool _disposed;

            internal CurrentTransactionContext(TransactionHandle handle)
            {
                _oldHandle = GetCurrent();
                SetCurrent(handle);
                _disposed = false;
            }

            public void Dispose()
            {
                if (!_disposed)
                {
                    SetCurrent(_oldHandle);
                    _disposed = true;
                }
            }
        }

        public static TransactionHandle Create(
            TransactionAccess access,
            TmHandle tmHandle,
            TransactionOptions createOptions,
            long timeout,
            string description
            )
        {
            return Create(
                access,
                null,
                0,
                null,
                Guid.Empty,
                tmHandle,
                createOptions,
                timeout,
                description
                );
        }

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
            ObjectAttributes oa = new ObjectAttributes(name, objectFlags, rootDirectory);
            IntPtr handle;

            if (unitOfWorkGuid == Guid.Empty)
                unitOfWorkGuid = Guid.NewGuid();

            try
            {
                UnicodeString descriptionStr = new UnicodeString(description);

                try
                {
                    Win32.NtCreateTransaction(
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
                        ).ThrowIf();
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

        public static TransactionHandle GetCurrent()
        {
            IntPtr handle = Win32.RtlGetCurrentTransaction();

            if (handle != IntPtr.Zero)
                return new TransactionHandle(handle, false);

            return null;
        }

        public static void SetCurrent(TransactionHandle transactionHandle)
        {
            Win32.RtlSetCurrentTransaction(transactionHandle ?? IntPtr.Zero);
        }

        public static CurrentTransactionContext SetCurrentContext(TransactionHandle transactionHandle)
        {
            return new CurrentTransactionContext(transactionHandle);
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
            ObjectAttributes oa = new ObjectAttributes(name, objectFlags, rootDirectory);
            IntPtr handle;

            try
            {
                Win32.NtOpenTransaction(
                    out handle,
                    access,
                    ref oa,
                    ref unitOfWorkGuid,
                    tmHandle ?? IntPtr.Zero
                    ).ThrowIf();
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
            Win32.NtCommitTransaction(this, wait).ThrowIf();
        }

        public TransactionBasicInformation BasicInformation
        {
            get
            {
                TransactionBasicInformation basicInfo;
                int retLength;

                Win32.NtQueryInformationTransaction(
                    this,
                    TransactionInformationClass.TransactionBasicInformation,
                    out basicInfo,
                    TransactionBasicInformation.SizeOf,
                    out retLength
                    ).ThrowIf();

                return basicInfo;
            }
        }

        public string Description
        {
            get
            {
                using (MemoryAlloc data = this.GetPropertiesInformation())
                {
                    TransactionPropertiesInformation propertiesInfo = data.ReadStruct<TransactionPropertiesInformation>();

                    return data.ReadUnicodeString(
                        TransactionPropertiesInformation.DescriptionOffset,
                        propertiesInfo.DescriptionLength / 2
                        );
                }
            }
        }

        private MemoryAlloc GetPropertiesInformation()
        {
            int retLength;

            MemoryAlloc data = new MemoryAlloc(0x1000);

            NtStatus status = Win32.NtQueryInformationTransaction(
                this,
                TransactionInformationClass.TransactionPropertiesInformation,
                data,
                data.Size,
                out retLength
                );

            if (status == NtStatus.BufferTooSmall)
            {
                // Resize the buffer and try again.
                data.ResizeNew(retLength);

                status = Win32.NtQueryInformationTransaction(
                    this,
                    TransactionInformationClass.TransactionPropertiesInformation,
                    data,
                    data.Size,
                    out retLength
                    );
            }

            if (status.IsError())
            {
                data.Dispose();
                status.Throw();
            }

            return data;
        }

        public long Timeout
        {
            get
            {
                using (MemoryAlloc data = this.GetPropertiesInformation())
                {
                    return data.ReadStruct<TransactionPropertiesInformation>().Timeout;
                }
            }
        }

        public void Rollback(bool wait)
        {
            Win32.NtRollbackTransaction(this, wait).ThrowIf();
        }
    }
}
