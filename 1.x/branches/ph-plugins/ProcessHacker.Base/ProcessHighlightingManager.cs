using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace ProcessHacker.Base
{
    public sealed class ProcessHighlightingManager
    {
        public delegate bool FilterDelegate(int pid, ref Color color);

        private Dictionary<string, FilterDelegate> _filters =
            new Dictionary<string, FilterDelegate>();

        public void AddFilter(string name, FilterDelegate tabPage)
        {
            _filters.Add(name, tabPage);
        }

        public IEnumerable<FilterDelegate> GetFilters()
        {
            return _filters.Values;
        }

        public void RemoveFilter(string name)
        {
            _filters.Remove(name);
        }
    }
}
