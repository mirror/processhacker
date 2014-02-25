/*
 * Process Hacker - 
 *   MFS structures and enumerations
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
using System.Runtime.InteropServices;
using System.Text;

namespace ProcessHacker.Native.Mfs
{
    // MFS consists of 64K blocks. Within each block are 512 128B cells.
    // The first block of the file system is the file system header. 
    // The first cell of each block is the block header.
    //
    // Data can only be added to MFS, not removed.

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct MfsCellId : IEquatable<MfsCellId>
    {
        public static readonly MfsCellId Empty = new MfsCellId(0, 0);

        public static bool operator ==(MfsCellId a, MfsCellId b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(MfsCellId a, MfsCellId b)
        {
            return !a.Equals(b);
        }

        public MfsCellId(ushort block, ushort cell)
        {
            this.Block = block;
            this.Cell = cell;
        }

        public ushort Block;
        public ushort Cell;

        public bool Equals(MfsCellId other)
        {
            return this.Block == other.Block && this.Cell == other.Cell;
        }

        public override bool Equals(object obj)
        {
            if (obj is MfsCellId)
                return this.Equals((MfsCellId)obj);
            else
                return false;
        }

        public override int GetHashCode()
        {
            return ((Block << 16) | Cell).GetHashCode();
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct MfsFsHeader
    {
        public int Magic; // MFS\0
        public ushort NextFreeBlock;
        public ushort Reserved;
        public MfsCellId RootObject;
        public int BlockSize;
        public int CellSize;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct MfsBlockHeader
    {
        public int Hash; // CRC32
        public ushort NextFreeCell;
        public ushort Reserved;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal unsafe struct MfsObjectHeader
    {
        public MfsCellId Flink;
        public MfsCellId Blink;
        public MfsCellId Parent;

        public int ChildCount;
        public MfsCellId ChildFlink;
        public MfsCellId ChildBlink;

        public int DataLength;
        public MfsCellId Data;
        public MfsCellId LastData;

        public int NameLength;
        public fixed char Name[32];
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal unsafe struct MfsDataCell
    {
        public static readonly int DataOffset = Marshal.OffsetOf(typeof(MfsDataCell), "Data").ToInt32();

        public MfsCellId NextCell;
        public int DataLength;
        public byte Data;
    }
}
