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
        public int Size;
        public Win32.MEMORY_TYPE Type;
        public Win32.MEMORY_STATE State;
        public Win32.MEMORY_PROTECTION Protection;
    }

    public class MemoryProvider : Provider<int, MemoryItem>
    {
        private int _pid;

        public MemoryProvider(int PID)
            : base()
        {
            _pid = PID;
            this.ProviderUpdate += new ProviderUpdateOnce(UpdateOnce);   
        }

        private void UpdateOnce()
        {
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

        public int PID
        {
            get { return _pid; }
        }
    }
}
