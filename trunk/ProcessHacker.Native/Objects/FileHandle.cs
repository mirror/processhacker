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
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ProcessHacker.Common;
using ProcessHacker.Common.Objects;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Security;

namespace ProcessHacker.Native.Objects
{
    /// <summary>
    /// Represents a handle to a file.
    /// </summary>
    public class FileHandle : NativeHandle<FileAccess>
    {
        public delegate bool EnumFilesDelegate(FileEntry file);
        public delegate bool EnumStreamsDelegate(FileStreamEntry stream);

        public static FileHandle FromFileStream(System.IO.FileStream fileStream)
        {
            return FromHandle(fileStream.SafeFileHandle.DangerousGetHandle());
        }

        public static FileHandle FromHandle(IntPtr handle)
        {
            return new FileHandle(handle, false);
        }

        /// <summary>
        /// Creates or opens a file for synchronous access.
        /// </summary>
        /// <param name="access">The desired access to the file.</param>
        /// <param name="fileName">
        /// An object name identifying the file to open. To use a DOS format 
        /// file name, prepend "\??\" to the file name.
        /// </param>
        /// <param name="createOptions">Options to use when creating the file.</param>
        public static FileHandle Create(FileAccess access, string fileName, FileCreateOptions createOptions)
        {
            return Create(access, fileName, FileShareMode.Exclusive, FileCreationDisposition.OpenIf, createOptions);
        }

        /// <summary>
        /// Creates or opens a file for synchronous access.
        /// </summary>
        /// <param name="access">The desired access to the file.</param>
        /// <param name="fileName">
        /// An object name identifying the file to open. To use a DOS format 
        /// file name, prepend "\??\" to the file name.
        /// </param>
        /// <param name="shareMode">The types of access to the file to grant to other threads.</param>
        /// <param name="createOptions">Options to use when creating the file.</param>
        public static FileHandle Create(FileAccess access, string fileName, FileShareMode shareMode, FileCreateOptions createOptions)
        {
            return Create(access, fileName, shareMode, FileCreationDisposition.OpenIf, createOptions);
        }

        public static FileHandle Create(
            FileAccess access,
            string fileName,
            FileShareMode shareMode,
            FileCreationDisposition creationDisposition,
            FileCreateOptions createOptions
            )
        {
            return Create(access, fileName, null, shareMode, creationDisposition, createOptions);
        }

        public static FileHandle Create(
            FileAccess access,
            string fileName,
            FileHandle rootDirectory,
            FileShareMode shareMode,
            FileCreationDisposition creationDisposition,
            FileCreateOptions createOptions
            )
        {
            FileIoStatus status;

            return Create(
                access,
                fileName,
                ObjectFlags.CaseInsensitive,
                rootDirectory,
                shareMode,
                creationDisposition,
                0,
                FileAttributes.Normal,
                createOptions,
                out status
                );
        }

        public static FileHandle Create(
            FileAccess access,
            string fileName,
            ObjectFlags objectFlags,
            FileHandle rootDirectory,
            FileShareMode shareMode,
            FileCreationDisposition creationDisposition,
            long allocationSize,
            FileAttributes attributes,
            FileCreateOptions createOptions,
            out FileIoStatus ioStatus
            )
        {
            NtStatus status;
            ObjectAttributes oa = new ObjectAttributes(fileName, objectFlags, rootDirectory);
            IoStatusBlock isb;
            IntPtr handle;

            try
            {
                if ((status = Win32.NtCreateFile(
                    out handle,
                    access,
                    ref oa,
                    out isb,
                    ref allocationSize,
                    attributes,
                    shareMode,
                    creationDisposition,
                    createOptions,
                    IntPtr.Zero,
                    0
                    )) >= NtStatus.Error)
                    Win32.ThrowLastError(status);

                ioStatus = (FileIoStatus)isb.Information.ToInt32();
            }
            finally
            {
                oa.Dispose();
            }

            return new FileHandle(handle, true);
        }

        public static FileHandle CreateWin32(string fileName, FileAccess desiredAccess)
        {
            return CreateWin32(fileName, desiredAccess, FileShareMode.Exclusive);
        }

        public static FileHandle CreateWin32(string fileName, FileAccess desiredAccess, FileShareMode shareMode)
        {
            return CreateWin32(fileName, desiredAccess, shareMode, FileCreationDispositionWin32.OpenAlways);
        }

        public static FileHandle CreateWin32(string fileName, FileAccess desiredAccess, FileShareMode shareMode,
            FileCreationDispositionWin32 creationDisposition)
        {
            IntPtr handle;
            
            handle = Win32.CreateFile(fileName, desiredAccess, shareMode, 0, creationDisposition, 0, IntPtr.Zero);

            if (handle == NativeHandle.MinusOne)
                Win32.ThrowLastError();

            return new FileHandle(handle, true);
        }

        public static void Delete(string fileName, ObjectFlags objectFlags)
        {
            NtStatus status;
            ObjectAttributes oa = new ObjectAttributes(fileName, objectFlags, null);

            try
            {
                if ((status = Win32.NtDeleteFile(ref oa)) >= NtStatus.Error)
                    Win32.ThrowLastError(status);
            }
            finally
            {
                oa.Dispose();
            }
        }

        protected FileHandle()
        { }

        protected FileHandle(IntPtr handle, bool owned)
            : base(handle, owned)
        { }

        /// <summary>
        /// Opens an existing file for synchronous access.
        /// </summary>
        /// <param name="fileName">
        /// An object name identifying the file to open. To use a DOS format 
        /// file name, prepend "\??\" to the file name.
        /// </param>
        /// <param name="access">The desired access to the file.</param>
        public FileHandle(string fileName, FileAccess access)
            : this(fileName, FileShareMode.Exclusive, access)
        { }

        /// <summary>
        /// Opens an existing file for synchronous access.
        /// </summary>
        /// <param name="fileName">
        /// An object name identifying the file to open. To use a DOS format 
        /// file name, prepend "\??\" to the file name.
        /// </param>
        /// <param name="shareMode">The share mode to use.</param>
        /// <param name="access">The desired access to the file.</param>
        public FileHandle(string fileName, FileShareMode shareMode, FileAccess access)
            : this(fileName, shareMode, FileCreateOptions.NonDirectoryFile | FileCreateOptions.SynchronousIoNonAlert, access)
        { }

        /// <summary>
        /// Opens an existing file.
        /// </summary>
        /// <param name="fileName">
        /// An object name identifying the file to open. To use a DOS format 
        /// file name, prepend "\??\" to the file name.
        /// </param>
        /// <param name="shareMode">The share mode to use.</param>
        /// <param name="openOptions">Open options to use.</param>
        /// <param name="access">The desired access to the file.</param>
        public FileHandle(string fileName, FileShareMode shareMode, FileCreateOptions openOptions, FileAccess access)
            : this(fileName, ObjectFlags.CaseInsensitive, null, shareMode, openOptions, access)
        { }

        /// <summary>
        /// Opens an existing file.
        /// </summary>
        /// <param name="fileName">
        /// An object name identifying the file to open. To use a DOS format 
        /// file name, prepend "\??\" to the file name.
        /// </param>
        /// <param name="objectFlags">Flags to use when opening the object.</param>
        /// <param name="rootDirectory">The directory to open the file relative to.</param>
        /// <param name="shareMode">The share mode to use.</param>
        /// <param name="openOptions">Open options to use.</param>
        /// <param name="access">The desired access to the file.</param>
        public FileHandle(
            string fileName,
            ObjectFlags objectFlags,
            FileHandle rootDirectory,
            FileShareMode shareMode,
            FileCreateOptions openOptions,
            FileAccess access
            )
        {
            NtStatus status;
            ObjectAttributes oa = new ObjectAttributes(fileName, objectFlags, rootDirectory);
            IoStatusBlock isb;
            IntPtr handle;

            try
            {
                if ((status = Win32.NtOpenFile(
                    out handle,
                    access,
                    ref oa,
                    out isb,
                    shareMode,
                    openOptions
                    )) >= NtStatus.Error)
                    Win32.ThrowLastError(status);
            }
            finally
            {
                oa.Dispose();
            }

            this.Handle = handle;
        }

        public AsyncIoContext BeginFsControl(
            int controlCode,
            byte[] inBuffer,
            int inBufferOffset,
            int inBufferLength,
            byte[] outBuffer,
            int outBufferOffset,
            int outBufferLength
            )
        {
            AsyncIoContext asyncContext;
            PinnedObject<byte[]> pinnedInBuffer = null;
            PinnedObject<byte[]> pinnedOutBuffer = null;

            Utils.ValidateBuffer(inBuffer, inBufferOffset, inBufferLength, true);
            Utils.ValidateBuffer(outBuffer, outBufferOffset, outBufferLength, true);

            asyncContext = new AsyncIoContext(this);

            if (inBuffer != null)
            {
                pinnedInBuffer = new PinnedObject<byte[]>(inBuffer);
                asyncContext.KeepAlive(pinnedInBuffer);
            }

            if (outBuffer != null)
            {
                pinnedOutBuffer = new PinnedObject<byte[]>(outBuffer);
                asyncContext.KeepAlive(pinnedOutBuffer);
            }

            this.BeginFsControl(
                asyncContext,
                controlCode,
                pinnedInBuffer != null ? pinnedInBuffer.Address.Increment(inBufferOffset) : IntPtr.Zero,
                inBufferLength,
                pinnedOutBuffer != null ? pinnedOutBuffer.Address.Increment(outBufferOffset) : IntPtr.Zero,
                outBufferLength
                );

            return asyncContext;
        }

        public AsyncIoContext BeginFsControl(int controlCode, MemoryRegion inBuffer, MemoryRegion outBuffer)
        {
            AsyncIoContext asyncContext = new AsyncIoContext(this);

            asyncContext.KeepAlive(inBuffer);
            asyncContext.KeepAlive(outBuffer);
            this.BeginFsControl(
                asyncContext,
                controlCode,
                inBuffer ?? IntPtr.Zero,
                inBuffer != null ? inBuffer.Size : 0,
                outBuffer ?? IntPtr.Zero,
                outBuffer != null ? outBuffer.Size : 0
                );

            return asyncContext;
        }

        protected void BeginFsControl(
            AsyncIoContext asyncContext,
            int controlCode,
            IntPtr inBuffer,
            int inBufferLength,
            IntPtr outBuffer,
            int outBufferLength
            )
        {
            NtStatus status;

            status = Win32.NtFsControlFile(
                this,
                asyncContext.EventHandle,
                null,
                IntPtr.Zero,
                asyncContext.StatusMemory,
                controlCode,
                inBuffer,
                inBufferLength,
                outBuffer,
                outBufferLength
                );

            asyncContext.NotifyBegin();

            if (status != NtStatus.Pending)
            {
                // The operation finished synchronously.
                asyncContext.CompletedSynchronously = true;
                asyncContext.Status = status;
            }
        }

        public AsyncIoContext BeginIoControl(
            int controlCode,
            byte[] inBuffer,
            int inBufferOffset,
            int inBufferLength,
            byte[] outBuffer,
            int outBufferOffset,
            int outBufferLength
            )
        {
            AsyncIoContext asyncContext;
            PinnedObject<byte[]> pinnedInBuffer = null;
            PinnedObject<byte[]> pinnedOutBuffer = null;

            Utils.ValidateBuffer(inBuffer, inBufferOffset, inBufferLength, true);
            Utils.ValidateBuffer(outBuffer, outBufferOffset, outBufferLength, true);

            asyncContext = new AsyncIoContext(this);

            if (inBuffer != null)
            {
                pinnedInBuffer = new PinnedObject<byte[]>(inBuffer);
                asyncContext.KeepAlive(pinnedInBuffer);
            }

            if (outBuffer != null)
            {
                pinnedOutBuffer = new PinnedObject<byte[]>(outBuffer);
                asyncContext.KeepAlive(pinnedOutBuffer);
            }

            this.BeginIoControl(
                asyncContext,
                controlCode,
                pinnedInBuffer != null ? pinnedInBuffer.Address.Increment(inBufferOffset) : IntPtr.Zero,
                inBufferLength,
                pinnedOutBuffer != null ? pinnedOutBuffer.Address.Increment(outBufferOffset) : IntPtr.Zero,
                outBufferLength
                );

            return asyncContext;
        }

        public AsyncIoContext BeginIoControl(int controlCode, MemoryRegion inBuffer, MemoryRegion outBuffer)
        {
            AsyncIoContext asyncContext = new AsyncIoContext(this);

            asyncContext.KeepAlive(inBuffer);
            asyncContext.KeepAlive(outBuffer);
            this.BeginIoControl(
                asyncContext,
                controlCode,
                inBuffer ?? IntPtr.Zero,
                inBuffer != null ? inBuffer.Size : 0,
                outBuffer ?? IntPtr.Zero,
                outBuffer != null ? outBuffer.Size : 0
                );

            return asyncContext;
        }

        protected void BeginIoControl(
            AsyncIoContext asyncContext,
            int controlCode,
            IntPtr inBuffer,
            int inBufferLength,
            IntPtr outBuffer,
            int outBufferLength
            )
        {            
            NtStatus status;

            status = Win32.NtDeviceIoControlFile(
                this,
                asyncContext.EventHandle,
                null,
                IntPtr.Zero,
                asyncContext.StatusMemory,
                controlCode,
                inBuffer,
                inBufferLength,
                outBuffer,
                outBufferLength
                );

            asyncContext.NotifyBegin();

            if (status != NtStatus.Pending)
            {
                // The operation finished synchronously.
                asyncContext.CompletedSynchronously = true;
                asyncContext.Status = status;
            }
        }

        public AsyncIoContext BeginLock(long offset, long length)
        {
            return this.BeginLock(offset, length, false);
        }

        public AsyncIoContext BeginLock(long offset, long length, bool wait)
        {
            return this.BeginLock(offset, length, wait, true);
        }

        public AsyncIoContext BeginLock(long offset, long length, bool wait, bool exclusive)
        {
            NtStatus status;
            AsyncIoContext asyncContext = new AsyncIoContext(this);

            status = Win32.NtLockFile(
                this,
                asyncContext.EventHandle,
                null,
                IntPtr.Zero,
                asyncContext.StatusMemory,
                ref offset,
                ref length,
                0,
                !wait,
                exclusive
                );

            asyncContext.NotifyBegin();

            if (status != NtStatus.Pending)
            {
                // The operation finished synchronously.
                asyncContext.CompletedSynchronously = true;
                asyncContext.Status = status;
            }

            return asyncContext;
        }

        public AsyncIoContext BeginRead(byte[] buffer)
        {
            return this.BeginRead(buffer, 0, buffer.Length);
        }

        public AsyncIoContext BeginRead(byte[] buffer, int offset, int length)
        {
            AsyncIoContext asyncContext;
            PinnedObject<byte[]> pinnedBuffer;

            Utils.ValidateBuffer(buffer, offset, length);

            // Pin the buffer because the I/O system may be writing to it after 
            // this call returns.
            pinnedBuffer = new PinnedObject<byte[]>(buffer);
            asyncContext = new AsyncIoContext(this);
            asyncContext.KeepAlive(pinnedBuffer);
            this.BeginRead(asyncContext, pinnedBuffer.Address.Increment(offset), length);

            return asyncContext;
        }

        public AsyncIoContext BeginRead(MemoryRegion buffer)
        {
            AsyncIoContext asyncContext = new AsyncIoContext(this);

            asyncContext.KeepAlive(buffer);
            this.BeginRead(asyncContext, buffer, buffer.Size);

            return asyncContext;
        }

        protected void BeginRead(AsyncIoContext asyncContext, IntPtr buffer, int length)
        {
            NtStatus status;

            status = Win32.NtReadFile(
                this,
                asyncContext.EventHandle,
                null,
                IntPtr.Zero,
                asyncContext.StatusMemory,
                buffer,
                length,
                IntPtr.Zero,
                IntPtr.Zero
                );

            asyncContext.NotifyBegin();

            if (status != NtStatus.Pending)
            {
                // The operation finished synchronously.
                asyncContext.CompletedSynchronously = true;
                asyncContext.Status = status;
            }
        }

        public AsyncIoContext BeginWrite(byte[] buffer)
        {
            return this.BeginWrite(buffer, 0, buffer.Length);
        }

        public AsyncIoContext BeginWrite(byte[] buffer, int offset, int length)
        {
            AsyncIoContext asyncContext;
            PinnedObject<byte[]> pinnedBuffer;

            Utils.ValidateBuffer(buffer, offset, length);

            pinnedBuffer = new PinnedObject<byte[]>(buffer);
            asyncContext = new AsyncIoContext(this);
            asyncContext.KeepAlive(pinnedBuffer);
            this.BeginWrite(asyncContext, pinnedBuffer.Address.Increment(offset), length);

            return asyncContext;
        }

        public AsyncIoContext BeginWrite(MemoryRegion buffer)
        {
            AsyncIoContext asyncContext = new AsyncIoContext(this);

            asyncContext.KeepAlive(buffer);
            this.BeginWrite(asyncContext, buffer, buffer.Size);

            return asyncContext;
        }

        protected void BeginWrite(AsyncIoContext asyncContext, IntPtr buffer, int length)
        {
            NtStatus status;

            status = Win32.NtWriteFile(
                this,
                asyncContext.EventHandle,
                null,
                IntPtr.Zero,
                asyncContext.StatusMemory,
                buffer,
                length,
                IntPtr.Zero,
                IntPtr.Zero
                );

            asyncContext.NotifyBegin();

            if (status != NtStatus.Pending)
            {
                // The operation finished synchronously.
                asyncContext.CompletedSynchronously = true;
                asyncContext.Status = status;
            }
        }

        public IoStatusBlock CancelIo()
        {
            NtStatus status;
            IoStatusBlock isb;

            if ((status = Win32.NtCancelIoFile(this, out isb)) >= NtStatus.Error)
                Win32.ThrowLastError(status);

            return isb;
        }

        internal void CancelIo(IntPtr isb)
        {
            NtStatus status;

            if ((status = Win32.NtCancelIoFile(this, isb)) >= NtStatus.Error)
                Win32.ThrowLastError(status);
        }

        /// <summary>
        /// Deletes the file when the handle is closed.
        /// </summary>
        public void Delete()
        {
            this.SetStruct<FileDispositionInformation>(
                FileInformationClass.FileDispositionInformation,
                new FileDispositionInformation() { DeleteFile = true }
                );
        }

        protected int EndCommonIo(AsyncIoContext asyncContext)
        {
            asyncContext.Wait();
            asyncContext.NotifyEnd();

            if (asyncContext.Status >= NtStatus.Error)
                Win32.ThrowLastError(asyncContext.Status);

            return asyncContext.StatusBlock.Information.ToInt32();
        }

        public int EndFsControl(AsyncIoContext asyncContext)
        {
            return this.EndCommonIo(asyncContext);
        }

        public int EndIoControl(AsyncIoContext asyncContext)
        {
            return this.EndCommonIo(asyncContext);
        }

        public bool EndLock(AsyncIoContext asyncContext)
        {
            asyncContext.Wait();
            asyncContext.NotifyEnd();

            if (asyncContext.Status == NtStatus.LockNotGranted)
                return false;

            if (asyncContext.Status >= NtStatus.Error)
                Win32.ThrowLastError(asyncContext.Status);

            return true;
        }

        public int EndRead(AsyncIoContext asyncContext)
        {
            return this.EndCommonIo(asyncContext);
        }

        public int EndWrite(AsyncIoContext asyncContext)
        {
            return this.EndCommonIo(asyncContext);
        }

        public void EnumFiles(EnumFilesDelegate callback)
        {
            NtStatus status;
            IoStatusBlock isb;
            bool firstTime = true;

            using (var data = new MemoryAlloc(0x400))
            {
                while (true)
                {
                    // Query the directory, doubling the buffer size each 
                    // time NtQueryDirectoryFile fails. We will also handle 
                    // any pending status.

                    while (true)
                    {
                        status = Win32.NtQueryDirectoryFile(
                            this,
                            IntPtr.Zero,
                            null,
                            IntPtr.Zero,
                            out isb,
                            data,
                            data.Size,
                            FileInformationClass.FileDirectoryInformation,
                            false,
                            IntPtr.Zero,
                            firstTime
                            );

                        // Our ISB is on the stack, so we have to wait for the operation to complete 
                        // before continuing.
                        if (status == NtStatus.Pending)
                        {
                            this.Wait();
                            status = isb.Status;
                        }

                        if (status == NtStatus.BufferOverflow || status == NtStatus.InfoLengthMismatch)
                            data.Resize(data.Size * 2);
                        else
                            break;
                    }

                    // If we don't have any entries to read, exit.
                    if (status == NtStatus.NoMoreFiles)
                        break;

                    // Handle any errors.
                    if (status >= NtStatus.Error)
                        Win32.ThrowLastError(status);

                    // Read the list of files we got in this batch.

                    int i = 0;

                    while (true)
                    {
                        FileDirectoryInformation info = data.ReadStruct<FileDirectoryInformation>(i, 0);
                        string name = data.ReadUnicodeString(
                            i + FileDirectoryInformation.FileNameOffset,
                            info.FileNameLength / 2
                            );

                        if (!callback(new FileEntry(
                            name,
                            info.FileIndex,
                            DateTime.FromFileTime(info.CreationTime),
                            DateTime.FromFileTime(info.LastAccessTime),
                            DateTime.FromFileTime(info.LastWriteTime),
                            DateTime.FromFileTime(info.ChangeTime),
                            info.EndOfFile,
                            info.AllocationSize,
                            info.FileAttributes
                            )))
                            return;

                        if (info.NextEntryOffset == 0)
                            break;
                        else
                            i += info.NextEntryOffset;
                    }

                    firstTime = false;

                    // Go back and get another batch of file entries.
                }
            }
        }

        public void EnumStreams(EnumStreamsDelegate callback)
        {
            using (var data = this.QueryVariableSize(FileInformationClass.FileStreamInformation))
            {
                int i = 0;

                while (true)
                {
                    FileStreamInformation info = data.ReadStruct<FileStreamInformation>(i, 0);
                    string name = data.ReadUnicodeString(
                        i + FileStreamInformation.StreamNameOffset,
                        info.StreamNameLength / 2
                        );

                    if (!callback(new FileStreamEntry(name, info.StreamSize, info.StreamAllocationSize)))
                        return;

                    if (info.NextEntryOffset == 0)
                        break;
                    else
                        i += info.NextEntryOffset;
                }
            }
        }

        public void Flush()
        {
            NtStatus status;
            IoStatusBlock isb;

            status = Win32.NtFlushBuffersFile(
                this,
                out isb
                );

            if (status == NtStatus.Pending)
            {
                this.Wait();
                status = isb.Status;
            }

            if (status >= NtStatus.Error)
                Win32.ThrowLastError(status);
        }

        public int FsControl(int controlCode, byte[] inBuffer, byte[] outBuffer)
        {
            return this.FsControl(
                controlCode,
                inBuffer,
                0,
                inBuffer != null ? inBuffer.Length : 0,
                outBuffer,
                0,
                outBuffer != null ? outBuffer.Length : 0
                );
        }

        public int FsControl(
            int controlCode,
            byte[] inBuffer,
            int inBufferOffset,
            int inBufferLength,
            byte[] outBuffer,
            int outBufferOffset,
            int outBufferLength
            )
        {
            Utils.ValidateBuffer(inBuffer, inBufferOffset, inBufferLength, true);
            Utils.ValidateBuffer(outBuffer, outBufferOffset, outBufferLength, true);

            unsafe
            {
                fixed (byte* inBufferPtr = inBuffer)
                {
                    fixed (byte* outBufferPtr = outBuffer)
                    {
                        return this.FsControl(
                            controlCode,
                            &inBufferPtr[inBufferOffset],
                            inBuffer != null ? inBuffer.Length : 0,
                            &outBufferPtr[outBufferOffset],
                            outBuffer != null ? outBuffer.Length : 0
                            );
                    }
                }
            }
        }

        public unsafe int FsControl(
            int controlCode,
            void* inBuffer,
            int inBufferLength,
            void* outBuffer,
            int outBufferLength
            )
        {
            return this.FsControl(controlCode, new IntPtr(inBuffer), inBufferLength, new IntPtr(outBuffer), outBufferLength);
        }

        public int FsControl(
            int controlCode,
            IntPtr inBuffer,
            int inBufferLength,
            IntPtr outBuffer,
            int outBufferLength
            )
        {
            NtStatus status;
            int returnLength;

            status = this.FsControl(controlCode, inBuffer, inBufferLength, outBuffer, outBufferLength, out returnLength);

            if (status >= NtStatus.Error)
                Win32.ThrowLastError(status);

            return returnLength;
        }

        public NtStatus FsControl(
            int controlCode,
            IntPtr inBuffer,
            int inBufferLength,
            IntPtr outBuffer,
            int outBufferLength,
            out int returnLength
            )
        {
            NtStatus status;
            IoStatusBlock isb;

            status = Win32.NtFsControlFile(
                this,
                IntPtr.Zero,
                null,
                IntPtr.Zero,
                out isb,
                controlCode,
                inBuffer,
                inBufferLength,
                outBuffer,
                outBufferLength
                );

            if (status == NtStatus.Pending)
            {
                this.Wait();
                status = isb.Status;
            }

            // Information contains the return length.
            returnLength = isb.Information.ToInt32();

            return status;
        }

        public FileAttributes GetAttributes()
        {
            return this.GetBasicInformation().FileAttributes;
        }

        public FileBasicInformation GetBasicInformation()
        {
            return this.QueryStruct<FileBasicInformation>(FileInformationClass.FileBasicInformation);
        }

        public string GetFileName()
        {
            using (var data = this.QueryVariableSize(FileInformationClass.FileNameInformation))
            {
                FileNameInformation info = data.ReadStruct<FileNameInformation>();

                return data.ReadUnicodeString(
                    FileNameInformation.FileNameOffset,
                    info.FileNameLength / 2
                    );
            }
        }

        public FileEntry[] GetFiles()
        {
            List<FileEntry> files = new List<FileEntry>();

            this.EnumFiles((file) =>
                {
                    files.Add(file);
                    return true;
                });

            return files.ToArray();
        }

        public long GetPosition()
        {
            return this.QueryStruct<FilePositionInformation>(FileInformationClass.FilePositionInformation).CurrentByteOffset;
        }

        public FileStreamEntry[] GetStreams()
        {
            List<FileStreamEntry> streams = new List<FileStreamEntry>();

            this.EnumStreams((file) =>
            {
                streams.Add(file);
                return true;
            });

            return streams.ToArray();
        }

        public long GetSize()
        {
            return this.GetStandardInformation().EndOfFile;
        }

        public FileStandardInformation GetStandardInformation()
        {
            return this.QueryStruct<FileStandardInformation>(FileInformationClass.FileStandardInformation);
        }

        public string GetVolumeFsName()
        {
            NtStatus status;
            IoStatusBlock isb;

            using (var data = new MemoryAlloc(0x200))
            {
                if ((status = Win32.NtQueryVolumeInformationFile(
                    this,
                    out isb,
                    data,
                    data.Size,
                    FsInformationClass.FileFsAttributeInformation
                    )) >= NtStatus.Error)
                    Win32.ThrowLastError(status);

                FileFsAttributeInformation info = data.ReadStruct<FileFsAttributeInformation>();

                return Marshal.PtrToStringUni(
                    data.Memory.Increment(Marshal.OffsetOf(typeof(FileFsAttributeInformation), "FileSystemName")),
                    info.FileSystemNameLength / 2
                    );
            }
        }

        public string GetVolumeLabel()
        {
            NtStatus status;
            IoStatusBlock isb;

            using (var data = new MemoryAlloc(0x200))
            {
                if ((status = Win32.NtQueryVolumeInformationFile(
                    this,
                    out isb,
                    data,
                    data.Size,
                    FsInformationClass.FileFsVolumeInformation
                    )) >= NtStatus.Error)
                    Win32.ThrowLastError(status);

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
        public int IoControl(int controlCode, byte[] inBuffer, byte[] outBuffer)
        {
            return this.IoControl(
                controlCode,
                inBuffer,
                0,
                inBuffer != null ? inBuffer.Length : 0,
                outBuffer,
                0,
                outBuffer != null ? outBuffer.Length : 0
                );
        }

        public int IoControl(
            int controlCode,
            byte[] inBuffer,
            int inBufferOffset,
            int inBufferLength,
            byte[] outBuffer,
            int outBufferOffset,
            int outBufferLength
            )
        {
            Utils.ValidateBuffer(inBuffer, inBufferOffset, inBufferLength, true);
            Utils.ValidateBuffer(outBuffer, outBufferOffset, outBufferLength, true);

            unsafe
            {
                fixed (byte* inBufferPtr = inBuffer)
                {
                    fixed (byte* outBufferPtr = outBuffer)
                    {
                        return this.IoControl(
                            controlCode,
                            &inBufferPtr[inBufferOffset],
                            inBuffer != null ? inBuffer.Length : 0,
                            &outBufferPtr[outBufferOffset],
                            outBuffer != null ? outBuffer.Length : 0
                            );
                    }
                }
            }
        }

        public unsafe int IoControl(
            int controlCode,
            byte* inBuffer,
            int inBufferLength,
            byte[] outBuffer
            )
        {
            fixed (byte* outBufferPtr = outBuffer)
            {
                return this.IoControl(
                    controlCode,
                    inBuffer,
                    inBufferLength,
                    outBufferPtr,
                    outBuffer != null ? outBuffer.Length : 0
                    );
            }
        }

        public unsafe int IoControl(
            int controlCode,
            void* inBuffer,
            int inBufferLength,
            void* outBuffer,
            int outBufferLength
            )
        {
            return this.IoControl(controlCode, new IntPtr(inBuffer), inBufferLength, new IntPtr(outBuffer), outBufferLength);
        }

        public int IoControl(
            int controlCode,
            IntPtr inBuffer,
            int inBufferLength,
            IntPtr outBuffer,
            int outBufferLength
            )
        {
            NtStatus status;
            int returnLength;

            status = this.IoControl(controlCode, inBuffer, inBufferLength, outBuffer, outBufferLength, out returnLength);

            if (status >= NtStatus.Error)
                Win32.ThrowLastError(status);

            return returnLength;
        }

        public NtStatus IoControl(
            int controlCode,
            IntPtr inBuffer,
            int inBufferLength,
            IntPtr outBuffer,
            int outBufferLength,
            out int returnLength
            )
        {
            NtStatus status;
            IoStatusBlock isb;

            status = Win32.NtDeviceIoControlFile(
                this,
                IntPtr.Zero,
                null,
                IntPtr.Zero,
                out isb,
                controlCode,
                inBuffer,
                inBufferLength,
                outBuffer,
                outBufferLength
                );

            if (status == NtStatus.Pending)
            {
                this.Wait();
                status = isb.Status;
            }

            // Information contains the return length.
            returnLength = isb.Information.ToInt32();

            return status;
        }

        public bool Lock(long offset, long length)
        {
            return this.Lock(offset, length, false);
        }

        public bool Lock(long offset, long length, bool wait)
        {
            return this.Lock(offset, length, wait, true);
        }

        public bool Lock(long offset, long length, bool wait, bool exclusive)
        {
            NtStatus status;
            IoStatusBlock isb;

            status = Win32.NtLockFile(
                this,
                IntPtr.Zero,
                null,
                IntPtr.Zero,
                out isb,
                ref offset,
                ref length,
                0,
                !wait,
                exclusive
                );

            if (status == NtStatus.Pending)
            {
                this.Wait();
                status = isb.Status;
            }

            if (status == NtStatus.LockNotGranted)
                return false;

            if (status >= NtStatus.Error)
                Win32.ThrowLastError(status);

            return true;
        }

        protected T QueryStruct<T>(FileInformationClass infoClass)
            where T : struct
        {
            NtStatus status;
            IoStatusBlock isb;

            using (var data = new MemoryAlloc(Marshal.SizeOf(typeof(T))))
            {
                if ((status = Win32.NtQueryInformationFile(
                    this,
                    out isb,
                    data,
                    data.Size,
                    infoClass
                    )) >= NtStatus.Error)
                    Win32.ThrowLastError(status);

                return data.ReadStruct<T>();
            }
        }

        protected MemoryAlloc QueryVariableSize(FileInformationClass infoClass)
        {
            NtStatus status;
            IoStatusBlock isb;
            var data = new MemoryAlloc(0x200);

            while (true)
            {
                status = Win32.NtQueryInformationFile(
                    this,
                    out isb,
                    data,
                    data.Size,
                    infoClass
                    );

                if (
                    status == NtStatus.BufferOverflow || 
                    status == NtStatus.BufferTooSmall || 
                    status == NtStatus.InfoLengthMismatch
                    )
                    data.Resize(data.Size * 2);
                else
                    break;
            }

            if (status >= NtStatus.Error)
            {
                data.Dispose();
                Win32.ThrowLastError(status);
            }

            return data;
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
            return this.Read(buffer, 0, buffer.Length);
        }

        public int Read(byte[] buffer, int offset, int length)
        {
            Utils.ValidateBuffer(buffer, offset, length);

            unsafe
            {
                fixed (byte* bufferPtr = buffer)
                {
                    return this.Read(&bufferPtr[offset], length);
                }
            }
        }

        public unsafe int Read(void* buffer, int length)
        {
            return this.Read(new IntPtr(buffer), length);
        }

        public int Read(IntPtr buffer, int length)
        {
            NtStatus status;
            IoStatusBlock isb;

            status = Win32.NtReadFile(
                this,
                IntPtr.Zero,
                null,
                IntPtr.Zero,
                out isb,
                buffer,
                length,
                IntPtr.Zero,
                IntPtr.Zero
                );

            if (status == NtStatus.Pending)
            {
                this.Wait();
                status = isb.Status;
            }

            if (status >= NtStatus.Error)
                Win32.ThrowLastError(status);

            return isb.Information.ToInt32();
        }

        public void SetEnd(long offset)
        {
            this.SetStruct<FileEndOfFileInformation>(
                FileInformationClass.FileEndOfFileInformation,
                new FileEndOfFileInformation() { EndOfFile = offset }
                );
        }

        public void SetIoCompletion(IoCompletionHandle ioCompletionHandle, IntPtr keyContext)
        {
            FileCompletionInformation info = new FileCompletionInformation();

            info.Port = ioCompletionHandle;
            info.Key = keyContext;
            this.SetStruct<FileCompletionInformation>(FileInformationClass.FileCompletionInformation, info);
        }

        public void SetPosition(long offset)
        {
            this.SetStruct<FilePositionInformation>(
                FileInformationClass.FilePositionInformation,
                new FilePositionInformation() { CurrentByteOffset = offset }
                );
        }

        public long SetPosition(long offset, PositionOrigin origin)
        {
            long currentPosition;

            currentPosition = this.GetPosition();

            switch (origin)
            {
                case PositionOrigin.Current:
                    currentPosition += offset;
                    break;
                case PositionOrigin.Start:
                    currentPosition = offset;
                    break;
                case PositionOrigin.End:
                    currentPosition = this.GetSize() + offset;
                    break;
            }

            this.SetPosition(currentPosition);

            return currentPosition;
        }

        protected void SetStruct<T>(FileInformationClass infoClass, T info)
            where T : struct
        {
            NtStatus status;
            IoStatusBlock isb;

            using (var data = new MemoryAlloc(Marshal.SizeOf(typeof(T))))
            {
                data.WriteStruct<T>(info);

                if ((status = Win32.NtSetInformationFile(
                    this,
                    out isb,
                    data,
                    data.Size,
                    infoClass
                    )) >= NtStatus.Error)
                    Win32.ThrowLastError(status);
            }
        }

        public void Unlock(long offset, long length)
        {
            NtStatus status;
            IoStatusBlock isb;

            status = Win32.NtUnlockFile(
                this,
                out isb,
                ref offset,
                ref length,
                0
                );

            if (status >= NtStatus.Error)
                Win32.ThrowLastError(status);
        }

        /// <summary>
        /// Writes data to the file.
        /// </summary>
        /// <param name="buffer">The data.</param>
        /// <returns>The number of bytes written to the file.</returns>
        public int Write(byte[] buffer)
        {
            return this.Write(buffer, 0, buffer.Length);
        }

        public int Write(byte[] buffer, int offset, int length)
        {
            Utils.ValidateBuffer(buffer, offset, length);

            unsafe
            {
                fixed (byte* bufferPtr = buffer)
                {
                    return this.Write(&bufferPtr[offset], length);
                }
            }
        }

        public unsafe int Write(void* buffer, int length)
        {
            return this.Write(new IntPtr(buffer), length);
        }      

        /// <summary>
        /// Writes data to the file.
        /// </summary>
        /// <param name="buffer">The data.</param>
        /// <param name="length">The number of bytes to write.</param>
        /// <returns>The number of bytes written to the file.</returns>
        public int Write(IntPtr buffer, int length)
        {
            NtStatus status;
            IoStatusBlock isb;

            status = Win32.NtWriteFile(
                this,
                IntPtr.Zero,
                null,
                IntPtr.Zero,
                out isb,
                buffer,
                length,
                IntPtr.Zero,
                IntPtr.Zero
                );

            if (status == NtStatus.Pending)
            {
                this.Wait();
                status = isb.Status;
            }

            if (status >= NtStatus.Error)
                Win32.ThrowLastError(status);

            return isb.Information.ToInt32();
        }
    }

    public enum PositionOrigin
    {
        Start,
        Current,
        End
    }

    public sealed class AsyncIoContext : BaseObject, ISynchronizable
    {
        private unsafe sealed class UnmanagedIsb : BaseObject
        {
            private static readonly int _isbSize = Marshal.SizeOf(typeof(IoStatusBlock));

            public static implicit operator IntPtr(UnmanagedIsb isb)
            {
                return isb.Memory;
            }

            private IoStatusBlock* _ioStatusBlock;

            public UnmanagedIsb()
            {
                _ioStatusBlock = (IoStatusBlock*)Heap.FromHandle(MemoryAlloc.PrivateHeap).Allocate(0, _isbSize).ToPointer();
            }

            protected override void DisposeObject(bool disposing)
            {
                if (_ioStatusBlock != null)
                    Heap.FromHandle(MemoryAlloc.PrivateHeap).Free(0, new IntPtr(_ioStatusBlock));
            }

            public IntPtr Information
            {
                get { return _ioStatusBlock->Information; }
                set { _ioStatusBlock->Information = value; }
            }

            public IntPtr Memory
            {
                get { return new IntPtr(_ioStatusBlock); }
            }

            public IntPtr Pointer
            {
                get { return _ioStatusBlock->Pointer; }
                set { _ioStatusBlock->Pointer = value; }
            }

            public NtStatus Status
            {
                get { return _ioStatusBlock->Status; }
                set { _ioStatusBlock->Status = value; }
            }

            public IoStatusBlock Struct
            {
                get { return *_ioStatusBlock; }
                set { *_ioStatusBlock = value; }
            }
        }

        private EventHandle _eventHandle;
        private FileHandle _fileHandle;
        private UnmanagedIsb _isb;
        private bool _completedSynchronously = false;
        private bool _started = false;

        private List<BaseObject> _keepAliveList = new List<BaseObject>();
        private object _tag;

        public AsyncIoContext(FileHandle fileHandle)
        {
            _eventHandle = EventHandle.Create(EventAccess.All, EventType.NotificationEvent, false);
            _fileHandle = fileHandle;
            _isb = new UnmanagedIsb();
            _isb.Status = NtStatus.Pending;

            _fileHandle.Reference();
        }

        protected override void DisposeObject(bool disposing)
        {
            if (_started && !this.Completed)
            {
                throw new InvalidOperationException(
                    "An attempt was made to dispose an asynchronous I/O context object " +
                    "before the I/O operation has finished."
                    );
            }

            this.ClearKeepAlive();

            if (_eventHandle != null)
                _eventHandle.Dispose();
            if (_fileHandle != null)
                _fileHandle.Dereference();
            if (_isb != null)
                _isb.Dispose();
        }

        public bool Cancelled
        {
            get { return this.Status == NtStatus.Cancelled; }
        }

        public bool Completed
        {
            get
            {
                return _isb.Status != NtStatus.Pending;
            }
        }

        public bool CompletedSynchronously
        {
            get { return _completedSynchronously; }
            internal set
            {
                _completedSynchronously = value;
                _eventHandle.Set();
            }
        }

        internal EventHandle EventHandle
        {
            get { return _eventHandle; }
        }

        public FileHandle FileHandle
        {
            get { return _fileHandle; }
        }

        public int Information
        {
            get { return _isb.Information.ToInt32(); }
        }

        public bool Started
        {
            get { return _started; }
        }

        public NtStatus Status
        {
            get { return _isb.Status; }
            internal set { _isb.Status = value; }
        }

        public IoStatusBlock StatusBlock
        {
            get
            {
                return _isb.Struct;
            }
            internal set
            {
                _isb.Struct = value;
            }
        }

        internal IntPtr StatusMemory
        {
            get { return _isb; }
        }

        public object Tag
        {
            get { return _tag; }
            set { _tag = value; }
        }

        public void Cancel()
        {
            if (!_started)
                return;

            _fileHandle.CancelIo();
            this.Wait();
            this.NotifyEnd();
        }

        private void ClearKeepAlive()
        {
            foreach (var obj in _keepAliveList)
                obj.Dereference();

            _keepAliveList.Clear();
        }

        internal void KeepAlive(BaseObject obj)
        {
            _keepAliveList.Add(obj);
            obj.Reference();
        }

        internal void NotifyBegin()
        {
            _started = true;
        }

        internal void NotifyEnd()
        {
            this.ClearKeepAlive();
        }

        #region ISynchronizable Members

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public IntPtr Handle
        {
            get { return _eventHandle.Handle; }
        }

        public NtStatus Wait()
        {
            return _eventHandle.Wait();
        }

        public NtStatus Wait(bool alertable)
        {
            return _eventHandle.Wait(alertable);
        }

        public NtStatus Wait(bool alertable, long timeout)
        {
            return _eventHandle.Wait(alertable, timeout);
        }

        #endregion
    }

    public class FileEntry
    {
        public FileEntry(
            string name,
            int index,
            DateTime creationTime,
            DateTime lastAccessTime,
            DateTime lastWriteTime,
            DateTime changeTime,
            long size,
            long allocationSize,
            FileAttributes attributes
            )
        {
            this.Name = name;
            this.Index = index;
            this.CreationTime = creationTime;
            this.LastAccessTime = lastAccessTime;
            this.LastWriteTime = lastWriteTime;
            this.ChangeTime = changeTime;
            this.Size = size;
            this.AllocationSize = allocationSize;
            this.Attributes = attributes;
        }

        public string Name { get; private set; }
        public int Index { get; private set; }

        public DateTime CreationTime { get; private set; }
        public DateTime LastAccessTime { get; private set; }
        public DateTime LastWriteTime { get; private set; }
        public DateTime ChangeTime { get; private set; }

        public long Size { get; private set; }
        public long AllocationSize { get; private set; }

        public FileAttributes Attributes { get; private set; }
    }

    public class FileStreamEntry
    {
        public FileStreamEntry(string name, long size, long allocationSize)
        {
            this.Name = name;
            this.Size = size;
            this.AllocationSize = allocationSize;
        }

        public string Name { get; private set; }
        public long Size { get; private set; }
        public long AllocationSize { get; private set; }
    }
}
