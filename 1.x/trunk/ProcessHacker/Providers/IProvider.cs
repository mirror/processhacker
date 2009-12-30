using System;
using System.Collections.Generic;
using System.Text;
using ProcessHacker.Common;

namespace ProcessHacker
{
    public interface IProvider
    {
        bool Busy { get; }
        bool Enabled { get; set; }
        LinkedListEntry<IProvider> ListEntry { get; }
        ProviderThread Owner { get; set; }
        void Run();
    }
}
