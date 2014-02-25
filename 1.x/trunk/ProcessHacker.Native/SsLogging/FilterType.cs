using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessHacker.Native.SsLogging
{
    public enum FilterType
    {
        Include,
        Exclude
    }

    public static class FilterTypeExtensions
    {
        public static KphSsFilterType ToKphSs(this FilterType filterType)
        {
            if (filterType == FilterType.Include)
                return KphSsFilterType.Include;
            else
                return KphSsFilterType.Exclude;
        }
    }
}
