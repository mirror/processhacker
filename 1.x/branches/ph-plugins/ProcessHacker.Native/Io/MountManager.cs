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
        public struct MountMgrCreatePointInput
        {
            public ushort SymbolicLinkNameOffset;
            public ushort SymbolicLinkNameLength;
            public ushort DeviceNameOffset;
            public ushort DeviceNameLength;
        }

        // Input for IoCtlDeletePoints, IoCtlQueryPoints and IoCtlDeletePointsDbOnly
        public struct MountMgrMountPoint
        {
            public int SymbolicLinkNameOffset;
            public ushort SymbolicLinkNameLength;
            public int UniqueIdOffset;
            public ushort UniqueIdLength;
            public int DeviceNameOffset;
            public ushort DeviceNameLength;
        }

        // Output for IoCtlDeletePoints, IoCtlQueryPoints and IoCtlDeletePointsDbOnly
        public struct MountMgrMountPoints
        {
            public static int MountPointsOffset =
                Marshal.OffsetOf(typeof(MountMgrMountPoints), "MountPoints").ToInt32();

            public int Size;
            public int NumberOfMountPoints;
            public MountMgrMountPoint MountPoints;
        }

        // Input for IoCtlNextDriveLetter
        public struct MountMgrDriveLetterTarget
        {
            public static int DeviceNameOffset =
                Marshal.OffsetOf(typeof(MountMgrDriveLetterTarget), "DeviceName").ToInt32();

            public ushort DeviceNameLength;
            public short DeviceName;
        }

        // Output for IoCtlNextDriveLetter
        public struct MountMgrDriveLetterInformation
        {
            [MarshalAs(UnmanagedType.I1)]
            public bool DriveLetterWasAssigned;
            [MarshalAs(UnmanagedType.I1)]
            public char CurrentDriveLetter;
        }

        // Input for IoCtlVolumeMountPointCreated and IoCtlVolumeMountPointDeleted
        public struct MountMgrVolumeMountPoint
        {
            public ushort SourceVolumeNameOffset;
            public ushort SourceVolumeNameLength;
            public ushort TargetVolumeNameOffset;
            public ushort TargetVolumeNameLength;
        }

        // Input, output for IoCtlChangeNotify
        public struct MountMgrChangeNotifyInfo
        {
            public int EpicNumber;
        }

        // Input for IoCtlKeepLinksWhenOffline, IoCtlVolumeArrivalNotification,
        // IoCtlQueryDosVolumePath, IoCtlQueryDosVolumePaths
        public struct MountMgrTargetName
        {
            public static int DeviceNameOffset =
                Marshal.OffsetOf(typeof(MountMgrTargetName), "DeviceName").ToInt32();

            public ushort DeviceNameLength;
            public short DeviceName;
        }

        // Output for IoCtlQueryDosVolumePath, IoCtlQueryDosVolumePaths
        public struct MountMgrVolumePaths
        {
            public static int MultiSzOffset =
                Marshal.OffsetOf(typeof(MountMgrVolumePaths), "MultiSz").ToInt32();

            public int MultiSzLength;
            public short MultiSz;
        }

        // Output for IoCtlQueryDeviceName
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
