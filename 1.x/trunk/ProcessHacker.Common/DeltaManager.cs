/*
 * Process Hacker - 
 *   delta manager
 * 
 * Copyright (C) 2008-2010 wj32
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

using System.Collections.Generic;

namespace ProcessHacker.Common
{
    /// <summary>
    /// Defines subtraction for a numeric type.
    /// </summary>
    /// <typeparam name="T">The numeric type.</typeparam>
    public interface ISubtractor<T>
    {
        /// <summary>
        /// Subtracts v2 from v1, i.e., v1 - v2.
        /// </summary>
        T Subtract(T v1, T v2);
    }

    public static class Subtractor
    {
        private static Int64Subtractor _int64Subtractor = new Int64Subtractor();
        private static Int32Subtractor _int32Subtractor = new Int32Subtractor();
        private static DoubleSubtractor _doubleSubtractor = new DoubleSubtractor();
        private static FloatSubtractor _floatSubtractor = new FloatSubtractor();

        public static Int64Subtractor Int64Subtractor
        {
            get { return _int64Subtractor; }
        }

        public static Int32Subtractor Int32Subtractor
        {
            get { return _int32Subtractor; }
        }

        public static DoubleSubtractor DoubleSubtractor
        {
            get { return _doubleSubtractor; }
        }

        public static FloatSubtractor FloatSubtractor
        {
            get { return _floatSubtractor; }
        }
    }

    /// <summary>
    /// Provides subtraction for 64-bit integers.
    /// </summary>
    public class Int64Subtractor : ISubtractor<long>
    {
        public long Subtract(long v1, long v2)
        {
            return v1 - v2;
        }
    }

    /// <summary>
    /// Provides subtraction for 32-bit integers.
    /// </summary>
    public class Int32Subtractor : ISubtractor<int>
    {
        public int Subtract(int v1, int v2)
        {
            return v1 - v2;
        }
    }

    /// <summary>
    /// Provides subtraction for double-precision floating-point values.
    /// </summary>
    public class DoubleSubtractor : ISubtractor<double>
    {
        public double Subtract(double v1, double v2)
        {
            return v1 - v2;
        }
    }

    /// <summary>
    /// Provides subtraction for single-precision floating-point values.
    /// </summary>
    public class FloatSubtractor : ISubtractor<float>
    {
        public float Subtract(float v1, float v2)
        {
            return v1 - v2;
        }
    }

    public interface IDeltaValue<T>
    {
        T Value { get; }
        T Delta { get; }
        void Update(T value);
    }

    public struct DeltaValue<T> : IDeltaValue<T>
    {
        private ISubtractor<T> _subtractor;
        private T _value;
        private T _delta;

        public DeltaValue(ISubtractor<T> subtractor, T value)
        {
            _subtractor = subtractor;
            _value = value;
            _delta = default(T);
        }

        public T Value
        {
            get { return _value; }
        }

        public T Delta
        {
            get { return _delta; }
        }

        public void Update(T value)
        {
            _delta = _subtractor.Subtract(value, _value);
            _value = value;
        }
    }

    public struct Int64Delta : IDeltaValue<long>
    {
        public static void Update(ref Int64Delta delta, long value)
        {
            delta._delta = value - delta._value;
            delta._value = value;
        }

        private long _value;
        private long _delta;

        public Int64Delta(long value)
        {
            _value = value;
            _delta = 0;
        }

        public long Value
        {
            get { return _value; }
        }

        public long Delta
        {
            get { return _delta; }
        }

        public void Update(long value)
        {
            _delta = value - _value;
            _value = value;
        }
    }
}
