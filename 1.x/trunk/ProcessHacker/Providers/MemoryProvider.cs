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
using ProcessHacker.Native;
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

        public int RunId;
        public IntPtr Address;
        public string ModuleName;
        public long Size;
        public MemoryType Type;
        public MemoryState State;
        public MemoryProtection Protection;

    }

    public class MemoryProvider : Provider<IntPtr, MemoryItem>
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

            this.Disposed += (provider) => { if (_processHandle != null) _processHandle.Dispose(); };
        }

        protected override void Update()
        {
            if (_processHandle == null)
                return;

            var modules = new Dictionary<IntPtr, ProcessModule>();

            try
            {
                foreach (var m in _processHandle.GetModules())
                    modules.Add(m.BaseAddress, m);
            }
            catch
            { }

            var memoryInfo = new Dictionary<IntPtr, MemoryBasicInformation>();
            var newdictionary = new Dictionary<IntPtr, MemoryItem>(this.Dictionary);

            _processHandle.EnumMemory((info) =>
                {
                    if ((this.IgnoreFreeRegions && info.State != MemoryState.Free) ||
                        !this.IgnoreFreeRegions)
                        memoryInfo.Add(info.BaseAddress, info);

                    return true;
                });

            // look for freed memory regions
            foreach (IntPtr address in Dictionary.Keys)
            {
                if (!memoryInfo.ContainsKey(address))
                {
                    this.OnDictionaryRemoved(this.Dictionary[address]);
                    newdictionary.Remove(address);
                }
            }

            string lastModuleName = null;
            IntPtr lastModuleAddress = IntPtr.Zero;
            int lastModuleSize = 0;

            foreach (IntPtr address in memoryInfo.Keys)
            {
                var info = memoryInfo[address];

                if (!this.Dictionary.ContainsKey(address))
                {
                    MemoryItem item = new MemoryItem();

                    item.RunId = this.RunCount;
                    item.Address = address;
                    item.Size = info.RegionSize.ToInt64();
                    item.Type = info.Type;
                    item.State = info.State;
                    item.Protection = info.Protect;

                    if (modules.ContainsKey(item.Address))
                    {
                        lastModuleName = modules[item.Address].BaseName; 
                        lastModuleAddress = modules[item.Address].BaseAddress;
                        lastModuleSize = modules[item.Address].Size;
                    }

                    if (
                        item.Address.IsGreaterThanOrEqualTo(lastModuleAddress) &&
                        item.Address.CompareTo(lastModuleAddress.Increment(lastModuleSize)) == -1
                        )
                        item.ModuleName = lastModuleName;
                    else
                        item.ModuleName = null;
                                                    
                    newdictionary.Add(address, item);
                    this.OnDictionaryAdded(item);
                }
                else
                {
                    MemoryItem item = this.Dictionary[address];

                    if (
                        info.RegionSize.ToInt64() != item.Size ||
                        info.Type != item.Type ||
                        info.State != item.State ||
                        info.Protect != item.Protection
                        )
                    {
                        MemoryItem newitem = item.Clone() as MemoryItem;

                        newitem.Size = info.RegionSize.ToInt64();
                        newitem.Type = info.Type;
                        newitem.State = info.State;
                        newitem.Protection = info.Protect;

                        newdictionary[address] = newitem;
                        this.OnDictionaryModified(item, newitem);
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
