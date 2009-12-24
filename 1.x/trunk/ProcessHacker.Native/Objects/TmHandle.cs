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
using System.Runtime.InteropServices;
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
            NtStatus status;
            ObjectAttributes oa = new ObjectAttributes(name, objectFlags, rootDirectory);
            IntPtr handle;

            try
            {
                UnicodeString logFileNameStr = new UnicodeString(logFileName);

                try
                {
                    if ((status = Win32.NtCreateTransactionManager(
                        out handle,
                        access,
                        ref oa,
                        ref logFileNameStr,
                        createOptions,
                        0
                        )) >= NtStatus.Error)
                        Win32.Throw(status);
                }
                finally
                {
                    logFileNameStr.Dispose();
                }
            }
            finally
            {
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
            NtStatus status;
            ObjectAttributes oa = new ObjectAttributes(name, objectFlags, rootDirectory);
            IntPtr handle;

            try
            {
                if ((status = Win32.NtOpenTransactionManager(
                    out handle,
                    access,
                    ref oa,
                    IntPtr.Zero,
                    IntPtr.Zero,
                    0
                    )) >= NtStatus.Error)
                    Win32.Throw(status);
            }
            finally
            {
                oa.Dispose();
            }

            this.Handle = handle;
        }

        public TmBasicInformation GetBasicInformation()
        {
            NtStatus status;
            TmBasicInformation basicInfo;
            int retLength;

            if ((status = Win32.NtQueryInformationTransactionManager(
                this,
                TmInformationClass.TransactionManagerBasicInformation,
                out basicInfo,
                Marshal.SizeOf(typeof(TmBasicInformation)),
                out retLength
                )) >= NtStatus.Error)
                Win32.Throw(status);

            return basicInfo;
        }

        public long GetLastRecoveredLsn()
        {
            NtStatus status;
            TmRecoveryInformation recoveryInfo;
            int retLength;

            if ((status = Win32.NtQueryInformationTransactionManager(
                this,
                TmInformationClass.TransactionManagerRecoveryInformation,
                out recoveryInfo,
                Marshal.SizeOf(typeof(TmRecoveryInformation)),
                out retLength
                )) >= NtStatus.Error)
                Win32.Throw(status);

            return recoveryInfo.LastRecoveredLsn;
        }

        public string GetLogFileName()
        {
            NtStatus status;
            int retLength;

            using (var data = new MemoryAlloc(0x1000))
            {
                status = Win32.NtQueryInformationTransactionManager(
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

                    status = Win32.NtQueryInformationTransactionManager(
                        this,
                        TmInformationClass.TransactionManagerLogPathInformation,
                        data,
                        data.Size,
                        out retLength
                        );
                }

                if (status >= NtStatus.Error)
                    Win32.Throw(status);

                TmLogPathInformation logPathInfo = data.ReadStruct<TmLogPathInformation>();

                return data.ReadUnicodeString(TmLogPathInformation.LogPathOffset, logPathInfo.LogPathLength);
            }
        }

        public Guid GetLogIdentity()
        {
            NtStatus status;
            TmLogInformation logInfo;
            int retLength;

            if ((status = Win32.NtQueryInformationTransactionManager(
                this,
                TmInformationClass.TransactionManagerLogInformation,
                out logInfo,
                Marshal.SizeOf(typeof(TmLogInformation)),
                out retLength
                )) >= NtStatus.Error)
                Win32.Throw(status);

            return logInfo.LogIdentity;
        }

        public void Recover()
        {
            NtStatus status;

            if ((status = Win32.NtRecoverTransactionManager(this)) >= NtStatus.Error)
                Win32.Throw(status);
        }

        public void Rollforward(long virtualClock)
        {
            NtStatus status;

            if ((status = Win32.NtRollforwardTransactionManager(
                this,
                ref virtualClock
                )) >= NtStatus.Error)
                Win32.Throw(status);
        }
    }
}
