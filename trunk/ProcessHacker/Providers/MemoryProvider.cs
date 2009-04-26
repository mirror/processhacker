/*
 * Process Hacker - 
 *   memory provider
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
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;

namespace ProcessHacker
{
    public class MemoryItem : ICloneable
    {
        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public int Address;
        public string ModuleName;
        public int Size;
        public MemoryType Type;
        public MemoryState State;
        public MemoryProtection Protection;
    }

    public class MemoryProvider : Provider<int, MemoryItem>
    {
        private ProcessHandle _processHandle;
        private int _pid;

        public MemoryProvider(int pid)
            : base()
        {
            this.Name = this.GetType().Name;
            _pid = pid;

            try
            {
                _processHandle = new ProcessHandle(_pid, ProcessAccess.QueryInformation |
                    Program.MinProcessReadMemoryRights);
            }
            catch
            { }

            this.ProviderUpdate += new ProviderUpdateOnce(UpdateOnce);
            this.Disposed += (provider) => { if (_processHandle != null) _processHandle.Dispose(); };
        }

        private void UpdateOnce()
        {
            var modules = new Dictionary<int, ProcessModule>();

            try
            {
                foreach (var m in _processHandle.GetModules())
                    modules.Add(m.BaseAddress.ToInt32(), m);
            }
            catch
            { }

            var memoryInfo = new Dictionary<int, MemoryBasicInformation>();
            var newdictionary = new Dictionary<int, MemoryItem>(this.Dictionary);

            _processHandle.EnumMemory((info) =>
                {
                    if ((this.IgnoreFreeRegions && info.State != MemoryState.Free) ||
                        !this.IgnoreFreeRegions)
                        memoryInfo.Add(info.BaseAddress, info);

                    return true;
                });

            // look for freed memory regions
            foreach (int address in Dictionary.Keys)
            {
                if (!memoryInfo.ContainsKey(address))
                {
                    this.CallDictionaryRemoved(this.Dictionary[address]);
                    newdictionary.Remove(address);
                }
            }

            string lastModuleName = null;
            int lastModuleAddress = 0;
            int lastModuleSize = 0;

            foreach (int address in memoryInfo.Keys)
            {
                var info = memoryInfo[address];

                if (!this.Dictionary.ContainsKey(address))
                {
                    MemoryItem item = new MemoryItem();

                    item.Address = address;
                    item.Size = info.RegionSize;
                    item.Type = info.Type;
                    item.State = info.State;
                    item.Protection = info.Protect;

                    if (modules.ContainsKey(item.Address))
                    {
                        lastModuleName = modules[item.Address].BaseName; 
                        lastModuleAddress = modules[item.Address].BaseAddress.ToInt32();
                        lastModuleSize = modules[item.Address].Size;
                    }

                    if (item.Address >= lastModuleAddress && item.Address < lastModuleAddress + lastModuleSize)
                        item.ModuleName = lastModuleName;
                    else
                        item.ModuleName = null;
                                                    
                    newdictionary.Add(address, item);
                    this.CallDictionaryAdded(item);
                }
                else
                {
                    MemoryItem item = this.Dictionary[address];

                    if (
                        info.RegionSize != item.Size ||
                        info.Type != item.Type ||
                        info.State != item.State ||
                        info.Protect != item.Protection
                        )
                    {
                        MemoryItem newitem = item.Clone() as MemoryItem;

                        newitem.Size = info.RegionSize;
                        newitem.Type = info.Type;
                        newitem.State = info.State;
                        newitem.Protection = info.Protect;

                        newdictionary[address] = newitem;
                        this.CallDictionaryModified(item, newitem);
                    }
                }
            }

            this.Dictionary = newdictionary;
        }

        public bool IgnoreFreeRegions { get; set; }

        public int Pid
        {
            get { return _pid; }
        }
    }
}
