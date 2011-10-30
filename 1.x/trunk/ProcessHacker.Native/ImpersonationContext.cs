using System;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;

namespace ProcessHacker.Native
{
    public class ImpersonationContext : IDisposable
    {
        private bool _disposed;

        public ImpersonationContext(TokenHandle token)
        {
            if (!Win32.ImpersonateLoggedOnUser(token))
                Win32.Throw();
        }

        public void Dispose()
        {
            if (_disposed) 
                return;

            Win32.RevertToSelf();
            this._disposed = true;
        }
    }
}
