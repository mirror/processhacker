/*
 * Process Hacker Driver - 
 *   Windows version-specific data
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

#define KPH_VERSION_PRIVATE
#include "include/version.h"
#include "include/debug.h"

NTSTATUS KvInit()
{
    NTSTATUS status = STATUS_SUCCESS;
    
    RtlWindowsVersion.dwOSVersionInfoSize = sizeof(RtlWindowsVersion);
    status = RtlGetVersion(&RtlWindowsVersion);
    
    if (!NT_SUCCESS(status))
        return status;
    
    /* Windows XP */
    if (RtlWindowsVersion.dwMajorVersion == 5 && RtlWindowsVersion.dwMinorVersion == 1)
    {
        WindowsVersion = WINDOWS_XP;
        ProcessAllAccess = STANDARD_RIGHTS_REQUIRED | SYNCHRONIZE | 0xfff;
        ThreadAllAccess = STANDARD_RIGHTS_REQUIRED | SYNCHRONIZE | 0x3ff;
        OffEtClientId = 0x1ec;
        OffEtStartAddress = 0x224;
        OffEtWin32StartAddress = 0x228;
        OffEpJob = 0x134;
        OffEpObjectTable = 0xc4;
        OffEpRundownProtect = 0x80;
        OffOtiGenericMapping = 0x68;
        dprintf("Initialized version-specific data for Windows XP\n");
    }
    /* Windows Vista */
    else if (RtlWindowsVersion.dwMajorVersion == 6 && RtlWindowsVersion.dwMinorVersion == 0)
    {
        WindowsVersion = WINDOWS_VISTA;
        ProcessAllAccess = STANDARD_RIGHTS_REQUIRED | SYNCHRONIZE | 0xffff;
        ThreadAllAccess = STANDARD_RIGHTS_REQUIRED | SYNCHRONIZE | 0xffff;
        OffEtClientId = 0x20c;
        OffEtStartAddress = 0x1f8;
        OffEtWin32StartAddress = 0x240;
        OffEpJob = 0x10c;
        OffEpObjectTable = 0xdc;
        OffEpRundownProtect = 0x98;
        OffOtiGenericMapping = 0x34;
        dprintf("Initialized version-specific data for Windows Vista\n");
    }
    else
    {
        return STATUS_NOT_SUPPORTED;
    }
    
    return status;
}
