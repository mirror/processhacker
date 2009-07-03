/*
 * Process Hacker - 
 *   memory allocation wrapper
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
using System.Runtime.InteropServices;
using System.Text;
using ProcessHacker.Common.Objects;

namespace ProcessHacker.Native
{
    /// <summary>
    /// Represents an unmanaged memory allocation.
    /// </summary>
    public class MemoryAlloc : BaseObject
    {
        private static Dictionary<Type, int> _sizeCache = new Dictionary<Type, int>();

        private IntPtr _memory;
        private int _size;

        public static implicit operator int(MemoryAlloc memory)
        {
            return memory.Memory.ToInt32();
        }

        public static implicit operator IntPtr(MemoryAlloc memory)
        {
            return memory.Memory;
        }

        public unsafe static implicit operator byte*(MemoryAlloc memory)
        {
            return (byte*)memory.Memory;
        }

        public unsafe static explicit operator void*(MemoryAlloc memory)
        {
            return (void*)memory.Memory;
        }

        public unsafe static explicit operator int*(MemoryAlloc memory)
        {
            return (int*)memory.Memory;
        }

        /// <summary>
        /// Creates a new, invalid memory allocation. 
        /// You must set the pointer using the Memory property.
        /// </summary>
        protected MemoryAlloc()
        { }

        public MemoryAlloc(IntPtr memory)
            : this(memory, true)
        { }

        public MemoryAlloc(IntPtr memory, bool owned)
            : this(memory, 0, owned)
        { }

        public MemoryAlloc(IntPtr memory, int size, bool owned)
            : base(owned)
        {
            _memory = memory;
            _size = size;
        }

        /// <summary>
        /// Creates a new memory allocation with the specified size.
        /// </summary>
        /// <param name="size">The amount of memory, in bytes, to allocate.</param>
        public MemoryAlloc(int size)
        {
            _memory = Marshal.AllocHGlobal((int)size);
            _size = size;
        }

        protected sealed override void DisposeObject(bool disposing)
        {
            this.Free();
        }

        protected virtual void Free()
        {
            Marshal.FreeHGlobal(this);
        }

        /// <summary>
        /// Gets a pointer to the allocated memory.
        /// </summary>
        public IntPtr Memory
        {
            get { return _memory; }
            protected set { _memory = value; }
        }

        /// <summary>
        /// Gets the size of the allocated memory.
        /// </summary>
        public virtual int Size
        {
            get { return _size; }
            protected set { _size = value; }
        }

        public MemoryAllocStream GetStream()
        {
            return new MemoryAllocStream(this);
        }

        private int GetStructSizeCached(Type structType)
        {
            if (!_sizeCache.ContainsKey(structType))
                _sizeCache.Add(structType, Marshal.SizeOf(structType));

            return _sizeCache[structType];
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
            return Marshal.ReadInt32(_memory, offset + index * sizeof(int));
        }

        public IntPtr ReadIntPtr(int offset)
        {
            return this.ReadIntPtr(offset, 0);
        }

        public IntPtr ReadIntPtr(int offset, int index)
        {
            return Marshal.ReadIntPtr(_memory, offset + index * IntPtr.Size);
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
            return (uint)this.ReadInt32(offset, index);
        }

        /// <summary>
        /// Creates a struct from the memory allocation.
        /// </summary>
        /// <typeparam name="T">The type of the struct.</typeparam>
        /// <returns>The new struct.</returns>
        public T ReadStruct<T>()
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
        {
            return (T)Marshal.PtrToStructure(
                _memory.Increment(offset + this.GetStructSizeCached(typeof(T)) * index), typeof(T));
        }

        /// <summary>
        /// Resizes the memory allocation.
        /// </summary>
        /// <param name="newSize">The new size of the allocation.</param>
        public virtual void Resize(int newSize)
        {
            _memory = Marshal.ReAllocHGlobal(_memory, new IntPtr(newSize));
            _size = newSize;
        }

        /// <summary>
        /// Writes a single byte to the memory allocation.
        /// </summary>
        /// <param name="offset">The offset at which to write.</param>
        /// <param name="b">The value of the byte.</param>
        public void WriteByte(int offset, byte b)
        {
            Marshal.WriteByte(this, offset, b);
        }

        public void WriteBytes(int offset, byte[] b)
        {
            Marshal.Copy(b, 0, _memory.Increment(offset), b.Length);
        }

        public void WriteInt16(int offset, short i)
        {
            Marshal.WriteInt16(this, offset, i);
        }

        public void WriteInt32(int offset, int i)
        {
            Marshal.WriteInt32(this, offset, i);
        }

        public void WriteIntPtr(int offset, IntPtr i)
        {
            Marshal.WriteIntPtr(this, offset, i);
        }

        public void WriteMemory(int destOffset, MemoryAlloc memory, int srcOffset, int length)
        {
            ProcessHacker.Native.Api.Win32.RtlMoveMemory(
                _memory.Increment(destOffset),
                memory.Memory.Increment(srcOffset),
                length.ToIntPtr()
                );
        }

        public void WriteStruct<T>(T s)
        {
            this.WriteStruct<T>(0, s);
        }

        public void WriteStruct<T>(int index, T s)
        {
            this.WriteStruct<T>(0, index, s);
        }

        public void WriteStruct<T>(int offset, int index, T s)
        {
            Marshal.StructureToPtr(s, 
                _memory.Increment(offset + this.GetStructSizeCached(typeof(T)) * index), false);
        }

        /// <summary>
        /// Writes a Unicode string to the allocated memory.
        /// </summary>
        /// <param name="offset">The offset to add.</param>
        /// <param name="s">The string to write.</param>
        public void WriteUnicodeString(int offset, string s)
        {
            byte[] b = UnicodeEncoding.Unicode.GetBytes(s);

            for (int i = 0; i < b.Length; i++)
                Marshal.WriteByte(this.Memory, offset + i, b[i]);
        }
    }
}
