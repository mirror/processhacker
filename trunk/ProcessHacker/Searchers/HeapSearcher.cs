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
using System.Runtime.InteropServices;

namespace ProcessHacker
{
    public class HeapSearcher : Searcher
    {
        public HeapSearcher(int PID) : base(PID) { }
        public override event SearchFinished SearchFinished;
        public override event SearchProgressChanged SearchProgressChanged;
        public override event SearchError SearchError;
        void h_SearchProgressChanged(string progress) { }
        void h_SearchFinished() { }

        public override void Search()
        {
            Results.Clear();

            SearchFinished += new SearchFinished(h_SearchFinished);
            SearchProgressChanged += new SearchProgressChanged(h_SearchProgressChanged);

            int snapshot;
            Win32.HEAPLIST32 hlist = new Win32.HEAPLIST32();
            Win32.HEAPENTRY32 heap = new Win32.HEAPENTRY32();
            int minsize = (int)BaseConverter.ToNumberParse((string)Params["h_ms"]);
            int count = 0;

            snapshot = Win32.CreateToolhelp32Snapshot(Win32.SnapshotFlags.HeapList, PID);

            hlist.dwSize = Marshal.SizeOf(typeof(Win32.HEAPLIST32));
            heap.dwSize = Marshal.SizeOf(typeof(Win32.HEAPENTRY32));

            if (snapshot != 0 && Marshal.GetLastWin32Error() == 0)
            {
                Win32.Heap32ListFirst(snapshot, ref hlist);

                do
                {
                    Win32.Heap32First(ref heap, hlist.th32ProcessID, hlist.th32HeapID);

                    do
                    {
                        this.SearchProgressChanged(
                            String.Format("Searching 0x{0:x8} ({1} found)...", heap.dwAddress, count));

                        if (heap.dwBlockSize <= minsize)
                            continue;

                        Results.Add(new string[] { String.Format("0x{0:x8}", heap.dwAddress),
                            "0x00000000", heap.dwBlockSize.ToString(), heap.dwFlags.ToString().Replace("LF32_", "") });

                        count++;
                    } while (Win32.Heap32Next(ref heap) != 0);
                } while (Win32.Heap32ListNext(snapshot, ref hlist) != 0);
            }
            else if (Marshal.GetLastWin32Error() == 5)
            {
                SearchError("Access is denied");
                return;                   
            }
            else
            {
                SearchError("Error " + Marshal.GetLastWin32Error().ToString());
                return;
            }

            SearchFinished();
        }
    }
}
