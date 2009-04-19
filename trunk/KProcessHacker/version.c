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
    ULONG majorVersion, minorVersion, servicePack;
    
    RtlWindowsVersion.dwOSVersionInfoSize = sizeof(RtlWindowsVersion);
    status = RtlGetVersion((PRTL_OSVERSIONINFOW)&RtlWindowsVersion);
    
    if (!NT_SUCCESS(status))
        return status;
    
    majorVersion = RtlWindowsVersion.dwMajorVersion;
    minorVersion = RtlWindowsVersion.dwMinorVersion;
    servicePack = RtlWindowsVersion.wServicePackMajor;
    
    /* Windows XP */
    if (majorVersion == 5 && minorVersion == 1)
    {
        WindowsVersion = WINDOWS_XP;
        ProcessAllAccess = STANDARD_RIGHTS_REQUIRED | SYNCHRONIZE | 0xfff;
        ThreadAllAccess = STANDARD_RIGHTS_REQUIRED | SYNCHRONIZE | 0x3ff;
        
        OffEtClientId = 0x1ec;
        OffEtStartAddress = 0x224;
        OffEtWin32StartAddress = 0x228;
        OffEpJob = 0x134;
        OffEpObjectTable = 0xc4;
        OffEpProtectedProcessOff = 0;
        OffEpProtectedProcessBit = 0;
        OffEpRundownProtect = 0x80;
        OffOtiGenericMapping = 0x60 + 0x8;
        
        /* Windows XP SP0 and 1 are not supported */
        if (servicePack == 0)
            return STATUS_NOT_SUPPORTED;
        else if (servicePack == 1)
            return STATUS_NOT_SUPPORTED;
        else if (servicePack == 2)
            ;
        else if (servicePack == 3)
            ;
        else
            return STATUS_NOT_SUPPORTED;
        
        dprintf("Initialized version-specific data for Windows XP SP%d\n", servicePack);
    }
    /* Windows Server 2003 */
    else if (majorVersion == 5 && minorVersion == 2)
    {
        WindowsVersion = WINDOWS_SERVER_2003;
        
        /* Not supported yet */
        return STATUS_NOT_SUPPORTED;
    }
    /* Windows Vista, Windows Server 2008 */
    else if (majorVersion == 6 && minorVersion == 0)
    {
        WindowsVersion = WINDOWS_VISTA;
        ProcessAllAccess = STANDARD_RIGHTS_REQUIRED | SYNCHRONIZE | 0xffff;
        ThreadAllAccess = STANDARD_RIGHTS_REQUIRED | SYNCHRONIZE | 0xffff;
        OffEtClientId = 0x20c;
        OffEtStartAddress = 0x1f8;
        OffEtWin32StartAddress = 0x240;
        OffEpJob = 0x10c;
        OffEpObjectTable = 0xdc;
        OffEpProtectedProcessOff = 0x224;
        OffEpProtectedProcessBit = 0xb;
        OffEpRundownProtect = 0x98;
        
        /* SP0 */
        if (servicePack == 0)
        {
            OffOtiGenericMapping = 0x60 + 0xc;
        }
        /* SP1 */
        else if (servicePack == 1)
        {
            /* They got rid of the Mutex (an ERESOURCE) */
            OffOtiGenericMapping = 0x28 + 0xc;
        }
        else
        {
            return STATUS_NOT_SUPPORTED;
        }
        
        dprintf("Initialized version-specific data for Windows Vista SP%d/Windows Server 2008\n", servicePack);
    }
    /* Windows 7 */
    else if (majorVersion == 6 && minorVersion == 1)
    {
        WindowsVersion = WINDOWS_7;
        ProcessAllAccess = STANDARD_RIGHTS_REQUIRED | SYNCHRONIZE | 0xffff;
        ThreadAllAccess = STANDARD_RIGHTS_REQUIRED | SYNCHRONIZE | 0xffff;
        OffEtClientId = 0x22c;
        OffEtStartAddress = 0x218;
        OffEtWin32StartAddress = 0x260;
        OffEpJob = 0x124;
        OffEpObjectTable = 0xf4;
        OffEpProtectedProcessOff = 0x25c;
        OffEpProtectedProcessBit = 0xb;
        OffEpRundownProtect = 0xb0;
        OffOtiGenericMapping = 0x28 + 0xc;
        
        /* SP0 */
        if (servicePack == 0)
            ;
        else
            return STATUS_NOT_SUPPORTED;
        
        dprintf("Initialized version-specific data for Windows 7 SP%d\n", servicePack);
    }
    else
    {
        return STATUS_NOT_SUPPORTED;
    }
    
    return status;
}
