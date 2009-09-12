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

        public static FileHandle FromFileStream(System.IO.FileStream fileStream)
        {
            return FromHandle(fileStream.SafeFileHandle.DangerousGetHandle());
        }

        public static FileHandle FromHandle(IntPtr handle)
        {
            return new FileHandle(handle, false);
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
            ObjectAttributes oa = new ObjectAttributes(fileName, 0, rootDirectory);
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

        protected FileHandle()
        { }

        private FileHandle(IntPtr handle, bool owned)
            : base(handle, owned)
        { }

        /// <summary>
        /// Opens an existing file for synchronous access.
        /// </summary>
        /// <param name="fileName">An object name identifying the file to open.</param>
        /// <param name="shareMode">The share mode to use.</param>
        /// <param name="access">The desired access to the file.</param>
        public FileHandle(string fileName, FileShareMode shareMode, FileAccess access)
            : this(fileName, null, shareMode, FileCreateOptions.NonDirectoryFile | FileCreateOptions.SynchronousIoNonAlert, access)
        { }

        /// <summary>
        /// Opens an existing file for synchronous access.
        /// </summary>
        /// <param name="fileName">An object name identifying the file to open.</param>
        /// <param name="shareMode">The share mode to use.</param>
        /// <param name="openOptions">
        /// Open options to use. Most callers will specify NonDirectoryFile and 
        /// SynchronousIoNonAlert for working with normal files.
        /// </param>
        /// <param name="access">The desired access to the file.</param>
        public FileHandle(string fileName, FileShareMode shareMode, FileCreateOptions openOptions, FileAccess access)
            : this(fileName, null, shareMode, openOptions, access)
        { }

        /// <summary>
        /// Opens an existing file for synchronous access.
        /// </summary>
        /// <param name="fileName">An object name identifying the file to open.</param>
        /// <param name="rootDirectory">The directory to open the file relative to.</param>
        /// <param name="shareMode">The share mode to use.</param>
        /// <param name="openOptions">
        /// Open options to use. Most callers will specify NonDirectoryFile and 
        /// SynchronousIoNonAlert for working with normal files.
        /// </param>
        /// <param name="access">The desired access to the file.</param>
        public FileHandle(
            string fileName,
            FileHandle rootDirectory,
            FileShareMode shareMode,
            FileCreateOptions openOptions,
            FileAccess access
            )
        {
            NtStatus status;
            ObjectAttributes oa = new ObjectAttributes(fileName, 0, rootDirectory);
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

        public AsyncIoContext BeginIoControl(int controlCode, MemoryRegion inBuffer, MemoryRegion outBuffer)
        {            
            NtStatus status;
            AsyncIoContext asyncContext = new AsyncIoContext(this);

            int inLen = inBuffer != null ? inBuffer.Size : 0;
            int outLen = outBuffer != null ? outBuffer.Size : 0;

            if (inBuffer != null)
                asyncContext.KeepAlive(inBuffer);
            if (outBuffer != null)
                asyncContext.KeepAlive(outBuffer);

            if ((status = Win32.NtDeviceIoControlFile(
                this,
                asyncContext.EventHandle,
                null,
                IntPtr.Zero,
                asyncContext.StatusMemory,
                controlCode,
                inBuffer,
                inLen,
                outBuffer,
                outLen
                )) >= NtStatus.Error)
                Win32.ThrowLastError(status);

            asyncContext.NotifyStarted();

            if (status == NtStatus.Success)
                asyncContext.CompletedSynchronously = true;

            return asyncContext;
        }

        public AsyncIoContext BeginRead(MemoryRegion buffer)
        {
            NtStatus status;
            AsyncIoContext asyncContext = new AsyncIoContext(this);

            asyncContext.KeepAlive(buffer);

            if ((status = Win32.NtReadFile(
                this,
                asyncContext.EventHandle,
                null,
                IntPtr.Zero,
                asyncContext.StatusMemory,
                buffer,
                buffer.Size,
                IntPtr.Zero,
                IntPtr.Zero
                )) >= NtStatus.Error)
                Win32.ThrowLastError(status);

            asyncContext.NotifyStarted();

            if (status == NtStatus.Success)
                asyncContext.CompletedSynchronously = true;

            return asyncContext;
        }

        public AsyncIoContext BeginWrite(MemoryRegion buffer)
        {
            NtStatus status;
            AsyncIoContext asyncContext = new AsyncIoContext(this);

            asyncContext.KeepAlive(buffer);

            if ((status = Win32.NtWriteFile(
                this,
                asyncContext.EventHandle,
                null,
                IntPtr.Zero,
                asyncContext.StatusMemory,
                buffer,
                buffer.Size,
                IntPtr.Zero,
                IntPtr.Zero
                )) >= NtStatus.Error)
                Win32.ThrowLastError(status);

            asyncContext.NotifyStarted();

            if (status == NtStatus.Success)
                asyncContext.CompletedSynchronously = true;

            return asyncContext;
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

        public int EndIoControl(AsyncIoContext asyncContext)
        {
            asyncContext.Wait();

            if (asyncContext.Status.Status >= NtStatus.Error)
                Win32.ThrowLastError(asyncContext.Status.Status);

            return asyncContext.Status.Information.ToInt32();
        }

        public int EndRead(AsyncIoContext asyncContext)
        {
            asyncContext.Wait();

            if (asyncContext.Status.Status >= NtStatus.Error)
                Win32.ThrowLastError(asyncContext.Status.Status);

            return asyncContext.Status.Information.ToInt32();
        }

        public int EndWrite(AsyncIoContext asyncContext)
        {
            asyncContext.Wait();

            if (asyncContext.Status.Status >= NtStatus.Error)
                Win32.ThrowLastError(asyncContext.Status.Status);

            return asyncContext.Status.Information.ToInt32();
        }

        public void EnumFiles(EnumFilesDelegate callback)
        {
            NtStatus status;
            IoStatusBlock isb;
            bool firstTime = true;

            using (var data = new MemoryAlloc(0x20))
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

                        if (status == NtStatus.BufferTooSmall || status == NtStatus.InfoLengthMismatch)
                            data.Resize(data.Size * 2);
                        else
                            break;
                    }

                    if (status == NtStatus.Pending)
                    {
                        this.Wait();
                        status = isb.Status;
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

                        callback(new FileEntry(
                            name,
                            info.FileIndex,
                            Utils.GetDateTimeFromLongTime(info.CreationTime),
                            Utils.GetDateTimeFromLongTime(info.LastAccessTime),
                            Utils.GetDateTimeFromLongTime(info.LastWriteTime),
                            Utils.GetDateTimeFromLongTime(info.ChangeTime),
                            info.EndOfFile,
                            info.AllocationSize,
                            info.FileAttributes
                            ));

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

        public FileBasicInformation GetBasicInformation()
        {
            return this.QueryStruct<FileBasicInformation>(FileInformationClass.FileBasicInformation);
        }

        public string GetFileName()
        {
            NtStatus status;
            IoStatusBlock isb;

            using (var data = new MemoryAlloc(0x1000))
            {
                if ((status = Win32.NtQueryInformationFile(
                    this,
                    out isb,
                    data,
                    data.Size,
                    FileInformationClass.FileNameInformation
                    )) >= NtStatus.Error)
                    Win32.ThrowLastError(status);

                FileNameInformation info = data.ReadStruct<FileNameInformation>();

                return Marshal.PtrToStringUni(
                    data.Memory.Increment(Marshal.OffsetOf(typeof(FileNameInformation), "FileName")),
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
        public unsafe int IoControl(int controlCode, byte[] inBuffer, byte[] outBuffer)
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

        public unsafe int IoControl(
            int controlCode,
            byte* inBuffer,
            int inBufferLength,
            byte[] outBuffer
            )
        {
            int outLen = outBuffer != null ? outBuffer.Length : 0;

            fixed (byte* outBufferPtr = outBuffer)
            {
                return this.IoControl(controlCode, inBuffer, inBufferLength, outBufferPtr, outLen);
            }
        }

        public unsafe int IoControl(
            int controlCode,
            byte* inBuffer,
            int inBufferLength,
            byte* outBuffer,
            int outBufferLength
            )
        {
            NtStatus status;
            IoStatusBlock isb;

            int inLen = inBuffer != null ? inBufferLength : 0;
            int outLen = outBuffer != null ? outBufferLength : 0;

            if ((status = Win32.NtDeviceIoControlFile(
                this,
                IntPtr.Zero,
                null,
                IntPtr.Zero,
                out isb,
                controlCode,
                inBuffer,
                inLen,
                outBuffer,
                outLen
                )) >= NtStatus.Error)
                Win32.ThrowLastError(status);

            // Information contains the return length.
            return isb.Information.ToInt32();
        }

        private T QueryStruct<T>(FileInformationClass infoClass)
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
            unsafe
            {
                fixed (byte* bufferPtr = buffer)
                {
                    return this.Read(bufferPtr, buffer.Length);
                }
            }
        }

        public unsafe int Read(void* buffer, int length)
        {
            NtStatus status;
            IoStatusBlock isb;

            if ((status = Win32.NtReadFile(
                this,
                IntPtr.Zero,
                null,
                IntPtr.Zero,
                out isb,
                new IntPtr(buffer),
                length,
                IntPtr.Zero,
                IntPtr.Zero
                )) >= NtStatus.Error)
                Win32.ThrowLastError(status);

            return isb.Information.ToInt32();
        }

        public void SetIoCompletion(IoCompletionHandle ioCompletionHandle, IntPtr keyContext)
        {
            FileCompletionInformation info = new FileCompletionInformation();

            info.Port = ioCompletionHandle;
            info.Key = keyContext;
            this.SetStruct<FileCompletionInformation>(FileInformationClass.FileCompletionInformation, info);
        }

        private void SetStruct<T>(FileInformationClass infoClass, T info)
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

        /// <summary>
        /// Writes data to the file.
        /// </summary>
        /// <param name="buffer">The data.</param>
        /// <returns>The number of bytes written to the file.</returns>
        public int Write(byte[] buffer)
        {
            unsafe
            {
                fixed (byte* bufferPtr = buffer)
                {
                    return this.Write(bufferPtr, buffer.Length);
                }
            }
        }

        /// <summary>
        /// Writes data to the file.
        /// </summary>
        /// <param name="buffer">The data.</param>
        /// <param name="length">The number of bytes to write.</param>
        /// <returns>The number of bytes written to the file.</returns>
        public unsafe int Write(void* buffer, int length)
        {
            NtStatus status;
            IoStatusBlock isb;

            if ((status = Win32.NtWriteFile(
                this,
                IntPtr.Zero,
                null,
                IntPtr.Zero,
                out isb,
                new IntPtr(buffer),
                length,
                IntPtr.Zero,
                IntPtr.Zero
                )) >= NtStatus.Error)
                Win32.ThrowLastError(status);

            return isb.Information.ToInt32();
        }
    }

    public sealed class AsyncIoContext : BaseObject, ISynchronizable
    {
        private EventHandle _eventHandle;
        private FileHandle _fileHandle;
        private MemoryAlloc _isb;
        private bool _canceled = false;
        private bool _completedSynchronously = false;
        private bool _started = false;

        private List<BaseObject> _keepAliveList = new List<BaseObject>();
        private object _tag;

        public AsyncIoContext(FileHandle fileHandle)
        {
            _eventHandle = EventHandle.Create(EventAccess.All, EventType.NotificationEvent, false);
            _fileHandle = fileHandle;
            _isb = new MemoryAlloc(Marshal.SizeOf(typeof(IoStatusBlock)));

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

        public bool Canceled
        {
            get { return _canceled; }
        }

        public bool Completed
        {
            get { return _eventHandle.GetBasicInformation().EventState != 0; }
        }

        public bool CompletedSynchronously
        {
            get { return _completedSynchronously; }
            internal set { _completedSynchronously = value; }
        }

        internal EventHandle EventHandle
        {
            get { return _eventHandle; }
        }

        public FileHandle FileHandle
        {
            get { return _fileHandle; }
        }

        public bool Started
        {
            get { return _started; }
        }

        public IoStatusBlock Status
        {
            get
            {
                if (!this.Completed)
                    throw new InvalidOperationException("The I/O operation has not yet completed.");

                return _isb.ReadStruct<IoStatusBlock>();
            }
        }

        internal MemoryRegion StatusMemory
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

            _fileHandle.CancelIo(_isb);
            _canceled = true;
            _eventHandle.Set();

            this.ClearKeepAlive();
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

        internal void NotifyStarted()
        {
            _started = true;
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
            int attributes
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

        public int Attributes { get; private set; }
    }
}
