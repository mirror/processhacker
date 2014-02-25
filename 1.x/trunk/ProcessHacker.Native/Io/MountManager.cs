/*
 * Process Hacker - 
 *   mount point manager API
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
using System.Text;
using ProcessHacker.Native;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;

namespace ProcessHacker.Native.Io
{
    public static class MountManager
    {
        // Input for IoCtlCreatePoint
        [StructLayout(LayoutKind.Sequential)]
        public struct MountMgrCreatePointInput
        {
            public ushort SymbolicLinkNameOffset;
            public ushort SymbolicLinkNameLength;
            public ushort DeviceNameOffset;
            public ushort DeviceNameLength;
        }

        // Input for IoCtlDeletePoints, IoCtlQueryPoints and IoCtlDeletePointsDbOnly
        [StructLayout(LayoutKind.Sequential)]
        public struct MountMgrMountPoint
        {
            public static readonly int Size = Marshal.SizeOf(typeof(MountMgrMountPoint));

            public int SymbolicLinkNameOffset;
            public ushort SymbolicLinkNameLength;
            public int UniqueIdOffset;
            public ushort UniqueIdLength;
            public int DeviceNameOffset;
            public ushort DeviceNameLength;
        }

        // Output for IoCtlDeletePoints, IoCtlQueryPoints and IoCtlDeletePointsDbOnly
        [StructLayout(LayoutKind.Sequential)]
        public struct MountMgrMountPoints
        {
            public static int MountPointsOffset =
                Marshal.OffsetOf(typeof(MountMgrMountPoints), "MountPoints").ToInt32();

            public int Size;
            public int NumberOfMountPoints;
            public MountMgrMountPoint MountPoints;
        }

        // Input for IoCtlNextDriveLetter
        [StructLayout(LayoutKind.Sequential)]
        public struct MountMgrDriveLetterTarget
        {
            public static int DeviceNameOffset =
                Marshal.OffsetOf(typeof(MountMgrDriveLetterTarget), "DeviceName").ToInt32();

            public ushort DeviceNameLength;
            public short DeviceName;
        }

        // Output for IoCtlNextDriveLetter
        [StructLayout(LayoutKind.Sequential)]
        public struct MountMgrDriveLetterInformation
        {
            [MarshalAs(UnmanagedType.I1)]
            public bool DriveLetterWasAssigned;
            [MarshalAs(UnmanagedType.I1)]
            public char CurrentDriveLetter;
        }

        // Input for IoCtlVolumeMountPointCreated and IoCtlVolumeMountPointDeleted
        [StructLayout(LayoutKind.Sequential)]
        public struct MountMgrVolumeMountPoint
        {
            public static readonly int Size = Marshal.SizeOf(typeof(MountMgrVolumeMountPoint));

            public ushort SourceVolumeNameOffset;
            public ushort SourceVolumeNameLength;
            public ushort TargetVolumeNameOffset;
            public ushort TargetVolumeNameLength;
        }

        // Input, output for IoCtlChangeNotify
        [StructLayout(LayoutKind.Sequential)]
        public struct MountMgrChangeNotifyInfo
        {
            public int EpicNumber;
        }

        // Input for IoCtlKeepLinksWhenOffline, IoCtlVolumeArrivalNotification,
        // IoCtlQueryDosVolumePath, IoCtlQueryDosVolumePaths
        [StructLayout(LayoutKind.Sequential)]
        public struct MountMgrTargetName
        {
            public static int DeviceNameOffset =
                Marshal.OffsetOf(typeof(MountMgrTargetName), "DeviceName").ToInt32();

            public ushort DeviceNameLength;
            public short DeviceName;
        }

        // Output for IoCtlQueryDosVolumePath, IoCtlQueryDosVolumePaths
        [StructLayout(LayoutKind.Sequential)]
        public struct MountMgrVolumePaths
        {
            public static int MultiSzOffset =
                Marshal.OffsetOf(typeof(MountMgrVolumePaths), "MultiSz").ToInt32();

            public int MultiSzLength;
            public short MultiSz;
        }

        // Output for IoCtlQueryDeviceName
        [StructLayout(LayoutKind.Sequential)]
        public struct MountDevName
        {
            public static int NameOffset =
                Marshal.OffsetOf(typeof(MountDevName), "Name").ToInt32();

            public ushort NameLength;
            public short Name;
        }

        public static readonly int IoCtlCreatePoint = Win32.CtlCode(DeviceType.MountMgr, 0, DeviceControlMethod.Buffered, DeviceControlAccess.Read | DeviceControlAccess.Write);
        public static readonly int IoCtlDeletePoints = Win32.CtlCode(DeviceType.MountMgr, 1, DeviceControlMethod.Buffered, DeviceControlAccess.Read | DeviceControlAccess.Write);
        public static readonly int IoCtlQueryPoints = Win32.CtlCode(DeviceType.MountMgr, 2, DeviceControlMethod.Buffered, DeviceControlAccess.Any);
        public static readonly int IoCtlDeletePointsDbOnly = Win32.CtlCode(DeviceType.MountMgr, 3, DeviceControlMethod.Buffered, DeviceControlAccess.Read | DeviceControlAccess.Write);
        public static readonly int IoCtlNextDriveLetter = Win32.CtlCode(DeviceType.MountMgr, 4, DeviceControlMethod.Buffered, DeviceControlAccess.Read | DeviceControlAccess.Write);
        public static readonly int IoCtlAutoDlAssignments = Win32.CtlCode(DeviceType.MountMgr, 5, DeviceControlMethod.Buffered, DeviceControlAccess.Read | DeviceControlAccess.Write);
        public static readonly int IoCtlVolumeMountPointCreated = Win32.CtlCode(DeviceType.MountMgr, 6, DeviceControlMethod.Buffered, DeviceControlAccess.Read | DeviceControlAccess.Write);
        public static readonly int IoCtlVolumeMountPointDeleted = Win32.CtlCode(DeviceType.MountMgr, 7, DeviceControlMethod.Buffered, DeviceControlAccess.Read | DeviceControlAccess.Write);
        public static readonly int IoCtlChangeNotify = Win32.CtlCode(DeviceType.MountMgr, 8, DeviceControlMethod.Buffered, DeviceControlAccess.Read);
        public static readonly int IoCtlKeepLinksWhenOffline = Win32.CtlCode(DeviceType.MountMgr, 9, DeviceControlMethod.Buffered, DeviceControlAccess.Read | DeviceControlAccess.Write);
        public static readonly int IoCtlCheckUnprocessedVolumes = Win32.CtlCode(DeviceType.MountMgr, 10, DeviceControlMethod.Buffered, DeviceControlAccess.Read);
        public static readonly int IoCtlVolumeArrivalNotification = Win32.CtlCode(DeviceType.MountMgr, 11, DeviceControlMethod.Buffered, DeviceControlAccess.Read);
        public static readonly int IoCtlQueryDosVolumePath = Win32.CtlCode(DeviceType.MountMgr, 12, DeviceControlMethod.Buffered, DeviceControlAccess.Any);
        public static readonly int IoCtlQueryDosVolumePaths = Win32.CtlCode(DeviceType.MountMgr, 13, DeviceControlMethod.Buffered, DeviceControlAccess.Any);
        public static readonly int IoCtlScrubRegistry = Win32.CtlCode(DeviceType.MountMgr, 14, DeviceControlMethod.Buffered, DeviceControlAccess.Read | DeviceControlAccess.Write);
        public static readonly int IoCtlQueryAutoMount = Win32.CtlCode(DeviceType.MountMgr, 15, DeviceControlMethod.Buffered, DeviceControlAccess.Any);
        public static readonly int IoCtlSetAutoMount = Win32.CtlCode(DeviceType.MountMgr, 16, DeviceControlMethod.Buffered, DeviceControlAccess.Read | DeviceControlAccess.Write);

        public static readonly int IoCtlQueryDeviceName = Win32.CtlCode(DeviceType.MountMgrDevice, 2, DeviceControlMethod.Buffered, DeviceControlAccess.Any);

        private static void DeleteSymbolicLink(string path)
        {
            using (var data = new MemoryAlloc(MountMgrMountPoint.Size + path.Length * 2))
            using (var outData = new MemoryAlloc(1600))
            {
                MountMgrMountPoint mountPoint = new MountMgrMountPoint();

                mountPoint.SymbolicLinkNameLength = (ushort)(path.Length * 2);
                mountPoint.SymbolicLinkNameOffset = MountMgrMountPoint.Size;
                data.WriteStruct<MountMgrMountPoint>(mountPoint);
                data.WriteUnicodeString(mountPoint.SymbolicLinkNameOffset, path);

                using (var fhandle = OpenMountManager(FileAccess.GenericRead | FileAccess.GenericWrite))
                {
                    fhandle.IoControl(IoCtlDeletePoints, data.Memory, data.Size, outData.Memory, outData.Size);
                }
            }
        }

        /// <summary>
        /// Gets the device name associated with the specified file name.
        /// </summary>
        /// <param name="fileName">
        /// A file name referring to a DOS drive. For example: "\??\C:" (no 
        /// trailing backslash).
        /// </param>
        /// <returns>The device name associated with the DOS drive.</returns>
        public static string GetDeviceName(string fileName)
        {
            using (var fhandle = new FileHandle(
                fileName,
                FileShareMode.ReadWrite,
                FileCreateOptions.SynchronousIoNonAlert,
                FileAccess.ReadAttributes | (FileAccess)StandardRights.Synchronize
                ))
                return GetDeviceName(fhandle);
        }

        public static string GetDeviceName(FileHandle fhandle)
        {
            using (var data = new MemoryAlloc(600))
            {
                fhandle.IoControl(IoCtlQueryDeviceName, IntPtr.Zero, 0, data, data.Size);

                MountDevName name = data.ReadStruct<MountDevName>();

                return data.ReadUnicodeString(MountDevName.NameOffset, name.NameLength / 2);
            }
        }

        private static string GetReparsePointTarget(FileHandle fhandle)
        {
            using (var data = new MemoryAlloc(FileSystem.MaximumReparseDataBufferSize))
            {
                fhandle.IoControl(FileSystem.FsCtlGetReparsePoint, IntPtr.Zero, 0, data, data.Size);

                FileSystem.ReparseDataBuffer buffer = data.ReadStruct<FileSystem.ReparseDataBuffer>();

                // Make sure it is in fact a mount point.
                if (buffer.ReparseTag != (uint)IoReparseTag.MountPoint)
                    Win32.Throw(NtStatus.InvalidParameter);

                return data.ReadUnicodeString(
                    FileSystem.ReparseDataBuffer.MountPointPathBuffer + buffer.SubstituteNameOffset,
                    buffer.SubstituteNameLength / 2
                    );
            }
        }

        public static string GetMountPointTarget(string fileName)
        {
            // Special cases for DOS drives.

            // "C:", "C:\"
            if (
                (fileName.Length == 2 && fileName[1] == ':') ||
                (fileName.Length == 3 && fileName[1] == ':' && fileName[2] == '\\')
                )
            {
                return GetVolumeName(fileName[0]);
            }

            // "\\.\C:\", "\\?\C:\" (and variants without the trailing backslash"
            if (
                (fileName.Length == 6 || fileName.Length == 7) &&
                fileName[0] == '\\' &&
                fileName[1] == '\\' &&
                (fileName[2] == '.' || fileName[2] == '?') &&
                fileName[3] == '\\' &&
                fileName[5] == ':'
                )
            {
                return GetVolumeName(fileName[4]);
            }

            // "\??\C:"
            if (
                fileName.Length == 6 &&
                fileName[0] == '\\' &&
                fileName[1] == '?' &&
                fileName[2] == '?' &&
                fileName[3] == '\\' &&
                fileName[5] == ':'
                )
            {
                return GetVolumeName(fileName[4]);
            }

            // Query the reparse point.
            using (var fhandle = new FileHandle(
                fileName,
                FileShareMode.ReadWrite,
                FileCreateOptions.OpenReparsePoint | FileCreateOptions.SynchronousIoNonAlert,
                FileAccess.GenericRead
                ))
            {
                return GetReparsePointTarget(fhandle);
            }
        }

        public static string GetVolumeName(char driveLetter)
        {
            return GetVolumeName(FileUtils.GetPathForDosDrive(driveLetter));
        }

        public static string GetVolumeName(string deviceName)
        {
            using (var data = new MemoryAlloc(MountMgrMountPoint.Size + deviceName.Length * 2))
            {
                MountMgrMountPoint mountPoint = new MountMgrMountPoint();

                mountPoint.DeviceNameLength = (ushort)(deviceName.Length * 2);
                mountPoint.DeviceNameOffset = MountMgrMountPoint.Size;
                data.WriteStruct<MountMgrMountPoint>(mountPoint);
                data.WriteUnicodeString(mountPoint.DeviceNameOffset, deviceName);

                using (var fhandle = OpenMountManager((FileAccess)StandardRights.Synchronize))
                {
                    NtStatus status;
                    int retLength;

                    using (var outData = new MemoryAlloc(0x100))
                    {
                        while (true)
                        {
                            status = fhandle.IoControl(
                                IoCtlQueryPoints,
                                data.Memory,
                                data.Size,
                                outData.Memory,
                                outData.Size,
                                out retLength
                                );

                            if (status == NtStatus.BufferOverflow)
                            {
                                outData.ResizeNew(Marshal.ReadInt32(outData.Memory)); // read Size field
                                continue;
                            }
                            else
                            {
                                break;
                            }
                        }

                        if (status >= NtStatus.Error)
                            Win32.Throw(status);

                        MountMgrMountPoints mountPoints = outData.ReadStruct<MountMgrMountPoints>();

                        // Go through the mount points given and return the first symbolic link that seems 
                        // to be a volume name.
                        for (int i = 0; i < mountPoints.NumberOfMountPoints; i++)
                        {
                            MountMgrMountPoint mp = outData.ReadStruct<MountMgrMountPoint>(
                                MountMgrMountPoints.MountPointsOffset,
                                i
                                );
                            string symLinkName;

                            symLinkName = Marshal.PtrToStringUni(
                                outData.Memory.Increment(mp.SymbolicLinkNameOffset),
                                mp.SymbolicLinkNameLength / 2
                                );

                            if (IsVolumePath(symLinkName))
                                return symLinkName;
                        }

                        return null;
                    }
                }
            }
        }

        public static bool IsDriveLetterPath(string path)
        {
            if (
                path.Length == 14 &&
                path.StartsWith(@"\DosDevices\") &&
                path[12] >= 'A' && path[12] <= 'Z' &&
                path[13] == ':'
                )
                return true;
            else
                return false;
        }

        public static bool IsVolumePath(string path)
        {
            if (
                (path.Length == 48 || (path.Length == 49 && path[48] == '\\')) &&
                (path.StartsWith(@"\??\Volume") || path.StartsWith(@"\\?\Volume")) &&
                path[10] == '{' &&
                path[19] == '-' &&
                path[24] == '-' &&
                path[29] == '-' &&
                path[34] == '-' &&
                path[47] == '}'
                )
                return true;
            else
                return false;
        }

        private static void Notify(bool created, string sourceVolumeName, string targetVolumeName)
        {
            using (var data = new MemoryAlloc(
                MountMgrVolumeMountPoint.Size +
                sourceVolumeName.Length * 2 +
                targetVolumeName.Length * 2
                ))
            {
                MountMgrVolumeMountPoint mountPoint = new MountMgrVolumeMountPoint();

                mountPoint.SourceVolumeNameLength = (ushort)(sourceVolumeName.Length * 2);
                mountPoint.SourceVolumeNameOffset = (ushort)MountMgrVolumeMountPoint.Size;
                mountPoint.TargetVolumeNameLength = (ushort)(targetVolumeName.Length * 2);
                mountPoint.TargetVolumeNameOffset = 
                    (ushort)(mountPoint.SourceVolumeNameOffset + mountPoint.SourceVolumeNameLength);

                data.WriteStruct<MountMgrVolumeMountPoint>(mountPoint);
                data.WriteUnicodeString(mountPoint.SourceVolumeNameOffset, sourceVolumeName);
                data.WriteUnicodeString(mountPoint.TargetVolumeNameOffset, targetVolumeName);

                using (var fhandle = OpenMountManager(FileAccess.GenericRead | FileAccess.GenericWrite))
                {
                    fhandle.IoControl(
                        created ? IoCtlVolumeMountPointCreated : IoCtlVolumeMountPointDeleted,
                        data.Memory,
                        data.Size,
                        IntPtr.Zero,
                        0
                        );
                }
            }
        }

        private static FileHandle OpenMountManager(FileAccess access)
        {
            return new FileHandle(
                Win32.MountMgrDeviceName,
                FileShareMode.ReadWrite,
                access
                );
        }
    }
}
