/*
 * Process Hacker - 
 *   MFS
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
using ProcessHacker.Common;
using ProcessHacker.Common.Objects;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;

namespace ProcessHacker.Native.Mfs
{
    public unsafe sealed class MemoryFileSystem : BaseObject
    {
        internal const int MfsMagic = 0x0053464d;
        internal const string MfsMagicString = "MFS\0";

        internal const int MfsBlockSizeBase = 0x10000;

        internal const int MfsDefaultBlockSize = 0x10000;
        internal const int MfsDefaultCellSize = 0x80;

        private class ViewDescriptor : IResettable
        {
            public ushort RefCount;
            public ushort BlockId;
            public SectionView View;

            public void ResetObject()
            {
                // No need.
            }
        }

        private bool _readOnly;
        private MemoryProtection _protection;
        private Section _section;

        private MfsFsHeader* _header;
        private int _blockSize;
        private int _blockMask;
        private int _cellSize;
        private int _cellCount;
        private int _dataCellDataMaxLength;

        private MfsCellId _rootObjectCellId = new MfsCellId(0, 1);
        private MfsObjectHeader* _rootObject;
        private MemoryObject _rootObjectMo;

        private FreeList<ViewDescriptor> _vdFreeList = new FreeList<ViewDescriptor>(16);
        private Dictionary<ushort, ViewDescriptor> _views =
            new Dictionary<ushort, ViewDescriptor>();
        private Dictionary<IntPtr, ViewDescriptor> _views2 =
            new Dictionary<IntPtr, ViewDescriptor>();

        private ViewDescriptor _cachedLastBlockView;

        public MemoryFileSystem(string fileName)
            : this(fileName, MfsOpenMode.OpenIf, false)
        { }

        public MemoryFileSystem(string fileName, MfsOpenMode mode)
            : this(fileName, mode, false)
        { }

        public MemoryFileSystem(string fileName, MfsOpenMode mode, bool readOnly)
            : this(fileName, mode, readOnly, null)
        { }

        public MemoryFileSystem(string fileName, MfsOpenMode mode, bool readOnly, MfsParameters createParams)
        {
            FileCreationDispositionWin32 cdWin32;

            if (readOnly && mode != MfsOpenMode.Open)
                throw new ArgumentException("Invalid mode for read only access.");

            switch (mode)
            {
                case MfsOpenMode.Open:
                    cdWin32 = FileCreationDispositionWin32.OpenExisting;
                    break;
                default:
                case MfsOpenMode.OpenIf:
                    cdWin32 = FileCreationDispositionWin32.OpenAlways;
                    break;
                case MfsOpenMode.OverwriteIf:
                    cdWin32 = FileCreationDispositionWin32.CreateAlways;
                    break;
            }

            using (var fhandle = FileHandle.CreateWin32(
                fileName,
                FileAccess.GenericRead | (!readOnly ? FileAccess.GenericWrite : 0),
                FileShareMode.Read,
                cdWin32
                ))
            {
                bool justCreated = false;

                _readOnly = readOnly;
                _protection = !readOnly ? MemoryProtection.ReadWrite : MemoryProtection.ReadOnly;

                if (fhandle.GetSize() == 0)
                {
                    if (readOnly)
                    {
                        throw new MfsInvalidFileSystemException();
                    }
                    else
                    {
                        // File is too small. Make it 1 byte large and we'll deal with it 
                        // soon.
                        fhandle.SetEnd(1);
                    }
                }

                _section = new Section(fhandle, _protection);

                _blockSize = MfsBlockSizeBase; // fake block size to begin with; we'll fix it up later.

                if (fhandle.GetSize() < _blockSize)
                {
                    if (readOnly)
                    {
                        throw new MfsInvalidFileSystemException();
                    }
                    else
                    {
                        // We're creating a new file system. We need the correct block size 
                        // now.
                        if (createParams != null)
                            _blockSize = createParams.BlockSize;
                        else
                            _blockSize = MfsDefaultBlockSize;

                        _section.Extend(_blockSize);

                        using (var view = _section.MapView(0, _blockSize, _protection))
                            this.InitializeFs((MfsFsHeader*)view.Memory, createParams);

                        justCreated = true;
                    }
                }

                _header = (MfsFsHeader*)this.ReferenceBlock(0);

                // Check the magic.
                if (_header->Magic != MfsMagic)
                    throw new MfsInvalidFileSystemException();

                // Set up the local constants.
                _blockSize = _header->BlockSize;
                _cellSize = _header->CellSize;

                // Backwards compatibility.
                if (_blockSize == 0)
                    _blockSize = MfsDefaultBlockSize;
                if (_cellSize == 0)
                    _cellSize = MfsDefaultCellSize;

                // Validate the parameters.
                this.ValidateFsParameters(_blockSize, _cellSize);

                _blockMask = _blockSize - 1;
                _cellCount = _blockSize / _cellSize;
                _dataCellDataMaxLength = _cellSize - MfsDataCell.DataOffset;

                // Remap block 0 with the correct block size.
                this.DereferenceBlock(0);

                // If we just created a new file system, fix the section size.
                if (justCreated)
                    _section.Extend(_blockSize);

                _header = (MfsFsHeader*)this.ReferenceBlock(0);

                // Set up the root object.
                _rootObject = (MfsObjectHeader*)((byte*)_header + _cellSize);
                _rootObjectMo = new MemoryObject(this, _rootObjectCellId, true);

                if (_header->NextFreeBlock != 1 && !readOnly)
                {
                    ushort lastBlockId = (ushort)(_header->NextFreeBlock - 1);

                    this.ReferenceBlock(lastBlockId);
                    _cachedLastBlockView = _views[lastBlockId];
                }
            }
        }

        protected override void DisposeObject(bool disposing)
        {
            foreach (var vd in _views.Values)
                vd.View.Dispose(disposing);

            if (_rootObjectMo != null)
                _rootObjectMo.Dispose();
            if (_section != null)
                _section.Dispose();
            _header = null;
            _rootObject = null;
        }

        public int BlockSize
        {
            get { return _blockSize; }
        }

        public int CellSize
        {
            get { return _cellSize; }
        }

        internal int CellCount
        {
            get { return _cellCount; }
        }

        internal int DataCellDataMaxLength
        {
            get { return _dataCellDataMaxLength; }
        }

        public bool ReadOnly
        {
            get { return _readOnly; }
        }

        public MemoryObject RootObject
        {
            get { return _rootObjectMo; }
        }

        private IntPtr AllocateBlock()
        {
            ushort blockId;

            return this.AllocateBlock(out blockId);
        }

        private IntPtr AllocateBlock(out ushort blockId)
        {
            blockId = _header->NextFreeBlock++;
            _section.Extend(_header->NextFreeBlock * _blockSize);

            SectionView view = _section.MapView(blockId * _blockSize, _blockSize, _protection);
            MfsBlockHeader* header = (MfsBlockHeader*)view.Memory;

            header->Hash = 0;
            header->NextFreeCell = 1;

            ViewDescriptor vd = _vdFreeList.Allocate();

            vd.RefCount = 1;
            vd.BlockId = blockId;
            vd.View = view;

            _views.Add(blockId, vd);
            _views2.Add(view.Memory, vd);

            return view;
        }

        private IntPtr AllocateCell()
        {
            MfsCellId cellId;

            return this.AllocateCell(out cellId);
        }

        private IntPtr AllocateCell(out MfsCellId cellIdOut)
        {  
            ushort blockId;
            ushort cellId;
            IntPtr lastBlock;
            MfsBlockHeader* header;

            if (_header->NextFreeBlock == 1)
            {
                // No blocks except for the FS header. Allocate one.
                lastBlock = this.AllocateBlock(out blockId);
            }
            else
            {
                blockId = (ushort)(_header->NextFreeBlock - 1);
                lastBlock = this.ReferenceBlock(blockId);
            }

            header = (MfsBlockHeader*)lastBlock;

            if (header->NextFreeCell == _cellCount)
            {
                // Block full. Allocate a new one.
                this.DereferenceBlock(lastBlock);

                // Fix up the cached reference.
                if (_cachedLastBlockView != null)
                {
                    this.DereferenceBlock(_cachedLastBlockView.BlockId);
                    _cachedLastBlockView = null;
                }

                lastBlock = this.AllocateBlock(out blockId);
                header = (MfsBlockHeader*)lastBlock;

                // Add a new cached reference.
                this.ReferenceBlock(blockId);
                _cachedLastBlockView = _views[blockId];
            }

            cellId = header->NextFreeCell++;
            cellIdOut = new MfsCellId(blockId, cellId);

            return lastBlock.Increment(cellId * _cellSize);
        }

        internal MfsCellId CreateDataCell(MfsCellId prev, byte[] buffer, int offset, int length)
        {
            MfsCellId cellId;
            MfsDataCell* dc;
            MfsDataCell* prevDc;

            if (_readOnly)
                throw new MfsInvalidOperationException();

            Utils.ValidateBuffer(buffer, offset, length);

            if (length > _dataCellDataMaxLength)
                throw new ArgumentException("length");

            dc = (MfsDataCell*)this.AllocateCell(out cellId);
            dc->NextCell = MfsCellId.Empty;
            dc->DataLength = length;

            if (prev != MfsCellId.Empty)
            {
                prevDc = (MfsDataCell*)this.ReferenceCell(prev);
                prevDc->NextCell = cellId;
                this.DereferenceCell(prev);
            }

            Marshal.Copy(buffer, offset, new IntPtr(&dc->Data), length);

            this.DereferenceCell(cellId);

            return cellId;
        }

        internal MfsCellId CreateObject(MfsCellId parent, string name)
        {
            MfsCellId cellId;
            MfsObjectHeader* obj;

            if (_readOnly)
                throw new MfsInvalidOperationException();

            obj = (MfsObjectHeader*)this.AllocateCell(out cellId);
            this.InitializeObject(obj, cellId);
            this.SetObjectName(obj, name);

            obj->Parent = parent;

            MfsObjectHeader* parentObj = this.ReferenceObject(parent);
            MfsCellId blink;
            MfsObjectHeader* blinkObj;

            // Increment the child count.
            parentObj->ChildCount++;

            // Insert tail.
            blink = parentObj->ChildBlink;
            blinkObj = this.ReferenceObject(blink);
            obj->Flink = parent;
            obj->Blink = blink;
            parentObj->ChildBlink = cellId;

            // Special case - we are not using linked lists properly, 
            // so we have to use this hack.
            if (blink == parent)
                blinkObj->ChildFlink = cellId;
            else
                blinkObj->Flink = cellId;

            this.DereferenceObject(blink);
            this.DereferenceObject(parent);
            this.DereferenceObject(cellId);

            return cellId;
        }

        private void DereferenceBlock(IntPtr view)
        {
            this.DereferenceBlock(_views2[view].BlockId);
        }

        private void DereferenceBlock(ushort blockId)
        {
            ViewDescriptor vd;

            vd = _views[blockId];
            vd.RefCount--;

            if (vd.RefCount == 0)
            {
                _views.Remove(vd.BlockId);
                _views2.Remove(vd.View.Memory);
                vd.View.Dispose();

                _vdFreeList.Free(vd);
            }
        } 

        private void DereferenceCell(MfsCellId cellId)
        {
            this.DereferenceBlock(cellId.Block);
        }

        private void DereferenceCell(IntPtr cellView)
        {
            this.DereferenceBlock(this.GetBlock(cellView));
        }

        internal void DereferenceObject(MfsCellId cellId)
        {
            this.DereferenceCell(cellId);
        }

        private IntPtr GetBlock(IntPtr cellView)
        {
            return cellView.And(_blockMask.ToIntPtr().Not());
        }

        private MfsCellId GetCellId(IntPtr cellView)
        {
            IntPtr view;

            view = this.GetBlock(cellView);

            return new MfsCellId(
                _views2[view].BlockId,
                (ushort)(cellView.Decrement(view).ToInt32() / _cellSize)
                );
        }

        internal string GetObjectName(MfsObjectHeader* obj)
        {
            return new string(obj->Name, 0, obj->NameLength);
        }

        private void InitializeFs(MfsFsHeader* header, MfsParameters createParams)
        {
            header->Magic = MfsMagic;
            header->NextFreeBlock = 1;

            if (createParams != null)
            {
                // Validate the parameters.
                this.ValidateFsParameters(createParams.BlockSize, createParams.CellSize);

                header->BlockSize = createParams.BlockSize;
                header->CellSize = createParams.CellSize;
            }
            else
            {
                header->BlockSize = MfsDefaultBlockSize;
                header->CellSize = MfsDefaultCellSize;
            }

            // Store root object in the next cell.
            MfsObjectHeader* obj = (MfsObjectHeader*)((byte*)header + header->CellSize);

            this.InitializeObject(obj, _rootObjectCellId);
            this.SetObjectName(obj, "root");
        }

        private void InitializeObject(MfsObjectHeader* obj, MfsCellId cellId)
        {
            obj->Flink = cellId;
            obj->Blink = cellId;
            obj->Parent = MfsCellId.Empty;
            obj->ChildFlink = cellId;
            obj->ChildBlink = cellId;
            obj->DataLength = 0;
            obj->Data = MfsCellId.Empty;
            obj->LastData = MfsCellId.Empty;
        }

        internal int ReadDataCell(MfsCellId cellId, byte[] buffer, int offset, int length)
        {
            MfsDataCell* dc;
            int bytesRead = 0;

            Utils.ValidateBuffer(buffer, offset, length);

            while (length > 0)
            {
                MfsCellId newCellId;
                int readLength;

                dc = (MfsDataCell*)this.ReferenceCell(cellId);

                readLength = length > dc->DataLength ? dc->DataLength : length;
                Marshal.Copy(new IntPtr(&dc->Data), buffer, offset, readLength);
                offset += readLength;
                bytesRead += readLength;
                length -= readLength;

                newCellId = dc->NextCell;
                this.DereferenceCell(cellId);
                cellId = newCellId;

                if (cellId == MfsCellId.Empty)
                    break;
            }

            return bytesRead;
        }

        internal int ReadDataCell(MfsCellId cellId, System.IO.Stream stream, int length)
        {
            MfsDataCell* dc;
            byte[] buf = new byte[_dataCellDataMaxLength];
            int bytesRead = 0;

            while (length > 0)
            {
                MfsCellId newCellId;
                int readLength;

                dc = (MfsDataCell*)this.ReferenceCell(cellId);

                readLength = length > dc->DataLength ? dc->DataLength : length;
                Marshal.Copy(new IntPtr(&dc->Data), buf, 0, readLength);
                stream.Write(buf, 0, readLength);

                bytesRead += readLength;
                length -= readLength;

                newCellId = dc->NextCell;
                this.DereferenceCell(cellId);
                cellId = newCellId;

                if (cellId == MfsCellId.Empty)
                    break;
            }

            return bytesRead;
        }

        private IntPtr ReferenceBlock(ushort blockId)
        {
            ViewDescriptor vd;
            SectionView view;

            if (_views.ContainsKey(blockId))
            {
                vd = _views[blockId];
                vd.RefCount++;
            }
            else
            {
                view = _section.MapView(blockId * _blockSize, _blockSize, _protection);
                vd = _vdFreeList.Allocate();
                vd.RefCount = 1;
                vd.BlockId = blockId;
                vd.View = view;

                _views.Add(blockId, vd);
                _views2.Add(vd.View.Memory, vd);
            }

            return vd.View;
        }

        private IntPtr ReferenceCell(MfsCellId cellId)
        {
            IntPtr block;

            block = this.ReferenceBlock(cellId.Block);

            return block.Increment(cellId.Cell * _cellSize);
        }

        internal MfsObjectHeader* ReferenceObject(MfsCellId cellId)
        {
            return (MfsObjectHeader*)this.ReferenceCell(cellId);
        }

        internal void SetObjectName(MfsObjectHeader* obj, string name)
        {
            if (_readOnly)
                throw new MfsInvalidOperationException();

            obj->NameLength = name.Length;
            Utils.StrCpy(obj->Name, name, 32);
        }

        private void ValidateFsParameters(int blockSize, int cellSize)
        {
            if (blockSize <= 0)
                throw new ArgumentException("The block size must be greater than zero.");
            if (cellSize <= 0)
                throw new ArgumentException("The cell size must be greater than zero.");

            if (blockSize.RoundUpTwo() != blockSize)
                throw new ArgumentException("The block size must be a power of two.");
            if (cellSize.RoundUpTwo() != cellSize)
                throw new ArgumentException("The cell size must be a power of two.");

            if ((blockSize & (MfsBlockSizeBase - 1)) != 0) // block size alignment
                throw new ArgumentException("The block size must be a multiple of " + MfsBlockSizeBase.ToString() + ".");
            if ((blockSize & (cellSize - 1)) != 0)
                throw new ArgumentException("The block size must be a multiple of the cell size.");
            if ((blockSize / cellSize) < 2)
                throw new ArgumentException("There must be a least 2 cells in each block.");
            if (cellSize < Marshal.SizeOf(typeof(MfsObjectHeader)))
                throw new ArgumentException("The cell size is too small.");
        }
    }
}
