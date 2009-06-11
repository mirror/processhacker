/*
 * Process Hacker - 
 *   privilege set
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
using ProcessHacker.Native.Api;

namespace ProcessHacker.Native.Security
{
    public class PrivilegeSet : IList<Privilege>
    {
        private static int _sizeOfLaa = Marshal.SizeOf(typeof(LuidAndAttributes));

        private List<Privilege> _privileges;
        private PrivilegeSetFlags _flags;

        public PrivilegeSet()
        {
            _privileges = new List<Privilege>();
            _flags = PrivilegeSetFlags.AllNecessary;
        }

        public PrivilegeSet(IntPtr memory)
        {
            MemoryAlloc memoryAlloc = new MemoryAlloc(memory, false);
            int count = memoryAlloc.ReadInt32(0);
            PrivilegeSetFlags flags = (PrivilegeSetFlags)memoryAlloc.ReadInt32(4);

            _privileges = new List<Privilege>(count);

            for (int i = 0; i < count; i++)
                _privileges.Add(new Privilege(memoryAlloc.ReadStruct<LuidAndAttributes>(8, i)));
        }

        public PrivilegeSetFlags Flags
        {
            get { return _flags; }
        }

        public MemoryAlloc ToMemory()
        {
            int requiredSize = 8 + _sizeOfLaa * _privileges.Count;
            MemoryAlloc memory = new MemoryAlloc(requiredSize);

            memory.WriteInt32(0, _privileges.Count);
            memory.WriteInt32(4, (int)_flags);

            for (int i = 0; i < _privileges.Count; i++)
                memory.WriteStruct<LuidAndAttributes>(8, i, _privileges[i].ToLuidAndAttributes());

            return memory;
        }

        public PrivilegeSetStruct ToPrivilegeSet()
        {
            return new PrivilegeSetStruct()
            {
                Count = _privileges.Count,
                Flags = _flags,
                Privileges = _privileges.ConvertAll<LuidAndAttributes>(
                (privilege) => privilege.ToLuidAndAttributes()).ToArray()
            };
        }

        public TokenPrivileges ToTokenPrivileges()
        {
            return new TokenPrivileges()
            {
                PrivilegeCount = _privileges.Count,
                Privileges = _privileges.ConvertAll<LuidAndAttributes>(
                (privilege) => privilege.ToLuidAndAttributes()).ToArray()
            };
        }

        #region IList<Privilege> Members

        public int IndexOf(Privilege item)
        {
            return _privileges.IndexOf(item);
        }

        public void Insert(int index, Privilege item)
        {
            _privileges.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _privileges.RemoveAt(index);
        }

        public Privilege this[int index]
        {
            get
            {
                return _privileges[index];
            }
            set
            {
                _privileges[index] = value;
            }
        }

        #endregion

        #region ICollection<Privilege> Members

        public void Add(Privilege item)
        {
            _privileges.Add(item);
        }

        public void Clear()
        {
            _privileges.Clear();
        }

        public bool Contains(Privilege item)
        {
            return _privileges.Contains(item);
        }

        public void CopyTo(Privilege[] array, int arrayIndex)
        {
            _privileges.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _privileges.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(Privilege item)
        {
            return _privileges.Remove(item);
        }

        #endregion

        #region IEnumerable<Privilege> Members

        public IEnumerator<Privilege> GetEnumerator()
        {
            return _privileges.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _privileges.GetEnumerator();
        }

        #endregion
    }
}
