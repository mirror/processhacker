using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;

namespace ProcessHacker.Native.Io
{
    public static class BeepDevice
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct BeepSetParameters
        {
            public static readonly int SizeOf;

            public int Frequency;
            public int Duration;

            static BeepSetParameters()
            {
                SizeOf = Marshal.SizeOf(typeof(BeepSetParameters));
            }
        }

        public const int BeepFrequencyMinimum = 0x25;
        public const int BeepFrequencyMaximum = 0x7fff;

        public static readonly int IoCtlSet = Win32.CtlCode(DeviceType.Beep, 0, DeviceControlMethod.Buffered, DeviceControlAccess.Any);

        public static unsafe void Beep(int frequency, int duration)
        {
            BeepSetParameters p;

            p.Frequency = frequency;
            p.Duration = duration;

            using (var fhandle = OpenBeepDevice(FileAccess.GenericRead))
                fhandle.IoControl(IoCtlSet, &p, BeepSetParameters.SizeOf, null, 0);
        }

        private static FileHandle OpenBeepDevice(FileAccess access)
        {
            return new FileHandle(
                Win32.BeepDeviceName,
                FileShareMode.ReadWrite,
                access
                );
        }
    }
}
