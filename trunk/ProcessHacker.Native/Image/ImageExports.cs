/*
 * Process Hacker - 
 *   image exports reader
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
using System.Collections.Generic;
using System.Text;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;

namespace ProcessHacker.Native.Image
{
    public unsafe sealed class ImageExports
    {
        public delegate bool EnumEntriesDelegate(ImageExportEntry entry);

        private MappedImage _mappedImage;
        private ImageDataDirectory* _dataDirectory;
        private ImageExportDirectory* _exportDirectory;
        private int* _addressTable;
        private int* _namePointerTable;
        private short* _ordinalTable;

        internal ImageExports(MappedImage mappedImage)
        {
            _mappedImage = mappedImage;
            _dataDirectory = mappedImage.GetDataEntry(ImageDataEntry.Export);
            _exportDirectory = mappedImage.GetExportDirectory();

            if (_exportDirectory != null)
            {
                _addressTable = (int*)mappedImage.RvaToVa(_exportDirectory->AddressOfFunctions);
                _namePointerTable = (int*)mappedImage.RvaToVa(_exportDirectory->AddressOfNames);
                _ordinalTable = (short*)mappedImage.RvaToVa(_exportDirectory->AddressOfNameOrdinals);
            }
        }

        public int Count
        {
            get
            {
                if (_exportDirectory != null)
                    return _exportDirectory->NumberOfFunctions;
                else
                    return 0;
            }
        }

        public ImageExportEntry GetEntry(int index)
        {
            if (_exportDirectory == null || _namePointerTable == null || _ordinalTable == null)
                return ImageExportEntry.Empty;
            if (index >= _exportDirectory->NumberOfFunctions)
                return ImageExportEntry.Empty;

            ImageExportEntry entry = new ImageExportEntry();

            entry.Ordinal = (short)(_ordinalTable[index] + _exportDirectory->Base);

            if (index < _exportDirectory->NumberOfNames)
                entry.Name = new string((sbyte*)_mappedImage.RvaToVa(_namePointerTable[index]));

            return entry;
        }

        public ImageExportFunction GetFunction(string name)
        {
            if (_exportDirectory == null || _namePointerTable == null || _ordinalTable == null)
                return ImageExportFunction.Empty;

            int index;

            index = this.LookupName(name);

            if (index == -1)
                return ImageExportFunction.Empty;

            return this.GetFunction((short)(_ordinalTable[index] + _exportDirectory->Base));
        }

        public ImageExportFunction GetFunction(short ordinal)
        {
            if (_exportDirectory == null || _addressTable == null)
                return ImageExportFunction.Empty;
            if (ordinal - _exportDirectory->Base >= _exportDirectory->NumberOfFunctions)
                return ImageExportFunction.Empty;

            int rva = _addressTable[ordinal - _exportDirectory->Base];

            if (
                rva >= _dataDirectory->VirtualAddress &&
                rva < _dataDirectory->VirtualAddress + _dataDirectory->Size
                )
            {
                // This is a forwarder RVA.
                return new ImageExportFunction() { ForwardedName = new string((sbyte*)_mappedImage.RvaToVa(rva)) };
            }
            else
            {
                // This is a function RVA.
                return new ImageExportFunction() { Function = (IntPtr)_mappedImage.RvaToVa(rva) };
            }
        }

        private int LookupName(string name)
        {
            int low = 0;
            int high = _exportDirectory->NumberOfNames - 1;

            // Do a binary search.
            while (low <= high)
            {
                int i;
                string n;

                i = (low + high) / 2;
                n = new string((sbyte*)_mappedImage.RvaToVa(_namePointerTable[i]));

                if (name == n)
                {
                    return i;
                }
                else if (name.CompareTo(n) > 0)
                {
                    low = i + 1;
                }
                else
                {
                    high = i - 1;
                }
            }

            return -1;
        }
    }

    public struct ImageExportEntry
    {
        public static readonly ImageExportEntry Empty = new ImageExportEntry();

        public string Name;
        public short Ordinal;
    }

    public struct ImageExportFunction
    {
        public static readonly ImageExportFunction Empty = new ImageExportFunction();

        public IntPtr Function;
        public string ForwardedName;
    }
}
