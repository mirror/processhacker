/*
 * Process Hacker - 
 *   heap searcher
 * 
 * Copyright (C) 2008-2009 wj32
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
using ProcessHacker.Common;
using ProcessHacker.Native.Api;

namespace ProcessHacker
{
    public class HeapSearcher : Searcher
    {
        public HeapSearcher(int PID) : base(PID) { }

        public override void Search()
        {
            Results.Clear();

            IntPtr snapshot;
            var hlist = new HeapList32();
            var heap = new HeapEntry32();
            int minsize;
            int count = 0;

            try
            {
                minsize = (int)BaseConverter.ToNumberParse((string)Params["h_ms"]);
            }
            catch (Exception ex)
            {
                CallSearchError("Unable to parse the minimum size argument: " + ex.Message);
                return;
            }

            snapshot = Win32.CreateToolhelp32Snapshot(SnapshotFlags.HeapList, PID);

            hlist.dwSize = Marshal.SizeOf(hlist);
            heap.dwSize = Marshal.SizeOf(heap);

            if (snapshot != IntPtr.Zero && Marshal.GetLastWin32Error() == 0)
            {
                Win32.Heap32ListFirst(snapshot, ref hlist);

                do
                {
                    Win32.Heap32First(ref heap, hlist.th32ProcessID, hlist.th32HeapID);

                    do
                    {
                        CallSearchProgressChanged(
                            String.Format("Searching 0x{0} ({1} found)...", heap.dwAddress.ToString("x"), count));

                        if (heap.dwBlockSize <= minsize)
                            continue;

                        Results.Add(new string[] { Utils.FormatAddress(heap.dwAddress),
                            "0x0", heap.dwBlockSize.ToString(), heap.dwFlags.ToString().Replace("LF32_", "") });

                        count++;
                    } while (Win32.Heap32Next(out heap) != 0);
                } while (Win32.Heap32ListNext(snapshot, out hlist));
            }
            else
            {
                CallSearchError(Win32.GetLastErrorMessage());
                return;
            }

            CallSearchFinished();
        }
    }
}
