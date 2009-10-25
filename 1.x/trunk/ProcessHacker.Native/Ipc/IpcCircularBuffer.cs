/*
 * Process Hacker - 
 *   inter-process circular buffer
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
using System.Runtime.InteropServices;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Security;
using ProcessHacker.Native.Threading;

namespace ProcessHacker.Native.Ipc
{
    public unsafe class IpcCircularBuffer
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct BufferHeader
        {
            public int BlockSize;
            public int NumberOfBlocks;

            public long ReadSemaphoreId;
            public long WriteSemaphoreId;

            public int ReadPosition;
            public int WritePosition;

            public long Data;
        }

        public static IpcCircularBuffer Create(string name, int blockSize, int numberOfBlocks)
        {
            Random r = new Random();
            long readSemaphoreId = ((long)r.Next() << 32) + r.Next();
            long writeSemaphoreId = ((long)r.Next() << 32) + r.Next();
            Section section;

            section = new Section(name, blockSize * numberOfBlocks, MemoryProtection.ReadWrite);

            using (var view = section.MapView(Marshal.SizeOf(typeof(BufferHeader))))
            {
                BufferHeader header = new BufferHeader();

                header.BlockSize = blockSize;
                header.NumberOfBlocks = numberOfBlocks;
                header.ReadSemaphoreId = readSemaphoreId;
                header.WriteSemaphoreId = writeSemaphoreId;
                header.ReadPosition = 0;
                header.WritePosition = 0;

                view.WriteStruct<BufferHeader>(header);
            }

            return new IpcCircularBuffer(
                section,
                name,
                new Semaphore(name + "_" + readSemaphoreId.ToString("x"), 0, numberOfBlocks),
                new Semaphore(name + "_" + writeSemaphoreId.ToString("x"), numberOfBlocks, numberOfBlocks)
                );
        }

        public static IpcCircularBuffer Open(string name)
        {
            return new IpcCircularBuffer(new Section(name, SectionAccess.All), name, null, null);
        }

        private Section _section;
        private SectionView _sectionView;
        private Semaphore _readSemaphore;
        private Semaphore _writeSemaphore;

        private BufferHeader* _header;
        private void* _data;

        private IpcCircularBuffer(Section section, string sectionName, Semaphore readSemaphore, Semaphore writeSemaphore)
        {
            BufferHeader header;

            _section = section;

            _sectionView = section.MapView(Marshal.SizeOf(typeof(BufferHeader)));
            header = _sectionView.ReadStruct<BufferHeader>();
            _sectionView.Dispose();

            if (readSemaphore == null || writeSemaphore == null)
            {
                _readSemaphore = new Semaphore(sectionName + "_" + header.ReadSemaphoreId.ToString("x"));
                _writeSemaphore = new Semaphore(sectionName + "_" + header.WriteSemaphoreId.ToString("x"));
            }
            else
            {
                _readSemaphore = readSemaphore;
                _writeSemaphore = writeSemaphore;
            }

            _sectionView = _section.MapView(header.BlockSize * header.NumberOfBlocks);
            _header = (BufferHeader*)_sectionView.Memory;
            _data = &_header->Data;
        }

        public T Read<T>()
            where T : struct
        {
            using (var data = this.Read())
                return data.ReadStruct<T>();
        }

        public MemoryAlloc Read()
        {
            var data = new MemoryAlloc(_header->BlockSize);

            this.Read(data);

            return data;
        }

        public void Read(MemoryRegion data)
        {
            this.Read((void*)data.Memory);
        }

        public void Read(void* buffer)
        {
            int readPosition;

            // Wait for a block to read.
            _readSemaphore.Wait();

            // Get a read position while simultaneously incrementing it 
            // and wrapping it if necessary.
            while (true)
            {
                readPosition = _header->ReadPosition;

                if (System.Threading.Interlocked.CompareExchange(
                    ref _header->ReadPosition,
                    (readPosition + 1) % _header->NumberOfBlocks,
                    readPosition
                    ) == readPosition)
                    break;
            }

            // Copy the data across.
            Win32.RtlMoveMemory(
                new IntPtr(buffer),
                (new IntPtr(_data)).Increment(readPosition * _header->BlockSize),
                _header->BlockSize.ToIntPtr()
                );

            // Release the write semaphore to allow a writer to write one more block.
            _writeSemaphore.Release();
        }

        public void Write<T>(T s)
            where T : struct
        {
            using (var data = new MemoryAlloc(Marshal.SizeOf(typeof(T))))
            {
                data.WriteStruct<T>(s);
                this.Write((MemoryRegion)data);
            }
        }

        public void Write(MemoryRegion data)
        {
            this.Write(data, 0);
        }

        public void Write(MemoryRegion data, int offset)
        {
            this.Write((void*)data.Memory.Increment(offset));
        }

        public void Write(void* buffer)
        {
            int writePosition;

            // Wait for an available write slot.
            _writeSemaphore.Wait();

            // Get a write position while simultaneously incrementing it
            // and wrapping it if necessary.
            while (true)
            {
                writePosition = _header->WritePosition;

                if (System.Threading.Interlocked.CompareExchange(
                    ref _header->WritePosition,
                    (writePosition + 1) % _header->NumberOfBlocks,
                    writePosition
                    ) == writePosition)
                    break;
            }

            // Copy the data across.
            Win32.RtlMoveMemory(
                (new IntPtr(_data)).Increment(writePosition * _header->BlockSize),
                new IntPtr(buffer),
                _header->BlockSize.ToIntPtr()
                );

            // Release the read semaphore to allow a reader to read one more block.
            _readSemaphore.Release();
        }
    }
}
