using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ProcessHacker.Common.Threading
{
    public static class Interlocked2
    {
        public static int Set(ref int location, Predicate<int> predicate, Func<int, int> transform)
        {
            int value;

            while (true)
            {
                value = location;

                if (!predicate(value))
                    return value;

                if (Interlocked.CompareExchange(
                    ref location,
                    transform(value),
                    value
                    ) == value)
                    return value;
            }
        }
    }
}
