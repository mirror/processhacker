/*
 * Process Hacker - 
 *   HResult values
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
using System.Runtime.InteropServices;

namespace ProcessHacker.Native.Api
{
    public enum HResult : uint
    {
        False = 0x0001,
        OK = 0x0000,
        Cancelled = 1223,

        Error = 0x80000000,
        NoInterface = 0x80004002,
        Fail = 0x80004005,
        TypeElementNotFound = 0x8002802b,
        NoObject = 0x800401e5,
        OutOfMemory = 0x8007000e,
        InvalidArgument = 0x80070057,
        ResourceInUse = 0x800700aa,
        ElementNotFound = 0x80070490
    }

    public static class HResultExtensions
    {
        public static bool IsError(this HResult result)
        {
            return result >= HResult.Error;
        }

        public static void ThrowIf(this HResult result)
        {
            if (result.IsError())
                throw Marshal.GetExceptionForHR((int)result);
        }
    }
}
