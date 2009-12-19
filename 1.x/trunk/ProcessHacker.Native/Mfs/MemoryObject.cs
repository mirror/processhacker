/*
 * Process Hacker - 
 *   MFS object
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

using System.Collections.Generic;
using ProcessHacker.Common;
using ProcessHacker.Common.Objects;

namespace ProcessHacker.Native.Mfs
{
    public unsafe sealed class MemoryObject : BaseObject
    {                     
        public delegate bool EnumChildrenDelegate(MemoryObject mo);

        private bool _fsInternal;
        private MemoryFileSystem _fs;
        private MfsCellId _cellId;
        private MfsObjectHeader* _obj;
        private string _name;

        internal MemoryObject(MemoryFileSystem fs, MfsCellId cellId)
            : this(fs, cellId, false)
        { }

        internal MemoryObject(MemoryFileSystem fs, MfsCellId cellId, bool fsInternal)
        {
            _fs = fs;
            _cellId = cellId;
            _obj = _fs.ReferenceObject(cellId);

            _fsInternal = fsInternal;

            if (!_fsInternal)
                _fs.Reference();

            // Prevent users from disposing the root object wrapper.
            if (_fsInternal)
                this.DisableOwnership(false);
        }

        protected override void DisposeObject(bool disposing)
        {
            if (!_fsInternal)
            {
                _fs.DereferenceObject(_cellId);
                _fs.Dereference(disposing);
                _obj = null;
            }
        }

        public int ChildCount
        {
            get { return _obj->ChildCount; }
        }

        public int DataLength
        {
            get { return _obj->DataLength; }
        }

        public string Name
        {
            get
            {
                if (_name == null)
                    _name = _fs.GetObjectName(_obj);

                return _name;
            }
        }

        public void AppendData(byte[] buffer)
        {
            this.AppendData(buffer, 0, buffer.Length);
        }

        public void AppendData(byte[] buffer, int offset, int length)
        {
            MfsCellId cellId;

            Utils.ValidateBuffer(buffer, offset, length);

            while (length > 0)
            {
                int writeLength;

                writeLength = length > _fs.DataCellDataMaxLength ? _fs.DataCellDataMaxLength : length;
                cellId = _fs.CreateDataCell(_obj->LastData, buffer, offset, writeLength);
                offset += writeLength;
                length -= writeLength;
                _obj->DataLength += writeLength;

                if (_obj->Data == MfsCellId.Empty)
                    _obj->Data = cellId;

                _obj->LastData = cellId;
            }
        }

        public MemoryObject CreateChild(string name)
        {
            return new MemoryObject(_fs, _fs.CreateObject(_cellId, name));
        }

        public void EnumChildren(EnumChildrenDelegate callback)
        {
            MfsCellId cellId;
            MfsObjectHeader* obj;

            cellId = _obj->ChildFlink;

            // Traverse the linked list.
            while (true)
            {
                MfsCellId newCellId;

                if (cellId == _cellId)
                    break;

                obj = _fs.ReferenceObject(cellId);

                if (!callback(new MemoryObject(_fs, cellId)))
                {
                    _fs.DereferenceObject(cellId);
                    break;
                }

                newCellId = obj->Flink;
                _fs.DereferenceObject(cellId);
                cellId = newCellId;
            }
        }

        public MemoryObject GetChild(string name)
        {
            MfsCellId cellId;
            MfsObjectHeader* obj;

            cellId = _obj->ChildFlink;

            // Traverse the linked list and find the child with the specified name.
            while (true)
            {
                MfsCellId newCellId;

                if (cellId == _cellId)
                    break;

                obj = _fs.ReferenceObject(cellId);

                if (_fs.GetObjectName(obj) == name)
                {
                    MemoryObject mo;

                    // Create the memory object before dereferencing to avoid re-mapping.
                    mo = new MemoryObject(_fs, cellId);
                    _fs.DereferenceObject(cellId);
                    return mo;
                }

                newCellId = obj->Flink;
                _fs.DereferenceObject(cellId);
                cellId = newCellId;
            }

            return null;
        }

        public string[] GetChildNames()
        {
            List<string> names = new List<string>();

            this.EnumChildren((mo) =>
                {
                    names.Add(mo.Name);
                    mo.Dispose();
                    return true;
                });

            return names.ToArray();
        }

        public MemoryObject GetParent()
        {
            return new MemoryObject(_fs, _obj->Parent);
        }

        public MemoryDataWriteStream GetWriteStream()
        {
            return new MemoryDataWriteStream(this, _fs.DataCellDataMaxLength);
        }

        public byte[] ReadData()
        {
            byte[] buffer = new byte[this.DataLength];

            this.ReadData(buffer, 0, buffer.Length);

            return buffer;
        }

        public int ReadData(System.IO.Stream stream)
        {
            return this.ReadData(stream, _obj->DataLength);
        }

        public int ReadData(byte[] buffer, int offset, int length)
        {
            if (_obj->Data == MfsCellId.Empty)
                return 0;

            return _fs.ReadDataCell(_obj->Data, buffer, offset, length);
        }

        public int ReadData(System.IO.Stream stream, int length)
        {
            if (_obj->Data == MfsCellId.Empty)
                return 0;

            return _fs.ReadDataCell(_obj->Data, stream, length);
        }
    }
}
