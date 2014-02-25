using System;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;

namespace ProcessHacker.Native
{
    public class ImpersonationContext : IDisposable
    {
        private bool _disposed = false;

        public ImpersonationContext(TokenHandle token)
        {
            if (!Win32.ImpersonateLoggedOnUser(token))
                Win32.Throw();
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                Win32.RevertToSelf();
                _disposed = true;
            }
        }
    }
}
