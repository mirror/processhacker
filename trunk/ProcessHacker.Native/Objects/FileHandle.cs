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
                Win32.ThrowLastError();
        }

        public long GetSize()
        {
            long fileSize;

            if (!Win32.GetFileSizeEx(this, out fileSize))
                Win32.ThrowLastError();

            return fileSize;
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
                Win32.ThrowLastError(status);

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
    }

    public enum FileCreationDisposition : uint
    {
        CreateNew = 1,
        CreateAlways = 2,
        OpenExisting = 3,
        OpenAlways = 4,
        TruncateExisting
    }

    [Flags]
    public enum FileObjectFlags : int
    {
        FileOpen = 0x00000001,
        SynchronousIo = 0x00000002,
        AlertableIo = 0x00000004,
        NoIntermediateBuffering = 0x00000008,
        WriteThrough = 0x00000010,
        SequentialOnly = 0x00000020,
        CacheSupported = 0x00000040,
        NamedPipe = 0x00000080,
        StreamFile = 0x00000100,
        MailSlot = 0x00000200,
        GenerateAuditOnClose = 0x00000400,
        QueueIrpToThread = GenerateAuditOnClose,
        DirectDeviceOpen = 0x00000800,
        FileModified = 0x00001000,
        FileSizeChanged = 0x00002000,
        CleanupComplete = 0x00004000,
        TemporaryFile = 0x00008000,
        DeleteOnClose = 0x00010000,
        OpenedCaseSensitivity = 0x00020000,
        HandleCreated = 0x00040000,
        FileFastIoRead = 0x00080000,
        RandomAccess = 0x00100000,
        FileOpenCancelled = 0x00200000,
        VolumeOpen = 0x00400000,
        RemoteOrigin = 0x01000000,
        SkipCompletionPort = 0x02000000,
        SkipSetEvent = 0x04000000,
        SkipSetFastIo = 0x08000000
    }

    [Flags]
    public enum FileShareMode : uint
    {
        Exclusive = 0,
        Read = 1,
        Write = 2,
        Delete = 4
    }
}
