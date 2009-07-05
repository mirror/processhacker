/*
 * Process Hacker - 
 *   file handle
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

namespace ProcessHacker.Native.Objects
{
    /// <summary>
    /// Represents a handle to a file.
    /// </summary>
    public class FileHandle : NativeHandle<FileAccess>
    {
        public static FileHandle FromFileStream(System.IO.FileStream fileStream)
        {
            return FromHandle(fileStream.SafeFileHandle.DangerousGetHandle());
        }

        public static FileHandle FromHandle(IntPtr handle)
        {
            return new FileHandle(handle, false);
        }

        protected FileHandle()
        { }

        private FileHandle(IntPtr handle, bool owned)
            : base(handle, owned)
        { }

        public FileHandle(string fileName, FileAccess desiredAccess)
            : this(fileName, desiredAccess, FileShareMode.Exclusive)
        { }

        public FileHandle(string fileName, FileAccess desiredAccess, FileShareMode shareMode)
            : this(fileName, desiredAccess, shareMode, FileCreationDisposition.CreateAlways)
        { }

        public FileHandle(string fileName, FileAccess desiredAccess, FileShareMode shareMode,
            FileCreationDisposition creationDisposition)
        {
            this.Handle = Win32.CreateFile(fileName, desiredAccess, shareMode, 0, creationDisposition, 0, IntPtr.Zero);

            if (this.Handle.ToInt32() == -1)
            {
                this.MarkAsInvalid();
                Win32.ThrowLastError();
            }
        }

        public string GetFileName()
        {
            NtStatus status;
            IoStatusBlock ioStatusBlock;

            using (var data = new MemoryAlloc(0x1000))
            {
                if ((status = Win32.NtQueryInformationFile(
                    this,
                    out ioStatusBlock,
                    data,
                    data.Size,
                    FileInformationClass.FileNameInformation
                    )) >= NtStatus.Error)
                    Win32.ThrowLastError(status);

                if (ioStatusBlock.Status >= NtStatus.Error)
                    Win32.ThrowLastError(ioStatusBlock.Status);

                FileNameInformation info = data.ReadStruct<FileNameInformation>();

                return Marshal.PtrToStringUni(
                    data.Memory.Increment(Marshal.OffsetOf(typeof(FileNameInformation), "FileName")),
                    info.FileNameLength / 2
                    );
            }
        }

        public long GetSize()
        {
            long fileSize;

            if (!Win32.GetFileSizeEx(this, out fileSize))
                Win32.ThrowLastError();

            return fileSize;
        }

        public string GetVolumeLabel()
        {
            NtStatus status;
            IoStatusBlock ioStatusBlock;

            using (var data = new MemoryAlloc(0x400))
            {
                if ((status = Win32.NtQueryVolumeInformationFile(
                    this,
                    out ioStatusBlock,
                    data,
                    data.Size,
                    FsInformationClass.FileFsVolumeInformation
                    )) >= NtStatus.Error)
                    Win32.ThrowLastError(status);

                if (ioStatusBlock.Status >= NtStatus.Error)
                    Win32.ThrowLastError(ioStatusBlock.Status);

                FileFsVolumeInformation info = data.ReadStruct<FileFsVolumeInformation>();

                return Marshal.PtrToStringUni(
                    data.Memory.Increment(Marshal.OffsetOf(typeof(FileFsVolumeInformation), "VolumeLabel")),
                    info.VolumeLabelLength / 2
                    );
            }
        }

        /// <summary>
        /// Sends an I/O control message to the device's associated driver.
        /// </summary>
        /// <param name="controlCode">The device-specific control code.</param>
        /// <param name="inBuffer">The input.</param>
        /// <param name="outBuffer">The output buffer.</param>
        /// <returns>The bytes returned in the output buffer.</returns>
        public unsafe int IoControl(uint controlCode, byte[] inBuffer, byte[] outBuffer)
        {
            byte[] inArr = inBuffer;
            int inLen = inArr != null ? inBuffer.Length : 0;
            byte[] outArr = outBuffer;
            int outLen = outArr != null ? outBuffer.Length : 0;

            fixed (byte* inArrPtr = inArr)
            {
                fixed (byte* outArrPtr = outArr)
                {
                    return this.IoControl(controlCode, inArrPtr, inLen, outArrPtr, outLen);
                }
            }
        }

        public unsafe int IoControl(uint controlCode,
            byte* inBuffer, int inBufferLength,
            byte[] outBuffer)
        {
            int outLen = outBuffer != null ? outBuffer.Length : 0;

            fixed (byte* outBufferPtr = outBuffer)
            {
                return this.IoControl(controlCode, inBuffer, inBufferLength, outBufferPtr, outLen);
            }
        }

        public unsafe int IoControl(uint controlCode,
            byte* inBuffer, int inBufferLength,
            byte* outBuffer, int outBufferLength)
        {
            byte* dummy = stackalloc byte[0];
            int inLen = inBuffer != null ? inBufferLength : 0;
            int outLen = outBuffer != null ? outBufferLength : 0;

            if (inBuffer == null)
                inBuffer = dummy;
            if (outBuffer == null)
                outBuffer = dummy;

            NtStatus status;
            IoStatusBlock isb;

            // The reason I'm using NtDeviceIoControlFile is because DeviceIoControl 
            // converts the NTSTATUS to a Win32 error, and sometimes I need 
            // the actual NTSTATUS value.
            if ((status = Win32.NtDeviceIoControlFile(
                this,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero,
                out isb,
                (int)controlCode,
                inBuffer,
                inLen,
                outBuffer,
                outLen
                )) >= NtStatus.Error)
                Win32.ThrowLastError(status);

            // Not a good idea, but...
            if (isb.Status >= NtStatus.Error)
                Win32.ThrowLastError(isb.Status);

            // Information contains the return length.
            return isb.Information.ToInt32();
        }

        /// <summary>
        /// Reads data from the file.
        /// </summary>
        /// <param name="length">The length to read.</param>
        /// <returns>The read data.</returns>
        public byte[] Read(int length)
        {
            byte[] buffer = new byte[length];

            this.Read(buffer);

            return buffer;
        }

        /// <summary>
        /// Reads data from the file.
        /// </summary>
        /// <param name="buffer">The buffer to store the data in.</param>
        /// <returns>The number of bytes read from the file.</returns>
        public int Read(byte[] buffer)
        {
            int bytesRead;

            if (!Win32.ReadFile(this, buffer, buffer.Length, out bytesRead, IntPtr.Zero))
                Win32.ThrowLastError();

            return bytesRead;
        }

        /// <summary>
        /// Writes data to the file.
        /// </summary>
        /// <param name="buffer">The data.</param>
        /// <returns>The number of bytes written to the file.</returns>
        public int Write(byte[] buffer)
        {
            int bytesWritten;

            if (!Win32.WriteFile(this, buffer, buffer.Length, out bytesWritten, IntPtr.Zero))
                Win32.ThrowLastError();

            return bytesWritten;
        }

        /// <summary>
        /// Writes data to the file.
        /// </summary>
        /// <param name="buffer">The data.</param>
        /// <param name="length">The number of bytes to write.</param>
        /// <returns>The number of bytes written to the file.</returns>
        public unsafe int Write(void* buffer, int length)
        {
            int bytesWritten;

            if (!Win32.WriteFile(this, buffer, length, out bytesWritten, IntPtr.Zero))
                Win32.ThrowLastError();

            return bytesWritten;
        }
    }
}
