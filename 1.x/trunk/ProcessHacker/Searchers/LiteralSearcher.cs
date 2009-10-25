/*
 * Process Hacker - 
 *   literal searcher
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
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;

namespace ProcessHacker
{
    public class LiteralSearcher : Searcher
    {
        public LiteralSearcher(int PID) : base(PID) { }

        public override void Search()
        {
            Results.Clear();

            byte[] text = (byte[])Params["text"];
            ProcessHandle phandle;
            int count = 0;

            bool opt_priv = (bool)Params["private"];
            bool opt_img = (bool)Params["image"];
            bool opt_map = (bool)Params["mapped"];

            bool nooverlap = (bool)Params["nooverlap"];

            if (text.Length == 0)
            {
                CallSearchFinished();
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

                    for (int i = 0; i < bytesRead; i++)
                    {
                        bool good = true;

                        for (int j = 0; j < text.Length; j++)
                        {
                            if (i + j > bytesRead - 1)
                                continue;

                            if (data[i + j] != text[j])
                            {
                                good = false;
                                break;
                            }
                        }

                        if (good)
                        {
                            Results.Add(new string[] { Utils.FormatAddress(info.BaseAddress),
                                String.Format("0x{0:x}", i), text.Length.ToString(), "" });

                            count++;

                            if (nooverlap)
                                i += text.Length - 1;
                        }
                    }

                    data = null;

                    return true;
                });

            phandle.Dispose();

            CallSearchFinished();
        }
    }
}
