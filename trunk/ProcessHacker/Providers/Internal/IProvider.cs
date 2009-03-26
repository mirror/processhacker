using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessHacker
{
    public interface IProvider
    {
        bool Busy { get; }
        bool CreateThread { get; set; }
        bool Enabled { get; set; }
        bool UseInvoke { get; set; }
        void RunOnce();
        void RunOnceAsync();
    }
}
