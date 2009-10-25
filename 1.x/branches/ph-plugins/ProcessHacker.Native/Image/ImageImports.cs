/*
 * Process Hacker - 
 *   image imports reader
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

using ProcessHacker.Native.Api;

namespace ProcessHacker.Native.Image
{
    public unsafe sealed class ImageImports
    {
        public delegate bool EnumEntriesDelegate(ImageExportEntry entry);

        private MappedImage _mappedImage;
        private int _count;
        private ImageImportDescriptor* _descriptorTable;
        private ImageImportDll[] _dlls;

        internal ImageImports(MappedImage mappedImage)
        {
            _mappedImage = mappedImage;
            _descriptorTable = mappedImage.GetImportDirectory();

            // Do a quick scan.
            if (_descriptorTable != null)
            {
                int i = 0;

                while (_descriptorTable[i].OriginalFirstThunk != 0 || _descriptorTable[i].FirstThunk != 0)
                    i++;

                _count = i;
                _dlls = new ImageImportDll[i];
            }
        }

        public ImageImportDll this[int index]
        {
            get { return this.GetDll(index); }
        }

        public int Count
        {
            get { return _count; }
        }

        public ImageImportDll GetDll(int index)
        {
            if (_descriptorTable == null)
                return null;

            if (index < _count)
            {
                if (_dlls[index] == null)
                    _dlls[index] = new ImageImportDll(_mappedImage, &_descriptorTable[index]);

                return _dlls[index];
            }
            else
            {
                return null;
            }
        }
    }

    public unsafe sealed class ImageImportDll
    {
        private MappedImage _mappedImage;
        private ImageImportDescriptor* _descriptor;
        private string _name;
        private void* _lookupTable;
        private int _count;

        internal ImageImportDll(MappedImage mappedImage, ImageImportDescriptor* descriptor)
        {
            _mappedImage = mappedImage;
            _descriptor = descriptor;

            if (_descriptor->OriginalFirstThunk != 0)
                _lookupTable = _mappedImage.RvaToVa(_descriptor->OriginalFirstThunk);
            else
                _lookupTable = _mappedImage.RvaToVa(_descriptor->FirstThunk);

            // Do a quick scan.
            if (_lookupTable != null)
            {
                int i = 0;

                if (_mappedImage.Magic == Win32.Pe32Magic)
                {
                    while (((int*)_lookupTable)[i] != 0)
                        i++;
                }
                else if (_mappedImage.Magic == Win32.Pe32PlusMagic)
                {
                    while (((long*)_lookupTable)[i] != 0)
                        i++;
                }

                _count = i;
            }
        }

        public ImageImportEntry this[int index]
        {
            get { return this.GetEntry(index); }
        }

        public int Count
        {
            get { return _count; }
        }

        public string Name
        {
            get
            {
                if (_name == null)
                    _name = new string((sbyte*)_mappedImage.RvaToVa(_descriptor->Name));

                return _name;
            }
        }

        public ImageImportEntry GetEntry(int index)
        {
            if (index >= _count)
                return ImageImportEntry.Empty;

            if (_mappedImage.Magic == Win32.Pe32Magic)
            {
                int entry = ((int*)_lookupTable)[index];

                // Is this entry using an ordinal?
                if ((entry & 0x80000000) != 0)
                {
                    return new ImageImportEntry() { Ordinal = (short)(entry & 0xffff) };
                }
                else
                {
                    ImageImportByName* nameEntry = (ImageImportByName*)_mappedImage.RvaToVa(entry);

                    return new ImageImportEntry()
                    {
                        NameHint = nameEntry->Hint,
                        Name = new string((sbyte*)&nameEntry->Name)
                    };
                }
            }
            else if (_mappedImage.Magic == Win32.Pe32PlusMagic)
            {
                long entry = ((long*)_lookupTable)[index];

                // Is this entry using an ordinal?
                if (((ulong)entry & 0x8000000000000000) != 0)
                {
                    return new ImageImportEntry() { Ordinal = (short)(entry & 0xffff) };
                }
                else
                {
                    ImageImportByName* nameEntry = (ImageImportByName*)_mappedImage.RvaToVa((int)(entry & 0xffffffff));

                    return new ImageImportEntry()
                    {
                        NameHint = nameEntry->Hint,
                        Name = new string((sbyte*)&nameEntry->Name)
                    };
                }
            }

            return ImageImportEntry.Empty;
        }
    }

    public struct ImageImportEntry
    {
        public static readonly ImageImportEntry Empty;

        public short Ordinal;
        public short NameHint;
        public string Name;
    }
}
