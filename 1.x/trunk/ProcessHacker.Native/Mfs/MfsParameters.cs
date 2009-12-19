using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessHacker.Native.Mfs
{
    public class MfsParameters
    {
        private int _blockSize;
        private int _cellSize;

        public int BlockSize
        {
            get { return _blockSize; }
            set { _blockSize = value; }
        }

        public int CellSize
        {
            get { return _cellSize; }
            set { _cellSize = value; }
        }
    }
}
