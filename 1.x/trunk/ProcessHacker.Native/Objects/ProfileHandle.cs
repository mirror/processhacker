/*
 * Process Hacker - 
 *   profile handle
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
using System.Runtime.InteropServices;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Security;

namespace ProcessHacker.Native.Objects
{
    public sealed class ProfileHandle : NativeHandle<ProfileAccess>
    {
        public static ProfileHandle Create(
            ProcessHandle processHandle,
            IntPtr rangeBase,
            uint rangeSize,
            int bucketSize,
            KProfileSource profileSource,
            IntPtr affinity
            )
        {
            NtStatus status;
            IntPtr handle;

            if (bucketSize < 2 || bucketSize > 30)
                throw new ArgumentException("Bucket size must be between 2 and 30, inclusive.");

            unchecked
            {
                uint realBucketSize = (uint)(2 << (bucketSize - 1));
                MemoryAlloc buffer = new MemoryAlloc((int)((rangeSize - 1) / realBucketSize + 1) * sizeof(int)); // divide, round up

                if ((status = Win32.NtCreateProfile(
                    out handle,
                    processHandle ?? IntPtr.Zero,
                    rangeBase,
                    new IntPtr(rangeSize),
                    bucketSize,
                    buffer,
                    buffer.Size,
                    profileSource,
                    affinity
                    )) >= NtStatus.Error)
                    Win32.Throw(status);

                return new ProfileHandle(handle, true, rangeBase, rangeSize, realBucketSize, buffer);
            }
        }

        public static int GetInterval(KProfileSource profileSource)
        {
            NtStatus status;
            int interval;

            if ((status = Win32.NtQueryIntervalProfile(profileSource, out interval)) >= NtStatus.Error)
                Win32.Throw(status);

            return interval;
        }

        public static void SetInterval(KProfileSource profileSource, int interval)
        {
            NtStatus status;

            if ((status = Win32.NtSetIntervalProfile(interval, profileSource)) >= NtStatus.Error)
                Win32.Throw(status);
        }

        private IntPtr _rangeBase;
        private uint _rangeSize;
        private uint _bucketSize; // not logarithmic
        private MemoryAlloc _buffer;

        private ProfileHandle(
            IntPtr handle,
            bool owned,
            IntPtr rangeBase,
            uint rangeSize,
            uint bucketSize,
            MemoryAlloc buffer
            )
            : base(handle, owned)
        {
            _rangeBase = rangeBase;
            _rangeSize = rangeSize;
            _bucketSize = bucketSize;
            _buffer = buffer;
        }

        protected override void Close()
        {
            _buffer.Dispose();

            base.Close();
        }

        public int[] Collect()
        {
            int[] counters = new int[_buffer.Size / sizeof(int)];

            Marshal.Copy(_buffer, counters, 0, counters.Length);

            return counters;
        }

        public void Start()
        {
            NtStatus status;

            if ((status = Win32.NtStartProfile(this)) >= NtStatus.Error)
                Win32.Throw(status);
        }

        public void Stop()
        {
            NtStatus status;

            if ((status = Win32.NtStopProfile(this)) >= NtStatus.Error)
                Win32.Throw(status);
        }
    }
}
