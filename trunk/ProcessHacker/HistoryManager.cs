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
    public static class HistoryManagerGlobal
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
        private Dictionary<TKey, List<TValue>> _history = new Dictionary<TKey, List<TValue>>();
        private int _maxCount = -1;

        public int MaxCount
        {
            get { return _maxCount; }
            set { _maxCount = value; }
        }

        public ReadOnlyCollection<TValue> this[TKey key]
        {
            get { return GetHistory(key); }
        }

        public void Add(TKey key)
        {
            _history.Add(key, new List<TValue>());
        }

        public ReadOnlyCollection<TValue> GetHistory(TKey key)
        {
            return new ReadOnlyCollection<TValue>(_history[key]);
        }

        public void Update(TKey key, TValue value)
        {
            int maxCount = _maxCount == -1 ? HistoryManagerGlobal.GlobalMaxCount : _maxCount;

            _history[key].Insert(0, value);

            if (_history[key].Count > maxCount)
                _history[key].RemoveRange(maxCount, _history[key].Count - maxCount);
        }
    }
}
