/*
 * Process Hacker - 
 *   history manager
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
using System.Collections.ObjectModel;
using System.Text;

namespace ProcessHacker
{
    public static class HistoryManager
    {
        private static int _globalMaxCount = 750;

        public static int GlobalMaxCount
        {
            get { return _globalMaxCount; }
            set { _globalMaxCount = value; }
        }
    }

    public class HistoryManager<TKey, TValue>
    {
        private Dictionary<TKey, CircularBuffer<TValue>> _history = new Dictionary<TKey, CircularBuffer<TValue>>();
        private Dictionary<TKey, ReadOnlyCollection<TValue>> _readOnlyCollections = new Dictionary<TKey, ReadOnlyCollection<TValue>>();
        private int _maxCount = -1;

        public int MaxCount
        {
            get { return _maxCount; }
            set { _maxCount = value; }
        }

        public int EffectiveMaxCount
        {
            get { return _maxCount == -1 ? HistoryManager.GlobalMaxCount : _maxCount; }
        }

        public ReadOnlyCollection<TValue> this[TKey key]
        {
            get { return GetHistory(key); }
        }

        public void Add(TKey key)
        {
            _history.Add(key, new CircularBuffer<TValue>(this.EffectiveMaxCount));
        }

        public ReadOnlyCollection<TValue> GetHistory(TKey key)
        {
            if (!_readOnlyCollections.ContainsKey(key))
                _readOnlyCollections.Add(key, new ReadOnlyCollection<TValue>(_history[key]));

            return _readOnlyCollections[key];
        }

        public void Update(TKey key, TValue value)
        {
            int maxCount = this.EffectiveMaxCount;

            if (_history[key].Size != maxCount)
                _history[key].Resize(maxCount);

            _history[key].Add(value);
        }
    }
}
