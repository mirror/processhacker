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
using System.Text;
using System.IO;

namespace ProcessHacker.PE
{
    public class PEFile
    {
        private static byte[] PESignature = { 0x50, 0x45, 0, 0 };

        private COFFHeader _coffHeader;
        private COFFOptionalHeader _coffOptionalHeader;
        private List<SectionHeader> _sections = new List<SectionHeader>();

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

            byte peSigLoc = br.ReadByte();

            s.Seek(peSigLoc, SeekOrigin.Begin);

            byte[] peSig = br.ReadBytes(4);

            if (!Utils.BytesEqual(peSig, PEFile.PESignature))
                throw new Exception("Invalid PE signature.");

            // read COFF header
            _coffHeader = new COFFHeader(br);

            // read COFF optional header
            _coffOptionalHeader = new COFFOptionalHeader(br);

            for (int i = 0; i < _coffHeader.NumberOfSections; i++)
            {
                _sections.Add(new SectionHeader(br));
            }
        }

        public COFFHeader COFFHeader
        {
            get { return _coffHeader; }
        }

        public COFFOptionalHeader COFFOptionalHeader
        {
            get { return _coffOptionalHeader; }
        }

        public List<SectionHeader> Sections
        {
            get { return _sections; }
        }
    }
}
