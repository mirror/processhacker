using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace ProcessHacker.Native.Mfs
{
    // MFS consists of 64K blocks. Within each block are 64 1K cells.
    // The first block of the file system is the file system header. 
    // The first cell of each block is the block header.
    //
    // Data can only be added to MFS, not removed.

    public enum MfsObjectType : int
    {
        File = 1,
        Directory = 2
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MfsCellId
    {
        public MfsCellId(short block, short cell)
        {
            this.Block = block;
            this.Cell = cell;
        }

        public short Block;
        public short Cell;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MfsFsHeader
    {
        public int Magic; // MFS\0
        public short NextFreeBlock;
        public short Reserved;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MfsBlockHeader
    {
        public int Hash; // CRC32
        public short NextFreeCell;
        public short Reserved;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct MfsObjectHeader
    {
        public MfsCellId Flink;
        public MfsCellId Blink;
        public MfsCellId Parent;
        public MfsCellId ChildFlink;
        public MfsCellId ChildBlink;

        public fixed char Name[256];
    }
}
