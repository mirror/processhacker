/*
 * Process Hacker - 
 *   regex searcher
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
using System.Text;
using System.Text.RegularExpressions;
using ProcessHacker.Common;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;

namespace ProcessHacker
{
    public class RegexSearcher : Searcher
    {
        public RegexSearcher(int PID) : base(PID) { }

        public override void Search()
        {
            Results.Clear();

            string regex = (string)Params["regex"];
            ProcessHandle phandle;
            int count = 0;

            RegexOptions options = RegexOptions.Singleline | RegexOptions.Compiled;
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

            try
            {
                phandle = new ProcessHandle(PID,
                    ProcessAccess.QueryInformation |
                    Program.MinProcessReadMemoryRights);
            }
            catch
            {
                CallSearchError("Could not open process: " + Win32.GetLastErrorMessage());
                return;
            }

            phandle.EnumMemory((info) =>
                {
                    // skip unreadable areas
                    if (info.Protect == MemoryProtection.AccessDenied)
                        return true;
                    if (info.State != MemoryState.Commit)
                        return true;

                    if ((!opt_priv) && (info.Type == MemoryType.Private))
                        return true;

                    if ((!opt_img) && (info.Type == MemoryType.Image))
                        return true;

                    if ((!opt_map) && (info.Type == MemoryType.Mapped))
                        return true;

                    byte[] data = new byte[info.RegionSize.ToInt32()];
                    int bytesRead = 0;

                    CallSearchProgressChanged(
                        String.Format("Searching 0x{0} ({1} found)...", info.BaseAddress.ToString("x"), count));

                    try
                    {
                        bytesRead = phandle.ReadMemory(info.BaseAddress, data, data.Length);

                        if (bytesRead == 0)
                            return true;
                    }
                    catch
                    {
                        return true;
                    }

                    StringBuilder sdata = new StringBuilder();
                    string sdata2 = "";

                    for (int i = 0; i < data.Length; i++)
                        sdata.Append((char)data[i]);

                    sdata2 = sdata.ToString();
                    sdata = null;

                    MatchCollection mc = rx.Matches(sdata2);

                    foreach (Match m in mc)
                    {
                        Results.Add(new string[] { Utils.FormatAddress(info.BaseAddress),
                                String.Format("0x{0:x}", m.Index), m.Length.ToString(),
                                Utils.MakePrintable(m.Value) });

                        count++;
                    }

                    data = null;

                    return true;
                });

            phandle.Dispose();

            CallSearchFinished();
        }
    }
}
