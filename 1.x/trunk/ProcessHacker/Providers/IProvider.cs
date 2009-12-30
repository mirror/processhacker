using System;
using System.Collections.Generic;
using System.Text;
using ProcessHacker.Common;

namespace ProcessHacker
{
    public interface IProvider
    {
        bool Boosting { get; set; }
        bool Busy { get; }
        bool Enabled { get; set; }
        LinkedListEntry<IProvider> ListEntry { get; }
        ProviderThread Owner { get; set; }
        bool Unregistering { get; set; }
        void Run();
    }
}
