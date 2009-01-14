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
using System.Runtime.InteropServices;
using System.Text;

namespace ProcessHacker
{
    /// <summary>
    /// Represents an unmanaged memory allocation.
    /// </summary>
    public class MemoryAlloc : IDisposable
    {
        private bool _freed = false;
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

        /// <summary>
        /// Creates a memory allocation from an existing pointer. The allocation 
        /// referenced by the pointer will be freed automatically. The Size 
        /// property will be set to 0.
        /// </summary>
        /// <param name="memory">A pointer to an existing memory allocation.</param>
        /// <returns>A new instance of a memory allocation.</returns>
        public static MemoryAlloc FromPointer(IntPtr memory)
        {
            return new MemoryAlloc(0) { _memory = memory };
        }

        /// <summary>
        /// Creates a new, invalid memory allocation. 
        /// You must set the pointer using the Memory property.
        /// </summary>
        protected MemoryAlloc()
        { }

        /// <summary>
        /// Creates a new memory allocation with the specified size.
        /// </summary>
        /// <param name="size">The amount of memory, in bytes, to allocate.</param>
        public MemoryAlloc(int size)
        {
            _memory = Marshal.AllocHGlobal(size);
            _size = size;
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
            return Marshal.ReadInt32(_memory,
                offset + index * 4);
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
                new IntPtr(_memory.ToInt32() + offset + Marshal.SizeOf(typeof(T)) * index), typeof(T));
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
        }

        protected virtual void Free()
        {
            Marshal.FreeHGlobal(this);
        }

        ~MemoryAlloc()
        {
            this.Dispose();
        }

        /// <summary>
        /// Frees the allocated memory.
        /// </summary>
        public void Dispose()
        {
            lock (this)
            {
                if (!_freed)
                {
                    _freed = true;
                    this.Free();
                }
            }
        }
    }
}
