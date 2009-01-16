/*
 * Process Hacker - 
 *   PE file reader
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
using System.Text;
using System.IO;

namespace ProcessHacker.PE
{
    public class PEFile
    {
        private static byte[] PESignature = { 0x50, 0x45, 0, 0 };

        private COFFHeader _coffHeader;
        private COFFOptionalHeader _coffOptionalHeader;
        private Dictionary<ImageDataType, ImageData> _imageData = new Dictionary<ImageDataType,ImageData>();
        private List<SectionHeader> _sections = new List<SectionHeader>();

        public ExportData ExportData;
        public ImportData ImportData;

        public PEFile(string path)
        {
            using (FileStream s = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                this.Read(s);
            }
        }

        public PEFile(Stream s)
        {
            this.Read(s);  
        }

        private void Read(Stream s)
        {
            BinaryReader br = new BinaryReader(s);

            // get location of PE signature
            s.Seek(0x3c, SeekOrigin.Begin);

            uint peSigLoc = br.ReadUInt32();

            try
            {
                s.Seek(peSigLoc, SeekOrigin.Begin);
            }
            catch
            {
                throw new Exception("Could not seek to location 0x" + peSigLoc.ToString("x") + ".");
            }

            byte[] peSig = br.ReadBytes(4);

            if (!Misc.BytesEqual(peSig, PEFile.PESignature))
                throw new Exception("Invalid PE signature.");

            // read COFF header
            _coffHeader = new COFFHeader(br);

            // read COFF optional header
            _coffOptionalHeader = new COFFOptionalHeader(br);

            // read image data directory
            for (int i = 0; i < _coffOptionalHeader.NumberOfRvaAndSizes; i++)
            {
                _imageData.Add((ImageDataType)i, new ImageData()
                {
                    VirtualAddress = br.ReadUInt32(),
                    Size = br.ReadUInt32()
                });
            }

            // read section headers
            for (int i = 0; i < _coffHeader.NumberOfSections; i++)
            {
                _sections.Add(new SectionHeader(br));
            }

            // read export table
            if (_imageData.ContainsKey(ImageDataType.ExportTable))
            {
                ImageData iD = _imageData[ImageDataType.ExportTable];

                if (iD.VirtualAddress != 0)
                {
                    s.Seek(PEFile.RvaToVa(this, iD.VirtualAddress), SeekOrigin.Begin);

                    this.ExportData = new ExportData(br, this);
                }
            }

            // read import table
            if (_imageData.ContainsKey(ImageDataType.ImportTable))
            {
                ImageData iD = _imageData[ImageDataType.ImportTable];

                if (iD.VirtualAddress != 0)
                {
                    s.Seek(PEFile.RvaToVa(this, iD.VirtualAddress), SeekOrigin.Begin);

                    this.ImportData = new ImportData(br, this);
                }
            }
        }

        public static long RvaToVa(PEFile peFile, long rva)
        {
            SectionHeader section = null;

            foreach (SectionHeader sh in peFile.Sections)
            {
                if (rva >= sh.VirtualAddress && rva < sh.VirtualAddress + sh.VirtualSize)
                {
                    section = sh;
                    break;
                }
            }

            if (section == null)
                throw new Exception("Relative virtual address has no matching section.");

            return section.PointerToRawData + rva - section.VirtualAddress;
        }

        public COFFHeader COFFHeader
        {
            get { return _coffHeader; }
        }

        public COFFOptionalHeader COFFOptionalHeader
        {
            get { return _coffOptionalHeader; }
        }

        public Dictionary<ImageDataType, ImageData> ImageData
        {
            get { return _imageData; }
        }

        public List<SectionHeader> Sections
        {
            get { return _sections; }
        }
    }
}
