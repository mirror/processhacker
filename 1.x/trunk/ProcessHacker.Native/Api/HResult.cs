/*
 * Process Hacker - 
 *   HResult values
 *
 * Copyright (C) 2009 wj32
 * Copyright (C) 2009 dmex
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
    /*//////////////////////////
    //                        //
    //     COM Error Codes    //
    //                        //
    ////////////////////////////
    //
    // The return value of COM functions and methods is an HRESULT.
    // This is not a handle to anything, but is merely a 32-bit value
    // with several fields encoded in the value. The parts of an
    // HRESULT are shown below.
    //
    // Many of the macros and functions below were orginally defined to
    // operate on SCODEs.  SCODEs are no longer used.  The macros are
    // still present for compatibility and easy porting of Win16 code.
    // Newly written code should use the HRESULT macros and functions.
    //
    //  HRESULTs are 32 bit values layed out as follows:
    //
    //   3 3 2 2 2 2 2 2 2 2 2 2 1 1 1 1 1 1 1 1 1 1
    //   1 0 9 8 7 6 5 4 3 2 1 0 9 8 7 6 5 4 3 2 1 0 9 8 7 6 5 4 3 2 1 0
    //  +-+-+-+-+-+---------------------+-------------------------------+
    //  | S | R | C | N | r |      Facility         |               Code                |
    //  +-+-+-+-+-+---------------------+-------------------------------+
    //
    //  where
    //
    //      S - Severity - indicates success/fail
    //
    //          0 - Success
    //          1 - Fail (COERROR)
    //
    //      R - reserved portion of the facility code, corresponds to NT's
    //              second severity bit.
    //
    //      C - reserved portion of the facility code, corresponds to NT's
    //              C field.
    //
    //      N - reserved portion of the facility code. Used to indicate a
    //              mapped NT status value.
    //
    //      r - reserved portion of the facility code. Reserved for internal
    //              use. Used to indicate HRESULT values that are not status
    //              values, but are instead message ids for display strings.
    //
    //      Facility - is the facility code
    //
    //      Code - is the facility's status code
    */

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
            //Return != OK because there are come errors with lower values than HResult.False
            return result != HResult.OK;
        }

        public static void ThrowIf(this HResult result)
        {
            if (result.IsError())
                throw Marshal.GetExceptionForHR((int)result);
        }
    }
}
