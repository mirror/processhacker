using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessHacker
{
    public interface IProvider
    {
        event Action<IProvider> Disposed;
        bool Busy { get; }
        bool CreateThread { get; set; }
        bool Enabled { get; set; }
        void RunOnce();
        void RunOnceAsync();
        void Wait();
        bool Wait(int timeout);
    }
}
