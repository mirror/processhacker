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
using System.Text.RegularExpressions;

namespace ProcessHacker
{
    public class RegexSearcher : Searcher
    {
        public RegexSearcher(int PID) : base(PID) { }

        public override void Search()
        {
            Results.Clear();

            string regex = (string)Params["regex"];
            int handle = 0;
            int address = 0;
            Win32.MEMORY_BASIC_INFORMATION info = new Win32.MEMORY_BASIC_INFORMATION();
            int count = 0;

            RegexOptions options = RegexOptions.Singleline;
            Regex rx = null;

            bool opt_priv = (bool)Params["private"];
            bool opt_img = (bool)Params["image"];
            bool opt_map = (bool)Params["mapped"];

            if (regex.Length == 0)
            {
                CallSearchFinished();
                return;
            }

            try
            {
                if ((bool)Params["ignorecase"])
                    options |= RegexOptions.IgnoreCase;

                rx = new Regex(regex, options);
            }
            catch (Exception ex)
            {
                CallSearchError("Could not initialize regex: " + ex.Message);
                return;
            }

            handle = Win32.OpenProcess(Win32.PROCESS_RIGHTS.PROCESS_VM_READ |
                Win32.PROCESS_RIGHTS.PROCESS_QUERY_INFORMATION, 0, PID);

            if (handle == 0)
            {
                CallSearchError("Could not open process: " + Win32.GetLastErrorMessage());
                return;
            }

            while (true)
            {
                if (!Win32.VirtualQueryEx(handle, address, ref info,
                    Marshal.SizeOf(typeof(Win32.MEMORY_BASIC_INFORMATION))))
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

                    CallSearchProgressChanged(
                        String.Format("Searching 0x{0:x8} ({1} found)...", info.BaseAddress, count));

                    Win32.ReadProcessMemory(handle, info.BaseAddress, data, info.RegionSize, out bytesRead);

                    if (bytesRead == 0)
                        continue;

                    StringBuilder sdata = new StringBuilder();
                    string sdata2 = "";

                    for (int i = 0; i < data.Length; i++)
                        sdata.Append((char)data[i]);

                    sdata2 = sdata.ToString();
                    sdata = null;

                    MatchCollection mc = rx.Matches(sdata2);

                    foreach (Match m in mc)
                    {
                        Results.Add(new string[] { String.Format("0x{0:x8}", info.BaseAddress),
                                String.Format("0x{0:x8}", m.Index), m.Length.ToString(),
                                Misc.MakePrintable(m.Value) });

                        count++;
                    }

                    data = null;
                }
            }

            Win32.CloseHandle(handle);

            CallSearchFinished();
        }
    }
}
