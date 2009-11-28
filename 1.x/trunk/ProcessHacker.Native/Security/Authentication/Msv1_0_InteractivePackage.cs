/*
 * Process Hacker - 
 *   MSV1_0 package client
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

namespace ProcessHacker.Native.Security.Authentication
{
    public sealed class Msv1_0_InteractivePackage : IAuthenticationPackage
    {
        private string _domainName;
        private string _userName;
        private string _password;

        public Msv1_0_InteractivePackage()
        { }

        public Msv1_0_InteractivePackage(Sid sid)
            : this(sid, null)
        { }

        public Msv1_0_InteractivePackage(Sid sid, string password)
            : this(sid.DomainName, sid.Name, password)
        { }

        public Msv1_0_InteractivePackage(string domainName, string userName)
            : this(domainName, userName, null)
        { }

        public Msv1_0_InteractivePackage(string domainName, string userName, string password)
        {
            _domainName = domainName;
            _userName = userName;
            _password = password;
        }

        public string PackageName
        {
            get { return Win32.Msv1_0_PackageName; }
        }

        public string DomainName
        {
            get { return _domainName; }
            set { _domainName = value; }
        }

        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }

        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        public MemoryRegion GetAuthData()
        {
            MemoryAlloc data;
            int dataSize;
            int domainNameOffset;
            int userNameOffset;
            int passwordOffset;
            string lDomainName = _domainName != null ? _domainName : "";
            string lUserName = _userName != null ? _userName : "";
            string lPassword = _password != null ? _password : "";

            // The structure plus the strings must be stored in the same buffer, 
            // so we have to do some computation.

            domainNameOffset = Marshal.SizeOf(typeof(Msv1_0_InteractiveLogon));
            userNameOffset = domainNameOffset + lDomainName.Length * 2;
            passwordOffset = userNameOffset + lUserName.Length * 2;
            dataSize = passwordOffset + lPassword.Length * 2;
            data = new MemoryAlloc(dataSize);

            Msv1_0_InteractiveLogon info = new Msv1_0_InteractiveLogon();

            info.MessageType = Msv1_0_LogonSubmitType.Interactive;

            info.LogonDomainName.MaximumLength = info.LogonDomainName.Length = (ushort)(lDomainName.Length * 2);
            info.LogonDomainName.Buffer = data.Memory.Increment(domainNameOffset);
            data.WriteUnicodeString(domainNameOffset, lDomainName);

            info.UserName.MaximumLength = info.UserName.Length = (ushort)(lUserName.Length * 2);
            info.UserName.Buffer = data.Memory.Increment(userNameOffset);
            data.WriteUnicodeString(userNameOffset, lUserName);

            info.Password.MaximumLength = info.Password.Length = (ushort)(lPassword.Length * 2);
            info.Password.Buffer = data.Memory.Increment(passwordOffset);
            data.WriteUnicodeString(passwordOffset, lPassword);

            data.WriteStruct<Msv1_0_InteractiveLogon>(info);

            return data;
        }

        public object GetProfileData(MemoryRegion buffer)
        {
            return null;
        }

        public void ReadAuthData(MemoryRegion buffer)
        {
            Msv1_0_InteractiveLogon info = buffer.ReadStruct<Msv1_0_InteractiveLogon>();

            // Fix up relative addresses.
            if (info.LogonDomainName.Buffer.CompareTo(buffer.Size) < 0)
                info.LogonDomainName.Buffer = info.LogonDomainName.Buffer.Increment(buffer);
            if (info.UserName.Buffer.CompareTo(buffer.Size) < 0)
                info.UserName.Buffer = info.UserName.Buffer.Increment(buffer);
            if (info.Password.Buffer.CompareTo(buffer.Size) < 0)
                info.Password.Buffer = info.Password.Buffer.Increment(buffer);

            _domainName = info.LogonDomainName.Read();
            _userName = info.UserName.Read();
            _password = info.Password.Read();
        }
    }
}
