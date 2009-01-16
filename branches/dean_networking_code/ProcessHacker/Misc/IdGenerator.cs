/*
 * Process Hacker - 
 *   unique ID generator
 * 
 * Copyright (C) 2008 wj32
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
using System.Text;

namespace ProcessHacker
{
    /// <summary>
    /// Provides a facility for generating unique Ids.
    /// </summary>
    public class IdGenerator
    {
        List<int> _ids = new List<int>();
        int _id;

        /// <summary>
        /// Creates a new instance of the class with a starting ID of 0.
        /// </summary>
        public IdGenerator()
        {
            _id = 0;
        }

        /// <summary>
        /// Creates a new instance of the class with the specified starting ID.
        /// </summary>
        /// <param name="start">The starting ID.</param>
        public IdGenerator(int start)
        {
            _id = start;
        }

        /// <summary>
        /// Generates a new ID.
        /// </summary>
        /// <returns></returns>
        public int Pop()
        {
            if (_ids.Count > 0)
            {
                int id = _ids[0];

                _ids.Remove(_ids[0]);

                return id;
            }

            return _id++;
        }

        /// <summary>
        /// Makes an ID available for use.
        /// </summary>
        /// <param name="id"></param>
        public void Push(int id)
        {
            _ids.Add(id);
            _ids.Sort();
        }
    }
}
