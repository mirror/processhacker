/*
 * Process Hacker - 
 *   string searcher
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
using ProcessHacker.Common;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;

namespace ProcessHacker
{
    public class StringSearcher : Searcher
    {
        public StringSearcher(int PID) : base(PID) { }

        private bool IsChar(byte b)
        {
            return (b >= ' ' && b <= '~') || b == '\n' || b == '\r' || b == '\t';
        }

        public override void Search()
        {
            Results.Clear();

            byte[] text = (byte[])Params["text"];
            ProcessHandle phandle;
            int count = 0;

            int minsize = (int)BaseConverter.ToNumberParse((string)Params["s_ms"]);
            bool unicode = (bool)Params["unicode"];

            bool opt_priv = (bool)Params["private"];
            bool opt_img = (bool)Params["image"];
            bool opt_map = (bool)Params["mapped"];

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

                    StringBuilder curstr = new StringBuilder();
                    bool isUnicode = false;
                    byte byte2 = 0;
                    byte byte1 = 0;

                    for (int i = 0; i < bytesRead; i++)
                    {
                        bool isChar = IsChar(data[i]);

                        if (unicode && isChar && isUnicode && byte1 != 0)
                        {
                            isUnicode = false;

                            if (curstr.Length > 0)
                                curstr.Remove(curstr.Length - 1, 1);

                            curstr.Append((char)data[i]);
                        }
                        else if (isChar)
                        {
                            curstr.Append((char)data[i]);
                        }
                        else if (unicode && data[i] == 0 && IsChar(byte1) && !IsChar(byte2))
                        {
                            // skip null byte
                            isUnicode = true;
                        }
                        else if (unicode &&
                            data[i] == 0 && IsChar(byte1) && IsChar(byte2) && curstr.Length < minsize)
                        {
                            // ... [char] [char] *[null]* ([char] [null] [char] [null]) ...
                            //                   ^ we are here
                            isUnicode = true;
                            curstr = new StringBuilder();
                            curstr.Append((char)byte1);
                        }
                        else
                        {
                            if (curstr.Length >= minsize)
                            {
                                int length = curstr.Length;

                                if (isUnicode)
                                    length *= 2;

                                Results.Add(new string[] { Utils.FormatAddress(info.BaseAddress),
                                    String.Format("0x{0:x}", i - length), length.ToString(), 
                                    curstr.ToString() });

                                count++;
                            }

                            isUnicode = false;
                            curstr = new StringBuilder();
                        }

                        byte2 = byte1;
                        byte1 = data[i];
                    }

                    data = null;

                    return true;
                });

            phandle.Dispose();

            CallSearchFinished();
        }
    }
}
