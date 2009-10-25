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

using System.Threading;

namespace ProcessHacker.Common
{
    /// <summary>
    /// Manages a list of free objects that can be re-used.
    /// </summary>
    public class FreeList<T>
        where T : IResettable, new()
    {
        private class FreeListEntry<U>
            where U : IResettable, new()
        {
            public U Object;
            public FreeListEntry<U> Next;
        }

        private FreeListEntry<T> _listHead = null;
        private int _count = 0;
        private int _maximumCount = 0;

        public int Count
        {
            get { return _count; }
        }

        public int MaximumCount
        {
            get { return _maximumCount; }
            set { _maximumCount = value; }
        }

        public T Allocate()
        {
            FreeListEntry<T> listHead;

            // Atomically pop an entry off and replace the list head 
            // pointer with a pointer to the next entry.
            while (true)
            {
                listHead = _listHead;

                // If the list head pointer is null, we don't have anything 
                // to use from the free list.
                if (listHead == null)
                    break;

                // Try to replace the list head pointer.
                if (Interlocked.CompareExchange<FreeListEntry<T>>(
                    ref _listHead,
                    listHead.Next,
                    listHead
                    ) == listHead)
                {
                    // Success.
                    _count--;
                    return listHead.Object;
                }
            }

            return this.AllocateNew();
        }

        private T AllocateNew()
        {
            T obj = new T();
            obj.ResetObject();
            return obj;
        }

        public void Free(T obj)
        {
            FreeListEntry<T> listHead;
            FreeListEntry<T> listEntry;

            // Add the object to the free list if we haven't 
            // exceeded the maximum count.
            if (_count < _maximumCount || _maximumCount == 0)
            {
                listEntry = new FreeListEntry<T>();

                listEntry.Object = obj;

                // Atomically add the list entry.
                while (true)
                {
                    listHead = _listHead;
                    listEntry.Next = listHead;

                    if (Interlocked.CompareExchange<FreeListEntry<T>>(
                        ref _listHead,
                        listEntry,
                        listHead
                        ) == listHead)
                    {
                        // Success.
                        _count++;
                        break;
                    }
                }
            }
        }
    }
}
