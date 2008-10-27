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
using System.Text;

namespace ProcessHacker
{
    public class StringSearcher : Searcher
    {
        public StringSearcher(int PID) : base(PID) { }
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

            byte[] text = (byte[])Params["text"];
            int handle = 0;
            int address = 0;
            Win32.MEMORY_BASIC_INFORMATION info = new Win32.MEMORY_BASIC_INFORMATION();
            int count = 0;

            int minsize = (int)BaseConverter.ToNumberParse((string)Params["s_ms"]);

            bool opt_priv = (bool)Params["private"];
            bool opt_img = (bool)Params["image"];
            bool opt_map = (bool)Params["mapped"];

            handle = Win32.OpenProcess(Win32.PROCESS_RIGHTS.PROCESS_VM_READ | 
                Win32.PROCESS_RIGHTS.PROCESS_QUERY_INFORMATION, 0, PID);

            if (handle == 0)
            {
                SearchError("Could not open process");
                return;
            }

            while (true)
            {
                if (Win32.VirtualQueryEx(handle, address, ref info,
                    Marshal.SizeOf(typeof(Win32.MEMORY_BASIC_INFORMATION))) == 0)
                {
                    break;
                }
                else
                {
                    address += info.RegionSize;

                    // skip unreadable areas
                    if (info.Protect == Win32.MEMORY_PROTECTION.PAGE_ACCESS_DENIED)
                        continue;
                    if (info.Protect == Win32.MEMORY_PROTECTION.PAGE_ACCESS_DENIED)
                        continue;
                    if (info.State != Win32.MEMORY_STATE.MEM_COMMIT)
                        continue;

                    if ((!opt_priv) && (info.Type == Win32.MEMORY_TYPE.MEM_PRIVATE))
                        continue;

                    if ((!opt_img) && (info.Type == Win32.MEMORY_TYPE.MEM_IMAGE))
                        continue;

                    if ((!opt_map) && (info.Type == Win32.MEMORY_TYPE.MEM_MAPPED))
                        continue;

                    byte[] data = new byte[info.RegionSize];
                    int bytesRead = 0;

                    this.SearchProgressChanged(
                        String.Format("Searching 0x{0:x8} ({1} found)...", info.BaseAddress, count));

                    Win32.ReadProcessMemory(handle, info.BaseAddress, data, info.RegionSize, ref bytesRead);

                    if (bytesRead == 0)
                        continue;

                    StringBuilder curstr = new StringBuilder();
                                               
                    for (int i = 0; i < bytesRead; i++)
                    {
                        if ((data[i] >= ' ' && data[i] <= '~') || data[i] == '\n' || data[i] == '\r' ||
                            data[i] == '\t')
                        {
                            curstr.Append(UnicodeEncoding.ASCII.GetChars(data, i, 1)[0]);
                        }
                        else
                        {
                            if (curstr.Length >= minsize)
                            {
                                Results.Add(new string[] { String.Format("0x{0:x8}", info.BaseAddress),
                                String.Format("0x{0:x8}", i - curstr.Length), curstr.Length.ToString(), curstr.ToString() });

                                count++;
                            }

                            curstr = new StringBuilder();
                        }
                    }

                    data = null;
                }
            }

            Win32.CloseHandle(handle);

            SearchFinished();
        }
    }
}
