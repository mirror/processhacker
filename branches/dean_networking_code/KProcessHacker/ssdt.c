/*
 * Process Hacker Driver - 
 *   SSDT code
 * 
 * 
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

#include "ssdt.h"

__declspec(dllimport) SSDT KeServiceDescriptorTable;
MDL *ssdtMdl;
PVOID *mappedSsdtCallTable;

NTSTATUS SsdtInit()
{
    ssdtMdl = MmCreateMdl(NULL, 
        KeServiceDescriptorTable.ServiceTableBase, KeServiceDescriptorTable.NumberOfServices * 4);
    
    if (ssdtMdl == NULL)
        return STATUS_UNSUCCESSFUL;
    
    MmBuildMdlForNonPagedPool(ssdtMdl);
    ssdtMdl->MdlFlags |= MDL_MAPPED_TO_SYSTEM_VA;
    mappedSsdtCallTable = MmMapLockedPages(ssdtMdl, KernelMode);
    
    return STATUS_SUCCESS;
}

void SsdtDeinit()
{
    if (ssdtMdl != NULL)
    {
        MmUnmapLockedPages(mappedSsdtCallTable, ssdtMdl);
        IoFreeMdl(ssdtMdl);
    }
}

PVOID SsdtGetEntry(PVOID zwFunction)
{
    return SYSCALL_NT(zwFunction);
}

PVOID SsdtModifyEntry(PVOID zwFunction, PVOID ntFunction)
{
    PVOID oldValue = SYSCALL_NT(zwFunction);
    
    InterlockedExchange(&mappedSsdtCallTable[SYSCALL_INDEX(zwFunction)], ntFunction);
    
    return oldValue;
}
