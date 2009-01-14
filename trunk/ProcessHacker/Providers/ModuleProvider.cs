/*
 * Process Hacker - 
 *   module provider
 * 
 * Copyright (C) 2008 wj32
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
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Text;

namespace ProcessHacker
{
    public struct ModuleItem
    {
        public int BaseAddress;
        public int Size;
        public string Name;
        public string FileName;
        public string FileDescription;
        public string FileVersion;
    }

    public class ModuleProvider : Provider<int, ModuleItem>
    {
        private int _pid;

        public ModuleProvider(int PID)
            : base()
        {
            _pid = PID;

            this.ProviderUpdate += new ProviderUpdateOnce(UpdateOnce);
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
                    StringBuilder name = new StringBuilder(256);
                    StringBuilder filename = new StringBuilder(256);

                    Win32.GetDeviceDriverBaseName(b, name, 255);
                    Win32.GetDeviceDriverFileName(b, filename, 255);

                    item.BaseAddress = b;
                    item.Name = name.ToString();
                    item.FileName = Misc.GetRealPath(filename.ToString());

                    try
                    {
                        System.IO.FileInfo fi = new System.IO.FileInfo(item.FileName);
                        item.FileName = fi.FullName;

                        FileVersionInfo info = FileVersionInfo.GetVersionInfo(item.FileName);

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
            Process process = Process.GetProcessById(_pid);
            ProcessModuleCollection modulesCollection = process.Modules;
            Dictionary<int, ProcessModule> modules = new Dictionary<int, ProcessModule>();
            Dictionary<int, ModuleItem> newdictionary = new Dictionary<int, ModuleItem>(this.Dictionary);

            foreach (ProcessModule m in modulesCollection)
                modules.Add(m.BaseAddress.ToInt32(), m);

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
                    ProcessModule m = modules[b];
                    ModuleItem item = new ModuleItem();

                    item.Name = m.ModuleName;

                    try { item.FileName = Misc.GetRealPath(m.FileName); }
                    catch { }
                    try { item.BaseAddress = b; }
                    catch { }
                    try { item.Size = m.ModuleMemorySize; }
                    catch { }

                    try
                    {
                        FileVersionInfo info = FileVersionInfo.GetVersionInfo(item.FileName);

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

        public int PID
        {
            get { return _pid; }
        }
    }
}
