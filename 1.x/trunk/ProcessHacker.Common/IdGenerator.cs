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

namespace ProcessHacker.Common
{
    /// <summary>
    /// Provides a facility for generating unique IDs.
    /// </summary>
    public class IdGenerator
    {
        private int _step = 1;
        private bool _sort = false;
        private List<int> _ids = new List<int>();
        private int _id;

        /// <summary>
        /// Creates a new ID generator.
        /// </summary>
        public IdGenerator()
            : this(0)
        { }

        /// <summary>
        /// Creates a new ID generator.
        /// </summary>
        /// <param name="start">The starting ID.</param>
        public IdGenerator(int start)
            : this(start, 1)
        { }

        /// <summary>
        /// Creates a new ID generator.
        /// </summary>
        /// <param name="start">The starting ID.</param>
        /// <param name="step">The number each ID will be divisible by.</param>
        public IdGenerator(int start, int step)
        {
            if (step == 0)
                throw new ArgumentException("step cannot be zero.");

            _id = start;
            _step = step;
        }

        public bool Sort
        {
            get { return _sort; }
            set { _sort = value; }
        }

        /// <summary>
        /// Generates a new ID.
        /// </summary>
        /// <returns></returns>
        public int Pop()
        {
            int id;

            lock (_ids)
            {
                if (_ids.Count > 0)
                {
                    id = _ids[0];

                    _ids.Remove(_ids[0]);

                    return id;
                }
                else
                {
                    id = _id;
                    _id += _step;
                }
            }

            return id;
        }

        /// <summary>
        /// Makes an ID available for use.
        /// </summary>
        /// <param name="id"></param>
        public void Push(int id)
        {
            lock (_ids)
            {
                _ids.Add(id);

                if (_sort)
                    _ids.Sort();
            }
        }
    }
}
