using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessHacker.Native.Image
{
    public enum ImageDataEntry : int
    {
        Export = 0,
        Import = 1,
        Resource = 2,
        Exception = 3,
        Security = 4,
        BaseRelocation = 5,
        Debug = 6,
        Copyright = 7,
        Architecture = 7,
        GlobalPtr = 8,
        Tls = 9,
        LoadConfig = 10,
        BoundImport = 11,
        Iat = 12,
        DelayImport = 13,
        ComDescriptor = 14
    }
}
