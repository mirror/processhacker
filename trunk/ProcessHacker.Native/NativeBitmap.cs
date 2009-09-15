/*
 * Process Hacker - 
 *   bitmap
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
using ProcessHacker.Common;
using ProcessHacker.Common.Objects;
using ProcessHacker.Native.Api;

namespace ProcessHacker.Native
{
    public class NativeBitmap : BaseObject
    {
        public struct BitmapRun
        {
            public BitmapRun(int index, int length)
            {
                _index = index;
                _length = length;
            }

            private int _index;
            private int _length;

            public int Index
            {
                get { return _index; }
                set { _index = value; }
            }

            public int Length
            {
                get { return _length; }
                set { _length = value; }
            }
        }

        private RtlBitmap _bitmap;
        private MemoryAlloc _buffer;

        public NativeBitmap(int bits)
        {
            if (bits <= 0)
                throw new ArgumentOutOfRangeException("The number of bits must be positive.");

            _buffer = new MemoryAlloc(Utils.DivideUp(bits, 32) * 4);
            Win32.RtlInitializeBitMap(out _bitmap, _buffer, bits);
        }

        protected override void DisposeObject(bool disposing)
        {
            if (_buffer != null)
                _buffer.Dispose(disposing);
        }

        public bool AreClear(int index, int length)
        {
            return Win32.RtlAreBitsClear(ref _bitmap, index, length);
        }

        public bool AreSet(int index, int length)
        {
            return Win32.RtlAreBitsSet(ref _bitmap, index, length);
        }

        public int Check(int index)
        {
            return Win32.RtlCheckBit(ref _bitmap, index);
        }

        public void Clear()
        {
            Win32.RtlClearAllBits(ref _bitmap);
        }

        public void Clear(int index)
        {
            Win32.RtlClearBit(ref _bitmap, index);
        }

        public void Clear(int index, int length)
        {
            Win32.RtlClearBits(ref _bitmap, index, length);
        }

        public int FindClear(int length)
        {
            return this.FindClear(length, 0);
        }

        public int FindClear(int length, int hintIndex)
        {
            return Win32.RtlFindClearBits(ref _bitmap, length, hintIndex);
        }

        public int FindClearAndSet(int length)
        {
            return this.FindClearAndSet(length, 0);
        }

        public int FindClearAndSet(int length, int hintIndex)
        {
            return Win32.RtlFindClearBitsAndSet(ref _bitmap, length, hintIndex);
        }

        public BitmapRun[] FindClearRuns(int count)
        {
            return this.FindClearRuns(count, false);
        }

        public BitmapRun[] FindClearRuns(int count, bool locateLongest)
        {
            RtlBitmapRun[] runs = new RtlBitmapRun[count];
            int numberOfRuns;

            numberOfRuns = Win32.RtlFindClearRuns(ref _bitmap, runs, count, locateLongest);

            BitmapRun[] returnRuns = new BitmapRun[numberOfRuns];

            for (int i = 0; i < numberOfRuns; i++)
                returnRuns[i] = new BitmapRun(runs[i].StartingIndex, runs[i].NumberOfBits);

            return returnRuns;
        }

        public BitmapRun FindBackwardClearRun(int index)
        {
            int startingIndex;
            int numberOfBits;

            numberOfBits = Win32.RtlFindLastBackwardRunClear(ref _bitmap, index, out startingIndex);

            return new BitmapRun(startingIndex, numberOfBits);
        }

        public BitmapRun FindFirstClearRun()
        {
            int startingIndex;
            int numberOfBits;

            numberOfBits = Win32.RtlFindFirstRunClear(ref _bitmap, out startingIndex);

            return new BitmapRun(startingIndex, numberOfBits);
        }

        public BitmapRun FindForwardClearRun(int index)
        {
            int startingIndex;
            int numberOfBits;

            numberOfBits = Win32.RtlFindNextForwardRunClear(ref _bitmap, index, out startingIndex);

            return new BitmapRun(startingIndex, numberOfBits);
        }

        public BitmapRun FindLongestClearRun()
        {
            int startingIndex;
            int numberOfBits;

            numberOfBits = Win32.RtlFindLongestRunClear(ref _bitmap, out startingIndex);

            return new BitmapRun(startingIndex, numberOfBits);
        }

        public int FindSet(int length)
        {
            return this.FindSet(length, 0);
        }

        public int FindSet(int length, int hintIndex)
        {
            return Win32.RtlFindSetBits(ref _bitmap, length, hintIndex);
        }

        public int FindSetAndClear(int length)
        {
            return this.FindSetAndClear(length, 0);
        }

        public int FindSetAndClear(int length, int hintIndex)
        {
            return Win32.RtlFindSetBitsAndClear(ref _bitmap, length, hintIndex);
        }

        public int GetClearCount()
        {
            return Win32.RtlNumberOfClearBits(ref _bitmap);
        }

        public int GetSetCount()
        {
            return Win32.RtlNumberOfSetBits(ref _bitmap);
        }

        public void Set()
        {
            Win32.RtlSetAllBits(ref _bitmap);
        }

        public void Set(int index)
        {
            Win32.RtlSetBit(ref _bitmap, index);
        }

        public void Set(int index, int length)
        {
            Win32.RtlSetBits(ref _bitmap, index, length);
        }

        public bool Test(int index)
        {
            return Win32.RtlTestBit(ref _bitmap, index);
        }
    }
}
