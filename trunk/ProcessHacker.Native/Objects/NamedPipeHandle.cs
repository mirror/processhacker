/*
 * Process Hacker - 
 *   named pipe handle
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
    /// Represents a handle to a named pipe.
    /// </summary>
    public sealed class NamedPipeHandle : FileHandle
    {
        public static readonly int FsCtlAssignEvent = Win32.CtlCode(DeviceType.NamedPipe, 0, DeviceControlMethod.Buffered, DeviceControlAccess.Any);
        public static readonly int FsCtlDisconnect = Win32.CtlCode(DeviceType.NamedPipe, 1, DeviceControlMethod.Buffered, DeviceControlAccess.Any);
        public static readonly int FsCtlListen = Win32.CtlCode(DeviceType.NamedPipe, 2, DeviceControlMethod.Buffered, DeviceControlAccess.Any);
        public static readonly int FsCtlPeek = Win32.CtlCode(DeviceType.NamedPipe, 3, DeviceControlMethod.Buffered, DeviceControlAccess.Read);
        public static readonly int FsCtlQueryEvent = Win32.CtlCode(DeviceType.NamedPipe, 4, DeviceControlMethod.Buffered, DeviceControlAccess.Any);
        public static readonly int FsCtlTransceive = Win32.CtlCode(DeviceType.NamedPipe, 5, DeviceControlMethod.Neither, DeviceControlAccess.Read | DeviceControlAccess.Write);
        public static readonly int FsCtlWait = Win32.CtlCode(DeviceType.NamedPipe, 6, DeviceControlMethod.Buffered, DeviceControlAccess.Any);
        public static readonly int FsCtlImpersonate = Win32.CtlCode(DeviceType.NamedPipe, 7, DeviceControlMethod.Buffered, DeviceControlAccess.Any);
        public static readonly int FsCtlSetClientProcess = Win32.CtlCode(DeviceType.NamedPipe, 8, DeviceControlMethod.Buffered, DeviceControlAccess.Any);
        public static readonly int FsCtlQueryClientProcess = Win32.CtlCode(DeviceType.NamedPipe, 9, DeviceControlMethod.Buffered, DeviceControlAccess.Any);

        public static NamedPipeHandle Create(
            FileAccess access,
            string fileName,
            PipeType type,
            int maximumInstances,
            long defaultTimeout
            )
        {
            return Create(
                access,
                fileName,
                ObjectFlags.CaseInsensitive,
                null,
                FileShareMode.ReadWrite,
                FileCreationDisposition.OpenIf,
                0,
                type,
                type,
                PipeCompletionMode.Queue,
                maximumInstances,
                0,
                0,
                defaultTimeout
                );
        }

        public static NamedPipeHandle Create(
            FileAccess access,
            string fileName,
            ObjectFlags objectFlags,
            FileHandle rootDirectory,
            FileShareMode shareMode,
            FileCreationDisposition creationDisposition,
            FileCreateOptions createOptions,
            PipeType type,
            PipeType readMode,
            PipeCompletionMode completionMode,
            int maximumInstances,
            int inboundQuota,
            int outboundQuota,
            long defaultTimeout
            )
        {
            NtStatus status;
            ObjectAttributes oa = new ObjectAttributes(fileName, objectFlags, rootDirectory);
            IoStatusBlock isb;
            IntPtr handle;

            try
            {
                if ((status = Win32.NtCreateNamedPipeFile(
                    out handle,
                    access,
                    ref oa,
                    out isb,
                    shareMode,
                    creationDisposition,
                    createOptions,
                    type,
                    readMode,
                    completionMode,
                    maximumInstances,
                    inboundQuota,
                    outboundQuota,
                    ref defaultTimeout
                    )) >= NtStatus.Error)
                    Win32.ThrowLastError(status);
            }
            finally
            {
                oa.Dispose();
            }

            return new NamedPipeHandle(handle, true);
        }

        /// <summary>
        /// Waits for an instance of the specified named pipe to 
        /// become available for connection.
        /// </summary>
        /// <param name="name">The name of the named pipe.</param>
        /// <param name="timeout">
        /// The timeout, in milliseconds. Use zero for the default timeout interval 
        /// and -1 for an infinite timeout.
        /// </param>
        /// <returns>
        /// True if an instance of the pipe was available before the timeout 
        /// interval elapsed, otherwise false.
        /// </returns>
        public static bool Wait(string name, int timeout)
        {
            return Win32.WaitNamedPipe(name, timeout);
        }

        private NamedPipeHandle(IntPtr handle, bool owned)
            : base(handle, owned)
        { }

        public AsyncIoContext BeginListen()
        {
            return this.BeginFsControl(FsCtlListen, null, null);
        }

        public bool EndListen(AsyncIoContext asyncContext)
        {
            asyncContext.Wait();
            asyncContext.NotifyEnd();

            if (asyncContext.Status == NtStatus.PipeConnected)
                return true;

            if (asyncContext.StatusBlock.Status >= NtStatus.Error)
                Win32.ThrowLastError(asyncContext.StatusBlock.Status);

            return false;
        }

        public void Disconnect()
        {
            this.FsControl(FsCtlDisconnect, IntPtr.Zero, 0, IntPtr.Zero, 0);
        }

        public bool Listen()
        {
            NtStatus status;
            int returnLength;

            status = this.FsControl(FsCtlListen, IntPtr.Zero, 0, IntPtr.Zero, 0, out returnLength);

            if (status == NtStatus.PipeConnected)
                return true;

            if (status >= NtStatus.Error)
                Win32.ThrowLastError(status);

            return false;
        }
    }
}
