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
            
            return KphSsFilterType.Exclude;
        }
    }
}
