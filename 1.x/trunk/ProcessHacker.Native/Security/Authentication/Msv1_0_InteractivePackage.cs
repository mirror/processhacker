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

        public MemoryRegion GetLogonData()
        {
            MemoryAlloc data;
            int dataSize;
            int domainNameOffset;
            int userNameOffset;
            int passwordOffset;

            // The structure plus the strings must be stored in the same buffer, 
            // so we have to do some computation.

            domainNameOffset = Marshal.SizeOf(typeof(Msv1_0_InteractiveLogon));
            userNameOffset = domainNameOffset + _domainName.Length * 2;
            passwordOffset = userNameOffset + _userName.Length * 2;
            dataSize = passwordOffset + _password.Length * 2;
            data = new MemoryAlloc(dataSize);

            Msv1_0_InteractiveLogon info = new Msv1_0_InteractiveLogon();

            info.MessageType = Msv1_0_LogonSubmitType.Interactive;

            info.LogonDomainName.MaximumLength = info.LogonDomainName.Length = (ushort)(_domainName.Length * 2);
            info.LogonDomainName.Buffer = data.Memory.Increment(domainNameOffset);
            data.WriteUnicodeString(domainNameOffset, _domainName);

            info.UserName.MaximumLength = info.UserName.Length = (ushort)(_userName.Length * 2);
            info.UserName.Buffer = data.Memory.Increment(userNameOffset);
            data.WriteUnicodeString(userNameOffset, _userName);

            info.Password.MaximumLength = info.Password.Length = (ushort)(_password.Length * 2);
            info.Password.Buffer = data.Memory.Increment(passwordOffset);
            data.WriteUnicodeString(passwordOffset, _password);

            data.WriteStruct<Msv1_0_InteractiveLogon>(info);

            return data;
        }

        public object GetProfileData(MemoryRegion buffer)
        {
            return null;
        }
    }
}
