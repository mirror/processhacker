using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
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

        internal const int MfsBlockSize = 0x10000;
        internal const int MfsBlockMask = MfsBlockSize - 1;
        internal const int MfsCellSize = 0x80;
        internal const int MfsCellCount = MfsBlockSize / MfsCellSize;

        internal static readonly int MfsDataCellDataMaxLength = MfsCellSize - MfsDataCell.DataOffset;

        private class ViewDescriptor
        {
            public ushort RefCount;
            public ushort BlockId;
            public SectionView View;
        }

        private bool _readOnly;
        private MemoryProtection _protection;
        private Section _section;
        private MfsFsHeader* _header;
        private MfsCellId _rootObjectCellId = new MfsCellId(0, 1);
        private MfsObjectHeader* _rootObject;
        private MemoryObject _rootObjectMo;

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

                if (fhandle.GetSize() < MfsBlockSize)
                {
                    if (readOnly)
                    {
                        throw new MfsInvalidFileSystemException();
                    }
                    else
                    {
                        _section.Extend(MfsBlockSize);

                        using (var view = _section.MapView(0, MfsBlockSize, _protection))
                            this.InitializeFs((MfsFsHeader*)view.Memory);
                    }
                }

                _header = (MfsFsHeader*)this.ReferenceBlock(0);

                // Check the magic.
                if (_header->Magic != MfsMagic)
                    throw new MfsInvalidFileSystemException();

                _rootObject = (MfsObjectHeader*)((byte*)_header + MfsCellSize);
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
            _section.Extend(_header->NextFreeBlock * MfsBlockSize);

            SectionView view = _section.MapView(blockId * MfsBlockSize, MfsBlockSize, _protection);
            MfsBlockHeader* header = (MfsBlockHeader*)view.Memory;

            header->Hash = 0;
            header->NextFreeCell = 1;

            ViewDescriptor vd = new ViewDescriptor();

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

            if (header->NextFreeCell == MfsCellCount)
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

            return lastBlock.Increment(cellId * MfsCellSize);
        }

        internal MfsCellId CreateDataCell(MfsCellId prev, byte[] buffer, int offset, int length)
        {
            MfsCellId cellId;
            MfsDataCell* dc;
            MfsDataCell* prevDc;

            if (_readOnly)
                throw new MfsInvalidOperationException();

            Utils.ValidateBuffer(buffer, offset, length);

            if (length > MfsDataCellDataMaxLength)
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
            return cellView.And(MfsBlockMask.ToIntPtr().Not());
        }

        private MfsCellId GetCellId(IntPtr cellView)
        {
            IntPtr view;

            view = this.GetBlock(cellView);

            return new MfsCellId(
                _views2[view].BlockId,
                (ushort)(cellView.Decrement(view).ToInt32() / MfsCellSize)
                );
        }

        internal string GetObjectName(MfsObjectHeader* obj)
        {
            return new string(obj->Name, 0, obj->NameLength);
        }

        private void InitializeFs(MfsFsHeader* header)
        {
            header->Magic = MfsMagic;
            header->NextFreeBlock = 1;

            // Store root object in the next cell.
            MfsObjectHeader* obj = (MfsObjectHeader*)((byte*)header + MfsCellSize);

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

            while (true)
            {
                MfsCellId newCellId;
                int readLength;

                dc = (MfsDataCell*)this.ReferenceCell(cellId);

                if (length == 0)
                    break;

                readLength = length > dc->DataLength ? dc->DataLength : length;
                Marshal.Copy(new IntPtr(&dc->Data), buffer, offset, readLength);
                offset += readLength;
                bytesRead += readLength;
                length -= readLength;

                if (dc->NextCell == MfsCellId.Empty)
                    break;

                newCellId = dc->NextCell;
                this.DereferenceCell(cellId);
                cellId = newCellId;
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
                view = _section.MapView(blockId * MfsBlockSize, MfsBlockSize, _protection);
                vd = new ViewDescriptor();
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

            return block.Increment(cellId.Cell * MfsCellSize);
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
    }
}
