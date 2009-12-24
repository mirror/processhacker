/*
 * Process Hacker - 
 *   memory region
 * 
 * Copyright (C) 2008-2009 wj32
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

#define SIZE_CACHE_USE_RESOURCE_LOCK

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using ProcessHacker.Common.Objects;
using ProcessHacker.Common.Threading;

namespace ProcessHacker.Native
{
    public class MemoryRegion : BaseObject
    {
        private static Dictionary<Type, int> _sizeCache = new Dictionary<Type, int>();
#if SIZE_CACHE_USE_RESOURCE_LOCK
        private static FastResourceLock _sizeCacheLock = new FastResourceLock();
#endif

        private static int GetStructSize(Type structType)
        {
            int size;

#if SIZE_CACHE_USE_RESOURCE_LOCK
            _sizeCacheLock.AcquireShared();

            if (_sizeCache.ContainsKey(structType))
            {
                size = _sizeCache[structType];
                _sizeCacheLock.ReleaseShared();
            }
            else
            {
                _sizeCacheLock.ReleaseShared();

                size = Marshal.SizeOf(structType);
                _sizeCacheLock.AcquireExclusive();

                try
                {
                    if (!_sizeCache.ContainsKey(structType))
                        _sizeCache.Add(structType, size);
                }
                finally
                {
                    _sizeCacheLock.ReleaseExclusive();
                }
            }

            return size;
#else
            lock (_sizeCache)
            {
                if (_sizeCache.ContainsKey(structType))
                    size = _sizeCache[structType];
                else
                    _sizeCache.Add(structType, size = Marshal.SizeOf(structType));

                return size;
            }
#endif
        }

        public static T ReadStruct<T>(IntPtr ptr)
        {
            return (T)Marshal.PtrToStructure(ptr, typeof(T));
        }

        public static implicit operator IntPtr(MemoryRegion memory)
        {
            return memory.Memory;
        }

        public unsafe static implicit operator void*(MemoryRegion memory)
        {
            return memory.Memory.ToPointer();
        }

        private MemoryRegion _parent;
        private IntPtr _memory;
        private int _size;

        /// <summary>
        /// Creates a new, invalid memory allocation. 
        /// You must set the pointer using the Memory property.
        /// </summary>
        protected MemoryRegion()
        { }

        public MemoryRegion(IntPtr memory)
            : this(memory, 0)
        { }

        public MemoryRegion(IntPtr memory, int offset)
            : this(memory, offset, 0)
        { }

        public MemoryRegion(IntPtr memory, int offset, int size)
            : this(memory.Increment(offset), size, false)
        { }

        protected MemoryRegion(IntPtr memory, int size, bool owned)
            : this(null, memory, size, owned)
        { }

        protected MemoryRegion(MemoryRegion parent, IntPtr memory, int size, bool owned)
            : base(owned)
        {       
            if (parent != null)
                parent.Reference();

            _parent = parent;
            _memory = memory;
            _size = size;
        }

        protected sealed override void DisposeObject(bool disposing)
        {
            this.Free();

            if (_parent != null)
                _parent.Dereference(disposing);

            _memory = IntPtr.Zero;
            _size = 0;
        }

        protected virtual void Free()
        { }

        /// <summary>
        /// Gets a pointer to the allocated memory.
        /// </summary>
        public IntPtr Memory
        {
            get { return _memory; }
            protected set { _memory = value; }
        }

        public MemoryRegion Parent
        {
            get { return _parent; }
        }

        /// <summary>
        /// Gets the size of the allocated memory.
        /// </summary>
        public virtual int Size
        {
            get { return _size; }
            protected set { _size = value; }
        }

        public void DestroyStruct<T>()
        {
            this.DestroyStruct<T>(0);
        }

        public void DestroyStruct<T>(int index)
        {
            this.DestroyStruct<T>(0, index);
        }

        public void DestroyStruct<T>(int offset, int index)
        {
            if (index == 0)
            {
                Marshal.DestroyStructure(_memory.Increment(offset), typeof(T));
            }
            else
            {
                Marshal.DestroyStructure(
                    _memory.Increment(offset + GetStructSize(typeof(T)) * index),
                    typeof(T)
                    );
            }
        }

        public void Fill(int offset, int length, byte value)
        {
            ProcessHacker.Native.Api.Win32.RtlFillMemory(
                _memory.Increment(offset),
                length.ToIntPtr(),
                value
                );
        }

        public MemoryRegionStream GetStream()
        {
            return new MemoryRegionStream(this);
        }

        public MemoryRegion MakeChild(int offset, int size)
        {
            return new MemoryRegion(this, _memory.Increment(offset), size, true);
        }

        public string ReadAnsiString(int offset)
        {
            return Marshal.PtrToStringAnsi(_memory.Increment(offset));
        }

        public string ReadAnsiString(int offset, int length)
        {
            return Marshal.PtrToStringAnsi(_memory.Increment(offset), length);
        }

        public byte[] ReadBytes(int length)
        {
            return this.ReadBytes(0, length);
        }

        public byte[] ReadBytes(int offset, int length)
        {
            byte[] buffer = new byte[length];

            this.ReadBytes(offset, buffer, 0, length);

            return buffer;
        }

        public void ReadBytes(byte[] buffer, int startIndex, int length)
        {
            this.ReadBytes(0, buffer, startIndex, length);
        }

        public void ReadBytes(int offset, byte[] buffer, int startIndex, int length)
        {
            Marshal.Copy(_memory.Increment(offset), buffer, startIndex, length);
        }

        /// <summary>
        /// Reads a signed integer.
        /// </summary>
        /// <param name="offset">The offset at which to begin reading.</param>
        /// <returns>The integer.</returns>
        public int ReadInt32(int offset)
        {
            return this.ReadInt32(offset, 0);
        }

        /// <summary>
        /// Reads a signed integer.
        /// </summary>
        /// <param name="offset">The offset at which to begin reading.</param>
        /// <param name="index">The index at which to begin reading, after the offset is added.</param>
        /// <returns>The integer.</returns>
        public int ReadInt32(int offset, int index)
        {
            unsafe
            {
                return ((int*)((byte*)_memory + offset))[index];
            }
        }

        public int[] ReadInt32Array(int offset, int count)
        {
            int[] array = new int[count];

            Marshal.Copy(_memory.Increment(offset), array, 0, count);

            return array;
        }

        public IntPtr ReadIntPtr(int offset)
        {
            return this.ReadIntPtr(offset, 0);
        }

        public IntPtr ReadIntPtr(int offset, int index)
        {
            unsafe
            {
                return ((IntPtr*)((byte*)_memory + offset))[index];
            }
        }

        public void ReadMemory(IntPtr buffer, int destOffset, int srcOffset, int length)
        {
            ProcessHacker.Native.Api.Win32.RtlMoveMemory(
                buffer.Increment(destOffset),
                _memory.Increment(srcOffset),
                length.ToIntPtr()
                );
        }

        /// <summary>
        /// Reads an unsigned integer.
        /// </summary>
        /// <param name="offset">The offset at which to begin reading.</param>
        /// <returns>The integer.</returns>
        public uint ReadUInt32(int offset)
        {
            return this.ReadUInt32(offset, 0);
        }

        /// <summary>
        /// Reads an unsigned integer.
        /// </summary>
        /// <param name="offset">The offset at which to begin reading.</param>
        /// <param name="index">The index at which to begin reading, after the offset is added.</param>
        /// <returns>The integer.</returns>
        public uint ReadUInt32(int offset, int index)
        {
            unsafe
            {
                return ((uint*)((byte*)_memory + offset))[index];
            }
        }

        /// <summary>
        /// Creates a struct from the memory allocation.
        /// </summary>
        /// <typeparam name="T">The type of the struct.</typeparam>
        /// <returns>The new struct.</returns>
        public T ReadStruct<T>()
            where T : struct
        {
            return this.ReadStruct<T>(0);
        }

        /// <summary>
        /// Creates a struct from the memory allocation.
        /// </summary>
        /// <typeparam name="T">The type of the struct.</typeparam>
        /// <param name="index">The index at which to begin reading to the struct. This is multiplied by  
        /// the size of the struct.</param>
        /// <returns>The new struct.</returns>
        public T ReadStruct<T>(int index)
            where T : struct
        {
            return this.ReadStruct<T>(0, index);
        }

        /// <summary>
        /// Creates a struct from the memory allocation.
        /// </summary>
        /// <typeparam name="T">The type of the struct.</typeparam>
        /// <param name="offset">The offset to add before reading.</param>
        /// <param name="index">The index at which to begin reading to the struct. This is multiplied by  
        /// the size of the struct.</param>
        /// <returns>The new struct.</returns>
        public T ReadStruct<T>(int offset, int index)
            where T : struct
        {
            if (index == 0)
            {
                return (T)Marshal.PtrToStructure(_memory.Increment(offset), typeof(T));
            }
            else
            {
                return (T)Marshal.PtrToStructure(
                    _memory.Increment(offset + GetStructSize(typeof(T)) * index),
                    typeof(T)
                    );
            }
        }

        public string ReadUnicodeString(int offset)
        {
            return Marshal.PtrToStringUni(_memory.Increment(offset));
        }

        public string ReadUnicodeString(int offset, int length)
        {
            return Marshal.PtrToStringUni(_memory.Increment(offset), length);
        }

        /// <summary>
        /// Writes a single byte to the memory allocation.
        /// </summary>
        /// <param name="offset">The offset at which to write.</param>
        /// <param name="b">The value of the byte.</param>
        public void WriteByte(int offset, byte b)
        {
            unsafe
            {
                *((byte*)_memory + offset) = b;
            }
        }

        public void WriteBytes(int offset, byte[] b)
        {
            Marshal.Copy(b, 0, _memory.Increment(offset), b.Length);
        }

        public void WriteInt16(int offset, short i)
        {
            unsafe
            {
                *(short*)((byte*)_memory + offset) = i;
            }
        }

        public void WriteInt32(int offset, int i)
        {
            unsafe
            {
                *(int*)((byte*)_memory + offset) = i;
            }
        }

        public void WriteIntPtr(int offset, IntPtr i)
        {
            unsafe
            {
                *(IntPtr*)((byte*)_memory + offset) = i;
            }
        }

        public void WriteMemory(int offset, IntPtr buffer, int length)
        {
            ProcessHacker.Native.Api.Win32.RtlMoveMemory(
                _memory.Increment(offset),
                buffer,
                length.ToIntPtr()
                );
        }

        public void WriteStruct<T>(T s)
            where T : struct
        {
            this.WriteStruct<T>(0, s);
        }

        public void WriteStruct<T>(int index, T s)
            where T : struct
        {
            this.WriteStruct<T>(0, index, s);
        }

        public void WriteStruct<T>(int offset, int index, T s)
            where T : struct
        {
            if (index == 0)
            {
                Marshal.StructureToPtr(s, _memory.Increment(offset), false);
            }
            else
            {
                Marshal.StructureToPtr(
                    s,
                    _memory.Increment(offset + GetStructSize(typeof(T)) * index),
                    false
                    );
            }
        }

        /// <summary>
        /// Writes a Unicode string (without a null terminator) to the allocated memory.
        /// </summary>
        /// <param name="offset">The offset to add.</param>
        /// <param name="s">The string to write.</param>
        public void WriteUnicodeString(int offset, string s)
        {
            unsafe
            {
                fixed (char* ptr = s)
                {
                    this.WriteMemory(offset, (IntPtr)ptr, s.Length * 2);
                }
            }
        }

        public void Zero(int offset, int length)
        {
            ProcessHacker.Native.Api.Win32.RtlZeroMemory(
                _memory.Increment(offset),
                length.ToIntPtr()
                );
        }
    }
}
