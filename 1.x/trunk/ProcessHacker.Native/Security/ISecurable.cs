using System;
using System.Collections.Generic;
using System.Text;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Security.AccessControl;

namespace ProcessHacker.Native.Security
{
    public interface ISecurable
    {
        SecurityDescriptor GetSecurity(SecurityInformation securityInformation);
        void SetSecurity(SecurityInformation securityInformation, SecurityDescriptor securityDescriptor);
    }
}
