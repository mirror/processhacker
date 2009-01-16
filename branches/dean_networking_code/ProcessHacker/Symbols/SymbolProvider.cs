/*
 * Process Hacker - 
 *   symbol server
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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using ProcessHacker.PE;

namespace ProcessHacker
{
    /// <summary>
    /// Provides services for retrieving symbol information.
    /// </summary>
    public class SymbolProvider
    {
        public static SymbolProvider BaseInstance { get; private set; }

        static SymbolProvider()
        {
            SymbolProvider.BaseInstance = new SymbolProvider();
        }

        /// <summary>
        /// Specifies the detail with which the address's name was resolved.
        /// </summary>
        public enum FoundLevel
        {
            /// <summary>
            /// Indicates that the address was resolved to a module, a function and possibly an offset. 
            /// For example: mymodule.dll!MyExportedFunction+0x123
            /// </summary>
            Function,

            /// <summary>
            /// Indicates that the address was resolved to a module and an offset.
            /// For example: mymodule.dll+0x4321
            /// </summary>
            Module,

            /// <summary>
            /// Indicates that the address was not resolved.
            /// For example: 0x12345678
            /// </summary>
            Address,

            /// <summary>
            /// Indicates that the address was invalid (for example, 0x0).
            /// </summary>
            Invalid
        }

        private List<KeyValuePair<int, string>> _libraryLookup;
        private Dictionary<string, List<KeyValuePair<int, string>>> _symbols;
        private Dictionary<string, uint> _librarySizes;

        public SymbolProvider()
        {
            if (SymbolProvider.BaseInstance != null)
            {
                _libraryLookup = new List<KeyValuePair<int, string>>(SymbolProvider.BaseInstance._libraryLookup);
                _symbols = new Dictionary<string, List<KeyValuePair<int, string>>>();

                foreach (string k in SymbolProvider.BaseInstance._symbols.Keys)
                    _symbols.Add(k, new List<KeyValuePair<int, string>>(SymbolProvider.BaseInstance._symbols[k]));

                _librarySizes = new Dictionary<string, uint>(SymbolProvider.BaseInstance._librarySizes);
            }
            else
            {
                _libraryLookup = new List<KeyValuePair<int, string>>();
                _symbols = new Dictionary<string, List<KeyValuePair<int, string>>>();
                _librarySizes = new Dictionary<string, uint>();
            }
        }

        public void LoadSymbolsFromLibrary(string path)
        {
            LoadSymbolsFromLibrary(path, Process.GetCurrentProcess().Modules);
        }

        public void LoadSymbolsFromLibrary(string path, ProcessModuleCollection modules)
        {
            string realPath = Misc.GetRealPath(path).ToLower();
            int imageBase = -1;

            foreach (ProcessModule module in modules)
            {
                string thisPath = Misc.GetRealPath(module.FileName).ToLower();

                if (thisPath == realPath)
                {
                    imageBase = module.BaseAddress.ToInt32();
                    break;
                }
            }

            if (imageBase == -1)
                throw new Exception("Could not get image base of library.");

            LoadSymbolsFromLibrary(path, imageBase);
        }

        public void LoadSymbolsFromLibrary(string path, int imageBase)
        {
            string realPath = Misc.GetRealPath(path).ToLower();

            // check if it is already loaded
            if (_symbols.ContainsKey(realPath))
                return;

            PEFile file = null;
            List<KeyValuePair<int, string>> list = new List<KeyValuePair<int,string>>();

            try
            {
                file = new PEFile(realPath);

                uint size = 0;

                foreach (SectionHeader sh in file.Sections)
                    size += sh.VirtualSize;

                _librarySizes.Add(realPath, size);
            }
            catch
            { }

            if (file == null || file.ExportData == null)
            {
                // no symbols (or error), but we can still display a module name in a lookup
                _libraryLookup.Add(new KeyValuePair<int, string>(imageBase, realPath));
                _symbols.Add(realPath, new List<KeyValuePair<int, string>>());

                // if we didn't even get to load the PE file
                if (!_librarySizes.ContainsKey(realPath))
                    _librarySizes.Add(realPath, 0x7fffffff);
            }
            else
            {
                for (int i = 0; i < file.ExportData.ExportOrdinalTable.Count; i++)
                {
                    ushort ordinal = file.ExportData.ExportOrdinalTable[i];

                    if (ordinal >= file.ExportData.ExportAddressTable.Count)
                        continue;

                    int address = (int)file.ExportData.ExportAddressTable[ordinal].ExportRVA;

                    string name;

                    if (i < file.ExportData.ExportNameTable.Count)
                        name = file.ExportData.ExportNameTable[i];
                    else
                        name = ordinal.ToString();

                    list.Add(new KeyValuePair<int, string>(imageBase + address, name));
                }

                _libraryLookup.Add(new KeyValuePair<int, string>(imageBase, realPath));
                _symbols.Add(realPath, list);
            }

            // sort the list
            list.Sort(new Comparison<KeyValuePair<int, string>>(
                    delegate(KeyValuePair<int, string> kvp1, KeyValuePair<int, string> kvp2)
                    {
                        return ((uint)kvp2.Key).CompareTo((uint)kvp1.Key);
                    })); 

            _libraryLookup.Sort(new Comparison<KeyValuePair<int, string>>(
                    delegate(KeyValuePair<int, string> kvp1, KeyValuePair<int, string> kvp2)
                    {
                        return ((uint)kvp2.Key).CompareTo((uint)kvp1.Key);
                    }));
        }

        public void UnloadSymbols(string path)
        {
            foreach (KeyValuePair<int, string> kvp in _libraryLookup)
            {
                if (kvp.Value == path)
                {
                    _libraryLookup.Remove(kvp);
                    break;
                }
            }

            _librarySizes.Remove(path);
            _symbols.Remove(path);
        }

        public string GetModuleFromAddress(int address)
        {
            foreach (KeyValuePair<int, string> kvp in _libraryLookup)
            {
                if ((uint)address >= (uint)kvp.Key)
                {
                    List<KeyValuePair<int, string>> symbolList = _symbols[kvp.Value];
                    FileInfo fi = new FileInfo(kvp.Value);

                    return fi.FullName;
                }
            }

            return "";
        }

        public string GetNameFromAddress(int address)
        {
            FoundLevel level;

            return GetNameFromAddress(address, out level);
        }

        public string GetNameFromAddress(int address, out FoundLevel level)
        {
            if (address == 0)
            {
                level = FoundLevel.Invalid;
                return "(invalid)";
            }

            // go through each loaded library
            foreach (KeyValuePair<int, string> kvp in _libraryLookup)
            {
                uint size = _librarySizes[kvp.Value];

                //if ((uint)address >= (uint)kvp.Key && (uint)address < ((uint)kvp.Key + size))       
                if ((uint)address >= (uint)kvp.Key)
                {
                    List<KeyValuePair<int, string>> symbolList = _symbols[kvp.Value];  
                    FileInfo fi = new FileInfo(kvp.Value);

                    // go through each symbol
                    foreach (KeyValuePair<int, string> kvps in symbolList)
                    {
                        if ((uint)address >= (uint)kvps.Key)
                        {
                            // we found a function name
                            int offset = address - kvps.Key;

                            level = FoundLevel.Function;

                            // don't need to put in the +
                            if (offset == 0)
                                return string.Format("{0}!{1}", fi.Name, kvps.Value);
                            else
                                return string.Format("{0}!{1}+0x{2:x}",
                                    fi.Name, kvps.Value, address - kvps.Key);
                        }
                    }

                    // no function name found, but we have a library name    
                    level = FoundLevel.Module;
                    return string.Format("{0}+0x{1:x}", fi.Name, address - kvp.Key);
                }
            }

            // we didn't find anything
            level = FoundLevel.Address;
            return "0x" + address.ToString("x8");
        }

        public int LibraryCount
        {
            get { return _libraryLookup.Count; }
        }

        public int SymbolCount
        {
            get
            {
                int count = 0;

                foreach (List<KeyValuePair<int, string>> list in _symbols.Values)
                    count += list.Count;

                return count;
            }
        }

        public IEnumerable<string> Keys
        {
            get
            {
                return _symbols.Keys;
            }
        }
    }
}
