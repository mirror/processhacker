/*
 * Process Hacker - 
 *   struct searcher
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
using ProcessHacker.Common;
using ProcessHacker.Native;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;
using ProcessHacker.Structs;

namespace ProcessHacker
{
    public class StructSearcher : Searcher
    {
        public StructSearcher(int PID) : base(PID) { }

        public override void Search()
        {
            Results.Clear();

            ProcessHandle phandle;
            int count = 0;

            bool opt_priv = (bool)Params["private"];
            bool opt_img = (bool)Params["image"];
            bool opt_map = (bool)Params["mapped"];

            string structName = (string)Params["struct"];
            int align = (int)BaseConverter.ToNumberParse((string)Params["struct_align"]);

            if (!Program.Structs.ContainsKey(structName))
            {
                CallSearchError("Struct '" + structName + "' is not defined.");
                return;
            }

            StructDef structDef = Program.Structs[structName];
            string structLen = structDef.Size.ToString();

            structDef.IOProvider = new ProcessMemoryIO(PID);

            try
            {
                phandle = new ProcessHandle(PID, ProcessHacker.Native.Security.ProcessAccess.QueryInformation);
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

                    CallSearchProgressChanged(
                        String.Format("Searching 0x{0} ({1} found)...", info.BaseAddress.ToString("x"), count));

                    for (int i = 0; i < info.RegionSize.ToInt32(); i += align)
                    {
                        try
                        {
                            structDef.Offset = info.BaseAddress.Increment(i);
                            structDef.Read();

                            // read succeeded, add it to the results
                            Results.Add(new string[] { Utils.FormatAddress(info.BaseAddress),
                                String.Format("0x{0:x}", i), structLen, "" });
                            count++;
                        }
                        catch
                        { }
                    }

                    return true;
                });

            phandle.Dispose();

            CallSearchFinished();
        }
    }
}
