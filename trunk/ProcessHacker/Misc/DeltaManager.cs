/*
 * Process Hacker
 * 
 * Copyright (C) 2008 wj32
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Diagnostics;
using System.Drawing;   
using System.Drawing.Drawing2D;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Aga.Controls.Tree;
using System.Collections.Generic;

namespace ProcessHacker
{
    public interface ISubtractor<T>
    {
        /// <summary>
        /// Subtracts v2 from v1, i.e., v1 - v2.
        /// </summary>
        T Subtract(T v1, T v2);
    }

    public class Int64Subtractor : ISubtractor<long>
    {
        public long Subtract(long v1, long v2)
        {
            return v1 - v2;
        }
    }

    public class Int32Subtractor : ISubtractor<int>
    {
        public int Subtract(int v1, int v2)
        {
            return v1 - v2;
        }
    }

    public class DoubleSubtractor : ISubtractor<double>
    {
        public double Subtract(double v1, double v2)
        {
            return v1 - v2;
        }
    }

    public class FloatSubtractor : ISubtractor<float>
    {
        public float Subtract(float v1, float v2)
        {
            return v1 - v2;
        }
    }

    /// <summary>
    /// Provides methods for managing discrete sets of data.
    /// </summary>
    public class DeltaManager<TKey, TValue>
    {
        private Dictionary<TKey, TValue> _values = new Dictionary<TKey, TValue>();
        private Dictionary<TKey, TValue> _deltas = new Dictionary<TKey, TValue>();
        private ISubtractor<TValue> _subtractor;

        public DeltaManager(ISubtractor<TValue> subtractor)
        {
            _subtractor = subtractor;
        }

        public TValue GetDelta(TKey key)
        {
            return _deltas[key];
        }

        public void Add(TKey key, TValue initialValue)
        {
            _values.Add(key, initialValue);
            _deltas.Add(key, initialValue);
        }

        public void SetDelta(TKey key, TValue value)
        {
            _deltas[key] = value;
        }

        public TValue Update(TKey key, TValue value)
        {
            _deltas[key] = _subtractor.Subtract(value, _values[key]);
            _values[key] = value;

            return _deltas[key];
        }
    }
}
