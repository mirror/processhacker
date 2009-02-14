/*
 * Process Hacker - 
 *   memory provider
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
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace ProcessHacker
{
    public struct MemoryItem
    {
        public int Address;
        public string ModuleName;
        public int Size;
        public Win32.MEMORY_TYPE Type;
        public Win32.MEMORY_STATE State;
        public Win32.MEMORY_PROTECTION Protection;
    }

    public class MemoryProvider : Provider<int, MemoryItem>
    {
        private Win32.ProcessHandle _processHandle;
        private int _pid;

        public MemoryProvider(int PID)
            : base()
        {
            _pid = PID;

            try
            {
                _processHandle = new Win32.ProcessHandle(_pid, Win32.PROCESS_RIGHTS.PROCESS_QUERY_INFORMATION |
                    Win32.PROCESS_RIGHTS.PROCESS_VM_READ);
            }
            catch
            { }

            this.ProviderUpdate += new ProviderUpdateOnce(UpdateOnce);
            this.Killed += () => { if (_processHandle != null) _processHandle.Dispose(); };
        }

        private void UpdateOnce()
        {
            var modules = new Dictionary<int, Win32.ProcessModule>();

            try
            {
                foreach (var m in _processHandle.GetModules())
                    modules.Add(m.BaseAddress.ToInt32(), m);
            }
            catch
            { }

            Win32.ProcessHandle phandle = new Win32.ProcessHandle(_pid, Win32.PROCESS_RIGHTS.PROCESS_QUERY_INFORMATION);
            Dictionary<int, Win32.MEMORY_BASIC_INFORMATION> memoryInfo = new Dictionary<int, Win32.MEMORY_BASIC_INFORMATION>();
            Dictionary<int, MemoryItem> newdictionary = new Dictionary<int, MemoryItem>(this.Dictionary);

            {
                Win32.MEMORY_BASIC_INFORMATION info = new Win32.MEMORY_BASIC_INFORMATION();
                int address = 0;

                while (true)
                {
                    if (!Win32.VirtualQueryEx(phandle, address, ref info, Marshal.SizeOf(info)))
                    {
                        break;
                    }
                    else
                    {
                        if ((this.IgnoreFreeRegions && info.State != Win32.MEMORY_STATE.MEM_FREE) || 
                            (!this.IgnoreFreeRegions))
                            memoryInfo.Add(info.BaseAddress, info);

                        address += info.RegionSize;
                    }
                }
            }

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
                Win32.MEMORY_BASIC_INFORMATION info = memoryInfo[address];

                if (!Dictionary.ContainsKey(address))
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
                    MemoryItem item = Dictionary[address];
                    MemoryItem newitem = item;

                    newitem.Size = info.RegionSize;
                    newitem.Type = info.Type;
                    newitem.State = info.State;
                    newitem.Protection = info.Protect;

                    if (
                        newitem.Size != item.Size ||
                        newitem.Type != item.Type ||
                        newitem.State != item.State ||
                        newitem.Protection != item.Protection
                        )
                    {
                        newdictionary[address] = newitem;
                        this.CallDictionaryModified(item, newitem);
                    }
                }
            }

            phandle.Dispose();
            Dictionary = newdictionary;
        }

        public bool IgnoreFreeRegions { get; set; }

        public int PID
        {
            get { return _pid; }
        }
    }
}
