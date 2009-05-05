/*
 * Process Hacker - 
 *   PE/COFF optional header reader
 * 
 * Copyright (C) 2008 wj32
 * Descriptions from the PE/COFF specification from Microsoft.
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
using System.IO;

namespace ProcessHacker.PE
{
    /// <summary>
    /// Represents a COFF optional header.
    /// </summary>
    public class COFFOptionalHeader
    {
        public const ushort PE32Magic = 0x10b;
        public const ushort PE32PlusMagic = 0x20b;

        public COFFOptionalHeader(BinaryReader br)
        {
            // standard fields
            this.Magic = br.ReadUInt16();
            this.MajorLinkerVersion = br.ReadByte();
            this.MinorLinkerVersion = br.ReadByte();
            this.SizeOfCode = br.ReadUInt32();
            this.SizeOfInitializedData = br.ReadUInt32();
            this.SizeOfUninitializedData = br.ReadUInt32();
            this.AddressOfEntryPoint = new IntPtr(br.ReadUInt32());
            this.BaseOfCode = br.ReadUInt32();

            if (this.Magic == COFFOptionalHeader.PE32Magic)
                this.BaseOfData = br.ReadUInt32();
            else
                this.BaseOfData = 0;

            // windows-specific fields
            if (this.Magic == COFFOptionalHeader.PE32Magic)
                this.ImageBase = br.ReadUInt32();
            else if (this.Magic == COFFOptionalHeader.PE32PlusMagic)
                this.ImageBase = br.ReadUInt64();
            else
                throw new Exception("Unknown magic number.");

            this.SectionAlignment = br.ReadUInt32();
            this.FileAlignment = br.ReadUInt32();
            this.MajorOperatingSystemVersion = br.ReadUInt16();
            this.MinorOperatingSystemVersion = br.ReadUInt16();
            this.MajorImageVersion = br.ReadUInt16();
            this.MinorImageVersion = br.ReadUInt16();
            this.MajorSubsystemVersion = br.ReadUInt16();
            this.MinorSubsystemVersion = br.ReadUInt16();
            this.Win32VersionValue = br.ReadUInt32();
            this.SizeOfImage = br.ReadUInt32();
            this.SizeOfHeaders = br.ReadUInt32();
            this.CheckSum = br.ReadUInt32();
            this.Subsystem = (ImageSubsystem)br.ReadUInt16();
            this.DllCharacteristics = (DllCharacteristics)br.ReadUInt16();

            if (this.Magic == COFFOptionalHeader.PE32Magic)
            {
                this.SizeOfStackReserve = br.ReadUInt32();
                this.SizeOfStackCommit = br.ReadUInt32();
                this.SizeOfHeapReserve = br.ReadUInt32();
                this.SizeOfHeapCommit = br.ReadUInt32();
            }
            else if (this.Magic == COFFOptionalHeader.PE32PlusMagic)
            {
                this.SizeOfStackReserve = br.ReadUInt64();
                this.SizeOfStackCommit = br.ReadUInt64();
                this.SizeOfHeapReserve = br.ReadUInt64();
                this.SizeOfHeapCommit = br.ReadUInt64();
            }
            else
            {
                throw new Exception("Unknown magic number.");
            }

            this.LoaderFlags = br.ReadUInt32();
            this.NumberOfRvaAndSizes = br.ReadUInt32();
        }

        #region Standard Fields

        /// <summary>
        /// The unsigned integer that identifies the state of the image file. 
        /// The most common number is 0x10B, which identifies it as a normal 
        /// executable file. 0x107 identifies it as a ROM image, and 0x20B 
        /// identifies it as a PE32+ executable.
        /// </summary>
        public ushort Magic;

        /// <summary>
        /// The linker major version number.
        /// </summary>
        public byte MajorLinkerVersion;

        /// <summary>
        /// The linker minor version number.
        /// </summary>
        public byte MinorLinkerVersion;

        /// <summary>
        /// The size of the code (text) section, or the sum of all code 
        /// sections if there are multiple sections.
        /// </summary>
        public uint SizeOfCode;

        /// <summary>
        /// The size of the initialized data section, or the sum of all 
        /// such sections if there are multiple data sections.
        /// </summary>
        public uint SizeOfInitializedData;

        /// <summary>
        /// The size of the uninitialized data section (BSS), or the sum 
        /// of all such sections if there are multiple BSS sections.
        /// </summary>
        public uint SizeOfUninitializedData;

        /// <summary>
        /// The address of the entry point relative to the image base when 
        /// the executable file is loaded into memory. For program images, 
        /// this is the starting address. For device drivers, this is the 
        /// address of the initialization function. An entry point is 
        /// optional for DLLs. When no entry point is present, this field 
        /// must be zero.
        /// </summary>
        public IntPtr AddressOfEntryPoint;

        /// <summary>
        /// The address that is relative to the image base of the 
        /// beginning-of-code section when it is loaded into memory.
        /// </summary>
        public uint BaseOfCode;

        /// <summary>
        /// The address that is relative to the image base of the 
        /// beginning-of-data section when it is loaded into memory. This
        /// field is only present in PE32.
        /// </summary>
        public uint BaseOfData;

        #endregion

        #region Windows-specific Fields

        /// <summary>
        /// The preferred address of the first byte of image when loaded into 
        /// memory; must be a multiple of 64 K. The default for DLLs is 
        /// 0x10000000. The default for Windows CE EXEs is 0x00010000. The 
        /// default for Windows NT, Windows 2000, Windows XP, Windows 95, 
        /// Windows 98, and Windows Me is 0x00400000. This field is only 4 bytes 
        /// long for PE32.
        /// </summary>
        public ulong ImageBase;

        /// <summary>
        /// The alignment (in bytes) of sections when they are loaded into memory. 
        /// It must be greater than or equal to FileAlignment. The default is 
        /// the page size for the architecture.
        /// </summary>
        public uint SectionAlignment;

        /// <summary>
        /// The alignment factor (in bytes) that is used to align the raw data of 
        /// sections in the image file. The value should be a power of 2 between 
        /// 512 and 64 K, inclusive. The default is 512. If the SectionAlignment 
        /// is less than the architecture’s page size, then FileAlignment must 
        /// match SectionAlignment.
        /// </summary>
        public uint FileAlignment;

        /// <summary>
        /// The major version number of the required operating system.
        /// </summary>
        public ushort MajorOperatingSystemVersion;

        /// <summary>
        /// The minor version number of the required operating system.
        /// </summary>
        public ushort MinorOperatingSystemVersion;

        /// <summary>
        /// The major version number of the image.
        /// </summary>
        public ushort MajorImageVersion;

        /// <summary>
        /// The minor version number of the image.
        /// </summary>
        public ushort MinorImageVersion;

        /// <summary>
        /// The major version number of the subsystem.
        /// </summary>
        public ushort MajorSubsystemVersion;

        /// <summary>
        /// The minor version number of the subsystem.
        /// </summary>
        public ushort MinorSubsystemVersion;

        /// <summary>
        /// Reserved, must be zero.
        /// </summary>
        public uint Win32VersionValue;

        /// <summary>
        /// The size (in bytes) of the image, including all headers, 
        /// as the image is loaded in memory. It must be a multiple 
        /// of SectionAlignment.
        /// </summary>
        public uint SizeOfImage;

        /// <summary>
        /// The combined size of an MS DOS stub, PE header, and section 
        /// headers rounded up to a multiple of FileAlignment.
        /// </summary>
        public uint SizeOfHeaders;

        /// <summary>
        /// The image file checksum. The algorithm for computing the 
        /// checksum is incorporated into IMAGHELP.DLL. The following are 
        /// checked for validation at load time: all drivers, any DLL 
        /// loaded at boot time, and any DLL that is loaded into a 
        /// critical Windows process.
        /// </summary>
        public uint CheckSum;

        /// <summary>
        /// The subsystem that is required to run this image.
        /// </summary>
        public ImageSubsystem Subsystem;

        /// <summary>
        /// DLL characteristics.
        /// </summary>
        public DllCharacteristics DllCharacteristics;

        /// <summary>
        /// The size of the stack to reserve. Only SizeOfStackCommit is 
        /// committed; the rest is made available one page at a time until 
        /// the reserve size is reached.
        /// </summary>
        public ulong SizeOfStackReserve;

        /// <summary>
        /// The size of the stack to commit.
        /// </summary>
        public ulong SizeOfStackCommit;

        /// <summary>
        /// The size of the local heap space to reserve. Only SizeOfHeapCommit 
        /// is committed; the rest is made available one page at a time 
        /// until the reserve size is reached.
        /// </summary>
        public ulong SizeOfHeapReserve;

        /// <summary>
        /// The size of the local heap space to commit.
        /// </summary>
        public ulong SizeOfHeapCommit;

        /// <summary>
        /// Reserved, must be zero.
        /// </summary>
        public uint LoaderFlags;

        /// <summary>
        /// The number of data-directory entries in the remainder of the 
        /// optional header. Each describes a location and size.
        /// </summary>
        public uint NumberOfRvaAndSizes;
                                 
        #endregion
    }
}
