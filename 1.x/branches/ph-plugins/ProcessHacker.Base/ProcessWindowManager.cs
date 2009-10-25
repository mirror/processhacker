using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace ProcessHacker.Base
{
    public sealed class ProcessWindowManager
    {
        public delegate TabPage GetTabPageDelegate(int pid);

        private Dictionary<string, GetTabPageDelegate> _tabPages =
            new Dictionary<string, GetTabPageDelegate>();

        public void AddTabPage(string name, GetTabPageDelegate tabPage)
        {
            _tabPages.Add(name, tabPage);
        }

        public IEnumerable<GetTabPageDelegate> GetTabPages()
        {
            return _tabPages.Values;
        }

        public void RemoveTabPage(string name)
        {
            _tabPages.Remove(name);
        }
    }
}
