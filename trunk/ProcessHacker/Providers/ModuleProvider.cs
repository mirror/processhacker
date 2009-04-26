/*
 * Process Hacker - 
 *   module provider
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
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using ProcessHacker.Native;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;

namespace ProcessHacker
{
    public class ModuleItem : ICloneable
    {
        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public int BaseAddress;
        public int Size;
        public string Name;
        public string FileName;
        public string FileDescription;
        public string FileVersion;
    }

    public class ModuleProvider : Provider<int, ModuleItem>
    {
        private ProcessHandle _processHandle;
        private int _pid;

        public ModuleProvider(int pid)
            : base()
        {
            this.Name = this.GetType().Name;
            _pid = pid;

            try
            {
                _processHandle = new ProcessHandle(_pid, 
                    ProcessAccess.QueryInformation | Program.MinProcessReadMemoryRights);
            }
            catch
            {
                try
                {
                    _processHandle = new ProcessHandle(_pid,
                        Program.MinProcessQueryRights | Program.MinProcessReadMemoryRights);
                }
                catch
                { }
            }

            this.ProviderUpdate += new ProviderUpdateOnce(UpdateOnce);
            this.Disposed += (provider) => { if (_processHandle != null) _processHandle.Dispose(); };
        }

        private void UpdateOnce()
        {
            if (_pid == 4)
                this.UpdateDrivers();
            else
                this.UpdateModules();
        }

        private void UpdateDrivers()
        {
            int requiredSize = 0;
            int[] imageBases;
            List<int> done = new List<int>();
            Dictionary<int, object> bases = new Dictionary<int, object>();
            Dictionary<int, ModuleItem> newdictionary = new Dictionary<int, ModuleItem>(this.Dictionary);

            Win32.EnumDeviceDrivers(null, 0, out requiredSize);
            imageBases = new int[requiredSize];
            Win32.EnumDeviceDrivers(imageBases, requiredSize * sizeof(int), out requiredSize);

            for (int i = 0; i < requiredSize; i++)
            {
                if (bases.ContainsKey(imageBases[i]) || imageBases[i] == 0)
                    continue;

                bases.Add(imageBases[i], null);
            }

            // look for unloaded drivers
            foreach (int b in Dictionary.Keys)
            {
                if (!bases.ContainsKey(b))
                {
                    this.CallDictionaryRemoved(this.Dictionary[b]);
                    newdictionary.Remove(b);
                }
            }

            // look for new drivers
            foreach (int b in bases.Keys)
            {
                if (!Dictionary.ContainsKey(b))
                {
                    ModuleItem item = new ModuleItem();
                    StringBuilder name = new StringBuilder(0x400);
                    StringBuilder filename = new StringBuilder(0x400);

                    Win32.GetDeviceDriverBaseName(b, name, name.Capacity * 2);
                    Win32.GetDeviceDriverFileName(b, filename, filename.Capacity * 2);

                    item.BaseAddress = b;
                    item.Name = name.ToString();
                    item.FileName = Misc.GetRealPath(filename.ToString());

                    try
                    {
                        System.IO.FileInfo fi = new System.IO.FileInfo(item.FileName);
                        item.FileName = fi.FullName;

                        var info = System.Diagnostics.FileVersionInfo.GetVersionInfo(item.FileName);

                        item.FileDescription = info.FileDescription;
                        item.FileVersion = info.FileVersion;
                    }
                    catch
                    { }

                    newdictionary.Add(b, item);
                    this.CallDictionaryAdded(item);
                }
            }

            this.Dictionary = newdictionary;
        }

        private void UpdateModules()
        {
            var processModules = _processHandle.GetModules();
            var modules = new Dictionary<int, ProcessModule>();
            var newdictionary = new Dictionary<int, ModuleItem>(this.Dictionary);

            foreach (var m in processModules)
                modules.Add(m.BaseAddress.ToInt32(), m);

            // add mapped files
            _processHandle.EnumMemory((info) =>
                {
                    if (info.Type == MemoryType.Mapped)
                    {
                        try
                        {
                            string fileName = _processHandle.GetMappedFileName(info.BaseAddress);

                            if (fileName != null)
                            {
                                var fi = new System.IO.FileInfo(fileName);

                                modules.Add(info.BaseAddress,
                                    new ProcessModule(
                                        new IntPtr(info.BaseAddress),
                                        info.RegionSize, IntPtr.Zero,
                                        fi.Name, fi.FullName));
                            }
                        }
                        catch
                        { }
                    }

                    return true;
                });

            // look for unloaded modules
            foreach (int b in Dictionary.Keys)
            {
                if (!modules.ContainsKey(b))
                {
                    this.CallDictionaryRemoved(this.Dictionary[b]);
                    newdictionary.Remove(b);
                }
            }

            // look for new modules
            foreach (int b in modules.Keys)
            {
                if (!Dictionary.ContainsKey(b))
                {
                    var m = modules[b];
                    ModuleItem item = new ModuleItem();

                    item.Name = m.BaseName;

                    try
                    {
                        item.FileName = Misc.GetRealPath(m.FileName);
                    }
                    catch
                    { }

                    item.BaseAddress = b;
                    item.Size = m.Size;

                    try
                    {
                        var info = System.Diagnostics.FileVersionInfo.GetVersionInfo(item.FileName);

                        item.FileDescription = info.FileDescription;
                        item.FileVersion = info.FileVersion;
                    }
                    catch
                    { }
                          
                    newdictionary.Add(b, item);
                    this.CallDictionaryAdded(item);
                }
            }

            this.Dictionary = newdictionary;
        }

        public int Pid
        {
            get { return _pid; }
        }
    }
}
