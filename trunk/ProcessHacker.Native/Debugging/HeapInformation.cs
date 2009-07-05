/*
 * Process Hacker - 
 *   heap information
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
using ProcessHacker.Native.Api;

namespace ProcessHacker.Native.Debugging
{
    public class HeapInformation
    {
        internal HeapInformation(RtlHeapInformation heapInfo)
        {
            this.Address = heapInfo.BaseAddress;
            this.BytesAllocated = heapInfo.BytesAllocated.ToInt32();
            this.BytesCommitted = heapInfo.BytesCommitted.ToInt32();
            this.TagCount = heapInfo.NumberOfTags;
            this.EntryCount = heapInfo.NumberOfEntries;
            this.PseudoTagCount = heapInfo.NumberOfPseudoTags;
        }

        public IntPtr Address { get; private set; }
        public int BytesAllocated { get; private set; }
        public int BytesCommitted { get; private set; }
        public int TagCount { get; private set; }
        public int EntryCount { get; private set; }
        public int PseudoTagCount { get; private set; }
    }
}
