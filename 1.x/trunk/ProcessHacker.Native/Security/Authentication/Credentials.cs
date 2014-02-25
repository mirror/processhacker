/*
 * Process Hacker - 
 *   credentials UI wrapper
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
using System.Windows.Forms;
using ProcessHacker.Native.Api;

namespace ProcessHacker.Native.Security.Authentication
{
    public static class Credentials
    {
        public static MemoryRegion PackCredentials(CredPackFlags flags, string userName, string password)
        {
            MemoryAlloc data = new MemoryAlloc(0x100);
            int size = data.Size;

            if (userName == null)
                userName = "";
            if (password == null)
                password = "";

            if (!Win32.CredPackAuthenticationBuffer(flags, userName, password, data, ref size))
            {
                data.ResizeNew(size);

                if (!Win32.CredPackAuthenticationBuffer(flags, userName, password, data, ref size))
                    Win32.Throw();
            }

            return data;
        }

        public static void UnpackCredentials(
            MemoryRegion buffer,
            CredPackFlags flags,
            out string domainName,
            out string userName,
            out string password
            )
        {
            using (var domainNameBuffer = new MemoryAlloc(0x100))
            using (var userNameBuffer = new MemoryAlloc(0x100))
            using (var passwordBuffer = new MemoryAlloc(0x100))
            {
                int domainNameSize = domainNameBuffer.Size / 2 - 1;
                int userNameSize = userNameBuffer.Size / 2 - 1;
                int passwordSize = passwordBuffer.Size / 2 - 1;

                if (!Win32.CredUnPackAuthenticationBuffer(
                    flags,
                    buffer,
                    buffer.Size,
                    userNameBuffer,
                    ref userNameSize,
                    domainNameBuffer,
                    ref domainNameSize,
                    passwordBuffer,
                    ref passwordSize
                    ))
                {
                    domainNameBuffer.ResizeNew(domainNameSize * 2 + 2);
                    userNameBuffer.ResizeNew(userNameSize * 2 + 2);
                    passwordBuffer.ResizeNew(passwordSize * 2 + 2);

                    if (!Win32.CredUnPackAuthenticationBuffer(
                        flags,
                        buffer,
                        buffer.Size,
                        userNameBuffer,
                        ref userNameSize,
                        domainNameBuffer,
                        ref domainNameSize,
                        passwordBuffer,
                        ref passwordSize
                        ))
                        Win32.Throw();
                }

                domainName = domainNameBuffer.ReadUnicodeString(0);
                userName = userNameBuffer.ReadUnicodeString(0);
                password = passwordBuffer.ReadUnicodeString(0);
            }
        }

        public static bool PromptForCredentials(
            IWin32Window parent,
            string messageText,
            string captionText,
            string targetName,
            Win32Error errorCode,
            ref string userName,
            ref string password,
            ref bool save,
            CredUiFlags flags
            )
        {
            const int maxBytes = 0x200;
            const int maxChars = (maxBytes - 2) / 2;

            Win32Error result;
            CredUiInfo info = new CredUiInfo();

            if (userName.Length > maxChars || password.Length > maxChars)
                throw new ArgumentException("The user name or password string is too long.");

            info.Size = System.Runtime.InteropServices.Marshal.SizeOf(typeof(CredUiInfo));
            info.Parent = parent != null ? parent.Handle : IntPtr.Zero;
            info.MessageText = messageText;
            info.CaptionText = captionText;

            using (var userNameAlloc = new MemoryAlloc(maxBytes))
            using (var passwordAlloc = new MemoryAlloc(maxBytes))
            {
                userNameAlloc.WriteUnicodeString(0, userName);
                userNameAlloc.WriteInt16(userName.Length * 2, 0);
                passwordAlloc.WriteUnicodeString(0, password);
                passwordAlloc.WriteInt16(password.Length * 2, 0);

                result = Win32.CredUIPromptForCredentials(
                    ref info,
                    targetName,
                    IntPtr.Zero,
                    errorCode,
                    userNameAlloc,
                    maxBytes / 2,
                    passwordAlloc,
                    maxBytes / 2,
                    ref save,
                    flags
                    );

                if (result == Win32Error.Cancelled)
                    return false;
                if (result != Win32Error.Success)
                    Win32.Throw(result);

                userName = userNameAlloc.ReadUnicodeString(0);
                password = passwordAlloc.ReadUnicodeString(0);

                return true;
            }
        }

        public static bool PromptForCredentials2(
            IWin32Window parent,
            string messageText,
            string captionText,
            Win32Error errorCode,
            ref string domainName,
            ref string userName,
            ref string password,
            ref bool save,
            CredUiWinFlags flags
            )
        {
            Win32Error result;
            CredUiInfo info = new CredUiInfo();
            int authenticationPackage = 0;
            IntPtr outAuthBuffer;
            int outAuthBufferSize;

            info.Size = System.Runtime.InteropServices.Marshal.SizeOf(typeof(CredUiInfo));
            info.Parent = parent != null ? parent.Handle : IntPtr.Zero;
            info.MessageText = messageText;
            info.CaptionText = captionText;

            using (var inAuthBuffer = PackCredentials(0, userName, password))
            {
                result = Win32.CredUIPromptForWindowsCredentials(
                    ref info,
                    errorCode,
                    ref authenticationPackage,
                    inAuthBuffer,
                    inAuthBuffer.Size,
                    out outAuthBuffer,
                    out outAuthBufferSize,
                    ref save,
                    flags
                    );

                if (result == Win32Error.Cancelled)
                    return false;
                if (result != Win32Error.Success)
                    Win32.Throw(result);

                try
                {
                    UnpackCredentials(
                        new MemoryRegion(outAuthBuffer, 0, outAuthBufferSize),
                        CredPackFlags.ProtectedCredentials,
                        out domainName,
                        out userName,
                        out password
                        );

                    return true;
                }
                finally
                {
                    System.Runtime.InteropServices.Marshal.FreeCoTaskMem(outAuthBuffer);
                }
            }
        }

        public static SecPkgInfo[] GetSSPackages()
        {
            int result;
            int count;
            IntPtr packages;

            result = Win32.EnumerateSecurityPackages(out count, out packages);

            if (result != 0)
                Win32.Throw(result);

            var alloc = new MemoryRegion(packages);

            try
            {
                SecPkgInfo[] array = new SecPkgInfo[count];

                for (int i = 0; i < count; i++)
                    array[i] = alloc.ReadStruct<SecPkgInfo>(i);

                return array;
            }
            finally
            {
                Win32.FreeContextBuffer(packages);
            }
        }
    }
}
