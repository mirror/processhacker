/*
 * Process Hacker - 
 *   transaction manager handle
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
    public class TmHandle : NativeHandle<TmAccess>
    {
        public static TmHandle Create(
            TmAccess access,
            string name,
            ObjectFlags objectFlags,
            DirectoryHandle rootDirectory,
            string logFileName,
            TmOptions createOptions
            )
        {
            ObjectAttributes oa = new ObjectAttributes(name, objectFlags, rootDirectory);
            UnicodeString logFileNameStr = new UnicodeString(logFileName);
            IntPtr handle;

            try
            {
                Win32.NtCreateTransactionManager(
                    out handle,
                    access,
                    ref oa,
                    ref logFileNameStr,
                    createOptions,
                    0
                    ).ThrowIf();
            }
            finally
            {
                logFileNameStr.Dispose();
                oa.Dispose();
            }

            return new TmHandle(handle, true);
        }

        public static TmHandle FromHandle(IntPtr handle)
        {
            return new TmHandle(handle, false);
        }

        private TmHandle(IntPtr handle, bool owned)
            : base(handle, owned)
        { }

        public TmHandle(string name, ObjectFlags objectFlags, DirectoryHandle rootDirectory, TmAccess access)
        {
            ObjectAttributes oa = new ObjectAttributes(name, objectFlags, rootDirectory);
            IntPtr handle;

            try
            {
                Win32.NtOpenTransactionManager(
                    out handle,
                    access,
                    ref oa,
                    IntPtr.Zero,
                    IntPtr.Zero,
                    0
                    ).ThrowIf();
            }
            finally
            {
                oa.Dispose();
            }

            this.Handle = handle;
        }

        public TmBasicInformation BasicInformation
        {
            get
            {
                TmBasicInformation basicInfo;
                int retLength;

                Win32.NtQueryInformationTransactionManager(
                    this,
                    TmInformationClass.TransactionManagerBasicInformation,
                    out basicInfo,
                    TmBasicInformation.SizeOf,
                    out retLength
                    ).ThrowIf();

                return basicInfo;
            }
        }

        public long LastRecoveredLsn
        {
            get
            {
                TmRecoveryInformation recoveryInfo;
                int retLength;

                Win32.NtQueryInformationTransactionManager(
                    this,
                    TmInformationClass.TransactionManagerRecoveryInformation,
                    out recoveryInfo,
                    TmRecoveryInformation.SizeOf,
                    out retLength
                    ).ThrowIf();

                return recoveryInfo.LastRecoveredLsn;
            }
        }

        public string LogFileName
        {
            get
            {
                int retLength;

                using (MemoryAlloc data = new MemoryAlloc(0x1000))
                {
                    NtStatus status = Win32.NtQueryInformationTransactionManager(
                        this,
                        TmInformationClass.TransactionManagerLogPathInformation,
                        data,
                        data.Size,
                        out retLength
                        );

                    if (status == NtStatus.BufferTooSmall)
                    {
                        // Resize the buffer and try again.
                        data.ResizeNew(retLength);

                        Win32.NtQueryInformationTransactionManager(
                            this,
                            TmInformationClass.TransactionManagerLogPathInformation,
                            data,
                            data.Size,
                            out retLength
                            ).ThrowIf();
                    }

                    status.ThrowIf();

                    TmLogPathInformation logPathInfo = data.ReadStruct<TmLogPathInformation>();

                    return data.ReadUnicodeString(TmLogPathInformation.LogPathOffset, logPathInfo.LogPathLength);
                }
            }
        }

        public Guid LogIdentity
        {
            get
            {
                TmLogInformation logInfo;
                int retLength;

                Win32.NtQueryInformationTransactionManager(
                    this,
                    TmInformationClass.TransactionManagerLogInformation,
                    out logInfo,
                    TmLogInformation.SizeOf,
                    out retLength
                    ).ThrowIf();

                return logInfo.LogIdentity;
            }
        }

        public void Recover()
        {
            Win32.NtRecoverTransactionManager(this).ThrowIf();
        }

        public void Rollforward(long virtualClock)
        {
            Win32.NtRollforwardTransactionManager(
                this,
                ref virtualClock
                ).ThrowIf();
        }
    }
}
