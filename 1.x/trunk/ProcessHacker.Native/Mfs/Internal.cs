using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace ProcessHacker.Native.Mfs
{
    // MFS consists of 64K blocks. Within each block are 256 256B cells.
    // The first block of the file system is the file system header. 
    // The first cell of each block is the block header.
    //
    // Data can only be added to MFS, not removed.

    [StructLayout(LayoutKind.Sequential)]
    public struct MfsCellId : IEquatable<MfsCellId>
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

        public override int GetHashCode()
        {
            return ((Block << 16) | Cell).GetHashCode();
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MfsFsHeader
    {
        public int Magic; // MFS\0
        public ushort NextFreeBlock;
        public ushort Reserved;
        public MfsCellId RootObject;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MfsBlockHeader
    {
        public int Hash; // CRC32
        public ushort NextFreeCell;
        public ushort Reserved;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct MfsObjectHeader
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
        public fixed char Name[64];
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct MfsDataCell
    {
        public static readonly int DataOffset = Marshal.OffsetOf(typeof(MfsDataCell), "Data").ToInt32();

        public MfsCellId NextCell;
        public int DataLength;
        public byte Data;
    }
}
