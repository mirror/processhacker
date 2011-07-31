/*
 * Process Hacker - 
 *   LSA secret handle
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
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Security;

namespace ProcessHacker.Native.Objects
{
    /// <summary>
    /// Represents a handle to a LSA secret.
    /// </summary>
    public sealed class LsaSecretHandle : LsaHandle<LsaSecretAccess>
    {
        public static LsaSecretHandle Create(LsaSecretAccess access, LsaPolicyHandle policyHandle, string name)
        {
            NtStatus status;
            UnicodeString nameStr;
            IntPtr handle;

            nameStr = new UnicodeString(name);

            try
            {
                if ((status = Win32.LsaCreateSecret(
                    policyHandle,
                    ref nameStr,
                    access,
                    out handle
                    )) >= NtStatus.Error)
                    Win32.Throw(status);
            }
            finally
            {
                nameStr.Dispose();
            }

            return new LsaSecretHandle(handle, true);
        }

        private LsaSecretHandle(IntPtr handle, bool owned)
            : base(handle, owned)
        { }

        public LsaSecretHandle(LsaPolicyHandle policyHandle, string name, LsaSecretAccess access)
        {
            NtStatus status;
            UnicodeString nameStr;
            IntPtr handle;

            nameStr = new UnicodeString(name);

            try
            {
                if ((status = Win32.LsaOpenSecret(
                    policyHandle,
                    ref nameStr,
                    access,
                    out handle
                    )) >= NtStatus.Error)
                    Win32.Throw(status);
            }
            finally
            {
                nameStr.Dispose();
            }

            this.Handle = handle;
        }

        public string Query()
        {
            string currentValue;
            string oldValue;

            this.Query(out currentValue, out oldValue);

            return currentValue;
        }

        public void Query(out string currentValue, out string oldValue)
        {
            DateTime currentValueSetTime;
            DateTime oldValueSetTime;

            this.Query(out currentValue, out currentValueSetTime, out oldValue, out oldValueSetTime);
        }

        public void Query(
            out string currentValue,
            out DateTime currentValueSetTime,
            out string oldValue,
            out DateTime oldValueSetTime
            )
        {
            NtStatus status;
            IntPtr currentValueStr;
            long currentValueSetTimeLong;
            IntPtr oldValueStr;
            long oldValueSetTimeLong;

            if ((status = Win32.LsaQuerySecret(
                this,
                out currentValueStr,
                out currentValueSetTimeLong,
                out oldValueStr,
                out oldValueSetTimeLong
                )) >= NtStatus.Error)
                Win32.Throw(status);

            using (var currentValueStrAlloc = new LsaMemoryAlloc(currentValueStr))
            using (var oldValueStrAlloc = new LsaMemoryAlloc(oldValueStr))
            {
                currentValue = currentValueStrAlloc.ReadStruct<UnicodeString>().Read();
                currentValueSetTime = DateTime.FromFileTime(currentValueSetTimeLong);
                oldValue = oldValueStrAlloc.ReadStruct<UnicodeString>().Read();
                oldValueSetTime = DateTime.FromFileTime(oldValueSetTimeLong);
            }
        }

        public void Set(string currentValue)
        {
            this.Set(currentValue, null);
        }

        public void Set(string currentValue, string oldValue)
        {
            NtStatus status;
            UnicodeString currentValueStr;
            UnicodeString oldValueStr;

            currentValueStr = new UnicodeString(currentValue);
            oldValueStr = new UnicodeString(oldValue);

            try
            {
                if ((status = Win32.LsaSetSecret(
                    this,
                    ref currentValueStr,
                    ref oldValueStr
                    )) >= NtStatus.Error)
                    Win32.Throw(status);
            }
            finally
            {
                currentValueStr.Dispose();
                oldValueStr.Dispose();
            }
        }
    }
}
