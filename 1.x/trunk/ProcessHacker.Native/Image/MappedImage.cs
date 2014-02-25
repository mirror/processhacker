/*
 * Process Hacker - 
 *   image mapper and reader
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
using ProcessHacker.Common.Objects;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;

namespace ProcessHacker.Native.Image
{
    public unsafe sealed class MappedImage : BaseObject
    {
        private SectionView _view;
        private int _size;
        private void* _memory;
        private ImageNtHeaders* _ntHeaders;
        private ImageSectionHeader* _sections;
        private short _magic;

        private ImageExports _exports;
        private ImageImports _imports;

        public MappedImage(string fileName)
            : this(fileName, true)
        { }

        public MappedImage(string fileName, bool readOnly)
        {
            using (var fhandle = FileHandle.CreateWin32(
                fileName,
                readOnly ? (FileAccess.Execute | FileAccess.ReadAttributes | FileAccess.ReadData) :
                (FileAccess.AppendData | FileAccess.Execute | FileAccess.ReadAttributes | FileAccess.ReadData | FileAccess.WriteAttributes | FileAccess.WriteData),
                FileShareMode.Read,
                FileCreationDispositionWin32.OpenExisting
                ))
                this.MapAndLoad(fhandle, readOnly);
        }

        public MappedImage(FileHandle fileHandle, bool readOnly)
        {
            this.MapAndLoad(fileHandle, readOnly);
        }

        public MappedImage(IntPtr memory, int size)
        {
            _size = size;
            this.Load((void*)memory);
        }

        protected override void DisposeObject(bool disposing)
        {
            if (_view != null)
                _view.Dispose(disposing);
        }

        public ImageExports Exports
        {
            get
            {
                if (_exports == null)
                    _exports = new ImageExports(this);

                return _exports;
            }
        }

        public ImageImports Imports
        {
            get
            {
                if (_imports == null)
                    _imports = new ImageImports(this);

                return _imports;
            }
        }

        public short Magic
        {
            get { return _magic; }
        }

        public void* Memory
        {
            get { return _memory; }
        }

        public int NumberOfDataEntries
        {
            get
            {
                if (_magic == Win32.Pe32Magic)
                    return this.GetOptionalHeader()->NumberOfRvaAndSizes;
                else if (_magic == Win32.Pe32PlusMagic)
                    return this.GetOptionalHeader64()->NumberOfRvaAndSizes;
                else
                    return 0;
            }
        }

        public int NumberOfSections
        {
            get { return _ntHeaders->FileHeader.NumberOfSections; }
        }

        public ImageNtHeaders* NtHeaders
        {
            get { return _ntHeaders; }
        }

        public ImageSectionHeader* Sections
        {
            get { return _sections; }
        }

        public int Size
        {
            get { return _size; }
        }

        public int GetChecksum()
        {
            int oldChecksum;

            return this.GetChecksum(out oldChecksum);
        }

        public int GetChecksum(out int oldChecksum)
        {
            int checksum;

            if (Win32.CheckSumMappedFile(_view, _size, out oldChecksum, out checksum) == IntPtr.Zero)
                Win32.Throw();

            return checksum;
        }

        public ImageDataDirectory* GetDataEntry(ImageDataEntry entry)
        {
            if (_magic == Win32.Pe32Magic)
            {
                if ((int)entry >= _ntHeaders->OptionalHeader.NumberOfRvaAndSizes)
                    return null;

                return &(&_ntHeaders->OptionalHeader.DataDirectory)[(int)entry];
            }
            else if (_magic == Win32.Pe32PlusMagic)
            {
                if ((int)entry >= this.GetOptionalHeader64()->NumberOfRvaAndSizes)
                    return null;

                return &(&this.GetOptionalHeader64()->DataDirectory)[(int)entry];
            }
            else
            {
                return null;
            }
        }

        public ImageExportDirectory* GetExportDirectory()
        {
            ImageDataDirectory* dataEntry;

            dataEntry = this.GetDataEntry(ImageDataEntry.Export);

            return (ImageExportDirectory*)this.RvaToVa(dataEntry->VirtualAddress);
        }

        public ImageImportDescriptor* GetImportDirectory()
        {
            ImageDataDirectory* dataEntry;

            dataEntry = this.GetDataEntry(ImageDataEntry.Import);

            return (ImageImportDescriptor*)this.RvaToVa(dataEntry->VirtualAddress);
        }

        private void* GetLoadConfig(short magic)
        {
            ImageDataDirectory* dataEntry;

            if (_magic != magic)
                return null;

            dataEntry = this.GetDataEntry(ImageDataEntry.LoadConfig);

            if (dataEntry == null)
                return null;

            return this.RvaToVa(dataEntry->VirtualAddress);
        }

        public ImageLoadConfigDirectory* GetLoadConfig()
        {
            return (ImageLoadConfigDirectory*)this.GetLoadConfig(Win32.Pe32Magic);
        }

        public ImageLoadConfigDirectory64* GetLoadConfig64()
        {
            return (ImageLoadConfigDirectory64*)this.GetLoadConfig(Win32.Pe32PlusMagic);
        }

        private ImageNtHeaders* GetNtHeaders()
        {
            int offset;
            ImageNtHeaders* ntHeaders;

            offset = *((int*)((byte*)_memory + 0x3c));

            if (offset == 0)
                throw new Exception("Invalid NT headers offset.");
            if (offset >= 0x10000000 || offset >= _size)
                throw new Exception("The NT headers offset is too large.");

            ntHeaders = (ImageNtHeaders*)((byte*)_memory + offset);

            return ntHeaders;
        }

        private void* GetOptionalHeader(short magic)
        {
            if (_magic != magic)
                return null;

            return &_ntHeaders->OptionalHeader;
        }

        public ImageOptionalHeader* GetOptionalHeader()
        {
            return (ImageOptionalHeader*)this.GetOptionalHeader(Win32.Pe32Magic);
        }

        public ImageOptionalHeader64* GetOptionalHeader64()
        {
            return (ImageOptionalHeader64*)this.GetOptionalHeader(Win32.Pe32PlusMagic);
        }

        public string GetSectionName(ImageSectionHeader* section)
        {
            return new string((sbyte*)section->Name, 0, 8).TrimEnd('\0');
        }

        private void Load(void* memory)
        {
            _memory = memory;

            byte* start = (byte*)_memory;

            if (start[0] != 'M' || start[1] != 'Z')
                throw new Exception("The file is not a valid executable image.");

            _ntHeaders = this.GetNtHeaders();
            _sections = (ImageSectionHeader*)((byte*)&_ntHeaders->OptionalHeader + _ntHeaders->FileHeader.SizeOfOptionalHeader);
            _magic = _ntHeaders->OptionalHeader.Magic;

            if (_magic != Win32.Pe32Magic && _magic != Win32.Pe32PlusMagic)
                throw new Exception("The file is not a PE32 or PE32+ image.");
        }

        private void MapAndLoad(FileHandle fileHandle, bool readOnly)
        {
            using (Section section = new Section(
                fileHandle,
                false,
                readOnly ? MemoryProtection.ExecuteRead : MemoryProtection.ExecuteReadWrite
                ))
            {
                _size = (int)fileHandle.GetSize();
                _view = section.MapView(_size);

                this.Load(_view);
            }
        }

        public ImageSectionHeader* RvaToSection(int rva)
        {
            if (_ntHeaders->FileHeader.NumberOfSections == 0)
                return null;

            for (int i = 0; i < _ntHeaders->FileHeader.NumberOfSections; i++)
            {
                if (
                    rva >= _sections[i].VirtualAddress &&
                    rva < (_sections[i].VirtualAddress + _sections[i].SizeOfRawData)
                    )
                {
                    return &_sections[i];
                }
            }

            return null;
        }

        public void* RvaToVa(int rva)
        {
            ImageSectionHeader* section;

            section = this.RvaToSection(rva);

            if (section == null)
                return null;

            return (byte*)_memory + section->PointerToRawData - section->VirtualAddress + rva;
        }
    }
}
