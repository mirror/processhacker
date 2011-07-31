/*
 * Process Hacker - 
 *   LSA authentication handle
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
using System.Text;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Security;
using ProcessHacker.Native.Security.Authentication;

namespace ProcessHacker.Native.Objects
{
    public sealed class LsaAuthHandle : NativeHandle
    {
        public static LsaAuthHandle Connect(string name)
        {
            NtStatus status;
            AnsiString nameStr;
            IntPtr handle;
            LsaOperationalMode mode;

            nameStr = new AnsiString(name);

            try
            {
                if ((status = Win32.LsaRegisterLogonProcess(
                    ref nameStr,
                    out handle,
                    out mode
                    )) >= NtStatus.Error)
                    Win32.Throw(status);
            }
            finally
            {
                nameStr.Dispose();
            }

            return new LsaAuthHandle(handle, true);
        }

        public static LsaAuthHandle ConnectUntrusted()
        {
            NtStatus status;
            IntPtr handle;

            if ((status = Win32.LsaConnectUntrusted(out handle)) >= NtStatus.Error)
                Win32.Throw(status);

            return new LsaAuthHandle(handle, true);
        }

        private LsaAuthHandle(IntPtr handle, bool owned)
            : base(handle, owned)
        { }

        protected override void Close()
        {
            Win32.LsaDeregisterLogonProcess(this);
        }

        public TokenHandle LogonUser(SecurityLogonType logonType, IAuthenticationPackage package)
        {
            return this.LogonUser("PH.N", logonType, package);
        }

        public TokenHandle LogonUser(string originName, SecurityLogonType logonType, IAuthenticationPackage package)
        {
            object profileData;
            Luid logonId;
            NtStatus subStatus;

            return this.LogonUser(
                originName,
                logonType,
                package,
                TokenHandle.PhTokenSource,
                out profileData,
                out logonId,
                out subStatus
                );
        }

        public TokenHandle LogonUser(
            string originName,
            SecurityLogonType logonType,
            IAuthenticationPackage package,
            TokenSource source,
            out object profileData,
            out Luid logonId,
            out NtStatus subStatus
            )
        {
            NtStatus status;
            AnsiString originNameStr;
            IntPtr profileBuffer;
            int profileBufferLength;
            IntPtr token;
            QuotaLimits quotas;

            originNameStr = new AnsiString(originName);

            try
            {
                using (var logonData = package.GetAuthData())
                {
                    if ((status = Win32.LsaLogonUser(
                        this,
                        ref originNameStr,
                        logonType,
                        this.LookupAuthenticationPackage(package.PackageName),
                        logonData,
                        logonData.Size,
                        IntPtr.Zero,
                        ref source,
                        out profileBuffer,
                        out profileBufferLength,
                        out logonId,
                        out token,
                        out quotas,
                        out subStatus
                        )) >= NtStatus.Error)
                        Win32.Throw(status);

                    using (var profileBufferAlloc = new LsaMemoryAlloc(profileBuffer, true))
                        profileData = package.GetProfileData(new MemoryRegion(profileBuffer, 0, profileBufferLength));

                    return new TokenHandle(token, true);
                }
            }
            finally
            {
                originNameStr.Dispose();
            }
        }

        public int LookupAuthenticationPackage(string packageName)
        {
            NtStatus status;
            AnsiString packageNameStr;
            int authenticationPackage;

            packageNameStr = new AnsiString(packageName);

            try
            {
                if ((status = Win32.LsaLookupAuthenticationPackage(
                    this,
                    ref packageNameStr,
                    out authenticationPackage
                    )) >= NtStatus.Error)
                    Win32.Throw(status);
            }
            finally
            {
                packageNameStr.Dispose();
            }

            return authenticationPackage;
        }
    }
}
