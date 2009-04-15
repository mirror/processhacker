/*
 * Process Hacker - 
 *   circular buffer
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
using System.Collections.Generic;
using System.Text;

namespace ProcessHacker
{
    public class CircularBuffer<T> : IList<T>
    {
        private int _size;
        private int _count;
        private int _index;
        private T[] _data;

        public CircularBuffer(int size)
        {
            _size = size;
            _count = 0;
            _index = _size - 1;
            _data = new T[size];
        }

        public T this[int index]
        {
            get
            {
                int i = (_index + index) % _size;

                if (i < 0)
                    i = _size + i;

                return _data[i];
            }
            set
            {
                int i = (_index + index) % _size;

                if (i < 0)
                    i = _size + i;

                _data[i] = value;
            }
        }

        public int Count
        {
            get { return _count; }
        }

        public int Size
        {
            get { return _size; }
        }

        public void Add(T value)
        {
            _index = --_index % _size;

            if (_index < 0)
                _index = _size + _index;

            _data[_index] = value;

            if (_count < _size)
                _count++;
        }

        public void Resize(int newSize)
        {
            if (newSize == _size)
                return;

            T[] newArray = new T[newSize];
            int tailSize = _size - _index;
            int headSize = _index;

            // The tail contains the most recent data.

            if (newSize > _size)
            {
                // Copy the tail, then the head.
                // This means that the tail will now be at the front.
                Array.Copy(_data, _index, newArray, 0, tailSize);
                Array.Copy(_data, 0, newArray, tailSize, headSize);
                _index = 0;
            }
            else if (newSize < _size)
            {
                if (tailSize >= newSize)
                {
                    // Copy only a part of the tail because we don't have enough room.
                    Array.Copy(_data, _index, newArray, 0, newSize);
                    _index = 0;
                }
                else
                {
                    // Copy the tail in full, then copy a part of the head.
                    Array.Copy(_data, _index, newArray, 0, tailSize);
                    Array.Copy(_data, 0, newArray, tailSize, newSize - tailSize);
                    _index = 0;
                }

                if (_count > newSize)
                    _count = newSize;
            }

            _data = newArray;
            _size = newSize;
        }

        #region IList<T> Members

        public int IndexOf(T item)
        {
            for (int i = 0; i < this.Count; i++)
                if (this[i].Equals(item))
                    return i;

            return -1;
        }

        public void Insert(int index, T item)
        {
            throw new NotSupportedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region ICollection<T> Members

        public void Clear()
        {
            throw new NotSupportedException();
        }

        public bool Contains(T item)
        {
            return this.IndexOf(item) != -1;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
