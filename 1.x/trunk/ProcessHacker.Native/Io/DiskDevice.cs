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
    public static class DiskDevice
    {
        public enum DiskCacheState : int
        {
            Normal,
            WriteThroughNotSupported,
            ModifyUnsuccessful
        }

        // Input for IoCtlGetCacheSetting, output for IoCtlSetCacheSetting
        [StructLayout(LayoutKind.Sequential)]
        public struct DiskCacheSetting
        {
            public int Version;
            public DiskCacheState State;
            [MarshalAs(UnmanagedType.I1)]
            public bool IsPowerProtected;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PartitionInformation
        {
            public long StartingOffset;
            public long PartitionLength;
            public int HiddenSectors;
            public int PartitionNumber;
            public PartitionType PartitionType;
            [MarshalAs(UnmanagedType.I1)]
            public bool BootIndicator;
            [MarshalAs(UnmanagedType.I1)]
            public bool RecognizedPartition;
            [MarshalAs(UnmanagedType.I1)]
            public bool RewritePartition;
        }

        public static int IoCtlGetCacheSetting = Win32.CtlCode(DeviceType.Disk, 0x0038, DeviceControlMethod.Buffered, DeviceControlAccess.Read);
        public static int IoCtlSetCacheSetting = Win32.CtlCode(DeviceType.Disk, 0x0039, DeviceControlMethod.Buffered, DeviceControlAccess.Read | DeviceControlAccess.Write);

        public static DiskCacheState GetCacheSetting(string fileName)
        {
            bool isPowerProtected;

            return GetCacheSetting(fileName, out isPowerProtected);
        }

        public static DiskCacheState GetCacheSetting(FileHandle fileHandle)
        {
            bool isPowerProtected;

            return GetCacheSetting(fileHandle, out isPowerProtected);
        }

        public static DiskCacheState GetCacheSetting(string fileName, out bool isPowerProtected)
        {
            using (var fhandle = StorageDevice.OpenStorageDevice(fileName, FileAccess.GenericRead))
                return GetCacheSetting(fhandle, out isPowerProtected);
        }

        public static DiskCacheState GetCacheSetting(FileHandle fileHandle, out bool isPowerProtected)
        {
            unsafe
            {
                DiskCacheSetting diskCacheSetting;

                fileHandle.IoControl(
                    IoCtlGetCacheSetting,
                    null,
                    0,
                    &diskCacheSetting,
                    Marshal.SizeOf(typeof(DiskCacheSetting))
                    );

                isPowerProtected = diskCacheSetting.IsPowerProtected;

                return diskCacheSetting.State;
            }
        }

        public static void SetCacheSetting(string fileName, DiskCacheState state, bool isPowerProtected)
        {
            using (var fhandle = StorageDevice.OpenStorageDevice(fileName, FileAccess.GenericRead | FileAccess.GenericWrite))
                SetCacheSetting(fhandle, state, isPowerProtected);
        }

        public static void SetCacheSetting(FileHandle fileHandle, DiskCacheState state, bool isPowerProtected)
        {
            unsafe
            {
                DiskCacheSetting diskCacheSetting;

                diskCacheSetting.Version = Marshal.SizeOf(typeof(DiskCacheSetting));
                diskCacheSetting.State = state;
                diskCacheSetting.IsPowerProtected = isPowerProtected;

                fileHandle.IoControl(
                    IoCtlSetCacheSetting,
                    &diskCacheSetting,
                    Marshal.SizeOf(typeof(DiskCacheSetting)),
                    null,
                    0
                    );
            }
        }
    }
}
