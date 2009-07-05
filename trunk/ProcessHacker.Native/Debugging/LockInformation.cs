/*
 * Process Hacker - 
 *   lock information
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
    public class LockInformation
    {
        internal LockInformation(RtlProcessLockInformation lockInfo)
        {
            this.Address = lockInfo.Address;
            this.Type = lockInfo.Type;
            this.OwningThreadId = lockInfo.OwningThread.ToInt32();
            this.LockCount = lockInfo.LockCount;
            this.ContentionCount = lockInfo.ContentionCount;
            this.EntryCount = lockInfo.EntryCount;

            this.RecursionCount = lockInfo.RecursionCount;

            this.SharedWaiters = lockInfo.NumberOfWaitingShared;
            this.ExclusiveWaiters = lockInfo.NumberOfWaitingExclusive;
        }

        public IntPtr Address { get; private set; }
        public RtlLockType Type { get; private set; }
        public int OwningThreadId { get; private set; }
        public int LockCount { get; private set; }
        public int ContentionCount { get; private set; }
        public int EntryCount { get; private set; }

        public int RecursionCount { get; private set; }

        public int SharedWaiters { get; private set; }
        public int ExclusiveWaiters { get; private set; }
    }
}
