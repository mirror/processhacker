/*
 * Process Hacker - 
 *   long extensions
 * 
 * Copyright (C) 2009 Dean
 * 
 * This file is part of Process Hacker.
 * 
 * Process Hacker is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * Process Hacker is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Process Hacker.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;

namespace ProcessHacker.Common
{
    public static class LongExtensions
    {
        /// <summary>
        /// Gets the largest value in the specified array of longs.
        /// </summary>
        /// <param name="source">The source array.</param>
        /// <returns>The largest value in the specified array.</returns>
        public static long Max(this IEnumerable<long> source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            long max = 0;
            bool afterFirst = false;

            foreach (long number in source)
            {
                if (afterFirst)
                {
                    if (number > max)
                        max = number;
                }
                else
                {
                    max = number;
                    afterFirst = true;
                }
            }

            return max;
        }

        /// <summary>
        /// Takes a number of elements from the specified array.
        /// </summary>
        /// <param name="source">The list to process.</param>
        /// <param name="count">The number of elements to take.</param>
        /// <returns>
        /// A new list containing the first <paramref name="count" /> 
        /// elements from the specified array.
        /// </returns>
        public static IList<long> Take(this IList<long> source, int count)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            // If we're trying to take more than we have, return the original set.
            if (count >= source.Count)
                return source;

            // Create a new list containing the elements.
            IList<long> newList = new List<long>();

            for (int i = 0; i < count; i++)
                newList.Add(source[i]);

            return newList;
        }
    }
}
