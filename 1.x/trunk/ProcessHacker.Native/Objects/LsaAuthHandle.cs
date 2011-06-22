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

using ProcessHacker.Native.Api;
using ProcessHacker.Native.Security.Authentication;

namespace ProcessHacker.Native.Objects
{
    public sealed class LsaAuthHandle : NativeHandle
    {
        public static LsaAuthHandle Connect(string name)
        {
            IntPtr handle;

            AnsiString nameStr = new AnsiString(name);

            try
            {
                LsaOperationalMode mode;

                Win32.LsaRegisterLogonProcess(nameStr, out handle, out mode).ThrowIf();
            }
            finally
            {
                nameStr.Dispose();
            }

            return new LsaAuthHandle(handle, true);
        }

        public static LsaAuthHandle ConnectUntrusted()
        {
            IntPtr handle;

            Win32.LsaConnectUntrusted(out handle).ThrowIf();

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
            IntPtr profileBuffer;
            int profileBufferLength;
            IntPtr token;
            QuotaLimits quotas;

            AnsiString originNameStr = new AnsiString(originName);

            try
            {
                using (MemoryRegion logonData = package.GetAuthData())
                {
                    Win32.LsaLogonUser(
                        this,
                        originNameStr,
                        logonType,
                        this.LookupAuthenticationPackage(package.PackageName),
                        logonData,
                        logonData.Size,
                        IntPtr.Zero,
                        source,
                        out profileBuffer,
                        out profileBufferLength,
                        out logonId,
                        out token,
                        out quotas,
                        out subStatus
                        ).ThrowIf();

                    using (LsaMemoryAlloc profileBufferAlloc = new LsaMemoryAlloc(profileBuffer, true))
                    {
                        profileData = package.GetProfileData(new MemoryRegion(profileBuffer, 0, profileBufferLength));
                    }

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
            int authenticationPackage;

            AnsiString packageNameStr = new AnsiString(packageName);

            try
            {
                Win32.LsaLookupAuthenticationPackage(this, packageNameStr, out authenticationPackage).ThrowIf();
            }
            finally
            {
                packageNameStr.Dispose();
            }

            return authenticationPackage;
        }
    }
}
