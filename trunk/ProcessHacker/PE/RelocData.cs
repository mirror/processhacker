/*
 * Process Hacker - 
 *   PE relocations reader
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
    public class ImageRelocationBlock
    {
        public int PageRVA;
        public int BlockSize;
        public List<ImageRelocationEntry> Entries;
    }

    public class ImageRelocationEntry
    {
        public ImageRelocationType Type;
        public int Offset;
    }

    public class RelocData
    {
        public RelocData(BinaryReader br, PEFile peFile)
        {
            long i = br.BaseStream.Position;

            try
            {
                while (i < peFile.ImageData[ImageDataType.BaseRelocationTable].VirtualAddress +
                    peFile.ImageData[ImageDataType.BaseRelocationTable].Size)
                {
                    var block = new ImageRelocationBlock();

                    block.PageRVA = br.ReadInt32();

                    if (block.PageRVA == 0)
                        break;

                    block.BlockSize = br.ReadInt32();

                    if (block.BlockSize == 0)
                        break;

                    block.Entries = new List<ImageRelocationEntry>();

                    for (int j = 8; j < block.BlockSize; j += 2)
                    {
                        var entry = new ImageRelocationEntry();
                        ushort val = br.ReadUInt16();

                        entry.Type = (ImageRelocationType)(val >> 12);
                        entry.Offset = val & 0xfff;
                        block.Entries.Add(entry);
                    }

                    RelocBlocks.Add(block);
                    br.BaseStream.Seek(i += block.BlockSize, SeekOrigin.Begin);
                }
            }
            catch
            { }
        }

        public List<ImageRelocationBlock> RelocBlocks = new List<ImageRelocationBlock>();
    }
}
