using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessHacker.Native.Api
{
    public static partial class Win32
    {
        public const int SamMaximumLookupCount = 100;
        public const int SamMaximumLookupLength = 32000;
        public const int SamMaximumPasswordLength = 256;

        public const int SamDaysPerWeek = 7;
        public const int SamHoursPerWeek = 24 * SamDaysPerWeek;
        public const int SamMinutesPerWeek = 60 * SamHoursPerWeek;
        public const int SamUnitsPerWeek = 10080;
    }
}
