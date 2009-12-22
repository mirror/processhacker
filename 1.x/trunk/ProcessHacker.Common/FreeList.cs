/*
 * Process Hacker - 
 *   free list
 * 
 * Copyright (C) 2009 wj32
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
using System.Threading;

namespace ProcessHacker.Common
{
    /// <summary>
    /// Manages a list of free objects that can be re-used.
    /// </summary>
    public class FreeList<T>
        where T : IResettable, new()
    {
        private T[] _list;
        private int _freeIndex = 0;

        public FreeList(int maximumCount)
        {
            _list = new T[maximumCount];
        }

        public int Count
        {
            get { return _freeIndex; }
        }

        public int MaximumCount
        {
            get { return _list.Length; }
        }

        public T Allocate()
        {
            int freeIndex;

            while (true)
            {
                freeIndex = _freeIndex;

                // Check if we have any stored objects.
                if (freeIndex == 0)
                    return this.AllocateNew();

                if (Interlocked.CompareExchange(
                    ref _freeIndex,
                    freeIndex - 1,
                    freeIndex
                    ) == freeIndex)
                {
                    return _list[freeIndex - 1];
                }
            }
        }

        private T AllocateNew()
        {
            T obj = new T();
            obj.ResetObject();
            return obj;
        }

        public void Free(T obj)
        {
            int freeIndex;

            while (true)
            {
                freeIndex = _freeIndex;

                // Check if we have room for another object.
                if (freeIndex == _list.Length)
                    return;

                if (Interlocked.CompareExchange(
                    ref _freeIndex,
                    freeIndex + 1,
                    freeIndex
                    ) == freeIndex)
                {
                    _list[freeIndex] = obj;
                    break;
                }
            }
        }
    }
}
